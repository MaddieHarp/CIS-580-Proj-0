using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using JumpJump.Collisions;

namespace JumpJump
{
    public class SpeechBubble
    {
        public Vector2 Position { get; private set; }
        public BoundingRectangle Bounds { get; private set; }
        private Texture2D texture;

        private string text;
        private bool flipped;

        private float scale = .33f; 

        public SpeechBubble(Vector2 position, string _text, bool flip)
        {
            Position = position;
            text = _text;
            flipped = flip;

        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("speechbubble"); 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // Use the extended Draw method to apply the scale visually
            spriteBatch.Draw(
                texture, 
                Position, 
                null, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                scale, // This stretches the image by our multiplier
                SpriteEffects.FlipHorizontally, 
                0f
            );
        }
    }
}