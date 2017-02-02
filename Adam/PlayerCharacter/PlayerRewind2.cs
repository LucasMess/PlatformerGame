using Adam.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Adam.PlayerCharacter
{
    public class RewindTracker
    {
        public struct Snapshot
        {
            public Rectangle DrawRectangle;
            public Rectangle SourceRectangle;
            public Texture Texture;
            public Vector2 Velocity;
        }

        private List<Snapshot> snapshots = new List<Snapshot>();
        private const double TimeBetweenSnapshots = 10;
        private const int Capacity = (int)(1000 / TimeBetweenSnapshots);
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
            }

            Snapshot snap = new Snapshot();
            snap.Position = player.Position;
            snap.Velocity = player.GetVelocity();

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
            for (int i = 0; i < snapshots.Count; i++)
            {
                int curr = snapshots.Count - i - 1;
                Rectangle drawRect = player.ComplexAnimation.GetDrawRectangle();
                spriteBatch.Draw(player.Texture, new Rectangle((int)snapshots[curr].Position.X, (int)snapshots[curr].Position.Y, drawRect.Width, drawRect.Height), player.ComplexAnimation.GetSourceRectangle(), Color.LightBlue);
            }
        }

    }
}
