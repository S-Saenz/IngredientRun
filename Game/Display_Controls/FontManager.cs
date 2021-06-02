using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace WillowWoodRefuge
{
    public enum Alignment { Left, Right, Centered }

    static class FontManager
    {
        static public SpriteFont _dialogueFont { private set; get; }
        static public SpriteFont _bigdialogueFont { private set; get; }
        static public SpriteFont _descriptionFont { private set; get; }
        static public SpriteFont _giantdialogueFont { private set; get; }

        static public void Initialize(ContentManager content)
        {
            _dialogueFont = content.Load<SpriteFont>("fonts/NPCDialogue");
            _bigdialogueFont = content.Load<SpriteFont>("fonts/NPCDialogueBigBoi");
            _descriptionFont = content.Load<SpriteFont>("fonts/NPCDialogueBiggerBoi");
            _giantdialogueFont = content.Load<SpriteFont>("fonts/NPCDialogueGiant");
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
        
        // Prints text in given alignment (isCentered indicates if position is 0,0 or center of text box)
        static public void PrintText(SpriteFont font, SpriteBatch spriteBatch, string text, Vector2 position, 
                                     Alignment alignment, Color color, bool isCentered = false)
        {
            Vector2 boxSize = font.MeasureString(text); // size of text box to be drawn
            Vector2 origin = isCentered ? position - boxSize / 2 : position; // 0,0 top left corner of text box

            // Left aligned
            if (alignment == Alignment.Left)
            {
                // round to nearest int so letters don't get cut
                origin.X = (int)origin.X;
                origin.Y = (int)origin.Y;
                spriteBatch.DrawString(font, text, origin, color);
                return;
            }

            string[] values = text.Split("\n"); // lines separated
            float yOffset = 0; // y offset of current line from origin

            // Right aligned
            if (alignment == Alignment.Right)
            {
                foreach (string line in values)
                {
                    Vector2 lineSize = font.MeasureString(line);
                    if (lineSize.Y == 0)
                        lineSize.Y = font.MeasureString("|").Y;
                    Vector2 pos = origin + new Vector2(boxSize.X - lineSize.X, yOffset);
                    // round to nearest int so letters don't get cut
                    pos.X = (int)pos.X;
                    pos.Y = (int)pos.Y;
                    spriteBatch.DrawString(font, line, pos, color);
                    yOffset += lineSize.Y;
                }
                return;
            }

            origin = isCentered ? position - new Vector2(0, boxSize.Y / 2) : position + new Vector2(boxSize.X / 2, 0); // center of top of text box
            
            // Center aligned
            if(alignment == Alignment.Centered)
            {
                foreach (string line in values)
                {
                    Vector2 lineSize = font.MeasureString(line);
                    if(lineSize.Y == 0)
                        lineSize.Y = font.MeasureString("|").Y;
                    Vector2 pos = origin + new Vector2(-lineSize.X / 2, yOffset);
                    // round to nearest int so letters don't get cut
                    pos.X = (int)pos.X;
                    pos.Y = (int)pos.Y;
                    spriteBatch.DrawString(font, line, pos, color);
                    yOffset += lineSize.Y;
                }
                return;
            }
        }
    }
}
