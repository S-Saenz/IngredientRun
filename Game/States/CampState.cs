using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace WillowWoodRefuge
{
    class CampState : GameplayState
    {
        private NPCDialogueSystem _dialogueSystem;

        Effect _lightEffect;
        LightManager _lightManager;

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

            // shader test
            _lightEffect = content.Load<Effect>("shaders/LightShader");
            _lightEffect.Parameters["TextureDimensions"].SetValue(new Vector2(campTileMap._mapBounds.Width, campTileMap._mapBounds.Height));
            _lightManager = new LightManager(_lightEffect);

            // setup lights
            _lightManager.AddLight(new Vector2(100, 100), 100);
            // _lightManager.AddLight(new Vector2(400, 30), 250);
            _lightManager.AddLight(new Vector2(336, 239), 200, new Vector2(0, 1), .5f * (float)MathHelper.Pi);
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
        }

        public override void unloadState()
        {
            _dialogueSystem.EndInteraction();
            base.unloadState();
        }
    }
}
