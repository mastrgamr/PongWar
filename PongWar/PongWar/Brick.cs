using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongWar
{
    class Brick
    {
        Texture2D texture;
        Rectangle location;
        Color tint;
        bool alive;
        bool brickCollided;
        static int tempscore1;
        static int tempscore2;
        static int tempscore3;

        #region Properties
        public bool BrickCollided
        {
            get { return brickCollided; }
            set { brickCollided = value; }
        }

        public Color Tint
        {
            get { return tint; }
        }

        public static int totalscore1
        {
            get { return tempscore1; }
            set { tempscore1 = value; }
        }

        public static int totalscore2
        {
            get { return tempscore2; }
            set { tempscore2 = value; }
        }

        public static int brickdestroyed
        {
            get { return tempscore3; }
            set { tempscore3 = value; }
        }

        public Rectangle Location
        {
            get { return location; }
        }

        public bool IsAlive
        {
            get { return alive; }
            set { alive = value; }
        }
        #endregion

        public Brick(Texture2D texture, Rectangle location, Color tint)
        {
            this.texture = texture;
            this.location = location;
            this.tint = tint;
            this.alive = true;
         }

        public void CheckCollision(Ball ball, int ballvar)
        {
            if (alive && ball.Bounds.Intersects(location))
            {
                alive = false;
                brickCollided = true;
                ball.Deflection(this);
                brickdestroyed = 1; // Used to reduce the number of bricks
                if (ballvar == 1) // Check if player 1 ball collided and increase score
                {
                    totalscore1 += 10;
                }
                else // Check if player 2 ball collided and increase score
                {
                    totalscore2 += 10;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
                spriteBatch.Draw(texture, location, tint);
        }
    }
}
