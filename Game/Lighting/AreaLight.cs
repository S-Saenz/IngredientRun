using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class AreaLight
    {
        Vector2 _loc;
        float _dist;

        public AreaLight(Vector2 loc, float dist)
        {
            _loc = loc;
            _dist = dist;
        }
    }
}
