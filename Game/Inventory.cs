using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace IngredientRun
{
    class Inventory
    {
        Texture2D inventorySq;
        bool showInv = false;
        KeyboardState oldKeyState;

        public Inventory()
        {
        }

        public void Load(ContentManager Content)
        {

            inventorySq = Content.Load<Texture2D>("ui/paper");
        }

        public void Update(MouseState mouseState, KeyboardState keyState)
        {
            if (oldKeyState.IsKeyUp(Keys.E) && keyState.IsKeyDown(Keys.E))
            {
                showInv = !showInv;
            }
            oldKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(showInv)
                spriteBatch.Draw(inventorySq, new Vector2(200, 50), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
    }
}
