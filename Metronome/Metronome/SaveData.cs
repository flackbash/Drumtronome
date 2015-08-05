// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using System;
using System.Collections.Generic;

namespace Metronome
{
    [Serializable()]
    sealed class SaveData
    {
        [NonSerialized]
        private static SaveData sInstance;

        // General Properties
        private int mStopAfter;
        private int mIncreaseAfter;
        private int mIncreaseBy;
        private int mIncreaseUpTo;
        // Speed Template stuff
        private List<SpeedTemplate> mSpeedTemplates;
        // Rudiment stuff
        private List<Rudiment> mRudiments; 

        public static SaveData GetInstance()
        {
            return sInstance ?? (sInstance = new SaveData());
        }

        public Metronome Load(string filename)
        {
            Metronome metronome = new Metronome();
            var serializer = new Serializer();
            var s = serializer.DeSerializeObject(filename);
            if (s == null) return null;

            metronome.mStopAfter = s.mStopAfter;
            metronome.mIncreaseAfter = s.mIncreaseAfter;
            metronome.mIncreaseBy = s.mIncreaseBy;
            metronome.mIncreaseUpTo = s.mIncreaseUpTo;
            metronome.mSpeedTemplates = s.mSpeedTemplates;
            metronome.mRudiments = s.mRudiments;

            foreach (SpeedTemplate template in s.mSpeedTemplates)
            {
                template.mMetronome = metronome;
            }

            return metronome;
        }

        public void Save(Metronome metronome, string filename)
        {
            mStopAfter = metronome.mStopAfter;
            mIncreaseAfter = metronome.mIncreaseAfter;
            mIncreaseBy = metronome.mIncreaseBy;
            mIncreaseUpTo = metronome.mIncreaseUpTo;
            mSpeedTemplates = metronome.mSpeedTemplates;
            mRudiments = metronome.mRudiments;

            Serializer serializer = new Serializer();
            serializer.SerializeObject(filename, this);
        }
    }
}
