// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using Microsoft.Xna.Framework;

namespace Metronome.Screens
{
    sealed class CreateRudimentHud : Hud
    {
        public CreateRudimentHud(ScreenManager screenManager, Metronome metronome, Rectangle rect) : base(screenManager, metronome, rect)
        {
            mButtons = new Button[0];
        }
    }
}
