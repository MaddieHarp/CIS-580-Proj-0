using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;

namespace CollisionExample
{
    public class portalSprite
    {
        public Vector2 Position { get; set; }
        public BoundingRectangle Bounds { get; private set; }
        public bool IsActive { get; set; } = false;

        private Texture2D portalTexture;
        
        // Add a scale multiplier. 2f makes it twice as big!
        private float scale = 3f; 

        public portalSprite(Vector2 position)
        {
            Position = position;
            
            // Multiply the original 28x65 dimensions by the scale 
            // so the physics box matches the new visual size
            Bounds = new BoundingRectangle(position, 28f * scale, 65f * scale);
        }

        public void LoadContent(ContentManager content)
        {
            portalTexture = content.Load<Texture2D>("portal"); 
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            // Use the extended Draw method to apply the scale
            spriteBatch.Draw(
                portalTexture, 
                Position, 
                null, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                scale, // Stretches the image
                SpriteEffects.None, 
                0f
            );
        }
    }
}