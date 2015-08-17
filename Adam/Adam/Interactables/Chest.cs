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

namespace Adam
{
    public class Chest 
    {
        bool isOpen;
        SoundFx openSound;
        Rectangle collRectangle;

        public bool IsGolden { get; set; }

        public Chest(Tile tile)
        {
            openSound = new SoundFx("Sounds/Chest/open");
            collRectangle = new Rectangle(tile.drawRectangle.X, tile.drawRectangle.Y, Main.Tilesize * 2, Main.Tilesize);
        }

        public void Update()
        {
            Player player = GameWorld.Instance.player;
            if (player.collRectangle.Intersects(collRectangle)&& !isOpen)
            {               
                Open();
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
