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
using System.IO;
using System.Reflection;

namespace WillowWoodRefuge
{
    public class RecipeSelection
    {
         KeyboardState _oldKeyState;


        //Texture2D mainUI, background, selectedFood;
        string _selectedFood;
        Point? _hoverOver = null; // entry being hovered over by mouse (null if not hovering over any entry) 
        Color _hoverColor = new Color(255, 255, 0);
        // Texture2D container, box1, box2, box1Selected, box2Selected, recipeFrame, blackPlus, yellowPlus;

        //recipes 
        //Texture2D grilledFish, appleMushroomSoup, monsterSoup, rabbitSoup, carrotSoup;

        //ingredients
        //Texture2D acorn, apple, carrot, egg, fish, gooseberry, meat, mouseMelon, mushroom, water, wood;

        //public bool _visibleUI = true;

        float _screenWidth;
        float _screenHeight;

        Vector2 _position = new Vector2(0,0); // upper left corner of recipe selection menu display
        Size2 _slotSize = new Size2(64,64);     // size of recipe box
        int _spacing = 2;                     // space between recipe boxes
        int _borderWidth = 4;                 // pixel width from edge of boxes to edge of texture
        Size _dimensions = new Size(3,3); // num rows/columns
        float _scale = 3f;

        Dictionary<string, Recipe> _recipes = new Dictionary<string, Recipe>();
        // float _foodScale = 0.165f;
        // float _foodScale = 4f; //changed after using texture atlas
        // public float _recipeScale = 2f;

        Cook _cookingUI;
        Inventory _inventory;

        // List<Ingredient> _availableIngredients;  //all the ingredients that are in the player's inventory

        // dict of ALL recipes
        // KEY = food, VALUE = recipe's ingredients 
        // Dictionary<string, List<string>> _recipes = new Dictionary<string, List<string>>();

        // Key = recipe, Value = viable?
        // Dictionary<string, bool> _canCook = new Dictionary<string, bool>();

        // Key = (column, row) grid position, Value = recipe
        Dictionary< Point, string> _recipesDisplay = new Dictionary<Point, string>();

        // Key = (column, row) grid position, Value = entry rectangle
        // Dictionary<Point, RectangleF> _recipeAreas = new Dictionary<Point, RectangleF>();

        // Key = (column, row) position, Value = (x,y) display coordinate
        // Dictionary<Vector2, Vector2> _recipeCoordinates = new Dictionary<Vector2, Vector2>();

        // MouseState _mouseState;

        bool _debugMode = false;

        public bool loaded = false;

        //exit button
        UIButton xButton;

        // public RecipeSelection(ref Cook cookingUI, ref Inventory inventory)
        // {
        //     cookingUI._cookingVisible = false;
        //     cookingUI = cookingUI;
        //     inventory = inventory;
        // }

        public RecipeSelection(Game1 game)
        {
            _cookingUI = game.cookingGame;
            _inventory = game.inventory;
        }

        /*
        public RecipeSelection(Cook cookingUI, Inventory inventory)
        {
            cookingUI._cookingVisible = false;
            this.cookingUI = cookingUI;
            this.inventory = inventory;
        }
        */

