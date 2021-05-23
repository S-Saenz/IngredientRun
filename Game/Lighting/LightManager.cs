using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace WillowWoodRefuge
{
    public partial class LightManager
    {
        // Light constants
        private static int MAX_AREA_LIGHTS = 10;
        private static int MAX_DIRECTIONAL_LIGHTS = 10;

        // Shader that lights are applied to
        static private Effect _shadowEffect;
        static private Effect _ditherEffect;

        // Various parameters
        private Vector2 _bounds;
        private Color _color;

        // Saved shadow textures
        protected Texture2D _shadowTexture;
        protected RenderTarget2D _casterBuffer;
        protected Texture2D _ditherMap;

        // Buffers
        private RenderTarget2D _bakedShadowBuffer;
        private RenderTarget2D _shadowBuffer;
        private RenderTarget2D _ditherShadowBuffer;

        // Helper containers of light objects
        private AreaLight[] _areaLights;
        private DirectionalLight[] _directionalLights;

        private SpriteBatch _spriteBatch;

        // Arrays of parameters to input into shader
        Vector2[] _aLightPosition;
        bool _aLightPosChanged = false;
        float[] _aLightDistance, _aLightFalloff;
        bool _aLightDistChanged = false;
        bool _aLightFalloffChanged = false;
        public int _numALights { get; private set; }
        bool _aLightNumChanged = false;

        Vector2[] _dLightPosition, _dLightDirection;
        bool _dLightPosChanged = false;
        bool _dLightDirChanged = false;
        float[] _dLightDistance, _dLightSpread, _dLightFalloff;
        bool _dLightDistChanged = false;
        bool _dLightSpreadChanged = false;
        bool _dLightFalloffChanged = false;
        public int _numDLights { get; private set; }
        bool _dLightNumChanged = false;

        // Helper containers of static light objects
        private AreaLight[] _sareaLights;
        private DirectionalLight[] _sdirectionalLights;

        // Arrays of parameters to input into shader
        Vector2[] _saLightPosition;
        float[] _saLightDistance, _saLightFalloff;
        public int _snumALights { get; private set; }

        Vector2[] _sdLightPosition, _sdLightDirection;
        float[] _sdLightDistance, _sdLightSpread, _sdLightFalloff;
        public int _snumDLights { get; private set; }

        // Constructor
        public LightManager(ContentManager content, SpriteBatch spriteBatch)
        {
            //  shaders, if not already loaded
            if(_shadowEffect == null)
            {
                _shadowEffect = content.Load<Effect>("shaders/CastShadows");
            }
            if(_ditherEffect == null)
            {
                _ditherEffect = content.Load<Effect>("shaders/DitherOpacity");
            }
            
            // Dynamic lights
            _numALights = _numDLights = 0;
            _areaLights = new AreaLight[MAX_AREA_LIGHTS];
            _directionalLights = new DirectionalLight[MAX_DIRECTIONAL_LIGHTS];

            // Static lights
            _snumALights = _snumDLights = 0;
            _sareaLights = new AreaLight[MAX_AREA_LIGHTS];
            _sdirectionalLights = new DirectionalLight[MAX_DIRECTIONAL_LIGHTS];

            _spriteBatch = spriteBatch;
        }

        public void Initialize(TileMap tileMap, Texture2D ditherMap, Color color)
        {
            _bounds = new Vector2(tileMap._mapBounds.Width, tileMap._mapBounds.Height);
            _ditherMap = ditherMap;
            _color = color;

            _shadowBuffer = new RenderTarget2D(
                Game1.instance.GraphicsDevice,
                (int)_bounds.X,
                (int)_bounds.Y,
                false,
                Game1.instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _bakedShadowBuffer = new RenderTarget2D(
                Game1.instance.GraphicsDevice,
                (int)_bounds.X,
                (int)_bounds.Y,
                false,
                Game1.instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _casterBuffer = new RenderTarget2D(
                Game1.instance.GraphicsDevice,
                (int)_bounds.X,
                (int)_bounds.Y,
                false,
                Game1.instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            _ditherShadowBuffer = new RenderTarget2D(
                Game1.instance.GraphicsDevice,
                (int)_bounds.X,
                (int)_bounds.Y,
                false,
                Game1.instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

            // Render casters
            Game1.instance.GraphicsDevice.SetRenderTarget(_casterBuffer);
            Game1.instance.GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            tileMap.DrawLayer(_spriteBatch, "Walls");
            tileMap.DrawLayer(_spriteBatch, "Foreground");
            _spriteBatch.End();

            Game1.instance.GraphicsDevice.SetRenderTarget(null);

            // populate shadow texture
            _shadowTexture = new Texture2D(Game1.instance.GraphicsDevice, (int)_bounds.X, (int)_bounds.Y);
            Color[] data = new Color[(int)_bounds.X * (int)_bounds.Y];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = _color;
                // data[i] = new Color((float)(i % _blankTexture.Width) / _blankTexture.Width, 0, (float)(i / _blankTexture.Width) / _blankTexture.Height);
            }
            _shadowTexture.SetData(data);

            CreateShaderArrays();
        }

        public void RenderStatic(ContentManager content)
        {
            LoadStatic();
            Game1.instance.GraphicsDevice.SetRenderTarget(_bakedShadowBuffer);
            Game1.instance.GraphicsDevice.Clear(Color.Transparent);

            // Add dynamic shadow render to baked shadow render
            _spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _shadowEffect);
            _spriteBatch.Draw(_shadowTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            Game1.instance.GraphicsDevice.SetRenderTarget(null);
        }

        public void Load(ContentManager content)
        {
            _shadowEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_bounds.X, _bounds.Y));
            _shadowEffect.Parameters["CasterTexture"].SetValue(_casterBuffer);

            _ditherEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_bounds.X, _bounds.Y));
            _ditherEffect.Parameters["ditherMap"].SetValue(_ditherMap);

            _aLightPosChanged = _aLightDistChanged = _aLightNumChanged = _aLightFalloffChanged = _dLightPosChanged = 
            _dLightDistChanged = _dLightDirChanged = _dLightSpreadChanged = _dLightNumChanged = _dLightFalloffChanged = true;

            LoadDynamic();
        }

        public void Update(SpriteBatch spriteBatch)
        {
            Game1.instance.GraphicsDevice.SetRenderTarget(_shadowBuffer);
            Game1.instance.GraphicsDevice.Clear(Color.Transparent);

            // Add dynamic shadow render to baked shadow render
            spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _shadowEffect);
            spriteBatch.Draw(_bakedShadowBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();
            
            Game1.instance.GraphicsDevice.SetRenderTarget(_ditherShadowBuffer);
            Game1.instance.GraphicsDevice.Clear(Color.Transparent);
            // 
            // Add dither to shadows
            spriteBatch.Begin(blendState: BlendState.Additive, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp, effect: _ditherEffect);
            spriteBatch.Draw(_shadowBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();

            Game1.instance.GraphicsDevice.SetRenderTarget(null);

            // Stream stream = File.Create("shadow.png");
            // _ditherShadowBuffer.SaveAsPng(stream, _shadowBuffer.Width, _shadowBuffer.Height);
            // stream.Dispose();
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            // Draw shadows
            spriteBatch.Begin(transformMatrix: viewMatrix, sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_ditherShadowBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        // Area light
        public bool AddLight(Vector2 loc, float dist, float falloff = .6f, bool isStatic = true)
        {
            if (isStatic)
            {
                if (_snumALights >= MAX_AREA_LIGHTS)
                {
                    return false;
                }

                _sareaLights[_snumALights] = new AreaLight(loc, dist, falloff);

                // add to shader arrays
                _saLightPosition[_snumALights] = loc;
                _saLightDistance[_snumALights] = dist;
                _saLightFalloff[_snumALights] = falloff;
                ++_snumALights;
            }
            else
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
            }
            UpdateShaderParameters();
            return true;
        }

        // Directional light
        public bool AddLight(Vector2 loc, float dist, Vector2 direction, float spread, float falloff = .6f, bool isStatic = true)
        {
            if (isStatic)
            {
                if (_snumDLights >= MAX_DIRECTIONAL_LIGHTS)
                {
                    return false;
                }

                _sdirectionalLights[_snumDLights] = new DirectionalLight(loc, dist, direction, spread, falloff);

                // add to shader arrays
                _sdLightPosition[_snumDLights] = loc;
                _sdLightDistance[_snumDLights] = dist;
                _sdLightDirection[_snumDLights] = direction;
                _saLightFalloff[_snumALights] = falloff;
                _sdLightSpread[_snumDLights] = spread;
                ++_snumDLights;
            }
            else
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
                _dLightFalloff[_numALights] = falloff;
                _dLightFalloffChanged = true;
                _dLightSpread[_numDLights] = spread;
                _dLightSpreadChanged = true;
                ++_numDLights;
                _dLightNumChanged = true;
            }

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
            // Static lights
            _saLightPosition = new Vector2[MAX_AREA_LIGHTS];
            _saLightDistance = new float[MAX_AREA_LIGHTS];
            _saLightFalloff = new float[MAX_AREA_LIGHTS];

            for (int i = 0; i < _snumALights; ++i)
            {
                _saLightPosition[i] = _sareaLights[i]._loc;
                _saLightDistance[i] = _sareaLights[i]._dist;
                _saLightFalloff[i]  = _sareaLights[i]._falloff;
            }

            _sdLightPosition  = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            _sdLightDistance  = new float[MAX_DIRECTIONAL_LIGHTS];
            _sdLightFalloff   = new float[MAX_DIRECTIONAL_LIGHTS];
            _sdLightDirection = new Vector2[MAX_DIRECTIONAL_LIGHTS];
            _sdLightSpread    = new float[MAX_DIRECTIONAL_LIGHTS];

            for (int i = 0; i < _snumDLights; ++i)
            {
                _sdLightPosition[i]  = _sdirectionalLights[i]._loc;
                _sdLightDistance[i]  = _sdirectionalLights[i]._dist;
                _sdLightFalloff[i]   = _sdirectionalLights[i]._falloff;
                _sdLightDirection[i] = _sdirectionalLights[i]._direction;
                _sdLightSpread[i]    = _sdirectionalLights[i]._spread;
            }

            // Dynamic lights
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
            // Dynamic lights
            if (_aLightPosChanged)
                _shadowEffect.Parameters["AreaLightPosition"].SetValue(_aLightPosition);
            if(_aLightDistChanged)
                _shadowEffect.Parameters["AreaLightDistance"].SetValue(_aLightDistance);
            if (_aLightFalloffChanged)
                _shadowEffect.Parameters["AreaLightFalloff"].SetValue(_aLightFalloff);
            if (_aLightNumChanged)
                _shadowEffect.Parameters["NumAreaLights"].SetValue(_numALights);
            _aLightPosChanged = _aLightDistChanged = _aLightNumChanged = _aLightFalloffChanged = false;

            if (_dLightPosChanged)
                _shadowEffect.Parameters["DirectionalLightPosition"].SetValue(_dLightPosition);
            if(_dLightDistChanged)
                _shadowEffect.Parameters["DirectionalLightDistance"].SetValue(_dLightDistance);
            if (_dLightFalloffChanged)
                _shadowEffect.Parameters["DirectionalLightFalloff"].SetValue(_dLightFalloff);
            if (_dLightDirChanged)
                _shadowEffect.Parameters["DirectionalLightDirection"].SetValue(_dLightDirection);
            if(_dLightSpreadChanged)
                _shadowEffect.Parameters["DirectionalLightSpread"].SetValue(_dLightSpread);
            if(_dLightNumChanged)
                _shadowEffect.Parameters["NumDirectionalLights"].SetValue(_numDLights);
            _dLightPosChanged = _dLightDistChanged = _dLightDirChanged = _dLightSpreadChanged = _dLightNumChanged = _dLightFalloffChanged = false;
        }

        private void LoadStatic()
        {
            _shadowEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_bounds.X, _bounds.Y));
            _shadowEffect.Parameters["CasterTexture"].SetValue(_casterBuffer);

            _shadowEffect.Parameters["AreaLightPosition"].SetValue(_saLightPosition);
            _shadowEffect.Parameters["AreaLightDistance"].SetValue(_saLightDistance);
            _shadowEffect.Parameters["AreaLightFalloff"].SetValue(_saLightFalloff);
            _shadowEffect.Parameters["NumAreaLights"].SetValue(_snumALights);

            _shadowEffect.Parameters["DirectionalLightPosition"].SetValue(_sdLightPosition);
            _shadowEffect.Parameters["DirectionalLightDistance"].SetValue(_sdLightDistance);
            _shadowEffect.Parameters["DirectionalLightFalloff"].SetValue(_sdLightFalloff);
            _shadowEffect.Parameters["DirectionalLightDirection"].SetValue(_sdLightDirection);
            _shadowEffect.Parameters["DirectionalLightSpread"].SetValue(_sdLightSpread);
            _shadowEffect.Parameters["NumDirectionalLights"].SetValue(_snumDLights);
        }

        private void LoadDynamic()
        {
            _shadowEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_bounds.X, _bounds.Y));
            _shadowEffect.Parameters["CasterTexture"].SetValue(_casterBuffer);

            _shadowEffect.Parameters["AreaLightPosition"].SetValue(_aLightPosition);
            _shadowEffect.Parameters["AreaLightDistance"].SetValue(_aLightDistance);
            _shadowEffect.Parameters["AreaLightFalloff"].SetValue(_aLightFalloff);
            _shadowEffect.Parameters["NumAreaLights"].SetValue(_numALights);

            _shadowEffect.Parameters["DirectionalLightPosition"].SetValue(_dLightPosition);
            _shadowEffect.Parameters["DirectionalLightDistance"].SetValue(_dLightDistance);
            _shadowEffect.Parameters["DirectionalLightFalloff"].SetValue(_dLightFalloff);
            _shadowEffect.Parameters["DirectionalLightDirection"].SetValue(_dLightDirection);
            _shadowEffect.Parameters["DirectionalLightSpread"].SetValue(_dLightSpread);
            _shadowEffect.Parameters["NumDirectionalLights"].SetValue(_numDLights);
        }

        public void SetOcclusion(bool isOn)
        {
            _shadowEffect.Parameters["Occlusion"].SetValue(isOn);
        }
    }
}
