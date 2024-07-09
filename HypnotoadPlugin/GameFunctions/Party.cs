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
using HypnotoadPlugin.Utils;
using System;

namespace HypnotoadPlugin.GameFunctions;

public class Party : IDisposable
{
    private static readonly Lazy<Party> LazyInstance = new(static () => new Party());

    private Party()
    {}

    public static Party Instance => LazyInstance.Value;

    private AutoSelect.AutoSelectYes YesNoAddon { get; set; } = null;

    public void Initialize()
    {
        YesNoAddon = new AutoSelect.AutoSelectYes();
    }

    public void Dispose()
    {
        YesNoAddon.Dispose();
        YesNoAddon = null;
    }

    public unsafe void PartyInvite(string message)
    {
        if (message == "")
        {
            AcceptPartyInviteEnable();
            return;
        }
        string character = message.Split(';')[0];
        ushort homeWorldId = Convert.ToUInt16(message.Split(';')[1]);
        InfoProxyPartyInvite.Instance()->InviteToParty(0, character, homeWorldId);
    }

    public unsafe void AcceptPartyInviteEnable()
    {
        YesNoAddon.Enable();
    }

    public unsafe void PromoteCharacter(string message)
    {
        YesNoAddon.Enable();

        foreach (var i in GroupManager.Instance()->GetGroup()->PartyMembers)
        {
            if (i.NameString.StartsWith(message) || i.NameString == message)
            {
                AgentPartyMember.Instance()->Promote(message, 0, i.ContentId);
                return;
            }
        }
    }

    public unsafe void EnterHouse()
    {
        IGameObject entrance = Misc.GetNearestEntrance(out var distance);
        if (entrance != null && distance < 4.8f)
            TargetSystem.Instance()->InteractWithObject((GameObject*)entrance.Address, false);

        YesNoAddon.Enable();
    }

    public unsafe void Teleport(bool showMenu)
    {
        if (showMenu)
            AgentTeleport.Instance()->Show();
        else
            YesNoAddon.Enable();
    }

    public unsafe void AcceptDisable()
    {
        YesNoAddon.Disable();
    }
}
