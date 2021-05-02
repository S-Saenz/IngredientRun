using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace WillowWoodRefuge
{
    class Background
    {
        private Texture2D Texture;      //The image to use
        private Vector2 Offset;         //Offset to start drawing our image
        public float Speed;           //Speed of movement of our parallax effect
        public RectangleF? WorldBounds;              //Zoom level of our image
        private float scale;
        //private Viewport Viewport;      //Our game viewport

        //Calculate Rectangle dimensions, based on offset/viewport/zoom values
        /*private Rectangle Rectangle
        {
            //get { return new Rectangle((int)(Offset.X), (int)(Offset.Y), (int)(Viewport.Width / Zoom), (int)(Viewport.Height / Zoom)); }
        }
        */
        public Background(Texture2D texture, float speed, RectangleF? worldBounds)
        {
            Texture = texture;
            Offset = Vector2.Zero;
            Speed = speed;
            WorldBounds = worldBounds;
            scale = 1;// WorldBounds.Value.Height / texture.Height;
        }
        
        public void Update(GameTime gametime, Vector2 direction, Viewport viewport)
        {
            float elapsed = (float)gametime.ElapsedGameTime.TotalSeconds;

            //Store the viewport
            //Viewport = viewport;

            //Calculate the distance to move our image, based on speed
            Vector2 distance = direction * Speed * elapsed;

            //Update our offset
            Offset += distance;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            //spriteBatch.Draw(Texture, Rectangle, Color.White, 0, Vector2.Zero, Zoom, SpriteEffects.None, 1);
            //spriteBatch.Draw(Texture, offset * (WorldBounds.HasValue ? WorldBounds.Value.Width : 0), Color.White);
            Vector2 pos = Speed * offset * (WorldBounds.HasValue ? WorldBounds.Value.Width : 0);
            pos += new Vector2(0, (WorldBounds.HasValue ? WorldBounds.Value.Height - (16 + Texture.Height * scale) : 0));
            spriteBatch.Draw(Texture, pos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}