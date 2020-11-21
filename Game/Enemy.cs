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
    class Enemy
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


        }

        public void Load(ContentManager Content)
        {
            hitBox = new Rectangle(new Point(100, 200), new Point(texture.Height, texture.Width));
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, new Vector2(1000, 250), null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);



        }
    }
}
