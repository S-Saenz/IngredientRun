using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    class CampState : GameplayState
    {
        

        protected WeatherManager _weatherManager = new WeatherManager();
        public CampState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // Initialize NPC dialogue content
            _dialogueSystem = new NPCDialogueSystem(game);

            _cameraSize = new Vector2(240, 135);
            _playerCamBounds = new RectangleF(0, 0, 80f, 45f);
        }

        protected override void LoadTilemap(ContentManager content)
        {
            // Setup Tilemap
            _tileMap = new TileMap("tilemaps/camp/TempCampMap", _content, game.GraphicsDevice, _physicsHandler, "camp");

            _isDark = false;

            // Setup Background Layers
            _backgroundLayers = new List<Background>();
            _backgroundLayers.Add(new Background(content.Load<Texture2D>("parallax/test"), 0.1f, _tileMap._mapBounds));
            // _backgroundLayers.Add(new Background(content.Load<Texture2D>("parallax/2"), 0.08f, _tileMap._mapBounds));
            // _backgroundLayers.Add(new Background(content.Load<Texture2D>("parallax/3"), 0.06f, _tileMap._mapBounds));
            // _backgroundLayers.Add(new Background(content.Load<Texture2D>("parallax/4"), 0.04f, _tileMap._mapBounds));
            // _backgroundLayers.Add(new Background(content.Load<Texture2D>("parallax/5"), 0.02f, _tileMap._mapBounds));

            // Setup lights
            // _staticLightManager.AddLight(new Vector2(64, 256), 50);
            // _staticLightManager.AddLight(new Vector2(160, 256), 50);
            // _staticLightManager.AddLight(new Vector2(368, 256), 50);
            // _staticLightManager.AddLight(new Vector2(488, 256), 50);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            // _backgroundLayers[0];
            base.Update(gameTime);
            
            //simple weather manager toggle
            if(state.IsKeyDown(Keys.N))
            {
                _weatherManager.nighttime();
            }
            if (state.IsKeyDown(Keys.M))
            {
                _weatherManager.daytime();
            }
            if (state.IsKeyDown(Keys.K))
            {
                _weatherManager.clear();
            }
            if (state.IsKeyDown(Keys.L))
            {
                _weatherManager.rain();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Gray);
            
            base.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            //music
            game.sounds.playSong("forestSong");

            // characters
            Area campArea = _tileMap.GetAreaObject("Camp")[0];
            Random rand = new Random();
            _characters = new Dictionary<string, NPC>();
            _characters.Add("Lura", new NPC("lura", 
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom), 
                            _physicsHandler, "camp", _tileMap._mapBounds, area: campArea));
            _characters.Add("Snäll", new NPC("snall",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, "camp", _tileMap._mapBounds, area: campArea));
            _characters.Add("Kall", new NPC("kall",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, "camp", _tileMap._mapBounds, area: campArea));
            _characters.Add("Arg", new NPC("arg",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, "camp", _tileMap._mapBounds, area: campArea));
            _characters.Add("Aiyo", new NPC("aiyo",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, "camp", _tileMap._mapBounds, area: campArea));

            foreach (NPC character in _characters.Values)
            {
                character.Load(_content);
                character.Injure("mushroom_medicine");
            }

            // dialogue system
            _dialogueSystem.Load(_characters);

            _isDark = false;
        }

        public override void unloadState()
        {
            _dialogueSystem.EndInteraction();
            base.unloadState();
        }
    }
}
