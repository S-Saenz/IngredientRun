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
    public class Ingredient : Sprite
    {
        public bool holding = false;
        //public float Rotation;
        public Vector2 index = new Vector2(1, 0);
        public bool falling = false;
        //public Vector2 pos;
        //public bool highest = false;
        //public float Scale = 1f;

        //assigned in the constructor
        //public Texture2D img;
        Vector2 pos;
        // float scale;
        //public Vector2 Origin;

        //for timing how fast items fall down inventory
        //public float timeSinceLastDrop = 0f;

        public String orientation = "up";           //which way is our ingredient rotated - up, down, left, right
        public bool doubleSquare = false;           //does this ingredient occupy more than one square? 
        public Vector2 index2 = new Vector2(1, 0);  //index of the other square your ingredient occupies 


        public Ingredient(Texture2D image, Vector2 position)
        {
            img = image;
            //this.img = image;
            pos = position;
            scale = .25f;
            Origin = new Vector2(img.Bounds.Center.X, img.Bounds.Center.Y);
        }

        public Ingredient(Texture2D image)
        {
            img = image;
            scale = .25f;
            Origin = new Vector2(img.Bounds.Center.X, img.Bounds.Center.Y);
        }

        public void Update(GameTime gameTime)
        {
            //timeSinceLastDrop += (float)gameTime.ElapsedGameTime.TotalSeconds; //add elapsed time to counter


        }
        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(img, pos, null, Color.White, Rotation, Origin, scale, SpriteEffects.None, 1f);

            //spriteBatch.Draw(myTexture, position, null, Color.White, rotation, origin, scale, SpriteEffects.FlipHorizontally, layer);


        }

        //public void resetCounter()
        //{
        //    this.timeSinceLastDrop = 0f;
        //}

        //change the orientation variable by one rotation (pi/2) 
        public void updateOrientation()
        {
            switch (this.orientation) {
                case "up":
                    this.orientation = "right";
                    break;
                case "right":
                    this.orientation = "down";
                    break;
                case "down":
                    this.orientation = "left";
                    break;
                case "left":
                    this.orientation = "up";
                    break;
            }
            //Debug.WriteLine(this.orientation);
        }

        //call this function to update index2 in relation to the ingredient rotation/orientation
        public void updateIndex2()
        {
            switch (this.orientation) {
                case "right":
                    //index2 is right of index
                    this.index2 = new Vector2(this.index.X + 1, this.index.Y);
                    break;
                case "down":
                    //index2 is below index
                    this.index2 = new Vector2(this.index.X, this.index.Y - 1);
                    break;
                case "left":
                    //index 2 is left of index
                    this.index2 = new Vector2(this.index.X - 1, this.index.Y);
                    break;
                case "up":
                    //index2 is above index
                    this.index2 = new Vector2(this.index.X, this.index.Y + 1);
                    break;
            }

        }

        //the following overrides are for comparation
        //I was motivated to do this from needing to remove from the inventory's ingredient list
        public override string ToString()
        {
            return img.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Ingredient objAsIngredient = obj as Ingredient;
            if (objAsIngredient == null) return false;
            else return Equals(objAsIngredient);
        }
        public bool Equals(Ingredient other)
        {
            if (other == null) return false;
            return (this.img.Equals(other.img));
        }
    }
}
   
