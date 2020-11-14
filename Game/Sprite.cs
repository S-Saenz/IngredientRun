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
        public Texture2D img;
        public Vector2 pos;
        float scale = 0.5f;

        public Sprite() { }

        public Sprite(Texture2D image, Vector2 position) {
            img = image;
            pos = position;
        }

        public Rectangle Bounds() {
            Rectangle rect = new Rectangle(new Point((int)pos.X,(int)pos.Y), new Point(img.Width, img.Height));
            return rect;
        }

        public void SetPosByMouse(Point p) {
            pos = new Vector2(p.X-(img.Width/2*scale), p.Y+20-(img.Height/2*scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(img, pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);

        }
    }
}
