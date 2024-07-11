using HypnotoadPlugin.Offsets;
using HypnotoadPlugin.Utils;
using Navmesh;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

/*********************************************************/
/***                 CLEAN UP THIS MESS                ***/
/*********************************************************/


namespace HypnotoadPlugin.GameFunctions
{
    public class MovementFactory : IDisposable
    {
        private static readonly Lazy<MovementFactory> LazyInstance = new(static () => new MovementFactory());

        private CancellationTokenSource cancelMovementToken = new CancellationTokenSource();

        private MovementFactory()
        {

        }

        public static MovementFactory Instance => LazyInstance.Value;

        CameraUtil cam { get; set; } = null;
        OverrideMovement move { get; set; } = null;

        Vector3 DesiredPosition { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);
        Angle DesiredRotation = new Angle();

        public void Initialize(Vector3 position, float rotation)
        {
            DesiredPosition = position;
            DesiredRotation = new Angle(rotation);

            Api.PluginLog.Debug(DesiredRotation.ToString());
        }

        public void MoveTo(string data)
        {
            float[] newPosCoordinates = data.Split(";")[0].Replace("<", "").Replace(">", "").Split(new string[] { ". " }, StringSplitOptions.None).Select(x => float.Parse(x)).ToArray();
            Vector3 newpos = new Vector3(newPosCoordinates[0], newPosCoordinates[1], newPosCoordinates[2]);
            float newRot = float.Parse(data.Split(";")[1]);
            if (newpos == Api.ClientState.LocalPlayer.Position)
                return;
            DesiredPosition = newpos;
            DesiredRotation = new Angle(newRot);
            Move();
        }

        public void Move()
        {
            FollowSystem.StopFollow();

            move = new OverrideMovement();
            move.Precision = 0.05f;
            move.DesiredPosition = DesiredPosition;

            cam = new CameraUtil();
            cam.DesiredAzimuth = DesiredRotation;
            cam.Enabled = true;
            
            cancelMovementToken = new CancellationTokenSource();
            Task.Factory.StartNew(() => RunMoveTask(cancelMovementToken.Token), TaskCreationOptions.LongRunning);
        }

        public void StopMovement()
        {
            cleanup();
            cancelMovementToken.Cancel();
        }

        public void cleanup()
        {
            if (cam != null)
            {
                cam.Enabled = false;
                cam.Dispose();
                cam = null;
            }
            if (move != null)
            {
                move.Enabled = false;
                move.Dispose();
                move = null;
            }
            Api.PluginLog.Debug("break itz");
        }

        private async Task RunMoveTask(CancellationToken token)
        {
            if (move == null)
                return;

            if (!move.Enabled)
                move.Enabled = true;

            while (!token.IsCancellationRequested)
            {
                if (token.IsCancellationRequested)
                {
                    cleanup();
                    Api.PluginLog.Debug("Clean itz");
                    break;
                }

                if (move != null)
                    if (move.Enabled)
                    {
                        var dist = move.DesiredPosition - Api.ClientState.LocalPlayer.Position;
                        if (dist.LengthSquared() <= move.Precision * move.Precision)
                        {
                            move.Enabled = false;
                            move.Dispose();
                            move = null;

                            await Task.Delay(300, token).ContinueWith(static tsk => { }, token);
                            Api.PluginLog.Debug(Api.ClientState.LocalPlayer.Rotation.ToString());
                            Api.PluginLog.Debug(DesiredRotation.Rad.ToString());

                            cam.Enabled = false;
                            cam.Dispose();
                            cam = null;
                            break;
                        }
                    }
                await Task.Delay(50, token).ContinueWith(static tsk => { }, token);
                Api.PluginLog.Debug("break itz");
            }
            cleanup();
        }

        public void Dispose()
        {
            if (cam != null)
            {
                cam.Enabled = false;
                cam.Dispose();
            }
            if (move != null)
            {
                move.Enabled = false;
                move.Dispose();
            }

        }

    }
}
