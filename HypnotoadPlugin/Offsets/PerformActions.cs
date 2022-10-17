/*
 * Copyright(c) 2022 Ori @MidiBard2
 */

using Dalamud.Logging;
using System;
using System.Runtime.InteropServices;

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
}