using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JumpJump.Collisions;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace JumpJump;

public class HellScreen : GameScreen, ISaveableScreen
{
    private readonly SaveData _save;   // null = arriving from level 1, otherwise resuming

    private JumpJumpSprite _jumpjump;
    private GroundSprite _ground;
    private hellLevel _hell;
    private readonly List<BoundingRectangle> _solids = new List<BoundingRectangle>();

    private readonly List<Bullet> _bullets = new List<Bullet>();
    private Texture2D _pixel;
    private KeyboardState _prevKeys;

    private const float FIRE_COOLDOWN = 0.5f; 
    private float _fireTimer = 0f;

    public HellScreen(SaveData save)
    {
        _save = save;
    }

    public override void Activate()
    {
        var content = ScreenManager.Game.Content;

        _jumpjump = new JumpJumpSprite();
        _ground = new GroundSprite(new Vector2(0, 1030));
        _hell = new hellLevel(_jumpjump);

        _jumpjump.LoadContent(content);
        _ground.LoadContent(content);
        _hell.LoadContent(content);

        _hell.Enter();

        _pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        if (_save != null)
            _jumpjump.Position = new Vector2(_save.PlayerX, _save.PlayerY);
        else
            SaveGame();
    }

    public void SaveGame()
    {
        new SaveData
        {
            Level = 2,
            PlayerX = _jumpjump.Position.X,
            PlayerY = _jumpjump.Position.Y,
            CollectedApples = Array.Empty<bool>(),
            MetLilJump = true
        }.Save();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        if (!IsActive) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Enemies move first so their bounds are current this frame
        _hell.Update(gameTime);

        // Rebuild solids from living enemies only
        _solids.Clear();
        foreach (var k in _hell.hellKangaroos)
            if (!k.IsDead) _solids.Add(k.Bounds);

        _jumpjump.Update(gameTime, _solids, _ground);

        // Firing with a cooldown
        var keys = Keyboard.GetState();
        _fireTimer -= dt;

        if (keys.IsKeyDown(Keys.K) && _fireTimer <= 0f)
        {
            float x = _jumpjump.flipped ? _jumpjump.Bounds.Left - 24 : _jumpjump.Bounds.Right;
            float y = _jumpjump.Bounds.Y + 50;
            _bullets.Add(new Bullet(new Vector2(x, y), _jumpjump.flipped));
            _fireTimer = FIRE_COOLDOWN;
        }

        // Move bullets and check hits against enemies
        foreach (var b in _bullets)
        {
            b.Update(gameTime, 3840f);

            foreach (var k in _hell.hellKangaroos)
            {
                if (b.Alive && !k.IsDead && b.Bounds.CollidesWith(k.Bounds))
                {
                    k.TakeDamage(1);
                    b.Alive = false;
                }
            }
        }
        _bullets.RemoveAll(b => !b.Alive);

        // Contact damage (the player's invulnerability timer limits how often this lands)
        foreach (var k in _hell.hellKangaroos)
            if (k.TouchingPlayer && !k.IsDead)
                _jumpjump.TakeDamage(1);

        if (_jumpjump.IsDead)
        {
            SaveData.Delete();
            foreach (var screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            ScreenManager.AddScreen(new TitleScreen());
            return;
            
        }
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = ScreenManager.SpriteBatch;
        sb.Begin();

        _hell.Draw(gameTime, sb);      // leaves the batch open with the camera transform
        _ground.Draw(gameTime, sb);
        _jumpjump.Draw(sb);

        foreach (var b in _bullets)
            b.Draw(sb, _pixel);

        // HUD: restart the batch without the camera transform so it stays fixed on screen
        sb.End();
        sb.Begin();

        const int barW = 300, barH = 30;
        sb.Draw(_pixel, new Rectangle(40, 40, barW, barH), Color.DarkRed);
        sb.Draw(_pixel, new Rectangle(40, 40, barW * _jumpjump.Health / JumpJumpSprite.maxhealth, barH), Color.LimeGreen);

        sb.End();
    }
}