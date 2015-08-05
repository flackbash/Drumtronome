// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;

namespace Metronome
{
    [Serializable()]
    sealed class SpeedTemplate
    {
        internal Metronome mMetronome;

        private readonly bool mRepeat;
        private readonly int[][] mStatements;  // 1.Number of Bars, 2. Tempo
        internal readonly string mName;
        private readonly string mString;
        private int mBarCounter;
        private int mStatementIndex;

        public SpeedTemplate(string name, int[] bars, int[] tempo, bool repeat, string str, Metronome metronome)
        {
            mMetronome = metronome;
            mRepeat = repeat;
            mName = name;
            mString = str;

            mStatements = new int[bars.Length][];
            for (var i = 0; i < mStatements.Length; i++)
            {
                mStatements[i] = new[] {bars[i],tempo[i]};
            }
        }

        public void StartTemplate()
        {
            mMetronome.Stop();
            mMetronome.Resume();
            if (mStatements != null && mStatements.Length > 0)
            {
                mBarCounter = 0;
                mStatementIndex = 0;
                mMetronome.SetTempo(mStatements[mStatementIndex][1]);
                mMetronome.mPlayTemplate = true;
                mMetronome.mCurrentTemplate = this;
            }
        }

        public void Update()
        {
            if (mStatements != null)
            {
                if (mBarCounter >= mStatements[mStatementIndex][0])
                {
                    mStatementIndex++;
                    if (mStatementIndex < mStatements.Length)
                    {
                        mMetronome.SetTempo(mStatements[mStatementIndex][1]);
                        mMetronome.mSoundToPlay = mStatements[mStatementIndex - 1][1] != mStatements[mStatementIndex][1] ? Metronome.SoundType.SpeedIncrease : Metronome.SoundType.Attention;
                        mBarCounter = 0;
                    }
                    else
                    {
                        if (mRepeat)
                        {
                            mStatementIndex = 0;
                            mBarCounter = 0;
                            mMetronome.SetTempo(mStatements[mStatementIndex][1]);
                            mMetronome.mSoundToPlay = mStatements[mStatements.Length - 1][1] != mStatements[mStatementIndex][1] ? Metronome.SoundType.SpeedIncrease : Metronome.SoundType.Attention;
                        }
                        else
                        {
                            mMetronome.mPlayTemplate = false;
                            mMetronome.Stop();
                        }
                    }
                }
                mBarCounter++;
            }
        }

        public new string ToString()
        {
            return mString;
        }
    }
}
