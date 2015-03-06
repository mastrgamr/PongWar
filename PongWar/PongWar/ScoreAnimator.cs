using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongWar
{
    class ScoreAnimator
    {
        float fontAlpha;
        bool animating;
        Vector2 position;

        #region Properties
        public Vector2 Position
        {
            set
            {
                position = value;
            }
        }

        public bool Animating
        {
            get
            {
                return animating;
            }
            set
            {
                animating = value;
            }
        }
        #endregion

        public ScoreAnimator()
        {
            animating = false;
            fontAlpha = 1f;
        }

        public void Update(GameTime gameTime)
        {
            if (animating)
            {
                position.Y -= 1f;
                fontAlpha -= 0.025f;
                if (fontAlpha <= 0)
                {
                    animating = false;
                    fontAlpha = 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, String score)
        {
            if (animating)
            {
                spriteBatch.DrawString(font, score, position, Color.White * fontAlpha);
            }
        }
    }
}
