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
        public Texture2D img { get; set; }
        public Vector2 pos { get; set; }
        float scale = 0.5f;


        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Vector2 Origin { get; set; }
        public Color Color { get; set; }
        public float Depth { get; set; }

        public Sprite() { }

        public Sprite(Texture2D texture)
        {
            this.img = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.img,
                             this.pos,
                             null,
                             this.Color,
                             0 - this.Rotation - 1.5f,
                             this.Origin,
                             this.Scale,
                             SpriteEffects.None,
                             this.Depth);
        }


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

        public void Draw(SpriteBatch spriteBatch, int i)
        {
            spriteBatch.Draw(img, pos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);

        }
    }
}
