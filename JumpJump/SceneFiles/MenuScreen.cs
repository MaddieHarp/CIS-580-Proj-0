using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpJump;

public class TitleScreen : GameScreen
{
    Crate crate;
    private Texture2D _background;
    private TitleScreenText _titleText;
    private StartButton _startButton;
    private EndButton _endButton;
    private ResumeButton _resumeButton;

    private SaveData _saveData;
    private bool _hasSave;

    public override void Activate()
    {
        var content = ScreenManager.Game.Content;

        _hasSave = SaveData.Exists();
        _saveData = SaveData.Load();

        _startButton = new StartButton("startbutton", new Vector2(750, 500), StartNewGame, 0.7f);
        _endButton = new EndButton("exitbutton", new Vector2(750, 800), ScreenManager.Game.Exit, 0.7f);
        _resumeButton = new ResumeButton("resumebutton", new Vector2(750, 650), ResumeGame, 0.7f);
        crate = new Crate(ScreenManager.Game, CrateType.Slats);
        Vector3 camPos = new Vector3(8, 9, 12);
Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.Zero - camPos, Vector3.Up));

crate.Position = right * -8f;   // positive = toward screen right, negative = left

        var inset = new Point(47, 86);
        _startButton.Inset = inset;
        _endButton.Inset = inset;
        _resumeButton.Inset = inset;

        if (!_hasSave)
        {
            _resumeButton.NormalColor = Color.Gray;
            _resumeButton.HoverColor = Color.Gray;
        }

        _titleText = new TitleScreenText("titletext", new Vector2(960, 300));

        _startButton.LoadContent(content);
        _endButton.LoadContent(content);
        _resumeButton.LoadContent(content);
        _titleText.LoadContent(content);
        _background = content.Load<Texture2D>("background");
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        if (!IsActive) return;

        _titleText.Update(gameTime);
    }

    public override void HandleInput(GameTime gameTime, InputState input)
    {
        _startButton.Update(gameTime);
        _endButton.Update(gameTime);
        _resumeButton.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = ScreenManager.SpriteBatch;
        sb.Begin();
        sb.Draw(_background, new Rectangle(0, 0, 1920, 1080), Color.White);
        _titleText.Draw(sb);
        _startButton.Draw(sb);
        _endButton.Draw(sb);
        _resumeButton.Draw(sb);
        sb.End();

        crate.Draw();
    }

    private void StartNewGame()
    {
        ScreenManager.AddScreen(new Level1Screen(null));
        ExitScreen();
    }

    private void ResumeGame()
    {
        if (!_hasSave) return;

        if (_saveData.Level == 2)
            ScreenManager.AddScreen(new HellScreen(_saveData));
        else
            ScreenManager.AddScreen(new Level1Screen(_saveData));

        ExitScreen();
    }
}