using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.IO;
using System.Windows;
using XivCommon;
using static HypnotoadPlugin.GfxSettings;

namespace HypnotoadPlugin;

public class TestPlugin : IDalamudPlugin
{
    public static XivCommonBase CBase;
    public string Name => "Hypnotoad";

    private const string commandName = "/hypnotoad";

    private DalamudPluginInterface PluginInterface { get; init; }
    private CommandManager CommandManager { get; init; }
    private Configuration Configuration { get; init; }
    private PluginUI PluginUi { get; init; }
    internal static AgentConfigSystem AgentConfigSystem { get; set; }

    [PluginService]
    //[RequiredVersion("1.0")]
    public static SigScanner SigScanner { get; private set; }

    public unsafe TestPlugin(DalamudPluginInterface pluginInterface, CommandManager commandManager, ChatGui chatGui)
    {
        api.Initialize(this, pluginInterface);
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;

        this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.Configuration.Initialize(this.PluginInterface);
        OffsetManager.Setup(SigScanner);
        try
        {
            CBase = new XivCommonBase();
        }
        catch (Exception ex)
        {
            PluginLog.LogError($"exception: {ex}");
        }

        // you might normally want to embed resources and load them from the manifest stream
        var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "toad.png");
        var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);
        this.PluginUi = new PluginUI(this.Configuration, goatImage);

        this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        AgentConfigSystem = new AgentConfigSystem(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.AgentConfigSystem));
        TestPlugin.AgentConfigSystem.GetObjQuantity();

        this.PluginInterface.UiBuilder.Draw += DrawUI;
        this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose()
    {
        TestPlugin.AgentConfigSystem.RestoreObjQuantity();
        TestPlugin.AgentConfigSystem.ApplyGraphicSettings();

        this.PluginUi.Dispose();
        CBase.Dispose();
        this.CommandManager.RemoveHandler(commandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just display our main ui
        this.PluginUi.Visible = true;
    }

    private void DrawUI()
    {
        this.PluginUi.Draw();
    }

    private void DrawConfigUI()
    {
        this.PluginUi.Visible = true;
    }
}
