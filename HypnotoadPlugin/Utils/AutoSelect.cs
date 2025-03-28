/*
 * Copyright(c) 2025 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.GameFunctions;

namespace HypnotoadPlugin.Utils;
public class AutoSelect
{
    public class AutoSelectYes : IDisposable
    {
        public AutoSelectYes()
        {
            Api.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", AddonSetup);
        }

        public void Dispose()
        {
            Api.AddonLifecycle.UnregisterListener(AddonSetup);
        }

        public void Enable()
        {
            listen = true;
        }

        public void Disable()
        {
            listen = false;
        }

        private bool listen { get; set; } = false;

        protected unsafe void AddonSetup(AddonEvent eventType, AddonArgs addonInfo)
        {
            if (!listen)
                return;

            var addon = (FFXIVClientStructs.FFXIV.Component.GUI.AtkUnitBase*)addonInfo.Addon;
            var text = addon->AtkValues[0].GetValueAsString();
            if (Langstrings.LfgPatterns.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
            if (Langstrings.LeavePartyPatterns.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
            else if (Langstrings.PromotePatterns.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
            else if (Langstrings.ConfirmHouseEntrance.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
            else if (Langstrings.ConfirmGroupTeleport.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
            else if (Langstrings.ConfirmLogout.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
            else if (Langstrings.ConfirmShutdown.Any(r => r.IsMatch(text)))
            {
                PerformActions.ClickYes();
                Party.Instance.AcceptDisable();
                return;
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x10)]
        private struct AddonSelectYesNoOnSetupData
        {
            [FieldOffset(0x8)]
            public nint TextPtr;
        }
    }
}

