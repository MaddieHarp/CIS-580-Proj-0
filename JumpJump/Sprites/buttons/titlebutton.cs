using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JumpJump
{
    public abstract class titlebutton
    {
        private readonly string assetName;
        private MouseState previousMouse;

        protected Texture2D texture;

        /// <summary>Top-left corner of the button.</summary>
        public Vector2 Position { get; protected set; }

        public bool IsHovered { get; private set; }

        public Color NormalColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.LightGray;

        /// <summary>Pixels (in the original image) to trim off each side of the hitbox.
        /// X = left/right, Y = top/bottom.</summary>
        public Point Inset { get; set; } = Point.Zero;

        public Rectangle Bounds
        {
            get
            {
                if (texture == null) return Rectangle.Empty;

                return new Rectangle(
                    (int)(Position.X + Inset.X * Scale),
                    (int)(Position.Y + Inset.Y * Scale),
                    (int)((texture.Width  - Inset.X * 2) * Scale),
                    (int)((texture.Height - Inset.Y * 2) * Scale));
            }
        }


        public virtual void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();

            IsHovered = Bounds.Contains(mouse.Position);

            // Fire once per click: pressed this frame, wasn't pressed last frame
            bool clicked = mouse.LeftButton == ButtonState.Pressed
                        && previousMouse.LeftButton == ButtonState.Released;

            if (IsHovered && clicked) OnClick();

            previousMouse = mouse;
        }

        public float Scale { get; private set; }

        protected titlebutton(string assetName, Vector2 position, float scale = 1f)
        {
            this.assetName = assetName;
            Position = position;
            Scale = scale;
        }

        public virtual void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(assetName);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, IsHovered ? HoverColor : NormalColor,
                            0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }

        /// <summary>What happens when the button is clicked.</summary>
        protected abstract void OnClick();
    }
}