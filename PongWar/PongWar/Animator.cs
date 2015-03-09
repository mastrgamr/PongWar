using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PongWar
{
    /// <summary>
    /// Used to check how the object will be animated.
    /// </summary>
    public enum Animation
    {
        FADEUP,
        PULSEFADE
    }

    class Animator
    {
        private float fontAlpha;
        private float imageAlpha;
        private bool animating;
        private Vector2 position;

        Animation animation;

        #region Properties
        public Animation AnimationProperty
        {
            get { return animation; }
            set { animation = value; }
        }

        public Vector2 Position
        {
            set { position = value; }
        }

        public bool Animating
        {
            get { return animating; }
            set { animating = value; }
        }
        #endregion

        /// <summary>
        /// Used to animate an object on the screen.
        /// </summary>
        public Animator()
        {
            animating = false;
            fontAlpha = 1f;
            imageAlpha = MathHelper.Clamp(imageAlpha, 0f, 0.25f);
        }

        public void Update(GameTime gameTime)
        {
            if (animation == Animation.FADEUP) { 
                if (animating)
                {
                    position.Y -= 0.75f;
                    fontAlpha -= 0.025f;
                    if (fontAlpha <= 0)
                    {
                        animating = false;
                        fontAlpha = 1;
                    }
                }
            }

            if (animation == Animation.PULSEFADE)
            {
                imageAlpha += 0.005f;
                if (imageAlpha >= 0.25f)
                    imageAlpha = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, String score)
        {
            if (animation == Animation.FADEUP)
            {
                if (animating)
                {
                    spriteBatch.DrawString(font, score, position, Color.White * fontAlpha);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 _position, Texture2D texture)
        {
            if (animation == Animation.PULSEFADE)
            {
                spriteBatch.Draw(texture, new Rectangle((int)_position.X, (int)_position.Y, 100, 100), null, Color.White * imageAlpha);
            }
        }
    }
}
