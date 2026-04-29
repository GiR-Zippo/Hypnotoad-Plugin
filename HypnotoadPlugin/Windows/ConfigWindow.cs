using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;

namespace HypnotoadPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow(Hypnotoad plugin) : base(
        "A Wonderful Configuration Window")
        //ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
    }

    public void Dispose() 
    {

    }

    public override void Draw()
    {
        if (ImGui.Button("test"))
        {

        }
        if (ImGui.Button("st"))
        {
        }
    }

    public unsafe static void TestCommand()
    {
        
    }
}