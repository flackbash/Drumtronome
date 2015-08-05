// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    sealed class EditSpeedTemplateHud : CreateSpeedTemplateHud
    {
        private int mTemplateIndex;

        public EditSpeedTemplateHud(ScreenManager screenManager, Metronome metronome, Rectangle rect) : base(screenManager, metronome, rect)
        {
            // Initialize basic buttons "Ok" and "Close" that should be displayed on every HUD.
            var tmpButtons = new Button[mButtons.Length + 1];
            tmpButtons[0] = mButtons[0];
            tmpButtons[1] = mButtons[1];
            tmpButtons[2].mTexName = "Delete";
            tmpButtons[2].mState = OwnButtonState.Normal;
            tmpButtons[2].mAction = Delete;
            tmpButtons[2].mTextures = new Texture2D[3];
            tmpButtons[2].mRectangle = new Rectangle(mRectangle.Width + 5, mRectangle.Height + 5, 32, 32);
            mButtons = tmpButtons;

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            // Load delete button
            string[] states = { "Normal", "Hot", "Pressed" };

            for (var j = 0; j < states.Length; j++)
            {
                mButtons[2].mTextures[j] = content.Load<Texture2D>("Textures/Buttons/" + mButtons[2].mTexName + states[j]);
            }
        }

        internal void SetTemplate(int index)
        {
            mTemplateIndex = index;
            mTextBoxes[0].SetText(mMetronome.mSpeedTemplates[mTemplateIndex].mName);
            mTextBoxes[1].SetText(mMetronome.mSpeedTemplates[mTemplateIndex].ToString());
        }

        protected override void Ok()
        {
            if (mTextBoxes != null)
            {
                foreach (var tbox in mTextBoxes)
                {
                    tbox.RemoveFocus();
                }
                if (StringToTemplate(mTextBoxes[1].GetText()))
                {
                    mName = mTextBoxes[0].GetText();
                    mMetronome.mSpeedTemplates[mTemplateIndex] = new SpeedTemplate(mName, mBars.ToArray(), mTempo.ToArray(), mRepeat, mString, mMetronome);
                    foreach (var tbox in mTextBoxes)
                    {
                        tbox.Clear();
                    }
                }
            }
            mScreenManager.StageScreenForRemoval();
 
        }

        protected override void Close()
        {
            foreach (var tbox in mTextBoxes)
            {
                tbox.RemoveFocus();
            }
            mScreenManager.StageScreenForRemoval();
        }

        private void Delete()
        {
            mMetronome.mSpeedTemplates.RemoveAt(mTemplateIndex);
            mScreenManager.StageScreenForRemoval();
        }
    }
}
