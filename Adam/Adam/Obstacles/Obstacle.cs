using Adam;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Obstacles
{
    public enum DamageType
    {
        All,
        Top,
        Sides,
        Bottom,
    }

    public class Obstacle : Entity
    {
        public bool IsTouching { get; set; }
        public bool IsCollidable { get; set; }
        public DamageType CurrentDamageType;
        protected Player player;
        protected Rectangle attackBox;

        protected List<Particle> particles = new List<Particle>();

        public Obstacle()
        {

        }

        public virtual void Update(GameTime gameTime, Player player, GameWorld map)
        {
            this.gameWorld = map;
            this.player = player;

            base.Update();

            //Defines the attack box depending on what the Damage type is.
            switch (CurrentDamageType)
            {
                case DamageType.All:
                    attackBox = collRectangle;
                    break;
                case DamageType.Top:
                    break;
                case DamageType.Sides:
                    break;
                case DamageType.Bottom:
                    attackBox = new Rectangle(drawRectangle.X + 8, drawRectangle.Y + drawRectangle.Height - 20, drawRectangle.Width - 16, 10);
                    break;
                default:
                    break;
            }

            if (player.collRectangle.Intersects(attackBox))
                IsTouching = true;
            else IsTouching = false;

        }

    }
}
