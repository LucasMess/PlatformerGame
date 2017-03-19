using Adam.Levels;
using Microsoft.Xna.Framework;
using System;

namespace Adam.Network.Packets
{

    [Serializable]
    public class EntityPacket : DataPacket
    {
        string entityId;

        Type type;
        float positionX;
        float positionY;
        float velocityX;
        float velocityY;
        bool _isDead;

        long TimeStamp
        {
            get; set;
        }

        /// <summary>
        /// Creates a new entity packet from the given entity.
        /// </summary>
        /// <returns></returns>
        public EntityPacket(string id, Entity entity)
        {
            entityId = id;
            positionX = entity.Position.X;
            positionY = entity.Position.Y;
            velocityX = entity.GetVelocity().X;
            velocityY = entity.GetVelocity().Y;
            _isDead = entity.IsDead;
            type = entity.GetType();
        }

        public void UpdateEntityInGameWorld()
        {
            Entity entity = GameWorld.GetEntityById(entityId);

            // Needs to create a new entity.
            if (entity == null)
            {
                entity = (Entity)Activator.CreateInstance(type, new object[] { (int)positionX, (int)positionY });
                entity.Id = entityId;
                GameWorld.AddEntity(entityId, entity);
            }

            entity.SetPosition(new Vector2(positionX, positionY));
            entity.SetVelX(velocityX);
            entity.SetVelY(velocityY);

        }
    }



}
