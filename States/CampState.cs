using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace IngredientRun.States
{
    class CampState : State
    {
        private NPCDialogueSystem _dialogueSystem;

        public CampState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // initialize NPC dialogue content
            _dialogueSystem = new NPCDialogueSystem("Content/dialogue/NPCDialogue.tsv", game);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Black);
        }

        public override void LoadContent()
        {
            
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void unloadState()
        {

        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
