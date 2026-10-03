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
        
        private CoinSprite[] coins;
        private bool gameWon = false;
        private bool onTitleScreen = true;

        private PlatformSprite[] platforms;
        private SlimeGhostSprite slimeGhost;
        private SpriteFont spriteFont;
        private GroundSprite ground;
        private int coinsLeft = 7;

        private SoundEffect eatSound;

        /// <summary>
        /// A game demonstrating collision detection
        /// </summary>
        public CollisionExampleGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
        }


        /// <summary>
        /// Initializes the game 
        /// </summary>
        protected override void Initialize()
        {
        // TODO: Add your initialization logic here
            System.Random rand = new System.Random();

            int platformWidth = 80; 
            int platformHeight = 20;
            int minX = 50;
            int maxX = GraphicsDevice.Viewport.Width - platformWidth - 50;

            int minY = 50;
            int maxY = GraphicsDevice.Viewport.Height - platformHeight - 50;
            int paddingX = 50;
            int paddingY = 100;

            // 3. Initialize arrays
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
                    // 1. Generate random position
                    int randomX = rand.Next(minX, maxX);
                    int randomY = rand.Next(minY, maxY);

                    // 2. Create the test rectangle and inflate it by 50px
                    Rectangle testBox = new Rectangle(randomX, randomY, platformWidth, platformHeight);
                    testBox.Inflate(paddingX, paddingY);

                    // 3. Check if this padded box hits any already placed platforms
                    bool overlaps = false;
                    foreach (Rectangle bounds in placedBounds)
                    {
                        if (testBox.Intersects(bounds))
                        {
                            overlaps = true;
                            break; // Overlaps, try again
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

            ground = new GroundSprite(new Vector2(300,1250));

            base.Initialize();
        }

        /// <summary>
        /// Loads content for the game
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            foreach (var coin in coins) coin.LoadContent(Content);
            foreach (var platform in platforms) platform.LoadContent(Content);
            slimeGhost.LoadContent(Content);
            spriteFont = Content.Load<SpriteFont>("arial");
            background = Content.Load<Texture2D>("background");
            eatSound = Content.Load<SoundEffect>("eating");
            backgroundMusic = Content.Load<Song>("backgroundMusic");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .1f;
            ground.LoadContent(Content);

            titleScreenTexture = Content.Load<Texture2D>("titlescreen");
        }

        /// <summary>
        /// Updates the game world
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            foreach (var coin in coins)
            {
                coin.Update(gameTime);
            }
            
            if (onTitleScreen)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    onTitleScreen = false; // Start the game!
                }
                
                base.Update(gameTime);
                return; // Prevent gameplay updates (like movement or collisions) while on the title screen
            }

            // TODO: Add your update logic here
            slimeGhost.Update(gameTime, platforms.ToList(), ground);

            //detect and process collisions
            slimeGhost.Color = Color.White;
            foreach(var coin in coins)
            {
                if (!coin.Collected && coin.Bounds.CollidesWith(slimeGhost.Bounds))
                {
                    slimeGhost.Color = Color.Red;
                    coin.Collected = true;
                    coinsLeft--;
                    eatSound.Play();
                    if(coinsLeft == 0)
                    {
                        gameWon = true;
                    }
                }
            }

            
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game world
        /// </summary>
        /// <param name="gameTime">The game time</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            spriteBatch.Begin();

            if (onTitleScreen)
            {
                spriteBatch.Draw(titleScreenTexture, new Rectangle(0, 0, 1920, 1080), Color.White);
            }
            else if (gameWon)
            {
                spriteBatch.DrawString(spriteFont, "YOU WON THE GAME! PRESS ESC TO EXIT",
                    new Vector2(40, 500), Color.Gold, 0f, Vector2.Zero, 2.7f, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), Color.White);

                // TODO: Add your drawing code here
                foreach (var coin in coins)
                {
                    coin.Draw(gameTime, spriteBatch);
                    
                } 
                foreach (var platform in platforms)
                {
                    platform.Draw(gameTime, spriteBatch);
                    
                } 
                slimeGhost.Draw(spriteBatch);
                ground.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(spriteFont, $"APPLES LEFT: {coinsLeft}", new Vector2(5,5), Color.Gold);
                spriteBatch.DrawString(spriteFont, $"TO EXIT GAME PRESS ESC", new Vector2(1500,2), Color.Gold);
                
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static List<PlatformSprite> GetNearestColliders(Collisions.BoundingRectangle bounds, List<PlatformSprite> platforms)
        {
            List<PlatformSprite> result = new();

            foreach (var collider in platforms)
            {
                if (collider.Bounds.CollidesWith(bounds))
                    result.Add(collider);
            }

            return result;
        }
    }
}
