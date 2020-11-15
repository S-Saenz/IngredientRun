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


    class Player
    {
        private Texture2D texture, FOW, FOWT;
        private Vector2 pos = new Vector2(100, 500);
        private int hp = 10;
        private Sprite FOWTSprite;
        private int speed = 5;

        public Player()
        {

        }

        public Vector2 GetPos()
        {
            return pos;
        }

        public void Shake()
        {
            pos.X += 10;
            pos.Y += 10;
        }

        public void DoDMG(int dmg)
        {
            hp -= dmg;
        }

        public void Update( MouseState mouseState, KeyboardState keyState)
        {
            //do movement here

            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
            FOWTSprite.pos = pos;
            Vector2 FOWPosVec = pos - mousePosition;
            FOWTSprite.Rotation = (float)((Math.Atan2(
                FOWPosVec.X,
                FOWPosVec.Y
                )));



        }

        public void Load(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("chars/refugee");
            FOW = Content.Load<Texture2D>("ui/visionFade");
            FOWT = Content.Load<Texture2D>("ui/visionFadeTriangle");
            FOWTSprite = new Sprite(FOWT)
            {
                pos = new Vector2(100, 100),
                Color = Color.White,
                Rotation = 0f,
                Scale = 1f,
                Origin = new Vector2(FOWT.Bounds.Center.X, FOWT.Bounds.Center.Y)
            };
        }


        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(texture, pos, null, Color.White, 0f, new Vector2(texture.Bounds.Center.X, texture.Bounds.Center.Y), 0.8f, SpriteEffects.None, 0.5f);
            FOWTSprite.Draw(spriteBatch);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }
    }


}
