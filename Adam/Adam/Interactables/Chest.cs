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
using Adam.Levels;
using Adam.Misc;
using Adam.UI.Elements;

namespace Adam
{
    public class Chest
    {
        bool _isOpen;
        SoundFx _openSound;
        Rectangle _collRectangle;
        Tile _sourceTile;

        public bool IsGolden { get; set; }

        public Chest(Tile tile)
        {
            //hello
            _openSound = new SoundFx("Sounds/Chest/open");
            _collRectangle = new Rectangle(tile.DrawRectangle.X, tile.DrawRectangle.Y, Main.Tilesize * 2, Main.Tilesize);
            _sourceTile = tile;
            _sourceTile.OnTileUpdate += Update;
            _sourceTile.OnTileDestroyed += SourceTile_OnTileDestroyed;
            _sourceTile.AnimationStopped = true;
        }

        private void SourceTile_OnTileDestroyed(Tile t)
        {
            _sourceTile.OnTileUpdate -= Update;
            _sourceTile.OnTileDestroyed -= SourceTile_OnTileDestroyed;
        }

        public void Update(Tile t)
        {
            Player player = GameWorld.Instance.Player;
            if (player.GetCollRectangle().Intersects(_collRectangle) && !_isOpen)
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
            _openSound.PlayOnce();
            _isOpen = true;

            int maxGems = GameWorld.RandGen.Next(10, 20);
            for (int i = 0; i < maxGems; i++)
            {
                GameWorld.Instance.Entities.Add(new Gem(_collRectangle.Center.X, _collRectangle.Center.Y));
            }
        }
    }
}
