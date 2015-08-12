// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    public enum OwnButtonState
    {
        Normal, Hot, Pressed
    }

    public struct Button
    {
        internal string mLabel;
        internal string mTexName;
        internal OwnButtonState mState;
        internal Texture2D[] mTextures;
        internal Rectangle mRectangle;
        internal MetronomeScreen.RunAction mAction;
        internal MetronomeScreen.RunAction mActionRight;
        internal MetronomeScreen.RunParameterAction mParameterAction;
        internal MetronomeScreen.RunParameterAction mParameterActionRight;
        internal int mParameter;
        internal bool mInvisible;
    }

    sealed class MetronomeScreen : IScreen
    {
        internal delegate void RunAction();
        internal delegate void RunParameterAction(int parameter);

        private readonly ScreenManager mScreenManager;
        private readonly ContentManager mContent;
        internal readonly Metronome mMetronome;

        private readonly SideBar mSideBar;
        private readonly SpeedTrainerHud mSpeedTrainerHud;
        private readonly StopHud mStopHud;

        private Rectangle mRectangle;
        private Rectangle mMetronomeRect;
        private Vector2 mPointerPos;
        private float mRotationAngle;
        private Vector2 mRotationPoint;
        private float mMetronomeScaling;
        private readonly Button[] mButtons;
        private bool mIsPointerLeft;

        // Content
        private Texture2D mBackground;
        private Texture2D mMetronomeBody;
        private Texture2D mMetronomePointer;
        private SpriteFont mBigFatFont;
        private SpriteFont mBigFont;
        private SpriteFont mMediumFont;

        public MetronomeScreen(ScreenManager screenManager, ContentManager content)
        {
            mContent = content;
            mMetronome = File.Exists("savedata.txt") ? SaveData.GetInstance().Load("savedata.txt") : new Metronome();
            mScreenManager = screenManager;
            mRectangle = new Rectangle(0, 0, Game.mScreenWidth, Game.mScreenHeight);

            // Initialize other screens
            const int width = 300;
            const int height = 200;
            var rect = new Rectangle(Game.mScreenWidth / 2 - width / 2, Game.mScreenHeight / 2 - height / 2, width, height);
            mSpeedTrainerHud = new SpeedTrainerHud(mScreenManager, mMetronome, rect);
            mStopHud = new StopHud(mScreenManager, mMetronome, rect);
            mSideBar = new SideBar(mScreenManager, mContent, mMetronome, new Rectangle(0, 0, Game.mScreenWidth / 5, Game.mScreenHeight));

            // Initialize buttons
            RunAction[] actions =
            {
                mMetronome.Resume, mMetronome.Pause, mMetronome.Stop, null, null, null, null, mMetronome.ToggleAutoStop, mMetronome.ToggleAutoIncrease
            };
            RunAction[] rightActions = {ShowStopHud, ShowSpeedTrainerHud};
            string[] names = {"Play", "Pause", "Stop", "Increase", "Decrease", "Increase10", "Decrease10", "StopMode", "SpeedMode"};
            int[] parameters = {1, -1, 10, -10};

            mButtons = new Button[actions.Length];
            for (var i = 0; i < mButtons.Length; i++)
            {
                mButtons[i].mTexName = names[i];
                mButtons[i].mState = OwnButtonState.Normal;
                mButtons[i].mAction = actions[i];
                mButtons[i].mTextures = new Texture2D[3];
                if (i > 2 && i < 7)
                {
                    mButtons[i].mParameterAction = mMetronome.ChangeTempoBy;
                    mButtons[i].mParameter = parameters[i - 3];
                }
                else if (i >= 7)
                {
                    mButtons[i].mActionRight = rightActions[i-7];
                }
            }

            var size = 64;
            mButtons[0].mRectangle = new Rectangle((int)(mRectangle.Width / 2f - (size + 10)), 20, size, size);
            mButtons[1].mRectangle = mButtons[0].mRectangle;
            mButtons[2].mRectangle = new Rectangle((int) (mRectangle.Width / 2f + 10), 20, size, size);
            size = 32;
            mButtons[3].mRectangle = new Rectangle((int)(mRectangle.Width / 2f - (size + 50)), 470, size, size);
            mButtons[4].mRectangle = new Rectangle((int)(mRectangle.Width / 2f + 50), 470, size, size);
            mButtons[5].mRectangle = new Rectangle((int)(mRectangle.Width / 2f - (size + 50)), 520, size, size);
            mButtons[6].mRectangle = new Rectangle((int)(mRectangle.Width / 2f + 50), 520, size, size);
            size = 50;
            mButtons[7].mRectangle = new Rectangle((int)(mRectangle.Width / 2f - (size + 130)), 490, size, size);
            mButtons[8].mRectangle = new Rectangle((int)(mRectangle.Width / 2f + 130), 490, size, size);

            mButtons[0].mInvisible = mMetronome.mPlayState == Metronome.PlayState.Playing;
            mButtons[1].mInvisible = mMetronome.mPlayState != Metronome.PlayState.Playing;

            mIsPointerLeft = true;
        }

        public void LoadContent(ContentManager content)
        {
            // Load textures
            mBackground = content.Load<Texture2D>("Textures/Background");
            mMetronomeBody = content.Load<Texture2D>("Textures/MetronomeBody");
            mMetronomePointer = content.Load<Texture2D>("Textures/MetronomePointer");

            // Adjust rectangles and positions according to the image size and scaling
            mMetronomeScaling = 0.25f;
            var width =(int)( mMetronomeBody.Width * mMetronomeScaling);
            var height = (int)(mMetronomeBody.Height * mMetronomeScaling);
            mMetronomeRect = new Rectangle(mRectangle.Width / 2 - width / 2, 130, width, height);
            var xpos = mMetronomeRect.X + mMetronomeRect.Width / 2 - mMetronomePointer.Width * mMetronomeScaling / 2 +
                       mMetronomePointer.Width * mMetronomeScaling / 2;
            var ypos = mMetronomeRect.Y + 136 * mMetronomeScaling + mMetronomePointer.Height * mMetronomeScaling;
            mPointerPos = new Vector2(xpos, ypos);
            // The rotation point has to be defined at the original size of the image...
            mRotationPoint = new Vector2(mMetronomePointer.Width / 2f, mMetronomePointer.Height);

            // Load buttons
            string[] states = {"Normal", "Hot", "Pressed"};
            for (var i = 0; i < mButtons.Length; i++)
            {
                for (var j = 0; j < states.Length; j++)
                {
                    mButtons[i].mTextures[j] = content.Load<Texture2D>("Textures/Buttons/" + mButtons[i].mTexName + states[j]);
                }
            }

            // Load fonts
            mBigFatFont = content.Load<SpriteFont>("Fonts/BigFatFont");
            mBigFont = content.Load<SpriteFont>("Fonts/BigFont");
            mMediumFont = content.Load<SpriteFont>("Fonts/MediumFont");

            mMetronome.LoadContent(content);
            mSideBar.LoadContent(content);
            mScreenManager.StageScreenForAdding(mSideBar);  // I know it's not nice here :( but I don't know where else to put it
            mSpeedTrainerHud.LoadContent(content);
            mStopHud.LoadContent(content);
        }

        public void Update(GameTime gameTime, Input mouseInput, Input keyboardInput)
        {
            mMetronome.Update(gameTime);

            UpdateMetronomePointer();

            HandleClicks(mouseInput);

            HandleMouseovers(mouseInput);

            mButtons[0].mInvisible = mMetronome.mPlayState == Metronome.PlayState.Playing;
            mButtons[1].mInvisible = mMetronome.mPlayState != Metronome.PlayState.Playing;
        }

        /// <summary>
        /// Updates the rotation angle of the metronome if the metronome is playing
        /// </summary>
        private void UpdateMetronomePointer()
        {
            if (mMetronome.mPlayState == Metronome.PlayState.Playing)
            {
                if (mMetronome.mPlayTimer.TotalMilliseconds < mMetronome.mMillisecondsBetweenBeats / 2)
                {
                    mRotationAngle = (float)((Math.PI / 4) * (mMetronome.mPlayTimer.TotalMilliseconds / mMetronome.mMillisecondsBetweenBeats));
                }
                else
                {
                    mRotationAngle = (float)((Math.PI / 4) - (Math.PI / 4) * (mMetronome.mPlayTimer.TotalMilliseconds / mMetronome.mMillisecondsBetweenBeats));
                }
                if (mIsPointerLeft)
                {
                    mRotationAngle = -mRotationAngle;
                }
                if (mMetronome.mPlayTimer == TimeSpan.Zero) mIsPointerLeft = !mIsPointerLeft;
            }
            else if (mMetronome.mPlayState == Metronome.PlayState.Stopped)
            {
                mRotationAngle = 0;
                mIsPointerLeft = true;
            }
        }

        /// <summary>
        /// Handles mouse clicks, left and right.
        /// </summary>
        /// <param name="mouseInput"></param>
        private void HandleClicks(Input mouseInput)
        {
            if (mouseInput.mType == InputType.LeftButtonDown || mouseInput.mType == InputType.RightButtonDown)
            {
                // Check for clicks on buttons
                for (var i = 0; i < mButtons.Length; i++)
                {
                    if (!mButtons[i].mInvisible &&
                        mButtons[i].mRectangle.Contains((int)mouseInput.mPosition.X, (int)mouseInput.mPosition.Y))
                    {
                        mButtons[i].mState = OwnButtonState.Pressed;
                    }
                }
            }
            else if (mouseInput.mType == InputType.LeftButtonUp || mouseInput.mType == InputType.RightButtonUp)
            {
                // Check for clicks on buttons
                for (var i = 0; i < mButtons.Length; i++)
                {
                    if (!mButtons[i].mInvisible &&
                        mButtons[i].mRectangle.Contains((int)mouseInput.mPosition.X, (int)mouseInput.mPosition.Y))
                    {
                        if (mouseInput.mType == InputType.RightButtonUp)
                        {
                            if (mButtons[i].mActionRight != null) mButtons[i].mActionRight();
                            if (mButtons[i].mParameterActionRight != null) mButtons[i].mParameterActionRight(mButtons[i].mParameter);
                        }
                        else
                        {
                            if (mButtons[i].mAction != null) mButtons[i].mAction();
                            if (mButtons[i].mParameterAction != null) mButtons[i].mParameterAction(mButtons[i].mParameter);
                        }
                    }
                }
            }
            mButtons[7].mState = mMetronome.mDoesStop
                ? OwnButtonState.Pressed
                : OwnButtonState.Normal;

            mButtons[8].mState = mMetronome.mDoesIncrease
                ? OwnButtonState.Pressed
                : OwnButtonState.Normal;
        }

        /// <summary>
        /// Handles Mouseovers, short and long ones.
        /// </summary>
        /// <param name="mouseInput"></param>
        private void HandleMouseovers(Input mouseInput)
        {
            // Handle Mouseovers
            if (mouseInput.mType == InputType.Mouseover)
            {
                // Check for mouseovers on buttons
                for (var i = 0; i < mButtons.Length - 2; i++)
                {
                    if (!mButtons[i].mInvisible &&
                        mButtons[i].mRectangle.Contains((int)mouseInput.mPosition.X, (int)mouseInput.mPosition.Y))
                    {
                        mButtons[i].mState = OwnButtonState.Hot;
                    }
                    else mButtons[i].mState = OwnButtonState.Normal;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the background
            spriteBatch.Draw(mBackground, mRectangle, Color.White);

            // draw the metronome
            spriteBatch.Draw(mMetronomeBody, mMetronomeRect, Color.White);
            spriteBatch.Draw(mMetronomePointer, mPointerPos, null, Color.White, mRotationAngle,
                             mRotationPoint, mMetronomeScaling, SpriteEffects.None, 0f);

            // draw the bar counters
            string text;
            Vector2 pos;
            if (mMetronome.mDoesStop)
            {
                text = mMetronome.mStopCounter + " /" + mMetronome.mStopAfter;
                pos = new Vector2(mButtons[7].mRectangle.X + (mButtons[7].mRectangle.Width - mMediumFont.MeasureString(text).X) / 2, mButtons[7].mRectangle.Y + mButtons[7].mRectangle.Height + 5);
                spriteBatch.DrawString(mMediumFont, text, pos, Color.White);
            }
            if (mMetronome.mDoesIncrease)
            {
                text = mMetronome.mIncreaseCounter + " /" + mMetronome.mIncreaseAfter;
                pos = new Vector2(mButtons[8].mRectangle.X + (mButtons[8].mRectangle.Width - mMediumFont.MeasureString(text).X) / 2, mButtons[8].mRectangle.Y + mButtons[8].mRectangle.Height + 5);
                spriteBatch.DrawString(mMediumFont, text, pos, Color.White);
            }
            text = mMetronome.mTotalBarCounter.ToString();
            pos = new Vector2(mMetronomeRect.X + (mMetronomeRect.Width - mBigFatFont.MeasureString(text).X) / 2, mMetronomeRect.Y + mMetronomeRect.Height - (mBigFatFont.MeasureString(text).Y + 20));
            spriteBatch.DrawString(mBigFatFont, text, pos, Color.Black);
  

            // draw the control buttons
            for (var i = 0; i < mButtons.Length; i++)
            {
                if (!mButtons[i].mInvisible)
                {
                    Texture2D button;
                    switch (mButtons[i].mState)
                    {
                        case OwnButtonState.Hot:
                            button = mButtons[i].mTextures[1];
                            break;
                        case OwnButtonState.Pressed:
                            button = mButtons[i].mTextures[2];
                            break;
                        default:
                            button = mButtons[i].mTextures[0];
                            break;
                    }
                    spriteBatch.Draw(button, mButtons[i].mRectangle, Color.White);
                }
            }

            // draw the bmp
            text = mMetronome.mTempo.ToString();
            spriteBatch.DrawString(mBigFont, text, new Vector2(Game.mScreenWidth / 2f - mBigFont.MeasureString(text).X / 2, 500), Color.White);
        }

        public bool IsInScreen(int x, int y)
        {
            return mRectangle.Contains(x, y);
        }

        public bool UpdateLower()
        {
            return false;
        }

        public bool DrawLower()
        {
            return false;
        }

        private void ShowStopHud()
        {
            mScreenManager.StageScreenForAdding(mStopHud);
        }

        private void ShowSpeedTrainerHud()
        {
            mScreenManager.StageScreenForAdding(mSpeedTrainerHud);
        }
    }
}
