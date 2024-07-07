/*
 * Copyright(c) 2024 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.Utils;

namespace HypnotoadPlugin.GameFunctions;

public static class Party
{
    public static AutoSelect.AutoSelectYes YesNoAddon { get; set; } = null;
    public unsafe static void PartyInvite(string message)
    {
        if (message == "")
        {
            AcceptPartyInviteEnable();
            return;
        }
        string character = message.Split(';')[0];
        ushort homeWorldId = System.Convert.ToUInt16(message.Split(';')[1]);
        InfoProxyPartyInvite.Instance()->InviteToParty(0, character, homeWorldId);
    }

    public unsafe static void AcceptPartyInviteEnable()
    {
        if (YesNoAddon != null)
            return;
        Api.PluginLog.Debug("Create new AcceptPartyInviteEnable");
        YesNoAddon = new AutoSelect.AutoSelectYes();
        YesNoAddon.Enable();
    }

    public unsafe static void PromoteCharacter(string message)
    {
        Api.PluginLog.Debug(message);
        if (YesNoAddon != null)
            return;
        Api.PluginLog.Debug("Create new AcceptPromote");
        YesNoAddon = new AutoSelect.AutoSelectYes();
        YesNoAddon.Enable();

        Api.PluginLog.Debug(message);
        foreach (var i in GroupManager.Instance()->GetGroup()->PartyMembers)
        {
            if (i.NameString.StartsWith(message))
            {
                AgentPartyMember.Instance()->Promote(message, 0, i.ContentId);
                return;
            }
        }
    }

    public unsafe static void EnterHouse()
    {
        IGameObject entrance = Misc.GetNearestEntrance(out var distance);
        if (entrance != null && distance < 4.8f)
            TargetSystem.Instance()->InteractWithObject((GameObject*)entrance.Address, false);

        if (YesNoAddon != null)
            return;
        Api.PluginLog.Debug("Create new AcceptPartyInviteEnable");
        YesNoAddon = new AutoSelect.AutoSelectYes();
        YesNoAddon.Enable();
    }

    public unsafe static void AcceptDisable()
    {
        if (YesNoAddon == null)
            return;
        YesNoAddon.Disable();
        YesNoAddon = null;
    }

}
