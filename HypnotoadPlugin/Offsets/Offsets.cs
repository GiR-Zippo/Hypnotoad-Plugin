/*
 * Copyright(c) 2024 GiR-Zippo, Meowchestra, akira0245 @MidiBard, Ori @MidiBard2
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
*/

using Dalamud.Hooking;
using System;
using System.Diagnostics.CodeAnalysis;

namespace HypnotoadPlugin.Offsets;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]

public static partial class Chat
{
    private static class Signatures
    {
        internal const string SendChat = "48 89 5C 24 ?? 48 89 74 24 10 57 48 83 EC 20 48 8B F2 48 8B F9 45 84 C9"; //Client::UI::UIModule_ProcessChatBoxEntry
        internal const string SanitiseString = "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 0F B6 F0 E8 ?? ?? ?? ?? 48 8D 4D C0"; //Client::System::String::Utf8String_SanitizeString
    }
}

public static class Offsets
{
    [StaticAddress("48 8B C2 0F B6 15 ?? ?? ?? ?? F6 C2 01")]
    public static nint PerformanceStructPtr { get; private set; }

    [Function("48 89 6C 24 10 48 89 74 24 18 57 48 83 EC ?? 48 83 3D ?? ?? ?? ?? ?? 41 8B E8")]
    public static nint DoPerformAction { get; private set; }

    [Function("40 53 48 83 EC 20 48 8B D9 48 83 C1 78 E8 ? ? ? ? 48 8D 8B ? ? ? ? E8 ? ? ? ? 48 8D 53 20")]
    public static IntPtr NetworkEnsembleStart { get; private set; }
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
                Pipe.Client.WriteAsync(new IPCMessage
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
