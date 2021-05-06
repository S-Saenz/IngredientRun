﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace WillowWoodRefuge
{
    public class Animation
    {
        public Texture2D texture { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }
        public delegate void AnimationEventHandler(int frame);
        private int currentFrame;
        private int totalFrames;
        private int delayFrames = 0;

        //events
        public event AnimationEventHandler _onAnimationEnd;

        //slow down frame animation
        private int timeSinceLastFrame = 0;
        private int frameSpeed = 0;
        public Animation(Texture2D texture_, int rows_, int columns_, int frameSpeed_)
        {
            texture = texture_;
            rows = rows_;
            columns = columns_;
            currentFrame = 0;
            totalFrames = rows * columns;
            frameSpeed = frameSpeed_;
        }

        public void Update(GameTime gameTime)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > frameSpeed)
            {
                timeSinceLastFrame -= frameSpeed;
                currentFrame++;
                timeSinceLastFrame = 0;
                if (currentFrame == totalFrames)
                {
                    currentFrame = 0;
                    CallAnimationEnd(0);
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, float scale)
        {
            int width = texture.Width / columns;
            int height = texture.Height / rows;
            int row = (int)((float)currentFrame / (float)columns);
            int column = currentFrame % columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);
            
            spriteBatch.Draw(texture, location, sourceRectangle, Color.White, 0f, 
                new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2), scale, SpriteEffects.None, 0.5f);
        }

        public void AddAnimationEndListener(AnimationEventHandler animationFunction)
        {
            _onAnimationEnd += animationFunction;
        }

        public void startAnimation(int frame)
        {
            delayFrames = frame;
        }

        public void CallAnimationEnd(int frame)
        {
            _onAnimationEnd?.Invoke(frame);
        }

        public int getDelayFrames()
        {
            return delayFrames;
        }
    }
}