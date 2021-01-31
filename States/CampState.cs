using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace IngredientRun.States
{
    class CampState : State
    {
        private NPCDialogueSystem _dialogueSystem;
        private SpriteFont _dialogueFont;

        public CampState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spriteBatch)
            : base(game, graphicsDevice, content, spriteBatch)
        {
            // initialize NPC dialogue content
            _dialogueSystem = new NPCDialogueSystem("Content/dialogue/NPCDialogue.tsv", game);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin();
            _dialogueSystem.Draw(_dialogueFont, new Vector2(100, 100), gameTime, spriteBatch);
            // spriteBatch.DrawString(_dialogueFont, "Arg: Again with your fuckin' omens!  Did your \"omens\" tell you about that silent nightmare that fuckin' destroyed our homes?", new Vector2(100, 100), Color.White);
            spriteBatch.End();
        }

        public override void LoadContent()
        {
            _dialogueFont = _content.Load<SpriteFont>("fonts/NPCDialogue");
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void unloadState()
        {

        }

        public override void Update(GameTime gameTime)
        {
            _dialogueSystem.PlayInteraction(game);
        }
    }
}