         public void Load(ContentManager Content)
         {
            //temp patch
            // _screenWidth = Game1.instance._cameraController._screenDimensions.X;
            // _screenHeight = Game1.instance._cameraController._screenDimensions.Y;

            //load in images
            // mainUI = Content.Load<Texture2D>("ui/Fake Recipe Selection");
            // background = Content.Load<Texture2D>("ui/new camp");
            // selectedFood = Content.Load<Texture2D>("ingredient/grilled_fishScaled");

            // container = Content.Load<Texture2D>("ui/UI Container");
            // box1 = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Single");
            // box2 = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Double");
            // box1Selected = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Single Yellow");
            // box2Selected = Content.Load<Texture2D>("ui/Recipe/Ingredient Box Double Yellow");
            // recipeFrame = Content.Load<Texture2D>("ui/Recipe/Food Frame");
            // blackPlus = Content.Load<Texture2D>("ui/Recipe/Black Plus");
            // yellowPlus = Content.Load<Texture2D>("ui/Recipe/Yellow Plus");


            /* this is causing errors meanwhile the inventory is fixed
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
            */

            // this.acorn = ItemTextures.GetTexture("acorn");
            // this.apple = ItemTextures.GetTexture("apple");
            // this.carrot = ItemTextures.GetTexture("carrot");
            // this.egg = ItemTextures.GetTexture("egg");
            // this.fish = ItemTextures.GetTexture("fish");
            // this.gooseberry = ItemTextures.GetTexture("gooseberry");
            // this.meat = ItemTextures.GetTexture("meat");
            // this.mouseMelon = ItemTextures.GetTexture("mousemelon");
            // this.mushroom = ItemTextures.GetTexture("mushroom");
            // this.water = ItemTextures.GetTexture("waterjug");
            // this.wood = ItemTextures.GetTexture("wood");

            // this.grilledFish = ItemTextures.GetTexture("grilled_fish");
            // this.appleMushroomSoup = ItemTextures.GetTexture("apple_mushroom_soup");
            // this.carrotSoup = ItemTextures.GetTexture("carrot_spice_soup");
            // this.monsterSoup = ItemTextures.GetTexture("rabbit_spice_soup(1)");
            // this.rabbitSoup = ItemTextures.GetTexture("rabbit_spice_soup");


            //ADD RECIPES HERE
            // _recipes.Add("grilledFish", new Recipe("grilledFish", new List<string>() { "fish" }, true,
            //              new Point(1, 1), GetGridRect(new Point(1, 1))));
            // _recipes.Add("appleMushroomSoup", new Recipe("appleMushroomSoup", new List<string>() { "apple", "water" }, true,
            //              new Point(1, 2), GetGridRect(new Point(1, 2))));
            // _recipes.Add("carrotSoup", new Recipe("carrotSoup", new List<string>() { "carrot", "water" }, true, 
            //              new Point(1, 3), GetGridRect(new Point(1, 3))));
            // _recipes.Add("rabbitSoup", new Recipe("rabbitSoup", new List<string>() { "meat", "water" }, true,
            //              new Point(2, 1), GetGridRect(new Point(2, 1))));
            // _recipes.Add("monsterSoup", new Recipe("monsterSoup", new List<string>() { "meat", "water" }, true,
            //              new Point(2, 2), GetGridRect(new Point(2, 2))));

            // save recipe grid locations
            // foreach(Recipe recipe in _recipes.Values)
            // {
            //     _recipesDisplay.Add(recipe._gridCoord, recipe._name);
            // }

            // Load recipes from file
            LoadRecipes();

            // Fill empty space
            Point loc;
            for (int x = 1; x <= _dimensions.Width; ++x)
            {
                for (int y = 1; y <= _dimensions.Height; ++y)
                {
                    loc = new Point(x, y);
                    if (!_recipesDisplay.ContainsKey(loc))
                    {
                        _recipesDisplay.Add(loc, null);
                    }
                }
            }

            // assign positions and rectangles
            // Point loc = new Point(1, 1);
            // foreach (string recipe in _recipes.Keys)
            // {
            //     _recipesDisplay.Add(loc, recipe);
            //     _recipeAreas.Add(loc, GetGridRect(loc));
            // 
            //     // jump to next entry coord
            //     loc.Y += loc.X > _dimensions.X ? 1 : 0;
            //     loc.X = (loc.X) % _dimensions.X + 1;
            // }

            //add in the coordinates where each recipe box will be displayed (assuming origin is at (0,0)
            // _recipeCoordinates.Add(new Vector2(0, 0), new Vector2(82, 47));
            // _recipeCoordinates.Add(new Vector2(1, 0), new Vector2(339, 47));
            // _recipeCoordinates.Add(new Vector2(0, 1), new Vector2(82, 109));
            // _recipeCoordinates.Add(new Vector2(1, 1), new Vector2(339, 109));
            // _recipeCoordinates.Add(new Vector2(0, 2), new Vector2(82, 170));
            // _recipeCoordinates.Add(new Vector2(1, 2), new Vector2(339, 170));

            //multiply each (x,y) by _scale
            // foreach(KeyValuePair<Vector2, Vector2> point in _recipeCoordinates.ToList())
            // {
            //     _recipeCoordinates[point.Key] = Vector2.Multiply(point.Value, Convert.ToSingle(_scale));
            // }


            //create exit button
            Texture2D xButtonTexture = Content.Load<Texture2D>("ui/x-button");
            Vector2 buttonPos = new Vector2(Game1.instance._cameraController._screenDimensions.X * 0.9f,
                                            Game1.instance._cameraController._screenDimensions.X * 0.1f);
            xButton = new UIButton(xButtonTexture, buttonPos);
            xButton.Depth = .01f;
            xButton.Scale = 3f;
            xButton.Click += xButton_Click;

            // add resize listener to update screen size values 
            Game1.instance._cameraController.AddResizeListener(onResize);

            loaded = true;
        }

