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
    private SoundEffect _collectSound;
    private SoundEffect _gunSound;

    private SoundEffect _portalSound;

    private JumpJumpSprite _jumpjump;
    private liljumpSprite _lilJump;
    private Texture2D _jumpPortrait;
    private Texture2D _lilJumpPortrait;
    private bool _dialogueStarted;
    private portalSprite _exitPortal;
    private particleEffect _portalExplosion;
    private particleEffect _changeExplo;
    private GroundSprite _ground;
    private Tilemap _tilemap;

    private readonly List<AppleSprite> _apples = new List<AppleSprite>();
    private readonly List<BoundingRectangle> _solids = new List<BoundingRectangle>();

    private int _applesLeft;
    private bool _metLilJump;
    private bool _lilJumpGone;

    private bool introstarted = false;


    private float _swapTimer;
    private const float SWAP_DELAY = .6f; 

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
        _exitPortal = new portalSprite(new Vector2(1760, 750), "portal");
        _ground = new GroundSprite(new Vector2(0, 1030));

        _portalExplosion = new particleEffect(game, 10000, Color.Red);
        _changeExplo = new particleEffect(game, 100000, Color.White);
        _portalExplosion.DrawOrder = 100;
        game.Components.Add(_portalExplosion);
        game.Components.Add(_changeExplo);

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
        _jumpPortrait = content.Load<Texture2D>("kangarooplain");
        _lilJumpPortrait = content.Load<Texture2D>("liljump");

        _spriteFont = content.Load<SpriteFont>("arial");
        _background = content.Load<Texture2D>("background");
        _eatSound = content.Load<SoundEffect>("eating");
        _gunSound = content.Load<SoundEffect>("gun_reload");
        _portalSound = content.Load<SoundEffect>("portal_open");
        _collectSound = content.Load<SoundEffect>("apple_pickup");

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
        ScreenManager.Game.Components.Remove(_changeExplo);
    }

    private void ApplySave(SaveData save)
    {
        for (int i = 0; i < _apples.Count && i < save.CollectedApples.Length; i++)
            _apples[i].Collected = save.CollectedApples[i];

        _applesLeft = _apples.Count(a => !a.Collected);

        _metLilJump = save.MetLilJump;
        _lilJumpGone = save.MetLilJump;
        _exitPortal.IsActive = save.MetLilJump;
        introstarted = save.StartDialog;

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
            MetLilJump = _metLilJump,
            StartDialog = introstarted
        }.Save();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        if (!IsActive) return;
        if (!introstarted)
        {
            introstarted = true;
            var StartLines = new List<DialogueLine>
                {
                    new DialogueLine("JumpJump I'm REALLY hungry", _lilJumpPortrait, new Rectangle(0, 0, 15, 15)){
                    PortraitScale = .6f,
                    PortraitOffset = new Point(-25, -20)
                    },
                    new DialogueLine("Can you get me those apples?", _lilJumpPortrait, new Rectangle(0, 0, 15, 15)){
                    PortraitScale = .6f,
                    PortraitOffset = new Point(-25, -20)
                    },
                    new DialogueLine("Of course son.", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                    PortraitScale = .54f,
                    PortraitOffset = new Point(-23, -15)
                    },
                };

            ScreenManager.AddScreen(new DialogueScreen(StartLines, null));
            introstarted = true;
            SaveGame();
            return;
            
        }
        if (_swapTimer > 0)
        {
            _swapTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_swapTimer <= 0)
            {
                _jumpjump.texture = ScreenManager.Game.Content.Load<Texture2D>("kangaroowitharmor");
            }
        }

        foreach (var apple in _apples) apple.Update(gameTime);

        _jumpjump.Update(gameTime, _solids, _ground);
        _jumpjump.Color = Color.White;

        const float BOUNDS_OFFSET_X = 50f;
        float minX = BOUNDS_OFFSET_X;
        float maxX = 1920f - _jumpjump.Bounds.Width + BOUNDS_OFFSET_X;

        float clampedX = MathHelper.Clamp(_jumpjump.Position.X, minX, maxX);
        if (clampedX != _jumpjump.Position.X)
        {
            _jumpjump.Position = new Vector2(clampedX, _jumpjump.Position.Y);
            _jumpjump.bounds.X = clampedX - BOUNDS_OFFSET_X;
        }

        foreach (var apple in _apples)
        {
            if (!apple.Collected && apple.Bounds.CollidesWith(_jumpjump.Bounds))
            {
                _jumpjump.Color = Color.Red;
                apple.Collected = true;
                _applesLeft--;
                _collectSound.Play();
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
            _eatSound.Play();

            ScreenManager.AddScreen(new DialogueScreen(lines, () =>
            {
                _portalSound.Play();
                _metLilJump = true;
                _exitPortal.IsActive = true;
                _lilJumpGone = true;
                SaveGame();

                Vector2 portalCenter = new Vector2(
                    _exitPortal.Position.X + (_exitPortal.Bounds.Width / 2)+50,
                    _exitPortal.Position.Y + (_exitPortal.Bounds.Height / 2));
                _portalExplosion.Explode(portalCenter);
                Vector2 jumpCenter = new Vector2(
                    _jumpjump.Position.X + (_jumpjump.Bounds.Width/2)-50,
                    _jumpjump.Position.Y + (_jumpjump.Bounds.Height/2)-50
                );

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
                    new DialogueLine("Good thing I have my old uniform \n in my pouch", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                        PortraitScale = .54f,
                        PortraitOffset = new Point(-23, -15)
                    },
                    new DialogueLine("and this sick ass gun. \n(Click or Press k to shoot)", _jumpPortrait, new Rectangle(45, 10, 20, 20)){
                        PortraitScale = .54f,
                        PortraitOffset = new Point(-23, -15)
                    },
                };

                ScreenManager.AddScreen(new DialogueScreen(afterLines, () =>
                {
                    _changeExplo.Explode(jumpCenter);
                    _swapTimer = SWAP_DELAY;
                    _gunSound.Play();
                    

                }));
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
            
        } 
        _exitPortal.Draw(sb);
        _jumpjump.Draw(sb);

        sb.DrawString(_spriteFont, $"APPLES LEFT: {_applesLeft}", new Vector2(5, 5), Color.Gold);
        sb.DrawString(_spriteFont, "TO EXIT GAME PRESS ESC", new Vector2(1490, 2), Color.Gold);

        sb.End();
    }
}