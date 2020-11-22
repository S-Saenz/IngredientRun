using System;
using System.Collections.Generic;
using System.Text;

namespace IngredientRun
{
    class PickUpable
    {
        private Texture2D texture;
        private Vector2 pos = new Vector2(100, 200);
        private Rectangle hitBox;

        public Enemy(Texture2D img)
        {
            texture = img;
        }

        public Vector2 GetPos()
        {
            return pos;
        }


        public void Update(KeyboardState keyState, KeyboardState oldKeyState)
        {
            hitBox = new Rectangle(new Point(pos.X, pos.Y), new Point(texture.Height, texture.Width));

        }

        public void Load(ContentManager Content)
        {
            hitBox = new Rectangle(new Point(pos.X, pos.Y), new Point(texture.Height, texture.Width));
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }
    }
}
