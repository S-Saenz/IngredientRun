using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    class CampState : GameplayState
    {
        public CampState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // Initialize NPC dialogue content
            _dialogueSystem = new NPCDialogueSystem(game);

            // Setup Background Layers
            _backgroundLayers = new List<Texture2D>();
            _backgroundLayers.Add(_content.Load<Texture2D>("bg/campsiteprototypemap"));

            // Setup Tilemap
            _tileMap = new TileMap("tilemaps/camp/TempCampMap", _content, game.GraphicsDevice, _physicsHandler);
            _isDark = false;

            // Setup lights
            _staticLightManager.AddLight(new Vector2(64, 256), 50);
            _staticLightManager.AddLight(new Vector2(160, 256), 50);
            _staticLightManager.AddLight(new Vector2(368, 256), 50);
            _staticLightManager.AddLight(new Vector2(488, 256), 50);

            _shadowEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            _ditherOpacityEffect.Parameters["TextureDimensions"].SetValue(new Vector2(_tileMap._mapBounds.Width, _tileMap._mapBounds.Height));
            PostConstruction();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
                            _physicsHandler, _tileMap._mapBounds, area: campArea));
            _characters.Add("Snäll", new NPC("snall",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, _tileMap._mapBounds, area: campArea));
            _characters.Add("Kall", new NPC("kall",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, _tileMap._mapBounds, area: campArea));
            _characters.Add("Arg", new NPC("arg",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, _tileMap._mapBounds, area: campArea));
            _characters.Add("Aiyo", new NPC("aiyo",
                            new Vector2(rand.Next() % (campArea._bounds.Width - 16) + campArea._bounds.Left + 8, campArea._bounds.Bottom),
                            _physicsHandler, _tileMap._mapBounds, area: campArea));
            foreach (NPC character in _characters.Values)
            {
                character.Load(_content);
            }

            // dialogue system
            _dialogueSystem.Load(_characters);
            _dialogueSystem.PlayInteraction(game);

            _isDark = false;
        }

        public override void unloadState()
        {
            _dialogueSystem.EndInteraction();
            base.unloadState();
        }
    }
}
