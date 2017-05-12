using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThereMustBeAnotherWay.Interactables
{
    class Spikes : Interactable
    {
        const int _spikeDamage = 10;

        public Spikes()
        {

        }

        public override void OnEntityTouch(Tile tile, Entity entity)
        {
            entity.TakeDamage(null, _spikeDamage); 

            base.OnEntityTouch(tile, entity);
        }

    }
}
