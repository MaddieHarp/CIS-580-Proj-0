using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using CollisionExample.Collisions;


namespace CollisionExample
{

    public class CoinSprite
    {


        private Vector2 position;

        private Texture2D texture;

        private double animationTimer;
        private short animationFrame;

        private BoundingCircle bounds;
        /// <summary>
        /// bounding vbolume of sprite
        /// </summary>
        public BoundingCircle Bounds => bounds;

        public bool Collected{get; set;} = false;

        /// <summary>
        /// Creates a new coin sprite
        /// </summary>
        /// <param name="position">The position of the sprite in the game</param>
        public CoinSprite(Vector2 position)
        {
            this.position = position;
            this.bounds = new BoundingCircle(position + new Vector2(-8,-8), 31);
        }

        public void Update(GameTime gameTime)
        {
            if (Collected) return;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 0.15)
            {
                animationFrame++;
                
                if (animationFrame > 2) 
                {
                    animationFrame = 0;
                }
                animationTimer -= 0.15; 
            }
        }

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("appleframes");
        }

        /// <summary>
        /// Draws the animated sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(Collected)return;

            var source = new Rectangle(animationFrame * 17, 0, 17, 20);
            
            spriteBatch.Draw(texture, position, source, Color.White, 0, new Vector2(16, 16), 3f, SpriteEffects.None, 0);
        }
    }
}
