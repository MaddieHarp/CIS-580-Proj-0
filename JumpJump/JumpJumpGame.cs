using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using JumpJump.Collisions;

namespace JumpJump
{
    public class JumpJumpGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Texture2D background;
        private Texture2D titleScreenTexture;
        private Song backgroundMusic;
        private liljumpSprite lilJump;
        private portalSprite exitPortal; 
        private particleEffect portalExplosion;       
        private bool metLilJump = false;
        private bool lilJumpGone = false; 
        private List<AppleSprite> apples = new List<AppleSprite>();
        private List<Vector2> applePoints;
        private GroundSprite ground;
        private int applesLeft = 7;
        private SoundEffect eatSound;
        private JumpJumpSprite jumpjump;
        private Texture2D pixel;
        private SpriteFont spriteFont;
        private bool onTitleScreen = true;
        private int currentLevel = 1;
        private hellLevel hell;

        private Tilemap tilemap;

        private TitleScreenText titleText;

        private StartButton startButton;
        private EndButton endButton;
        private ResumeButton resumeButton;

        private List<BoundingRectangle> solids = new List<BoundingRectangle>();

        //saving!!
        private SaveData saveData;
        private bool hasSave;

        public JumpJumpGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            hasSave = SaveData.Exists();
            saveData = SaveData.Load();

            startButton = new StartButton("startbutton", new Vector2(750, 500), StartNewGame, 0.7f);
            endButton = new EndButton("exitbutton", new Vector2(750, 800), Exit, 0.7f);
            resumeButton = new ResumeButton("resumebutton", new Vector2(750, 650), ResumeGame, 0.7f);
            var inset = new Point(47, 86);
            startButton.Inset  = inset;
            endButton.Inset    = inset;
            resumeButton.Inset = inset;
            if (!hasSave)
            {
                resumeButton.NormalColor = Color.Gray;
                resumeButton.HoverColor = Color.Gray;
            }

            jumpjump = new JumpJumpSprite();
            lilJump = new liljumpSprite(new Vector2(1700, 855));
            exitPortal = new portalSprite(new Vector2(1760, 750));
            ground = new GroundSprite(new Vector2(0, 1030));
            titleText = new TitleScreenText("titletext", new Vector2(960, 300));
            
            portalExplosion = new particleEffect(this, 5000);
            Components.Add(portalExplosion);

            hell = new hellLevel(jumpjump);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            startButton.LoadContent(Content);
            endButton.LoadContent(Content);
            resumeButton.LoadContent(Content);
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            tilemap = new Tilemap("Content/level1.tmx", "platformTile", "ground3");
            tilemap.LoadContent(Content);



            applePoints = tilemap.GetPoints("AppleLayer", "apple");
            foreach (Vector2 a in applePoints)
            {
                apples.Add(new AppleSprite(a));
                
            }
            applesLeft = apples.Count;

            foreach (var apple in apples) apple.LoadContent(Content);

            

            solids.AddRange(tilemap.GetTileColliders());

            lilJump.LoadContent(Content);
            exitPortal.LoadContent(Content); 
            ground.LoadContent(Content);

            jumpjump.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("arial");
            background = Content.Load<Texture2D>("background");
            eatSound = Content.Load<SoundEffect>("eating");
            backgroundMusic = Content.Load<Song>("backgroundMusic");
            titleScreenTexture = Content.Load<Texture2D>("background");
            titleText.LoadContent(Content);


