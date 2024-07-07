/*
 * Copyright(c) 2024 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Common.Math;
using HypnotoadPlugin.Offsets;
using System.Linq;

namespace HypnotoadPlugin.Utils;

static internal class Misc
{
    internal static IGameObject GetNearestEntrance(out float Distance, bool bypassPredefined = false)
    {
        var currentDistance = float.MaxValue;
        IGameObject currentObject = null;

        foreach (var x in Api.Objects)
        {
            if (x.IsTargetable && Langstrings.Entrance.Any(r => r.IsMatch(x.Name.TextValue)))
            {
                var distance = Vector3.Distance(Api.ClientState.LocalPlayer.Position, x.Position);
                if (distance < currentDistance)
                {
                    currentDistance = distance;
                    currentObject = x;

                    Distance = currentDistance;
                    return currentObject;
                }
            }
        }
        Distance = currentDistance;
        return currentObject;
    }
}
