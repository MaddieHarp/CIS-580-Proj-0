using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using JumpJump.Collisions;

namespace JumpJump
{
    public class firewheelSprite
    {
        private const float GRAVITY = 3000f;
        private Texture2D texture;
        private const float SCALE = 1f;
        public Vector2 position;
        private float rotationAngle = 0f;

        private Vector2 _velocity;

        public bool dropped;
        public bool alive;

        public float X => position.X;

        public firewheelSprite(Vector2 startPosition)
        {
            position = startPosition;
            dropped = false;
            alive = false;
        }

        public void SetX(float x) => position.X = x;

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("firewheel");
        }

        public void Update(GameTime gameTime)
        {
            if (!alive || !dropped) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _velocity.Y += GRAVITY * dt;
            position += _velocity * dt;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture == null) return;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, position, null, Color.White, rotationAngle, origin, 1f, SpriteEffects.None, 0f);
        }

        public void Drop(Vector2 pos)
        {
            position = pos;
            _velocity = Vector2.Zero;
            dropped = true;
            alive = true;
        }

        public bool Intersects(BoundingRectangle rect)
        {
            if (!alive || !dropped || texture == null) return false;

            // circle (wheel) vs rectangle (player) test
            float radius = Math.Min(texture.Width, texture.Height) * SCALE * 0.5f * 0.8f; // 0.8 = slightly forgiving
            float nearestX = MathHelper.Clamp(position.X, rect.Left, rect.Right);
            float nearestY = MathHelper.Clamp(position.Y, rect.Top, rect.Bottom);
            float dx = position.X - nearestX;
            float dy = position.Y - nearestY;
            return dx * dx + dy * dy <= radius * radius;
        }


    }
}