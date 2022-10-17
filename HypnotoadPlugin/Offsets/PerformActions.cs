/*
 * Copyright(c) 2022 Ori @MidiBard2
 */

using Dalamud.Logging;
using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace HypnotoadPlugin;

static class PerformActions
{
    internal delegate void DoPerformActionDelegate(IntPtr performInfoPtr, uint instrumentId, int a3 = 0);
    private static DoPerformActionDelegate doPerformAction { get; } = Marshal.GetDelegateForFunctionPointer<DoPerformActionDelegate>(Offsets.DoPerformAction);
    public static void DoPerformAction(uint instrumentId)
    {
        PluginLog.Information($"[DoPerformAction] instrumentId: {instrumentId}");
        doPerformAction(Offsets.PerformanceStructPtr, instrumentId);
    }

    private static unsafe IntPtr GetWindowByName(string s) => (IntPtr)AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonByName(s);
    public static void SendAction(nint ptr, params ulong[] param)
    {
        if (param.Length % 2 != 0) throw new ArgumentException("The parameter length must be an integer multiple of 2.");
        if (ptr == IntPtr.Zero) throw new ArgumentException("input pointer is null");
        var paircount = param.Length / 2;
        unsafe
        {
            fixed (ulong* u = param)
            {
                AtkUnitBase.fpFireCallback((AtkUnitBase*)ptr, paircount, (AtkValue*)u, (void*)1);
            }
        }
    }

    public static bool SendAction(string name, params ulong[] param)
    {
        var ptr = GetWindowByName(name);
        if (ptr == IntPtr.Zero) return false;
        SendAction(ptr, param);
        return true;
    }

    public static unsafe bool ConfirmReceiveReadyCheck() => SendAction("PerformanceReadyCheckReceive", 3, 2);
}