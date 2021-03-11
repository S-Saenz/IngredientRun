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

namespace WillowWoodRefuge
{
    public class Inventory
    {
        Texture2D inventorySq, acornT, appleT, fishT, meatT, woodT;
        public Texture2D acorn, apple, appleMushroomSoup, carrot, carrotSoup, egg, fish, gooseberry, grilledFish, meat, monsterSoup, mouseMelon, rabbitSoup, water, wood;
        // List<Texture2D> ingredientTextures;
        Ingredient FISH, ACORN, APPLE, MEAT, WOOD;
        public bool showInv = false;
        bool handsFull = false;
        KeyboardState oldKeyState;

        public List<Ingredient> ingredientList = new List<Ingredient>();
        List<Vector2> boxes = new List<Vector2>();
        Dictionary<Vector2, Vector2> boxDict = new Dictionary<Vector2, Vector2>(); // key = [i,j], value = (x,y)

        //for timing how fast items fall down inventory
        //float timeSinceLastDrop = 0f;

        //config for temp inventory (Minecraft)
        //int gridWidth = 9;
        //int gridHeight = 3;
        //float gridWidthMargin = 100;
        //float gridHeightMargin = 100;
        //Vector2 topLeft = new Vector2(250, 162);

        //config for backpack inventory - inluding side pockets
        int gridWidth = 6;
        int gridHeight = 5;
        float gridWidthMargin = 140;
        float gridHeightMargin = 140;
        Vector2 topLeft = new Vector2(690, 125);

        Texture2D lilBackpack;
        Sprite backpackButton;

        public Inventory()
        {
        }

        public void Load(ContentManager Content)
        {
            initializeInventoryGrid();

            //inventorySq = Content.Load<Texture2D>("ui/Temp Inventory"); //minecraft inventory
            inventorySq = Content.Load<Texture2D>("ui/Inventory/Inventory Backpack and Grid");

            acorn = Content.Load<Texture2D>("Ingredient/acornScaled");
            apple = Content.Load<Texture2D>("Ingredient/appleScaled");
            appleMushroomSoup = Content.Load<Texture2D>("Ingredient/apple_mushroom_soupScaled");
            carrot = Content.Load<Texture2D>("Ingredient/carrotScaled");
            carrotSoup = Content.Load<Texture2D>("Ingredient/carrot_spice_soupScaled");
            egg = Content.Load<Texture2D>("Ingredient/eggScaled");
            gooseberry = Content.Load<Texture2D>("Ingredient/gooseberryScaled");
            fish = Content.Load<Texture2D>("Ingredient/fishScaled");
            grilledFish = Content.Load<Texture2D>("Ingredient/grilled_fishScaled");
            meat = Content.Load<Texture2D>("Ingredient/meatScaled");
            monsterSoup = Content.Load<Texture2D>("Ingredient/rabbit_spice_soup (1)Scaled");
            mouseMelon = Content.Load<Texture2D>("Ingredient/mousemelonScaled");
            rabbitSoup = Content.Load<Texture2D>("Ingredient/rabbit_spice_soupScaled");
            water = Content.Load<Texture2D>("Ingredient/waterjugScaled");
            wood = Content.Load<Texture2D>("Ingredient/woodScaled");

            // ingredientTextures = new List<Texture2D>() { acorn, apple, appleMushroomSoup, carrot, carrotSoup, egg, fish, gooseberry, grilledFish, meat, mouseMelon, rabbitSoup, water, wood };

            ingredientList.Add(new Ingredient(ItemTextures.GetTexture("acornScaled"), randomBox(), "acorn"));
            ingredientList.Add(new Ingredient(ItemTextures.GetTexture("appleScaled"), randomBox(), "apple"));
            ingredientList.Add(new Ingredient(ItemTextures.GetTexture("fishScaled"), randomBox(), "fish"));
            ingredientList.Add(new Ingredient(ItemTextures.GetTexture("meatScaled"), randomBox(), "meat"));
            ingredientList.Add(new Ingredient(ItemTextures.GetTexture("woodScaled"), randomBox(), "wood"));

            // ACORN = new Ingredient(acorn, randomBox());
            // APPLE = new Ingredient(apple, randomBox());
            // FISH = new Ingredient(fish, randomBox());
            // MEAT = new Ingredient(meat, randomBox());
            // WOOD = new Ingredient(wood, randomBox());    

            // MEAT.scale = .25f;
            // FISH.scale = .25f;
            // ACORN.scale = .4f;
            //Debug.WriteLine(meat.Scale);

            // ingredientList.Add(ACORN);
            // ingredientList.Add(APPLE);
            // ingredientList.Add(FISH);
            // ingredientList.Add(MEAT);
            // ingredientList.Add(WOOD);
            // addIngredient(water);

            shakeBag();

            foreach (Ingredient ing in ingredientList)
            {
                //set origin at center
                ing.Origin = new Vector2(ing.img.Bounds.Center.X, ing.img.Bounds.Center.Y);
                //Debug.WriteLine($"{ing.img} is highest = {Highest(ing)}");
            }

            //inventory icon 
            float _screenWidth = 1728;
            float _screenHeight = 972;
            lilBackpack = Content.Load<Texture2D>("ui/Inventory/BackpackPixel");
            backpackButton = new Sprite(lilBackpack, new Vector2(_screenWidth / 4, _screenHeight / 4));
            backpackButton.Depth = .01f;
            backpackButton.Scale = 2f;
            //Debug.

        }

