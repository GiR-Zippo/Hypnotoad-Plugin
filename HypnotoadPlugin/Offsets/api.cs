/*
 * Copyright(c) 2024 GiR-Zippo, Meowchestra
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace HypnotoadPlugin.Offsets;

public class Api
{
    [PluginService]
    //[RequiredVersion("1.0")]
    public static DalamudPluginInterface PluginInterface { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IBuddyList BuddyList { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IChatGui ChatGui { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IClientState ClientState { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static ICommandManager CommandManager { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static ICondition Condition { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IGameInteropProvider GameInteropProvider { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IDataManager DataManager { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IFateTable FateTable { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IFlyTextGui FlyTextGui { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IFramework Framework { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IGameGui GameGui { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IGameNetwork GameNetwork { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IJobGauges JobGauges { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IKeyState KeyState { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static ILibcFunction LibcFunction { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IObjectTable ObjectTable { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IPartyFinderGui PartyFinderGui { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IPartyList PartyList { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static ISigScanner SigScanner { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static ITargetManager TargetManager { get; private set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static IToastGui ToastGui { get; private set; }
    
    [PluginService]
    //[RequiredVersion("1.0")]
    public static IPluginLog PluginLog { get; private set; }

    [PluginService]
    public static IGameConfig GameConfig { get; private set; }

    [PluginService]
    public static IGameLifecycle GameLifecycle { get; private set; }
}