        public void Update(MouseState mouseState, KeyboardState keyState)
         {
            xButton.Update(mouseState);

            //we are calling mouseState in Draw(), so update a member variable with mousestate so we can use it in draw
            // this._mouseState = mouseState;

            //recipe selection and cooking cannot be simultaneously visible
            //if (cookingUI._cookingVisible)
            //{
            //    _visibleUI = false;
            //}

            //if(cookingUI._finished && !_visibleUI && !cookingUI._cookingVisible)
            //{
            //    UpdateInventoryAfterCooking();
            //    cookingUI._finished = false;
            //}
            
             ////////////////////////////// debugging tools \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

             //press ALT + Q to toggle visibility visiblity 
             // if( oldKeyState.IsKeyUp(Keys.Q) && keyState.IsKeyDown(Keys.Q) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
             // {
             //     //debug($"before: {_visibleUI}");
             //     _visibleUI = !_visibleUI; //toggle on/off
             //     //debug($"after: {_visibleUI}");
             // }

             //press ALT + W to switch to cooking UI with selected food
             if(_oldKeyState.IsKeyUp(Keys.W) && keyState.IsKeyDown(Keys.W) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
            {
               // _visibleUI = false;
                _cookingUI._finished = false;
                _cookingUI._cookingVisible = true;
                _cookingUI.foodName = _selectedFood; //selectedFood = grilledFish
            }

            //press ALT + E to print available recipes 
            // if (_oldKeyState.IsKeyUp(Keys.E) && keyState.IsKeyDown(Keys.E) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
            // {
            //     Debug.WriteLine("Printing recipes: ");
            //     foreach (string food in CookableRecipes())
            //     {
            //         debug($"{food}");
            //     }
            // }

            //press ALT + W to add water
            //this adds water lol
            if (_oldKeyState.IsKeyUp(Keys.R) && keyState.IsKeyDown(Keys.R) && keyState.IsKeyDown(Keys.LeftAlt) && _debugMode)
            {
                //inventory.addIngredient(inventory.water, "water");
            }

            //press HOME to toggle debug mode
            if (_oldKeyState.IsKeyUp(Keys.Home) && keyState.IsKeyDown(Keys.Home))
            {
                _debugMode = !_debugMode;
                Debug.WriteLine($"Debug mode turned {_debugMode}");
            }

            if (_oldKeyState.IsKeyUp(Keys.H) && keyState.IsKeyDown(Keys.H)) { }
            //this._visibleUI = true;

            // reset hover over
            _hoverOver = null;

            // check for recipe selected
            foreach (Recipe recipe in _recipes.Values)
            {
                if (recipe._area.Contains(mouseState.Position))
                {
                    string recipeFood = recipe._name;
                    // Texture2D box = pickBoxForRecipe(recipeFood, point.Value);
                    if (mouseState.LeftButton == ButtonState.Pressed && recipe._canCook)
                        SwitchToCooking(recipeFood);
                    else
                        _hoverOver = recipe._gridCoord;
                }
            }

            _oldKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            //Debug.WriteLine($"recipeMenu is being drawn - _visibleUI = {_visibleUI}");
            // float UIdepth = 0.04f;
            //draw the fake background only when debug mode is on 
            // float bgOpacity = _debugMode ? 1f : 0f;
            // spriteBatch.Draw(background, new Vector2(-_screenWidth * 4 / 3, 0), null, Color.White * bgOpacity, 0f, Vector2.Zero, 1.4f, SpriteEffects.None, 1f);

            // float uiOpacity = 1f;
            //float uiOpacity = _visibleUI ? 1f : 0f; //disable/enable visibility for the cooking UI through its opacity value
            //spriteBatch.Draw(mainUI, new Vector2(0, 0), null, Color.White * uiOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);

            //if (_visibleUI)
            //{
            //draw the UI container
            // spriteBatch.Draw(container, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, UIdepth);

            // TODO: Draw possible recipes

            // Draw menu background
            TextureAtlasManager.DrawTexture(spriteBatch, "UI", "Menu", _position, Color.White, _scale);

            // Draw hover over TODO: Draw hover recipe information display
            if (_hoverOver.HasValue)
                spriteBatch.DrawRectangle(GetGridRect(_hoverOver.Value), _hoverColor);

            Point loc;
            // Draw recipes
            for(int x = 1; x <= _dimensions.Width; ++x)
            {
                for(int y = 1; y <= _dimensions.Height; ++y)
                {
                    loc = new Point(x, y);
                    if(_recipesDisplay[loc] != null && _recipes[_recipesDisplay[loc]]._canCook)
                    {
                        TextureAtlasManager.DrawTexture(spriteBatch, "Item", _recipes[_recipesDisplay[loc]]._name, GridToWorld(loc), Color.White);
                    }
                    else
                    {
                        TextureAtlasManager.DrawTexture(spriteBatch, "UI", "QuestionMark", GridToWorld(loc), Color.White);
                    }
                }
            }
                
            xButton.Draw(spriteBatch);

            //}
        }

        public void UpdatePossibleRecipes()
        {
            foreach(Recipe recipe in _recipes.Values)
            {
                recipe._canCook = CanCook(recipe);
            }
            // List<string> cookableRecipes = CookableRecipes();
            // int numRecipes = cookableRecipes.Count;
            // foreach (KeyValuePair<Vector2, Vector2> point in _recipeCoordinates)
            // {
            //     if (numRecipes > 0)
            //     {
            //         string recipeFood = cookableRecipes.ElementAt(numRecipes - 1);
            //         List<string> recipeIngredients = _recipes[recipeFood];
            // 
            //         int box = pickBoxSize(recipeFood);
            // 
            //         // spriteBatch.Draw(box, point.Value, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, UIdepth);
            //         // spriteBatch.Draw(recipeFrame, point.Value, null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, UIdepth);
            // 
            //         // spriteBatch.Draw(recipeFood, point.Value, null, Color.White, 0f, Vector2.Zero, _recipeScale, SpriteEffects.None, UIdepth);
            //         // spriteBatch.Draw(recipeIngredients.ElementAt(0), new Vector2(point.Value.X + 60*_scale, point.Value.Y), null, Color.White, 0f, Vector2.Zero, _foodScale, SpriteEffects.None, UIdepth);
            //         // TextureAtlasManager.DrawTexture(spriteBatch, "Item", recipeFood, point.Value, Color.White, _recipeScale);
            //         // TextureAtlasManager.DrawTexture(spriteBatch, "Item", recipeIngredients.ElementAt(0),
            //         //                                 new Vector2(point.Value.X + 60 * _scale, point.Value.Y),
            //         //                                 Color.White, _foodScale);
            //         if (recipeIngredients.Count == 2)
            //         {
            //             // spriteBatch.Draw(recipeIngredients.ElementAt(1), new Vector2(point.Value.X + 160 * _scale, point.Value.Y), null, Color.White, 0f, Vector2.Zero, _foodScale, SpriteEffects.None, UIdepth);
            //             TextureAtlasManager.DrawTexture(spriteBatch, "Item", recipeIngredients.ElementAt(1),
            //                                             new Vector2(point.Value.X + 160 * _scale, point.Value.Y),
            //                                             Color.White, _foodScale);
            //             spriteBatch.Draw(yellowPlus, new Vector2(point.Value.X + 119 * _scale, point.Value.Y + 30), null, Color.White, 0f, Vector2.Zero, _scale, SpriteEffects.None, UIdepth);
            //         }
            // 
            // 
            //         numRecipes--;
            //     }
            // }
        }

        //when xButton is clicked, close inventory
        private void xButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Recipe Menu Exit Clicked!");
            //Game1.instance.inventory.showInv = false;
            Game1.instance.UI.SwitchState(UIState.None);
        }

