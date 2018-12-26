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
    public class Game1 : Game
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected ContentManager contentManager;
        bool pause; 
        int ball_x, ball_y, ball_w, ball_h;   //Ball position and dimensions
        int p1_x, p1_y;                       //Player 1 position
        int p2_x, p2_y;                       //Player 2 position
        int p_w, p_h;                         //Player dimensions
        float ball_f;
        int rscore , lscore ;                   //Score 
        Texture2D ball_tex, p_tex;            //Ball and Player texture

        Rectangle ball_hit_box;
        Rectangle p1_hit_box;                 //Ball and Players hit boxes
        Rectangle p2_hit_box;

        float dir_x, dir_y;
        bool right;
        SpriteFont HUDfont;
        SpriteFont Winfont;
        Vector2 lsPosition;
        Vector2 rsPosition;
        Vector2 winPosition; 
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = Constants._WIDTH * Constants._SIZE,
                PreferredBackBufferHeight = Constants._HEIGHT * Constants._SIZE
            };
            Content.RootDirectory = "Content";
            contentManager = Content;
        }

        protected override void Initialize()
        {
            pause = false; 
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
            rscore = 0;
            lscore =0;
            rsPosition.X = 30;
            rsPosition.Y = 30 ;
            lsPosition.X = (Constants._WIDTH * Constants._SIZE - p_w) - 2*Constants._WIDTH;
            lsPosition.Y = 30;
            winPosition.X= Constants._WIDTH * Constants._SIZE / 2 - ball_w / 2 - Constants._WIDTH;
            winPosition.Y = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2 -10 ;
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
            HUDfont = Content.Load<SpriteFont>("HUDfont");
            Winfont = Content.Load<SpriteFont>("Winfont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (!pause) { 
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            BallMove(Constants._SIZE);

            PlayerMove(Constants._SIZE * 2);
            Score();
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Space))
            {
                ball_x = Constants._WIDTH * Constants._SIZE / 2 - ball_w / 2;
                ball_y = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;
                ball_f = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;
                dir_x = 1;
                dir_y = 0;
                right = false;
            }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter)) Exit();
                else pause = true; 
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(ball_tex, ball_hit_box, Color.White);
            spriteBatch.Draw(p_tex, p1_hit_box, Color.White);
            spriteBatch.Draw(p_tex, p2_hit_box, Color.White);
            spriteBatch.DrawString(HUDfont, "Player 1    Score : "+rscore.ToString(), rsPosition, Color.White);
            spriteBatch.DrawString(HUDfont, "Player 2    Score : " + lscore.ToString(), lsPosition, Color.White);
            if (pause)
            {   
                if(rscore>lscore)
                  spriteBatch.DrawString(Winfont, "Player 1    Wins " , winPosition, Color.White);
                else
                  spriteBatch.DrawString(Winfont, "Player 2    Wins ", winPosition, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void BallMove(int speed)
        {
            ball_x += (int)(speed * dir_x);
            ball_f += speed * dir_y;
            ball_y = (int)ball_f;

            //TODO: Make the ball move at an angle

            ball_hit_box.X = ball_x;
            ball_hit_box.Y = ball_y;

            if ((!right && ball_hit_box.Intersects(p1_hit_box)) || (right && ball_hit_box.Intersects(p2_hit_box)))
            {
                right = !right;
                dir_x = -dir_x;
                int p_y = p1_y;
                if (!right)
                    p_y = p2_y;
                float delta = ((ball_y + ball_h / 2) - (p_y + p_h / 2));
                dir_y = delta / p_h * 5;
                Debug.Print(dir_y + "");
            }

            if (ball_y <= 0 || ball_y >= Constants._HEIGHT * Constants._SIZE - ball_h)
                dir_y = -dir_y;

        }

        public void PlayerMove(int speed)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up))
            {
                //if (!right)
                    p1_y -= speed;
                //else p2_y -= speed;
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                //if (!right)
                    p1_y += speed;
                //else p2_y += speed;
            }
            if (keyState.IsKeyDown(Keys.W))
            {
                //if (!right)
                    //p1_y -= speed;
               // else
                 p2_y -= speed;
            }

            if (keyState.IsKeyDown(Keys.S))
            {
                //if (!right)
                //p1_y += speed;
                //else
                p2_y += speed;
            }
            p1_hit_box.Y = p1_y;
            p2_hit_box.Y = p2_y;
        }
        public void Score()
        {
            if (ball_x > (Constants._WIDTH * Constants._SIZE - p_w + 4 * Constants._SIZE) || ball_x < -1 * (4 * Constants._SIZE))
            {
                if (ball_x > Constants._WIDTH * Constants._SIZE - p_w) rscore++;
                if (ball_x < 0) lscore++;
                if (lscore == 11 || rscore == 11) pause = true;
                ball_x = Constants._WIDTH * Constants._SIZE / 2 - ball_w / 2;
                ball_y = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;
                ball_f = Constants._HEIGHT * Constants._SIZE / 2 - ball_h / 2;
                dir_x = -dir_x;
                dir_y = 0;
                right = !right ;
                p1_x = Constants._WIDTH * Constants._SIZE - p_w;
                p1_y = Constants._HEIGHT * Constants._SIZE / 2 - p_h / 2;
                p2_x = 0;                                                         
                p2_y = Constants._HEIGHT * Constants._SIZE / 2 - p_h / 2;
                
                Debug.Print(rscore + " " + lscore + " ");
            }


        }
    }
}
