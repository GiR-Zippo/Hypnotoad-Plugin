using Dalamud.Logging;
using H.Pipes;
using H.Formatters;
using ImGuiNET;
using System;
using System.Numerics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using static HypnotoadPlugin.GfxSettings;

namespace HypnotoadPlugin
{
    public class Message
    {
        public MessageType msgType { get; set; } = MessageType.None;
        public int msgChannel { get; set; } = 0;
        public string message { get; set; } = "";
    }

    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private System.Timers.Timer _reconnectTimer { get; set; } = new System.Timers.Timer();
        private readonly PipeClient<Message> _pipeClient;
        private Queue<Message> qt = new Queue<Message>();
        private Configuration configuration;

        private ImGuiScene.TextureWrap goatImage;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool ManuallyDisconnect = false;
        public bool ManuallyDisconnected
        {
            get { return this.ManuallyDisconnect; }
            set { this.ManuallyDisconnect = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration, ImGuiScene.TextureWrap goatImage)
        {
            this.configuration = configuration;
            this.goatImage = goatImage;
            _pipeClient = new PipeClient<Message>("LightAmp-DalamudBridge", formatter: new NewtonsoftJsonFormatter());
            _pipeClient.MessageReceived += pipeClient_MessageReceived;
            _pipeClient.Disconnected += pipeClient_Disconnected;
            this._reconnectTimer.Elapsed += reconnectTimer_Elapsed;
            this._reconnectTimer.Interval = 2000;
            this._reconnectTimer.Enabled = configuration.Autoconnect;

            Visible = true;
        }

        private void pipeClient_Disconnected(object sender, H.Pipes.Args.ConnectionEventArgs<Message> e)
        {
            if (!configuration.Autoconnect)
                return;

            this._reconnectTimer.Interval = 2000;
            this._reconnectTimer.Enabled = configuration.Autoconnect;
        }

        private void reconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (ManuallyDisconnect)
                return;

            if (_pipeClient.IsConnected)
            {
                _reconnectTimer.Enabled = false;
                return;
            }

            if (!_pipeClient.IsConnecting)
            {
                _pipeClient.ConnectAsync();
                _pipeClient.WriteAsync(new Message
                {
                    msgType = MessageType.Handshake,
                    msgChannel = 0,
                    message = Process.GetCurrentProcess().Id.ToString()
                });

                _pipeClient.WriteAsync(new Message
                {
                    msgType = MessageType.SetGfx,
                    msgChannel = 0,
                    message = Process.GetCurrentProcess().Id.ToString() + ":" + TestPlugin.AgentConfigSystem.CheckLowSettings().ToString()
                });
            }
        }

        private void pipeClient_MessageReceived(object sender, H.Pipes.Args.ConnectionMessageEventArgs<Message> e)
        {
            if (!Visible)
            {
                return;
            }

            Message? inMsg = e.Message as Message;
            if (inMsg == null)
                return;

            if (Visible && (inMsg.msgType == MessageType.Chat || 
                            inMsg.msgType == MessageType.Instrument ||
                            inMsg.msgType == MessageType.AcceptReply ||
                            inMsg.msgType == MessageType.SetGfx ))
                qt.Enqueue(inMsg);
        }

        public void Dispose()
        {
            ManuallyDisconnected = true;
            this._pipeClient.DisconnectAsync();
            this._pipeClient.DisposeAsync();
            this.goatImage.Dispose();
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
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(300, 80), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(300, 90), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Hypnotoad", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse))
            {
                // can't ref a property, so use a local copy
                var configValue = this.configuration.Autoconnect;
                if (ImGui.Checkbox("Autoconnect", ref configValue))
                {
                    this.configuration.Autoconnect = configValue;
                    // can save immediately on change, if you don't want to provide a "Save and Close" button
                    this.configuration.Save();
                }

                //The connect Button
                if (ImGui.Button("Connect"))
                {
                    if (this.configuration.Autoconnect)
                        ManuallyDisconnect = false;
                    _reconnectTimer.Interval = 500;
                    _reconnectTimer.Enabled = true;
                }
                ImGui.SameLine();
                //The disconnect Button
                if (ImGui.Button("Disconnect"))
                {
                    if (!_pipeClient.IsConnected)
                        return;

                    _pipeClient.DisconnectAsync();

                    ManuallyDisconnected = true;
                }

                ImGui.Text($"Is connected to LA: {this._pipeClient.IsConnected}");

                ImGui.Spacing();

                ImGui.Image(this.goatImage.ImGuiHandle, new Vector2(this.goatImage.Width, this.goatImage.Height));

                while (qt.Count > 0)
                {
                    try
                    {
                        Message msg = qt.Dequeue();
                        switch (msg.msgType)
                        {
                            case MessageType.Chat:
                                ChatMessageChannelType chatMessageChannelType = ChatMessageChannelType.ParseByChannelCode(msg.msgChannel);
                                if (chatMessageChannelType.Equals(ChatMessageChannelType.None))
                                    TestPlugin.CBase.Functions.Chat.SendMessage(msg.message);
                                else
                                    TestPlugin.CBase.Functions.Chat.SendMessage(chatMessageChannelType.ChannelShortCut + " " + msg.message);
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
                                    TestPlugin.AgentConfigSystem.GetObjQuantity();
                                    TestPlugin.AgentConfigSystem.SetMinimalObjQuantity();
                                    TestPlugin.AgentConfigSystem.ApplyGraphicSettings();
                                }
                                else
                                {
                                    TestPlugin.AgentConfigSystem.RestoreObjQuantity();
                                    TestPlugin.AgentConfigSystem.ApplyGraphicSettings();
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        PluginLog.LogError($"exception: {ex}");
                    }
                }
            }
            ImGui.End();
        }
    }
}
