/*
 * Copyright(c) 2024 GiR-Zippo, Meowchestra
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace HypnotoadPlugin.Offsets;

public class Api
{
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; }
    [PluginService] public static IBuddyList Buddies { get; private set; }
    [PluginService] public static IChatGui Chat { get; private set; }
    [PluginService] public static IClientState ClientState { get; private set; }
    [PluginService] public static ICommandManager CommandManager { get; private set; }
    [PluginService] public static ICondition Condition { get; private set; }
    [PluginService] public static IDataManager Data { get; private set; }
    [PluginService] public static IFateTable Fates { get; private set; }
    [PluginService] public static IFlyTextGui FlyText { get; private set; }
    [PluginService] public static IFramework Framework { get; private set; }
    [PluginService] public static IGameGui GameGui { get; private set; }
    [PluginService] public static IJobGauges Gauges { get; private set; }
    [PluginService] public static IKeyState KeyState { get; private set; }
    [PluginService] public static IObjectTable Objects { get; private set; }
    [PluginService] public static IPartyFinderGui PfGui { get; private set; }
    [PluginService] public static IPartyList Party { get; private set; }
    [PluginService] public static ISigScanner SigScanner { get; private set; }
    [PluginService] public static ITargetManager Targets { get; private set; }
    [PluginService] public static IToastGui Toasts { get; private set; }
    [PluginService] public static IGameConfig GameConfig { get; private set; }
    [PluginService] public static IGameLifecycle GameLifecycle { get; private set; }
    [PluginService] public static IGamepadState GamepadState { get; private set; }
    [PluginService] public static IDtrBar DtrBar { get; private set; }
    [PluginService] public static IDutyState DutyState { get; private set; }
    [PluginService] public static IGameInteropProvider Hook { get; private set; }
    [PluginService] public static ITextureProvider Texture { get; private set; }
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; private set; }
    [PluginService] public static IAetheryteList AetheryteList { get; private set; }
    [PluginService] public static IAddonEventManager AddonEventManager { get; private set; }
    [PluginService] public static IGameInventory GameInventory { get; private set; }
    [PluginService] public static ITextureSubstitutionProvider TextureSubstitution { get; private set; }
    [PluginService] public static ITitleScreenMenu TitleScreenMenu { get; private set; }
    [PluginService] public static INotificationManager NotificationManager { get; private set; }
    [PluginService] public static IContextMenu ContextMenu { get; private set; }
    [PluginService] public static IMarketBoard MarketBoard { get; private set; }
    [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; }
    [PluginService] public static IPluginLog PluginLog { get; private set; }

    internal static bool IsInitialized = false;

    /// <summary>
    /// Get the local player - has to run in tick
    /// </summary>
    /// <returns></returns>
    public static IPlayerCharacter GetLocalPlayer()
    {
        return Api.Framework.RunOnTick(delegate
            {
                return Api.ClientState?.LocalPlayer;
            }, default(TimeSpan), 0, default(CancellationToken)).Result;
    }

    public static void Init(IDalamudPluginInterface pi)
    {
        if (IsInitialized)
        {
            PluginLog.Debug("Services already initialized, skipping");
        }
        IsInitialized = true;
        try
        {
            pi.Create<Api>();
        }
        catch
        {
            PluginLog.Error("Services already initialized, skipping");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Safe(Action a, bool suppressErrors = false)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            if (!suppressErrors) PluginLog.Error($"{e.Message}\n{e.StackTrace ?? ""}");
        }
    }
}
