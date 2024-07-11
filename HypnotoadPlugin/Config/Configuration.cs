/*
 * Copyright(c) 2024 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using Dalamud.Configuration;
using Dalamud.Plugin;

namespace HypnotoadPlugin.Config;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool Autoconnect { get; set; } = true;

    // the below exist just to make saving less cumbersome

    [NonSerialized]
    private IDalamudPluginInterface pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }
}