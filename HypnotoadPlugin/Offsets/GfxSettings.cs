/*
 * Copyright(c) 2022 Ori @MidiBard2
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace HypnotoadPlugin.Offsets;

internal static class GfxSettings
{

    internal class AgentConfigSystem : AgentInterface
    {
        private static unsafe ConfigModule* _configModule = Framework.Instance()->UIModule->GetConfigModule();
        public AgentConfigSystem(AgentInterface agentInterface) : base(agentInterface.Pointer, agentInterface.Id) { }

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
        public static unsafe AtkValue* GetOptionValue(ConfigOption option) => _configModule->GetValue(option);
        public static unsafe void SetOptionValue(ConfigOption option, int value) => _configModule->SetOption(option, value);
        public static unsafe void ToggleBoolOptionValue(ConfigOption option) => _configModule->SetOption(option, GetOptionValue(option)->Byte == 0 ? 1 : 0);

        private static int FPSInActive;
        private static int originalObjQuantity;
        private static int WaterWet_DX11;
        private static int OcclusionCulling_DX11;
        private static int LodType_DX11;
        private static int ReflectionType_DX11;
        private static int AntiAliasing_DX11;
        private static int TranslucentQuality_DX11;
        private static int GrassQuality_DX11;
        private static int ParallaxOcclusion_DX11;
        private static int Tessellation_DX11;
        private static int GlareRepresentation_DX11;
        private static int MapResolution_DX11;
        private static int ShadowVisibilityTypeSelf_DX11;
        private static int ShadowVisibilityTypeParty_DX11;
        private static int ShadowVisibilityTypeOther_DX11;
        private static int ShadowVisibilityTypeEnemy_DX11;
        private static int ShadowLOD_DX11;
        private static int ShadowTextureSizeType_DX11;
        private static int ShadowCascadeCountType_DX11;
        private static int ShadowSoftShadowType_DX11;
        private static int TextureFilterQuality_DX11;
        private static int TextureAnisotropicQuality_DX11;
        private static int PhysicsTypeSelf_DX11;
        private static int PhysicsTypeParty_DX11;
        private static int PhysicsTypeOther_DX11;
        private static int PhysicsTypeEnemy_DX11;
        private static int RadialBlur_DX11;
        private static int SSAO_DX11;
        private static int Glare_DX11;
        private static int DistortionWater_DX11;

        public static bool CheckLowSettings()
        {
            return originalObjQuantity == 4      &&
                   WaterWet_DX11 == 0            &&
                   OcclusionCulling_DX11 == 1    &&
                   ReflectionType_DX11 == 3      &&
                   GrassQuality_DX11 == 3        &&
                   SSAO_DX11 == 4;
        }

        public static unsafe void GetObjQuantity()
        {
            FPSInActive                    = _configModule->GetIntValue(ConfigOption.FPSInActive);
            originalObjQuantity            = _configModule->GetIntValue(ConfigOption.DisplayObjectLimitType);
            WaterWet_DX11                  = _configModule->GetIntValue(ConfigOption.WaterWet_DX11);
            OcclusionCulling_DX11          = _configModule->GetIntValue(ConfigOption.OcclusionCulling_DX11);
            LodType_DX11                   = _configModule->GetIntValue(ConfigOption.LodType_DX11);
            ReflectionType_DX11            = _configModule->GetIntValue(ConfigOption.ReflectionType_DX11);
            AntiAliasing_DX11              = _configModule->GetIntValue(ConfigOption.AntiAliasing_DX11);
            TranslucentQuality_DX11        = _configModule->GetIntValue(ConfigOption.TranslucentQuality_DX11);
            GrassQuality_DX11              = _configModule->GetIntValue(ConfigOption.GrassQuality_DX11);
            ParallaxOcclusion_DX11         = _configModule->GetIntValue(ConfigOption.ParallaxOcclusion_DX11);
            Tessellation_DX11              = _configModule->GetIntValue(ConfigOption.Tessellation_DX11);
            GlareRepresentation_DX11       = _configModule->GetIntValue(ConfigOption.GlareRepresentation_DX11);
            MapResolution_DX11             = _configModule->GetIntValue(ConfigOption.MapResolution_DX11);
            ShadowVisibilityTypeSelf_DX11  = _configModule->GetIntValue(ConfigOption.ShadowVisibilityTypeSelf_DX11);
            ShadowVisibilityTypeParty_DX11 = _configModule->GetIntValue(ConfigOption.ShadowVisibilityTypeParty_DX11);
            ShadowVisibilityTypeOther_DX11 = _configModule->GetIntValue(ConfigOption.ShadowVisibilityTypeOther_DX11);
            ShadowVisibilityTypeEnemy_DX11 = _configModule->GetIntValue(ConfigOption.ShadowVisibilityTypeEnemy_DX11);
            ShadowLOD_DX11                 = _configModule->GetIntValue(ConfigOption.ShadowLOD_DX11);
            ShadowTextureSizeType_DX11     = _configModule->GetIntValue(ConfigOption.ShadowTextureSizeType_DX11);
            ShadowCascadeCountType_DX11    = _configModule->GetIntValue(ConfigOption.ShadowCascadeCountType_DX11);
            ShadowSoftShadowType_DX11      = _configModule->GetIntValue(ConfigOption.ShadowSoftShadowType_DX11);
            TextureFilterQuality_DX11      = _configModule->GetIntValue(ConfigOption.TextureFilterQuality_DX11);
            TextureAnisotropicQuality_DX11 = _configModule->GetIntValue(ConfigOption.TextureAnisotropicQuality_DX11);
            PhysicsTypeSelf_DX11           = _configModule->GetIntValue(ConfigOption.PhysicsTypeSelf_DX11);
            PhysicsTypeParty_DX11          = _configModule->GetIntValue(ConfigOption.PhysicsTypeParty_DX11);
            PhysicsTypeOther_DX11          = _configModule->GetIntValue(ConfigOption.PhysicsTypeOther_DX11);
            PhysicsTypeEnemy_DX11          = _configModule->GetIntValue(ConfigOption.PhysicsTypeEnemy_DX11);
            RadialBlur_DX11                = _configModule->GetIntValue(ConfigOption.RadialBlur_DX11);
            SSAO_DX11                      = _configModule->GetIntValue(ConfigOption.SSAO_DX11);
            Glare_DX11                     = _configModule->GetIntValue(ConfigOption.Glare_DX11);
            DistortionWater_DX11           = _configModule->GetIntValue(ConfigOption.DistortionWater_DX11);
        }

        public static unsafe void SetMinimalObjQuantity()
        {
            _configModule->SetOption(ConfigOption.FPSInActive, 0);
            _configModule->SetOption(ConfigOption.DisplayObjectLimitType, 4);

            _configModule->SetOption(ConfigOption.WaterWet_DX11, 0);
            _configModule->SetOption(ConfigOption.OcclusionCulling_DX11, 1);
            _configModule->SetOption(ConfigOption.LodType_DX11, 1);
            _configModule->SetOption(ConfigOption.ReflectionType_DX11, 3);
            _configModule->SetOption(ConfigOption.AntiAliasing_DX11, 1);
            _configModule->SetOption(ConfigOption.TranslucentQuality_DX11, 1);
            _configModule->SetOption(ConfigOption.GrassQuality_DX11, 3);
            _configModule->SetOption(ConfigOption.ParallaxOcclusion_DX11, 1);
            _configModule->SetOption(ConfigOption.Tessellation_DX11, 1);
            _configModule->SetOption(ConfigOption.GlareRepresentation_DX11, 1);
            _configModule->SetOption(ConfigOption.MapResolution_DX11, 2);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeSelf_DX11, 1);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeParty_DX11, 1);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeOther_DX11, 1);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeEnemy_DX11, 1);
            _configModule->SetOption(ConfigOption.ShadowLOD_DX11, 1);
            _configModule->SetOption(ConfigOption.ShadowTextureSizeType_DX11, 2);
            _configModule->SetOption(ConfigOption.ShadowCascadeCountType_DX11, 2);
            _configModule->SetOption(ConfigOption.ShadowSoftShadowType_DX11, 1);
            _configModule->SetOption(ConfigOption.TextureFilterQuality_DX11, 2);
            _configModule->SetOption(ConfigOption.TextureAnisotropicQuality_DX11, 2);
            _configModule->SetOption(ConfigOption.PhysicsTypeSelf_DX11, 2);
            _configModule->SetOption(ConfigOption.PhysicsTypeParty_DX11, 2);
            _configModule->SetOption(ConfigOption.PhysicsTypeOther_DX11, 2);
            _configModule->SetOption(ConfigOption.PhysicsTypeEnemy_DX11, 2);
            _configModule->SetOption(ConfigOption.RadialBlur_DX11, 0);
            _configModule->SetOption(ConfigOption.SSAO_DX11, 4);
            _configModule->SetOption(ConfigOption.Glare_DX11, 2);
            _configModule->SetOption(ConfigOption.DistortionWater_DX11, 2);

        }

        public static unsafe void RestoreObjQuantity()
        {
            _configModule->SetOption(ConfigOption.FPSInActive,                      FPSInActive);
            _configModule->SetOption(ConfigOption.DisplayObjectLimitType,           originalObjQuantity);
            _configModule->SetOption(ConfigOption.WaterWet_DX11,                    WaterWet_DX11);
            _configModule->SetOption(ConfigOption.OcclusionCulling_DX11,            OcclusionCulling_DX11);
            _configModule->SetOption(ConfigOption.LodType_DX11,                     LodType_DX11);
            _configModule->SetOption(ConfigOption.ReflectionType_DX11,              ReflectionType_DX11);
            _configModule->SetOption(ConfigOption.AntiAliasing_DX11,                AntiAliasing_DX11);
            _configModule->SetOption(ConfigOption.TranslucentQuality_DX11,          TranslucentQuality_DX11);
            _configModule->SetOption(ConfigOption.GrassQuality_DX11,                GrassQuality_DX11);
            _configModule->SetOption(ConfigOption.ParallaxOcclusion_DX11,           ParallaxOcclusion_DX11);
            _configModule->SetOption(ConfigOption.Tessellation_DX11,                Tessellation_DX11);
            _configModule->SetOption(ConfigOption.GlareRepresentation_DX11,         GlareRepresentation_DX11);
            _configModule->SetOption(ConfigOption.MapResolution_DX11,               MapResolution_DX11);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeSelf_DX11,    ShadowVisibilityTypeSelf_DX11);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeParty_DX11,   ShadowVisibilityTypeParty_DX11);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeOther_DX11,   ShadowVisibilityTypeOther_DX11);
            _configModule->SetOption(ConfigOption.ShadowVisibilityTypeEnemy_DX11,   ShadowVisibilityTypeEnemy_DX11);
            _configModule->SetOption(ConfigOption.ShadowLOD_DX11,                   ShadowLOD_DX11);
            _configModule->SetOption(ConfigOption.ShadowTextureSizeType_DX11,       ShadowTextureSizeType_DX11);
            _configModule->SetOption(ConfigOption.ShadowCascadeCountType_DX11,      ShadowCascadeCountType_DX11);
            _configModule->SetOption(ConfigOption.ShadowSoftShadowType_DX11,        ShadowSoftShadowType_DX11);
            _configModule->SetOption(ConfigOption.TextureFilterQuality_DX11,        TextureFilterQuality_DX11);
            _configModule->SetOption(ConfigOption.TextureAnisotropicQuality_DX11,   TextureAnisotropicQuality_DX11);
            _configModule->SetOption(ConfigOption.PhysicsTypeSelf_DX11,             PhysicsTypeSelf_DX11);
            _configModule->SetOption(ConfigOption.PhysicsTypeParty_DX11,            PhysicsTypeParty_DX11);
            _configModule->SetOption(ConfigOption.PhysicsTypeOther_DX11,            PhysicsTypeOther_DX11);
            _configModule->SetOption(ConfigOption.PhysicsTypeEnemy_DX11,            PhysicsTypeEnemy_DX11);
            _configModule->SetOption(ConfigOption.RadialBlur_DX11,                  RadialBlur_DX11);
            _configModule->SetOption(ConfigOption.SSAO_DX11,                        SSAO_DX11);
            _configModule->SetOption(ConfigOption.Glare_DX11,                       Glare_DX11);
            _configModule->SetOption(ConfigOption.DistortionWater_DX11,             DistortionWater_DX11);


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
            PluginLog.Error(e.ToString());
        }
    }

    public static AgentManager Instance { get; } = new();

    internal AgentInterface FindAgentInterfaceById(int id) => AgentTable[id];

    internal AgentInterface FindAgentInterfaceByVtable(nint vtbl) => AgentTable.First(i => i.VTable == vtbl);
}