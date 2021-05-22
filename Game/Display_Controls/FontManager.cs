using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WillowWoodRefuge
{
    static class FontManager
    {
        static public SpriteFont _dialogueFont { private set; get; }
        static public SpriteFont _bigdialogueFont { private set; get; }

        static public void Initialize(ContentManager content)
        {
            _dialogueFont = content.Load<SpriteFont>("fonts/NPCDialogue");
            _bigdialogueFont = content.Load<SpriteFont>("fonts/NPCDialogueBigBoi");
        }

        // Parses a given single line of text into multiple lines within the given max line length in pixels using the given font
        // Warning: will remove duplicate spaces
        static public string AddLineBreaks(string unparsed, SpriteFont font, float maxLineLength)
        {
            string[] values = unparsed.Split(" "); // each word separated in order
            string parsed = ""; // final parsed string with line breaks
            float currWidth = 0; // tally of running current line width
            foreach (string word in values)
            {
                if (word.Length > 0) // if not empty word (parsing bug)
                {
                    float wordWidth = font.MeasureString(" " + word).X;
                    if (currWidth + wordWidth > maxLineLength) // start new line
                    {
                        currWidth = -1;
                        parsed += "\n";
                    }
                    else // continue current line
                    {
                        parsed += " ";
                    }
                    currWidth += wordWidth;
                    parsed += word;
                }
            }

            return parsed;
        }
    }
}
