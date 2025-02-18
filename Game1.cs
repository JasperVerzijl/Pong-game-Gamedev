using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Pong_game_Gamedev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D paddleTexture, ballTexture;
        Vector2 paddlePosition1, paddlePosition2, paddlePosition3, paddlePosition4, ballPosition, ballVelocity;
        float ballSpeed, paddleSpeed;
        int screenWidth, screenHeight;
        const int paddleWidth = 20, paddleHeight = 200;
        const int ballWidth = 20, ballHeight = 20;
        int scorePlayer3, scorePlayer2, scorePlayer1, scorePlayer4;
        SpriteFont scoreFont;
        string loser;
        bool gameOver;
        bool gameStart;
        bool playerActive;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1020;
            _graphics.PreferredBackBufferHeight = 1020;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;
            // paddle initializen
            paddleTexture = new Texture2D(GraphicsDevice, 1, 1);
            paddleTexture.SetData(new Color[] { Color.Orange });
            // bal initializen
            ballTexture = new Texture2D(GraphicsDevice, 1, 1);
            ballTexture.SetData(new Color[] { Color.Blue });
            // positie van paddle definen ( X , Y )
            paddlePosition1 = new Vector2(screenWidth / 2 - paddleWidth / 2, screenHeight - 40);
            paddlePosition2 = new Vector2(20, screenHeight / 2 - paddleHeight / 2);
            paddlePosition3 = new Vector2(screenWidth - 40, screenHeight / 2 - paddleHeight / 2);
            paddlePosition4 = new Vector2(screenWidth / 2 - paddleWidth / 2, 20);
            // bal positie
            ballPosition = new Vector2(screenWidth / 2 - ballWidth / 2, screenHeight / 2 - ballHeight / 2);
            // bal snelheid en bewegingsrichting, hele tijd positie += velocity om nieuwe positie te bepalen.
            ballSpeed = 10;
            Random random = new Random();
            double angle = random.NextDouble() * Math.PI * 2; // Random angle between 0 and 2*PI

            float randomXDirection = (float)Math.Cos(angle);
            float randomYDirection = (float)Math.Sin(angle);

            ballVelocity = new Vector2(randomXDirection * ballSpeed, randomYDirection * ballSpeed);

            // Aantal punten per speler.
            scorePlayer3 = 10;
            scorePlayer2 = 10;
            scorePlayer1 = 10;
            scorePlayer4 = 10;
            gameOver = false;
            gameStart = true;
            playerActive = false;
            loser = string.Empty;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // SpriteFont is het bestand waarin je de stijl bepaald van de scoreFont.
            // Hiervoor moet je wel een bestand aanmaken in de content map.
            scoreFont = Content.Load<SpriteFont>("File");
        }

        private static Vector2 Bounce(Vector2 incident, Vector2 normal)
        {
            return incident - 2 * (Vector2.Dot(incident, normal) * normal);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var kstate = Keyboard.GetState();
            if (gameStart)
            {
                if (kstate.IsKeyDown(Keys.Enter))
                {
                    gameStart = false;
                }
                else
                {
                    return;
                }
            }
            if (gameOver)
            {
                if (kstate.IsKeyDown(Keys.Enter))
                {
                    ResetGame();
                }
                return;
            }
            paddleSpeed = 15f;
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right))
            {
                playerActive = true;
            }

            if (kstate.IsKeyDown(Keys.W)) paddlePosition2.Y -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.S)) paddlePosition2.Y += paddleSpeed;
            if (kstate.IsKeyDown(Keys.Up)) paddlePosition3.Y -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.Down)) paddlePosition3.Y += paddleSpeed;
            if (kstate.IsKeyDown(Keys.A)) paddlePosition4.X -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.D)) paddlePosition4.X += paddleSpeed;
            if (kstate.IsKeyDown(Keys.Left)) paddlePosition1.X -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.Right)) paddlePosition1.X += paddleSpeed;
            // Keep paddles within screen bounds
            paddlePosition1.X = Math.Clamp(paddlePosition1.X, 20, screenWidth - paddleHeight - paddleWidth);
            paddlePosition2.Y = Math.Clamp(paddlePosition2.Y, 20, screenHeight - paddleHeight - paddleWidth);
            paddlePosition3.Y = Math.Clamp(paddlePosition3.Y, 20, screenHeight - paddleHeight - paddleWidth);
            paddlePosition4.X = Math.Clamp(paddlePosition4.X, 20, screenWidth - paddleHeight - paddleWidth);
            // Move the ball
            ballPosition += ballVelocity;
            // Ball collision with paddles
            Rectangle ballRect = new Rectangle((int)ballPosition.X, (int)ballPosition.Y, ballWidth, ballHeight);
            Rectangle paddle1Rect = new Rectangle((int)paddlePosition1.X, (int)paddlePosition1.Y, paddleHeight, paddleWidth);
            Rectangle paddle2Rect = new Rectangle((int)paddlePosition2.X, (int)paddlePosition2.Y, paddleWidth, paddleHeight);
            Rectangle paddle3Rect = new Rectangle((int)paddlePosition3.X, (int)paddlePosition3.Y, paddleWidth, paddleHeight);
            Rectangle paddle4Rect = new Rectangle((int)paddlePosition4.X, (int)paddlePosition4.Y, paddleHeight, paddleWidth);

            if (ballRect.Intersects(paddle2Rect))
            {
                float relativeIntersectY = (paddlePosition2.Y + paddleHeight / 2) - ballPosition.Y;
                float normalizedIntersection = relativeIntersectY / (paddleHeight / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4); // Max bounce angle = 45 degrees
                Vector2 normal = new Vector2(1, 0); // Left paddle normal
                //ballVelocity = Bounce(ballVelocity, normal);
                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                // Adjust bounce angle
                ballVelocity.Y = ballSpeed * (float)Math.Sin(bounceAngle);
                ballVelocity.X = Math.Abs(ballVelocity.X); // Ensure it moves to the right

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            else if (ballRect.Intersects(paddle3Rect))
            {
                float relativeIntersectY = (paddlePosition3.Y + paddleHeight / 2) - ballPosition.Y;
                float normalizedIntersection = relativeIntersectY / (paddleHeight / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);
                Vector2 normal = new Vector2(-1, 0); // Right paddle normal
                //ballVelocity = Bounce(ballVelocity, normal);
                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                // Adjust bounce angle
                ballVelocity.Y = ballSpeed * (float)Math.Sin(bounceAngle);
                ballVelocity.X = -Math.Abs(ballVelocity.X); // Ensure it moves to the left

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            else if (ballRect.Intersects(paddle4Rect))
            {
                float relativeIntersectX = (paddlePosition4.X + paddleWidth / 2) - ballPosition.X;
                float normalizedIntersection = relativeIntersectX / (paddleWidth / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);
                Vector2 normal = new Vector2(0, 1); // Top paddle normal
                //ballVelocity = Bounce(ballVelocity, normal);
                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                // Adjust bounce angle
                ballVelocity.X = ballSpeed * (float)Math.Sin(bounceAngle);
                ballVelocity.Y = Math.Abs(ballVelocity.Y); // Ensure it moves downward

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            else if (ballRect.Intersects(paddle1Rect))
            {
                float relativeIntersectX = (paddlePosition1.X + paddleWidth / 2) - ballPosition.X;
                float normalizedIntersection = relativeIntersectX / (paddleWidth / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);
                Vector2 normal = new Vector2(0, -1); // Bottom paddle normal
                //ballVelocity = Bounce(ballVelocity, normal);
                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                // Adjust bounce angle
                ballVelocity.X = ballSpeed * (float)Math.Sin(bounceAngle);
                ballVelocity.Y = -Math.Abs(ballVelocity.Y); // Ensure it moves upward

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            if (!playerActive)
            {
                if (paddlePosition1.X > ballVelocity.X)
                {
                    paddlePosition1.X -= ballVelocity.X;
                }
                else
                {
                    paddlePosition1.X += ballVelocity.X;
                }
            }
            //if (paddlePosition2.Y > ballVelocity.Y)
            //{
            //    paddlePosition2.Y -= ballVelocity.Y;
            //}
            //else
            //{
            //    paddlePosition2.Y += ballVelocity.Y;
            //}
            //if (paddlePosition3.Y > ballVelocity.Y)
            //{
            //    paddlePosition3.Y -= ballVelocity.Y;
            //}
            //else
            //{
            //    paddlePosition3.Y += ballVelocity.Y;
            //}
            //if (paddlePosition4.X > ballVelocity.X)
            //{
            //    paddlePosition4.X -= ballVelocity.X;
            //}
            //else
            //{
            //    paddlePosition4.X += ballVelocity.X;
            //}

            // Check for scoring
            if (ballPosition.X < 0)
            {
                scorePlayer2--;
                ResetBall();
            }
            else if (ballPosition.X > screenWidth)
            {
                scorePlayer3--;
                ResetBall();
            }
            else if (ballPosition.Y < 0)
            {
                scorePlayer4--;
                ResetBall();
            }
            else if (ballPosition.Y > screenHeight)
            {
                scorePlayer1--;
                ResetBall();
            }
            // Kijken of er een speler game over is
            if (scorePlayer3 == 0)
            {
                gameOver = true;
                loser = "Player 3";
            }
            else if (scorePlayer2 == 0)
            {
                gameOver = true;
                loser = "Player 2";
            }
            else if (scorePlayer1 == 0)
            {
                gameOver = true;
                loser = "Player 1";
            }
            else if (scorePlayer4 == 0)
            {
                gameOver = true;
                loser = "Player 4";
            }
            base.Update(gameTime);
        }

        private void ResetBall()
        {
            ballPosition = new Vector2(screenWidth / 2 - ballWidth / 2, screenHeight / 2 - ballHeight / 2);
            ballSpeed = 10;
            Random random = new Random();
            double angle = random.NextDouble() * Math.PI * 2; // random getal tussen 0 en 2*PI (0 en 360 graden)

            float randomXDirection = (float)Math.Cos(angle);
            float randomYDirection = (float)Math.Sin(angle);

            ballVelocity = new Vector2(randomXDirection * ballSpeed, randomYDirection * ballSpeed);
        }

        private void ResetGame()

        {
            scorePlayer3 = 10;
            scorePlayer2 = 10;
            scorePlayer1 = 10;
            scorePlayer4 = 10;
            gameOver = false;
            playerActive = false;
            loser = string.Empty;
            ResetBall();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.Draw(paddleTexture, new Rectangle((int)paddlePosition1.X, (int)paddlePosition1.Y, paddleHeight, paddleWidth), Color.White);
            _spriteBatch.Draw(paddleTexture, new Rectangle((int)paddlePosition2.X, (int)paddlePosition2.Y, paddleWidth, paddleHeight), Color.White);
            _spriteBatch.Draw(paddleTexture, new Rectangle((int)paddlePosition3.X, (int)paddlePosition3.Y, paddleWidth, paddleHeight), Color.White);
            _spriteBatch.Draw(paddleTexture, new Rectangle((int)paddlePosition4.X, (int)paddlePosition4.Y, paddleHeight, paddleWidth), Color.White);


            _spriteBatch.Draw(ballTexture, ballPosition, new Rectangle((int)ballPosition.X, (int)ballPosition.Y, ballWidth, ballHeight), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.05f);
            _spriteBatch.DrawString(scoreFont, $"Speler 1: {scorePlayer1}", new Vector2(screenWidth / 2, screenHeight - 100), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0.1f);
            _spriteBatch.DrawString(scoreFont, $"Speler 2: {scorePlayer2}", new Vector2(50, screenHeight / 2), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0.1f);
            _spriteBatch.DrawString(scoreFont, $"Speler 3: {scorePlayer3}", new Vector2(screenWidth - 290, screenHeight / 2), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0.1f);
            _spriteBatch.DrawString(scoreFont, $"Speler 4: {scorePlayer4}", new Vector2(screenWidth / 2, 50), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0.1f);
            // balsnelheid laten zien
            _spriteBatch.DrawString(scoreFont, $"speed: {ballSpeed}", new Vector2(screenWidth / 2, screenHeight / 2 - 50), Color.White);

            if (gameStart)
            {
                _spriteBatch.DrawString(scoreFont, $"Press Enter to start the game", new Vector2(screenWidth / 2 - 150, screenHeight / 2 - 100), Color.White);
            }
            if (gameOver)
            {
                _spriteBatch.DrawString(scoreFont, $"{loser} has lost the game. Press Enter to restart", new Vector2(screenWidth / 2 - 150, screenHeight / 2 - 100), Color.White);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}