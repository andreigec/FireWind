using System.Windows.Forms;
using Project;

namespace FireWindServer
{
    public partial class GameWindow : UserControl
    {
        public static int maxCount = 100;
        public ServerWindow parentServer;
        public Sector sec;

        public GameWindow(Sector s, ServerWindow parent)
        {
            InitializeComponent();
            sec = s;
            parentServer = parent;
        }
    }
}