        void debug(string message)
         {
            Debug.WriteLineIf(_debugMode, message);
         }

        //will return a list of the foods that can be cooked
        // List<string> CookableRecipes()
        // {
        //     List<string> cookable = new List<string>();
        // 
        //     foreach(KeyValuePair<string, List<string>> recipe in _recipes) //iterate through the recipes
        //     {
        //         List<string> ingredientsNeeded = recipe.Value;
        //         bool ingredientsAvailable = true; //until proven otherwise
        // 
        //         foreach(string ingredientNeeded in ingredientsNeeded) //iterate through all the ingredientsNeeded for the recipe
        //         {
        //             bool ingredientFound = false;
        //             foreach(Ingredient inventoryIngredient in inventory.ingredientList) //iterate through the available ingredients in inventory 
        //             {
        //                 if (ingredientNeeded == inventoryIngredient._name) //check if the ingredient needed is in the available ingredients
        //                     ingredientFound = true;
        //             }
        //             //missing ingredient means we can't make the recipe!
        //             if (!ingredientFound)
        //                 ingredientsAvailable = false;
        //         }
        // 
        //         //if all ingredients were found, we can cook it!
        //         if(ingredientsAvailable)
        //             cookable.Add(recipe.Key);
        //     }
        //     return cookable;
        // }

