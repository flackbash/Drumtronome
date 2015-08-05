// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Metronome
{
    sealed class TextBox
    {
        private string mText;
        private string mDefaultText;
        private Rectangle mRect;
        private Vector2 mStringPos;  // The absolute position of the string
        private bool mIsFocused;
        private bool mUseSmallFont;
        private Color mTextColor;
        private Color mBoxColor;
        private int mCursorIndex;  // Cursor is displayed in front of the character with this index
        private bool mDigitsOnly;
        private int mMaxTextLength;

        private SpriteFont mFont;
        private SpriteFont mMediumFont;
        private SpriteFont mSmallFont;
        private Texture2D mTextBox;
        private Texture2D mCursor;

        enum Direction
        {
            Right, Left
        }

        public TextBox(Rectangle rect, bool digitsOnly=false, int maxTextLength=3)
        {
            mBoxColor = Color.White;
            mTextColor = Color.Black;

            mMaxTextLength = maxTextLength;
            mRect = rect;
            mDefaultText = "";
            mText = mDefaultText;
            mDigitsOnly = digitsOnly;
        }

        public void LoadContent(ContentManager content)
        {
            mMediumFont = content.Load<SpriteFont>("Fonts/MediumFont");
            mSmallFont = content.Load<SpriteFont>("Fonts/SmallFont");
            mFont = mUseSmallFont ? mSmallFont : mMediumFont;

            mTextBox = content.Load<Texture2D>("Textures/TextBox");
            mCursor = content.Load<Texture2D>("Textures/BlackSquare");

            var yVal = mFont.MeasureString("A").Y;
            mStringPos = new Vector2(mRect.X + 5, mRect.Y + (mRect.Height - yVal) / 2);
        }

        /// <summary>
        /// Draws the TextBox, the text it contains and if focused the focus.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            mFont = mUseSmallFont ? mSmallFont : mMediumFont;
            spriteBatch.Draw(mTextBox, mRect, mBoxColor);
            spriteBatch.DrawString(mFont, mText, mStringPos, mTextColor);
            if (mIsFocused)
            {
                var xPos = mStringPos.X + mFont.MeasureString(mText.Substring(0, mCursorIndex)).X;
                var yPos = mStringPos.Y;
                spriteBatch.Draw(mCursor, new Rectangle((int) xPos, (int) yPos + 1, 1 ,(int) mFont.MeasureString("A").Y), mTextColor);
            }
        }

        /// <summary>
        /// Returns true if the position is within the rectangle of the box.
        /// </summary>
        /// <returns></returns>
        public bool IsInBox(Vector2 position)
        {
            return mRect.Contains((int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Returns the current TextBox text.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return mText;
        }

        /// <summary>
        /// Adds a character at the current cursor position.
        /// </summary>
        /// <param name="c"></param>
        private void AddChar(char c)
        {
            // TODO: Allow '-' when only digits?
            if (mText.Length >= mMaxTextLength || (mDigitsOnly && (c < 48 || c > 57))) return;
            mText = mText.Substring(0, mCursorIndex) + c + mText.Substring(mCursorIndex, mText.Length - mCursorIndex);
            mCursorIndex++;
        }

        /// <summary>
        /// Removes the character in front of the current cursor position.
        /// </summary>
        private void RemoveChar(bool behindCursor=false)
        {
            if (behindCursor)
            {
                if (mCursorIndex >= mText.Length) return;
                mText = mText.Substring(0, mCursorIndex) + mText.Substring(mCursorIndex + 1, mText.Length - (mCursorIndex + 1));
            }
            else
            {
                if (mCursorIndex == 0) return;
                mText = mText.Substring(0, mCursorIndex - 1) + mText.Substring(mCursorIndex, mText.Length - mCursorIndex);
                mCursorIndex--;
            }
        }

        /// <summary>
        /// Sets the text of the text box to the specified text.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            mText = text;
        }

        /// <summary>
        /// Sets the default text to the specified text.
        /// </summary>
        /// <param name="text"></param>
        public void SetDefaultText(string text)
        {
            mDefaultText = text;
        }

        /// <summary>
        /// Clears the textbox.
        /// </summary>
        public void Clear()
        {
            mText = "";
            mCursorIndex = 0;
        }

        /// <summary>
        /// Sets the focus and the cursor index.
        /// </summary>
        /// <param name="position">The position of the click</param>
        public void OnClick(Vector2 position)
        {
            mIsFocused = true;
            SetCursorIndex(position.X);
        }

        /// <summary>
        /// Returns true if the text box has the focus.
        /// </summary>
        /// <returns></returns>
        public bool IsFocused()
        {
            return mIsFocused;
        }

        /// <summary>
        /// Sets the focus on the text box
        /// </summary>
        public void SetFocus()
        {
            mIsFocused = true;
            mCursorIndex = mText.Length;
        }

        /// <summary>
        /// Removes the focus from the text box and sets the text to its default value if needed.
        /// </summary>
        public void RemoveFocus()
        {
            mIsFocused = false;
            if(mText == "") SetText(mDefaultText);
        }

        private void SetCursorIndex(float x)
        {
            for (var i = 0; i < mText.Length; i++)
            {
                if (Math.Round(mFont.MeasureString(mText.Substring(0, i)).X + mStringPos.X) >= x)
                {
                    mCursorIndex = i;
                    return;
                }
            }
            mCursorIndex = mText.Length;
        }

        private void MoveCursorIndex(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    if (mCursorIndex < mText.Length) mCursorIndex++;
                    break;
                case Direction.Left:
                    if (mCursorIndex > 0) mCursorIndex--;
                    break;
            }
        }

        public void UseSmallFont(bool smallFont)
        {
            mUseSmallFont = smallFont;
        }

        /// <summary>
        /// Takes the input key and transfers it to an appropriate text box input or action. 
        /// Returns true if the key is used by the text box.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal bool OnKeyDown(Keys key)
        {
            // TODO: Handle CapsLock
            // Handle letters
            if (key.ToString().Length == 1)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    AddChar(key.ToString()[0]);
                    return true;
                }
                AddChar(key.ToString().ToLower()[0]);
                return true;
            }
            switch (key)
            {
                // Handle numbers
                case Keys.D0:
                    AddChar('0');
                    return true;
                case Keys.D1:
                    AddChar('1');
                    return true;
                case Keys.D2:
                    AddChar('2');
                    return true;
                case Keys.D3:
                    AddChar('3');
                    return true;
                case Keys.D4:
                    AddChar('4');
                    return true;
                case Keys.D5:
                    AddChar('5');
                    return true;
                case Keys.D6:
                    AddChar('6');
                    return true;
                case Keys.D7:
                    AddChar('7');
                    return true;
                case Keys.D8:
                    AddChar('8');
                    return true;
                case Keys.D9:
                    AddChar('9');
                    return true;
                case Keys.NumPad0:
                    AddChar('0');
                    return true;
                case Keys.NumPad1:
                    AddChar('1');
                    return true;
                case Keys.NumPad2:
                    AddChar('2');
                    return true;
                case Keys.NumPad3:
                    AddChar('3');
                    return true;
                case Keys.NumPad4:
                    AddChar('4');
                    return true;
                case Keys.NumPad5:
                    AddChar('5');
                    return true;
                case Keys.NumPad6:
                    AddChar('6');
                    return true;
                case Keys.NumPad7:
                    AddChar('7');
                    return true;
                case Keys.NumPad8:
                    AddChar('8');
                    return true;
                case Keys.NumPad9:
                    AddChar('9');
                    return true;
                // Handle other characters
                case Keys.OemPeriod:
                    AddChar('.');
                    return true;
                case Keys.OemComma:
                    AddChar(',');
                    return true;
                case Keys.OemMinus:
                    AddChar('-');
                    return true;
                case Keys.OemPlus:
                    AddChar('+');
                    return true;
                case Keys.OemQuestion:
                    AddChar('?');
                    return true;
                case Keys.OemQuotes:
                    AddChar('\'');
                    return true;
                case Keys.OemSemicolon:
                    AddChar(';');
                    return true;
                case Keys.OemTilde:
                    AddChar('~');
                    return true;
                case Keys.OemBackslash:
                    AddChar('/');
                    return true;
                case Keys.OemOpenBrackets:
                    AddChar('(');
                    return true;
                case Keys.OemCloseBrackets:
                    AddChar(')');
                    return true;
                case Keys.OemPipe:
                    AddChar('|');
                    return true;
                case Keys.Space:
                    AddChar(' ');
                    return true;
                case Keys.Subtract:
                    AddChar('-');
                    return true;
                case Keys.Multiply:
                    AddChar('*');
                    return true;
                // Handle action keys
                case Keys.Back:
                    RemoveChar();
                    return true;
                case Keys.Delete:
                    RemoveChar(true);
                    return true;
                case Keys.Right:
                    MoveCursorIndex(Direction.Right);
                    return true;
                case Keys.Left:
                    MoveCursorIndex(Direction.Left);
                    return true;
                case Keys.End:
                    if (mText.Length > 0) mCursorIndex = mText.Length;
                    return true;
            }
            return false;
        }
    }
}
