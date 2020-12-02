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

namespace IngredientRun
{
    class Inventory
    {
        Texture2D inventorySq, acornT, appleT, fishT, meatT, woodT;
        Ingredient fish, acorn, apple, meat, wood;
        public bool showInv = false;
        bool handsFull = false;
        KeyboardState oldKeyState;

        List<Ingredient> ingredientList = new List<Ingredient>();
        List<Vector2> boxes = new List<Vector2>();
        Dictionary<Vector2, Vector2> boxDict = new Dictionary<Vector2, Vector2>(); // key = [i,j], value = (x,y)

        //for timing how fast items fall down inventory
        float timeSinceLastDrop = 0f;

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

        public Inventory()
        {
        }

        public void Load(ContentManager Content)
        {

            initializeInventoryGrid();

            //inventorySq = Content.Load<Texture2D>("ui/Temp Inventory"); //minecraft inventory
            inventorySq = Content.Load<Texture2D>("ui/Inventory/Inventory Backpack and Grid");
            acornT = Content.Load<Texture2D>("Ingredient/acorn");
            appleT = Content.Load<Texture2D>("Ingredient/apple");
            fishT = Content.Load<Texture2D>("Ingredient/fish");
            meatT = Content.Load<Texture2D>("Ingredient/meat");
            woodT = Content.Load<Texture2D>("Ingredient/wood");

            acorn = new Ingredient(acornT, randomBox());
            apple = new Ingredient(appleT, randomBox());
            fish = new Ingredient(fishT, randomBox());
            meat = new Ingredient(meatT, randomBox());
            wood = new Ingredient(woodT, randomBox());

            meat.scale = .25f;
            fish.scale = .25f;
            acorn.scale = .4f;
            //Debug.WriteLine(meat.Scale);

            ingredientList.Add(acorn);
            ingredientList.Add(apple);
            ingredientList.Add(fish);
            ingredientList.Add(meat);
            ingredientList.Add(wood);
            shakeBag();

            foreach(Ingredient ing in ingredientList)
            {
                //set origin at center
                ing.Origin = new Vector2(ing.img.Bounds.Center.X, ing.img.Bounds.Center.Y);
                //Debug.WriteLine($"{ing.img} is highest = {Highest(ing)}");
            }

        }

        public void Update(MouseState mouseState, KeyboardState keyState)
        {

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                //Print mouse cursor position to debug console
                //Debug.WriteLine($"{mouseState.Position.X} {mouseState.Position.Y}");
            }
            foreach (Ingredient ingredient in ingredientList) {

                    //use mouse to move objects
                    MoveIngredient(ingredient, mouseState);

                    //rotate objects when space bar pressed
                    if (ingredient.holding && oldKeyState.IsKeyUp(Keys.Space) && keyState.IsKeyDown(Keys.Space))
                    {
                        Debug.WriteLine("Rotate!");
                        ingredient.Rotation += Convert.ToSingle(Math.PI) / 2f; //rotate by 90 degrees                                                                      
                    }

                    //inventory gravity - objects fall to bottom and stack on each other
                    //if (canIngredientFall(ing) && (ing.timeSinceLastDrop > 1f)) //if 1 sec since last drop and box directly under is empty
                    
                    if(ingredient.falling) 
                    {
                        //ingredient will fall until it reaches its target
                        Vector2 targetBox = boxIndexBelow(ingredient);
                        float targetY = boxDict[targetBox].Y;
                        
                        //target has not been met yet
                        if( ingredient.pos.Y < targetY)
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
                    else {
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
            if (oldKeyState.IsKeyUp(Keys.E) && keyState.IsKeyDown(Keys.E))
            {
                showInv = !showInv;
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
            spriteBatch.Draw(inventorySq, new Vector2(0,0), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.4f);
            
            //for (int i=0; i < ings.Count; i++)
            //{
            //    Vector2 ingredientPos = new Vector2(200 + inventorySq.Width / 2, 50 + (i * (50 / 3)) );
            //    spriteBatch.Draw( (ings[i].img), ingredientPos, null, Color.White, ings[i].rotation, Vector2.Zero, SpriteEffects.None, 0f); 
            //}

            for (int pos = (ingredientList.Count() - 1); pos >= 0; --pos)
            {
                ingredientList[pos].Draw(spriteBatch, 1);
            }
        }

        


        //mouse interactions with ingredients
        void MoveIngredient(Ingredient ing, MouseState mouseState)
        {

            if (IsPointOver(mouseState.Position, ing) && !handsFull)
            {
                if ( mouseState.LeftButton == ButtonState.Pressed )
                {
                    //ingredient is clicked on
                    Debug.WriteLine($"{ing.img} clicked!");//debugging

                    //can only select if there is no item directly above
                    if( !isIngredientStackedOn(ing) )
                    {
                        ing.holding = true;
                        handsFull = true;
                    }
                    else
                    {
                        Debug.WriteLine($"{ing.img} is stacked on!");
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
                Debug.WriteLine($"{ing.img} - {ing.index}");
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
            foreach(Ingredient ingredient in ingredientList)
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

                    if( !sidePocketColumn )
                    {
                        //we are in the main grid (excluding the first and last columns for the backpack's side pockets)
                        boxPosX = topLeft.X + ( (i - 1) * gridWidthMargin);
                    }
                    else
                    {
                        if (sidePocketRow)
                        {
                            if(i == 0)
                            {
                                //left side backpack pocket
                                boxPosX = topLeft.X - 180;
                            }
                            else
                            {
                                //right side backpack pocket
                                boxPosX = topLeft.X + ( (i - 2) * gridWidthMargin) + 180;
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



    }
}
