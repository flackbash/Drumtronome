// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Metronome.Screens
{
    abstract class Hud : IScreen
    {
        protected readonly ScreenManager mScreenManager;
        protected readonly Metronome mMetronome;
        protected Rectangle mRectangle;
        protected Button[] mButtons;
        protected TextBox[] mTextBoxes;
        protected string[,] mTexts;
        protected Vector2[] mTextPositions;

        // Content
        protected Texture2D mBackground;
        protected SpriteFont mFont;
        private SpriteFont mSmallFont;

        protected Hud(ScreenManager screenManager, Metronome metronome, Rectangle rect)
        {
            mScreenManager = screenManager;
            mMetronome = metronome;
            mRectangle = rect;

            // Initialize basic buttons "Ok" and "Close" that should be displayed on every HUD.
            var actions = new MetronomeScreen.RunAction[] { Ok, Close };
            var names = new[] { "Ok", "Close" };

            Initialize(actions, names);

            var size = 32;
            mButtons[0].mRectangle = new Rectangle(mRectangle.X + mRectangle.Width - (size * 2 + 20), mRectangle.Y + 5, size, size);
            mButtons[1].mRectangle = new Rectangle(mRectangle.X + mRectangle.Width - (size + 10), mRectangle.Y + 5, size, size);
        }

        /// <summary>
        /// Initializes the buttons of the HUD.
        /// </summary>
        private void Initialize(MetronomeScreen.RunAction[] actions, string[] names, string[] labels=null)
        {
            mButtons = new Button[actions.Length];
            for (var i = 0; i < mButtons.Length; i++)
            {
                if (labels != null) mButtons[i].mLabel = labels[i];
                mButtons[i].mTexName = names[i];
                mButtons[i].mState = OwnButtonState.Normal;
                mButtons[i].mAction = actions[i];
                mButtons[i].mTextures = new Texture2D[3];
            }
        }

        public virtual void LoadContent(ContentManager content)
        {
            // Load textures
            mBackground = content.Load<Texture2D>("Textures/BlackSquare");

            // Load buttons
            string[] states = { "Normal", "Hot", "Pressed" };
            for (var i = 0; i < mButtons.Length; i++)
            {
                for (var j = 0; j < states.Length; j++)
                {
                    mButtons[i].mTextures[j] = content.Load<Texture2D>("Textures/Buttons/" + mButtons[i].mTexName + states[j]);
                }
            }

            // Load fonts
            mFont = content.Load<SpriteFont>("Fonts/MediumFont");
            mSmallFont = content.Load<SpriteFont>("Fonts/SmallFont");

            // Load Content for text boxes
            if (mTextBoxes == null) return;
            foreach (TextBox tbox in mTextBoxes)
            {
                tbox.LoadContent(content);
            }
        }

        public virtual void Update(GameTime gameTime, Input mouseInput, Input keyboardInput)
        {
            HandleClicks(mouseInput);
            HandleMouseovers(mouseInput);
            HandleKeyboardInput(keyboardInput);
        }

        private void HandleClicks(Input mouseInput)
        {
            // Handle clicks on buttons
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
                    mButtons[i].mState = OwnButtonState.Normal;
                }
            }
            // Handle clicks on text boxes
            if (mouseInput.mType == InputType.LeftButtonUp && mTextBoxes != null)
            {
                foreach (TextBox tbox in mTextBoxes)
                {
                    if (tbox.IsInBox(mouseInput.mPosition))
                    {
                        tbox.OnClick(mouseInput.mPosition);
                    }
                    else
                    {
                        tbox.RemoveFocus();
                    }
                }
            }
        }

        private void HandleMouseovers(Input mouseInput)
        {
            if (mouseInput.mType == InputType.Mouseover)
            {
                // Check for mouseovers on buttons
                for (var i = 0; i < mButtons.Length; i++)
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

        private void HandleKeyboardInput(Input keyboardInput)
        {
            // Handle keyboard input
            if (keyboardInput.mType != InputType.Keystroke) return;
            var unusedKeys = new List<Keys>();
            var nothingUsed = true;
            if (mTextBoxes != null)
            {
                foreach (var tbox in mTextBoxes.Where(tbox => tbox.IsFocused()))
                {
                    unusedKeys.AddRange(keyboardInput.mKey.Where(key => !tbox.OnKeyDown(key)));
                    nothingUsed = false;
                    break;
                }
            }
            if (nothingUsed) unusedKeys = keyboardInput.mKey.ToList();
            foreach (var key in unusedKeys)
            {
                OnKeyDown(key);
            }
        }

        private void OnKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Tab:
                    if (mTextBoxes != null && mTextBoxes.Length > 1)
                    {
                        var focused = false;
                        for (var i = 0; i < mTextBoxes.Length; i++)
                        {
                            if (!mTextBoxes[i].IsFocused())
                            {
                                continue;
                            }
                            mTextBoxes[i].RemoveFocus();
                            mTextBoxes[++i % mTextBoxes.Length].SetFocus();
                            focused = true;
                            break;
                        }
                        if (!focused && mTextBoxes.Length > 0) mTextBoxes[0].SetFocus();
                    }
                    break;
                case Keys.Escape:
                    Close();
                    break;
                case Keys.Enter:
                    Ok();
                    break;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background
            spriteBatch.Draw(mBackground, mRectangle, Color.White * 0.8f);

            // Draw the buttons
            for (var i = 0; i < mButtons.Length; i++)
            {
                if (mButtons[i].mInvisible)
                {
                    continue;
                }
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
                if (mButtons[i].mLabel != null)
                {
                    var strSize = mSmallFont.MeasureString(mButtons[i].mLabel);
                    var pos = new Vector2(mButtons[i].mRectangle.X + (mButtons[i].mRectangle.Width - strSize.X) / 2, mButtons[i].mRectangle.Y + (mButtons[i].mRectangle.Height - strSize.Y) / 2);
                    spriteBatch.DrawString(mSmallFont, mButtons[i].mLabel, pos, Color.Black);                        
                }
            }

            // Draw the text boxes and their content
            if (mTextBoxes == null) return;
            for (var i = 0; i < mTextBoxes.Length; i++)
            {
                spriteBatch.DrawString(mFont, mTexts[i, 0], mTextPositions[i], Color.White);
                spriteBatch.DrawString(mSmallFont, mTexts[i, 1], new Vector2(mRectangle.X + mRectangle.Width - 50, mTextPositions[i].Y), Color.White);
                mTextBoxes[i].Draw(spriteBatch);
            }
        }

        public bool IsInScreen(int x, int y)
        {
            return mRectangle.Contains(x, y);
        }

        public virtual bool UpdateLower()
        {
            return false;
        }

        public bool DrawLower()
        {
            return true;
        }

        /// <summary>
        /// Stages HUD for removal and takes appropriate save/adjust actions.
        /// </summary>
        protected virtual void Ok()
        {
            if (mTextBoxes != null)
            {
                foreach (var tbox in mTextBoxes)
                {
                    tbox.RemoveFocus();
                }
            }
            mScreenManager.StageScreenForRemoval();
        }

        /// <summary>
        /// Stages HUD for removal and discards all changes.
        /// </summary>
        protected virtual void Close()
        {
            if (mTextBoxes != null)
            {
                foreach (var tbox in mTextBoxes)
                {
                    tbox.RemoveFocus();
                }
            }
            mScreenManager.StageScreenForRemoval();
        }
    }
}
