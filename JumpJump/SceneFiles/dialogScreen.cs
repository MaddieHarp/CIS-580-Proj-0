using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JumpJump
{
    public class DialogueScreen : GameScreen
    {
        private const float CHARS_PER_SECOND = 45f;

        private readonly List<DialogueLine> _lines;
        private readonly Action _onFinished;

        private SpriteFont _font;
        private Texture2D _pixel;
        private DialogBoxSprite _box;

        private int _index;
        private string _wrapped = "";
        private float _visibleChars;

        private KeyboardState _prevKeys;
        private MouseState _prevMouse;

        public DialogueScreen(List<DialogueLine> lines, Action onFinished = null)
        {
            _lines = lines;
            _onFinished = onFinished;
            IsPopup = true;
        }

        public override void Activate()
        {
            var content = ScreenManager.Game.Content;
            _font = content.Load<SpriteFont>("arial");      // your font asset name

            _box = new DialogBoxSprite(scale: 1.3f);
            _box.LoadContent(content, "dialogBox");              // your box asset name
            _box.CenterBottom(1920, 1080, 40);

            _prevKeys = Keyboard.GetState();
            _prevMouse = Mouse.GetState();

            StartLine();
        }

        private void StartLine()
        {
            _wrapped = Wrap(_lines[_index].Text, _font, _box.TextArea.Width);
            _visibleChars = 0;
        }

        private void Advance()
        {
            if (_visibleChars < _wrapped.Length)
            {
                _visibleChars = _wrapped.Length;   // first click finishes the line
                return;
            }

            _index++;
            if (_index >= _lines.Count)
            {
                ExitScreen();
                _onFinished?.Invoke();
            }
            else StartLine();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (!IsActive) return;

            _visibleChars = Math.Min(_wrapped.Length,
                _visibleChars + CHARS_PER_SECOND * (float)gameTime.ElapsedGameTime.TotalSeconds);

            var keys = Keyboard.GetState();
            var mouse = Mouse.GetState();

            bool clicked = mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released;
            bool keyed = keys.IsKeyDown(Keys.Enter) && _prevKeys.IsKeyUp(Keys.Enter);

            _prevKeys = keys;
            _prevMouse = mouse;

            if (clicked || keyed) Advance();
        }

        public override void Draw(GameTime gameTime)
        {
            var sb = ScreenManager.SpriteBatch;
            sb.Begin(samplerState: SamplerState.PointClamp);

            _box.Draw(sb);
            DrawPortrait(sb, _lines[_index]);

            var area = _box.TextArea;
            string shown = _wrapped.Substring(0, (int)_visibleChars);
            sb.DrawString(_font, shown, new Vector2(area.X, area.Y), Color.Black);

            sb.End();
        }

        // Fit the portrait inside its slot, keeping its proportions and centering it
        private void DrawPortrait(SpriteBatch sb, DialogueLine line)
        {
            if (line.Portrait == null) return;

            Rectangle src = line.PortraitSource ?? line.Portrait.Bounds;
            Rectangle slot = _box.PortraitArea;

            float fit = MathF.Min(slot.Width / (float)src.Width, slot.Height / (float)src.Height);
            if (fit >= 1f) fit = MathF.Floor(fit);
            fit *= line.PortraitScale;

            int w = (int)(src.Width * fit);
            int h = (int)(src.Height * fit);
            var dest = new Rectangle(
                slot.X + (slot.Width - w) / 2 + line.PortraitOffset.X,
                slot.Y + (slot.Height - h) / 2 + line.PortraitOffset.Y, w, h);

            var effects = line.FlipPortrait ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sb.Draw(line.Portrait, dest, src, Color.White, 0f, Vector2.Zero, effects, 0f);
        }

        private static string Wrap(string text, SpriteFont font, int maxWidth)
        {
            var result = new StringBuilder();
            string line = "";

            foreach (var word in text.Split(' '))
            {
                string test = line.Length == 0 ? word : line + " " + word;
                if (font.MeasureString(test).X > maxWidth && line.Length > 0)
                {
                    result.AppendLine(line);
                    line = word;
                }
                else line = test;
            }
            result.Append(line);
            return result.ToString();
        }
    }
}