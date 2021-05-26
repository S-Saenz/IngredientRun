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

        static public void Initialize(ContentManager content)
        {
            _dialogueFont = content.Load<SpriteFont>("fonts/NPCDialogue");
            _bigdialogueFont = content.Load<SpriteFont>("fonts/NPCDialogueBigBoi");
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
                    spriteBatch.DrawString(font, line, origin + new Vector2(boxSize.X - lineSize.X, yOffset), color);
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
                    spriteBatch.DrawString(font, line, origin + new Vector2(-lineSize.X / 2, yOffset), color);
                    yOffset += lineSize.Y;
                }
                return;
            }
        }
    }
}