        // int pickBoxSize(string recipeFood)
        // {
        //     //pick the appropriate sized box for the amount of ingredients
        //     List<string> recipeIngredients = _recipes[recipeFood];
        //     return recipeIngredients.Count == 1 ? 1 : 2;
        // }

        //determine whether a recipe getting displayed gets a short or long box or whether its highlighted or not
        // Texture2D pickBoxForRecipe(string recipeFood, Vector2 point)
        // {
        //     Texture2D box;
        // 
        //     //pick the appropriate sized box for the amount of ingredients
        //     List<string> recipeIngredients = _recipes[recipeFood];
        //     box = recipeIngredients.Count == 1 ? box1 : box2; 
        // 
        //     //determine whether or not the mouse is hovering over the box and if so highlight the box!
        //     bool highlight = IsPointOverRecipeBox(_mouseState.Position, box, point, _scale);
        //     if(highlight)
        //     {
        //         box = box == box1 ? box1Selected : box2Selected; 
        //     }
        // 
        //     return box;
        // }


        //this is my version of IsPointover()
        //differences include accepting Texture2D as a @param instead of a Sprite and factoring scale into calculations
        bool IsPointOverRecipeBox(Point xy, Texture2D image, Vector2 pos,  float scale)
        {
            int intScale = (int)scale;
            //assume origin is at (0,0)
            Rectangle rect = new Rectangle(new Point((int)pos.X, (int)pos.Y), new Point(image.Width*intScale, image.Height*intScale));
            return (rect.Contains(xy.X, xy.Y));
        }

