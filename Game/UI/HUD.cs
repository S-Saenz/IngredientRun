﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WillowWoodRefuge
{
    public class HUD
    {
        MouseState _previousMouse;
        MouseState _currentMouse;

        public float _depth = .01f; //all HUD elements should use this depth

        Texture2D lilBackpack;
        UIButton backpackButton;
        UIButton xButton;
    
        public HUD()
        {

        }

        public void Load(ContentManager Content)
        {
            float _screenWidth = 1728;
            float _screenHeight = 972;
            float _screenScale = Game1.instance._cameraController._screenScale;

            //create the inventory button 
            lilBackpack = Content.Load<Texture2D>("ui/Inventory/BackpackPixel");
            backpackButton = new UIButton(lilBackpack); //use the new schmancy UIButton class

            //backpackButton.Depth = _depth;
            backpackButton._scale = 2f;

            //inventory goes in top right corner of screen
            float padding = 30f; //px
            backpackButton._position = new Vector2(_screenWidth - backpackButton._texture.Width * backpackButton._scale - padding, 
                                                    padding);
            backpackButton._position *= _screenScale;

            //because we haven't used the texture atlas for this button yet, we will manually create the rectangle
            backpackButton._rectangle = new Rectangle((int)backpackButton._position.X, 
                                                    (int)backpackButton._position.Y, 
                                                    (int)(backpackButton._texture.Width * backpackButton._scale *_screenScale),
                                                    (int)(backpackButton._texture.Height * backpackButton._scale * _screenScale));

            //have the button in the bottom right corner - right up against the screen edges
            //backpackButton.pos = new Vector2(_screenWidth - (backpackButton.img.Width * backpackButton.Scale), _screenHeight - (backpackButton.img.Height * backpackButton.Scale));

            backpackButton.Click += InventoryButton_Click;

        }
        public void Update(MouseState mouseState)
        {
            //Debug.WriteLine("HUD update called");
            backpackButton.Update(mouseState);
            //xButton.Update(mouseState);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //Debug.WriteLine("HUD draw being called");   
            backpackButton.Draw(spriteBatch);
        }

        //when inventory button is clicked, open the inventory
        private void InventoryButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Inventory Button Clicked!");
            Game1.instance.UI.SwitchState(UIState.Inventory);
            //Game1.instance.inventory.showInv = true;
        }

        public void unload()
        {

        }
    }
}
