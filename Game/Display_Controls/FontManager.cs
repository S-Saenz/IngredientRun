using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WillowWoodRefuge
{
    static class FontManager
    {
        static public SpriteFont _dialogueFont { private set; get; }

        static public void Initialize(ContentManager content)
        {
            _dialogueFont = content.Load<SpriteFont>("fonts/NPCDialogue");
        }
    }
}
