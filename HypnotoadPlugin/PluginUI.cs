/*
 * Copyright(c) 2023 GiR-Zippo, Meowchestra 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Timers;
using H.Pipes.Args;
using HypnotoadPlugin.Offsets;
using ImGuiNET;

namespace HypnotoadPlugin;

public class Message
{
    public MessageType msgType { get; init; } = MessageType.None;
    public int msgChannel { get; init; }
    public string message { get; init; } = "";
}

// It is good to have this be disposable in general, in case you ever need it
// to do any cleanup
class PluginUI : IDisposable
{
    private Timer _reconnectTimer { get; set; } = new();
    private Queue<Message> qt = new();
    private Configuration configuration;

    // this extra bool exists for ImGui, since you can't ref a property
    private bool visible;
    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    public bool ManuallyDisconnected { get; set; }

    // passing in the image here just for simplicity
    public PluginUI(Configuration configuration)
    {
        this.configuration          =  configuration;

        Pipe.Initialize();
        Pipe.Client.Connected       += pipeClient_Connected;
        Pipe.Client.MessageReceived += pipeClient_MessageReceived;
        Pipe.Client.Disconnected    += pipeClient_Disconnected;
        _reconnectTimer.Elapsed     += reconnectTimer_Elapsed;

        _reconnectTimer.Interval    =  2000;
        _reconnectTimer.Enabled     =  configuration.Autoconnect;

        Visible = false;
    }

    private void pipeClient_Connected(object sender, ConnectionEventArgs<Message> e)
    {
        Pipe.Client.WriteAsync(new Message
        {
            msgType    = MessageType.Handshake,
            msgChannel = 0,
            message    = Environment.ProcessId.ToString()
        });


        Pipe.Client.WriteAsync(new Message
        {
            msgType = MessageType.Version,
            msgChannel = 0,
            message = Environment.ProcessId + ":" + Assembly.GetExecutingAssembly().GetName().Version.ToString()
        });

        Pipe.Write(MessageType.SetGfx, 0, GameSettings.AgentConfigSystem.CheckLowSettings());
        Pipe.Write(MessageType.MasterSoundState, 0, GameSettings.AgentConfigSystem.GetMasterSoundEnable());
        Pipe.Write(MessageType.MasterVolume, 0, GameSettings.AgentConfigSystem.GetMasterSoundVolume());

        Collector.Instance.UpdateClientStats();
    }

    private void pipeClient_Disconnected(object sender, ConnectionEventArgs<Message> e)
    {
        if (!configuration.Autoconnect)
            return;

        _reconnectTimer.Interval = 2000;
        _reconnectTimer.Enabled  = configuration.Autoconnect;
    }

    private void reconnectTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (ManuallyDisconnected)
            return;

        if (Pipe.Client.IsConnected)
        {
            _reconnectTimer.Enabled = false;
            return;
        }

        if (!Pipe.Client.IsConnecting)
        {
            Pipe.Client.ConnectAsync();
        }
    }

    private void pipeClient_MessageReceived(object sender, ConnectionMessageEventArgs<Message> e)
    {
        var inMsg = e.Message;
        if (inMsg == null)
            return;
            
        switch (inMsg.msgType)
        {
            case MessageType.Version:
                if (new Version(inMsg.message) > Assembly.GetEntryAssembly().GetName().Version)
                {
                    ManuallyDisconnected = true;
                    Pipe.Client.DisconnectAsync();
                    Api.PluginLog.Error($"Hypnotoad is out of date and cannot work with the running bard program.");
                }
                break;
            case MessageType.NoteOn:
                PerformActions.PlayNote(Convert.ToInt16(inMsg.message), true);
                break;
            case MessageType.NoteOff:
                PerformActions.PlayNote(Convert.ToInt16(inMsg.message), false);
                break;
            case MessageType.ProgramChange:
                PerformActions.GuitarSwitchTone(Convert.ToInt32(inMsg.message));
                break;
            case MessageType.Chat:
            case MessageType.Instrument:
            case MessageType.AcceptReply:
            case MessageType.SetGfx:
            case MessageType.MasterSoundState:
            case MessageType.MasterVolume:
            case MessageType.VoiceSoundState:
            case MessageType.EffectsSoundState:
            case MessageType.StartEnsemble:
            case MessageType.ExitGame:
                qt.Enqueue(inMsg);
                break;
        }
    }

    public void Dispose()
    {
        ManuallyDisconnected = true;

        Pipe.Client.Connected -= pipeClient_Connected;
        Pipe.Client.MessageReceived -= pipeClient_MessageReceived;
        Pipe.Client.Disconnected -= pipeClient_Disconnected;
        _reconnectTimer.Elapsed -= reconnectTimer_Elapsed;

        Pipe.Client.DisconnectAsync();
        Pipe.Client.DisposeAsync();
        Pipe.Dispose();
    }

    public void Draw()
    {
        // This is our only draw handler attached to UIBuilder, so it needs to be
        // able to draw any windows we might have open.
        // Each method checks its own visibility/state to ensure it only draws when
        // it actually makes sense.
        // There are other ways to do this, but it is generally best to keep the number of
        // draw delegates as low as possible.

        DrawMainWindow();
    }

    public void DrawMainWindow()
    {
        if (Visible)
        {
            ImGui.SetNextWindowSize(new Vector2(300, 110), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(300, 110), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Hypnotoad", ref visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                // can't ref a property, so use a local copy
                var configValue = configuration.Autoconnect;
                if (ImGui.Checkbox("Autoconnect", ref configValue))
                {
                    configuration.Autoconnect = configValue;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    configuration.Save();
                }

                //The connect Button
                if (ImGui.Button("Connect"))
                {
                    if (configuration.Autoconnect)
                        ManuallyDisconnected = false;
                    _reconnectTimer.Interval = 500;
                    _reconnectTimer.Enabled  = true;
                }
                ImGui.SameLine();
                //The disconnect Button
                if (ImGui.Button("Disconnect"))
                {
                    if (!Pipe.Client.IsConnected)
                        return;

                    Pipe.Client.DisconnectAsync();

                    ManuallyDisconnected = true;
                }

                ImGui.Text($"Is connected: {Pipe.Client.IsConnected}");
            }
            ImGui.End();
        }

        //Check performance state
        /*if (Hypnotoad.AgentPerformance.InPerformanceMode != performanceModeOpen)
        {
            performanceModeOpen = Hypnotoad.AgentPerformance.InPerformanceMode;
            if (Pipe.Client != null && Pipe.Client.IsConnected)
            {
                Pipe.Client.WriteAsync(new Message
                {
                    msgType = MessageType.PerformanceModeState,
                    message = Environment.ProcessId + ":" + Hypnotoad.AgentPerformance.Instrument.ToString()
                });
            }
        }*/

        //Do the in queue
        while (qt.Count > 0)
        {
            try
            {
                var msg = qt.Dequeue();
                switch (msg.msgType)
                {
                    case MessageType.Chat:
                        var chatMessageChannelType = ChatMessageChannelType.ParseByChannelCode(msg.msgChannel);
                        if (chatMessageChannelType.Equals(ChatMessageChannelType.None))
                            Chat.SendMessage(msg.message);
                        else
                            Chat.SendMessage(chatMessageChannelType.ChannelShortCut + " " + msg.message);
                        break;
                    case MessageType.Instrument:
                        PerformActions.DoPerformAction(Convert.ToUInt32(msg.message));
                        break;
                    case MessageType.AcceptReply:
                        PerformActions.ConfirmReceiveReadyCheck();
                        break;
                    case MessageType.SetGfx:
                        //TODO remove me after 3 version init: 1.0.5.4
                        bool lowGfx;
                        if (Char.IsNumber(msg.message, 0))
                            lowGfx = (Convert.ToUInt32(msg.message) == 1);
                        else
                            lowGfx = Convert.ToBoolean(msg.message);
                        if (lowGfx)
                        {
                            GameSettings.AgentConfigSystem.GetSettings();
                            GameSettings.AgentConfigSystem.SetMinimalGfx();
                            Hypnotoad.AgentConfigSystem.ApplyGraphicSettings();
                        }
                        else
                        {
                            GameSettings.AgentConfigSystem.RestoreSettings();
                            Hypnotoad.AgentConfigSystem.ApplyGraphicSettings();
                        }
                        break;
                    case MessageType.MasterSoundState:
                        GameSettings.AgentConfigSystem.SetMasterSoundEnable(Convert.ToBoolean(msg.message));
                        break;
                    case MessageType.MasterVolume:
                        if ((short)Convert.ToInt16(msg.message) == -1)
                        {
                            Pipe.Write(MessageType.MasterVolume, 0, GameSettings.AgentConfigSystem.GetMasterSoundVolume());
                            Pipe.Write(MessageType.MasterSoundState, 0, GameSettings.AgentConfigSystem.GetMasterSoundEnable());
                            Pipe.Write(MessageType.VoiceSoundState, 0, GameSettings.AgentConfigSystem.GetVoiceSoundEnable());
                            Pipe.Write(MessageType.EffectsSoundState, 0, GameSettings.AgentConfigSystem.GetEffectsSoundEnable());
                        }
                        else
                            GameSettings.AgentConfigSystem.SetMasterSoundVolume(Convert.ToInt16(msg.message));
                        break;
                    case MessageType.VoiceSoundState:
                        GameSettings.AgentConfigSystem.SetVoiceSoundEnable(Convert.ToBoolean(msg.message));
                        break;
                    case MessageType.EffectsSoundState:
                        GameSettings.AgentConfigSystem.SetEffectsSoundEnable(Convert.ToBoolean(msg.message));
                        break;
                    case MessageType.ExitGame:
                        Process.GetCurrentProcess().Kill();
                        break;
                    case MessageType.StartEnsemble:
                        PerformActions.BeginReadyCheck();
                        PerformActions.ConfirmBeginReadyCheck();
                        break;
                }
            }
            catch (Exception ex)
            {
                Api.PluginLog.Error($"exception: {ex}");
            }
        }
    }
}