using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using JumpJump.Collisions;

namespace JumpJump;

public class Level1Screen : GameScreen, ISaveableScreen
{
    private readonly SaveData _save;

    private Texture2D _background;
    private SpriteFont _spriteFont;
    private SoundEffect _eatSound;

    private JumpJumpSprite _jumpjump;
    private liljumpSprite _lilJump;
    private Texture2D _jumpPortrait;
    private Texture2D _lilJumpPortrait;
    private bool _dialogueStarted;
    private SpeechBubble speechBubble;
    private portalSprite _exitPortal;
    private particleEffect _portalExplosion;
    private GroundSprite _ground;
    private Tilemap _tilemap;

    private readonly List<AppleSprite> _apples = new List<AppleSprite>();
    private readonly List<BoundingRectangle> _solids = new List<BoundingRectangle>();

    private int _applesLeft;
    private bool _metLilJump;
    private bool _lilJumpGone;

    public Level1Screen(SaveData save)
    {
        _save = save;
    }

    public override void Activate()
    {
        var game = ScreenManager.Game;
        var content = game.Content;

        _jumpjump = new JumpJumpSprite();
        _lilJump = new liljumpSprite(new Vector2(1600, 855));
        _exitPortal = new portalSprite(new Vector2(1760, 750));
        _ground = new GroundSprite(new Vector2(0, 1030));
        speechBubble = new SpeechBubble(new Vector2(1500, 800),"test", false);

        _portalExplosion = new particleEffect(game, 5000);
        _portalExplosion.DrawOrder = 100;
        game.Components.Add(_portalExplosion);

        _tilemap = new Tilemap("Content/level1.tmx", "platformTile", "ground3");
        _tilemap.LoadContent(content);

        foreach (Vector2 point in _tilemap.GetPoints("AppleLayer", "apple"))
            _apples.Add(new AppleSprite(point));
        _applesLeft = _apples.Count;
        foreach (var apple in _apples) apple.LoadContent(content);

        _solids.AddRange(_tilemap.GetTileColliders());

        _lilJump.LoadContent(content);
        _exitPortal.LoadContent(content);
        _ground.LoadContent(content);
        _jumpjump.LoadContent(content);
        speechBubble.LoadContent(content);
        _jumpPortrait = content.Load<Texture2D>("kangroo");
        _lilJumpPortrait = content.Load<Texture2D>("liljump");

        _spriteFont = content.Load<SpriteFont>("arial");
        _background = content.Load<Texture2D>("background");
        _eatSound = content.Load<SoundEffect>("eating");

        if (_save != null)
        {
            ApplySave(_save);
        }
        else
        {
            SaveGame();
        }
    }

    public override void Unload()
    {
        ScreenManager.Game.Components.Remove(_portalExplosion);
    }

    private void ApplySave(SaveData save)
    {
        for (int i = 0; i < _apples.Count && i < save.CollectedApples.Length; i++)
            _apples[i].Collected = save.CollectedApples[i];

        _applesLeft = _apples.Count(a => !a.Collected);

        _metLilJump = save.MetLilJump;
        _lilJumpGone = save.MetLilJump;
        _exitPortal.IsActive = save.MetLilJump;

        _jumpjump.Position = new Vector2(save.PlayerX, save.PlayerY);
    }

    public void SaveGame()
    {
        new SaveData
        {
            Level = 1,
            PlayerX = _jumpjump.Position.X,
            PlayerY = _jumpjump.Position.Y,
            CollectedApples = _apples.Select(a => a.Collected).ToArray(),
            MetLilJump = _metLilJump
        }.Save();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        if (!IsActive) return;

        foreach (var apple in _apples) apple.Update(gameTime);

        _jumpjump.Update(gameTime, _solids, _ground);
        _jumpjump.Color = Color.White;

        foreach (var apple in _apples)
        {
            if (!apple.Collected && apple.Bounds.CollidesWith(_jumpjump.Bounds))
            {
                _jumpjump.Color = Color.Red;
                apple.Collected = true;
                _applesLeft--;
                _eatSound.Play();
                SaveGame();
            }
        }

        if (_applesLeft == 0 && !_metLilJump && !_dialogueStarted
            && _jumpjump.Bounds.CollidesWith(_lilJump.Bounds))
        {
            _dialogueStarted = true;

            var lines = new List<DialogueLine>
            {
                new DialogueLine("Thank you JumpJump! Yum!", _lilJumpPortrait, new Rectangle(0, 0, 15, 15)){
                    PortraitScale = .6f,
                    PortraitOffset = new Point(-25, -20)
                },
                new DialogueLine("You're Welcome Son!", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                    PortraitScale = .54f,
                    PortraitOffset = new Point(-23, -15)
                },
            };

            ScreenManager.AddScreen(new DialogueScreen(lines, () =>
            {
                _metLilJump = true;
                _lilJumpGone = true;
                _exitPortal.IsActive = true;
                SaveGame();

                Vector2 portalCenter = new Vector2(
                    _exitPortal.Position.X + (_exitPortal.Bounds.Width / 2),
                    _exitPortal.Position.Y + (_exitPortal.Bounds.Height / 2));
                _portalExplosion.Explode(portalCenter);

                var afterLines = new List<DialogueLine>
                {
                    new DialogueLine("OH NO!", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                        PortraitScale = .54f,
                        PortraitOffset = new Point(-23, -15)
                    },
                    new DialogueLine("My son has been taken to hell", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                        PortraitScale = .54f,
                        PortraitOffset = new Point(-23, -15)
                    },
                    new DialogueLine("I must save him!", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                        PortraitScale = .54f,
                        PortraitOffset = new Point(-23, -15)
                    },
                };

                ScreenManager.AddScreen(new DialogueScreen(afterLines));
            }));
        }

        if (_exitPortal.IsActive && _jumpjump.Bounds.CollidesWith(_exitPortal.Bounds))
        {
            ScreenManager.AddScreen(new HellScreen(null));
            ExitScreen();
        }
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = ScreenManager.SpriteBatch;
        sb.Begin();

        sb.Draw(_background, new Rectangle(0, 0, 1920, 1080), Color.White);

        foreach (var apple in _apples) apple.Draw(gameTime, sb);

        _tilemap.Draw(sb);


        if (!_lilJumpGone)
        {
            _lilJump.Draw(sb);
            speechBubble.Draw(sb);
            
        } 
        _exitPortal.Draw(sb);
        _jumpjump.Draw(sb);

        sb.DrawString(_spriteFont, $"APPLES LEFT: {_applesLeft}", new Vector2(5, 5), Color.Gold);
        sb.DrawString(_spriteFont, "TO EXIT GAME PRESS ESC", new Vector2(1500, 2), Color.Gold);
        sb.DrawString(_spriteFont, "Collect all apples and give it to your son", new Vector2(500, 2), Color.Gold);

        sb.End();
    }
}