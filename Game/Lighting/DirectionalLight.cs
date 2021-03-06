using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public partial class LightManager
    {
        private class DirectionalLight : AreaLight
        {
            public Vector2 _direction;
            public float _spread;

            public DirectionalLight(Vector2 loc, float dist, Vector2 direction, float spread) : base(loc, dist)
            {
                _direction = direction;
                _spread = spread;
            }

            public void ChangeLight(Vector2? loc = null, float? dist = null, Vector2? direction = null, float? spread = null)
            {
                base.ChangeLight(loc, dist);
                if (direction.HasValue)
                {
                    _direction = direction.Value;
                }
                if (spread.HasValue)
                {
                    _spread = spread.Value;
                }
            }
        }
    }
}
