using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
            _characters = new Dictionary<string, NPC>();
            _characters.Add("Lura", new NPC(_content.Load<Texture2D>("chars/lura"), Vector2.Zero));
            _characters.Add("Snäll", new NPC(_content.Load<Texture2D>("chars/snall"), Vector2.Zero));
            _characters.Add("Kall", new NPC(_content.Load<Texture2D>("chars/kall"), Vector2.Zero));
            _characters.Add("Arg", new NPC(_content.Load<Texture2D>("chars/arg"), Vector2.Zero));
            _characters.Add("Aiyo", new NPC(_content.Load<Texture2D>("chars/aiyo"), Vector2.Zero));
            _tileMap.PlaceNPCs(_characters);

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
