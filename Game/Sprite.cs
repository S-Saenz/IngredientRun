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
    class Sprite
    {
        Texture2D img;
        Vector2 pos;

        public Sprite(Texture2D image, Vector2 position) {
            img = image;
            pos = position;
        }

        Rectangle Bounds() {
            Rectangle rect = new Rectangle(new Point((int)pos.X,(int)pos.Y), new Point(img.Width, img.Height));
            return rect;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, pos, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.5f);

        }
    }
}