        //checks to see if a recipe box is being clicked
        // bool IsRecipeBeingClicked(Point mousePoint, Texture2D image, Vector2 pos, float scale)
        // {
        //     bool mouseClicked = this._mouseState.LeftButton == ButtonState.Pressed;
        //     return IsPointOverRecipeBox(mousePoint, image, pos, scale) && mouseClicked;
        // }

        //use this when a recipe box is clicked to cook that recipe!
        void SwitchToCooking(string selectedFood)
        {
            //make sure the recipe selection UI is active/visible when implementing this logic
            //if (_visibleUI)
            //{
                //_visibleUI = false; //recipe selecion is no longer visible
                _cookingUI._cookingVisible = true; //cooking ui is now visible

                _cookingUI._finished = false; //resets cooking UI
                _cookingUI.foodName = selectedFood; //display food being cooked in the cooking ui
            //}

            //UI Manager switched to cooking
            Game1.instance.UI.SwitchState(UIState.CookingGame);
        }

        // returns space coordinate of upper left hand corner of given grid box
        Vector2 GridToWorld(Point index)
        {
            return new Vector2((index.X - 1) * (_slotSize.Width + _spacing) + _borderWidth, 
                               (index.Y - 1) * (_slotSize.Height + _spacing) + _borderWidth) * _scale;
        }

        // returns rectangle describing world space of given grid box
        RectangleF GetGridRect(Point index)
        {
            return new RectangleF(GridToWorld(index), _slotSize * _scale);
        }

        // returns whether a particular recipe can be cooked or not based on inventory state (by grid coord)
        bool CanCook(Point index)
        {
            if (_recipesDisplay[index] == null)
                return false;
            return CanCook(_recipes[_recipesDisplay[index]]);
        }

        // returns whether a particular recipe can be cooked or not based on inventory state (by name)
        bool CanCook(string recipeName)
        {
            return CanCook(_recipes[recipeName]);
        }

        // returns whether a particular recipe can be cooked or not based on inventory state (by recipe)
        bool CanCook(Recipe recipe)
        {
            foreach (string ingredient in recipe._ingredients)
            {
                bool found = false;
                for (int i = 0; i < _inventory.ingredientList.Count && !found; ++i)
                {
                    if (_inventory.ingredientList[i]._name == ingredient) // found
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        public List<string> GetIngredients(string recipe)
        {
            return _recipes[recipe]._ingredients;
        }

        // called when window is resized
        private void onResize(Vector2 size)
        {
            _screenWidth = size.X;
            _screenHeight = size.Y;
        }
        
        void LoadRecipes()
        {
            // Set up stream
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WillowWoodRefuge.Content.dialogue.Recipe_Info.tsv");

            // grow system from file
            using (StreamReader reader = new StreamReader(stream))
            {
                // throw away first line (headers)
                string line = reader.ReadLine();

                // list info
                Point loc = new Point(1, 1);

                while (!reader.EndOfStream)
                {
                    RectangleF area = GetGridRect(loc);
                    line = reader.ReadLine();

                    // create new recipe
                    Recipe recipe = ParseRecipe(line, loc, area);
                    _recipes.Add(recipe._name, recipe);
                    _recipesDisplay.Add(loc, recipe._name);

                    // jump to next entry coord
                    loc.Y += loc.X >= _dimensions.Width ? 1 : 0;
                    loc.X = (loc.X) % _dimensions.Width + 1;
                }
            }
        }

        Recipe ParseRecipe(string unparsed, Point pos, RectangleF area)
        {
            string[] values = unparsed.Split('\t');

            // add all listed ingredients
            List<string> ingredients = new List<string>();
            for(int i = 4; i < values.Length; ++i)
            {
                if(values[i].Length > 0)
                    ingredients.Add(values[i]);
            }

            return new Recipe(values[0], values[1], values[2], values[3], ingredients, false, pos, area);
        }
    }
}