using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace CottonSFML
{
    public class Game
    {
        public Game()
        {

        }
        RenderWindow window;
        public void Run()
        {
            window = new RenderWindow(new VideoMode(800, 600), "CottonSFML", Styles.Close | Styles.Titlebar);
            window.Closed += Window_Closed;
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Blue);
                window.Display();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }
    }
}
