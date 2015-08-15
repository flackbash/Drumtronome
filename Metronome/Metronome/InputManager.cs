// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System.Diagnostics;
using System.Linq;
using Metronome.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Metronome
{
    /// <summary>
    /// Defines the different input action types
    /// </summary>
    public enum InputType
    {
        LeftButtonDown, LeftButtonUp, RightButtonDown, RightButtonUp, Mouseover, Keystroke, Idle
    }

    public struct Input
    {
        public readonly InputType mType;
        public readonly Keys[] mKey;
        public Vector2 mPosition;

        public Input(InputType type, Vector2 position, Keys[] keys = null)
        {
            mType = type;
            mKey = keys;
            mPosition = position;
        }
    }

    sealed class InputManager
    {
        private MouseState mMouseState;
        private KeyboardState mKeyboardState;
        private MouseState mLastMouseState;
        private KeyboardState mLastKeyboardState;

        private bool mInputTaken;
        private int mPressedCounter;

        /// <summary>
        /// Returns true if the current screen gets an input.
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool GetsInput(IScreen screen, Input input)
        {
            if (mInputTaken) return false;
            if (input.mType == InputType.Keystroke) return true;
            var getsInput = screen.IsInScreen(mMouseState.X, mMouseState.Y);
            mInputTaken = getsInput;
            return getsInput;
        }

        /// <summary>
        /// Returns the type of the current mouse input state.
        /// </summary>
        /// <returns></returns>
        public Input GetMouseInputType()
        {
            Vector2 mousePos = new Vector2(mMouseState.X, mMouseState.Y);
            if (mMouseState.LeftButton == ButtonState.Released && mLastMouseState.LeftButton == ButtonState.Released &&
                mMouseState.RightButton == ButtonState.Released && mLastMouseState.RightButton == ButtonState.Released)
            {
                return new Input(InputType.Mouseover, mousePos);
            }
            if (mMouseState.LeftButton == ButtonState.Pressed && mLastMouseState.LeftButton == ButtonState.Released)
            {
                return new Input(InputType.LeftButtonDown, mousePos);
            }
            if (mMouseState.LeftButton == ButtonState.Released && mLastMouseState.LeftButton == ButtonState.Pressed)
            {
                return new Input(InputType.LeftButtonUp, mousePos);
            }
            if (mMouseState.RightButton == ButtonState.Pressed && mLastMouseState.RightButton == ButtonState.Released)
            {
                Debug.WriteLine("Right Down in Input Manager");
                return new Input(InputType.RightButtonDown, mousePos);
            }
            if (mMouseState.RightButton == ButtonState.Released && mLastMouseState.RightButton == ButtonState.Pressed)
            {
                Debug.WriteLine("Right Up in Input Manager");            
                return new Input(InputType.RightButtonUp, mousePos);
            }
            return new Input(InputType.Idle, Vector2.Zero);
        }

        /// <summary>
        /// Returns the type of the current mouse input state.
        /// </summary>
        /// <returns></returns>
        public Input GetKeyboardInput()
        {
            if (mKeyboardState.GetPressedKeys().Length == 1)
            {
                var currKey = mKeyboardState.GetPressedKeys()[0];
                if (mLastKeyboardState.IsKeyUp(currKey))
                {
                    mPressedCounter++;
                    return new Input(InputType.Keystroke, Vector2.Zero, new[] { currKey });
                }
                if (mLastKeyboardState.IsKeyDown(currKey))
                {
                    mPressedCounter++;
                    if (mPressedCounter > 20 && mPressedCounter % 5 == 0)
                    {
                        return new Input(InputType.Keystroke, Vector2.Zero, new[] { currKey });
                    }
                }
            }
            else if (mKeyboardState.GetPressedKeys().Length == 0) mPressedCounter = 0;
            else if (mKeyboardState.GetPressedKeys().Length > mLastKeyboardState.GetPressedKeys().Length)
            {
                var currentKeys = mKeyboardState.GetPressedKeys();
                var oldKeys = mLastKeyboardState.GetPressedKeys();
                return new Input(InputType.Keystroke, Vector2.Zero, currentKeys.Where(key => !oldKeys.Contains(key)).ToArray());
            }
            return new Input(InputType.Idle, Vector2.Zero);
        }

        /// <summary>
        /// Updates the current input state.
        /// </summary>
        public void UpdateInputState()
        {
            mMouseState = Mouse.GetState();
            mKeyboardState = Keyboard.GetState();
            mInputTaken = false;
        }

        /// <summary>
        /// Updates the last input state.
        /// </summary>
        public void UpdateLastInputState()
        {
            mLastKeyboardState = mKeyboardState;
            mLastMouseState = mMouseState;
        }
    }
}
