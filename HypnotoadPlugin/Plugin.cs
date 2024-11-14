/*
 * Copyright(c) 2024 GiR-Zippo, Meowchestra
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using HypnotoadPlugin.Config;
using HypnotoadPlugin.GameFunctions;
using HypnotoadPlugin.IPC;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.Windows;

using static HypnotoadPlugin.Offsets.GameSettings;

namespace HypnotoadPlugin;

public class Hypnotoad : IDalamudPlugin
{
    public string Name => "Hypnotoad";

    //The windows
    public WindowSystem WindowSystem = new("Hypnotoad");
    private MainWindow PluginUi { get; init; }
    private ConfigWindow ConfigUi { get; set; }

    private const string commandName = "/hypnotoad";
    private static IDalamudPluginInterface PluginInterface { get; set; }
    private Configuration Configuration { get; init; }
    internal static AgentPerformance AgentPerformance { get; set; }
    internal static EnsembleManager EnsembleManager { get; set; }

    public Api api { get; set; }
    private readonly IPCProvider _ipc;

    public Hypnotoad(IDalamudPluginInterface pluginInterface, IChatGui chatGui, IDataManager data, ICommandManager commandManager, IClientState clientState, IPartyList partyList)
    {
        api = pluginInterface.Create<Api>();
        PluginInterface = pluginInterface;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        OffsetManager.Setup(Api.SigScanner);

        Api.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        AgentPerformance = new AgentPerformance(AgentId.PerformanceMode);
        EnsembleManager = new EnsembleManager();
        Party.Instance.Initialize();

        Collector.Instance.Initialize(data, clientState, partyList);

        AgentConfigSystem.GetSettings(GameSettingsTables.Instance.StartupTable);
        AgentConfigSystem.GetSettings(GameSettingsTables.Instance.CustomTable);

        //NetworkReader.Initialize();

        // you might normally want to embed resources and load them from the manifest stream
        PluginUi = new MainWindow(this, Configuration);
        ConfigUi = new ConfigWindow(this);

        WindowSystem.AddWindow(PluginUi);
        WindowSystem.AddWindow(ConfigUi);

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += UiBuilder_DrawConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += UiBuilder_OpenMainUi;

        AgentConfigSystem.LoadConfig();
        Api.ClientState.Login += OnLogin;
        Api.ClientState.Logout += OnLogout;

        _ipc = new IPCProvider(this);
    }

    private void OnLogin()
    {
        AgentConfigSystem.LoadConfig();
    }

    private void OnLogout(int type, int code)
    {
        AgentConfigSystem.RestoreSettings(GameSettingsTables.Instance.StartupTable);
    }

    public void Dispose()
    {
        _ipc.Dispose();
        MovementFactory.Instance.Dispose();
        Party.Instance.Dispose();
        Api.ClientState.Login -= OnLogin;
        Api.ClientState.Logout -= OnLogout;
        //NetworkReader.Dispose();
        AgentConfigSystem.RestoreSettings(GameSettingsTables.Instance.StartupTable);
        EnsembleManager.Dispose();
        Collector.Instance.Dispose();

        this.WindowSystem.RemoveAllWindows();
        PluginUi.Dispose();
        ConfigUi.Dispose();

        Api.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        PluginUi.IsOpen = !PluginUi.IsOpen;
    }

    private void DrawUI()
    {
        this.WindowSystem.Draw();
        PluginUi.Update(); //update the mainwindow... for the msg queue
    }

    private void UiBuilder_OpenMainUi()
    {
        PluginUi.IsOpen = !PluginUi.IsOpen;
    }

    private void UiBuilder_DrawConfigUI()
    {
        ConfigUi.IsOpen = !ConfigUi.IsOpen;
    }
}