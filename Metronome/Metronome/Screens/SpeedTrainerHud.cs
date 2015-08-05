// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    sealed class SpeedTrainerHud : Hud
    {
        public SpeedTrainerHud(ScreenManager screenManager, Metronome metronome, Rectangle rect) : base(screenManager, metronome, rect)
        {
            // Initialize Texts
            mTexts = new [,]{{"Increase By", "BMP"},{"Increase After", "Bars"},{ "Increase Up To", "BMP"}};
            mTextPositions = new Vector2[mTexts.GetLength(0)];
            for (var i = 0; i < mTexts.GetLength(0); i++)
            {
                var xpos = mRectangle.X + 10;
                var ypos = mRectangle.Y + 70 + i * 40;
                mTextPositions[i] = new Vector2(xpos, ypos);
            }

            // Initialize TextBoxes
            mTextBoxes = new TextBox[mTexts.GetLength(0)];
            for (var i = 0; i < mTextBoxes.Length; i++)
            {
                mTextBoxes[i] = new TextBox(new Rectangle(mRectangle.X + mRectangle.Width - 120, (int) mTextPositions[i].Y, 60, 25), true);
            }

            mTextBoxes[0].SetDefaultText("10");
            mTextBoxes[1].SetDefaultText("8");
            mTextBoxes[2].SetDefaultText("200");

            // Initialize values from Metronome
            mTextBoxes[0].SetText(mMetronome.mIncreaseBy.ToString());
            mTextBoxes[1].SetText(mMetronome.mIncreaseAfter.ToString());
            mTextBoxes[2].SetText(mMetronome.mIncreaseUpTo.ToString());
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
            mMetronome.SetIncreaseValues(int.Parse(mTextBoxes[0].GetText()), int.Parse(mTextBoxes[1].GetText()), int.Parse(mTextBoxes[2].GetText()));
        }

        protected override void Close()
        {
            base.Close();
            mTextBoxes[0].SetText(mMetronome.mIncreaseBy.ToString());
            mTextBoxes[1].SetText(mMetronome.mIncreaseAfter.ToString());
            mTextBoxes[2].SetText(mMetronome.mIncreaseUpTo.ToString());
        }
    }
}
