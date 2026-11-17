using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Linq;
using System.Collections.Generic;

namespace JumpJump
{
public class hellLevel
{
    private const float LEVEL_WIDTH = 3840f;
    private const float SCREEN_WIDTH = 1920f;
    private const float CAMERA_LEAD = 300f;
    private const float BOUNDS_OFFSET_X = 50f;

    private const int FLY_COUNT = 3;
    private const float FLY_SPACING = 900f;
    private const float FLY_Y = 200f;

    private const int KANGAROO_COUNT = 3;
    private const float KANGAROO_SPACING = 900f;
    private const float KANGAROO_Y = 949f;
    public hellKanagrooSprite[] hellKangaroos;
    public flyingKangarooSprite[] flyKangaroos;
    public firewheelSprite[] wheels;
    private float[] prevDelta = new float[FLY_COUNT];  
    private bool[] hasDropped = new bool[FLY_COUNT]; 

    private const float FALL_TIME = 0.55f;  
    private const float LEAD_FACTOR = 0.7f;
    private float prevPlayerX;
    private float playerVelX;
    public bool change;

    private Texture2D parallaxBackground;
    private JumpJumpSprite player;
    private GroundSprite _ground;

    private SaveData _save;

    private portalSprite portal = new portalSprite(new Vector2(3700,830), "overworldPortal");

    public hellLevel(JumpJumpSprite player, GroundSprite ground, SaveData save)
    {
        this.player = player;
        _save = save;
        _ground = ground;
        hellKangaroos = new hellKanagrooSprite[KANGAROO_COUNT];
        wheels = new firewheelSprite[KANGAROO_COUNT];
        flyKangaroos = new flyingKangarooSprite[FLY_COUNT];
        for (int i = 0; i < KANGAROO_COUNT; i++)
            hellKangaroos[i] = new hellKanagrooSprite(new Vector2(i * KANGAROO_SPACING, KANGAROO_Y));
        for (int i = 0; i < KANGAROO_COUNT; i++)
            wheels[i] = new firewheelSprite(new Vector2(i * KANGAROO_SPACING, KANGAROO_Y));
        for (int i = 0; i < FLY_COUNT; i++)
            flyKangaroos[i] = new flyingKangarooSprite(new Vector2(i * FLY_SPACING, FLY_Y));
        Array.Fill(prevDelta, float.NaN);
            
    }

    public void LoadContent(ContentManager content)
    {
        player.texture = content.Load<Texture2D>("kangaroowitharmor");
        parallaxBackground = content.Load<Texture2D>("hellbackground");
        foreach (var w in hellKangaroos) w.LoadContent(content);
        foreach (var w in wheels) w.LoadContent(content);
        foreach (var w in flyKangaroos) w.LoadContent(content);
        portal.LoadContent(content);
    }

    private float GetCameraLeft()
    {
        float maxCamX = LEVEL_WIDTH - (SCREEN_WIDTH - CAMERA_LEAD);
        float playerX = MathHelper.Clamp(player.Position.X, CAMERA_LEAD, maxCamX);
        return playerX - CAMERA_LEAD;
    }
    public void Enter()
    {
            change = false;
            player.SetPosition(new Vector2(200, 960));

            // reset player velocity tracking (so the first frame doesn't compute a fake velocity)
            prevPlayerX = 200f;
            playerVelX = 0f;

            // the camera starts at X = 0 when you spawn, so its right edge is SCREEN_WIDTH
            float spawnStart = SCREEN_WIDTH + 100f;

            for (int i = 0; i < hellKangaroos.Length; i++)
                hellKangaroos[i].Respawn(spawnStart + i * KANGAROO_SPACING);

            for (int i = 0; i < flyKangaroos.Length; i++)
            {
                flyKangaroos[i].SetX(spawnStart + i * FLY_SPACING);
                prevDelta[i] = float.NaN;
                hasDropped[i] = false;
            }

            foreach (var w in wheels)
            {
                w.alive = false;
                w.dropped = false;
            }
    }

    public void Update(GameTime gameTime)
    {
        float camLeft = GetCameraLeft();
        float camRight = camLeft + SCREEN_WIDTH;
        portal.IsActive = true;
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (dt > 0f)
        {
            float rawVel = (player.Position.X - prevPlayerX) / dt;
            playerVelX = MathHelper.Lerp(playerVelX, rawVel, 0.2f);   // smooth out jitter
        }
        prevPlayerX = player.Position.X;

        foreach (var w in hellKangaroos)
        {
            if (w.IsDead)
            {
                float rightmost = hellKangaroos.Where(k => k != w).Max(k => k.X);
                float safeSpawnX = MathF.Max(rightmost + KANGAROO_SPACING, camRight + 100f);
                
                w.Respawn(safeSpawnX);
                continue;
            }
            
            w.Update(gameTime, player.Position.X, player.bounds, hellKangaroos);
        }

        for (int i = 0; i < flyKangaroos.Length; i++)
        {
            var w = flyKangaroos[i];
            w.Update(gameTime);

            if (w.X < camLeft - 100f)
            {
                float rightmost = flyKangaroos.Max(f => f.X);
                w.SetX(MathF.Max(rightmost + KANGAROO_SPACING, camRight + 100f));
                prevDelta[i] = float.NaN;     // ignore the teleport
                hasDropped[i] = false;        // re-arm for the next pass
                continue;
            }

            float targetX = player.Position.X + playerVelX * FALL_TIME * LEAD_FACTOR;
            float delta = w.X - targetX;
            if (!hasDropped[i] && !float.IsNaN(prevDelta[i]) && prevDelta[i] * delta <= 0f)
            {
                var wheel = wheels.FirstOrDefault(x => !x.alive);
                if (wheel != null)
                {
                    wheel.Drop(new Vector2(w.X, w.position.Y));
                    hasDropped[i] = true;
                }
            }

            prevDelta[i] = delta;
        }

        foreach (var w in wheels)
        {
            if (!w.alive) continue;

            w.Update(gameTime);

            if (w.Intersects(player.Bounds))
            {
                player.TakeDamage(2);      // replace with however your player takes damage
                w.alive = false;           // wheel is consumed so it only hits once
                w.dropped = false;
                continue;
            }

            if (w.position.Y >= KANGAROO_Y) 
            {
                w.alive = false;
                w.dropped = false;
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

        if (portal.IsActive && player.Bounds.CollidesWith(portal.Bounds))
        {
            change = true;
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        float offset = -GetCameraLeft();
        Matrix transform = Matrix.CreateTranslation(offset, 0, 0);

        spriteBatch.End();
        spriteBatch.Begin(transformMatrix: transform);

        spriteBatch.Draw(parallaxBackground, new Rectangle(0, -1080, 3840, 2160), Color.White);
        foreach (var w in hellKangaroos) w.Draw(gameTime, spriteBatch);
        foreach (var w in wheels)
            {
                if (w.dropped && w.alive)
                {
                    w.Draw(gameTime, spriteBatch);
                }
            }
        foreach (var w in flyKangaroos) w.Draw(gameTime, spriteBatch);
        portal.Draw(spriteBatch);

        spriteBatch.End();
        spriteBatch.Begin(transformMatrix: transform);
    }

    }
}