/*
 * Copyright(c) 2024 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.GameFunctions;

namespace HypnotoadPlugin.Utils;
public class AutoSelect
{
    public class AutoSelectYes
    {
        public void Enable()
        {
            Api.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", AddonSetup);
        }

        public void Disable()
        {
            Api.AddonLifecycle.UnregisterListener(AddonSetup);
        }

        protected unsafe void AddonSetup(AddonEvent eventType, AddonArgs addonInfo)
        {
            var addon = (AtkUnitBase*)addonInfo.Addon;
            var dataPtr = (AddonSelectYesNoOnSetupData*)addon;
            if (dataPtr == null)
                return;

            var text = GetSeStringText(MemoryHelper.ReadSeStringNullTerminated(new nint(addon->AtkValues[0].String)));
            if (Langstrings.LfgPatterns.Any(r => r.IsMatch(text)))
            {
                SelectYes(addon);
                Party.AcceptDisable();
                return;
            }
            else if (Langstrings.PromotePatterns.Any(r => r.IsMatch(text)))
            {
                SelectYes(addon);
                Party.AcceptDisable();
                return;
            }
            else if (Langstrings.ConfirmHouseEntrance.Any(r => r.IsMatch(text)))
            {
                SelectYes(addon);
                Party.AcceptDisable();
                return;
            }
            else if (Langstrings.ConfirmGroupTeleport.Any(r => r.IsMatch(text)))
            {
                SelectYes(addon);
                Party.AcceptDisable();
                return;
            }
        }

        public static unsafe bool SelectYes(AtkUnitBase* addon)
        {
            if (addon == null) return false;
            GenerateCallback(addon, 0);
            addon->Close(false);
            return true;
        }

        public static unsafe void GenerateCallback(AtkUnitBase* unitBase, params object[] values)
        {
            if (unitBase == null) throw new Exception("Null UnitBase");
            var atkValues = (AtkValue*)Marshal.AllocHGlobal(values.Length * sizeof(AtkValue));
            if (atkValues == null) return;
            try
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var v = values[i];
                    switch (v)
                    {
                        case uint uintValue:
                            atkValues[i].Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.UInt;
                            atkValues[i].UInt = uintValue;
                            break;
                        case int intValue:
                            atkValues[i].Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int;
                            atkValues[i].Int = intValue;
                            break;
                        case float floatValue:
                            atkValues[i].Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Float;
                            atkValues[i].Float = floatValue;
                            break;
                        case bool boolValue:
                            atkValues[i].Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Bool;
                            atkValues[i].Byte = (byte)(boolValue ? 1 : 0);
                            break;
                        case string stringValue:
                            {
                                atkValues[i].Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.String;
                                var stringBytes = Encoding.UTF8.GetBytes(stringValue);
                                var stringAlloc = Marshal.AllocHGlobal(stringBytes.Length + 1);
                                Marshal.Copy(stringBytes, 0, stringAlloc, stringBytes.Length);
                                Marshal.WriteByte(stringAlloc, stringBytes.Length, 0);
                                atkValues[i].String = (byte*)stringAlloc;
                                break;
                            }
                        default:
                            throw new ArgumentException($"Unable to convert type {v.GetType()} to AtkValue");
                    }
                }
                unitBase->FireCallback((ushort)values.Length, atkValues);
            }
            finally
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (atkValues[i].Type == FFXIVClientStructs.FFXIV.Component.GUI.ValueType.String)
                    {
                        Marshal.FreeHGlobal(new nint(atkValues[i].String));
                    }
                }
                Marshal.FreeHGlobal(new nint(atkValues));
            }
        }

        internal static string GetSeStringText(Dalamud.Game.Text.SeStringHandling.SeString seString)
        {
            var pieces = seString.Payloads.OfType<Dalamud.Game.Text.SeStringHandling.Payloads.TextPayload>().Select(t => t.Text);
            var text = string.Join(string.Empty, pieces).Replace('\n', ' ').Trim();
            return text;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x10)]
        private struct AddonSelectYesNoOnSetupData
        {
            [FieldOffset(0x8)]
            public nint TextPtr;
        }
    }
}
