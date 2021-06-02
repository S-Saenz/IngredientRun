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
    class IntroState : State
    {
        private Texture2D slide1a, slide1b, slide2, slide3, slide4, slide5a, slide5b, slide6, slide7, slide8, slide9, slide10, slide11, slide12, slide13, slide14, slide15, slide16, slide17a, slide17b;
        private Animation scene1a, scene1b, scene2, scene3, scene4, scene5a, scene5b, scene6, scene7, scene8, scene9, scene10, scene11, scene12, scene13, scene14, scene15, scene16, scene17a, scene17b;
        private List<Animation> animationList = new List<Animation>();
        private int currentScene;
        public IntroState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, SpriteBatch spritebatch) : base(game, graphicsDevice, content, spritebatch)
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
            scene5a.reset();
            scene5b.reset();
            scene6.reset();
            scene7.reset();
            scene8.reset();
            scene9.reset();
            scene10.reset();
            scene11.reset();
            scene12.reset();
            scene13.reset();
            scene14.reset();
            scene15.reset();
            scene16.reset();
            scene17a.reset();
            scene17b.reset();

            currentScene = 0;
        }
        public void LoadScenes(ContentManager Content)
        {
            slide1a = Content.Load<Texture2D>("intro/scene1a");
            slide1b = Content.Load<Texture2D>("intro/scene1b");
            slide2 = Content.Load<Texture2D>("intro/scene2");
            slide3 = Content.Load<Texture2D>("intro/scene3");
            slide4 = Content.Load<Texture2D>("intro/scene4");
            slide5a = Content.Load<Texture2D>("intro/scene5a");
            slide5b = Content.Load<Texture2D>("intro/scene5b");
            slide6 = Content.Load<Texture2D>("intro/scene6");
            slide7 = Content.Load<Texture2D>("intro/scene7");
            slide8 = Content.Load<Texture2D>("intro/scene8");
            slide9 = Content.Load<Texture2D>("intro/scene9");
            slide10 = Content.Load<Texture2D>("intro/scene10");
            slide11 = Content.Load<Texture2D>("intro/scene11");
            slide12 = Content.Load<Texture2D>("intro/scene12");
            slide13 = Content.Load<Texture2D>("intro/scene13");
            slide14 = Content.Load<Texture2D>("intro/scene14");
            slide15 = Content.Load<Texture2D>("intro/scene15");
            slide16 = Content.Load<Texture2D>("intro/scene16");
            slide17a = Content.Load<Texture2D>("intro/scene17a");
            slide17b = Content.Load<Texture2D>("intro/scene17b");

            //artists/animators, change number in the back to speed/slow down scenes.
            scene1a = new Animation(slide1a, 1, 6, 200);
            scene1b = new Animation(slide1b, 1, 6, 200);
            scene2 = new Animation(slide2, 1, 10, 200);
            scene3 = new Animation(slide3, 1, 10, 100);
            scene4 = new Animation(slide4, 1, 6, 200);
            scene5a = new Animation(slide5a, 1, 7, 200);
            scene5b = new Animation(slide5b, 1, 7, 200);
            scene6 = new Animation(slide6, 1, 2, 400);
            scene7 = new Animation(slide7, 1, 2, 400);
            scene8 = new Animation(slide8, 1, 2, 400);
            scene9 = new Animation(slide9, 1, 2, 400);
            scene10 = new Animation(slide10, 1, 2, 400);
            scene11 = new Animation(slide11, 1, 2, 300);
            scene12 = new Animation(slide12, 1, 2, 300);
            scene13 = new Animation(slide13, 1, 6, 300);
            scene14 = new Animation(slide14, 1, 6, 300);
            scene15 = new Animation(slide15, 1, 2, 300);
            scene16 = new Animation(slide16, 1, 2, 300);
            scene17a = new Animation(slide17a, 1, 6, 300);
            scene17b = new Animation(slide17b, 1, 6, 300);

            animationList.Add(scene1a); // index 0
            animationList.Add(scene1b);
            animationList.Add(scene2);
            animationList.Add(scene3);
            animationList.Add(scene4);
            animationList.Add(scene5a);
            animationList.Add(scene5b);
            animationList.Add(scene6);
            animationList.Add(scene7);
            animationList.Add(scene8);
            animationList.Add(scene9);
            animationList.Add(scene10);
            animationList.Add(scene11);
            animationList.Add(scene12);
            animationList.Add(scene13);
            animationList.Add(scene14);
            animationList.Add(scene15);
            animationList.Add(scene16);
            animationList.Add(scene17a);
            animationList.Add(scene17b);
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
            if(Game1.instance.input.JustPressed("skip"))
            {
                toCamp();
                currentScene = 0;
            }

            if(animationList[currentScene].numLoops > 0)//goes through the scenes
            {
                animationList[currentScene].reset();
                currentScene++;
                if (currentScene == animationList.Count)
                {
                    toCamp();
                    //currentScene = 0;
                }
            }
        }

        private void toCamp()
        {
            Game1.instance.ChangeState("CampState");
        }
    }
}
