using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using JumpJump.Collisions;
using System;
using System.Collections.Generic;

namespace JumpJump
{
    public class hellKanagrooSprite
    {
        private const int MAX_HEALTH = 3;

        public int Health { get; private set; } = MAX_HEALTH;
        public bool IsDead => Health <= 0;
        public bool TouchingPlayer { get; private set; }

        public void TakeDamage(int amount) => Health -= amount;

        public void Respawn(float x)
        {
            Health = MAX_HEALTH;
            SetX(x);
        }
        private Texture2D texture;
        private Vector2 position;
        private float speed = 100f;
        private bool flipped;

        public float X => position.X;

        public BoundingRectangle bounds = new BoundingRectangle(new Vector2(32,29), 124, 124);
        /// <summary>
        /// color overlay of the ghost
        /// </summary>
        public Color Color{get; set;} = Color.White;

        /// <summary>
        /// bounding volume of sprite
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        public hellKanagrooSprite(Vector2 startPosition)
        {
            position = startPosition;
            UpdateBounds();
        }

        public void SetX(float x)
        {
            position.X = x;
            UpdateBounds();
        }
        

        private void UpdateBounds()
        {
            bounds.X = position.X - 50;
            bounds.Y = position.Y - 40;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("kangaroo");
        }

        public void Update(GameTime gameTime, float targetX, BoundingRectangle playerBounds, IEnumerable<hellKanagrooSprite> others)
        {

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float step = speed * dt;
            float dx = targetX - position.X;
            float oldX = position.X;

            position.X = MathF.Abs(dx) <= step ? targetX : position.X + MathF.Sign(dx) * step;
            if (MathF.Abs(dx) > 1f)
                flipped = dx < 0;

            position.X = MathF.Abs(dx) <= step ? targetX : position.X + MathF.Sign(dx) * step;
            UpdateBounds();

            TouchingPlayer = bounds.CollidesWith(playerBounds);
            bool blocked = TouchingPlayer;
            foreach (var o in others)
                if (o != this && bounds.CollidesWith(o.Bounds))
                    blocked = true;

            if (blocked)
            {
                position.X = oldX;
                UpdateBounds();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture == null) return;
            var source = new Rectangle(66, 0, 57, 64);
            SpriteEffects flip;
            if (flipped)
            {
                flip = SpriteEffects.FlipHorizontally;
            }
            else{
                flip = SpriteEffects.None;
            }
            
            spriteBatch.Draw(texture, position, source, Color.Red, 0, new Vector2(32, 32), 3f, flip, 0);

        }
    }
}