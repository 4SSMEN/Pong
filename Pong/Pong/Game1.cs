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
    public enum State { Start, Game, End };

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int ball_x, ball_y, ball_w, ball_h;             //Ball position and dimensions
        float ball_f;                                   //Ball Y position as float

        int p1_x, p1_y;                                 //Player 1 position
        int p2_x, p2_y;                                 //Player 2 position
        int p_w, p_h;                                   //Player dimensions

        Texture2D ball_tex, p_tex;                      //Ball and Player texture

        Rectangle ball_hit_box;
        Rectangle p1_hit_box;                           //Ball and Players hit boxes
        Rectangle p2_hit_box;

        float dir_x, dir_y;
        bool right;

        int p1_score, p2_score;                         //Score 

        SoundEffect padel_hit, player_scored, wall_hit; //SFX

        UI gameUI;

        public State GameState { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Constants._WIDTH * Constants._SIZE,
                PreferredBackBufferHeight = Constants._HEIGHT * Constants._SIZE
            };

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            ball_w = 4 * Constants._SIZE;
            ball_h = 4 * Constants._SIZE;
            ball_x = Constants._WIDTH * Constants._SIZE / 2 - ball_w / 2;
            ball_y = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;
            ball_f = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;

            p_w = 4 * Constants._SIZE;
            p_h = 16 * Constants._SIZE;

            p1_x = Constants._WIDTH * Constants._SIZE - p_w;
            p1_y = Constants._HEIGHT * Constants._SIZE / 2 - p_h / 2;

            p2_x = 0;                                                           //Assigning values to all the variables
            p2_y = Constants._HEIGHT * Constants._SIZE / 2 - p_h / 2;

            ball_tex = new Texture2D(GraphicsDevice, ball_w, ball_h);
            p_tex = new Texture2D(GraphicsDevice, p_w, p_h);

            ball_hit_box = new Rectangle(ball_x, ball_y, ball_w, ball_h);
            p1_hit_box = new Rectangle(p1_x, p1_y, p_w, p_h);
            p2_hit_box = new Rectangle(p2_x, p2_y, p_w, p_h);

            dir_x = 1;
            dir_y = 0;

            right = false;

            gameUI = new UI();

            gameUI.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            int ball_tex_size = ball_w * ball_h;
            int p_tex_size = p_w * p_h;

            Color[] ball_data = new Color[ball_tex_size];
            Color[] p_data = new Color[p_tex_size];                                         //Creating the textures

            for (int i = 0; ++i <= ball_tex_size; ball_data[i - 1] = Color.White) { }
            for (int i = 0; ++i <= p_tex_size; p_data[i - 1] = Color.White) { }

            ball_tex.SetData(ball_data);
            p_tex.SetData(p_data);

            padel_hit = Content.Load<SoundEffect>("SFX\\Padel Hit");
            player_scored = Content.Load<SoundEffect>("SFX\\Player Scored");
            wall_hit = Content.Load<SoundEffect>("SFX\\Wall Hit");

            gameUI.LoadContent(Content);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
                Exit();

            switch (GameState)
            {
                case State.Start:
                    if (keyState.IsKeyDown(Keys.Enter))
                        GameState = State.Game;
                    break;

                case State.Game:
                    BallMove(Constants._SIZE);
                    PlayerMove(Constants._SIZE * 2);

                    Score();

                    if (p1_score == Constants._WINSCORE || p2_score == Constants._WINSCORE)
                        GameState = State.End;

                    if (keyState.IsKeyDown(Keys.Space))
                        ResetBall();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (GameState)
            {
                case State.Game:
                case State.End:
                    spriteBatch.Draw(ball_tex, ball_hit_box, Color.White);
                    spriteBatch.Draw(p_tex, p1_hit_box, Color.White);
                    spriteBatch.Draw(p_tex, p2_hit_box, Color.White);
                    break;
            }

            gameUI.Draw(spriteBatch, GameState, p1_score, p2_score);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void BallMove(int speed)
        {
            ball_x += (int)(speed * dir_x);
            ball_f += speed * dir_y;
            ball_y = (int)ball_f;

            ball_hit_box.X = ball_x;
            ball_hit_box.Y = ball_y;

            if ((!right && ball_hit_box.Intersects(p1_hit_box)) || (right && ball_hit_box.Intersects(p2_hit_box)))
            {
                padel_hit.Play();

                right = !right;
                dir_x = -dir_x;

                int p_y = p1_y;
                if (!right)
                    p_y = p2_y;

                float delta = ((ball_y + ball_h / 2) - (p_y + p_h / 2));
                dir_y = delta / p_h * 5;
            }

            if (ball_y <= 0 || ball_y >= Constants._HEIGHT * Constants._SIZE - ball_h)
            {
                wall_hit.Play();
                dir_y = -dir_y;
            }
        }

        public void PlayerMove(int speed)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up) && p1_y > 0)
                p1_y -= speed;
            if (keyState.IsKeyDown(Keys.Down) && p1_y < Constants._HEIGHT *Constants._SIZE - p_h)
                p1_y += speed;

            if (keyState.IsKeyDown(Keys.W) && p2_y > 0)
                p2_y -= speed;
            if (keyState.IsKeyDown(Keys.S) && p2_y < Constants._HEIGHT * Constants._SIZE - p_h)
                p2_y += speed;

            p1_hit_box.Y = p1_y;
            p2_hit_box.Y = p2_y;
        }

        public void Score()
        {
            if (ball_x >= Constants._WIDTH * Constants._SIZE - p_w  || ball_x <= 0)
            {
                player_scored.Play();

                if (ball_x >= Constants._WIDTH * Constants._SIZE - p_w)
                    p2_score++;
                else if (ball_x <= 0)
                    p1_score++;

                ResetBall();
            }
        }

        public void ResetBall()
        {
            ball_x = Constants._WIDTH * Constants._SIZE / 2 - ball_w / 2;
            ball_y = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;
            ball_f = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;

            dir_y = 0;

            p1_y = p2_y = Constants._HEIGHT * Constants._SIZE / 2 - p_h / 2;
        }
    }
}
