using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Network.Packets
{

    [Serializable]
    public class EntityPacket : DataPacket
    {
        static int PacketTimer;

        int[] IDs;
        Vector2[] positions;
        Vector2[] velocities;
        long TimeStamp
        {
            get; set;
        }

        /// <summary>
        /// Creates a new entity packet from the current gameworld.
        /// </summary>
        /// <param name="gameWorld"></param>
        /// <returns></returns>
        public EntityPacket(GameWorld gameWorld)
        {
            Entity[] entities = gameWorld.entities.ToArray();

            positions = new Vector2[entities.Length];
            velocities = new Vector2[entities.Length];

            for (int i = 0; i < entities.Length; i++)
            {
                positions[i] = new Vector2(entities[i].GetCollRectangle().X, entities[i].GetCollRectangle().Y);
                velocities[i] = entities[i].GetVelocity();
            }

        }

        public void ExtractTo(GameWorld gameWorld)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                if (i > gameWorld.entities.Count)
                {
                    Console.WriteLine("There is no entity number {0} in this gameworld. There are only {1}", i, gameWorld.entities.Count);
                    break;
                }

                gameWorld.entities[i].UpdateFromPacket(positions[i], velocities[i]);
            }
        }
    }



}
