using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;
using System.Runtime.CompilerServices;
using System.Net;
using Microsoft.Xna.Framework.Audio;

namespace CollisionExample
{
    /// <summary>
    /// A class representing a slime ghost
    /// </summary>
    public class SlimeGhostSprite
    {
        private const float GRAVITY = 5000f;
        private const float SPEED = 750f;
        private const float JUMP = 1500f;

        private SoundEffect jumpSound;

        private bool wasJumpKeyDown = false;
        private bool _OnGround = true;
        private Vector2 _velocity;
        private KeyboardState keyboardState;

        private Texture2D texture;

        private Vector2 position = new Vector2(200, 960);

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        

        private bool flipped;

        private bool jump;

        private int currentFrame = 0;
        private double animationTimer = 0;
        private const double ANIMATION_SPEED = .2;

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
            _velocity = Vector2.Zero;
            bounds.X = position.X - 50;
            bounds.Y = position.Y - 40;
        }

        public BoundingRectangle bounds = new BoundingRectangle(new Vector2(32,29), 124, 124);
        /// <summary>
        /// color overlay of the ghost
        /// </summary>
        public Color Color{get; set;} = Color.White;

        /// <summary>
        /// bounding volume of sprite
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            jumpSound = content.Load<SoundEffect>("jumpSound");
            texture = content.Load<Texture2D>("kangroo");
        }
        /// <summary>
        /// followed tutorial: https://www.youtube.com/watch?v=HujUFaqh-D8
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateVelocity(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            { 
                flipped = true;
                _velocity.X = -SPEED;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                flipped = false;
                _velocity.X = SPEED;
            }
            else
            {
                _velocity.X = 0;
            }

            if (!_OnGround)
            {
                _velocity.Y += GRAVITY * (float)gameTime.ElapsedGameTime.TotalSeconds;
                jump = true;
            } 

            bool jumpKeyDown = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Space);

            if (jumpKeyDown && !wasJumpKeyDown)
            {
                _velocity.Y = -JUMP;
                _OnGround = false;
                jump = true;
                currentFrame = 1;
                animationTimer = 0;
                jumpSound.Play();
            }

            wasJumpKeyDown = jumpKeyDown; 

        }

        public void Update(GameTime gameTime, List<PlatformSprite> platforms, GroundSprite ground)
        {
            UpdateVelocity(gameTime);
            position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            bounds.X = position.X - 50;
            bounds.Y = position.Y - 40;

            _OnGround = false;

            foreach (var platform in platforms)
            {
                if (bounds.CollidesWith(platform.Bounds))
                {
                    ResolveCollision(platform.Bounds);
                }
            }
            if (bounds.CollidesWith(ground.Bounds))
            {
                ResolveCollision(ground.Bounds);
            }

            if (jump)
            {
                if (currentFrame == 0) 
                {
                    currentFrame = 1; 
                    animationTimer = 0;
                }

                if (currentFrame < 2)
                {
                    animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    
                    if (animationTimer >= ANIMATION_SPEED)
                    {
                        currentFrame++; 
                        animationTimer = 0; 
                    }
                }
            }
            else
            {
                currentFrame = 0;
                animationTimer = 0;
            }
        }

        private void ResolveCollision(BoundingRectangle other)
        {
            float overlapX = MathF.Min(bounds.Right, other.Right) - MathF.Max(bounds.Left, other.Left);
            float overlapY = MathF.Min(bounds.Bottom, other.Bottom) - MathF.Max(bounds.Top, other.Top);

            if (overlapX < overlapY)
            {
                if (bounds.X < other.X)
                    position.X -= overlapX;
                else
                    position.X += overlapX;

                _velocity.X = 0;
            }
            else
            {
                if (bounds.Y < other.Y)
                {
                    position.Y -= overlapY;
                    _OnGround = true;

                    jump = false;
                }
                else
                {
                    position.Y += overlapY;
                }

                _velocity.Y = 0;
            }

            bounds.X = position.X - 50;
            bounds.Y = position.Y - 40;
        }

        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            // Multiply currentFrame by 64 to shift the X position of the rectangle to the correct image
            var source = new Rectangle(currentFrame * 60 + 6, 0, 57, 64);
            
            spriteBatch.Draw(texture, position, source, Color, 0, new Vector2(32, 32), 3f, spriteEffects, 0);
        }   
    }
}
