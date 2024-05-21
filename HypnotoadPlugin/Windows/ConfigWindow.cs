using Dalamud.Interface.Windowing;
using HypnotoadPlugin.Offsets;
using ImGuiNET;
using System;

namespace HypnotoadPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow(Hypnotoad plugin) : base(
        "A Wonderful Configuration Window",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
    }

    public void Dispose() 
    { 

    }

    public override void Draw()
    {
        if (ImGui.Button("Connect"))
        {
            Api.PluginLog.Debug("config");
        }
    }
}