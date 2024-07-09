using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using HypnotoadPlugin.Offsets;
using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Common.Math;
using HypnotoadPlugin.Utils;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling.Payloads;

namespace HypnotoadPlugin.GameFunctions;

public class FollowSystem : IDisposable
{
    internal bool Follow = false;
    internal bool Following = false;
    internal int FollowDistance = 2;
    internal string FollowTarget = "";
    internal uint HomeWorldId { get; set; } = 0;
    internal IGameObject FollowTargetObject = null;
    internal IPlayerCharacter TChar = null;
    private readonly OverrideMovement _overrideMovement;

    public FollowSystem(string targetName, uint homeWorldId)
    {
        FollowTarget = targetName;
        HomeWorldId = homeWorldId;
        _overrideMovement = new OverrideMovement();
        Api.Framework.Update += OnGameFrameworkUpdate;
    }

    private static IGameObject GetGameObjectFromName(string _objectName, uint _worldId)
    {
        var obj = Api.Objects.AsEnumerable().FirstOrDefault(s => s.Name.ToString().Equals(_objectName));
        var f = obj as IPlayerCharacter;
        if (f == null)
            return null;
        if (f.HomeWorld.Id == _worldId)
            return obj;
        return null;
    }

    public bool GetFollowTargetObject()
    {
        var ftarget = GetGameObjectFromName(FollowTarget, HomeWorldId);
        if (ftarget == null)
        {
            FollowTargetObject = null;
            if (Following)
                Stop();
            return false;
        }
        else
        {
            FollowTargetObject = ftarget;
            return true;
        }
    }

    private void MoveTo(Vector3 position, float precision = 0.1f)
    {
        if (_overrideMovement.Precision != precision)
            _overrideMovement.Precision = precision;

        _overrideMovement.DesiredPosition = position;
    }

    public void OnGameFrameworkUpdate(IFramework framework)
    {
        //If follow is not enabled clear TextColored's and return
        if (!Follow)
        {
            if (Following)
            {
                Stop();
                Following = false;
            }
            return;
        }

        //If LocalPlayer object is null return (we are not logged in or between zones etc..)
        if (Api.ClientState.LocalPlayer == null) return;

        //If followTarget is not empty GetFollowTargetObject then set our player variable and calculate the distance
        //between player and followTargetObject and if distance > followDistance move to the followTargetObject
        if (!string.IsNullOrEmpty(FollowTarget))
        {
            try
            {
                var player = Api.ClientState.LocalPlayer;
                if (!GetFollowTargetObject())
                    return;

                if (FollowTargetObject == null || player == null) return;

                if (!Follow)
                {
                    if (Following)
                    {
                        Stop();
                        Following = false;
                    }
                    return;
                }

                var distance = Vector3.Distance(player.Position, FollowTargetObject.Position);
                if ((distance > (FollowDistance + .1f)) && distance < 100)
                {
                    Following = true;
                    MoveTo(FollowTargetObject.Position, FollowDistance + .1f);
                }
                else if (Following)
                {
                    Following = false;
                    Stop();
                }
            }
            catch (Exception e)
            {
                Api.PluginLog.Error(e.ToString());
                if (Following)
                {
                    Following = false;
                    Stop();
                }
            }
        }
        else
        {
            if (Following)
            {
                Following = false;
                Stop();
            }
        }
    }

    private void StopAllMovement()
    {
        Follow = false;
        Following = false;
        Stop();
    }

    private void Stop()
    {
        if (_overrideMovement.DesiredPosition != null)
            _overrideMovement.DesiredPosition = null;
    }

    public void Dispose()
    {
        StopAllMovement();
        Api.Framework.Update -= OnGameFrameworkUpdate;
        _overrideMovement.Dispose();
    }

}

