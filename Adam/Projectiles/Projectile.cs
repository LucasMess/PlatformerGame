using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Adam.Projectiles
{
    public class Projectile : Entity
    {
        protected override Rectangle DrawRectangle
        {
            get
            {
                return ComplexAnim.GetDrawRectangle();
            }
        }
    }
}
