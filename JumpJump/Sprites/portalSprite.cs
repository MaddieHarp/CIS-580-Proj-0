using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using JumpJump.Collisions;

namespace JumpJump
{
    public class portalSprite
    {
        public Vector2 Position { get; set; }
        public BoundingRectangle Bounds { get; private set; }
        public bool IsActive { get; set; } = false;

        private Texture2D portalTexture;
        private string texture;
        private float scale = 3f; 

        private const float BOUNDS_OFFSET_X = 70f;   // pixels to shift right

        public portalSprite(Vector2 position, string texture)
        {
            Position = position;
            this.texture = texture;

            Bounds = new BoundingRectangle(
                new Vector2(position.X + BOUNDS_OFFSET_X, position.Y),
                28f * scale, 65f * scale);
        }

        public void LoadContent(ContentManager content)
        {
            portalTexture = content.Load<Texture2D>(texture); 
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