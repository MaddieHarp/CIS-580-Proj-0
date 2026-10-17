using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;

namespace CollisionExample
{
    public class GroundSprite
    {
        private Vector2 position;
        private Texture2D texture;
        private BoundingRectangle bounds;

        public BoundingRectangle Bounds => bounds;
        public bool Collected { get; set; } = false;

        public GroundSprite(Vector2 position)
        {
            this.position = position;
            float width = 3840f; 
            float height = 50f;

            bounds = new BoundingRectangle(position, width, height);
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("ground");
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(Collected) return;
            float scaleX = 3840f / texture.Width;
            float scaleY = 50f / texture.Height;

            spriteBatch.Draw(
                texture, 
                position, 
                null, 
                Color.White, 
                0, 
                Vector2.Zero, 
                new Vector2(scaleX, scaleY),
                SpriteEffects.None, 
                0
            );
        }
    }
}