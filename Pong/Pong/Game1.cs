using System;
using System.Collections.Generic;
using System.Linq;
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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int ball_x, ball_y, ball_w, ball_h;   //Ball position and dimensions
        int p1_x, p1_y;                       //Player 1 position
        int p2_x, p2_y;                       //Player 2 position
        int p_w, p_h;                         //Player dimensions

        Texture2D ball_tex, p_tex;            //Ball and Player texture

        Rectangle ball_hit_box;
        Rectangle p1_hit_box;                 //Ball and Players hit boxes
        Rectangle p2_hit_box;

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

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            BallMove(4);

            PlayerMove(4);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(ball_tex, ball_hit_box, Color.White);
            spriteBatch.Draw(p_tex, p1_hit_box, Color.White);
            spriteBatch.Draw(p_tex, p2_hit_box, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void BallMove(int speed)
        {
            ball_x += speed;

            //TODO: Make the ball move at an angle

            ball_hit_box.X = ball_x;
            ball_hit_box.Y = ball_y;
        }

        public void PlayerMove(int speed)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Up))
            {
                p1_y -= speed;
            }

            if (keyState.IsKeyDown(Keys.Down))
            {
                p1_y += speed;
            }

            p1_hit_box.Y = p1_y;
        }
    }
}
