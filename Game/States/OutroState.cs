using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WillowWoodRefuge
{
    class OutroState : State
    {
        private Texture2D slide1a, slide1b, slide2, slide3, slide4, slide5, slide6, slide7, slide8, slide9, slide10a, slide10b;
        private Animation scene1a, scene1b, scene2, scene3, scene4, scene5, scene6, scene7, scene8, scene9, scene10a, scene10b;
        private List<Animation> animationList = new List<Animation>();
        private int currentScene;
        public OutroState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) : base(game, graphicsDevice, content, spritebatch)
        {
            LoadScenes(content);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (currentScene < animationList.Count)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.Immediate, samplerState: SamplerState.PointClamp);
                animationList[currentScene].Draw(spriteBatch, new Rectangle(0, 0, (int)Game1.instance._cameraController._screenDimensions.X, (int)Game1.instance._cameraController._screenDimensions.Y));
                spriteBatch.End();
            }
        }

        public override void LoadContent()
        {
            scene1a.reset();
            scene1b.reset();
            scene2.reset();
            scene3.reset();
            scene4.reset();
            scene5.reset();
            scene6.reset();
            scene7.reset();
            scene8.reset();
            scene9.reset();
            scene10a.reset();
            scene10b.reset();
        }
        public void LoadScenes(ContentManager Content)
        {
            slide1a = Content.Load<Texture2D>("outro/scene1a_outro");
            slide1b = Content.Load<Texture2D>("outro/scene1b_outro");
            slide2 = Content.Load<Texture2D>("outro/scene2_outro");
            slide3 = Content.Load<Texture2D>("outro/scene3_outro");
            slide4 = Content.Load<Texture2D>("outro/scene4_outro");
            slide5 = Content.Load<Texture2D>("outro/scene5_outro");
            slide6 = Content.Load<Texture2D>("outro/scene6_outro");
            slide7 = Content.Load<Texture2D>("outro/scene7_outro");
            slide8 = Content.Load<Texture2D>("outro/scene8_outro");
            slide9 = Content.Load<Texture2D>("outro/scene9_outro");
            slide10a = Content.Load<Texture2D>("outro/scene10a_outro");
            slide10b = Content.Load<Texture2D>("outro/scene10b_outro");

            //artists/animators, change number in the back to speed/slow down scenes.
            scene1a = new Animation(slide1a, 1, 6, 200);
            scene1b = new Animation(slide1b, 1, 6, 200);
            scene2 = new Animation(slide2, 1, 2, 200);
            scene3 = new Animation(slide3, 1, 8, 200);
            scene4 = new Animation(slide4, 1, 8, 200);
            scene5 = new Animation(slide5, 1, 8, 200);
            scene6 = new Animation(slide6, 1, 6, 200);
            scene7 = new Animation(slide7, 1, 5, 200);
            scene8 = new Animation(slide8, 1, 4, 300);
            scene9 = new Animation(slide9, 1, 8, 300);
            scene10a = new Animation(slide10a, 1, 6, 300);
            scene10b = new Animation(slide10b, 1, 6, 300);

            animationList.Add(scene1a); // index 0
            animationList.Add(scene1b);
            animationList.Add(scene2);
            animationList.Add(scene3);
            animationList.Add(scene4);
            animationList.Add(scene5);
            animationList.Add(scene6);
            animationList.Add(scene7);
            animationList.Add(scene8);
            animationList.Add(scene9);
            animationList.Add(scene10a);
            animationList.Add(scene10b);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void unloadState()
        {

        }

        public override void Update(GameTime gameTime)
        {
            animationList[currentScene].Update(gameTime);

            // skip cutscene
            if (Game1.instance.input.JustPressed("skip"))
            {
                toMenu();
                currentScene = 0;
            }

            if (animationList[currentScene].numLoops > 0)//goes through the scenes
            {
                animationList[currentScene].reset();
                currentScene++;
                if (currentScene == 10)
                {
                    toMenu();
                    currentScene = 0;
                }
            }
        }

        private void toMenu()
        {
            Game1.instance.ChangeState("MenuState");
        }
    }
}