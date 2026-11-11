using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JumpJump
{
    public class DialogBoxSprite
    {
        private Texture2D texture;

        public Vector2 Position { get; set; }
        public float Scale { get; set; }

        public Rectangle PortraitSlot = new Rectangle(20, 20, 160, 160);
        public Rectangle TextSlot     = new Rectangle(150, 40, 780, 160);

        public DialogBoxSprite(float scale = 1f)
        {
            Scale = scale;
        }

        public int Width  => (int)(texture.Width  * Scale);
        public int Height => (int)(texture.Height * Scale);

        public Rectangle PortraitArea => ToScreen(PortraitSlot);
        public Rectangle TextArea     => ToScreen(TextSlot);

        private Rectangle ToScreen(Rectangle r) => new Rectangle(
            (int)(Position.X + r.X * Scale),
            (int)(Position.Y + r.Y * Scale),
            (int)(r.Width  * Scale),
            (int)(r.Height * Scale));

        public void LoadContent(ContentManager content, string assetName)
        {
            texture = content.Load<Texture2D>(assetName);
        }

        public void CenterBottom(int screenW, int screenH, int margin)
        {
            Position = new Vector2((screenW - Width) / 2f, screenH - Height - margin);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}