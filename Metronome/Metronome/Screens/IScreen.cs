// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.
// Thanks to github user shukon for contributing this class.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    /// <summary>
    /// Interface for all sorts of screens and huds.
    /// </summary>
    interface IScreen
    {
        /// <summary>
        /// Loads the content for the screen.
        /// </summary>
        /// <param name="content">The content manager of the project.</param>
        void LoadContent(ContentManager content);

        /// <summary>
        /// Updates the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="mouseInput"></param>
        /// <param name="keyboardInput"></param>
        void Update(GameTime gameTime, Input mouseInput, Input keyboardInput);

        /// <summary>
        /// Draws the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch of the project.</param>
        void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Returns true if the position is within the screens Rectangle.
        /// </summary>
        /// <param name="x">The x parameter of the position.</param>
        /// <param name="y">The y parameter of the position.</param>
        bool IsInScreen(int x, int y);

        /// <summary>
        /// Returns true if underlying screens should be updated.
        /// </summary>
        /// <returns></returns>
        bool UpdateLower();

        /// <summary>
        /// Returns true if underlying screens should be drawn.
        /// </summary>
        /// <returns></returns>
        bool DrawLower();
    }
}
