using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace IngredientRun.Game
{
    class InputManager
    {
        private Dictionary<string, Button> _buttons;

        public void Initialize()//initializes default button mapping
        {
            List<Keys> leftKeys = new List<Keys>()
            {
                Keys.A,
                Keys.Left
            };
            List<Keys> rightKeys = new List<Keys>()
            {
                Keys.D,
                Keys.Right
            };
            List<Keys> jumpKeys = new List<Keys>()
            {
                Keys.W,
                Keys.Up
            };
            _buttons.Add("left", new Button(leftKeys));
            _buttons.Add("right", new Button(rightKeys));
            _buttons.Add("jump", new Button(jumpKeys));
        }

        public void Update(GameTime time)
        {
            //for each button
                //for each key in button
                    //check state and update
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
