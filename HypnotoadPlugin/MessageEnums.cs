﻿/*
 * Copyright(c) 2024 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HypnotoadPlugin;

public enum MessageType
{
    None                    = 0,
    Handshake               = 1,
    Version                 = 2,

    SetGfx                  = 10,   //Get<->Set
    NameAndHomeWorld        = 11,   //Get
    MasterSoundState        = 12,   //Set<->Get
    MasterVolume            = 13,   //Set<->Get
    VoiceSoundState         = 14,   //Set<->Get
    EffectsSoundState       = 15,   //Set<->Get

    Instrument              = 20,
    NoteOn                  = 21,
    NoteOff                 = 22,
    ProgramChange           = 23,

    StartEnsemble           = 30,   //Get<->Set
    AcceptReply             = 31,
    PerformanceModeState    = 32,   //Get

    Chat                    = 40,

    NetworkPacket           = 50,
    ExitGame                = 55
}

public readonly struct ChatMessageChannelType
{
    public static readonly ChatMessageChannelType None  = new("None",   0x0000, "");
    public static readonly ChatMessageChannelType Say   = new("Say",    0x000A, "/s");
    public static readonly ChatMessageChannelType Yell  = new("Yell",   0x001E, "/y");
    public static readonly ChatMessageChannelType Shout = new("Shout",  0x000B, "/sh");
    public static readonly ChatMessageChannelType Party = new("Party",  0x000E, "/p");
    public static readonly ChatMessageChannelType FC    = new("FC",     0x0018, "/fc");
    public static readonly IReadOnlyList<ChatMessageChannelType> All = new ReadOnlyCollection<ChatMessageChannelType>(new List<ChatMessageChannelType>
    {
        None,
        Say,
        Yell,
        Shout,
        Party,
        FC
    });

    public string Name { get; }
    public int ChannelCode { get; }
    public string ChannelShortCut { get; }

    private ChatMessageChannelType(string name, int channelCode, string channelShortCut)
    {
        Name            = name;
        ChannelCode     = channelCode;
        ChannelShortCut = channelShortCut;
    }

    public static ChatMessageChannelType ParseByChannelCode(int channelCode)
    {
        TryParseByChannelCode(channelCode, out var result);
        return result;
    }

    public static bool TryParseByChannelCode(int channelCode, out ChatMessageChannelType result)
    {
        if (All.Any(x => x.ChannelCode.Equals(channelCode)))
        {
            result = All.First(x => x.ChannelCode.Equals(channelCode));
            return true;
        }
        result = None;
        return false;
    }
}