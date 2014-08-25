using System.Drawing;
using System.Windows.Forms;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class PurchasableItemWFControlItem : UserControl
    {
        public PurchasableItemWFControlItem()
        {
            InitializeComponent();
        }

        public PurchasableItemWFControlItem(int col1X, int col2X)
        {
            Controls.Clear();
            InitializeComponent();

            equippeditemstat.Location = new Point(col1X, 0);
            newitemstat.Location = new Point(col2X, 0);
        }
    }
}