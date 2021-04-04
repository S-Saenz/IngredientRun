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
            public float _falloff;

            public DirectionalLight(Vector2 loc, float dist, Vector2 direction, float spread, float falloff) : base(loc, dist, falloff)
            {
                _direction = direction;
                _spread = spread;
                _falloff = falloff;
            }

            public void ChangeLight(Vector2? loc = null, float? dist = null, Vector2? direction = null, float? spread = null, float? falloff = null)
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
                if (falloff.HasValue)
                {
                    _falloff = falloff.Value;
                }
            }
        }
    }
}
