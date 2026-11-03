using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JumpJump
{
    public class TitleScreenText
    {
        private Texture2D texture;
        private readonly string assetName;
        private readonly Vector2 basePosition; 
        private double timer;
        public float Height { get; set; } = 25f;
        public float Speed { get; set; } = 0.8f;

        public bool Bounce { get; set; } = true;

        public TitleScreenText(string assetName, Vector2 centerPosition)
        {
            this.assetName = assetName;
            basePosition = centerPosition;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(assetName);
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            double wave = Math.Sin(timer * Speed * MathHelper.TwoPi);
            float offset = Bounce
                ? -(float)Math.Abs(wave) * Height
                : (float)wave * (Height / 2f);

            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            spriteBatch.Draw(texture, basePosition + new Vector2(0, offset), null, Color.White, 0f, origin, 1.5f, SpriteEffects.None, 0f);
        }
    }
}