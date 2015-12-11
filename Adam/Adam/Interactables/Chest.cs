using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Adam;
using Adam.Misc;
using Adam.UI.Elements;

namespace Adam
{
    public class Chest
    {
        bool isOpen;
        SoundFx openSound;
        Rectangle collRectangle;
        Tile sourceTile;

        public bool IsGolden { get; set; }

        public Chest(Tile tile)
        {
            //hello
            openSound = new SoundFx("Sounds/Chest/open");
            collRectangle = new Rectangle(tile.drawRectangle.X, tile.drawRectangle.Y, Main.Tilesize * 2, Main.Tilesize);
            sourceTile = tile;
            sourceTile.OnTileUpdate += Update;
            sourceTile.OnTileDestroyed += SourceTile_OnTileDestroyed;
            sourceTile.AnimationStopped = true;
        }

        private void SourceTile_OnTileDestroyed(Tile t)
        {
            sourceTile.OnTileUpdate -= Update;
            sourceTile.OnTileDestroyed -= SourceTile_OnTileDestroyed;
        }

        public void Update(Tile t)
        {
            Player player = GameWorld.Instance.player;
            if (player.GetCollRectangle().Intersects(collRectangle) && !isOpen)
            {
                // If player presses open button, open chest.
                if (InputHelper.IsKeyDown(Keys.W))
                {
                    t.AnimationStopped = false;
                    Open();
                }
            }
        }

        void Open()
        {
            openSound.PlayOnce();
            isOpen = true;

            int maxGems = GameWorld.RandGen.Next(10, 20);
            for (int i = 0; i < maxGems; i++)
            {
                GameWorld.Instance.entities.Add(new Gem(collRectangle.Center.X, collRectangle.Center.Y));
            }
        }
    }
}
