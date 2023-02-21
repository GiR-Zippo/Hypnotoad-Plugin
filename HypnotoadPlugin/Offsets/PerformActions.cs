/*
 * Copyright(c) 2022 Ori @MidiBard2
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace HypnotoadPlugin.Offsets;

public class PerformActions
{
    internal delegate void DoPerformActionDelegate(nint performInfoPtr, uint instrumentId, int a3 = 0);
    private static DoPerformActionDelegate doPerformAction { get; } = Marshal.GetDelegateForFunctionPointer<DoPerformActionDelegate>(Offsets.DoPerformAction);
    public static void DoPerformAction(uint instrumentId)
    {
        PluginLog.Information($"[DoPerformAction] instrumentId: {instrumentId}");
        doPerformAction(Offsets.PerformanceStructPtr, instrumentId);
    }

    private PerformActions() { }
    private static unsafe nint GetWindowByName(string s) => (nint)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(s);
    public static void init() => SignatureHelper.Initialise(new PerformActions());
    public static void SendAction(nint ptr, params ulong[] param)
    {
        if (param.Length % 2 != 0) 
            throw new ArgumentException("The parameter length must be an integer multiple of 2.");
        if (ptr == nint.Zero) 
            throw new ArgumentException("input pointer is null");

        var paircount = param.Length / 2;
        unsafe
        {
            fixed (ulong* u = param)
            {
                AtkUnitBase.MemberFunctionPointers.FireCallback((AtkUnitBase*)ptr, paircount, (AtkValue*)u, (void*)1);
            }
        }
    }

    public static bool SendAction(string name, params ulong[] param)
    {
        var ptr = GetWindowByName(name);
        if (ptr == nint.Zero) return false;
        SendAction(ptr, param);
        return true;
    }

    public static bool PressKey(int keynumber, ref int offset, ref int octave)
    {
        if (!TargetWindowPtr(out var miniMode, out var targetWindowPtr)) return false;
        offset = 0;
        octave = 0;

        if (miniMode)
        {
            keynumber = ConvertMiniKeyNumber(keynumber, ref offset, ref octave);
        }

        SendAction(targetWindowPtr, 3, 1, 4, (ulong)keynumber);

        return true;

    }

    public static bool ReleaseKey(int keynumber)
    {
        if (!TargetWindowPtr(out var miniMode, out var targetWindowPtr)) return false;
        if (miniMode) keynumber = ConvertMiniKeyNumber(keynumber);

        SendAction(targetWindowPtr, 3, 2, 4, (ulong)keynumber);

        return true;

    }

    private static int ConvertMiniKeyNumber(int keynumber)
    {
        keynumber -= 12;
        switch (keynumber)
        {
            case < 0:
                keynumber += 12;
                break;
            case > 12:
                keynumber -= 12;
                break;
        }

        return keynumber;
    }

    private static int ConvertMiniKeyNumber(int keynumber, ref int offset, ref int octave)
    {
        keynumber -= 12;
        switch (keynumber)
        {
            case < 0:
                keynumber += 12;
                offset    =  -12;
                octave    =  -1;
                break;
            case > 12:
                keynumber -= 12;
                offset    =  12;
                octave    =  1;
                break;
        }

        return keynumber;
    }

    private static bool TargetWindowPtr(out bool miniMode, out nint targetWindowPtr)
    {
        targetWindowPtr = GetWindowByName("PerformanceMode");
        if (targetWindowPtr != nint.Zero)
        {
            miniMode = true;
            return true;
        }

        targetWindowPtr = GetWindowByName("PerformanceModeWide");
        if (targetWindowPtr != nint.Zero)
        {
            miniMode = false;
            return true;
        }

        miniMode = false;
        return false;
    }

    public static bool GuitarSwitchTone(int tone)
    {
        var ptr = GetWindowByName("PerformanceToneChange");
        if (ptr == nint.Zero) return false;

        SendAction(ptr, 3, 0, 3, (ulong)tone);
        return true;
    }

    public static bool BeginReadyCheck() => SendAction("PerformanceMetronome", 3, 2, 2, 0);
    public static bool ConfirmBeginReadyCheck() => SendAction("PerformanceReadyCheck", 3, 2);
    public static bool ConfirmReceiveReadyCheck() => SendAction("PerformanceReadyCheckReceive", 3, 2);

    public static string MainModuleRva(nint ptr)
    {
        var modules = Process.GetCurrentProcess().Modules;
        List<ProcessModule> mh = new();
        for (var i = 0; i < modules.Count; i++)
            mh.Add(modules[i]);

        mh.Sort((x, y) => x.BaseAddress > (long)y.BaseAddress ? -1 : 1);
        foreach (var module in mh.Where(module => module.BaseAddress <= (long)ptr))
        {
            return $"[{module.ModuleName}+0x{ptr - (long)module.BaseAddress:X}]";
        }
        return $"[0x{(long)ptr:X}]";
    }

    public static unsafe void PlayNote(int noteNum, bool on)
    {
        if (on)
        {
            if (Hypnotoad.AgentPerformance.noteNumber - 39 == noteNum)
                if (ReleaseKey(noteNum))
                    Hypnotoad.AgentPerformance.Struct->CurrentPressingNote = -100;

            if (PressKey(noteNum, ref Hypnotoad.AgentPerformance.Struct->NoteOffset, ref Hypnotoad.AgentPerformance.Struct->OctaveOffset))
            {
                Hypnotoad.AgentPerformance.Struct->CurrentPressingNote = noteNum + 39;
            }

        }
        else
        {
            if (Hypnotoad.AgentPerformance.Struct->CurrentPressingNote - 39 != noteNum)
                return;

            if (ReleaseKey(noteNum))
            {
                Hypnotoad.AgentPerformance.Struct->CurrentPressingNote = -100;
            }
        }
    }

}