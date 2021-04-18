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
    public class Cook
    {
        KeyboardState oldKeyState;

        Texture2D meter, needle, startButton, burntText, niceText, perfectText, rawText, square;
        
        public string foodName;
        public bool _cookingVisible = true;
        public bool _finished = false;

        static float _screenWidth = 1728;
        float _screenHeight = 972;
        float _scale = 2.7f;

        float _needleX;
        float _needleSpeed = 20.0f;
        float _needleStart = _screenWidth * 0.12f;
        bool _attemptRemaining = true; //you can't spam the space button to move the needle

        float _meterEnd = 1517;

        //left-side boundaries for the cooking zones
        float _mediumZone = 902;
        float _wellDoneZone = 1284;
        float _burntZone = 1457;

        //for grading text
        Texture2D _grade;
        float _gradeOpacity;
        float _gradeX;

        //for the timer to turn the UI off
        int counter = 1;
        int limit = 3;
        float countDuration = 1f; //every  1s.
        float currentTime = 0f;

        bool _debugMode = false;

        public bool loaded = false;

        public Cook(string selectedFood)
        {
            foodName = selectedFood;
            _cookingVisible = true;
        }

        public Cook()
        {

        }

        public void Load(ContentManager Content)
        {
            //WHY IS IT SO HARD TO GET A DAMN SQUARE IN THIS GAME
            //Texture2d rect = new Texture2d(_graphics.graphicsdevice, 80, 30);

            //color[] data = new color[80 * 30];
            //for (int i = 0; i < data.length; i++) data[i] = color.chocolate;
            //rect.setdata(data);

            //load in images
            meter = Content.Load<Texture2D>("ui/cooking/Cooking-UI-2");
            needle = Content.Load<Texture2D>("ui/cooking/needle");
            startButton = Content.Load<Texture2D>("ui/cooking/Green-Button-2");

            burntText = Content.Load<Texture2D>("ui/cooking/text/Burnt");
            niceText = Content.Load<Texture2D>("ui/cooking/text/Nice");
            perfectText = Content.Load<Texture2D>("ui/cooking/text/Perfect");
            rawText = Content.Load<Texture2D>("ui/cooking/text/Raw");

            //foodImage = Content.Load<Texture2D>("ingredient/acornScaled");
            //square = Content.Load<Texture2D>("ui/1pxSquare");
            square = Content.Load<Texture2D>("ui/Recipe/Food Frame");

            _needleX = _needleStart; //set needle to starting position
    }

        public void Update(MouseState mouseState, KeyboardState keyState, GameTime gameTime)
        {
            //feedback text for how well you scored!
            _grade = rawText; //temporarily assigned before it gets changed by AssignGrade()
            AssignGrade(ref _grade);
            _gradeOpacity = _attemptRemaining ? 0f : 1f; //disable/enable visibility for the grade text

            ///rare text apperas on left, the rest appears on right
            if (GradeCooking() == "rare")
                _gradeX = _screenWidth * 1 / 4 - _grade.Width / 2 * _scale / 2;

            else
                _gradeX = _screenWidth * .7f - _grade.Width / 2 * _scale / 2;


            //space bar held down - move needle
            if (Game1.instance.input.IsDown("cook") && _needleX <= _meterEnd && _attemptRemaining)
                _needleX += _needleSpeed;
            

            //space bar is released
            if(_needleX > _needleStart && !Game1.instance.input.IsDown("cook"))
            {
                _attemptRemaining = false;
                //debug(GradeCooking()); 
            }

            //don't let needle move past the end of the meter
            if (_needleX > _meterEnd)
                _needleX = _meterEnd;

            if(!_attemptRemaining)
                CookingFinished(gameTime);

            ////////////////////////////// debugging tools \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            //press 1 to check needleX
            if ( oldKeyState.IsKeyUp(Keys.D1) && keyState.IsKeyDown(Keys.D1) && keyState.IsKeyDown(Keys.LeftAlt) )
                debug($"{_needleX}");

            //press 3 to check mouse position 
            if ( oldKeyState.IsKeyUp(Keys.D3) && keyState.IsKeyDown(Keys.D3) && keyState.IsKeyDown(Keys.LeftAlt) )
                debug($"{mouseState.Position.X} {mouseState.Position.Y}");

            //press 2 to restart needle
            if ( oldKeyState.IsKeyUp(Keys.D2) && keyState.IsKeyDown(Keys.D2) && keyState.IsKeyDown(Keys.LeftAlt) )
                ResetNeedle();

            //press 4 to en/disable visiblity 
            if( oldKeyState.IsKeyUp(Keys.D4) && keyState.IsKeyDown(Keys.D4) && keyState.IsKeyDown(Keys.LeftAlt) )
            {
                debug($"before: {_cookingVisible}");
                _cookingVisible = _cookingVisible ? false : true;
                debug($"after: {_cookingVisible}");
            }

            //press HOME to toggle debug mode
            if ( oldKeyState.IsKeyUp(Keys.Home) && keyState.IsKeyDown(Keys.Home) )
                _debugMode = !_debugMode;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float cookingOpacity = _cookingVisible ? 1f : 0f; //toggle visibility for th cooking UI

            //meter 
            //Vector2 origin = new Vector2(meter.Width / 2 * (1 /scale), meter.Height / 2 * (1 / scale));
            spriteBatch.Draw(meter, new Vector2(0, 0), null, Color.White * cookingOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);

            //needle
            spriteBatch.Draw(needle, new Vector2(_needleX, _screenHeight * 0.317f), null, Color.White * cookingOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);


            //GOD HELP ME UNDERSTAND WHAT XNA IS DOING WITH ORIGINS AND SCALES

            //startButton
            //origin = new Vector2(startButton.Width / 2 / (scale), startButton.Height / 2 / (scale));
            //float x = _screenWidth/2 - startButton.Width/2 * _scale; //x pos for the startButton
            //spriteBatch.Draw(startButton, new Vector2(x, _screenHeight / 7), null, Color.White * cookingOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);

            //background square for the food being cooked 
            float squareSize = 15; //px
            //float squareX = _screenWidth / 2 - (squareSize * _scale /2); // this si my failed attempt :(
            float squareX = _screenWidth / 2 - 60; //this is the hardcoded version because I can't figure out scaling
            Vector2 squarePos = new Vector2(squareX, _screenHeight / 7f);
            //spriteBatch.Draw(square, squarePos, null, Color.White * cookingOpacity, 0f, Vector2.Zero, squareSize, SpriteEffects.None, 1f);

            spriteBatch.Draw(square, squarePos, null, Color.White * cookingOpacity, 0f, Vector2.Zero, _scale, SpriteEffects.None, 1f);


            //food being cooked
            //float foodScale = foodImage.ToString() == "Ingredient/acornScaled" ? 0.5f : .15f; //scale for an acorn or the grilled fish
            Size2 foodSize = TextureAtlasManager.GetSize("Item", foodName);
            float foodScale = Game1.instance.recipeMenu._recipeScale; //reuse the scale value from the recipe menu
            float foodX = _screenWidth / 2 - foodSize.Width / 2 * foodScale;
            // spriteBatch.Draw(foodImage, new Vector2(foodX, _screenHeight / 7), null, Color.White * cookingOpacity, 0f, Vector2.Zero, foodScale, SpriteEffects.None, 1f);
            TextureAtlasManager.DrawTexture(spriteBatch, "Item", foodName, new Vector2(foodX, _screenHeight / 7), Color.White * cookingOpacity, foodScale);


            //text for cooking feedback
            _gradeOpacity *= cookingOpacity;
            if(_grade != null)
                spriteBatch.Draw(_grade, new Vector2(_gradeX, _screenHeight / 5f), null, Color.White * _gradeOpacity, 0f, Vector2.Zero, _scale/2, SpriteEffects.None, 1f);


            //squares don't work :/
            //FillRectangle(spriteBatch, new Vector2(1728/2, 972/2), new Size2(10, 10), Color.White);

            // temp tutorial text
            spriteBatch.DrawString(FontManager._bigdialogueFont, "Hold 'Space' and release when food is cooked but not burnt", 
                                   new Vector2(0, 0), Color.White);
        }

        void debug(string message)
        {
            Debug.WriteLineIf(_debugMode, message);
        }

        void ResetNeedle()
        {
            _needleX = _needleStart;
            _attemptRemaining = true;
        }

        string GradeCooking()
        {
            if (_needleX < _mediumZone) return "rare";
            else if (_needleX < _wellDoneZone) return "medium";
            else if (_needleX < _burntZone) return "well done";
            else return "burnt";
        }

        void AssignGrade(ref Texture2D display)
        {
            string grade = GradeCooking();

            switch(grade)
            {
                case "rare":
                    display = rawText;
                    break;
                case "medium":
                    display = niceText;
                    break;
                case "well done":
                    display = perfectText;
                    break;
                case "burnt":
                    display = burntText;
                    break;
            }
        }


        //inspired from - https://stackoverflow.com/questions/13394892/how-to-create-a-timer-counter-in-c-sharp-xna
        void CookingFinished(GameTime gameTime)
        {
            debug($"before: {currentTime}");
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 
            debug($"after: {currentTime}");
            //debug($"{gameTime.ElapsedGameTime.TotalSeconds}");
            
            if (currentTime >= countDuration) //this is true at least once every sec
            {
                //update how much time has passed
                counter++;
                currentTime -= countDuration; // "use up" the time & recalibrate the currentTime                  
            }

            debug($"counter: {counter}\ncurrentTime: {currentTime}");
            //timer finished
            if (counter >= limit)
            {
                counter = 0; //reset counter
                debug("counter hit limit!");

                UpdateInventoryAfterCooking();

                //turn off the cooking UI
                //_cookingVisible = false; 
                ResetNeedle();
                _finished = true;
                Game1.instance.UI.SwitchState(UIState.RecipeMenu); //have UI manager switch back to HUD
            }
        }

        public void UpdateInventoryAfterCooking()
        {
            //add cooked food to inventory
            Game1.instance.inventory.addIngredient(foodName);

            //remove used ingredients from inventory
            List<string> ingredients = Game1.instance.recipeMenu._recipes[foodName];
            foreach (string ingredient in ingredients)
                Game1.instance.inventory.removeIngredient(ingredient);
        }

        //Texture names are formatted as "ingredient/[actualIngredientName]"
        string formatFoodName(string foodName)
        {
            string realName = foodName.Split("/")[1];
            Debug.WriteLine($"{foodName} -> {realName}");
            return realName;
        }
    }


}