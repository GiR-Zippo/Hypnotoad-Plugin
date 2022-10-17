/*
 * Copyright(c) 2022 Ori @MidiBard2
 */

using FFXIVClientStructs.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HypnotoadPlugin;
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public static class Offsets
{
    [StaticAddress("48 8D 05 ?? ?? ?? ?? 48 89 03 48 8D 4B 40")]
    public static IntPtr MetronomeAgent { get; private set; }

    [StaticAddress("48 8D 05 ?? ?? ?? ?? 48 8B F9 48 89 01 48 8D 05 ?? ?? ?? ?? 48 89 41 28 48 8B 49 48")]
    public static IntPtr PerformanceAgent { get; private set; }

    [StaticAddress("48 8D 05 ?? ?? ?? ?? C7 83 E0 00 00 00 ?? ?? ?? ??")]
    public static IntPtr AgentConfigSystem { get; private set; }

    [StaticAddress("48 8B 15 ?? ?? ?? ?? F6 C2 ??")]
    public static IntPtr PerformanceStructPtr { get; private set; }

    [Function("48 89 6C 24 10 48 89 74 24 18 57 48 83 EC ?? 48 83 3D ?? ?? ?? ?? ?? 41 8B E8")]
    public static IntPtr DoPerformAction { get; private set; }

    [Offset("40 88 ?? ?? 66 89 ?? ?? 40 84", +3)]
    public static byte InstrumentOffset { get; private set; }

    [Function("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 0F B6 FA 48 8B D9 84 D2 ")]
    public static IntPtr UpdateMetronome { get; private set; }

    [Function("83 FA 04 77 4E")]
    public static IntPtr UISetTone { get; private set; }

    [Function("48 8B C4 56 48 81 EC ?? ?? ?? ?? 48 89 58 10 ")]
    public static IntPtr ApplyGraphicConfigsFunc { get; private set; }

    [Function("48 89 ? ? ? 48 89 ? ? ? 57 48 83 EC ? 8B FA 41 0F ? ? 03 79")]
    public static IntPtr PressNote { get; private set; }
}
