using System.IO;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using HypnotoadPlugin.Offsets;
using static HypnotoadPlugin.Offsets.GfxSettings;

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

    [PluginService]
    public static SigScanner SigScanner { get; private set; }

    public Hypnotoad(DalamudPluginInterface pluginInterface, CommandManager commandManager, ChatGui chatGui)
    {
        Api.Initialize(this, pluginInterface);
        PluginInterface = pluginInterface;
        CommandManager  = commandManager;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        OffsetManager.Setup(SigScanner);

        // you might normally want to embed resources and load them from the manifest stream
        var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "toad.png");
        var goatImage = PluginInterface.UiBuilder.LoadImage(imagePath);
        PluginUi = new PluginUI(Configuration, goatImage);

        CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        AgentConfigSystem = new AgentConfigSystem(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.Offsets.AgentConfigSystem));
        AgentPerformance  = new AgentPerformance(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.Offsets.AgentPerformance));
        AgentConfigSystem.GetObjQuantity();

        PluginInterface.UiBuilder.Draw         += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        //NetworkReader.Initialize();
    }

    public void Dispose()
    {
        //NetworkReader.Dispose();
        AgentConfigSystem.RestoreObjQuantity();
        AgentConfigSystem.ApplyGraphicSettings();

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