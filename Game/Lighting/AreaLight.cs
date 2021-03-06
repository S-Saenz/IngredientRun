using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    public partial class LightManager
    {
        private class AreaLight
        {
            public Vector2 _loc;
            public float _dist;

            public AreaLight(Vector2 loc, float dist)
            {
                _loc = loc;
                _dist = dist;
            }

            public void ChangeLight(Vector2? loc = null, float? dist = null)
            {
                if(loc.HasValue)
                {
                    _loc = loc.Value;
                }
                if(dist.HasValue)
                {
                    _dist = dist.Value;
                }
            }
        }
    }
}
