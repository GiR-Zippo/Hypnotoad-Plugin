/*
 * Copyright(c) 2025 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.Utils;
using System;
using System.Text;
using System.Threading;

namespace HypnotoadPlugin.GameFunctions;

public class Party : IDisposable
{
    public enum AcceptFlags
    { 
        Accept_Teleport = 0b00000001,
        Accept_GroupInv = 0b00000010,
    }

    private static readonly Lazy<Party> LazyInstance = new(static () => new Party());

    private Party()
    {}

    public static Party Instance => LazyInstance.Value;

    private AutoSelect.AutoSelectYes YesNoAddon { get; set; } = null;

    private byte AcceptLock { get; set; } = 0;


    public void Initialize()
    {
        YesNoAddon = new AutoSelect.AutoSelectYes();
    }

    public void Dispose()
    {
        YesNoAddon.Dispose();
        YesNoAddon = null;
    }

    public bool IsAcceptFlagSet(AcceptFlags flag) => ((AcceptFlags)AcceptLock & flag) == flag;

    public void ClearFlags() => AcceptLock = 0;

    public void SetFlag(AcceptFlags flag) => AcceptLock |= (byte)flag;

    public unsafe void PartyInvite(string message)
    {
        if (message == "")
        {
            YesNoAddon.Enable();
            return;
        }
        string character = message.Split(';')[0];
        ushort homeWorldId = Convert.ToUInt16(message.Split(';')[1]);
        PartyInvite(character, homeWorldId);
    }

    public unsafe void PartyInvite(string character, ushort homeWorldId)
    {
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
        if (entrance != null && distance < 5.8f)
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

    public unsafe void PartyLeave()
    {
        YesNoAddon.Enable();

        Api.Framework.RunOnTick(delegate
        {
            Chat.SendMessage("/leave");
        }, default(TimeSpan), 10, default(CancellationToken));
    }

    public unsafe void AcceptDisable()
    {
        //if (AcceptLock == 0)
            YesNoAddon.Disable();
    }
                
    public unsafe void Kick(string name, ulong contentId)
    {
        Framework.Instance()->GetUIModule()->GetAgentModule()->GetAgentPartyMember()->Kick(name, 0,contentId);
    }
}

internal static class StringUtil
{
    internal static byte[] ToTerminatedBytes(this string s)
    {
        var utf8 = Encoding.UTF8;
        var bytes = new byte[utf8.GetByteCount(s) + 1];
        utf8.GetBytes(s, 0, s.Length, bytes, 0);
        bytes[^1] = 0;
        return bytes;
    }
}