﻿using System;
using System.Collections.Generic;
using System.Text;

using MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WillowWoodRefuge
{
    //gameState can check state to see if player input for movement should be ignored.
    public enum UIState { None, Inventory, RecipeMenu, CookingGame }; //put it here to make it glcbal

    class UIManager
    {
        State _gameState;
        public UIState _currState;

        public UIManager()
        {

        }

        public void Update(GameTime gametime)
        {
            // Separate updating and input checks for different states 
            // so that we can safely reuse keybindings without worrying.
            // This also means that we don't have to use any processing 
            // power on updating ui that isn't showing. Cases in here
            // should handle when the UI state should change.
            switch (_currState)
            { 
                case UIState.None:
                    //check if inventory icon clicked
                    Game1.instance.gameHUD.Update(Mouse.GetState());
                    break;
                case UIState.Inventory:
                    //check inventory input, call inventory update
                    //Game1.instance.inventory.Update(Mouse.GetState(), Keyboard.GetState());
                    Game1.instance.inventory.Update(Mouse.GetState(), Keyboard.GetState());
                    break;
                case UIState.RecipeMenu:
                    //check recipeselect input, call update
                    Game1.instance.recipeMenu.Update(Mouse.GetState(), Keyboard.GetState());
                    break;
                case UIState.CookingGame:
                    //same as ^
                    Game1.instance.cookingGame.Update(Mouse.GetState(), Keyboard.GetState(), gametime);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Similarly to the switch statement in Update, this
            // will just clean up drawing the different states
            // without having to worry about errors with
            // multiple states being drawn at the same time.
            switch (_currState)
            {
                case UIState.None:
                    //draw inventory icon
                    Game1.instance.gameHUD.Draw(spriteBatch);
                    break;
                case UIState.Inventory:
                    //draw inventory
                    //Game1.inventory.Draw(spriteBatch);
                    Game1.instance.inventory.Draw(spriteBatch);
                    break;
                case UIState.RecipeMenu:
                    //draw RecipeSelection
                    //Game1.recipeMenu.Draw(spriteBatch);
                    Game1.instance.recipeMenu.Draw(spriteBatch);
                    break;
                case UIState.CookingGame:
                    //Game1.cookingGame.Draw(spriteBatch);
                    Game1.instance.cookingGame.Draw(spriteBatch);
                    break;
            }
        }

        public void SwitchState(UIState nextState) {

            // Similar to load, just code reliably called when
            // leaving a state. Clean up unneeded variables, stop
            // and resolve animations, etc. Could be moved to a
            // separate function
            switch (_currState)
            {
                case UIState.None:
                    //unload none
                    break;
                case UIState.Inventory:
                    //unload inventory

                    break;
                case UIState.RecipeMenu:
                    //pass the chosen recipe to cookingGame
                    //cookingGame.foodToCook = recipeMenu.chosenRecipe
                    break;
                case UIState.CookingGame:
                    break;
            }

            // Logic that you know will be called any time a given
            // state is changed to, in order to set things up for
            // drawing and interacting. Could be moved to a separate
            // method if things get to be more than a few lines
            switch (nextState)
            {
                case UIState.None:
                    //load none
                    break;
                case UIState.Inventory:
                    //load inventory
                    break;
                case UIState.RecipeMenu:
                    break;
                case UIState.CookingGame:
                    break;
            }
        }

    }
}