        public void Update(MouseState mouseState, KeyboardState keyState)
        {
            //bool boxClicked = false;
            //Vector2 clickedBox = new Vector2(-1,-1); //just give it a dummy temp value

            //Debug.WriteLine(mouseState.Position);

            if (mouseState.LeftButton == ButtonState.Pressed)

            {

                //Print mouse cursor position to debug console

                Debug.WriteLine($"{mouseState.Position.X} {mouseState.Position.Y}");


                //boxClicked = !closestBoxToMouse(mouseState).Equals(new Vector2(-1, -1));              

                //if(boxClicked) 
                //    clickedBox = closestBoxToMouse(mouseState);

                //Debug.WriteLine($"Mouse clicked!\nboxClicked = {boxClicked}");

            }
            foreach (Ingredient ingredient in ingredientList)
            {

                //use mouse to move ingredient if it's in the clicked Box
                //if(boxDict[ingredient.index] == clickedBox)
                MoveIngredient(ingredient, mouseState);

                //rotate objects when space bar pressed
                if (ingredient.holding && oldKeyState.IsKeyUp(Keys.Space) && keyState.IsKeyDown(Keys.Space))
                {
                    Debug.WriteLine("Rotate!");

                    ingredient.Rotation += Convert.ToSingle(Math.PI) / 2f; //rotate by 90 degrees     
                    ingredient.updateOrientation();

                    //if ingredient is larger than one inventory square
                    if (ingredient.doubleSquare)
                    {
                        ingredient.updateIndex2();
                    }

                    //Debug.WriteLine($"{ingredient.img} rotation: pi/{ Math.Round( Math.PI/ingredient.Rotation ) }");
                    Debug.WriteLine($"{ingredient.img} {ingredient.Rotation}");
                }

                //inventory gravity - objects fall to bottom and stack on each other
                //if (canIngredientFall(ing) && (ing.timeSinceLastDrop > 1f)) //if 1 sec since last drop and box directly under is empty

                if (ingredient.falling)
                {
                    //ingredient will fall until it reaches its target
                    Vector2 targetBox = boxIndexBelow(ingredient);
                    float targetY = boxDict[targetBox].Y;

                    //target has not been met yet
                    if (ingredient.pos.Y < targetY)
                    {
                        //ingredient.pos.Y += 10;
                        ingredient.pos = new Vector2(ingredient.pos.X, ingredient.pos.Y + 5);
                    }

                    //ingredient has fallen to target!
                    else
                    {
                        ingredient.falling = false;
                        //assign ingredient to that box in the inventory
                        ingredient.index = targetBox;
                        ingredient.pos = boxDict[targetBox];
                    }

                }
                else
                {
                    //ingredient is not falling! But will start to fall if it can
                    ingredient.falling = canIngredientFall(ingredient);
                }

            }

            //Press G to shake bag and randomly redistribute items in inventory
            if (oldKeyState.IsKeyUp(Keys.G) && keyState.IsKeyDown(Keys.G))
            {
                Debug.WriteLine("Bag has been shaken");
                shakeBag();
            }

            //press E for inventory
            if (oldKeyState.IsKeyUp(Keys.I) && keyState.IsKeyDown(Keys.I))
            {
                showInv = !showInv;
            }

            if (oldKeyState.IsKeyUp(Keys.V) && keyState.IsKeyDown(Keys.V))
            {
                Debug.WriteLine("V pressed");
                Random rnd = new Random();
                int randIndex = rnd.Next(ItemTextures._allItems.Count);
                addIngredient(null, ItemTextures._allItems[randIndex]);
            }
            oldKeyState = keyState;



            //-------------------------------------------- DEBUGGING -----------------------------------------
            //rotation testing 
            //fish.Rotation += .01f;
            //fish.pos.X += 1;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(inventorySq, new Vector2(200, 50), null, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);
            spriteBatch.Draw(inventorySq, new Vector2(0, 0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);

            //for (int i=0; i < ings.Count; i++)
            //{
            //    Vector2 ingredientPos = new Vector2(200 + inventorySq.Width / 2, 50 + (i * (50 / 3)) );
            //    spriteBatch.Draw( (ings[i].img), ingredientPos, null, Color.White, ings[i].rotation, Vector2.Zero, SpriteEffects.None, 0f); 
            //}

            for (int pos = (ingredientList.Count() - 1); pos >= 0; --pos)
            {
                ingredientList[pos].Draw(spriteBatch);
            }

            // backpackButton.Draw(spriteBatch);
            // spriteBatch.Draw(lilBackpack, new Vector2(1728 / 3, 972 / 3), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, .01f);
        }




        //mouse interactions with ingredients
        void MoveIngredient(Ingredient ing, MouseState mouseState)
        {

            if (!handsFull)
            {
                if (mouseState.LeftButton == ButtonState.Pressed) //if player is not holding anything and clicks
                {
                    //this is the box that has been clicked (will return an invalid box if player clicks outside of inventory) 
                    Vector2 clickedBox = closestBoxToMouse(mouseState);

                    //make sure there is not another ingredient above AND the ingredient's box is being clicked
                    if (!isIngredientStackedOn(ing) && boxDict[ing.index] == clickedBox)
                    {
                        ing.holding = true;
                        handsFull = true;
                    }
                    else
                    {
                        Debug.WriteLine($"{ing.img} is stacked on and can't be moved!");
                    }
                }
            }

            //have ingredient follow mouse pointer 
            if (ing.holding)
            {
                ing.SetPosByMouse(mouseState.Position);
            }

            //ingredient is let go after selection
            if (ing.holding && mouseState.LeftButton != ButtonState.Pressed)
            {
                handsFull = false;
                ing.holding = false;

                //snap ingredient to grid
                ing.pos = closestEmptyBox(ing);
                ing.index = findGridIndex(ing.pos);
                //Debug.WriteLine($"{ing.img} - {ing.index}"); //ingredient snaps where?
            }
        }

        bool IsPointOver(Point xy, Sprite sprite)
        {
            return (sprite.Bounds().Contains(xy.X, xy.Y));
        }

        //check if ingredient can fall down the inventory grid
        public bool canIngredientFall(Ingredient ingredient)
        {
            if (ingredient.index.Y == gridHeight - 1)
            {
                return false; //ingredient is in bottom row
            }

            Vector2 underBox = boxIndexBelow(ingredient);
            return isSquareEmpty(underBox); //return whether or not the box below is empty
        }

        //randomize ingredient placement
        public void shakeBag()
        {
            foreach (Ingredient ingredient in ingredientList)
            {
                assignDistinctSpace(ingredient);
            }
        }

        //create the inventory grid and populate the boxDict
        public void initializeInventoryGrid()
        {
            Vector2 gridIndex;
            float boxPosX;
            float boxPosY;
            Vector2 boxPos;

            //these for loops will set the coordinates for each square in the inventory grid
            for (int i = 0; i < gridWidth; i++) //space out columns 
            {
                for (int j = 0; j < gridHeight; j++) //space out rows 
                {

                    //add in the pockets on the sides of the backpack - they deviate from the grid a bit
                    bool sidePocketColumn = (i == 0) || (i == (gridWidth - 1)); //loop is in first or last column
                    bool sidePocketRow = j >= (gridHeight - 2); //loop is in last 2 rows

                    if (!sidePocketColumn)
                    {
                        //we are in the main grid (excluding the first and last columns for the backpack's side pockets)
                        boxPosX = topLeft.X + ((i - 1) * gridWidthMargin);
                    }
                    else
                    {
                        if (sidePocketRow)
                        {
                            if (i == 0)
                            {
                                //left side backpack pocket
                                boxPosX = topLeft.X - 180;
                            }
                            else
                            {
                                //right side backpack pocket
                                boxPosX = topLeft.X + ((i - 2) * gridWidthMargin) + 180;
                            }
                        }
                        else
                        {
                            //this is an empty square (above the side pockets where there is no grid)
                            continue;
                        }
                    }

                    //add the grid square to the dictionary!
                    boxPosY = topLeft.Y + (j * gridHeightMargin);
                    boxPos = new Vector2(boxPosX, boxPosY);
                    gridIndex = new Vector2(i, j);

                    boxes.Add(boxPos); //rearrange code so this line will not be necessary
                    boxDict.Add(gridIndex, boxPos); //add the box to the dictionary 
                    //Debug.WriteLine($"{gridIndex}  {boxPos}");
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //////////////////////// HELPER FUNCTIONS ////////////////////////////////
        //////////////////////////////////////////////////////////////////////////

        //add a new ingredient into the inventory if there's space!
        public bool addIngredient(Texture2D texture, string name)
        {
            if (ingredientList.Count == boxes.Count - 1)
            {
                Debug.WriteLine("Inventory Full!");
                return false;
            }

            Ingredient newIngredient = new Ingredient(texture, randomBox(), name);
            ingredientList.Add(newIngredient);
            assignDistinctSpace(newIngredient);
            return true;
        }

        //remove an ingredient from the inventory
        public void removeIngredient(Ingredient ingredient)
        {
            ingredientList.Remove(ingredient);
        }

        public void removeIngredient(Texture2D ingredientTexture)
        {
            bool done = false; //ensure only one ingredient is removed if there are duplicates
            foreach (Ingredient ingredient in ingredientList.ToList())
            {
                if (ingredient.img == ingredientTexture && !done)
                {
                    ingredientList.Remove(ingredient);
                    done = true;
                }

            }
        }

        //returns grid index of a random box in the inventory
        public Vector2 randomBox()
        {
            Random rnd = new Random();
            int randIndex = rnd.Next(boxes.Count);
            Vector2 coordinate = boxes[randIndex]; //x,y pos of the grid box
            return findGridIndex(coordinate);
        }

        //check if square has an ingredient in it 
        public bool isSquareEmpty(Vector2 boxIndex)
        {
            //check every single ingredient's position to make sure none are in that square
            foreach (Ingredient ingredient in ingredientList)
            {
                if (ingredient.index == boxIndex)
                {
                    return false; //ingredient found in square!
                }
            }
            return true; //it's empty!
        }

        //take in (x,y) pos of a square to find its index within the inventory grid
        public Vector2 findGridIndex(Vector2 coordinate)
        {
            Vector2 gridIndex = new Vector2(-1, -1); //random initialization

            foreach (KeyValuePair<Vector2, Vector2> pair in boxDict)
            {
                if (pair.Value == coordinate)
                {
                    //a match in pos coordinates has been found!
                    gridIndex = pair.Key;
                    break;
                }
            }
            return gridIndex;
        }

        //checks if there is an ingredient in the box immediately above
        public bool isIngredientStackedOn(Ingredient ingredient)
        {
            //Vector2 upperBox = new Vector2(ingredient.pos.X, ingredient.pos.Y - gridHeight); //box directly above ingredient
            //return isSquareEmpty(upperBox); 
            return !isSquareEmpty(boxIndexAbove(ingredient));
        }

        //returns the grid index of the inventory box above the ingredient
        public Vector2 boxIndexAbove(Ingredient ingredient)
        {
            ///return boxDict[new Vector2(ingredient.index.X, ingredient.index.Y + 1)]; //returns x,y of box above
            return new Vector2(ingredient.index.X, ingredient.index.Y - 1);
        }

        //returns the grid index of the inventory box below the ingredient
        public Vector2 boxIndexBelow(Ingredient ingredient)
        {
            return new Vector2(ingredient.index.X, ingredient.index.Y + 1);
        }

        //put an ingredient in a random inventory square, but assure it is empty before placing it
        public void assignDistinctSpace(Ingredient ingredient)
        {
            Vector2 randBox = randomBox();

            if (isSquareEmpty(randBox))
            {
                //assign ingredient to empty box in inventory
                ingredient.pos = boxDict[randBox];
                ingredient.index = randBox;
            }
            else
            {
                assignDistinctSpace(ingredient); //try again
            }
        }

        //calculate closest, empty box to ingredient  and return its x,y pos
        public Vector2 closestEmptyBox(Ingredient ingredient)
        {
            //create a dictionary of boxes w their respective distance from ingredient
            Dictionary<Vector2, float> boxDistanceDictionary = new Dictionary<Vector2, float>();

            //add box coordinate and distance pair to boxDistanceDictionary
            foreach (Vector2 box in boxes)
            {
                float distance = Vector2.Distance(ingredient.pos, box); //distance between ingredient and given box
                boxDistanceDictionary.Add(box, distance);
            }

            //sort the dictionary using LINQ to specify sorting by value (distance)
            var sortedBoxes = from pair in boxDistanceDictionary
                              orderby pair.Value ascending
                              select pair;

            //for debugging's sake
            //foreach(KeyValuePair<Vector2, float> pair in sortedBoxes)
            //{
            //    //Debug.WriteLine("{0}: {1}", pair.Key, pair.Value);

            //    //we are going to be printing the gridIndex and distance 
            //    foreach(KeyValuePair<Vector2, Vector2> box in boxDict)
            //    {
            //        if(pair.Key == box.Value)
            //        {
            //            //Debug.WriteLine($"{box.Key}: {pair.Value}");
            //        }
            //    }
            //}

            bool assigned = false; //flag for when ingredient has been assigned
            int skip = 0;          //when closest box(es) occupied, skip index by value
            Vector2 closestBox = new Vector2(-1, -1);    //this variable will carry the answer (given a random initialization)

            //this while loop will try each box starting with the closest
            while (!assigned)
            {
                closestBox = sortedBoxes.ElementAt(skip).Key;

                //find grid index of closestBox in boxDict
                Vector2 gridIndex = findGridIndex(closestBox);

                //this is where the magic happens
                if (isSquareEmpty(gridIndex) || (gridIndex == ingredient.index))
                {
                    //closestBox is empty! or is the ingredient's original square
                    assigned = true; //exit the loop
                    Debug.WriteLine(gridIndex + "is empty and closest!");
                }
                else
                {
                    skip++; //make loop check the next box in sortedBoxes
                    Debug.WriteLine(gridIndex + "is not empty! Moving on!");
                }
            }
            return closestBox; //final answer
        }

        public Vector2 closestBoxToMouse(MouseState mouseState)
        {
            Vector2 mousePoint = new Vector2(mouseState.Position.X, mouseState.Position.Y);
            Vector2 closestBox = randomBox();
            float closestDistance = Vector2.Distance(mousePoint, closestBox);

            //iterate through the list of boxes to find the closest!
            foreach (Vector2 box in boxes)
            {
                float distance = Vector2.Distance(mousePoint, box); //distance between point and given boxfloat closestDistance = Vector2.Distance(mousePoint, box); //distance betwen point and the current closest box
                closestBox = distance < closestDistance ? box : closestBox; //switch closestBox if this box is closer
                closestDistance = distance < closestDistance ? distance : closestDistance; //switch closestDistance if box is cl
            }

            //gridWidthMargin is how wide the inventory boxes are; if the closest box is farther, the player is clicking outside of the inventory
            if (closestDistance > this.gridWidthMargin)
                closestBox = new Vector2(-1, -1); //we'll know if the player clicked outside the inventory by returning a negative distance

            return closestBox;
        }



    }
}
