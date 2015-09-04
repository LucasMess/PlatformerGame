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

    public abstract class Obstacle : Entity
    {
        public bool IsTouchingPlayer { get; set; }
        public bool IsCollidable { get; set; }
        public DamageType CurrentDamageType;
        protected Player player;
        protected Rectangle attackBox;

        protected List<Particle> particles = new List<Particle>();

        public Obstacle()
        {

        }

        public override void Update()
        {

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
                    attackBox = new Rectangle(DrawRectangle.X + 8, DrawRectangle.Y + DrawRectangle.Height - 20, DrawRectangle.Width - 16, 10);
                    break;
                default:
                    break;
            }

            if (GameWorld.Instance.player.GetCollRectangle().Intersects(attackBox))
                IsTouchingPlayer = true;
            else IsTouchingPlayer = false;

        }

    }
}
