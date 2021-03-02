using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class DirectionalLight : AreaLight
    {
        Vector2 _direction;
        float _spread;

        public DirectionalLight(Vector2 loc, float dist, Vector2 direction, float spread) : base(loc, dist)
        {
            _direction = direction;
            _spread = spread;
        }
    }
}
