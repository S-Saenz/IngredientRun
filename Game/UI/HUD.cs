using System;
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

            //create the inventory button 
            lilBackpack = Content.Load<Texture2D>("ui/Inventory/BackpackPixel");
            backpackButton = new UIButton(lilBackpack); //use the new schmancy UIButton class

            backpackButton.Depth = _depth;
            backpackButton.Scale = 2f;

            //inventory goes in top right corner of screen
            float padding = 30f; //px
            backpackButton.pos = new Vector2(_screenWidth - (backpackButton.img.Width * backpackButton.Scale) - padding, padding);

            //have the button in the bottom right corner - right up against the screen edges
            //backpackButton.pos = new Vector2(_screenWidth - (backpackButton.img.Width * backpackButton.Scale), _screenHeight - (backpackButton.img.Height * backpackButton.Scale));

            backpackButton.Click += InventoryButton_Click;

            //create exit button 1183 23
            Vector2 testVector = new Vector2(1728 / 2, 972 / 2);
            Texture2D xButtonTexture = Content.Load<Texture2D>("ui/x-button");
            xButton = new UIButton(xButtonTexture, new Vector2(1183, 23));
            xButton.Depth = .01f;// Game1.instance.gameHUD._depth;
            xButton.Scale = 5f;
        }
        public void Update(MouseState mouseState)
        {
            //temp.. replace this logic with the UImanager
            if (!Game1.instance.inventory.showInv)
            {
                backpackButton.Update(mouseState);
            }
            //xButton.Update(mouseState);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //temp.... replace this logic with the  UIManager
            if (!Game1.instance.inventory.showInv)
            {
                backpackButton.Draw(spriteBatch);
            }
            //xButton.Draw(spriteBatch);
        }

        //when inventory button is clicked, open the inventory
        private void InventoryButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Inventory Button Clicked!");
            Game1.instance.inventory.showInv = true;
        }

    }
}
