using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CollisionExample
{
public class hellLevel
{
    private const float LEVEL_WIDTH = 3840f;
    private const float SCREEN_WIDTH = 1920f;
    private const float CAMERA_LEAD = 300f;
    private const float BOUNDS_OFFSET_X = 50f;

    private firewheelSprite fireWheel;

    private const int WHEEL_COUNT = 5;
    private const float WHEEL_SPACING = 600f;
    private const float WHEEL_Y = 950f;
    private firewheelSprite[] fireWheels;

    private Texture2D parallaxBackground;
    private SlimeGhostSprite player;

    public hellLevel(SlimeGhostSprite player)
    {
        this.player = player;
        fireWheels = new firewheelSprite[WHEEL_COUNT];
    for (int i = 0; i < WHEEL_COUNT; i++)
        fireWheels[i] = new firewheelSprite(new Vector2(0, WHEEL_Y));
    }

    public void LoadContent(ContentManager content)
    {
        parallaxBackground = content.Load<Texture2D>("hellbackground");
        foreach (var w in fireWheels) w.LoadContent(content);
    }

    private float GetCameraLeft()
    {
        float maxCamX = LEVEL_WIDTH - (SCREEN_WIDTH - CAMERA_LEAD);
        float playerX = MathHelper.Clamp(player.Position.X, CAMERA_LEAD, maxCamX);
        return playerX - CAMERA_LEAD;   // world x at the left edge of the screen
    }
    public void Enter()
    {
        player.SetPosition(new Vector2(200, 960));
    }

    public void Update(GameTime gameTime)
    {
        float camLeft = GetCameraLeft();
        float camRight = camLeft + SCREEN_WIDTH;

        foreach (var w in fireWheels)
        {
            w.Update(gameTime);

            // Off the left side: send it behind the rightmost wheel, but never on screen
            if (w.X < camLeft - 100f)
            {
                float rightmost = fireWheels.Max(f => f.X);
                w.SetX(MathF.Max(rightmost + WHEEL_SPACING, camRight + 100f));
            }
        }
        float minX = BOUNDS_OFFSET_X;
        float maxX = LEVEL_WIDTH - player.Bounds.Width + BOUNDS_OFFSET_X;

        float clampedX = MathHelper.Clamp(player.Position.X, minX, maxX);
        if (clampedX != player.Position.X)
        {
            player.Position = new Vector2(clampedX, player.Position.Y);
            player.bounds.X = clampedX - BOUNDS_OFFSET_X;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        float offset = -GetCameraLeft();
        Matrix transform = Matrix.CreateTranslation(offset, 0, 0);

        spriteBatch.End();
        spriteBatch.Begin(transformMatrix: transform);

        spriteBatch.Draw(parallaxBackground, new Rectangle(0, -1080, 3840, 2160), Color.White);
        foreach (var w in fireWheels) w.Draw(gameTime, spriteBatch);

        spriteBatch.End();
        spriteBatch.Begin(transformMatrix: transform);
    }
}
}