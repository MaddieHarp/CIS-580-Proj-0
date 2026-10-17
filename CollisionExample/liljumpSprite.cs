using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;

namespace CollisionExample
{
    public class liljumpSprite
    {
        public Vector2 Position { get; private set; }
        public BoundingRectangle Bounds { get; private set; }
        private Texture2D texture;

        // Add a scale multiplier. 3f makes him 3 times bigger (108x99 pixels)
        private float scale = 3f; 

        public liljumpSprite(Vector2 position)
        {
            Position = position;
            
            // Multiply his exact dimensions (36 width, 33 height) by the scale
            // so the invisible collision box grows perfectly with the drawing!
            Bounds = new BoundingRectangle(position, 36f * scale, 33f * scale);
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("liljump"); 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Use the extended Draw method to apply the scale visually
            spriteBatch.Draw(
                texture, 
                Position, 
                null, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                scale, // This stretches the image by our multiplier
                SpriteEffects.None, 
                0f
            );
        }
    }
}