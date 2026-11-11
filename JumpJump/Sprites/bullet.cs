using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JumpJump.Collisions;

namespace JumpJump
{
    public class Bullet
    {
        private const float SPEED = 1000f;
        private const int WIDTH = 5;
        private const int HEIGHT = 10;

        private Vector2 position;
        private readonly float direction; 

        public bool Alive { get; set; } = true;
        public BoundingRectangle Bounds => new BoundingRectangle(position, WIDTH, HEIGHT);

        public Bullet(Vector2 start, bool facingLeft)
        {
            position = start;
            direction = facingLeft ? -1f : 1f;
        }

        public void Update(GameTime gameTime, float levelWidth)
        {
            position.X += direction * SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (position.X < 0 || position.X > levelWidth)
                Alive = false;
        }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(pixel, new Rectangle((int)position.X, (int)position.Y, WIDTH, HEIGHT), Color.Yellow);
        }
    }
}