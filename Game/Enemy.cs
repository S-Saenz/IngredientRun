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
        private Vector2 pos;
        private Vector2 staticPos;
        private Rectangle hitBox;

        public Enemy(Texture2D img, Vector2 position)
        {
            pos = staticPos = position;
            texture = img;
        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public void Update(Vector2 mapPos)
        {
            pos = mapPos + staticPos;
            hitBox = new Rectangle(new Point((int)(mapPos.X + pos.X), (int)(mapPos.Y + pos.Y)), new Point(texture.Height, texture.Width));

        }

        public void Load(ContentManager Content)
        {
            hitBox = new Rectangle(new Point(100, 200), new Point(texture.Height, texture.Width));
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }
    }
}
