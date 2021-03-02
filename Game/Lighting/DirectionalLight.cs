using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class DirectionalLight : AreaLight
    {
        private static int MAX_DIRECTIONAL_LIGHTS = 10;
        private static DirectionalLight[] _directionalLights = new DirectionalLight[MAX_DIRECTIONAL_LIGHTS];
        private static int _numDirectionalLights = 0;

        protected Vector2 _direction;
        protected float _spread;

        protected DirectionalLight(Vector2 loc, float dist, Vector2 direction, float spread) : base(loc, dist)
        {
            _direction = direction;
            _spread = spread;
        }

        static public bool AddLight(Vector2 loc, float dist, Vector2 direction, float spread)
        {
            if (_numDirectionalLights >= MAX_DIRECTIONAL_LIGHTS)
            {
                return false;
            }

            _directionalLights[_numDirectionalLights] = new DirectionalLight(loc, dist, direction, spread);
            ++_numDirectionalLights;

            return true;
        }

        static public int CreateShaderArrays(out Vector2[] position, out float[] distance, out Vector2[] direction, out float[] spread)
        {
            position = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            distance = new float[MAX_DIRECTIONAL_LIGHTS];
            direction = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            spread = new float[MAX_DIRECTIONAL_LIGHTS];

            for (int i = 0; i < _numDirectionalLights; ++i)
            {
                position[i] = _directionalLights[i]._loc;
                distance[i] = _directionalLights[i]._dist;
                direction[i] = _directionalLights[i]._direction;
                spread[i] = _directionalLights[i]._spread;
            }

            return _numDirectionalLights;
        }
    }
}
