using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Timers;
using Dalamud.Logging;
using H.Formatters;
using H.Pipes;
using H.Pipes.Args;
using HypnotoadPlugin.Offsets;
using ImGuiNET;
using ImGuiScene;

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

    private TextureWrap goatImage;

    // this extra bool exists for ImGui, since you can't ref a property
    private bool visible;
    public bool Visible
    {
        get => visible;
        set => visible = value;
    }

    public bool ManuallyDisconnected { get; set; }

    // passing in the image here just for simplicity
    public PluginUI(Configuration configuration, TextureWrap goatImage)
    {
        this.configuration          =  configuration;
        this.goatImage              =  goatImage;

        Pipe.Initialize();
        Pipe.Client.Connected += pipeClient_Connected;
        Pipe.Client.MessageReceived += pipeClient_MessageReceived;
        Pipe.Client.Disconnected += pipeClient_Disconnected;
        _reconnectTimer.Elapsed += reconnectTimer_Elapsed;

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

        Pipe.Client.WriteAsync(new Message
        {
            msgType    = MessageType.SetGfx,
            msgChannel = 0,
            message    = Environment.ProcessId + ":" + GfxSettings.AgentConfigSystem.CheckLowSettings()
        });
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
                    PluginLog.LogError($"Hypnotoad is out of date and cannot work with the running bard program.");
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
            case MessageType.StartEnsemble:
                qt.Enqueue(inMsg);
                break;
        }
    }

    public void Dispose()
    {
        ManuallyDisconnected = true;
        Pipe.Client.DisconnectAsync();
        Pipe.Client.DisposeAsync();
        Pipe.Dispose();
        goatImage.Dispose();
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
                        if (Convert.ToUInt32(msg.message) == 1)
                        {
                            GfxSettings.AgentConfigSystem.GetObjQuantity();
                            GfxSettings.AgentConfigSystem.SetMinimalObjQuantity();
                            Hypnotoad.AgentConfigSystem.ApplyGraphicSettings();
                        }
                        else
                        {
                            GfxSettings.AgentConfigSystem.RestoreObjQuantity();
                            Hypnotoad.AgentConfigSystem.ApplyGraphicSettings();
                        }
                        break;
                    case MessageType.StartEnsemble:
                        PerformActions.BeginReadyCheck();
                        PerformActions.ConfirmBeginReadyCheck();
                        break;
                }
            }
            catch (Exception ex)
            {
                PluginLog.LogError($"exception: {ex}");
            }
        }
    }
}