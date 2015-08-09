using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adam.Lights
{
    class SunLight : Light
    {
        public SunLight(Rectangle tileDrawRectangle)
        {
            lightHere = true;
            SetPosition(tileDrawRectangle);
        }
    }
}
