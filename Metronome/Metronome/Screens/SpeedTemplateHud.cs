// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    sealed class SpeedTemplateHud : IScreen
    {
        private Rectangle mRectangle;
        private readonly Metronome mMetronome;
        private readonly ScreenManager mScreenManager;
        private readonly SpeedTemplate mSpeedTemplate;
        private readonly string[] mStatements;
        private readonly Vector2[] mStatementPos;
        private readonly int[][] mBars;
        private int mRunningStatement;

        // Content
        private Texture2D mBackground;
        private Texture2D mRedDot;
        private SpriteFont mFont;
        private SpriteFont mSmallFont;

        public SpeedTemplateHud(ScreenManager screenManager, Metronome metronome)
        {
            var width = Game.mScreenWidth / 5;
            var height = Game.mScreenHeight;
            mRectangle = new Rectangle(Game.mScreenWidth - width, 0, width, height);
            mScreenManager = screenManager;
            mMetronome = metronome;
            mSpeedTemplate = mMetronome.mCurrentTemplate;

            // Initialize the statement values
            var templateString = mSpeedTemplate.ToString();
            mStatements = new string[templateString.Count(x => x == '.')];
            mStatementPos = new Vector2[mStatements.Length];
            mBars = new int[mStatements.Length][];
            var i = 0;
            for (var j = 0; j < mStatements.Length; j++)
            {
                if ((templateString.IndexOf('.', i)) == -1) break;

                // initialize string
                var strLength = (templateString.IndexOf('.', i)) - i;
                mStatements[j] = mSpeedTemplate.ToString().Substring(i, strLength);
                if (mStatements[j][0] == 'r')
                {
                    mStatements[j] = mStatements[j].Substring(1);
                }

                // initialize position
                mStatementPos[j] = new Vector2(mRectangle.X + 45, 40 + j*20);

                // initialize number of bars 
                mBars[j] = new int[3];
                mBars[j][0] = int.Parse(mStatements[j].Substring(0, mStatements[j].IndexOf('x')));
                if (mStatements[j].Count(x => x == 'x') > 1)
                {
                    var startIndex = mStatements[j].LastIndexOf('x') + 1;
                    mBars[j][1] = int.Parse(mStatements[j].Substring(startIndex, mStatements[j].Length - startIndex));
                    mBars[j][2] = mBars[j][0] * mBars[j][1];
                }
                else
                {
                    mBars[j][1] = 0;
                    mBars[j][2] = mBars[j][0];
                }

                i = templateString.IndexOf('.', i) + 1;
            }
        }

        public void LoadContent(ContentManager content)
        {
            // Load textures
            mBackground = content.Load<Texture2D>("Textures/BlackSquare");
            mRedDot = content.Load<Texture2D>("Textures/RedDot");

            // Load fonts
            mFont = content.Load<SpriteFont>("Fonts/MediumFont");
            mSmallFont = content.Load<SpriteFont>("Fonts/SmallFont");
        }

        public void Update(GameTime gameTime, Input mouseInput, Input keyboardInput)
        {
            // TODO: make statements clickable to enable jumps to statements

            var barCount = 0;
            for (var i = 0; i < mBars.Length; i++)
            {
                barCount += mBars[i][2];
                if (barCount >= mMetronome.mTotalBarCounter)
                {
                    mRunningStatement = i;
                    break;
                }
            }

            if (mMetronome.mPlayState == Metronome.PlayState.Stopped)
            {
                mScreenManager.StageScreenForRemoval();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background
            spriteBatch.Draw(mBackground, mRectangle, Color.White * 0.8f);

            // Draw the template name
            spriteBatch.DrawString(mFont, mSpeedTemplate.mName, new Vector2(mRectangle.X + 10, 10), Color.White);

            // Draw the statements
            for (var i = 0; i < mStatements.Length; i++)
            {
                spriteBatch.DrawString(mSmallFont, mStatements[i], mStatementPos[i], Color.White);   
            }

            // Draw the "You Are Here"-Dot
            var rect = new Rectangle(mRectangle.X + 10, (int)mStatementPos[mRunningStatement].Y - 5, 25, 25);
            spriteBatch.Draw(mRedDot, rect, Color.White);

            // TODO: Draw progress within the current statement

        }

        public bool IsInScreen(int x, int y)
        {
            return false;
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }
    }
}
