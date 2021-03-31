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
            public float _falloff;

            public AreaLight(Vector2 loc, float dist, float falloff)
            {
                _loc = loc;
                _dist = dist;
                _falloff = falloff;
            }

            public void ChangeLight(Vector2? loc = null, float? dist = null, float? falloff = null)
            {
                if(loc.HasValue)
                {
                    _loc = loc.Value;
                }
                if(dist.HasValue)
                {
                    _dist = dist.Value;
                }
                if (falloff.HasValue)
                {
                    _falloff = falloff.Value;
                }
            }
        }
    }
}
