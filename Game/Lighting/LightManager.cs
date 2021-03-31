using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WillowWoodRefuge
{
    public partial class LightManager
    {
        // Light constants
        private static int MAX_AREA_LIGHTS = 10;
        private static int MAX_DIRECTIONAL_LIGHTS = 10;

        // Shader that lights are applied to
        private Effect _shader;

        // Helper containers of light objects
        private AreaLight[] _areaLights;
        private DirectionalLight[] _directionalLights;

        // Arrays of parameters to input into shader
        Vector2[] _aLightPosition;
        bool _aLightPosChanged = false;
        float[] _aLightDistance, _aLightFalloff;
        bool _aLightDistChanged = false;
        bool _aLightFalloffChanged = false;
        Vector4[] _aLightColor;
        bool _aLightColorChanged = false;
        public int _numALights { get; private set; }
        bool _aLightNumChanged = false;

        Vector2[] _dLightPosition, _dLightDirection;
        bool _dLightPosChanged = false;
        bool _dLightDirChanged = false;
        float[] _dLightDistance, _dLightSpread, _dLightFalloff;
        bool _dLightDistChanged = false;
        bool _dLightSpreadChanged = false;
        bool _dLightFalloffChanged = false;
        Vector4[] _dLightColor;
        bool _dLightColorChanged = false;
        public int _numDLights { get; private set; }
        bool _dLightNumChanged = false;

        // Constructor
        public LightManager(Effect lightShader)
        {
            _shader = lightShader;
            _numALights = _numDLights = 0;
            _areaLights = new AreaLight[MAX_AREA_LIGHTS];
            _directionalLights = new DirectionalLight[MAX_DIRECTIONAL_LIGHTS];
            CreateShaderArrays();
        }

        // Area light
        public bool AddLight(Vector2 loc, float dist, float falloff = .6f)
        {
            if (_numALights >= MAX_AREA_LIGHTS)
            {
                return false;
            }

            _areaLights[_numALights] = new AreaLight(loc, dist, falloff);

            // add to shader arrays
            _aLightPosition[_numALights] = loc;
            _aLightPosChanged = true;
            _aLightDistance[_numALights] = dist;
            _aLightDistChanged = true;
            _aLightFalloff[_numALights] = falloff;
            _aLightFalloffChanged = true;
            ++_numALights;
            _aLightNumChanged = true;

            UpdateShaderParameters();

            return true;
        }

        // Directional light
        public bool AddLight(Vector2 loc, float dist, Vector2 direction, float spread, float falloff = .6f)
        {
            if (_numDLights >= MAX_DIRECTIONAL_LIGHTS)
            {
                return false;
            }

            _directionalLights[_numDLights] = new DirectionalLight(loc, dist, direction, spread, falloff);

            // add to shader arrays
            _dLightPosition[_numDLights] = loc;
            _dLightPosChanged = true;
            _dLightDistance[_numDLights] = dist;
            _dLightDistChanged = true;
            _dLightDirection[_numDLights] = direction;
            _dLightDirChanged = true;
            _aLightFalloff[_numALights] = falloff;
            _aLightFalloffChanged = true;
            _dLightSpread[_numDLights] = spread;
            _dLightSpreadChanged = true;
            ++_numDLights;
            _dLightNumChanged = true;

            UpdateShaderParameters();

            return true;
        }

        public void ChangeAreaLight(int index, Vector2? loc = null, float? dist = null, float? falloff = null)
        {
            _areaLights[index].ChangeLight(loc, dist, falloff);

            if(loc.HasValue)
            {
                _aLightPosition[index] = loc.Value;
                _aLightPosChanged = true;
            }
            if (dist.HasValue)
            {
                _aLightDistance[index] = dist.Value;
                _aLightDistChanged = true;
            }
            if (falloff.HasValue)
            {
                _aLightFalloff[index] = falloff.Value;
                _aLightFalloffChanged = true;
            }

            UpdateShaderParameters();
        }

        public void ChangeDirectionLight(int index, Vector2? loc = null, float? dist = null, Vector2? direction = null, float? spread = null, float? falloff = null)
        {
            _directionalLights[index].ChangeLight(loc, dist, direction, spread, falloff);

            if (loc.HasValue)
            {
                _dLightPosition[index] = loc.Value;
                _dLightPosChanged = true;
            }
            if (dist.HasValue)
            {
                _dLightDistance[index] = dist.Value;
                _dLightDistChanged = true;
            }
            if (direction.HasValue)
            {
                _dLightDirection[index] = direction.Value;
                _dLightDirChanged = true;
            }
            if (spread.HasValue)
            {
                _dLightSpread[index] = spread.Value;
                _dLightSpreadChanged = true;
            }
            if (falloff.HasValue)
            {
                _dLightFalloff[index] = falloff.Value;
                _dLightFalloffChanged = true;
            }

            UpdateShaderParameters();
        }

        public void CreateShaderArrays()
        {
            _aLightPosition = new Vector2[MAX_AREA_LIGHTS];
            _aLightDistance = new float[MAX_AREA_LIGHTS];
            _aLightFalloff = new float[MAX_AREA_LIGHTS];

            for (int i = 0; i < _numALights; ++i)
            {
                _aLightPosition[i] = _areaLights[i]._loc;
                _aLightDistance[i] = _areaLights[i]._dist;
                _aLightFalloff[i] = _areaLights[i]._falloff;
            }

            _aLightPosChanged = _aLightDistChanged = _aLightNumChanged = _aLightFalloffChanged = true;

            _dLightPosition = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            _dLightDistance = new float[MAX_DIRECTIONAL_LIGHTS];
            _dLightFalloff = new float[MAX_DIRECTIONAL_LIGHTS];
            _dLightDirection = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            _dLightSpread = new float[MAX_DIRECTIONAL_LIGHTS];

            for (int i = 0; i < _numDLights; ++i)
            {
                _dLightPosition[i] = _directionalLights[i]._loc;
                _dLightDistance[i] = _directionalLights[i]._dist;
                _dLightFalloff[i] = _directionalLights[i]._falloff;
                _dLightDirection[i] = _directionalLights[i]._direction;
                _dLightSpread[i] = _directionalLights[i]._spread;
            }
            _dLightPosChanged = _dLightDistChanged = _dLightDirChanged = _dLightSpreadChanged = _dLightNumChanged = _dLightFalloffChanged = true;

            UpdateShaderParameters();
        }

        private void UpdateShaderParameters()
        {
            if(_aLightPosChanged)
                _shader.Parameters["AreaLightPosition"].SetValue(_aLightPosition);
            if(_aLightDistChanged)
                _shader.Parameters["AreaLightDistance"].SetValue(_aLightDistance);
            if (_aLightFalloffChanged)
                _shader.Parameters["AreaLightFalloff"].SetValue(_aLightFalloff);
            if (_aLightNumChanged)
                _shader.Parameters["NumAreaLights"].SetValue(_numALights);
            _aLightPosChanged = _aLightDistChanged = _aLightNumChanged = _aLightFalloffChanged = false;

            if (_dLightPosChanged)
                _shader.Parameters["DirectionalLightPosition"].SetValue(_dLightPosition);
            if(_dLightDistChanged)
                _shader.Parameters["DirectionalLightDistance"].SetValue(_dLightDistance);
            if (_dLightFalloffChanged)
                _shader.Parameters["DirectionalLightFalloff"].SetValue(_dLightFalloff);
            if (_dLightDirChanged)
                _shader.Parameters["DirectionalLightDirection"].SetValue(_dLightDirection);
            if(_dLightSpreadChanged)
                _shader.Parameters["DirectionalLightSpread"].SetValue(_dLightSpread);
            if(_dLightNumChanged)
                _shader.Parameters["NumDirectionalLights"].SetValue(_numDLights);
            _dLightPosChanged = _dLightDistChanged = _dLightDirChanged = _dLightSpreadChanged = _dLightNumChanged = _dLightFalloffChanged = false;
        }
    }
}
