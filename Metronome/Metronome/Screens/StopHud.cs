// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    sealed class StopHud : Hud
    {        
        public StopHud(ScreenManager screenManager, Metronome metronome, Rectangle rect) : base(screenManager, metronome, rect)
        {
            // Initialize Texts
            mTexts = new[,] { { "Stop After", "Bars" }};
            mTextPositions = new Vector2[1];
            mTextPositions[0] = new Vector2(mRectangle.X + 10, mRectangle.Y + 70);

            // Initialize TextBoxes
            mTextBoxes = new TextBox[1];
            mTextBoxes[0] = new TextBox(new Rectangle(mRectangle.X + mRectangle.Width - 120, (int)mTextPositions[0].Y, 60, 25), true);
            mTextBoxes[0].SetDefaultText("100");
            // Initialize Values from Metronome
            mTextBoxes[0].SetText(mMetronome.mStopAfter.ToString());
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw a shader over underlying layers
            spriteBatch.Draw(mBackground, new Rectangle(0, 0, Game.mScreenWidth, Game.mScreenHeight), Color.White * 0.25f);

            base.Draw(spriteBatch);
        }

        protected override void Ok()
        {
            base.Ok();
            mMetronome.SetStopValues(Int32.Parse(mTextBoxes[0].GetText()));
            
        }

        protected override void Close()
        {
            base.Close();
            mTextBoxes[0].SetText(mMetronome.mStopAfter.ToString());
        }
    }
}
