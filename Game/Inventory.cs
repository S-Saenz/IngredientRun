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
        Texture2D inventorySq, acornT, appleT, fishT;
        Ingredient fish, acorn, apple;
        List<Ingredient> ings = new List<Ingredient>();
        public bool showInv = false;
        bool handsFull = false;
        KeyboardState oldKeyState;



        public Inventory()
        {
        }

        public void Load(ContentManager Content)
        {

            inventorySq = Content.Load<Texture2D>("ui/paper");
            acornT = Content.Load<Texture2D>("Ingredient/acorn");
            appleT = Content.Load<Texture2D>("Ingredient/apple");
            fishT = Content.Load<Texture2D>("Ingredient/fish");

            acorn = new Ingredient(acornT, new Vector2(200, 50));
            apple = new Ingredient(appleT, new Vector2(250, 80));
            fish = new Ingredient(fishT, new Vector2(200, 100));
            ings.Add(acorn);
            ings.Add(apple);
            ings.Add(fish);
        }

        public void Update(MouseState mouseState, KeyboardState keyState)
        {
            //use mouse to move objects
            foreach (Ingredient ing in ings) {
                MoveIng(ing,mouseState);
            }

            //press E for inventory
            if (oldKeyState.IsKeyUp(Keys.E) && keyState.IsKeyDown(Keys.E))
            {
                showInv = !showInv;
            }
            oldKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(inventorySq, new Vector2(200, 50), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.4f);
            acorn.Draw(spriteBatch);
            apple.Draw(spriteBatch);
            fish.Draw(spriteBatch);
        }

        bool IsPointOver(Point xy, Sprite sprite)
        {
            return (sprite.Bounds().Contains(xy.X, xy.Y));
        }

        void MoveIng(Ingredient ing, MouseState mouseState) {

            if (IsPointOver(mouseState.Position, ing) && !handsFull)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    ing.holding = true;
                    handsFull = true;
                }
            }
            if (ing.holding && mouseState.LeftButton != ButtonState.Pressed) {

                handsFull = false;
                ing.holding = false;
            }
            if (ing.holding)
            {
                ing.SetPosByMouse(mouseState.Position);
            }

        }
    }
}
