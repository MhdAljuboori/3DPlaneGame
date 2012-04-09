using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace _3D_PlaneGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ModelManager modelManager;

        public enum GameState { START, PLAY, LEVEL_CHANGE, END }
        GameState currentGameState = GameState.START;

        //variable to shoot
        float shotSpeed = 10;
        int shotDelay = 300;
        int shotCountdown = 0;

        //texture for crosshair
        Texture2D crosshairTexture;

        SplashScreen splashScreen;
        int score = 0;

        SpriteFont spriteFont;

        public Effect effect { get; protected set; }

        public Random rnd { get; protected set; }

        public Camera camera { get; protected set; }

        public void ChangeGameState(GameState state, int level)
        {
            currentGameState = state;
            switch (currentGameState)
            {
                case GameState.LEVEL_CHANGE:
                    splashScreen.SetData("Level " + (level + 1),GameState.LEVEL_CHANGE);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    break;
                case GameState.PLAY:
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    splashScreen.Enabled = false;
                    splashScreen.Visible = false;
                    break;
                case GameState.END:
                    splashScreen.SetData("Game Over.\nLevel: " + (level + 1) +
                        "\nScore: " + score, GameState.END);
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    splashScreen.Enabled = true;
                    splashScreen.Visible = true;
                    break;
            }
        }

        public void AddPoints(int points)
        {
            score += points;
        }

        protected void FireShots(GameTime gameTime)
        {
            if (shotCountdown <= 0)
            {
                // Did player press space bar or left mouse button?
                if (Keyboard.GetState().IsKeyDown(Keys.Space) ||
                    Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    // Add a shot to the model manager
                    modelManager.AddShot(camera.cameraPosition + new Vector3(0, -5, 0)
                        , camera.GetCameraDirection * shotSpeed);
                    
                    // Reset the shot countdown
                    shotCountdown = shotDelay;
                }
            }
            else
                shotCountdown -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            rnd = new Random();

            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 1024;
            #if !DEBUG
                graphics.IsFullScreen = true;
            #endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            camera = new Camera(this, new Vector3(0, 0, 300),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);

            modelManager.Enabled = false;
            modelManager.Visible = false;
            
            // Splash screen component
            splashScreen = new SplashScreen(this);
            Components.Add(splashScreen);

            splashScreen.SetData("Welcome to Space Defender!\n       Mohammed Aljobory"
                , currentGameState);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            crosshairTexture = Content.Load<Texture2D>(@"Textures\Crosshair");
            spriteFont = Content.Load<SpriteFont>(@"Fonts\Tahoma");
            effect = Content.Load<Effect>(@"Effects\Red");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            
            // Only check for shots if you're in the play game state
            if (currentGameState == GameState.PLAY)
            {
                // See if the player has fired a shot
                FireShots(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            // TODO: Add your drawing code here
            
            // Only draw crosshair if in play game state
            if (currentGameState == GameState.PLAY)
            {
                //Draw Crosshair
                spriteBatch.Begin();
                spriteBatch.Draw(crosshairTexture, new Vector2((Window.ClientBounds.Width / 2)
                    - (crosshairTexture.Width / 2), (Window.ClientBounds.Height / 2)
                    - (crosshairTexture.Height / 2)), Color.White);
                
                // Draw the current score
                string scoreText = "Score: " + score;
                spriteBatch.DrawString(spriteFont, scoreText, new Vector2(10, 10), Color.Red);
                
                // Let the player know how many misses he has left
                spriteBatch.DrawString(spriteFont, "Misses Left: " + modelManager.missesLeft,
                new Vector2(10, spriteFont.MeasureString(scoreText).Y + 20), Color.Red);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
