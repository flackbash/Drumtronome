// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    class SpeedTemplateHud : IScreen
    {
        public void LoadContent(ContentManager content)
        {
        }

        public void Update(GameTime gameTime, Input mouseInput, Input keyboardInput)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public bool IsInScreen(int x, int y)
        {
            return false;
        }

        public bool UpdateLower()
        {
            return true;
        }

        public bool DrawLower()
        {
            return true;
        }
    }
}
