/*
 * Copyright(c) 2024 GiR-Zippo, Meowchestra
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using H.Pipes.Args;
using HypnotoadPlugin.Config;
using HypnotoadPlugin.GameFunctions;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Timers;

namespace HypnotoadPlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private Timer _reconnectTimer { get; set; } = new();
    private Queue<IPCMessage> qt { get; set; } = new();
    private Configuration configuration { get; init; }

    // this extra bool exists for ImGui, since you can't ref a property
    private bool visible;
    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    public bool ManuallyDisconnected { get; set; }


    public MainWindow(Hypnotoad plugin, Configuration configuration) : base(
        "Hypnotoad", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.configuration = configuration;

        Pipe.Initialize(this.configuration.ConnectToBMP);
        Pipe.Client.Connected += pipeClient_Connected;
        Pipe.Client.MessageReceived += pipeClient_MessageReceived;
        Pipe.Client.Disconnected += pipeClient_Disconnected;
        _reconnectTimer.Elapsed += reconnectTimer_Elapsed;

        _reconnectTimer.Interval = 2000;
        _reconnectTimer.Enabled = configuration.Autoconnect;

        Visible = false;
    }
    private void pipeClient_Connected(object sender, ConnectionEventArgs<IPCMessage> e)
    {
        Pipe.Client.WriteAsync(new IPCMessage
        {
            msgType = MessageType.Handshake,
            msgChannel = 0,
            message = Environment.ProcessId.ToString()
        });


        Pipe.Client.WriteAsync(new IPCMessage
        {
            msgType = MessageType.Version,
            msgChannel = 0,
            message = Environment.ProcessId + ":" + Assembly.GetExecutingAssembly().GetName().Version.ToString()
        });

        Pipe.Write(MessageType.SetGfx, 0, GameSettings.AgentConfigSystem.CheckLowSettings(GameSettingsTables.Instance.CustomTable));
        Pipe.Write(MessageType.MasterSoundState, 0, GameSettings.AgentConfigSystem.GetMasterSoundEnable());
        Pipe.Write(MessageType.MasterVolume, 0, GameSettings.AgentConfigSystem.GetMasterSoundVolume());

        Collector.Instance.UpdateClientStats();
    }

    private void pipeClient_Disconnected(object sender, ConnectionEventArgs<IPCMessage> e)
    {
        if (!configuration.Autoconnect)
            return;

        _reconnectTimer.Interval = 2000;
        _reconnectTimer.Enabled = configuration.Autoconnect;
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

    private void pipeClient_MessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> e)
    {
        var inMsg = e.Message;
        if (inMsg == null)
            return;

        switch (inMsg.msgType)
        {
            case MessageType.Version:
                /*if (new Version(inMsg.message) > Assembly.GetEntryAssembly().GetName().Version)
                {
                    ManuallyDisconnected = true;
                    Pipe.Client.DisconnectAsync();
                    Api.PluginLog.Error($"Hypnotoad is out of date and cannot work with the running bard program.");
                }*/
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
            case MessageType.ClientLogout:
            case MessageType.GameShutdown:
            case MessageType.Chat:
            case MessageType.Instrument:
            case MessageType.AcceptReply:
            case MessageType.SetGfx:
            case MessageType.MasterSoundState:
            case MessageType.MasterVolume:
            case MessageType.VoiceSoundState:
            case MessageType.EffectsSoundState:
            case MessageType.SetWindowRenderSize:
            case MessageType.StartEnsemble:
            case MessageType.ExitGame:
            case MessageType.PartyInvite:
            case MessageType.PartyInviteAccept:
            case MessageType.PartyPromote:
            case MessageType.PartyEnterHouse:
            case MessageType.PartyTeleport:
            case MessageType.PartyFollow:
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

    public override void Update()
    {
        //Do the in queue
        while (qt.Count > 0)
        {
            try
            {
                var msg = qt.Dequeue();
                switch (msg.msgType)
                {
                    case MessageType.ClientLogout:
                        MiscGameFunctions.CharacterLogout();
                        break;
                    case MessageType.GameShutdown:
                        MiscGameFunctions.GameShutdown();
                        break;
                    case MessageType.Chat:
                        var chatMessageChannelType = ChatMessageChannelType.ParseByChannelCode(msg.msgChannel);
                        if (chatMessageChannelType.Equals(ChatMessageChannelType.None))
                            Chat.SendMessage(msg.message);
                        else
                            Chat.SendMessage(chatMessageChannelType.ChannelShortCut + " " + msg.message);
                        break;
                    case MessageType.Instrument:
                        PerformActions.DoPerformActionOnTick(Convert.ToUInt32(msg.message));
                        break;
                    case MessageType.AcceptReply:
                        PerformActions.ConfirmReceiveReadyCheck();
                        break;
                    case MessageType.SetGfx:
                        GameSettings.AgentConfigSystem.SetGfx(Convert.ToBoolean(msg.message));
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
                    case MessageType.SetWindowRenderSize:
                        Misc.SetGameRenderSize(Convert.ToUInt32(msg.message.Split(';')[0]), Convert.ToUInt32(msg.message.Split(';')[1]));
                        break;
                    case MessageType.StartEnsemble:
                        PerformActions.BeginReadyCheck();
                        PerformActions.ConfirmBeginReadyCheck();
                        break;
                    case MessageType.ExitGame:
                        Process.GetCurrentProcess().Kill();
                        break;
                    case MessageType.PartyInvite:
                        Party.Instance.PartyInvite(msg.message);
                        break;
                    case MessageType.PartyInviteAccept:
                        Party.Instance.AcceptPartyInviteEnable();
                        break;
                    case MessageType.PartyPromote:
                        Party.Instance.PromoteCharacter(msg.message);
                        break;
                    case MessageType.PartyEnterHouse:
                        FollowSystem.StopFollow();
                        Party.Instance.EnterHouse();
                        break;
                    case MessageType.PartyTeleport:
                        FollowSystem.StopFollow();
                        Party.Instance.Teleport(Convert.ToBoolean(msg.message));
                        break;
                    case MessageType.PartyFollow:
                        if (msg.message == "")
                            FollowSystem.StopFollow();
                        else
                            FollowSystem.FollowCharacter(msg.message.Split(';')[0], Convert.ToUInt16(msg.message.Split(';')[1]));
                        break;
                }
            }
            catch (Exception ex)
            {
                Api.PluginLog.Error($"exception: {ex}");
            }
        }
    }

    bool showModal = false;
    public override void Draw()
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
                _reconnectTimer.Enabled = true;
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

            ImGui.NewLine();
            var bmpValue = configuration.ConnectToBMP;
            if (ImGui.Checkbox("Use BMP", ref bmpValue))
            {
                configuration.ConnectToBMP = bmpValue;
                configuration.Save();
                showModal = true;
            }

            if (showModal)
            {
                if (ImGui.Begin("##modal"))
                {
                    ImGui.Text("Please restart the plugin.");
                    ImGui.Text("I'm staying here until you did it :P");
                    ImGui.End();
                }
            }

            //PlayerConfig Save/Erase
            ImGui.NewLine();
            ImGui.Text($"Player configuration");
            ImGui.BeginGroup();
            if (ImGui.Button("Save"))
            {
                GameSettings.AgentConfigSystem.SaveConfig();
            }
            ImGui.SameLine();
            if (ImGui.Button("Erase"))
            {
                File.Delete($"{Api.PluginInterface.GetPluginConfigDirectory()}\\{Api.ClientState.LocalPlayer.Name}-({Api.ClientState.LocalPlayer.HomeWorld.ValueNullable?.Name.ToDalamudString().TextValue}).json");
            }
            ImGui.EndGroup();
        }
        ImGui.End();
    }
}