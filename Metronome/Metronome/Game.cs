// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Metronome
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public sealed class Game : Microsoft.Xna.Framework.Game
    {
        SpriteBatch mSpriteBatch;
        private readonly ScreenManager mScreenManager;
        public static int mScreenWidth;
        public static int mScreenHeight;

        public Game()
        {
            IsMouseVisible = true;
            var graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 600;
            mScreenWidth = graphics.PreferredBackBufferWidth;
            mScreenHeight = graphics.PreferredBackBufferHeight;
            Content.RootDirectory = "Content";
            mScreenManager = new ScreenManager();


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the content of the screens
            mScreenManager.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            mScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin();
            mScreenManager.Draw(mSpriteBatch);
            mSpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            SaveData.GetInstance().Save(mScreenManager.mMetronomeScreen.mMetronome, "savedata.txt");
            base.OnExiting(sender, args);
        }
    }
}
