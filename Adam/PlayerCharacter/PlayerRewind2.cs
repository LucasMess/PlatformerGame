using Adam.Levels;
using Adam.Misc;
using Adam.Misc.Helpers;
using Adam.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Adam.PlayerCharacter
{
    public class RewindTracker
    {
        public class Snapshot
        {
            public Vector2 Position;
            public Rectangle DrawRectangle;
            public Rectangle SourceRectangle;
            public Texture2D Texture;
            public Vector2 Velocity;
            public bool IsFacingRight;
            public float Opacity = .5f;
            public const float DeltaOpacity = .3f;
        }

        private class Tracker
        {
            public bool IsDrawingBackwards = true;
            public int CurrentDraw = 0;
            public Timer DrawTimer = new Timer();
            public const double DrawInterval = 1;
        }

        private class RewindRing
        {
            static Texture2D _texture = ContentHelper.LoadTexture("Overlay/rewind_ring");
            static Vector2 _center = new Vector2(_texture.Width / 2, _texture.Height / 2);
            Timer _particleTimer = new Timer();

            int lastDeg = 0;
            const int changeInDeg = 20;

            public void Draw(Player player, SpriteBatch spriteBatch, double timeSinceLastRewind)
            {
                double opacity = (PlayerScript.RewindCooldown - timeSinceLastRewind) / PlayerScript.RewindCooldown;

                Overlay.RewindEffectSetOpacity((float)opacity);
                //spriteBatch.Draw(_texture, new Vector2(player.GetDrawRectangle().Center.X, player.GetDrawRectangle().Center.Y),
                   // new Rectangle(0, 0, _texture.Width, _texture.Height), Color.White * (float)opacity, 0, _center, 1, SpriteEffects.None, 0);

                _particleTimer.Increment();
                if (_particleTimer.TimeElapsedInMilliSeconds > 10)
                {
                    int radius = 100;
                    Vector2 position = new Vector2(player.GetDrawRectangle().Center.X, player.GetDrawRectangle().Center.Y);
                    lastDeg %= 360;
                    for (int i = lastDeg; i < lastDeg + changeInDeg; i += 1)
                    {
                        double rads = Math.PI * i / 180;
                        float x = (float)Math.Cos(rads);
                        float y = (float)Math.Sin(rads);
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.RewindFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)AdamGame.Random.NextDouble(), Color.White * (float)opacity);

                        x *= -1;
                        y *= -1;
                        GameWorld.ParticleSystem.Add(Particles.ParticleType.RewindFire, new Vector2(radius * x, radius * y) + position,
                            new Vector2(x, y) * 2 * (float)AdamGame.Random.NextDouble(), Color.White * (float)opacity);
                    }
                    lastDeg += changeInDeg;
                }
            }
        }

        private Tracker tracker = new Tracker();
        private List<Snapshot> snapshots = new List<Snapshot>();
        private List<Snapshot> drawableSnapshots = new List<Snapshot>();
        private const double TimeBetweenSnapshots = 10;
        private const int Capacity = (int)(1000 / TimeBetweenSnapshots);
        private const float MaxDistance = AdamGame.Tilesize * 5;
        private Timer snapshotTimer = new Timer();

        SoundFx _startSound = new SoundFx("Sounds/Player/rewind_start");
        SoundFx _stopSound = new SoundFx("Sounds/Player/rewind_stop");
        public bool IsRewinding { get; private set; } = false;
        RewindRing _rewindRing = new RewindRing();

        public void Reset()
        {
            snapshots = new List<Snapshot>();
            drawableSnapshots = new List<Snapshot>();
            snapshotTimer.Reset();
        }

        public void Update(Player player)
        {
            snapshotTimer.Increment();
            if (snapshotTimer.TimeElapsedInMilliSeconds > TimeBetweenSnapshots)
            {
                snapshotTimer.Reset();
                AddSnapshot(player);
            }
        }

        private void AddSnapshot(Player player)
        {
            if (IsRewinding) return;

            if (snapshots.Count > Capacity)
            {
                snapshots.RemoveAt(0);

                float dist = Vector2.DistanceSquared(snapshots[0].Position, player.Position);
                float opacity = dist / (MaxDistance * MaxDistance);
                if (opacity > .5f) opacity = .5f;
                snapshots[0].Opacity = opacity;
                drawableSnapshots.Add(snapshots[0]);
            }

            Snapshot snap = new Snapshot();
            snap.Position = player.Position;
            snap.Velocity = player.GetVelocity();
            snap.DrawRectangle = player.ComplexAnimation.GetDrawRectangle();
            snap.SourceRectangle = player.ComplexAnimation.GetSourceRectangle();
            snap.Texture = player.ComplexAnimation.GetCurrentTexture();
            snap.IsFacingRight = player.IsFacingRight;

            snapshots.Add(snap);
        }

        public Snapshot StartRewind()
        {
            IsRewinding = true;
            tracker = new Tracker();
            AdamGame.TimeFreeze.AddFrozenTime(1000);
            Overlay.ActivateRewindEffect();
            _startSound.Play();
            _stopSound.Play();

            if (snapshots.Count == 0)
                return new Snapshot();
            return snapshots[0];
        }

        private void DrawSnapshot(Snapshot snap, Player player, SpriteBatch spriteBatch)
        {
            if (snap.IsFacingRight)
            {
                spriteBatch.Draw(snap.Texture, snap.DrawRectangle, snap.SourceRectangle, Color.White * snap.Opacity);
            }
            else
            {
                spriteBatch.Draw(snap.Texture, snap.DrawRectangle, snap.SourceRectangle, Color.White * snap.Opacity, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }

        }

        public void Draw(Player player, SpriteBatch spriteBatch)
        {
            if (tracker.IsDrawingBackwards || !IsRewinding)
                _rewindRing.Draw(player, spriteBatch, player.rewindTimer.TimeElapsedInMilliSeconds);


            if (IsRewinding)
            {
                if (tracker.IsDrawingBackwards)
                {
                    tracker.DrawTimer.Increment();
                    for (int i = snapshots.Count - 1; i >= snapshots.Count - 1 - tracker.CurrentDraw; i--)
                    {
                        if (i % 5 == 0)
                        {
                            Snapshot snap = snapshots[i];
                            if (snap.Texture == null || snap.Opacity < 0)
                            {
                                drawableSnapshots.Remove(snap);
                                continue;
                            }
                            DrawSnapshot(snap, player, spriteBatch);
                        }
                    }
                    if (tracker.DrawTimer.TimeElapsedInMilliSeconds > Tracker.DrawInterval)
                    {
                        tracker.DrawTimer.Reset();
                        tracker.CurrentDraw += 5;
                    }
                    if (tracker.CurrentDraw >= snapshots.Count)
                    {
                        tracker.IsDrawingBackwards = false;
                        tracker.CurrentDraw = snapshots.Count;
                    }
                }
                else
                {

                    player.SetPosition(snapshots[0].Position);
                    player.SetVelX(snapshots[0].Velocity.X);
                    player.SetVelY(snapshots[0].Velocity.Y);
                    player.ComplexAnimation.RemoveAllFromQueue();
                    player.ComplexAnimation.UpdatePositionOnly(player);


                    //player.IsVisible = false;
                    for (int i = 0; i < tracker.CurrentDraw; i++)
                    {
                        if (i % 5 == 0)
                        {
                            Snapshot snap = snapshots[i];
                            if (snap.Texture == null || snap.Opacity < 0)
                            {
                                drawableSnapshots.Remove(snap);
                                continue;
                            }
                            DrawSnapshot(snap, player, spriteBatch);
                        }
                    }

                    tracker.DrawTimer.Increment();
                    if (tracker.DrawTimer.TimeElapsedInMilliSeconds > Tracker.DrawInterval)
                    {
                        tracker.DrawTimer.Reset();
                        tracker.CurrentDraw -= 5;
                        if (tracker.CurrentDraw < 0) tracker.CurrentDraw = 0;
                        //player.SetPosition(snapshots[tracker.CurrentDraw].Position);
                        //player.SetVelX(snapshots[tracker.CurrentDraw].Velocity.X);
                        //player.SetVelY(snapshots[tracker.CurrentDraw].Velocity.Y);
                        //snapshots[tracker.CurrentDraw].Opacity = 1f;
                        //player.ComplexAnimation.UpdatePositionOnly(player);
                    }
                    if (tracker.CurrentDraw <= 0)
                    {
                        IsRewinding = false;
                        player.IsVisible = true;
                        Reset();
                    }
                }
            }
            else
            {
                for (int i = drawableSnapshots.Count - 1; i >= 0; i--)
                {
                    Snapshot snap = drawableSnapshots[i];

                    if (snap.Texture == null || snap.Opacity < 0)
                    {
                        drawableSnapshots.Remove(snap);
                        continue;
                    }

                    DrawSnapshot(snap, player, spriteBatch);
                    snap.Opacity -= Snapshot.DeltaOpacity;
                }
            }
        }

    }
}
