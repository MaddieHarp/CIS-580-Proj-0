using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JumpJump.Collisions;

namespace JumpJump;

public class Level2Screen : GameScreen, ISaveableScreen
{
    private readonly SaveData _save;
    private Tilemap _tilemap;
    private Texture2D _background;
    private JumpJumpSprite _jumpjump;
    private GroundSprite _ground;
    private readonly List<BoundingRectangle> _solids = new List<BoundingRectangle>();

    private static readonly Vector2 SPAWN = new Vector2(200, 960);

    public Level2Screen(SaveData save)
    {
        _save = save;
    }

    public override void Activate()
    {
        try
        {
            var content = ScreenManager.Game.Content;

            _ground = new GroundSprite(new Vector2(0, 1030));
            _ground.LoadContent(content);

            _jumpjump = new JumpJumpSprite();
            _jumpjump.LoadContent(content);
            _jumpjump.texture = content.Load<Texture2D>("kangaroowitharmor");
            _jumpjump.SetPosition(SPAWN);

            _tilemap = new Tilemap("Content/level2.tmx", "ground3", "platformTile");
            _tilemap.LoadContent(content);
            _solids.AddRange(_tilemap.GetTileColliders());

            _background = content.Load<Texture2D>("background");   // or "hellbackground"

            // only restore a position if the save really belongs to this level
            if (_save != null && _save.Level == 2) ApplySave(_save);
            else SaveGame();
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LEVEL2 ACTIVATE FAILED: " + ex);
            throw;   // keep throwing so Visual Studio stops on the real line
        }
    }

    public override void Unload() { }

    private void ApplySave(SaveData save)
    {
        _jumpjump.SetPosition(new Vector2(save.PlayerX, save.PlayerY));
    }

    public void SaveGame()
    {
        new SaveData
        {
            Level = 2,
            PlayerX = _jumpjump.Position.X,
            PlayerY = _jumpjump.Position.Y,
            CollectedApples = new bool[0],
            MetLilJump = true,
            StartDialog = true
        }.Save();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        if (!IsActive) return;

        _jumpjump.Update(gameTime, _solids, _ground);
    }

    public override void Draw(GameTime gameTime)
    {
        if (_tilemap == null) return;   // Activate failed or hasn't run

        var sb = ScreenManager.SpriteBatch;
        sb.Begin();

        sb.Draw(_background, new Rectangle(0, 0, 1920, 1080), Color.White);
        _tilemap.Draw(sb);
        _jumpjump.Draw(sb);

        sb.End();
    }
}