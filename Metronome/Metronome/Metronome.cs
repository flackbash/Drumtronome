// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Metronome
{
    [Serializable()]
    sealed class Metronome
    {
        // General Properties
        internal int mTempo;
        internal float mMillisecondsBetweenBeats;
        internal int mBeatsPerBar;
        internal int mStopAfter;
        internal int mIncreaseAfter;
        internal int mIncreaseBy;
        internal int mIncreaseUpTo;
        internal int mStopCounter;
        internal int mIncreaseCounter;
        private int mTickCounter;
        internal int mTotalBarCounter;
        private readonly int mUpperTempoLimit;

        internal bool mDoesStop;
        internal bool mDoesIncrease;
        internal PlayState mPlayState;
        internal SoundType mSoundToPlay;

        // Speed Template stuff
        internal List<SpeedTemplate> mSpeedTemplates;
        internal bool mPlayTemplate;
        internal SpeedTemplate mCurrentTemplate;

        // Rudiment stuff
        internal List<Rudiment> mRudiments; 

        // Content
        [NonSerialized]
        private SoundEffect mTickHigh;
        [NonSerialized]
        private SoundEffect mTickLow;
        [NonSerialized]
        private SoundEffect mTickAttention;
        [NonSerialized]
        private SoundEffect mTickSpeedIncrease;

        internal TimeSpan mPlayTimer;

        public enum PlayState
        {
            Playing, Paused, Stopped
        }

        internal enum SoundType
        {
            High, Low, Attention, SpeedIncrease, None
        }

        public Metronome()
        {
            mTempo = 100;
            mBeatsPerBar = 4;
            mMillisecondsBetweenBeats = 60000f / mTempo;
            mStopAfter = 100;
            mIncreaseAfter = 16;
            mIncreaseBy = 10;
            mIncreaseUpTo = 200;
            mTickCounter = 0;
            mPlayState = PlayState.Stopped;
            mPlayTimer = TimeSpan.Zero;
            mUpperTempoLimit = 500;
            mSpeedTemplates = new List<SpeedTemplate>();
            mRudiments = new List<Rudiment>();
        }

        public void LoadContent(ContentManager content)
        {
            // Load sounds
            mTickHigh = content.Load<SoundEffect>("Sounds/PingHigh");
            mTickLow = content.Load<SoundEffect>("Sounds/PingLow");
            mTickAttention = content.Load<SoundEffect>("Sounds/Attention");
            mTickSpeedIncrease = content.Load<SoundEffect>("Sounds/SpeedIncrease");
        }

        public void AddTemplate(SpeedTemplate template)
        {
            mSpeedTemplates.Add(template);
        }

        public void SetTempo(int tempo)
        {
            mTempo = tempo;
            if (mTempo > mUpperTempoLimit) mTempo = mUpperTempoLimit;
            mMillisecondsBetweenBeats = 60000f / mTempo;
        }

        public void ChangeTempoBy(int increase)
        {
            mTempo += increase;
            if (mTempo > mUpperTempoLimit) mTempo = mUpperTempoLimit;
            mMillisecondsBetweenBeats = 60000f / mTempo;
        }

        public void SetIncreaseValues(int increaseBy, int increaseAfter, int increaseUpTo)
        {
            mIncreaseBy = increaseBy;
            mIncreaseAfter = increaseAfter;
            mIncreaseUpTo = increaseUpTo;
        }

        public void ToggleAutoIncrease()
        {
            mDoesIncrease = !mDoesIncrease;
            mIncreaseCounter = 0;
        }

        public void SetStopValues(int stopAfter)
        {
            mStopAfter = stopAfter;            
        }

        public void ToggleAutoStop()
        {
            mDoesStop = !mDoesStop;
            mStopCounter = 0;
        }

        public void Resume()
        {
            mPlayState = PlayState.Playing;
        }

        public void Pause()
        {
            mPlayState = PlayState.Paused;
        }

        public void Stop()
        {
            mPlayTimer = TimeSpan.Zero;
            mPlayState = PlayState.Stopped;
            mTickCounter = 0;
            mIncreaseCounter = 0;
            mStopCounter = 0;
            mTotalBarCounter = 0;
            mPlayTemplate = false;
        }

        public void Update(GameTime gameTime)
        {
            if (mPlayState == PlayState.Playing)
            {
                mPlayTimer += gameTime.ElapsedGameTime;
                if (mPlayTimer.TotalMilliseconds >= mMillisecondsBetweenBeats)
                {
                    mSoundToPlay = SoundType.Low;
                    if (mTickCounter % mBeatsPerBar == 0)
                    {
                        mSoundToPlay = SoundType.High;
                        mTotalBarCounter++;

                        // Play Template if on
                        if (mPlayTemplate)
                        {
                            mCurrentTemplate.Update();
                        }
                        else
                        {
                            AutoStop();
                            AutoIncrease();
                        }
                    }
                    if (mPlayState == PlayState.Playing)
                    {
                        Play(mSoundToPlay);
                        mTickCounter++;
                        mPlayTimer = TimeSpan.Zero;
                    }
                }
            }
        }

        private void AutoStop()
        {
            if (mDoesStop)
            {
                if (mStopCounter >= mStopAfter)
                {
                    Stop();
                    mStopCounter = 0;
                    mSoundToPlay = SoundType.None;
                }
                else
                {
                    mStopCounter++;
                }
            }
        }

        private void AutoIncrease()
        {
            if (mDoesIncrease && mTempo < mIncreaseUpTo)
            {
                if (mIncreaseCounter >= mIncreaseAfter)
                {
                    ChangeTempoBy(mIncreaseBy);
                    mIncreaseCounter = 0;
                    mSoundToPlay = SoundType.SpeedIncrease;
                }
                mIncreaseCounter++;
            }
        }

        private void Play(SoundType sound)
        {
            switch (sound)
            {
                case SoundType.SpeedIncrease:
                    mTickSpeedIncrease.Play();
                    break;
                case SoundType.Attention:
                    mTickAttention.Play();
                    break;
                case SoundType.High:
                    mTickHigh.Play();
                    break;
                case SoundType.Low:
                    mTickLow.Play();
                    break;
            }
        }
    }
}
