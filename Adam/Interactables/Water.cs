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
            
            base.OnEntityTouch(tile, entity);
        }
    }
}
