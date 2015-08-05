// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    class CreateSpeedTemplateHud : Hud
    {
        protected List<int> mBars;
        protected List<int> mTempo;
        protected bool mRepeat;
        protected string mName;
        protected string mString;

        public CreateSpeedTemplateHud(ScreenManager screenManager, Metronome metronome, Rectangle rect) : base(screenManager, metronome, rect)
        {
            // Initialize Texts
            mTexts = new[,] {{"Name", ""}, {"Speed Template", ""}};
            mTextPositions = new Vector2[2];
            mTextPositions[0] = new Vector2(mRectangle.X + 10, mRectangle.Y + 70);
            mTextPositions[1] = new Vector2(mRectangle.X + 10, mRectangle.Y + 110);

            // Initialize TextBoxes
            mTextBoxes = new TextBox[2];
            mTextBoxes[0] = new TextBox(new Rectangle(mRectangle.X + (mRectangle.Width - 160), (int)mTextPositions[0].Y, 150, 25), maxTextLength: 20);
            mTextBoxes[1] = new TextBox(new Rectangle(mRectangle.X + 10, (int)mTextPositions[1].Y + 30, (mRectangle.Width - 20), 25), maxTextLength: 150);
            mTextBoxes[1].UseSmallFont(true);
            mTextBoxes[0].SetDefaultText("new template");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw a shader over underlying layers
            spriteBatch.Draw(mBackground, new Rectangle(0, 0, Game.mScreenWidth, Game.mScreenHeight), Color.White * 0.25f);

            base.Draw(spriteBatch);
        }

        protected bool StringToTemplate(string template)
        {
            if (template == "") return false;
            var statementCount = template.Count(x => x == '.');
            mBars = new List<int>();
            mTempo = new List<int>();
            mRepeat = false;
            mString = "";
            var statementNum = 0;
            var seperator = 0;
            var bars = "";
            var tempo = "";
            var times = "";
            for (var i = 0; i < template.Length; i++)
            {
                char c = template[i];
                if (i == 0 && !(c == 'r' || c > 47 || c < 58) || statementNum >= statementCount)
                {
                    return false;
                }
                if (i == 0 && c == 'r')
                {
                    mRepeat = true;
                }
                else if (template[i] == '.')
                {
                    if (bars == "" || tempo == "") return false;

                    // Set template values
                    mBars.Add(int.Parse(bars));
                    mTempo.Add(int.Parse(tempo));
                    if (times != "")
                    {
                        for (var j = 0; j < int.Parse(times) - 1; j++)
                        {
                            mBars.Add(int.Parse(bars));
                            mTempo.Add(int.Parse(tempo));
                        }
                    }

                    // Set loop values
                    statementNum++;
                    seperator = 0;
                    bars = "";
                    tempo = "";
                    times = "";
                }
                else if (c == '-')
                {
                    if (bars == "" || (tempo != "" && times != "" )) return false;
                    if (tempo == "") seperator = 1;
                    else if (times == "") seperator = 2;
                }
                else if (c > 47 && c < 58)
                {
                    if (seperator == 0)
                    {
                        bars += c;
                    }
                    else if (seperator == 1)
                    {
                        tempo += c;
                    }
                    else if (seperator == 2)
                    {
                        times += c;
                    }
                }
            }
            mString = template;
            return true;
        }

        protected override void Ok()
        {
            base.Ok();
            if (StringToTemplate(mTextBoxes[1].GetText()))
            {
                mName = mTextBoxes[0].GetText();
                mMetronome.AddTemplate(new SpeedTemplate(mName, mBars.ToArray(), mTempo.ToArray(), mRepeat, mString, mMetronome));
                foreach (var tbox in mTextBoxes)
                {
                    tbox.Clear();
                }
            }
        }

        protected override void Close()
        {
            base.Close();
            foreach (var tbox in mTextBoxes)
            {
                tbox.Clear();
            }
        }
    }
}
