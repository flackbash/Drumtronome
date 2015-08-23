// Copyright (C) 2015 Natalie Prange, flack2bash_at_gmail_dot_com.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Metronome.Screens
{
    sealed class SideBar : Hud
    {
        private readonly ContentManager mContent; 
        private readonly CreateRudimentHud mCreateRudimentHud;
        private readonly EditSpeedTemplateHud mEditSpeedTemplateHud;
        private readonly CreateSpeedTemplateHud mCreateSpeedTemplateHud;
        private bool mCheckForChanges;

        // Content
        private Texture2D[] mTextButtonTextures;

        public SideBar(ScreenManager screenManager, ContentManager content, Metronome metronome, Rectangle rect) : base(screenManager, metronome, rect)
        {
            mContent = content;
            // Initialize other screens
            const int width = 300;
            const int height = 200;
            var rectangle = new Rectangle(Game.mScreenWidth / 2 - width / 2, Game.mScreenHeight / 2 - height / 2, width, height);
            mCreateSpeedTemplateHud = new CreateSpeedTemplateHud(mScreenManager, mMetronome, rectangle);
            mCreateRudimentHud = new CreateRudimentHud(mScreenManager, mMetronome, rectangle);
            mEditSpeedTemplateHud = new EditSpeedTemplateHud(mScreenManager, mMetronome, rectangle);
       
            // Initialize Create buttons
            var labels = new[] {"New Template", "New Rudiment"};
            var actions = new MetronomeScreen.RunAction[] {CreateNewTemplate, CreateNewRudiment};
            mButtons = new Button[labels.Length + mMetronome.mSpeedTemplates.Count];
            for (var i = 0; i < labels.Length; i++)
            {
                mButtons[i].mLabel = labels[i];
                mButtons[i].mTexName = "TextButton";
                mButtons[i].mState = OwnButtonState.Normal;
                mButtons[i].mAction = actions[i];
                mButtons[i].mTextures = new Texture2D[3];
            }
            mButtons[0].mRectangle = new Rectangle(mRectangle.X + 10, mRectangle.Y + 40 + mMetronome.mSpeedTemplates.Count*30, 100, 25);
            mButtons[1].mRectangle = new Rectangle(mRectangle.X + 10, mButtons[0].mRectangle.Y + 80 + mMetronome.mRudiments.Count*30, 100, 25);
            // Initialize Speed Template buttons
            for (var i = 2; i - 2 < mMetronome.mSpeedTemplates.Count; i++)
            {
                mButtons[i].mLabel = mMetronome.mSpeedTemplates[i - 2].mName;
                mButtons[i].mTexName = "TextButtonBlue";
                mButtons[i].mState = OwnButtonState.Normal;
                mButtons[i].mParameterAction = StartTemplate;
                mButtons[i].mParameterActionRight = EditTemplate;
                mButtons[i].mParameter = i - 2;
                mButtons[i].mTextures = new Texture2D[3];
                mButtons[i].mRectangle = new Rectangle(10, 40 + (i - 2) * 30, 100, 20);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            // Load text button images for buttons that might be added later
            string[] states = {"Normal", "Hot", "Pressed"};
            mTextButtonTextures = new Texture2D[states.Length];
            for (var i = 0; i < states.Length; i++)
            {
                mTextButtonTextures[i] = content.Load<Texture2D>("Textures/Buttons/TextButtonBlue" + states[i]);
            }

            mCreateRudimentHud.LoadContent(content);
            mCreateSpeedTemplateHud.LoadContent(content);
            mEditSpeedTemplateHud.LoadContent(content);
        }

        public override void Update(GameTime gameTime, Input mouseInput, Input keyboardInput)
        {
            if (mCheckForChanges)
            {
                // Update the position of the buttons, in case a template or rudiment was added
                mButtons[0].mRectangle = new Rectangle(mRectangle.X + 10, mRectangle.Y + 40 + mMetronome.mSpeedTemplates.Count * 30, 100, 25);
                mButtons[1].mRectangle = new Rectangle(mRectangle.X + 10, mButtons[0].mRectangle.Y + 80 + mMetronome.mRudiments.Count * 30, 100, 25);

                // Update the buttons
                var tmpButtons = new Button[mMetronome.mSpeedTemplates.Count + 2];
                tmpButtons[0] = mButtons[0];
                tmpButtons[1] = mButtons[1];
                for (var i = 2; i - 2 < mMetronome.mSpeedTemplates.Count; i++)
                {
                    tmpButtons[i].mLabel = mMetronome.mSpeedTemplates[i - 2].mName;
                    tmpButtons[i].mTexName = "TextButtonBlue";
                    tmpButtons[i].mState = OwnButtonState.Normal;
                    tmpButtons[i].mParameterAction = StartTemplate;
                    tmpButtons[i].mParameterActionRight = EditTemplate;
                    tmpButtons[i].mParameter = i - 2;
                    tmpButtons[i].mTextures = mTextButtonTextures;
                    tmpButtons[i].mRectangle = new Rectangle(10, 40 + (i - 2) * 30, 100, 20);
                }
                mButtons = tmpButtons;
                mCheckForChanges = false;
            }

            base.Update(gameTime, mouseInput, keyboardInput);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.DrawString(mFont, "Speed Templates:", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(mFont, "Rudiments:", new Vector2(10, mButtons[0].mRectangle.Y + 50), Color.White);
        }

        public override bool UpdateLower()
        {
            return true;
        }

        private void CreateNewTemplate()
        {
            mScreenManager.StageScreenForAdding(mCreateSpeedTemplateHud);
            mCheckForChanges = true;
        }

        private void CreateNewRudiment()
        {

        }

        private void StartTemplate(int template)
        {
            if (mMetronome.mPlayTemplate)
            {
                mScreenManager.StageScreenForRemoval();
            }

            mMetronome.mSpeedTemplates[template].StartTemplate();

            var speedTemplateHud = new SpeedTemplateHud(mScreenManager, mMetronome);
            speedTemplateHud.LoadContent(mContent);
            mScreenManager.StageScreenForAdding(speedTemplateHud);

        }

        private void EditTemplate(int template)
        {
            mEditSpeedTemplateHud.SetTemplate(template);
            mScreenManager.StageScreenForAdding(mEditSpeedTemplateHud);
            mCheckForChanges = true;
        }

        protected override void Ok(){}

        protected override void Close() {}
    }
}
