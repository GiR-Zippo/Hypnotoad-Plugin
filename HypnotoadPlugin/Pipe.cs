using H.Formatters;
using H.Pipes;
using HypnotoadPlugin.Offsets;
using System;

namespace HypnotoadPlugin
{
    internal static class Pipe
    {
        internal static PipeClient<Message> Client { get; private set; }

        internal static void Initialize()
        {
            Client = new PipeClient<Message>("Hypnotoad", formatter: new NewtonsoftJsonFormatter());
        }

        internal static void Write(MessageType messageType, int channel, bool msg)
        {
            Pipe.Client.WriteAsync(new Message
            {
                msgType = messageType,
                msgChannel = channel,
                message = Environment.ProcessId + ":" + msg.ToString()
            });
        }

        internal static void Write(MessageType messageType, int channel, float msg)
        {
            Pipe.Client.WriteAsync(new Message
            {
                msgType = messageType,
                msgChannel = channel,
                message = Environment.ProcessId + ":" + msg.ToString()
            });
        }

        internal static void Write(MessageType messageType, int channel, int msg)
        {
            Pipe.Client.WriteAsync(new Message
            {
                msgType = messageType,
                msgChannel = channel,
                message = Environment.ProcessId + ":" + msg.ToString()
            });
        }

        internal static void Dispose()
        {

        }
    }
}
