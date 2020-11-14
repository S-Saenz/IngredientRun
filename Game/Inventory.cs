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
        Texture2D inventorySq, acorn, apple, fish;
        bool showInv = false;
        KeyboardState oldKeyState;


        public Inventory()
        {
        }

        public void Load(ContentManager Content)
        {

            inventorySq = Content.Load<Texture2D>("ui/paper");
            acorn = Content.Load<Texture2D>("Ingredient/acorn");
            apple = Content.Load<Texture2D>("Ingredient/apple");
            fish = Content.Load<Texture2D>("Ingredient/fish");
        }

        public void Update(MouseState mouseState, KeyboardState keyState)
        {
            //use mouse to move objects

            //press E for inventory
            if (oldKeyState.IsKeyUp(Keys.E) && keyState.IsKeyDown(Keys.E))
            {
                showInv = !showInv;
            }
            oldKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(showInv)
                spriteBatch.Draw(inventorySq, new Vector2(200, 50), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.4f);
            spriteBatch.Draw(acorn, new Vector2(200, 50), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(apple, new Vector2(250, 20), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(fish, new Vector2(250, 70), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);
        }

        bool IsPointOver(float x, float y, Texture2D sprite)
        {
            return (sprite.Bounds.Contains(x, y));
        }
    }
}
