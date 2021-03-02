using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class AreaLight
    {
        private static int MAX_AREA_LIGHTS = 10;
        private static AreaLight[] _areaLights = new AreaLight[MAX_AREA_LIGHTS];
        private static int _numAreaLights = 0;
        
        protected Vector2 _loc;
        protected float _dist;

        protected AreaLight(Vector2 loc, float dist)
        {
            _loc = loc;
            _dist = dist;
        }

        static public bool AddLight(Vector2 loc, float dist)
        {
            if(_numAreaLights >= MAX_AREA_LIGHTS)
            {
                return false;
            }

            _areaLights[_numAreaLights] = new AreaLight(loc, dist);
            ++_numAreaLights;

            return true;
        }

        static public int CreateShaderArrays(out Vector2[] position, out float[] distance)
        {
            position = new Vector2[MAX_AREA_LIGHTS];
            distance = new float[MAX_AREA_LIGHTS];

            for(int i = 0; i < _numAreaLights; ++i)
            {
                position[i] = _areaLights[i]._loc;
                distance[i] = _areaLights[i]._dist;
            }

            return _numAreaLights;
        }
    }
}
