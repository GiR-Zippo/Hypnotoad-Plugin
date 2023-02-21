using System;
using System.IO;
using Dalamud.Game.Network;

namespace HypnotoadPlugin.Offsets
{
    public static unsafe class NetworkReader
    {
        public static void NetworkMessage(nint dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, NetworkMessageDirection direction)
        {
            if (direction == NetworkMessageDirection.ZoneDown && Pipe.Client != null && Pipe.Client.IsConnected)
                Pipe.Client.WriteAsync(new Message
                {
                    msgType = MessageType.NetworkPacket,
                    message = Environment.ProcessId + ":" + Convert.ToBase64String(GetPacket(dataPtr))
                });
        }

        internal static void Initialize() => Api.GameNetwork.NetworkMessage += NetworkMessage;
        internal static void Dispose() => Api.GameNetwork.NetworkMessage -= NetworkMessage;

        private static unsafe byte[] GetPacket(IntPtr dataPtr)
        {
            using var memoryStream = new MemoryStream();
            using var unmanagedMemoryStream = new UnmanagedMemoryStream((byte*)(dataPtr - 0x20).ToPointer(), 4096);
            unmanagedMemoryStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
