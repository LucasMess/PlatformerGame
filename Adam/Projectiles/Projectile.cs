using Adam.Misc;
using Adam.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adam.Projectiles
{
    public abstract class Projectile
    {
        public enum Type
        {

        }

        public Type CurrentType;
        private Texture2D texture;
        private Vector2 velocity;
        private Rectangle collRectangle;
        private Rectangle drawRectangle;
        private Rectangle sourceRectangle;
        private ComplexAnimation complexAnimation;

        public Vector2 Position { get; set; }




        public virtual void OnPlayerTouch(Player Player)
        {

        }

        public Vector2 GetVelocity()
        {
            return velocity;
        }

        public void SetVelX(float x)
        {
            SetVelocity(x, GetVelocity().Y);
        }

        public void SetVelY(float y)
        {
            SetVelocity(GetVelocity().X, y);
        }

        public void SetVelocity(float x, float y)
        {
            SetVelocity(new Vector2(x, y));
        }

        public void SetVelocity(Vector2 vel)
        {
            velocity = vel;
        }

        public Vector2 GetPosition()
        {
            return new Vector2(collRectangle.X, collRectangle.Y);
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }
    }
}
