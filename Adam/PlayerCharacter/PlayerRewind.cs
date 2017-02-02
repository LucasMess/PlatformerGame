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
            public const float DeltaOpacity = .2f;
        }

        private List<Snapshot> snapshots = new List<Snapshot>();
        private List<Snapshot> drawableSnapshots = new List<Snapshot>();
        private const double TimeBetweenSnapshots = 10;
        private const int Capacity = (int)(1000 / TimeBetweenSnapshots);
        private const float MaxDistance = AdamGame.Tilesize * 5;
        private Timer snapshotTimer = new Timer();

        public void Reset()
        {
            snapshots = new List<Snapshot>();
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

        public Snapshot GetRewindSnapShot()
        {
            if (snapshots.Count == 0)
                return new Snapshot();
            return snapshots[0];
        }

        public void Draw(Player player, SpriteBatch spriteBatch)
        {
            for (int i = drawableSnapshots.Count - 1; i >= 0; i--)
            {
                Snapshot snap = drawableSnapshots[i];
                if (snap.Texture == null || snap.Opacity < 0)
                {
                    drawableSnapshots.Remove(snap);
                    continue;
                }


                if (snap.IsFacingRight)
                {
                    spriteBatch.Draw(snap.Texture, snap.DrawRectangle, snap.SourceRectangle, Color.White * snap.Opacity);
                }
                else
                {
                    spriteBatch.Draw(snap.Texture, snap.DrawRectangle, snap.SourceRectangle, Color.White * snap.Opacity, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                }

                snap.Opacity -= Snapshot.DeltaOpacity;
            }
        }

    }
}
