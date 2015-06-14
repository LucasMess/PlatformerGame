using CodenameAdam;
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

    class Obstacle : Entity
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

        public virtual void Update(GameTime gameTime, Player player, Map map)
        {
            base.Update();
            this.player = player;

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
                    attackBox = new Rectangle(drawRectangle.X + 2, drawRectangle.Y + drawRectangle.Height - 5, drawRectangle.Width - 4, 10);
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
