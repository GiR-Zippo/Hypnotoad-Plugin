﻿/*
 * Copyright(c) 2023 GiR-Zippo, Meowchestra, Ori@MidiBard2
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Game.Config;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Newtonsoft.Json;

namespace HypnotoadPlugin.Offsets;

[Serializable]
public class GameSettingsVarTable
{
    public uint FPSInActive { get; set; }
    public uint originalObjQuantity { get; set; }
    public uint WaterWet_DX11 { get; set; }
    public uint OcclusionCulling_DX11 { get; set; }
    public uint LodType_DX11 { get; set; }
    public uint ReflectionType_DX11 { get; set; }
    public uint AntiAliasing_DX11 { get; set; }
    public uint TranslucentQuality_DX11 { get; set; }
    public uint GrassQuality_DX11 { get; set; }
    public uint ParallaxOcclusion_DX11 { get; set; }
    public uint Tessellation_DX11 { get; set; }
    public uint GlareRepresentation_DX11 { get; set; }
    public uint MapResolution_DX11 { get; set; }
    public uint ShadowVisibilityTypeSelf_DX11 { get; set; }
    public uint ShadowVisibilityTypeParty_DX11 { get; set; }
    public uint ShadowVisibilityTypeOther_DX11 { get; set; }
    public uint ShadowVisibilityTypeEnemy_DX11 { get; set; }
    public uint ShadowLOD_DX11 { get; set; }
    public uint ShadowTextureSizeType_DX11 { get; set; }
    public uint ShadowCascadeCountType_DX11 { get; set; }
    public uint ShadowSoftShadowType_DX11 { get; set; }
    public uint TextureFilterQuality_DX11 { get; set; }
    public uint TextureAnisotropicQuality_DX11 { get; set; }
    public uint PhysicsTypeSelf_DX11 { get; set; }
    public uint PhysicsTypeParty_DX11 { get; set; }
    public uint PhysicsTypeOther_DX11 { get; set; }
    public uint PhysicsTypeEnemy_DX11 { get; set; }
    public uint RadialBlur_DX11 { get; set; }
    public uint SSAO_DX11 { get; set; }
    public uint Glare_DX11 { get; set; }
    public uint DistortionWater_DX11 { get; set; }
    public uint SoundEnabled { get; set; }
}

public sealed class GameSettingsTables
{
    private static GameSettingsTables instance = null;
    private static readonly object padlock = new object();
    public GameSettingsVarTable StartupTable { get; set; } = new GameSettingsVarTable();
    public GameSettingsVarTable CustomTable { get; set; } = new GameSettingsVarTable();
    GameSettingsTables()
    {
    }

    public static GameSettingsTables Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GameSettingsTables();
                }
                return instance;
            }
        }
    }
}

internal static class GameSettings
{
    /// <summary>
    /// The agent for the client config
    /// </summary>
    internal class AgentConfigSystem : AgentInterface
    {
        public AgentConfigSystem(AgentInterface agentInterface) : base(agentInterface.Pointer, agentInterface.Id) { }

        /// <summary>
        /// Apply Settings
        /// </summary>
        public unsafe void ApplyGraphicSettings()
        {
            void OnToast(ref SeString message, ref ToastOptions options, ref bool handled)
            {
                handled            =  true;
                Api.ToastGui.Toast -= OnToast;
            }

            var refreshConfigGraphicState = (delegate* unmanaged<nint, long>)Offsets.ApplyGraphicConfigsFunc;
            var result = refreshConfigGraphicState(Pointer);
            Api.ToastGui.Toast += OnToast;
        }

        #region Get/Restore config
        /// <summary>
        /// Get the gfx settings and save them
        /// </summary>
        public static unsafe void GetSettings(GameSettingsVarTable varTable)
        {
            var configEntry = Framework.Instance()->SystemConfig.CommonSystemConfig.ConfigBase.ConfigEntry;

            varTable.FPSInActive                    = configEntry[(int)ConfigOption.FPSInActive].Value.UInt;
            varTable.originalObjQuantity            = configEntry[(int)ConfigOption.DisplayObjectLimitType].Value.UInt;
            varTable.WaterWet_DX11                  = configEntry[(int)ConfigOption.WaterWet_DX11].Value.UInt;
            varTable.OcclusionCulling_DX11          = configEntry[(int)ConfigOption.OcclusionCulling_DX11].Value.UInt;
            varTable.LodType_DX11                   = configEntry[(int)ConfigOption.LodType_DX11].Value.UInt;
            varTable.ReflectionType_DX11            = configEntry[(int)ConfigOption.ReflectionType_DX11].Value.UInt;
            varTable.AntiAliasing_DX11              = configEntry[(int)ConfigOption.AntiAliasing_DX11].Value.UInt;
            varTable.TranslucentQuality_DX11        = configEntry[(int)ConfigOption.TranslucentQuality_DX11].Value.UInt;
            varTable.GrassQuality_DX11              = configEntry[(int)ConfigOption.GrassQuality_DX11].Value.UInt;
            varTable.ParallaxOcclusion_DX11         = configEntry[(int)ConfigOption.ParallaxOcclusion_DX11].Value.UInt;
            varTable.Tessellation_DX11              = configEntry[(int)ConfigOption.Tessellation_DX11].Value.UInt;
            varTable.GlareRepresentation_DX11       = configEntry[(int)ConfigOption.GlareRepresentation_DX11].Value.UInt;
            varTable.MapResolution_DX11             = configEntry[(int)ConfigOption.MapResolution_DX11].Value.UInt;
            varTable.ShadowVisibilityTypeSelf_DX11  = configEntry[(int)ConfigOption.ShadowVisibilityTypeSelf_DX11].Value.UInt;
            varTable.ShadowVisibilityTypeParty_DX11 = configEntry[(int)ConfigOption.ShadowVisibilityTypeParty_DX11].Value.UInt;
            varTable.ShadowVisibilityTypeOther_DX11 = configEntry[(int)ConfigOption.ShadowVisibilityTypeOther_DX11].Value.UInt;
            varTable.ShadowVisibilityTypeEnemy_DX11 = configEntry[(int)ConfigOption.ShadowVisibilityTypeEnemy_DX11].Value.UInt;
            varTable.ShadowLOD_DX11                 = configEntry[(int)ConfigOption.ShadowLOD_DX11].Value.UInt;
            varTable.ShadowTextureSizeType_DX11     = configEntry[(int)ConfigOption.ShadowTextureSizeType_DX11].Value.UInt;
            varTable.ShadowCascadeCountType_DX11    = configEntry[(int)ConfigOption.ShadowCascadeCountType_DX11].Value.UInt;
            varTable.ShadowSoftShadowType_DX11      = configEntry[(int)ConfigOption.ShadowSoftShadowType_DX11].Value.UInt;
            varTable.TextureFilterQuality_DX11      = configEntry[(int)ConfigOption.TextureFilterQuality_DX11].Value.UInt;
            varTable.TextureAnisotropicQuality_DX11 = configEntry[(int)ConfigOption.TextureAnisotropicQuality_DX11].Value.UInt;
            varTable.PhysicsTypeSelf_DX11           = configEntry[(int)ConfigOption.PhysicsTypeSelf_DX11].Value.UInt;
            varTable.PhysicsTypeParty_DX11          = configEntry[(int)ConfigOption.PhysicsTypeParty_DX11].Value.UInt;
            varTable.PhysicsTypeOther_DX11          = configEntry[(int)ConfigOption.PhysicsTypeOther_DX11].Value.UInt;
            varTable.PhysicsTypeEnemy_DX11          = configEntry[(int)ConfigOption.PhysicsTypeEnemy_DX11].Value.UInt;
            varTable.RadialBlur_DX11                = configEntry[(int)ConfigOption.RadialBlur_DX11].Value.UInt;
            varTable.SSAO_DX11                      = configEntry[(int)ConfigOption.SSAO_DX11].Value.UInt;
            varTable.Glare_DX11                     = configEntry[(int)ConfigOption.Glare_DX11].Value.UInt;
            varTable.DistortionWater_DX11           = configEntry[(int)ConfigOption.DistortionWater_DX11].Value.UInt;
            varTable.SoundEnabled                   = configEntry[(int)ConfigOption.IsSndMaster].Value.UInt;
        }

        /// <summary>
        /// Restore the GFX settings
        /// </summary>
        public static unsafe void RestoreSettings(GameSettingsVarTable varTable)
        {
            var configEntry = Framework.Instance()->SystemConfig.CommonSystemConfig.ConfigBase.ConfigEntry;

            configEntry[(int)ConfigOption.FPSInActive].SetValueUInt(varTable.FPSInActive);
            configEntry[(int)ConfigOption.DisplayObjectLimitType].SetValueUInt(varTable.originalObjQuantity);
            configEntry[(int)ConfigOption.WaterWet_DX11].SetValueUInt(varTable.WaterWet_DX11);
            configEntry[(int)ConfigOption.OcclusionCulling_DX11].SetValueUInt(varTable.OcclusionCulling_DX11);
            configEntry[(int)ConfigOption.LodType_DX11].SetValueUInt(varTable.LodType_DX11);
            configEntry[(int)ConfigOption.ReflectionType_DX11].SetValueUInt(varTable.ReflectionType_DX11);
            configEntry[(int)ConfigOption.AntiAliasing_DX11].SetValueUInt(varTable.AntiAliasing_DX11);
            configEntry[(int)ConfigOption.TranslucentQuality_DX11].SetValueUInt(varTable.TranslucentQuality_DX11);
            configEntry[(int)ConfigOption.GrassQuality_DX11].SetValueUInt(varTable.GrassQuality_DX11);
            configEntry[(int)ConfigOption.ParallaxOcclusion_DX11].SetValueUInt(varTable.ParallaxOcclusion_DX11);
            configEntry[(int)ConfigOption.Tessellation_DX11].SetValueUInt(varTable.Tessellation_DX11);
            configEntry[(int)ConfigOption.GlareRepresentation_DX11].SetValueUInt(varTable.GlareRepresentation_DX11);
            configEntry[(int)ConfigOption.MapResolution_DX11].SetValueUInt(varTable.MapResolution_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeSelf_DX11].SetValueUInt(varTable.ShadowVisibilityTypeSelf_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeParty_DX11].SetValueUInt(varTable.ShadowVisibilityTypeParty_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeOther_DX11].SetValueUInt(varTable.ShadowVisibilityTypeOther_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeEnemy_DX11].SetValueUInt(varTable.ShadowVisibilityTypeEnemy_DX11);
            configEntry[(int)ConfigOption.ShadowLOD_DX11].SetValueUInt(varTable.ShadowLOD_DX11);
            configEntry[(int)ConfigOption.ShadowTextureSizeType_DX11].SetValueUInt(varTable.ShadowTextureSizeType_DX11);
            configEntry[(int)ConfigOption.ShadowCascadeCountType_DX11].SetValueUInt(varTable.ShadowCascadeCountType_DX11);
            configEntry[(int)ConfigOption.ShadowSoftShadowType_DX11].SetValueUInt(varTable.ShadowSoftShadowType_DX11);
            configEntry[(int)ConfigOption.TextureFilterQuality_DX11].SetValueUInt(varTable.TextureFilterQuality_DX11);
            configEntry[(int)ConfigOption.TextureAnisotropicQuality_DX11].SetValueUInt(varTable.TextureAnisotropicQuality_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeSelf_DX11].SetValueUInt(varTable.PhysicsTypeSelf_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeParty_DX11].SetValueUInt(varTable.PhysicsTypeParty_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeOther_DX11].SetValueUInt(varTable.PhysicsTypeOther_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeEnemy_DX11].SetValueUInt(varTable.PhysicsTypeEnemy_DX11);
            configEntry[(int)ConfigOption.RadialBlur_DX11].SetValueUInt(varTable.RadialBlur_DX11);
            configEntry[(int)ConfigOption.SSAO_DX11].SetValueUInt(varTable.SSAO_DX11);
            configEntry[(int)ConfigOption.Glare_DX11].SetValueUInt(varTable.Glare_DX11);
            configEntry[(int)ConfigOption.DistortionWater_DX11].SetValueUInt(varTable.DistortionWater_DX11);
            configEntry[(int)ConfigOption.IsSndMaster].SetValueUInt(varTable.SoundEnabled);
        }
        #endregion

        #region GfxConfig
        /// <summary>
        /// Basic check if GFX settings are low
        /// </summary>
        /// <returns></returns>
        public static bool CheckLowSettings(GameSettingsVarTable varTable)
        {
            return varTable.originalObjQuantity == 4 &&
                   varTable.WaterWet_DX11 == 0 &&
                   varTable.OcclusionCulling_DX11 == 1 &&
                   varTable.ReflectionType_DX11 == 3 &&
                   varTable.GrassQuality_DX11 == 3 &&
                   varTable.SSAO_DX11 == 4;
        }

        /// <summary>
        /// Set the GFX to minimal
        /// </summary>
        public static unsafe void SetMinimalGfx()
        {
            var configEntry = Framework.Instance()->SystemConfig.CommonSystemConfig.ConfigBase.ConfigEntry;

            configEntry[(int)ConfigOption.FPSInActive].SetValueUInt(0);
            configEntry[(int)ConfigOption.DisplayObjectLimitType].SetValueUInt(4);
            configEntry[(int)ConfigOption.WaterWet_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.OcclusionCulling_DX11].SetValueUInt(1);
            configEntry[(int)ConfigOption.LodType_DX11].SetValueUInt(1);
            configEntry[(int)ConfigOption.ReflectionType_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.AntiAliasing_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.TranslucentQuality_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.GrassQuality_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ParallaxOcclusion_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.Tessellation_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.GlareRepresentation_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.MapResolution_DX11].SetValueUInt(2);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeSelf_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeParty_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeOther_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeEnemy_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ShadowLOD_DX11].SetValueUInt(1);
            configEntry[(int)ConfigOption.ShadowTextureSizeType_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ShadowCascadeCountType_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.ShadowSoftShadowType_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.TextureFilterQuality_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.TextureAnisotropicQuality_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.PhysicsTypeSelf_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.PhysicsTypeParty_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.PhysicsTypeOther_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.PhysicsTypeEnemy_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.RadialBlur_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.SSAO_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.Glare_DX11].SetValueUInt(0);
            configEntry[(int)ConfigOption.DistortionWater_DX11].SetValueUInt(0);
        }
        #endregion

        #region Mute/Unmute MasterSound
        /// <summary>
        /// Mutes/Unmutes the sound
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetMasterSoundEnable(bool enabled)
        {
            Api.GameConfig.Set(SystemConfigOption.IsSndMaster, !enabled);
        }

        public static bool GetMasterSoundEnable()
        {
            return Api.GameConfig.TryGet(SystemConfigOption.IsSndMaster, out bool isSndMaster) && isSndMaster;
        }

        /// <summary>
        /// Gets/Sets the master-volume
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetMasterSoundVolume(short value)
        {
            Api.GameConfig.Set(SystemConfigOption.SoundMaster, (uint)value);
        }

        public static int GetMasterSoundVolume()
        {
            return Api.GameConfig.TryGet(SystemConfigOption.SoundMaster, out uint isSndMaster) ? (int)isSndMaster : -1;
        }

        /// <summary>
        /// Mutes/Unmutes the voices
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetVoiceSoundEnable(bool enabled)
        {
            Api.GameConfig.Set(SystemConfigOption.IsSndVoice, !enabled);
        }

        public static bool GetVoiceSoundEnable()
        {
            return Api.GameConfig.TryGet(SystemConfigOption.IsSndVoice, out bool isSndMaster) && isSndMaster;
        }

        /// <summary>
        /// Mutes/Unmutes the sound effects
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetEffectsSoundEnable(bool enabled)
        {
            Api.GameConfig.Set(SystemConfigOption.IsSndSe, !enabled);
        }

        public static bool GetEffectsSoundEnable()
        {
            return Api.GameConfig.TryGet(SystemConfigOption.IsSndSe, out bool isSndMaster) && isSndMaster;
        }
        #endregion


        public static void LoadConfig()
        {
            if (!Api.ClientState.IsLoggedIn)
                return;

            string file = $"{Api.PluginInterface.GetPluginConfigDirectory()}\\{Api.ClientState.LocalPlayer.Name}-({Api.ClientState.LocalPlayer.HomeWorld.GameData.Name}).json";
            if (!File.Exists(file))
                return;

            GameSettingsTables.Instance.CustomTable = JsonConvert.DeserializeObject<GameSettingsVarTable>(File.ReadAllText(file));
            RestoreSettings(GameSettingsTables.Instance.CustomTable);
            Hypnotoad.AgentConfigSystem.ApplyGraphicSettings();
        }

        public static void SaveConfig()
        {
            if (!Api.ClientState.IsLoggedIn)
                return;

            //Save the config
            GetSettings(GameSettingsTables.Instance.CustomTable);
            string jsonString = JsonConvert.SerializeObject(GameSettingsTables.Instance.CustomTable);
            File.WriteAllText($"{Api.PluginInterface.GetPluginConfigDirectory()}\\{Api.ClientState.LocalPlayer.Name}-({Api.ClientState.LocalPlayer.HomeWorld.GameData.Name}).json",
                              JsonConvert.SerializeObject(GameSettingsTables.Instance.CustomTable));
        }
    }
}

public unsafe class AgentInterface
{
    public nint Pointer { get; }
    public nint VTable { get; }
    public int Id { get; }
    public FFXIVClientStructs.FFXIV.Component.GUI.AgentInterface* Struct => (FFXIVClientStructs.FFXIV.Component.GUI.AgentInterface*)Pointer;

    public AgentInterface(nint pointer, int id)
    {
        Pointer = pointer;
        Id      = id;
        VTable  = Marshal.ReadIntPtr(Pointer);
    }

    public override string ToString()
    {
        return $"{Id} {(long)Pointer:X} {(long)VTable:X}";
    }
}

internal unsafe class AgentManager
{
    internal List<AgentInterface> AgentTable { get; } = new(400);

    private AgentManager()
    {
        try
        {
            var instance = Framework.Instance();
            var agentModule = instance->UIModule->GetAgentModule();
            var i = 0;
            foreach (var pointer in agentModule->AgentsSpan)
                AgentTable.Add(new AgentInterface((nint)pointer.Value, i++));
        }
        catch (Exception e)
        {
            Api.PluginLog.Error(e.ToString());
        }
    }

    public static AgentManager Instance { get; } = new();

    internal AgentInterface FindAgentInterfaceByVtable(nint vtbl) => AgentTable.First(i => i.VTable == vtbl);
}
