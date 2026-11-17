using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace JumpJump
{
    public class flyingKangarooSprite
    {
        private Texture2D texture;
        public Vector2 position;
        private float baseY;
        
        private float speed = 200f;

        private float bounceAmplitude = 50f;
        private float bounceFrequency = 4f;

        public float X => position.X;
        public float Y => position.Y;

        public flyingKangarooSprite(Vector2 startPosition)
        {
            position = startPosition;
            baseY = startPosition.Y;
        }

        public void SetX(float x) => position.X = x;

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("kangrarooflying");
        }

        public void Update(GameTime gameTime)
        {

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float totalTime = (float)gameTime.TotalGameTime.TotalSeconds;
            position.X -= speed * dt;
            position.Y = baseY + (float)Math.Sin(totalTime * bounceFrequency) * bounceAmplitude;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture == null) return;
            Vector2 origin = new Vector2(15f, 14.5f);
            spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, 3f, SpriteEffects.FlipHorizontally, 0f);
        }
    }
}