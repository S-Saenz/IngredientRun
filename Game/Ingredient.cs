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
    public class Ingredient : Sprite
    {
        public bool holding = false;
        //public float Rotation;
        Vector2 Origin;
        Vector2 coordinate;
        //public Vector2 pos;
        public bool highest = false;
        //public float Scale = 1f;

        //for timing how fast items fall down inventory
        public float timeSinceLastDrop = 0f;

        public Ingredient(Texture2D image, Vector2 position)
        {
            img = image;
            pos = position;
            scale = .5f;
            Origin = new Vector2(img.Bounds.Center.X, img.Bounds.Center.Y);
            
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastDrop += (float)gameTime.ElapsedGameTime.TotalSeconds; //add elapsed time to counter


        }
        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(img, pos, null, Color.White, Rotation, Origin, scale, SpriteEffects.None, 1f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }

        public void resetCounter()
        {
            this.timeSinceLastDrop = 0f;
        }
    }
}
   
