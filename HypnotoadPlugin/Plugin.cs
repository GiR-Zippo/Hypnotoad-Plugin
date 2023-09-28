/*
 * Copyright(c) 2023 GiR-Zippo, Meowchestra 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using HypnotoadPlugin.Network;
using HypnotoadPlugin.Offsets;
using static HypnotoadPlugin.Offsets.GameSettings;

namespace HypnotoadPlugin;

public class Hypnotoad : IDalamudPlugin
{
    //public static XivCommonBase CBase;
    public string Name => "Hypnotoad";

    private const string commandName = "/hypnotoad";

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private Configuration Configuration { get; init; }
    private PluginUI PluginUi { get; init; }
    internal static AgentConfigSystem AgentConfigSystem { get; set; }
    internal static AgentPerformance AgentPerformance { get; set; }
    internal static EnsembleManager EnsembleManager { get; set; }

    [PluginService]
    public static SigScanner SigScanner { get; private set; }

    public Hypnotoad(DalamudPluginInterface pluginInterface, ChatGui chatGui, DataManager data, CommandManager commandManager, ClientState clientState, PartyList partyList)
    {
        Api.Initialize(this, pluginInterface);
        PluginInterface = pluginInterface;
        CommandManager  = commandManager;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        OffsetManager.Setup(SigScanner);

        CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        AgentConfigSystem = new AgentConfigSystem(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.Offsets.AgentConfigSystem));
        AgentPerformance  = new AgentPerformance(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.Offsets.AgentPerformance));
        EnsembleManager   = new EnsembleManager();

        Collector.Instance.Initialize(data, clientState, partyList);

        AgentConfigSystem.GetSettings();

        //NetworkReader.Initialize();

        // you might normally want to embed resources and load them from the manifest stream
        PluginUi = new PluginUI(Configuration);

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose()
    {
        //NetworkReader.Dispose();
        AgentConfigSystem.RestoreSettings();
        AgentConfigSystem.ApplyGraphicSettings();
        EnsembleManager.Dispose();
        Collector.Instance.Dispose();

        PluginUi.Dispose();
        CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        PluginUi.Visible = true;
    }

    private void DrawUI()
    {
        PluginUi.Draw();
    }

    private void DrawConfigUI()
    {
        PluginUi.Visible = true;
    }
}