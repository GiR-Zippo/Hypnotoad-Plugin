/*
 * Copyright(c) 2023 GiR-Zippo, Meowchestra, Ori@MidiBard2
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Game.Config;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;

namespace HypnotoadPlugin.Offsets;

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
        private static uint FPSInActive;
        private static uint originalObjQuantity;
        private static uint WaterWet_DX11;
        private static uint OcclusionCulling_DX11;
        private static uint LodType_DX11;
        private static uint ReflectionType_DX11;
        private static uint AntiAliasing_DX11;
        private static uint TranslucentQuality_DX11;
        private static uint GrassQuality_DX11;
        private static uint ParallaxOcclusion_DX11;
        private static uint Tessellation_DX11;
        private static uint GlareRepresentation_DX11;
        private static uint MapResolution_DX11;
        private static uint ShadowVisibilityTypeSelf_DX11;
        private static uint ShadowVisibilityTypeParty_DX11;
        private static uint ShadowVisibilityTypeOther_DX11;
        private static uint ShadowVisibilityTypeEnemy_DX11;
        private static uint ShadowLOD_DX11;
        private static uint ShadowTextureSizeType_DX11;
        private static uint ShadowCascadeCountType_DX11;
        private static uint ShadowSoftShadowType_DX11;
        private static uint TextureFilterQuality_DX11;
        private static uint TextureAnisotropicQuality_DX11;
        private static uint PhysicsTypeSelf_DX11;
        private static uint PhysicsTypeParty_DX11;
        private static uint PhysicsTypeOther_DX11;
        private static uint PhysicsTypeEnemy_DX11;
        private static uint RadialBlur_DX11;
        private static uint SSAO_DX11;
        private static uint Glare_DX11;
        private static uint DistortionWater_DX11;
        private static uint SoundEnabled;

        /// <summary>
        /// Get the gfx settings and save them
        /// </summary>
        public static unsafe void GetSettings()
        {
            var configEntry = Framework.Instance()->SystemConfig.CommonSystemConfig.ConfigBase.ConfigEntry;

            FPSInActive                    = configEntry[(int)ConfigOption.FPSInActive].Value.UInt;
            originalObjQuantity            = configEntry[(int)ConfigOption.DisplayObjectLimitType].Value.UInt;
            WaterWet_DX11                  = configEntry[(int)ConfigOption.WaterWet_DX11].Value.UInt;
            OcclusionCulling_DX11          = configEntry[(int)ConfigOption.OcclusionCulling_DX11].Value.UInt;
            LodType_DX11                   = configEntry[(int)ConfigOption.LodType_DX11].Value.UInt;
            ReflectionType_DX11            = configEntry[(int)ConfigOption.ReflectionType_DX11].Value.UInt;
            AntiAliasing_DX11              = configEntry[(int)ConfigOption.AntiAliasing_DX11].Value.UInt;
            TranslucentQuality_DX11        = configEntry[(int)ConfigOption.TranslucentQuality_DX11].Value.UInt;
            GrassQuality_DX11              = configEntry[(int)ConfigOption.GrassQuality_DX11].Value.UInt;
            ParallaxOcclusion_DX11         = configEntry[(int)ConfigOption.ParallaxOcclusion_DX11].Value.UInt;
            Tessellation_DX11              = configEntry[(int)ConfigOption.Tessellation_DX11].Value.UInt;
            GlareRepresentation_DX11       = configEntry[(int)ConfigOption.GlareRepresentation_DX11].Value.UInt;
            MapResolution_DX11             = configEntry[(int)ConfigOption.MapResolution_DX11].Value.UInt;
            ShadowVisibilityTypeSelf_DX11  = configEntry[(int)ConfigOption.ShadowVisibilityTypeSelf_DX11].Value.UInt;
            ShadowVisibilityTypeParty_DX11 = configEntry[(int)ConfigOption.ShadowVisibilityTypeParty_DX11].Value.UInt;
            ShadowVisibilityTypeOther_DX11 = configEntry[(int)ConfigOption.ShadowVisibilityTypeOther_DX11].Value.UInt;
            ShadowVisibilityTypeEnemy_DX11 = configEntry[(int)ConfigOption.ShadowVisibilityTypeEnemy_DX11].Value.UInt;
            ShadowLOD_DX11                 = configEntry[(int)ConfigOption.ShadowLOD_DX11].Value.UInt;
            ShadowTextureSizeType_DX11     = configEntry[(int)ConfigOption.ShadowTextureSizeType_DX11].Value.UInt;
            ShadowCascadeCountType_DX11    = configEntry[(int)ConfigOption.ShadowCascadeCountType_DX11].Value.UInt;
            ShadowSoftShadowType_DX11      = configEntry[(int)ConfigOption.ShadowSoftShadowType_DX11].Value.UInt;
            TextureFilterQuality_DX11      = configEntry[(int)ConfigOption.TextureFilterQuality_DX11].Value.UInt;
            TextureAnisotropicQuality_DX11 = configEntry[(int)ConfigOption.TextureAnisotropicQuality_DX11].Value.UInt;
            PhysicsTypeSelf_DX11           = configEntry[(int)ConfigOption.PhysicsTypeSelf_DX11].Value.UInt;
            PhysicsTypeParty_DX11          = configEntry[(int)ConfigOption.PhysicsTypeParty_DX11].Value.UInt;
            PhysicsTypeOther_DX11          = configEntry[(int)ConfigOption.PhysicsTypeOther_DX11].Value.UInt;
            PhysicsTypeEnemy_DX11          = configEntry[(int)ConfigOption.PhysicsTypeEnemy_DX11].Value.UInt;
            RadialBlur_DX11                = configEntry[(int)ConfigOption.RadialBlur_DX11].Value.UInt;
            SSAO_DX11                      = configEntry[(int)ConfigOption.SSAO_DX11].Value.UInt;
            Glare_DX11                     = configEntry[(int)ConfigOption.Glare_DX11].Value.UInt;
            DistortionWater_DX11           = configEntry[(int)ConfigOption.DistortionWater_DX11].Value.UInt;
            SoundEnabled                   = configEntry[(int)ConfigOption.IsSndMaster].Value.UInt;
        }

        /// <summary>
        /// Restore the GFX settings
        /// </summary>
        public static unsafe void RestoreSettings()
        {
            var configEntry = Framework.Instance()->SystemConfig.CommonSystemConfig.ConfigBase.ConfigEntry;

            configEntry[(int)ConfigOption.FPSInActive].SetValueUInt(FPSInActive);
            configEntry[(int)ConfigOption.DisplayObjectLimitType].SetValueUInt(originalObjQuantity);
            configEntry[(int)ConfigOption.WaterWet_DX11].SetValueUInt(WaterWet_DX11);
            configEntry[(int)ConfigOption.OcclusionCulling_DX11].SetValueUInt(OcclusionCulling_DX11);
            configEntry[(int)ConfigOption.LodType_DX11].SetValueUInt(LodType_DX11);
            configEntry[(int)ConfigOption.ReflectionType_DX11].SetValueUInt(ReflectionType_DX11);
            configEntry[(int)ConfigOption.AntiAliasing_DX11].SetValueUInt(AntiAliasing_DX11);
            configEntry[(int)ConfigOption.TranslucentQuality_DX11].SetValueUInt(TranslucentQuality_DX11);
            configEntry[(int)ConfigOption.GrassQuality_DX11].SetValueUInt(GrassQuality_DX11);
            configEntry[(int)ConfigOption.ParallaxOcclusion_DX11].SetValueUInt(ParallaxOcclusion_DX11);
            configEntry[(int)ConfigOption.Tessellation_DX11].SetValueUInt(Tessellation_DX11);
            configEntry[(int)ConfigOption.GlareRepresentation_DX11].SetValueUInt(GlareRepresentation_DX11);
            configEntry[(int)ConfigOption.MapResolution_DX11].SetValueUInt(MapResolution_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeSelf_DX11].SetValueUInt(ShadowVisibilityTypeSelf_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeParty_DX11].SetValueUInt(ShadowVisibilityTypeParty_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeOther_DX11].SetValueUInt(ShadowVisibilityTypeOther_DX11);
            configEntry[(int)ConfigOption.ShadowVisibilityTypeEnemy_DX11].SetValueUInt(ShadowVisibilityTypeEnemy_DX11);
            configEntry[(int)ConfigOption.ShadowLOD_DX11].SetValueUInt(ShadowLOD_DX11);
            configEntry[(int)ConfigOption.ShadowTextureSizeType_DX11].SetValueUInt(ShadowTextureSizeType_DX11);
            configEntry[(int)ConfigOption.ShadowCascadeCountType_DX11].SetValueUInt(ShadowCascadeCountType_DX11);
            configEntry[(int)ConfigOption.ShadowSoftShadowType_DX11].SetValueUInt(ShadowSoftShadowType_DX11);
            configEntry[(int)ConfigOption.TextureFilterQuality_DX11].SetValueUInt(TextureFilterQuality_DX11);
            configEntry[(int)ConfigOption.TextureAnisotropicQuality_DX11].SetValueUInt(TextureAnisotropicQuality_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeSelf_DX11].SetValueUInt(PhysicsTypeSelf_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeParty_DX11].SetValueUInt(PhysicsTypeParty_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeOther_DX11].SetValueUInt(PhysicsTypeOther_DX11);
            configEntry[(int)ConfigOption.PhysicsTypeEnemy_DX11].SetValueUInt(PhysicsTypeEnemy_DX11);
            configEntry[(int)ConfigOption.RadialBlur_DX11].SetValueUInt(RadialBlur_DX11);
            configEntry[(int)ConfigOption.SSAO_DX11].SetValueUInt(SSAO_DX11);
            configEntry[(int)ConfigOption.Glare_DX11].SetValueUInt(Glare_DX11);
            configEntry[(int)ConfigOption.DistortionWater_DX11].SetValueUInt(DistortionWater_DX11);
            configEntry[(int)ConfigOption.IsSndMaster].SetValueUInt(SoundEnabled);
        }
        #endregion

        #region GfxConfig
        /// <summary>
        /// Basic check if GFX settings are low
        /// </summary>
        /// <returns></returns>
        public static bool CheckLowSettings()
        {
            return originalObjQuantity == 4 &&
                   WaterWet_DX11 == 0 &&
                   OcclusionCulling_DX11 == 1 &&
                   ReflectionType_DX11 == 3 &&
                   GrassQuality_DX11 == 3 &&
                   SSAO_DX11 == 4;
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
        #endregion
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
