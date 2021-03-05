using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using MonoGame.Extended.Shapes;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public class RecipeSelection
    {
         KeyboardState oldKeyState;

         Texture2D mainUI, background, selectedFood;
        Texture2D container, box1, box2, box1Selected, box2Selected, recipeFrame, blackPlus, yellowPlus;

        //recipes 
        Texture2D grilledFish, appleMushroomSoup, monsterSoup, rabbitSoup, carrotSoup;

        //ingredients
        Texture2D acorn, apple, carrot, egg, fish, gooseberry, meat, mouseMelon, water, wood;

        public bool _visibleUI = false;

         static float _screenWidth = 1728;
         float _screenHeight = 972;
         float _scale = 2.7f;
        float _foodScale = 0.165f;

        Cook cookingUI;
        Inventory inventory;

        List<Ingredient> _availableIngredients;  //all the ingredients that are in the player's inventory

        //dict of ALL recipes
        //KEY = food, VALUE = recipe's ingredients 
        Dictionary<Texture2D, List<Texture2D>> _recipes = 
            new Dictionary<Texture2D, List<Texture2D>>();


        //there are 6 max recipes available
        //Key = [row,height], Value = recipe's end food
        Dictionary< Vector2, Texture2D> _recipesDisplay
            = new Dictionary<Vector2, Texture2D>();

        //Key = [row,height], Value = (x,y)
        Dictionary<Vector2, Vector2> _recipeCoordinates
            = new Dictionary<Vector2, Vector2>();


        MouseState _mouseState;

         Boolean _debugMode = true;

         public RecipeSelection(ref Cook cookingUI, ref Inventory inventory)
         {
            cookingUI._cookingVisible = false;
            this.cookingUI = cookingUI;
            this.inventory = inventory;
         }

         public void Load(ContentManager Content)
         {
             //load in images
             mainUI = Content.Load<Texture2D>("ui/Fake Recipe Selection");
             background = Content.Load<Texture2D>("ui/new camp");
            selectedFood = Content.Load<Texture2D>("ingredient/grilled_fishScaled");

            container = Content.Load<Texture2D>("ui/UI Container");
            box1 = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Single");
            box2 = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Double");
            box1Selected = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Single Yellow");
            box2Selected = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Double Yellow");
            recipeFrame = Content.Load<Texture2D>("ui/Recipe/Food Frame");
            blackPlus = Content.Load<Texture2D>("ui/Recipe/Black Plus");
            yellowPlus = Content.Load<Texture2D>("ui/Recipe/Yellow Plus");


            //copy the ingredient textures 
            this.acorn = inventory.acorn;
            this.apple = inventory.apple;
            this.carrot = inventory.carrot;
            this.egg = inventory.egg;
            this.fish = inventory.fish;
            this.gooseberry = inventory.gooseberry;
            this.meat = inventory.meat;
            this.mouseMelon = inventory.mouseMelon;
            this.water = inventory.water;
            this.wood = inventory.wood;
            //copy recipe textures
            this.grilledFish = inventory.grilledFish;
            this.appleMushroomSoup = inventory.appleMushroomSoup;
            this.carrotSoup = inventory.carrotSoup;
            this.monsterSoup = inventory.monsterSoup;
            this.rabbitSoup = inventory.rabbitSoup;


            //ADD RECIPES HERE
            _recipes.Add(grilledFish, new List<Texture2D>() { fish });
            _recipes.Add(appleMushroomSoup, new List<Texture2D>() { apple, water });
            _recipes.Add(carrotSoup, new List<Texture2D>() { carrot, water });
            _recipes.Add(rabbitSoup, new List<Texture2D>() { meat, water });
            _recipes.Add(monsterSoup, new List<Texture2D>() { meat, water });

            //add in the coordinates where each recipe box will be displayed (assuming origin is at (0,0)
            _recipeCoordinates.Add(new Vector2(0, 0), new Vector2(82, 47));
            _recipeCoordinates.Add(new Vector2(1, 0), new Vector2(339, 47));
            _recipeCoordinates.Add(new Vector2(0, 1), new Vector2(82, 109));
            _recipeCoordinates.Add(new Vector2(1, 1), new Vector2(339, 109));
            _recipeCoordinates.Add(new Vector2(0, 2), new Vector2(82, 170));
            _recipeCoordinates.Add(new Vector2(1, 2), new Vector2(339, 170));

            //multiply each (x,y) by _scale
            foreach(KeyValuePair<Vector2, Vector2> point in _recipeCoordinates.ToList())
            {
                _recipeCoordinates[point.Key] = Vector2.Multiply(point.Value, Convert.ToSingle(_scale));
            }

     }

         public void Update(MouseState mouseState, KeyboardState keyState)
         {
            //we are calling mouseState in Draw(), so update a member variable with mousestate so we can use it in draw
            this._mouseState = mouseState;

            //recipe selection and cooking cannot be simultaneously visible
            if (cookingUI._cookingVisible)
            {
                _visibleUI = false;
            }

            if(cookingUI._finished && !_visibleUI && !cookingUI._cookingVisible)
            {
                UpdateInventoryAfterCooking();
                cookingUI._finished = false;
            }
            
             ////////////////////////////// debugging tools \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

             //press ALT + Q to toggle visibility visiblity 
             // if( oldKeyState.IsKeyUp(Keys.Q) && keyState.IsKeyDown(Keys.Q) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
             // {
             //     //debug($"before: {_visibleUI}");
             //     _visibleUI = !_visibleUI; //toggle on/off
             //     //debug($"after: {_visibleUI}");
             // }

             //press ALT + W to switch to cooking UI with selected food
             if(oldKeyState.IsKeyUp(Keys.W) && keyState.IsKeyDown(Keys.W) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
            {
                _visibleUI = false;
                cookingUI._finished = false;
                cookingUI._cookingVisible = true;
                cookingUI.foodImage = selectedFood; //selectedFood = grilledFish
            }

            //press ALT + E to print available recipes 
            // if (oldKeyState.IsKeyUp(Keys.E) && keyState.IsKeyDown(Keys.E) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
            // {
            //     foreach (Texture2D food in CookableRecipes())
            //     {
            //         debug($"{food}"); 
            //     }
            // }
            //press ALT + R to print available recipes 
            if (oldKeyState.IsKeyUp(Keys.R) && keyState.IsKeyDown(Keys.R) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
            {
                inventory.addIngredient(inventory.water);
            }

            //press HOME to toggle debug mode
            if (oldKeyState.IsKeyUp(Keys.Home) && keyState.IsKeyDown(Keys.Home))
                _debugMode = !_debugMode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //draw the fake background only when debug mode is on 
            float bgOpacity = _debugMode ? 1f : 0f;
            // spriteBatch.Draw(background, new Vector2(-_screenWidth * 4 / 3, 0), null, Color.White * bgOpacity, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);

            float uiOpacity = _visibleUI ? 1f : 0f; //disable/enable visibility for the cooking UI through its opacity value
            //spriteBatch.Draw(mainUI, new Vector2(0, 0), null, Color.White * uiOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);

            if (_visibleUI)
            {
                //draw the UI container
                spriteBatch.Draw(container, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);

                //draw the recipes
                List<Texture2D> cookableRecipes = CookableRecipes();
                int numRecipes = cookableRecipes.Count;
                foreach(KeyValuePair<Vector2, Vector2> point in _recipeCoordinates)
                {
                    if (numRecipes > 0)
                    {
                        

                        Texture2D recipeFood = cookableRecipes.ElementAt(numRecipes - 1);
                        List<Texture2D> recipeIngredients = _recipes[recipeFood];

                        //ummmmm, I fucked up, but I'm in too deep. The following logic should probably be implemented in update
                        //because the mouse position is being used for calculations. I think it could've been done if the recipe boxes were separate object types
                        //with their own draw function. But screw it, this draw function gonna go brazy
                        Texture2D box = pickBoxForRecipe(recipeFood, point.Value);
                        if (IsRecipeBeingClicked(this._mouseState.Position, box, point.Value, _scale))
                            SwitchToCooking(recipeFood);

                        spriteBatch.Draw(box, point.Value, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);
                        spriteBatch.Draw(recipeFrame, point.Value, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);

                        spriteBatch.Draw(recipeFood, point.Value, null, Color.White, 0f, Vector2.Zero, _foodScale, SpriteEffects.None, 1f);
                        spriteBatch.Draw(recipeIngredients.ElementAt(0), new Vector2(point.Value.X + 60*_scale, point.Value.Y), null, Color.White, 0f, Vector2.Zero, .25f, SpriteEffects.None, 1f);
                        if(recipeIngredients.Count == 2)
                        {
                            spriteBatch.Draw(recipeIngredients.ElementAt(1), new Vector2(point.Value.X + 160 * _scale, point.Value.Y), null, Color.White, 0f, Vector2.Zero, .25f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(yellowPlus, new Vector2(point.Value.X + 119 * _scale, point.Value.Y + 30), null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);
                        }


                        numRecipes--;
                    }
                }
                
            }
        }

        void debug(String message)
         {
            Debug.WriteLineIf(_debugMode, message);
         }

        //will return a list of the foods that can be cooked
        
        List<Texture2D> CookableRecipes()
        {
            List<Texture2D> cookable = new List<Texture2D>();

            foreach(KeyValuePair<Texture2D, List<Texture2D>> recipe in _recipes) //iterate through the recipes
            {
                List<Texture2D> ingredientsNeeded = recipe.Value;
                bool ingredientsAvailable = true; //until proven otherwise

                foreach(Texture2D ingredientNeeded in ingredientsNeeded) //iterate through all the ingredientsNeeded for the recipe
                {
                    bool ingredientFound = false;
                    foreach(Ingredient inventoryIngredient in inventory.ingredientList) //iterate through the available ingredients in inventory 
                    {
                        if (ingredientNeeded == inventoryIngredient.img) //check if the ingredient needed is in the available ingredients
                            ingredientFound = true;
                    }
                    //missing ingredient means we can't make the recipe!
                    if (!ingredientFound)
                        ingredientsAvailable = false;
                }

                //if all ingredients were found, we can cook it!
                if(ingredientsAvailable)
                    cookable.Add(recipe.Key);
            }
            return cookable;
        }

        //determine whether a recipe getting displayed gets a short or long box or whether its highlighted or not
        Texture2D pickBoxForRecipe(Texture2D recipeFood, Vector2 point)
        {
            Texture2D box;

            //pick the appropriate sized box for the amount of ingredients
            List<Texture2D> recipeIngredients = _recipes[recipeFood];
            box = recipeIngredients.Count == 1 ? box1 : box2; 

            //determine whether or not the mouse is hovering over the box and if so highlight the box!
            Boolean highlight = IsPointOverRecipeBox(this._mouseState.Position, box, point, _scale);
            if(highlight)
            {
                box = box == box1 ? box1Selected : box2Selected; 
            }

            return box;
        }


        //this is my version of IsPointover()
        //differences include accepting Texture2D as a @param instead of a Sprite and factoring scale into calculations
        bool IsPointOverRecipeBox(Point xy, Texture2D image, Vector2 pos,  float scale)
        {
            int intScale = Convert.ToInt32(scale);
            //assume origin is at (0,0)
            Rectangle rect = new Rectangle(new Point((int)pos.X, (int)pos.Y), new Point(image.Width*intScale, image.Height*intScale));
            return (rect.Contains(xy.X, xy.Y));
        }

        //checks to see if a recipe box is being clicked
        bool IsRecipeBeingClicked(Point mousePoint, Texture2D image, Vector2 pos, float scale)
        {
            bool mouseClicked = this._mouseState.LeftButton == ButtonState.Pressed;
            return IsPointOverRecipeBox(mousePoint, image, pos, scale) && mouseClicked;
        }

        //use this when a recipe box is clicked to cook that recipe!
        void SwitchToCooking(Texture2D selectedFood)
        {
            //make sure the recipe selection UI is active/visible when implementing this logic
            if (_visibleUI)
            {
                _visibleUI = false; //recipe selecion is no longer visible
                cookingUI._cookingVisible = true; //cooking ui is now visible

                cookingUI._finished = false; //resets cooking UI
                cookingUI.foodImage = selectedFood; //display food being cooked in the cooking ui
            }
        }

        void UpdateInventoryAfterCooking()
        {
            //add cooked food to inventory
            Texture2D cookedFood = cookingUI.foodImage;
            inventory.addIngredient(cookedFood);

            //remove used ingredients from inventory
            List<Texture2D> ingredients = _recipes[cookedFood];
            foreach(Texture2D ingredient in ingredients)
                inventory.removeIngredient(ingredient);   
        }
    }
}