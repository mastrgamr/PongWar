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


namespace PongWar
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        SpriteBatch spriteBatch;
        SpriteFont font;

        enum GameState
        {
            INTRO,
            PLAYING,
            PAUSED,
            GAMEOVER
        }
        GameState currentState;

        Animator scoreAnimator;
        Animator losingAnimator1;
        Animator losingAnimator2;
        Ball ball1;
        Ball ball2;
        Paddle paddle1;
        Paddle paddle2;
        Rectangle screenRectangle;
        float RotationAngle;

        TimeSpan tSpan = TimeSpan.FromSeconds(1);
    
        const int BRICKS_WIDE = 6;
        const int BRICKS_HIGH = 9;
        int totalbricks;
        int lives = 4;
        int lifeLeft1 = 4;
        int lifeLeft2 = 4;

        Texture2D brickImage;
        Texture2D texturebackground;
        Texture2D introPane;
        Texture2D caution;
        Texture2D gameResults;
        Rectangle gameResultRect;
        Texture2D[] lifeHeartsP1;
        Texture2D[] lifeHeartsP2;
        
        Brick[,] bricks;
        int screenWidth;
        int screenHeight;

        SoundEffect paddleHit;
        SoundEffect brickPop;
        SoundEffect brickPop2;
        SoundEffect brickPop3;
        SoundEffect resetBall;
        Song deadlytechno;
        bool songstart = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //720p BIZZNITCH
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 720;

            screenRectangle = new Rectangle(
                0, 0,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight);
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Pong War v1.0";

            scoreAnimator = new Animator();
            scoreAnimator.AnimationProperty = Animation.FADEUP;
            losingAnimator1 = new Animator();
            losingAnimator1.AnimationProperty = Animation.PULSEFADE;
            losingAnimator2 = new Animator();
            losingAnimator2.AnimationProperty = Animation.PULSEFADE;
            Ball.totallife1 = Ball.totallife2 = lives;
            lifeHeartsP1 = new Texture2D[lives];
            lifeHeartsP2 = new Texture2D[lives];
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load media
            deadlytechno = Content.Load<Song>("Sounds//deadlytechno");
            MediaPlayer.IsRepeating = true;
            paddleHit = Content.Load<SoundEffect>("Sounds//paddlePop");
            brickPop = Content.Load<SoundEffect>("Sounds//brickPop");
            brickPop2 = Content.Load<SoundEffect>("Sounds//brickPop2");
            brickPop3 = Content.Load<SoundEffect>("Sounds//brickPop3");
            resetBall = Content.Load<SoundEffect>("Sounds//resetBall");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            
            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;
            texturebackground = Content.Load<Texture2D>("Images//background");
            
            introPane = Content.Load<Texture2D>("Images//PongWarIntro");
            caution = Content.Load<Texture2D>("Images//caution");

            for (int i = 0; i < lifeHeartsP1.Length; i++)
            {
                lifeHeartsP1[i] = Content.Load<Texture2D>("Images//lifeHeart");
                lifeHeartsP2[i] = Content.Load<Texture2D>("Images//lifeHeart");
            }

            Texture2D tempTexture = Content.Load<Texture2D>("Images//paddle");
            
            paddle1 = new Paddle(tempTexture, screenRectangle, 15);
            paddle2 = new Paddle(tempTexture, screenRectangle, (screenRectangle.Width - tempTexture.Width) - 15);

            tempTexture = Content.Load<Texture2D>("Images//ball1");
            ball1 = new Ball(tempTexture, screenRectangle, Color.White);
            ball2 = new Ball(tempTexture, screenRectangle, Color.White);

            brickImage = Content.Load<Texture2D>("Images//brick");

            gameResults = Content.Load<Texture2D>("Images//gameResults");

            font = Content.Load<SpriteFont>("gameFont");

            StartGame();
            currentState = GameState.INTRO;
        }

        private void StartGame()
        {
            paddle1.SetInStartPosition();
            paddle2.SetInStartPosition();
            ball1.SetInStartPosition(paddle1.GetBounds(), 1);
            ball2.SetInStartPosition(paddle2.GetBounds(), 2);
            
            bricks = new Brick[BRICKS_WIDE, BRICKS_HIGH];

            for (int x = 0; x < BRICKS_WIDE; x++)
            {
                Color tint = Color.White;

                switch (x)
                {
                    case 0:
                    case 5:
                        tint = Color.MintCream;
                        break;
                    case 1:
                    case 4:
                        tint = Color.Aquamarine;
                        break;
                    case 2:
                    case 3:
                        tint = Color.Red;
                        break;
                }

                for (int y = 0; y < BRICKS_HIGH; y++)
                {
                    bricks[x, y] = new Brick(
                        brickImage,
                        new Rectangle(
                            x * brickImage.Width + (screenRectangle.Width / 2 - ((BRICKS_WIDE * brickImage.Width) / 2)), //center ALL bricks in the middle of screen
                            y * brickImage.Height + (screenRectangle.Height / 2 - ((BRICKS_HIGH * brickImage.Height) / 2)), //and vertically
                            brickImage.Width,
                            brickImage.Height),
                        tint);
                    totalbricks += 1; // Variable to count the number of total bricks
                }
            }
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
            // Load Song
            if (!songstart)
            {
                MediaPlayer.Volume = 0.4f; // Background song volume control
                //MediaPlayer.Play(deadlytechno);
                songstart = true;
            }

            if (currentState == GameState.GAMEOVER)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Y))
                {
                    currentState = GameState.PLAYING;
                    StartGame();
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.N))
                {
                    this.Exit();
                }
            }

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Allows full screen 
            if (Keyboard.GetState().IsKeyDown(Keys.F))
                graphics.ToggleFullScreen();

            if ((Mouse.GetState().RightButton == ButtonState.Pressed) && currentState == GameState.INTRO)
            {
                currentState = GameState.PLAYING;
            }

            // Allows game to pause and exit with Escape key
            if (Keyboard.GetState().IsKeyDown(Keys.P)) 
            {
                currentState = (currentState == GameState.PLAYING) ? GameState.PAUSED : GameState.PLAYING;
            }

            if (currentState != GameState.PLAYING)
            {
                return;
            }
            
            paddle1.Update(1);
            paddle2.Update(2);
            ball1.Update(gameTime.ElapsedGameTime.Seconds, paddle1.GetBounds());
            ball2.Update(gameTime.ElapsedGameTime.Seconds, paddle2.GetBounds());

            for (int i = 0; i < BRICKS_WIDE; i++)
            {
                for (int j = 0; j < BRICKS_HIGH; j++)
                {
                    if (bricks[i,j].BrickCollided)
                    {
                        bricks[i, j].IsAlive = false;
                        totalbricks -= 1;
                        scoreAnimator = new Animator();
                        scoreAnimator.AnimationProperty = Animation.FADEUP;
                        scoreAnimator.Position = new Vector2(bricks[i,j].Location.X, bricks[i,j].Location.Y);

                        if (bricks[i, j].Tint == Color.Red)
                        {
                            brickPop3.Play();
                            scoreAnimator.Animating = true;
                        }
                        else if (bricks[i, j].Tint == Color.Aquamarine)
                        {
                            brickPop2.Play();
                            scoreAnimator.Animating = true;
                        }
                        else if (bricks[i, j].Tint == Color.MintCream)
                        {
                            brickPop.Play();
                            scoreAnimator.Animating = true;
                        }
                        bricks[i, j].BrickCollided = false;
                    }
                    bricks[i, j].CheckCollision(ball1, 1);
                    bricks[i, j].CheckCollision(ball2, 2);
                }
            }

            if (Ball.totallife2 <= 0)
            {
                gameResultRect = new Rectangle(373, 2, 355, 49);
                currentState = GameState.GAMEOVER;
            }
            else if (Ball.totallife1 <= 0)
            {
                gameResultRect = new Rectangle(2, 2, 369, 52);
                currentState = GameState.GAMEOVER;
            }

            scoreAnimator.Update(gameTime);
            losingAnimator1.Update(gameTime);
            losingAnimator2.Update(gameTime);

            if (totalbricks == 0) // If there are no more bricks game is over
            {
                currentState = GameState.GAMEOVER; // Set variable for game over
                if (Brick.totalscore1 > Brick.totalscore2)
                {
                    gameResultRect = new Rectangle(373, 2, 355, 49); // player 1 wins
                }
                else if (Brick.totalscore2 > Brick.totalscore1)
                {
                    gameResultRect = new Rectangle(2, 2, 369, 52); //player 2 wins
                }
                else
                {
                    gameResultRect = new Rectangle(730, 2, 141, 46);
                }
            }

            ball1.PaddleCollision(paddle1.GetBounds());
            ball2.PaddleCollision(paddle2.GetBounds());
            if (ball1.PaddleCollided)
            {
                ball1.BallAngleUpdate(paddle1.GetBounds());
                paddleHit.Play();
                ball1.PaddleCollided = false;
            }
            else if (ball2.PaddleCollided)
            {
                ball2.BallAngleUpdate(paddle2.GetBounds());
                paddleHit.Play();
                ball2.PaddleCollided = false;
            }

            // Ball Rotation 
            RotationAngle += (float)gameTime.ElapsedGameTime.TotalSeconds * 6;

            #region Reset Ball
            if (ball1.Position.X < 0 || ball1.Position.X > Window.ClientBounds.Width)
            {
                tSpan -= gameTime.ElapsedGameTime;
                if (tSpan < TimeSpan.Zero)
                {
                    resetBall.Play();
                    lifeLeft1 -= 1;
                    ball1.SetInStartPosition(paddle1.GetBounds(), 1);
                    lifeHeartsP1[Ball.totallife1 - 1] = Content.Load<Texture2D>("Images//empty");
                    Ball.totallife1 -= 1;
                    tSpan = TimeSpan.FromSeconds(1);
                }
            }
            if (ball2.Position.X < 0 || ball2.Position.X > Window.ClientBounds.Width)
            {
                tSpan -= gameTime.ElapsedGameTime;
                if (tSpan < TimeSpan.Zero)
                {
                    resetBall.Play();
                    lifeLeft2 = 1;
                    ball2.SetInStartPosition(paddle2.GetBounds(), 2);
                    lifeHeartsP2[Ball.totallife2 - 1] = Content.Load<Texture2D>("Images//empty");
                    Ball.totallife2 -= 1;
                    tSpan = TimeSpan.FromSeconds(1);
                }
            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.MidnightBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(texturebackground, screenRectangle, Color.White);

            if (currentState == GameState.INTRO)
            {
                spriteBatch.Draw(introPane, new Vector2(screenWidth / 2 - introPane.Width / 2, screenHeight / 2 - introPane.Height / 2), Color.White);
            }

            if (currentState == GameState.PLAYING || currentState == GameState.PAUSED)
            {
                foreach (Brick brick in bricks)
                {
                    brick.Draw(spriteBatch);
                }

                scoreAnimator.Draw(spriteBatch, font, "10");
                paddle1.Draw(spriteBatch);
                paddle2.Draw(spriteBatch);
                ball1.Draw(spriteBatch, RotationAngle);
                ball2.Draw(spriteBatch, RotationAngle);

                spriteBatch.DrawString(font, "Player 1: " + Brick.totalscore1, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 5), Color.White);
                spriteBatch.DrawString(font, "Player 2: " + Brick.totalscore2, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 630, GraphicsDevice.Viewport.TitleSafeArea.Y + 5), Color.White);

                float anotherLife = GraphicsDevice.Viewport.TitleSafeArea.X;
                for (int i = 0; i < lives; i++)
                {
                    spriteBatch.Draw(lifeHeartsP1[i], new Vector2(anotherLife, screenHeight - 50),
                        null, Color.White, 0f, Vector2.Zero, .15f, SpriteEffects.None, 0);
                    spriteBatch.Draw(lifeHeartsP2[i], new Vector2(anotherLife + 630, screenHeight - 50),
                        null, Color.White, 0f, Vector2.Zero, .15f, SpriteEffects.None, 0);

                    anotherLife += 25;
                }

                if (lifeLeft1 <= 3)
                {
                    losingAnimator1.Draw(spriteBatch, new Vector2(screenWidth / 4 - 50, screenHeight / 2 - 50), caution);
                }
                if (lifeLeft2 <= 3)
                {
                    losingAnimator2.Draw(spriteBatch, new Vector2((screenWidth - 100) - 100, screenHeight / 2 - 50), caution);
                }
            }

            if (currentState == GameState.GAMEOVER) // Check if the game is over and display final message
            {
                spriteBatch.Draw(gameResults, new Vector2((screenWidth / 2 - gameResultRect.Width / 2), (screenHeight / 2 - gameResultRect.Height / 2)), gameResultRect, Color.White);
                spriteBatch.DrawString(font, "Play Again? (Y/N)", new Vector2((screenWidth / 2 - gameResultRect.Width / 2), (screenHeight / 2 - gameResultRect.Height / 2) + 100), Color.White);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
