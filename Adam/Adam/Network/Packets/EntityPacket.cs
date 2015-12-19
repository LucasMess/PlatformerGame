using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adam.Levels;

namespace Adam.Network.Packets
{

    [Serializable]
    public class EntityPacket : DataPacket
    {
        static int _packetTimer;

        int[] _ds;
        Vector2[] _positions;
        Vector2[] _velocities;
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
            Entity[] entities = gameWorld.Entities.ToArray();

            _positions = new Vector2[entities.Length];
            _velocities = new Vector2[entities.Length];

            for (int i = 0; i < entities.Length; i++)
            {
                _positions[i] = new Vector2(entities[i].GetCollRectangle().X, entities[i].GetCollRectangle().Y);
                _velocities[i] = entities[i].GetVelocity();
            }
        }

        public void ExtractTo(GameWorld gameWorld)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                if (i > gameWorld.Entities.Count)
                {
                    Console.WriteLine("There is no entity number {0} in this gameworld. There are only {1}", i, gameWorld.Entities.Count);
                    break;
                }

                gameWorld.Entities[i].UpdateFromPacket(_positions[i], _velocities[i]);
            }
        }
    }



}
