using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Game.Network;
using Dalamud.Logging;
using HypnotoadPlugin.Offsets;

namespace HypnotoadPlugin.Network
{
    public static unsafe class NetworkReader
    {
        private static string EnsembleStart   = "?? ?? ?? ?? ?? 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        private static string EnsembleRequest = "?? ?? ?? ?? ?? 00 ?? 00 ?? ?? ?? 10 00 00 00 00 00 00 ?? ?? ?? 00 00 00";
        private static string EnsembleEquip   = "02 00 00 00 10 00 00 00 ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        private static string EnsembleUnequip = "02 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";
        private static string EnsembleStop    = "?? ?? ?? ?? ?? 00 ?? 00 ?? ?? ?? ?? 00 00 00 00";

        private static byte[] GenerateHeader(uint size, byte[] time, ushort opCode, uint sourceActorId, uint targetActorId)
        {
            byte[] src = BitConverter.GetBytes(sourceActorId);
            byte[] tar = BitConverter.GetBytes(targetActorId);
            byte[] opcode = BitConverter.GetBytes(opCode);
            byte[] sz = BitConverter.GetBytes(size);

            byte[] outPacketArray = new byte[size];
            outPacketArray[0] = sz[0];
            outPacketArray[1] = sz[1];
            outPacketArray[2] = sz[2];
            outPacketArray[3] = sz[3];

            outPacketArray[4] = tar[0];
            outPacketArray[5] = tar[1];
            outPacketArray[6] = tar[2];
            outPacketArray[7] = tar[3];

            outPacketArray[8] = src[0];
            outPacketArray[9] = src[1];
            outPacketArray[10] = src[2];
            outPacketArray[11] = src[3];

            outPacketArray[18] = opcode[0];
            outPacketArray[19] = opcode[1];

            outPacketArray[24] = time[0];
            outPacketArray[25] = time[1];
            outPacketArray[26] = time[2];
            outPacketArray[27] = time[3];
            return outPacketArray;
        }

        private static byte[] Packet48(byte[] time, ushort opCode, uint sourceActorId, uint targetActorId, byte[] trunk)
        {
            byte[] outPacketArray = GenerateHeader(48, time, opCode, sourceActorId, targetActorId);

            for (int i = 0; i < 15; i++)
                outPacketArray[i + 32] = trunk[i];
            return outPacketArray;
        }

        private static byte[] Packet56(byte[] time, ushort opCode, uint sourceActorId, uint targetActorId, byte[] trunk)
        {
            byte[] outPacketArray = GenerateHeader(56, time, opCode, sourceActorId, targetActorId);

            for (int i = 0; i < 23; i++)
                outPacketArray[i+32] = trunk[i];
            return outPacketArray;
        }

        private static byte[] Packet88(byte[] time, ushort opCode, uint sourceActorId, uint targetActorId, byte[] trunk)
        {
            byte[] outPacketArray = GenerateHeader(88, time, opCode, sourceActorId, targetActorId);

            for (int i = 0; i < 54; i++)
                outPacketArray[i + 32] = trunk[i];
            return outPacketArray;
        }

        public static void NetworkMessage(nint dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, NetworkMessageDirection direction)
        {
            if (direction == NetworkMessageDirection.ZoneDown && Pipe.Client != null && Pipe.Client.IsConnected)
            {
                byte[] inPacket = GetPacket(dataPtr);
                byte[] ti = new byte[5];
                Marshal.Copy(dataPtr - 8, ti, 0, 4);
                string hexString = BitConverter.ToString(inPacket);
                byte[] packet = null;

                /*if (opCode == 1234)
                {
                    PluginLog.Debug("NET: " + Convert.ToString(opCode));
                    PluginLog.Debug(BitConverter.ToString(ti));
                    PluginLog.Debug(hexString);
                    PluginLog.Debug(sourceActorId.ToString());
                    PluginLog.Debug(targetActorId.ToString());
                }*/

                if (checkPattern(EnsembleStart, inPacket))
                    packet = Packet88(ti, opCode, sourceActorId, targetActorId, inPacket);
                else if (checkPattern(EnsembleEquip, inPacket))
                    packet = Packet56(ti, opCode, sourceActorId, targetActorId, inPacket);
                else if (checkPattern(EnsembleUnequip, inPacket))
                    packet = Packet56(ti, opCode, sourceActorId, targetActorId, inPacket);
                else if (checkPattern(EnsembleRequest, inPacket))
                    packet = Packet56(ti, opCode, sourceActorId, targetActorId, inPacket);
                else if (checkPattern(EnsembleStop, inPacket))
                    packet = Packet48(ti, opCode, sourceActorId, targetActorId, inPacket);

                if (packet != null)
                {
                    Pipe.Client.WriteAsync(new Message
                    {
                        msgType = MessageType.NetworkPacket,
                        message = Environment.ProcessId + ":" + Convert.ToBase64String(packet)
                    });
                }
            }
        }

        internal static void Initialize() => Api.GameNetwork.NetworkMessage += NetworkMessage;
        internal static void Dispose() => Api.GameNetwork.NetworkMessage -= NetworkMessage;

        private static unsafe byte[] GetPacket(IntPtr dataPtr)
        {
            using var memoryStream = new MemoryStream();
            using var unmanagedMemoryStream = new UnmanagedMemoryStream((byte*)(dataPtr /*- 0x20*/).ToPointer(), 4096);
            unmanagedMemoryStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private static bool checkPattern(string pattern, byte[] array2check)
        {
            string[] strBytes = pattern.Split(' ');
            for (int i = 0; i < strBytes.Length; i++)
            {
                if (strBytes[i] == "?" || strBytes[i] == "??")
                {
                    continue;
                }
                else if (byte.Parse(strBytes[i], NumberStyles.HexNumber) == array2check[i])
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }



    }
}
