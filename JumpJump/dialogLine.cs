using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpJump
{
    public class DialogueLine
    {
        public string Text;
        public Texture2D Portrait;
        public Rectangle? PortraitSource;
        public float PortraitScale = 1f;
        public Point PortraitOffset = Point.Zero; 
        public bool FlipPortrait;

        public DialogueLine(string text, Texture2D portrait,
                            Rectangle? portraitSource = null, bool flipPortrait = false)
        {
            Text = text;
            Portrait = portrait;
            PortraitSource = portraitSource;
            FlipPortrait = flipPortrait;
        }
    }
}