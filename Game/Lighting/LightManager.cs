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
        private static AreaLight[] _areaLights;
        private static DirectionalLight[] _directionalLights;

        // Arrays of parameters to input into shader
        Vector2[] _aLightPosition;
        bool _aLightPosChanged = false;
        float[] _aLightDistance;
        bool _aLightDistChanged = false;
        Vector4[] _aLightColor;
        bool _aLightColorChanged = false;
        public int _numALights { get; private set; }
        bool _aLightNumChanged = false;

        Vector2[] _dLightPosition, _dLightDirection;
        bool _dLightPosChanged = false;
        bool _dLightDirChanged = false;
        float[] _dLightDistance, _dLightSpread;
        bool _dLightDistChanged = false;
        bool _dLightSpreadChanged = false;
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
        public bool AddLight(Vector2 loc, float dist)
        {
            if (_numALights >= MAX_AREA_LIGHTS)
            {
                return false;
            }

            _areaLights[_numALights] = new AreaLight(loc, dist);

            // add to shader arrays
            _aLightPosition[_numALights] = loc;
            _aLightPosChanged = true;
            _aLightDistance[_numALights] = dist;
            _aLightDistChanged = true;
            ++_numALights;
            _aLightNumChanged = true;

            UpdateShaderParameters();

            return true;
        }

        // Directional light
        public bool AddLight(Vector2 loc, float dist, Vector2 direction, float spread)
        {
            if (_numDLights >= MAX_DIRECTIONAL_LIGHTS)
            {
                return false;
            }

            _directionalLights[_numDLights] = new DirectionalLight(loc, dist, direction, spread);

            // add to shader arrays
            _dLightPosition[_numDLights] = loc;
            _dLightPosChanged = true;
            _dLightDistance[_numDLights] = dist;
            _dLightDistChanged = true;
            _dLightDirection[_numDLights] = direction;
            _dLightDirChanged = true;
            _dLightSpread[_numDLights] = spread;
            _dLightSpreadChanged = true;
            ++_numDLights;
            _dLightNumChanged = true;

            UpdateShaderParameters();

            return true;
        }

        public void ChangeAreaLight(int index, Vector2? loc = null, float? dist = null)
        {
            _areaLights[index].ChangeLight(loc, dist);

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

            UpdateShaderParameters();
        }

        public void ChangeDirectionLight(int index, Vector2? loc = null, float? dist = null, Vector2? direction = null, float? spread = null)
        {
            _directionalLights[index].ChangeLight(loc, dist, direction, spread);

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

            UpdateShaderParameters();
        }

        public void CreateShaderArrays()
        {
            _aLightPosition = new Vector2[MAX_AREA_LIGHTS];
            _aLightDistance = new float[MAX_AREA_LIGHTS];

            for (int i = 0; i < _numALights; ++i)
            {
                _aLightPosition[i] = _areaLights[i]._loc;
                _aLightDistance[i] = _areaLights[i]._dist;
            }

            _aLightPosChanged = _aLightDistChanged = _aLightNumChanged = true;

            _dLightPosition = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            _dLightDistance = new float[MAX_DIRECTIONAL_LIGHTS];
            _dLightDirection = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            _dLightSpread = new float[MAX_DIRECTIONAL_LIGHTS];

            for (int i = 0; i < _numDLights; ++i)
            {
                _dLightPosition[i] = _directionalLights[i]._loc;
                _dLightDistance[i] = _directionalLights[i]._dist;
                _dLightDirection[i] = _directionalLights[i]._direction;
                _dLightSpread[i] = _directionalLights[i]._spread;
            }
            _dLightPosChanged = _dLightDistChanged = _dLightDirChanged = _dLightSpreadChanged = _dLightNumChanged = true;

            UpdateShaderParameters();
        }

        private void UpdateShaderParameters()
        {
            if(_aLightPosChanged)
                _shader.Parameters["AreaLightPosition"].SetValue(_aLightPosition);
            if(_aLightDistChanged)
                _shader.Parameters["AreaLightDistance"].SetValue(_aLightDistance);
            if(_aLightNumChanged)
                _shader.Parameters["NumAreaLights"].SetValue(_numALights);
            _aLightPosChanged = _aLightDistChanged = _aLightNumChanged = false;

            if (_dLightPosChanged)
                _shader.Parameters["DirectionalLightPosition"].SetValue(_dLightPosition);
            if(_dLightDistChanged)
                _shader.Parameters["DirectionalLightDistance"].SetValue(_dLightDistance);
            if(_dLightDirChanged)
                _shader.Parameters["DirectionalLightDirection"].SetValue(_dLightDirection);
            if(_dLightSpreadChanged)
                _shader.Parameters["DirectionalLightSpread"].SetValue(_dLightSpread);
            if(_dLightNumChanged)
                _shader.Parameters["NumDirectionalLights"].SetValue(_numDLights);
            _dLightPosChanged = _dLightDistChanged = _dLightDirChanged = _dLightSpreadChanged = _dLightNumChanged = false;
        }
    }
}
