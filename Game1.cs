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
        bool player1Active, player2Active, player3Active, player4Active;

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
            player1Active = false;
            player2Active = false;
            player3Active = false;
            player4Active = false;
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
                player1Active = true;
            }
            if (kstate.IsKeyDown(Keys.W) || kstate.IsKeyDown(Keys.S))
            {
                player2Active = true;
            }
            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down))
            {
                player3Active = true;
            }
            if (kstate.IsKeyDown(Keys.A) || kstate.IsKeyDown(Keys.D))
            {
                player4Active = true;
            }
            if (kstate.IsKeyDown(Keys.Left)) paddlePosition1.X -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.Right)) paddlePosition1.X += paddleSpeed;
            if (kstate.IsKeyDown(Keys.W)) paddlePosition2.Y -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.S)) paddlePosition2.Y += paddleSpeed;
            if (kstate.IsKeyDown(Keys.Up)) paddlePosition3.Y -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.Down)) paddlePosition3.Y += paddleSpeed;
            if (kstate.IsKeyDown(Keys.A)) paddlePosition4.X -= paddleSpeed;
            if (kstate.IsKeyDown(Keys.D)) paddlePosition4.X += paddleSpeed;
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

            if (ballRect.Intersects(paddle1Rect)) // Onderste paddle
            {
                float relativeIntersectX = ballPosition.X - (paddlePosition1.X + paddleHeight / 2);
                float normalizedIntersection = relativeIntersectX / (paddleHeight / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);

                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                ballVelocity.X = ballSpeed * (float)Math.Sin(bounceAngle);
                ballVelocity.Y = -Math.Abs(ballSpeed * (float)Math.Cos(bounceAngle));

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            else if (ballRect.Intersects(paddle2Rect)) // Linker paddle
            {
                float relativeIntersectY = ballPosition.Y - (paddlePosition2.Y + paddleHeight / 2);
                float normalizedIntersection = relativeIntersectY / (paddleHeight / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);

                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                ballVelocity.X = Math.Abs(ballSpeed * (float)Math.Cos(bounceAngle));
                ballVelocity.Y = ballSpeed * (float)Math.Sin(bounceAngle);

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            else if (ballRect.Intersects(paddle3Rect)) // Rechter paddle
            {
                float relativeIntersectY = ballPosition.Y - (paddlePosition3.Y + paddleHeight / 2);
                float normalizedIntersection = relativeIntersectY / (paddleHeight / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);

                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                ballVelocity.X = -Math.Abs(ballSpeed * (float)Math.Cos(bounceAngle));
                ballVelocity.Y = ballSpeed * (float)Math.Sin(bounceAngle);

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            else if (ballRect.Intersects(paddle4Rect)) // Bovenste paddle
            {
                float relativeIntersectX = ballPosition.X - (paddlePosition4.X + paddleHeight / 2);
                float normalizedIntersection = relativeIntersectX / (paddleHeight / 2);
                float bounceAngle = normalizedIntersection * (float)(Math.PI / 4);

                if (ballSpeed * 1.05 <= 20)
                {
                    ballSpeed *= 1.05f;
                }
                else
                {
                    ballSpeed = 20;
                }

                ballVelocity.X = ballSpeed * (float)Math.Sin(bounceAngle);
                ballVelocity.Y = Math.Abs(ballSpeed * (float)Math.Cos(bounceAngle));

                ballVelocity = Vector2.Normalize(ballVelocity) * ballSpeed;
            }
            if (!player1Active)
            {
                if (paddlePosition1.X > ballPosition.X)
                {
                    paddlePosition1.X -= paddleSpeed / 2;
                }
                else
                {
                    paddlePosition1.X += paddleSpeed / 2;
                }
            }
            if (!player2Active)
            {
                if (paddlePosition2.Y > ballPosition.Y)
                {
                    paddlePosition2.Y -= paddleSpeed / 2;
                }
                else
                {
                    paddlePosition2.Y += paddleSpeed / 2;
                }
            }
            if (!player3Active)
            {
                if (paddlePosition3.Y > ballPosition.Y)
                {
                    paddlePosition3.Y -= paddleSpeed / 2;
                }
                else
                {
                    paddlePosition3.Y += paddleSpeed / 2;
                }
            }
            if (!player4Active)
            {
                if (paddlePosition4.X > ballPosition.X)
                {
                    paddlePosition4.X -= paddleSpeed / 2;
                }
                else
                {
                    paddlePosition4.X += paddleSpeed / 2;
                }
            }
            // Wanneer er gescoord is.
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
            if (scorePlayer1 == 0)
            {
                gameOver = true;
                loser = "Speler 1";
            }
            else if (scorePlayer2 == 0)
            {
                gameOver = true;
                loser = "Speler 2";
            }
            else if (scorePlayer3 == 0)
            {
                gameOver = true;
                loser = "Speler 3";
            }
            else if (scorePlayer4 == 0)
            {
                gameOver = true;
                loser = "Speler 4";
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
            player1Active = false;
            player2Active = false;
            player3Active = false;
            player4Active = false;
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
                _spriteBatch.DrawString(scoreFont, $"Klik op Enter om te beginnen", new Vector2(screenWidth / 2 - 150, screenHeight / 2 - 100), Color.White);
            }
            if (gameOver)
            {
                _spriteBatch.DrawString(scoreFont, $"{loser} heeft verloren. klik op Enter om opnieuw te spelen.", new Vector2(screenWidth / 2 - 150, screenHeight / 2 - 100), Color.White);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}