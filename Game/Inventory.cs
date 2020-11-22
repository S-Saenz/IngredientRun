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
        List<Ingredient> ings = new List<Ingredient>();
        public bool showInv = false;
        bool handsFull = false;
        KeyboardState oldKeyState;

        List<Vector2> boxes = new List<Vector2>();

        //for timing how fast items fall down inventory
        float timeSinceLastDrop = 0f;



        public Inventory()
        {
        }

        public void Load(ContentManager Content)
        {
            //manually add in box coordinates
            //row 1
            addBox(new Vector2(250, 162));
            addBox(new Vector2(350, 162));
            addBox(new Vector2(450, 162));
            addBox(new Vector2(550, 162));
            addBox(new Vector2(650, 162));
            addBox(new Vector2(750, 162));
            addBox(new Vector2(850, 162));
            addBox(new Vector2(950, 162));
            addBox(new Vector2(1050, 162));

            //row 2
            addBox(new Vector2(1050, 262));
            addBox(new Vector2(950, 262));
            addBox(new Vector2(850, 262));
            addBox(new Vector2(750, 262));
            addBox(new Vector2(650, 262));
            addBox(new Vector2(550, 262));
            addBox(new Vector2(450, 262));
            addBox(new Vector2(350, 262));
            addBox(new Vector2(250, 262));

            //row3
            addBox(new Vector2(1050, 362));
            addBox(new Vector2(950, 362));
            addBox(new Vector2(850, 362));
            addBox(new Vector2(750, 362));
            addBox(new Vector2(650, 362));
            addBox(new Vector2(550, 362));
            addBox(new Vector2(450, 362));
            addBox(new Vector2(350, 362));
            addBox(new Vector2(250, 362));

            inventorySq = Content.Load<Texture2D>("ui/Temp Inventory");
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

            ings.Add(acorn);
            ings.Add(apple);
            ings.Add(fish);
            ings.Add(meat);
            ings.Add(wood);

            foreach(Ingredient ing in ings)
            {
                //set origin at center
                ing.Origin = new Vector2(ing.img.Bounds.Center.X, ing.img.Bounds.Center.Y);
                Debug.WriteLine($"{ing.img} is highest = {Highest(ing)}");
            }




            

        }

        public void Update(MouseState mouseState, KeyboardState keyState)
        {

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                //Print mouse cursor position to debug console
                //Debug.WriteLine($"{mouseState.Position.X} {mouseState.Position.Y}");
            }
            foreach (Ingredient ing in ings) {


                    //use mouse to move objects
                    MoveIng(ing, mouseState);

                    //rotate objects when space bar pressed
                    if (ing.holding && oldKeyState.IsKeyUp(Keys.Space) && keyState.IsKeyDown(Keys.Space))
                    {
                        Debug.WriteLine("Rotate!");
                        ing.Rotation += Convert.ToSingle(Math.PI) / 2f; //rotate by 90 degrees                                                                      
                    }


                //inventory gravity - objects fall to bottom and stack on each other
                //if (canIngredientFall(ing) && (ing.timeSinceLastDrop > 1f)) //if 1 sec since last drop and box directly under is empty
                //Press F to let ingredients fall
                if( canIngredientFall(ing) && oldKeyState.IsKeyUp(Keys.F) && keyState.IsKeyDown(Keys.F))
                {
                    Debug.WriteLine($"dropping {ing.img}");
                    dropIngredient(ing);
                    ing.resetCounter();
                }

            }

            //Press G to shake bag and randomly redistribute items in inventory
            if (oldKeyState.IsKeyUp(Keys.G) && keyState.IsKeyDown(Keys.G))
            {
                Debug.WriteLine("Shaking bag");
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
            spriteBatch.Draw(inventorySq, new Vector2(200, 50), null, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.4f);

            //for (int i=0; i < ings.Count; i++)
            //{
            //    Vector2 ingredientPos = new Vector2(200 + inventorySq.Width / 2, 50 + (i * (50 / 3)) );
            //    spriteBatch.Draw( (ings[i].img), ingredientPos, null, Color.White, ings[i].rotation, Vector2.Zero, SpriteEffects.None, 0f); 
            //}

            for (int pos = (ings.Count() - 1); pos >= 0; --pos)
            {
                 ings[pos].Draw(spriteBatch, 1);
            }
        }

        bool IsPointOver(Point xy, Sprite sprite)
        {
            return (sprite.Bounds().Contains(xy.X, xy.Y));
        }

        void MoveIng(Ingredient ing, MouseState mouseState)
        {

            if (IsPointOver(mouseState.Position, ing) && !handsFull)
            {
                //Debug.WriteLine(Highest(ing));
                
                if ( mouseState.LeftButton == ButtonState.Pressed )
                {
                    Debug.WriteLine($"{ing.img} clicked!");//debugging
                    

                    //ing.holding = true;
                    //handsFull = true;

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

                    //if(Highest(ing)) {
                    //    Debug.WriteLine(true);
                    //    ing.holding = true;
                    //    handsFull = true;
                    //}
                    //else
                    //{
                    //    Debug.WriteLine(false);
                    //}
                }
            }
            //ingredient is let go after selection
            if (ing.holding && mouseState.LeftButton != ButtonState.Pressed)
            {
                handsFull = false;
                ing.holding = false;

                //snap ingredient to grid
                ing.pos = closestBox(ing);
                Debug.WriteLine(canIngredientFall(ing));

            }
            if (ing.holding)
            {
                ing.SetPosByMouse(mouseState.Position);
            }

        }

        public void addIng(Ingredient ing) {
            ings.Add(ing);
        }

        public void addBox(Vector2 coordinate)
        {
            boxes.Add(coordinate);
        }

        //determine whether ingredient is at top of inventory
        public bool Highest(Ingredient ingredient)
        {
            bool highest = false;
            foreach (Ingredient otherIng in ings)
            {
                if (ingredient.pos.Y >= otherIng.pos.Y)
                {
                    highest = false;
                }
                else
                {
                    highest = true;
                }
            }
            return highest;
        }

        //calculate closest box to ingredient 
        public Vector2 closestBox(Ingredient ingredient)
        {
            Vector2 closestBoxCoordinate;
            float shortestDistance;

            //arbitrary initialization
            closestBoxCoordinate = boxes[1];
            shortestDistance = Vector2.Distance(ingredient.pos, closestBoxCoordinate);

            foreach(Vector2 boxCoordinate in boxes)
            {
                //calc distance between ingredient and given box
                float distance = Vector2.Distance(boxCoordinate, ingredient.pos);
                
                //closer box is found!
                if (distance < shortestDistance )
                {
                    shortestDistance = distance;
                    closestBoxCoordinate = boxCoordinate;
                }
            }

            return closestBoxCoordinate;
        }

        //drop ingredient down grid
        public void dropIngredient(Ingredient ingredient)
        {
            float rowHeight = 100f;
            ingredient.pos = new Vector2(ingredient.pos.X, ingredient.pos.Y + rowHeight);
            //ingredient.pos.Y += 100f; // I don't understand why this doesn't work
        }


        //check if ingredient can fall down the inventory grid
        public bool canIngredientFall(Ingredient ingredient)
        {
            float rowHeight = 100f; //vertical distance between rows
            Vector2 underBox = new Vector2(ingredient.pos.X, ingredient.pos.Y + rowHeight); //box directly under ingredient
            bool underBoxEmpty = true;

            //check if item is in bottom row of inventory
            bool bottomRow = true;
            foreach (Vector2 box in boxes)
            {
                if( box.Equals(underBox) ) //there is a box below!
                {
                    bottomRow = false; // so ingredient is NOT in bottom row
                }
            }

            //if ingredient is in bottom row - return false (item cannot fall)
            if(bottomRow)
            {
                return false;
            }


            foreach (Ingredient other in ings) {
                if(other.pos.Equals(underBox)) //check if there is an item in the box below
                {
                    return false; //item cannot fall
                }
            }
          
            return underBoxEmpty; //item can fall
        }

        //checks if there is an ingredient in the box immediately above
        public bool isIngredientStackedOn(Ingredient ingredient)
        {
            float rowHeight = 100f; //vertical distance between rows
            Vector2 upperBox = new Vector2(ingredient.pos.X, ingredient.pos.Y - rowHeight); //box directly above ingredient
            bool upperBoxEmpty = false;

            foreach (Ingredient other in ings)
            {
                if (other.pos.Equals(upperBox)) //check if there is an item in the box above
                {
                    return true; //item in upper box
                }
            }

            return upperBoxEmpty; // no item in upper box
        }

        public Vector2 randomBox()
        {
            Random rnd = new Random();
            int randIndex = rnd.Next(boxes.Count);
            return boxes[randIndex];
        }

        //randomize ingredient placement
        public void shakeBag()
        {
            foreach(Ingredient ingredient in ings)
            {
                ingredient.pos = randomBox();
            }
        }

    }
}
