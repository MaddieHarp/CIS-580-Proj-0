using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace CollisionExample
{
    public class CollisionExampleGame : Game
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
        private CoinSprite[] coins;
        private PlatformSprite[] platforms;
        private GroundSprite ground;
        private int coinsLeft = 7;
        private SoundEffect eatSound;
        private SlimeGhostSprite slimeGhost;
        private SpriteFont spriteFont;
        private bool onTitleScreen = true;
        private int currentLevel = 1;
        private hellLevel hell;

        public CollisionExampleGame()
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
            System.Random rand = new System.Random();

            int platformWidth = 80; 
            int platformHeight = 20;
            int minX = 50;
            int maxX = GraphicsDevice.Viewport.Width - platformWidth - 50;
            int minY = 50;
            int maxY = GraphicsDevice.Viewport.Height - platformHeight - 50;
            int paddingX = 50;
            int paddingY = 100;

            platforms = new PlatformSprite[7];
            coins = new CoinSprite[7];
            List<Rectangle> placedBounds = new List<Rectangle>();

            for (int i = 0; i < 7; i++)
            {
                int maxAttempts = 100;
                bool foundValidSpot = false;
                float finalX = 0;
                float finalY = 0;

                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    int randomX = rand.Next(minX, maxX);
                    int randomY = rand.Next(minY, maxY);
                    Rectangle testBox = new Rectangle(randomX, randomY, platformWidth, platformHeight);
                    testBox.Inflate(paddingX, paddingY);

                    bool overlaps = false;
                    foreach (Rectangle bounds in placedBounds)
                    {
                        if (testBox.Intersects(bounds))
                        {
                            overlaps = true;
                            break; 
                        }
                    }
                    if (!overlaps)
                    {
                        finalX = randomX;
                        finalY = randomY;
                        foundValidSpot = true;
                        placedBounds.Add(new Rectangle(randomX, randomY, platformWidth, platformHeight));
                        break;
                    }
                }

                if (!foundValidSpot)
                {
                    finalX = rand.Next(minX, maxX);
                    finalY = rand.Next(minY, maxY);
                }

                platforms[i] = new PlatformSprite(new Vector2(finalX, finalY));
                coins[i] = new CoinSprite(new Vector2(finalX + 100f, finalY - 30f));
            }
            
            slimeGhost = new SlimeGhostSprite();
            lilJump = new liljumpSprite(new Vector2(1700, 931));
            exitPortal = new portalSprite(new Vector2(1760, 840));
            ground = new GroundSprite(new Vector2(0, 1030));
            
            portalExplosion = new particleEffect(this, 5000);
            Components.Add(portalExplosion);

            hell = new hellLevel(slimeGhost);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var coin in coins) coin.LoadContent(Content);
            foreach (var platform in platforms) platform.LoadContent(Content);
            lilJump.LoadContent(Content);
            exitPortal.LoadContent(Content); 
            ground.LoadContent(Content);

            slimeGhost.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("arial");
            background = Content.Load<Texture2D>("background");
            eatSound = Content.Load<SoundEffect>("eating");
            backgroundMusic = Content.Load<Song>("backgroundMusic");
            titleScreenTexture = Content.Load<Texture2D>("titlescreen");


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
                if (Keyboard.GetState().IsKeyDown(Keys.E)) onTitleScreen = false; 
                base.Update(gameTime);
                return; 
            }
            if (currentLevel == 1)
            {
                foreach (var coin in coins) coin.Update(gameTime);
                
                slimeGhost.Update(gameTime, platforms.ToList(), ground);
                slimeGhost.Color = Color.White;

                foreach(var coin in coins)
                {
                    if (!coin.Collected && coin.Bounds.CollidesWith(slimeGhost.Bounds))
                    {
                        slimeGhost.Color = Color.Red;
                        coin.Collected = true;
                        coinsLeft--;
                        eatSound.Play();
                    }
                }

                if (coinsLeft == 0 && !metLilJump && !lilJumpGone && slimeGhost.Bounds.CollidesWith(lilJump.Bounds))
                {
                    metLilJump = true; 
                    lilJumpGone = true;      
                    exitPortal.IsActive = true; 

                    Vector2 portalCenter = new Vector2(exitPortal.Position.X + (exitPortal.Bounds.Width / 2), exitPortal.Position.Y + (exitPortal.Bounds.Height / 2));
                    portalExplosion.Explode(portalCenter);
                }

                if (exitPortal.IsActive && slimeGhost.Bounds.CollidesWith(exitPortal.Bounds))
                {
                    currentLevel = 2;
                    hell.Enter();
                }
            }
            else if (currentLevel == 2)
            {
                slimeGhost.Update(gameTime, new List<PlatformSprite>(), ground);
                hell.Update(gameTime);
                slimeGhost.Color = Color.Red;
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
            }
            else if (currentLevel == 1)
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);

                foreach (var coin in coins) coin.Draw(gameTime, spriteBatch);
                foreach (var platform in platforms) platform.Draw(gameTime, spriteBatch);
                
                ground.Draw(gameTime, spriteBatch);
                
                if (!lilJumpGone) lilJump.Draw(spriteBatch);
                exitPortal.Draw(spriteBatch);

                slimeGhost.Draw(spriteBatch);

                spriteBatch.DrawString(spriteFont, $"APPLES LEFT: {coinsLeft}", new Vector2(5,5), Color.Gold);
                spriteBatch.DrawString(spriteFont, $"TO EXIT GAME PRESS ESC", new Vector2(1500,2), Color.Gold);
                spriteBatch.DrawString(spriteFont, "Collect all apples and give it to your son", new Vector2(500,2), Color.Gold);
            }
            else if (currentLevel == 2)
            {
                hell.Draw(gameTime, spriteBatch);
                
                ground.Draw(gameTime, spriteBatch);
                
                slimeGhost.Draw(spriteBatch); 
            }

            spriteBatch.End();
            base.Draw(gameTime);
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