/*
 * Copyright(c) 2024 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using H.Formatters;
using H.Pipes;
using System;

namespace HypnotoadPlugin
{
    public class IPCMessage
    {
        public MessageType msgType { get; init; } = MessageType.None;
        public int msgChannel { get; init; }
        public string message { get; init; } = "";
    }

    internal static class Pipe
    {
        internal static PipeClient<IPCMessage> Client { get; private set; }

        internal static void Initialize(bool connectBMP = false)
        {
            Client = new PipeClient<IPCMessage>(connectBMP? "DalamudDoot" : "Hypnotoad", formatter: new NewtonsoftJsonFormatter());
        }

        internal static void Write(MessageType messageType, int channel, bool msg)
        {
            Pipe.Client.WriteAsync(new IPCMessage
            {
                msgType = messageType,
                msgChannel = channel,
                message = Environment.ProcessId + ":" + msg.ToString()
            });
        }

        internal static void Write(MessageType messageType, int channel, float msg)
        {
            Pipe.Client.WriteAsync(new IPCMessage
            {
                msgType = messageType,
                msgChannel = channel,
                message = Environment.ProcessId + ":" + msg.ToString()
            });
        }

        internal static void Write(MessageType messageType, int channel, int msg)
        {
            Pipe.Client.WriteAsync(new IPCMessage
            {
                msgType = messageType,
                msgChannel = channel,
                message = Environment.ProcessId + ":" + msg.ToString()
            });
        }

        internal static void Dispose()
        {
            Client.DisconnectAsync();
        }
    }
}
