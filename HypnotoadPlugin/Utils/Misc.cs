/*
 * Copyright(c) 2025 GiR-Zippo 
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Common.Math;
using HypnotoadPlugin.Offsets;
using System.Linq;

namespace HypnotoadPlugin.Utils;

static internal class Misc
{
    internal unsafe static void SetGameRenderSize(uint width, uint height)
    {
        Device* dev = Device.Instance();
        dev->NewWidth = (uint)width;
        dev->NewHeight = (uint)height;
        dev->RequestResolutionChange = 1;
    }

    internal static IGameObject GetNearestEntrance(out float Distance, bool bypassPredefined = false)
    {
        var currentDistance = float.MaxValue;
        IGameObject currentObject = null;

        foreach (var x in Api.Objects)
        {
            if (x.IsTargetable && Langstrings.Entrance.Any(r => r.IsMatch(x.Name.TextValue)))
            {
                var distance = Vector3.Distance(Api.GetLocalPlayer().Position, x.Position);
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
