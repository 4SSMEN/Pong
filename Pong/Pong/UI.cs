using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    public class UI
    {

        Vector2 p1_scorePosition, p2_scorePosition;     //Score positions

        Vector2 start_position, win_position;           //Win text position

        SpriteFont hud_font, win_font;                  //Fonts

        public void Initialize()
        {
            p1_scorePosition = new Vector2(Constants._WIDTH * Constants._SIZE - 7 * Constants._SIZE, 7 * Constants._SIZE);
            p2_scorePosition = new Vector2(7 * Constants._SIZE, 7 * Constants._SIZE);

            start_position = new Vector2(Constants._WIDTH * Constants._SIZE / 2, Constants._HEIGHT * Constants._SIZE / 2);
            win_position = new Vector2(Constants._WIDTH * Constants._SIZE / 2, Constants._HEIGHT * Constants._SIZE / 2);
        }

        public  void LoadContent(ContentManager content)
        {
            hud_font = content.Load<SpriteFont>("Fonts\\HUD" + Constants._SIZE);
            win_font = content.Load<SpriteFont>("Fonts\\WIN" + Constants._SIZE);

            p1_scorePosition.X -= hud_font.MeasureString("Player1 Score: 11").X / 2;

            start_position.X -= win_font.MeasureString("Player 1    W-S \nPlayer 2   UP-Down").X / 4;
            start_position.Y -= win_font.MeasureString("a\na").Y / 2;

            win_position.X -= win_font.MeasureString("Player 1 Wins").X / 4;
        }

        public void Draw(SpriteBatch spriteBatch, State state, int p1_score, int p2_score)
        {
            switch (state)
            {
                case State.Start:
                    spriteBatch.DrawString(win_font, "Player 1    W-S \nPlayer 2   UP-Down", start_position, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(hud_font, "\n\n\n\n\npress Enter to continue", start_position, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    break;

                case State.Game:
                    spriteBatch.DrawString(hud_font, "Player 1  Score: " + p1_score, p1_scorePosition, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(hud_font, "Player 2  Score: " + p2_score, p2_scorePosition, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    break;

                case State.End:
                    spriteBatch.DrawString(win_font, "Player " + ((p1_score > p2_score) ? 1 : 2) + "  Wins", win_position, Color.White, 0, new Vector2(), 0.5f, SpriteEffects.None, 0);
                    break;
            }
        }
    }
}
