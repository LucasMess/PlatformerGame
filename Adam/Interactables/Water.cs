using ThereMustBeAnotherWay.Levels;
using ThereMustBeAnotherWay.PlayerCharacter;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Interactables
{
    class Water : Interactable
    {
        public Water()
        {

        }

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            Rectangle coll = new Rectangle(entity.GetCollRectangle().Center.X, entity.GetCollRectangle().Center.Y, 1, 1);
            if (coll.Intersects(tile.DrawRectangle))
            {
                entity.IsInWater = true;
                entity.SwimTimer.Increment();
                if (entity is Player)
                {
                    Player player = (Player)entity;
                    if (player.IsJumpButtonPressed() && entity.SwimTimer.TimeElapsedInMilliSeconds > 100)
                    {
                        entity.SwimTimer.Reset();
                        if (GameWorld.GetTileAbove(tile.TileIndex).Id == TMBAW_Game.TileType.Air)
                        {
                            player.SetVelY(-15f);
                        }
                        else
                        {
                            player.SetVelY(-2);
                        }
                    }
                }
                else
                {
                    if (entity.SwimTimer.TimeElapsedInMilliSeconds > 100)
                    {
                        entity.SwimTimer.Reset();
                        entity.SetVelY(-2);
                    }
                }
            }
            base.OnEntityTouch(tile, entity);
        }
    }
}
