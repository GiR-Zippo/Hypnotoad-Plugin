using HypnotoadPlugin.Offsets;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace HypnotoadPlugin.GameFunctions;

public static class MiscGameFunctions
{
    public static void CharacterLogout()
    {
        Party.Instance.AcceptPartyInviteEnable();
        Api.Framework.RunOnTick(delegate
        {
            Chat.SendMessage("/logout");
        }, default(TimeSpan), 10, default(CancellationToken));
    }

    public static void GameShutdown()
    {
        Party.Instance.AcceptPartyInviteEnable();
        Api.Framework.RunOnTick(delegate
        {
            Chat.SendMessage("/shutdown");
        }, default(TimeSpan), 10, default(CancellationToken));
    }
}
