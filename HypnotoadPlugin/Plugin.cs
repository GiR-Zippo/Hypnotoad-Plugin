using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using static HypnotoadPlugin.GfxSettings;

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

    public unsafe Hypnotoad(DalamudPluginInterface pluginInterface, CommandManager commandManager, ChatGui chatGui)
    {
        Api.Initialize(this, pluginInterface);
        this.PluginInterface = pluginInterface;
        this.CommandManager = commandManager;

        this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.Configuration.Initialize(this.PluginInterface);
        OffsetManager.Setup(SigScanner);

        // you might normally want to embed resources and load them from the manifest stream
        var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "toad.png");
        var goatImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);
        this.PluginUi = new PluginUI(this.Configuration, goatImage);

        this.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        AgentConfigSystem = new AgentConfigSystem(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.AgentConfigSystem));
        AgentPerformance = new AgentPerformance(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.AgentPerformance));
        Hypnotoad.AgentConfigSystem.GetObjQuantity();

        this.PluginInterface.UiBuilder.Draw += DrawUI;
        this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void Dispose()
    {
        Hypnotoad.AgentConfigSystem.RestoreObjQuantity();
        Hypnotoad.AgentConfigSystem.ApplyGraphicSettings();

        this.PluginUi.Dispose();
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
