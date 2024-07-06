using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace HypnotoadPlugin.Offsets
{
    public static class Party
    {
        public static AutoSelectYesNo YesNoAddon { get; set; } = null;
        public unsafe static void PartyInvite(string message)
        {
            if (message == "")
            {
                Party.AcceptPartyInviteEnable();
                return;
            }
            string character = message.Split(';')[0];
            ushort homeWorldId = System.Convert.ToUInt16(message.Split(';')[1]);
            InfoProxyPartyInvite.Instance()->InviteToParty(0, character, (ushort)homeWorldId);
        }

        public unsafe static void AcceptPartyInviteEnable()
        {
            if (YesNoAddon != null)
                return;
            Api.PluginLog.Debug("Create new AcceptPartyInviteEnable");
            YesNoAddon = new AutoSelectYesNo();
            YesNoAddon.Enable();
        }

        public unsafe static void AcceptPartyInviteDisable()
        {
            if (YesNoAddon == null)
                return;
            YesNoAddon.Disable();
            YesNoAddon = null;
        }

        public static readonly List<Regex> LfgPatterns =
        [
            new Regex(@"Join .* party\?"),
            new Regex(@".*のパーティに参加します。よろしいですか？"),
            new Regex(@"Der Gruppe von .* beitreten\?"),
            new Regex(@"Rejoindre l'équipe de .*\?")
            // if someone could add the chinese and korean translations that'd be nice
        ];
    }

    public class AutoSelectYesNo
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
            if (Party.LfgPatterns.Any(r => r.IsMatch(text)))
            {
                SelectYes(addon);
                Party.AcceptPartyInviteDisable();
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
                        Marshal.FreeHGlobal(new IntPtr(atkValues[i].String));
                    }
                }
                Marshal.FreeHGlobal(new IntPtr(atkValues));
            }
        }

        internal static string GetSeStringText(Dalamud.Game.Text.SeStringHandling.SeString seString)
        {
            var pieces = seString.Payloads.OfType< Dalamud.Game.Text.SeStringHandling.Payloads.TextPayload >().Select(t => t.Text);
            var text = string.Join(string.Empty, pieces).Replace('\n', ' ').Trim();
            return text;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x10)]
        private struct AddonSelectYesNoOnSetupData
        {
            [FieldOffset(0x8)]
            public IntPtr TextPtr;
        }
    }
}
