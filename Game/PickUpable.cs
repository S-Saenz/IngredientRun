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
    class PickUpable
    {
        private Texture2D texture;
        private Vector2 pos = new Vector2(100, 200);
        private Vector2 staticPos = new Vector2(100, 200);
        private Rectangle hitBox;

        public PickUpable(Texture2D img)
        {
            texture = img;
        }

        public Vector2 GetPos()
        {
            return pos;
        }


        public void Update( Vector2 mapPos)
        {
            pos = mapPos+staticPos;
            hitBox = new Rectangle(new Point((int)(mapPos.X + pos.X), (int)(mapPos.Y + pos.Y)), new Point(texture.Height, texture.Width));

        }

        public void Load(ContentManager Content)
        {
            hitBox = new Rectangle(new Point((int)(pos.X), (int)(pos.Y)), new Point(texture.Height, texture.Width));
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }
    }
}
