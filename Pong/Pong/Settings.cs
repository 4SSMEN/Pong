using System;
using System.Threading;
using System.Windows.Forms;

namespace Pong
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            Thread thread = new Thread(StartGame);
            thread.Start();
        }

        private void StartGame()
        {
            Constants._SIZE = (int)numericUpDown2.Value;
            Constants._WINSCORE = (int)numericUpDown1.Value;
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
