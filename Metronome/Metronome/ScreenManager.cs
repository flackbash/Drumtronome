// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.
// Thanks to github user shukon, who wrote most parts of this class.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Metronome.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Metronome
{
    sealed class ScreenManager
    {
        private readonly ContentManager mContent;
        private readonly InputManager mInputManager;
        private readonly Stack<IScreen> mScreenStack;
        private readonly Stack<IScreen> mScreensToDraw;
        private readonly Stack<IScreen> mScreensToUpdate;
        private readonly List<IScreen> mScreensToAdd;
        private int mRemovalCount;

        internal readonly MetronomeScreen mMetronomeScreen;

        public ScreenManager(ContentManager content)
        {
            mContent = content;
            mInputManager = new InputManager();
            mScreenStack = new Stack<IScreen>();
            mScreensToDraw = new Stack<IScreen>();
            mScreensToUpdate = new Stack<IScreen>();
            mScreensToAdd = new List<IScreen>();

            mMetronomeScreen = new MetronomeScreen(this, mContent);

            mScreenStack.Push(mMetronomeScreen);
        }

        /// <summary>
        /// Stages a screen to be added after the update phase.
        /// </summary>
        public void StageScreenForAdding(IScreen screen)
        {
            mScreensToAdd.Add(screen);
        }

        /// <summary>
        /// Increments the number of screens to be removed after the update phase.
        /// </summary>
        public void StageScreenForRemoval(int num = 1)
        {
            mRemovalCount += num;
        }

        /// <summary>
        /// Adds all screens that were staged during the update phase in the order of their staging.
        /// </summary>
        private void AddScreens()
        {
            foreach (var screen in mScreensToAdd)
            {
                mScreenStack.Push(screen);
            }
            mScreensToAdd.Clear();
        }

        /// <summary>
        /// Removes the number of screens that were staged for removal during the update phase.
        /// </summary>
        private void RemoveScreens()
        {
            for (var i = 0; i < mRemovalCount; i++) mScreenStack.Pop();
            mRemovalCount = 0;
        }

        /// <summary>
        /// Loads the content for already existing screens.
        /// </summary>
        /// <param name="content"></param>
        public void LoadContent(ContentManager content)
        {
            mMetronomeScreen.LoadContent(content);
        }

        /// <summary>
        /// Updates all underlying screens if they are supposed to be drawn.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                Debug.WriteLine("Right click!");
            }
            mInputManager.UpdateInputState();
            Input mouseInput = mInputManager.GetMouseInputType();
            Input keyboardInput = mInputManager.GetKeyboardInput();
            bool update = true;
            while (update)
            {
                var currMouseInput = mouseInput;
                if (!mInputManager.GetsInput(mScreenStack.Peek(), mouseInput))
                {
                    currMouseInput = new Input(InputType.Idle, Vector2.Zero);
                }
                mScreenStack.Peek().Update(gameTime, currMouseInput, keyboardInput);
                update = mScreenStack.Peek().UpdateLower();
                mScreensToUpdate.Push(mScreenStack.Pop());
            }
            while (mScreensToUpdate.Count != 0)
            {
                mScreenStack.Push(mScreensToUpdate.Pop());
            }
            RemoveScreens();
            AddScreens();
            mInputManager.UpdateLastInputState();
        }

        /// <summary>
        /// Draws all underlying screens if they are supposed to be drawn.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            mScreensToDraw.Push(mScreenStack.Pop());
            while (mScreensToDraw.Peek().DrawLower())
            {
                mScreensToDraw.Push(mScreenStack.Pop());
            }
            while (mScreensToDraw.Any())
            {
                mScreensToDraw.Peek().Draw(spriteBatch);
                mScreenStack.Push(mScreensToDraw.Pop());
            }
        }
    }
}
