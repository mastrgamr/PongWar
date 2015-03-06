using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PongWar
{
    class Paddle
    {
        Vector2 position;
        Vector2 motion;
        float paddleSpeed = 8f;

        KeyboardState keyboardState;

        Texture2D texture;
        Rectangle screenBounds;
        int startposition;
        
        public Paddle(Texture2D texture, Rectangle screenBounds, int startposition)
        {
            this.texture = texture;
            this.screenBounds = screenBounds;
            this.startposition = startposition;
            SetInStartPosition();
               
        }
        
        public void Update(int paddlenumber)
        {
            motion = Vector2.Zero;
           
            if (paddlenumber == 1)
            {    
            keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.W))
                    motion.Y = -1;
            
                if (keyboardState.IsKeyDown(Keys.S))
                    motion.Y = 1;
            }
            else
            {
                keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.Up))
                    motion.Y = -1;
            
                if (keyboardState.IsKeyDown(Keys.Down))
                    motion.Y = 1;
            }


            motion.Y *= paddleSpeed;
            position += motion;
            LockPaddle();
        }
        
        /// <summary>
        /// Keep the paddle within the screen bounds.
        /// </summary>
        private void LockPaddle()
        {
            position.Y = MathHelper.Clamp(position.Y, 0, screenBounds.Height - texture.Height);
        }
        
        /// <summary>
        /// Sets the start position to the center of the screen along the Y axis.
        /// </summary>
        public void SetInStartPosition()
        {
            position.X = startposition;
            position.Y = (screenBounds.Height - texture.Height) /2;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    
        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X,
                (int)position.Y,
                texture.Width,
                texture.Height);
        }
    }
}
