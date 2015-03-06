using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;


namespace PongWar
{
    class Ball
    {
        Vector2 motion;
        Vector2 position;
        Rectangle bounds;
        bool collided;
        bool paddleCollided;
        static int templife1;
        static int templife2;

        const float ballStartSpeed = 5f;
        float ballSpeed;

        Texture2D texture;
        Rectangle screenBounds;
        Color tint;

        public Rectangle Bounds
        {
            get 
            {
                bounds.X = (int)position.X;
                bounds.Y = (int)position.Y;
                return bounds;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public bool PaddleCollided
        {
            get
            {
                return paddleCollided;
            }
            set
            {
                paddleCollided = value;
            }
        }

        public static int totallife1
        {

            get { return templife1; }

            set { templife1 = value; }
        }

        public static int totallife2
        {

            get { return templife2; }

            set { templife2 = value; }
        }

        public Ball(Texture2D texture, Rectangle screenBounds, Color tint)
        {
            bounds = new Rectangle(0, 0, texture.Width, texture.Height);
            this.texture = texture;
            this.screenBounds = screenBounds;
            this.tint = tint;
        }

        public void Update(int gameTime, Rectangle paddleLocation)
        {
            collided = false;
            position += motion * ballSpeed;
            ballSpeed += 0.003f;

            #region Check Wall Collision
#if DEBUG //Ball gets completley bounded in the screen when running in Debug (not Release) mode
            if (position.X < 0)
            {
                position.X = 0;
                motion.X *= -1;
            }
            if (position.X > screenBounds.Width)
            {
                position.X = screenBounds.Width;
                motion.X *= -1;
            }
#endif
            if (position.Y < 0)
            {
                position.Y = 0;
                motion.Y *= -1;
            }
            if (position.Y + texture.Height > screenBounds.Height)
            {
                position.Y = screenBounds.Height - texture.Height;
                motion.Y *= -1;
            }
            #endregion
        }

        public void SetInStartPosition(Rectangle paddleLocation, int paddlenumber)
        {
            if (paddlenumber == 1)
            {
                Random rand = new Random();

                motion = new Vector2(rand.Next(4, 5), -rand.Next(3, 4));
                motion.Normalize();

                ballSpeed = ballStartSpeed;

                position.Y = paddleLocation.Y - texture.Height;
                position.X = paddleLocation.X + (paddleLocation.Width - texture.Width) / 2;
            }
            else
            {
                Random rand = new Random();

                motion = new Vector2(-rand.Next(4, 6), rand.Next(3, 5));
                motion.Normalize();

                ballSpeed = ballStartSpeed;

                position.Y = paddleLocation.Y - texture.Height;
                position.X = paddleLocation.X + (paddleLocation.Width - texture.Width) / 2;
            }
        }

        /// <summary>
        /// Checks for paddle collision. When the ball hits the paddle set the position of the ball to where it hit the paddle, then reverse speed.
        /// This handles and collision error involved in frame skips and/or increased ball speed preventing the ball from "going through" the paddle.
        /// </summary>
        /// <param name="paddleLocation">Collision rectangle surrounding the paddle to detect ball rectangle intersection.</param>
        public void PaddleCollision(Rectangle paddleLocation)
        {
            Rectangle ballLocation = new Rectangle(
                (int)position.X,
                (int)position.Y,
                texture.Width,
                texture.Height);

            if (paddleLocation.Intersects(ballLocation))
            {
                paddleCollided = true;
                //determines where the ball hit (any) paddle and reflects it
                position.X = (paddleLocation.X > 512) ? position.X = paddleLocation.X - paddleLocation.Width : 
                    position.X = paddleLocation.X + paddleLocation.Width;
                motion.X *= -1;
            }
        }

        /// <summary>
        /// Used to detect if the ball hit a brick, if it does reverse the ball's X motion.
        /// </summary>
        /// <param name="brick">A brick in the wall of bricks.</param>
        public void Deflection(Brick brick)
        {
            if (!collided)
            {
                motion.X *= -1;
                collided = true;           
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, tint);
        }
    }
}
