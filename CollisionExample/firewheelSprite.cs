using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CollisionExample
{
    public class firewheelSprite
    {
        private Texture2D texture;
        private Vector2 position;
        private float rotationAngle = 0f;
        private float speed = 300f;

        public float X => position.X;

        public firewheelSprite(Vector2 startPosition)
        {
            position = startPosition;
        }

        public void SetX(float x) => position.X = x;

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("firewheel");
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.X -= speed * dt;
            rotationAngle -= 5f * dt;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture == null) return;
            Vector2 origin = new Vector2(15f, 14.5f);
            spriteBatch.Draw(texture, position, null, Color.White, rotationAngle, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}