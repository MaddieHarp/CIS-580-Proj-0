using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using JumpJump.Collisions;
using System.Runtime.CompilerServices;
using System.Net;
using Microsoft.Xna.Framework.Audio;

namespace JumpJump
{
    /// <summary>
    /// A class representing a slime ghost
    /// </summary>
    public class JumpJumpSprite
    {
        private const float GRAVITY = 5000f;
        private const float SPEED = 750f;
        private const float JUMP = 1500f;

        private const int MAX_JUMPS = 2;
        private int jumpsUsed = 0;

        public const int maxhealth = 5;
        private const float INVULN_TIME = 1f;   
        private float invulnTimer;

        public int Health { get; private set; } = maxhealth;
        public bool IsDead => Health <= 0;

        private SoundEffect jumpSound;

        private bool wasJumpKeyDown = false;
        private bool _OnGround = true;
        private Vector2 _velocity;
        private KeyboardState keyboardState;

        public Texture2D texture;

        private Vector2 position = new Vector2(200, 877);

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        

        public bool flipped;

        private bool jump;

        private int currentFrame = 0;
        private double animationTimer = 0;

        private const int FRAME_W = 80;
        private const int FRAME_H = 60;
        private const int FRAME_COUNT = 5;
        private const double WALK_FRAME_TIME = 0.1;

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
            _velocity = Vector2.Zero;
            bounds.X = position.X - 50;
            bounds.Y = position.Y - 40;
        }

        public BoundingRectangle bounds = new BoundingRectangle(new Vector2(32,29), 80, 130);
        /// <summary>
        /// color overlay of the ghost
        /// </summary>
        public Color Color{get; set;} = Color.White;

        /// <summary>
        /// bounding volume of sprite
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        public void TakeDamage(int amount)
        {
            if (invulnTimer > 0 || IsDead) return;
            Health -= amount;
            invulnTimer = INVULN_TIME;
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            jumpSound = content.Load<SoundEffect>("jumpSound");
            texture = content.Load<Texture2D>("kangarooplain");

     //       pixel = new Texture2D(texture.GraphicsDevice, 1, 1);
       //     pixel.SetData(new[] { Color.Red });
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
                if (_wasJumping && jumpsUsed == 0)
                    jumpsUsed = 1;

                if (jumpsUsed < MAX_JUMPS)
                {
                    _velocity.Y = -JUMP;
                    _OnGround = false;
                    jump = true;
                    jumpsUsed++;
                    jumpSound.Play();

                    currentFrame = 0;
                    animationTimer = 0;
                }
            }

            wasJumpKeyDown = jumpKeyDown; 

        }

        public void Update(GameTime gameTime, List<BoundingRectangle> solids, GroundSprite ground)
        {
            if (invulnTimer > 0) invulnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateVelocity(gameTime);
            position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            bounds.X = position.X - 50;
            bounds.Y = position.Y - 40;

            _OnGround = false;

            foreach (var solid in solids)
            {
                if (bounds.CollidesWith(solid))
                {
                    ResolveCollision(solid);
                }
            }
            if (bounds.CollidesWith(ground.Bounds))
            {
                ResolveCollision(ground.Bounds);
            }
            UpdateAnimation(gameTime);
        }

        private const double JUMP_FRAME_TIME = 0.05;
        private bool _wasJumping;

        private void UpdateAnimation(GameTime gameTime)
        {
            if (jump)
            {
                if (!_wasJumping)
                {
                    currentFrame = 0;
                    animationTimer = 0;
                }

                if (currentFrame < FRAME_COUNT - 1)
                {
                    animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (animationTimer >= JUMP_FRAME_TIME)
                    {
                        animationTimer -= JUMP_FRAME_TIME;
                        currentFrame++;
                    }
                }
            }
            else if (_velocity.X != 0)                  
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer >= WALK_FRAME_TIME)
                {
                    animationTimer -= WALK_FRAME_TIME;
                    currentFrame = (currentFrame + 1) % FRAME_COUNT;
                }
            }
            else
            {
                currentFrame = 0;
                animationTimer = 0;
            }

            _wasJumping = jump;
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
                    jumpsUsed = 0; 
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
            var source = new Rectangle(currentFrame * FRAME_W, 0, FRAME_W, FRAME_H);
            var origin = new Vector2(FRAME_W / 2f, FRAME_H / 2f);

            spriteBatch.Draw(texture, position, source, Color.PaleVioletRed, 0, origin, 3f, spriteEffects, 0);
            bool hidden = invulnTimer > 0 && ((int)(invulnTimer * 10) % 2 == 0);
            if (!hidden)
                spriteBatch.Draw(texture, position, source, Color, 0, origin, 3f, spriteEffects, 0);
        }   
    }
}