            hell.LoadContent(Content);
            
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .1f;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (onTitleScreen)
            {
                titleText.Update(gameTime);
                startButton.Update(gameTime);
                endButton.Update(gameTime);
                resumeButton.Update(gameTime);
                base.Update(gameTime);
                return;
            }
            if (currentLevel == 1)
            {
                foreach (var apple in apples) apple.Update(gameTime);
                
                jumpjump.Update(gameTime, solids, ground);
                jumpjump.Color = Color.White;

                foreach(var apple in apples)
                {
                    if (!apple.Collected && apple.Bounds.CollidesWith(jumpjump.Bounds))
                    {
                        jumpjump.Color = Color.Red;
                        apple.Collected = true;
                        applesLeft--;
                        eatSound.Play();
                        SaveGame();
                    }
                }

                if (applesLeft == 0 && !metLilJump && !lilJumpGone && jumpjump.Bounds.CollidesWith(lilJump.Bounds))
                {
                    metLilJump = true; 
                    SaveGame();
                    lilJumpGone = true;      
                    exitPortal.IsActive = true; 

                    Vector2 portalCenter = new Vector2(exitPortal.Position.X + (exitPortal.Bounds.Width / 2), exitPortal.Position.Y + (exitPortal.Bounds.Height / 2));
                    portalExplosion.Explode(portalCenter);
                }

                if (exitPortal.IsActive && jumpjump.Bounds.CollidesWith(exitPortal.Bounds))
                {
                    currentLevel = 2;
                    hell.Enter();
                    SaveGame();
                }
            }
            else if (currentLevel == 2)
            {
                jumpjump.Update(gameTime, solids, ground);
                hell.Update(gameTime);
                jumpjump.Color = Color.Red;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            spriteBatch.Begin();

            if (onTitleScreen)
            {
                spriteBatch.Draw(titleScreenTexture, new Rectangle(0, 0, 1920, 1080), Color.White);
                titleText.Draw(spriteBatch);
                startButton.Draw(spriteBatch);
                endButton.Draw(spriteBatch);
                resumeButton.Draw(spriteBatch);
            }
            else if (currentLevel == 1)
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);

                foreach (var apple in apples) apple.Draw(gameTime, spriteBatch);

                
                //ground.Draw(gameTime, spriteBatch);
                tilemap.Draw(spriteBatch);
                
                if (!lilJumpGone) lilJump.Draw(spriteBatch);
                exitPortal.Draw(spriteBatch);

                jumpjump.Draw(spriteBatch);

                spriteBatch.DrawString(spriteFont, $"APPLES LEFT: {applesLeft}", new Vector2(5,5), Color.Gold);
                spriteBatch.DrawString(spriteFont, $"TO EXIT GAME PRESS ESC", new Vector2(1500,2), Color.Gold);
                spriteBatch.DrawString(spriteFont, "Collect all apples and give it to your son", new Vector2(500,2), Color.Gold);
            }
            else if (currentLevel == 2)
            {
                hell.Draw(gameTime, spriteBatch);
                
                ground.Draw(gameTime, spriteBatch);
                
                jumpjump.Draw(spriteBatch); 
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void StartNewGame()
        {
            onTitleScreen = false;
            SaveGame();
        }

        private void ResumeGame()
        {
            if (!hasSave) return;

            ApplySave();
            onTitleScreen = false;
        }

        private void ApplySave()
        {
            for (int i = 0; i < apples.Count && i < saveData.CollectedApples.Length; i++)
                apples[i].Collected = saveData.CollectedApples[i];

            applesLeft = apples.Count(c => !c.Collected);

            metLilJump = saveData.MetLilJump;
            lilJumpGone = saveData.MetLilJump;
            exitPortal.IsActive = saveData.MetLilJump;


            jumpjump.Position = new Vector2(saveData.PlayerX, saveData.PlayerY);

            currentLevel = saveData.Level;
            if (currentLevel == 2) hell.Enter();
            jumpjump.Position = new Vector2(saveData.PlayerX, saveData.PlayerY);
        }

        private void SaveGame()
        {
            new SaveData
            {
                Level = currentLevel,
                PlayerX = jumpjump.Position.X,
                PlayerY = jumpjump.Position.Y,
                CollectedApples = apples.Select(c => c.Collected).ToArray(),
                MetLilJump = metLilJump
            }.Save();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (!onTitleScreen) SaveGame();
            base.OnExiting(sender, args);
        }
        public static List<PlatformSprite> GetNearestColliders(Collisions.BoundingRectangle bounds, List<PlatformSprite> platforms)
        {
            List<PlatformSprite> result = new();
            foreach (var collider in platforms)
            {
                if (collider.Bounds.CollidesWith(bounds)) result.Add(collider);
            }
            return result;
        }
    }
}