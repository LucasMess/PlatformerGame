using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private Tracker tracker = new Tracker();
        private List<Snapshot> snapshots = new List<Snapshot>();
        private List<Snapshot> drawableSnapshots = new List<Snapshot>();
        private const double TimeBetweenSnapshots = 10;
        private const int Capacity = (int)(1000 / TimeBetweenSnapshots);
        private const float MaxDistance = AdamGame.Tilesize * 5;
        private Timer snapshotTimer = new Timer();

        public bool IsRewinding { get; private set; } = false;

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
                    player.IsVisible = false;
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
                        player.SetPosition(snapshots[tracker.CurrentDraw].Position);
                        //player.SetVelX(snapshots[tracker.CurrentDraw].Velocity.X);
                        //player.SetVelY(snapshots[tracker.CurrentDraw].Velocity.Y);
                        snapshots[tracker.CurrentDraw].Opacity = 1f;
                        player.ComplexAnimation.UpdatePositionOnly(player);
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
