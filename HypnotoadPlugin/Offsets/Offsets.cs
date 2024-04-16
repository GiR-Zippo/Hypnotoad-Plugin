/*
 * Copyright(c) 2023 GiR-Zippo, Meowchestra, akira0245 @MidiBard, Ori @MidiBard2
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
*/

using Dalamud.Hooking;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HypnotoadPlugin.Offsets;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]

public static partial class Chat
{
    private static class Signatures
    {
        internal const string SendChat = "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9";
        internal const string SanitiseString = "E8 ?? ?? ?? ?? EB 0A 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 8D";
    }
}

public static class Offsets
{
    [StaticAddress("48 8D 05 ?? ?? ?? ?? 48 8B ?? 48 89 01 48 8D 05 ?? ?? ?? ?? 48 89 41 28 48 8B 49 48")]
    public static nint AgentPerformance { get; private set; }

    [StaticAddress("48 8D 05 ?? ?? ?? ?? C7 83 E0 00 00 00 ?? ?? ?? ??")]
    public static nint AgentConfigSystem { get; private set; }

    [StaticAddress("48 8B 15 ?? ?? ?? ?? F6 C2 ??")]
    public static nint PerformanceStructPtr { get; private set; }

    [Function("48 89 6C 24 10 48 89 74 24 18 57 48 83 EC ?? 48 83 3D ?? ?? ?? ?? ?? 41 8B E8")]
    public static nint DoPerformAction { get; private set; }

    [Function("48 8B C4 56 48 81 EC ?? ?? ?? ?? 48 89 58 10 ")]
    public static nint ApplyGraphicConfigsFunc { get; private set; }

    [Function("40 53 48 83 EC 20 48 8B D9 48 83 C1 78 E8 ? ? ? ? 48 8D 8B ? ? ? ? E8 ? ? ? ? 48 8D 53 20 ")]
    public static IntPtr NetworkEnsembleStart { get; private set; }

}

public sealed unsafe class AgentPerformance : AgentInterface
{
    public AgentPerformance(AgentInterface agentInterface) : base(agentInterface.Pointer, agentInterface.Id) { }
    public static AgentPerformance Instance => Hypnotoad.AgentPerformance;
    public new AgentPerformanceStruct* Struct => (AgentPerformanceStruct*)Pointer;

    [StructLayout(LayoutKind.Explicit)]
    public struct AgentPerformanceStruct
    {
        [FieldOffset(0)] public FFXIVClientStructs.FFXIV.Component.GUI.AgentInterface AgentInterface;
        [FieldOffset(0x20)] public byte InPerformanceMode;
        [FieldOffset(0x1F)] public byte Instrument;
        [FieldOffset(0x38)] public long PerformanceTimer1;
        [FieldOffset(0x40)] public long PerformanceTimer2;
        [FieldOffset(0x5C)] public int NoteOffset;
        [FieldOffset(0x60)] public int CurrentPressingNote;
        [FieldOffset(0xFC)] public int OctaveOffset;
        [FieldOffset(0x1B0)] public int GroupTone;
    }

    internal int CurrentGroupTone => Struct->GroupTone;
    internal bool InPerformanceMode => Struct->InPerformanceMode != 0;
    internal bool notePressed => Struct->CurrentPressingNote != -100;
    internal int noteNumber => Struct->CurrentPressingNote;
    internal long PerformanceTimer1 => Struct->PerformanceTimer1;
    internal long PerformanceTimer2 => Struct->PerformanceTimer2;
    internal byte Instrument => Struct->Instrument;
}

internal class EnsembleManager : IDisposable
{
    private delegate long sub_NetworkEnsemble(IntPtr a1, IntPtr a2);
    private Hook<sub_NetworkEnsemble> NetworkEnsembleHook;
    internal EnsembleManager()
    {
        //Get the ensemble start
        NetworkEnsembleHook = Api.GameInteropProvider.HookFromAddress<sub_NetworkEnsemble>(Offsets.NetworkEnsembleStart, (a1, a2) =>
        {
            //and pipe it
            if (Pipe.Client != null && Pipe.Client.IsConnected)
            {
                Pipe.Client.WriteAsync(new Message
                {
                    msgType = MessageType.StartEnsemble,
                    message = Environment.ProcessId + ":1"
                });
            }
            return NetworkEnsembleHook.Original(a1, a2);
        });
        NetworkEnsembleHook.Enable();
    }

    public void Dispose()
    {
        NetworkEnsembleHook?.Dispose();
    }
}
