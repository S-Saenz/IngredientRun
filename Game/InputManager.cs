using IngredientRun.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace IngredientRun
{
    public class InputManager
    {
        private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();
        private KeyboardState oldstate = Keyboard.GetState();

        public void Initialize()//initializes default button mapping
        {
            List<Keys> leftKeys = new List<Keys>()
            {
                Keys.Left,
                Keys.A
            };
            List<Keys> rightKeys = new List<Keys>()
            {
                Keys.Right,
                Keys.D
            };
            List<Keys> jumpKeys = new List<Keys>()
            {
                Keys.Up,
                Keys.W
            };
            List<Keys> interactKeys = new List<Keys>()
            {
                Keys.F
            };
            List<Keys> runKeys = new List<Keys>()
            {
                Keys.LeftShift
            };
            List<Keys> selectKeys = new List<Keys>()
            {
                Keys.Enter
            };
            List<Keys> returnKeys = new List<Keys>()
            {
                Keys.Back
            };
            _buttons.Add("left", new Button(leftKeys));
            _buttons.Add("right", new Button(rightKeys));
            _buttons.Add("jump", new Button(jumpKeys));
            _buttons.Add("interact", new Button(interactKeys));
            _buttons.Add("select", new Button(selectKeys));
            _buttons.Add("return", new Button(returnKeys));
            _buttons.Add("run", new Button(runKeys));
        }

        public void Update(GameTime time)
        {
            KeyboardState newstate = Keyboard.GetState();
            //for each button
            foreach (KeyValuePair<string, Button> entry in _buttons)
            {
                Button button = entry.Value;
                button._isDown = false;
                button._justPressed = false;
                button._justReleased = false;
                //for each key in button
                foreach (Keys key in button._keys)
                {
                    //check state and update
                    
                    if (newstate.IsKeyDown(key) && oldstate.IsKeyUp(key))
                    {
                        button._isDown = true;
                        button._justPressed = true;
                        break;
                        //Debug.WriteLine("is down"); this works
                    }
                    else if (newstate.IsKeyDown(key) && oldstate.IsKeyDown(key))
                    {
                        button._isDown = true;
                        button._justPressed = false;
                        break;
                    }
                    else if(newstate.IsKeyUp(key) && oldstate.IsKeyDown(key))
                    {
                        button._isDown = false;
                        button._justReleased = true;
                        break;
                    }
                    /*else if (newstate.IsKeyUp(key) && oldstate.IsKeyUp(key))
                    {
                        button._isDown = false;
                        button._justReleased = false;
                    }*/
                    
                }
            }
            oldstate = newstate;
        }

        public bool IsDown(string buttonName)
        {
            return _buttons[buttonName]._isDown;
        }
        public bool JustPressed(string buttonName)
        {
            return _buttons[buttonName]._justPressed;
        }
        public bool JustReleased(string buttonName)
        {
            return _buttons[buttonName]._justReleased;
        }
        public void newmap(string buttonName, Keys newKey)
        {
            _buttons[buttonName]._keys.Add(newKey);
        }
        public bool unmap(string buttonName, Keys oldKey)
        {
            return _buttons[buttonName]._keys.Remove(oldKey);
        }
        public bool swapmap(string buttonName, Keys oldKey, Keys newKey)
        {
            if(unmap(buttonName, oldKey))
            {
                newmap(buttonName, newKey);
                return true;
            }
            return false;           
        }
    }

    class Button
    {
        public bool _isDown, _justPressed, _justReleased = false;
        public List<Keys> _keys;

        public Button(List<Keys> keys)
        {
            _keys = keys;
        }
    }
}
