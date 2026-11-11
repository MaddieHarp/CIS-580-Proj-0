using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace JumpJump;

public class JumpJumpGame : Game
{
    private GraphicsDeviceManager graphics;
    private ScreenManager screenManager;
    private Song backgroundMusic;

    public JumpJumpGame()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        graphics.PreferredBackBufferWidth = 1920;
        graphics.PreferredBackBufferHeight = 1080;
        graphics.ApplyChanges();

        screenManager = new ScreenManager(this);
        Components.Add(screenManager);
        screenManager.AddScreen(new TitleScreen());
    }

    protected override void LoadContent()
    {
        backgroundMusic = Content.Load<Song>("backgroundMusic");
        MediaPlayer.Play(backgroundMusic);
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = .1f;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSkyBlue);
        base.Draw(gameTime); 
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        foreach (var screen in screenManager.GetScreens().OfType<ISaveableScreen>())
            screen.SaveGame();
        base.OnExiting(sender, args);
    }
}