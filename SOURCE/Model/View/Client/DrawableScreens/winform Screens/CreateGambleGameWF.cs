using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Instances;
using Project.Model.mapInfo;
using Project.Networking;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;
using Color = System.Drawing.Color;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class CreateGambleGameWF : Form, IScreenControls
    {
        public static XNA_WF_Wrapper XnaWfWrapperInstance = null;
        private int CurrShipCost = -1;
        private int CurrShipCount = -1;

        private int MaxShipCost = -1;
        private int MinShipCost = -1;
        private int PlayerCredits = -1;

        public CreateGambleGameWF()
        {
            InitializeComponent();
            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            Init();
        }

        #region IScreenControls Members

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            Control c = XnaWfWrapperInstance.MouseUpdate(gt);
            if (c == null)
                return;

            if (mc.ButtonsPressed() == false)
                return;

            if (c == backbutton)
            {
                GameControlClient.ShowChooseGameModeScreen();
            }
            else if (c == creategamebutton)
            {
                var gic = new GameInitConfig();
                SectorConfig sc = new SectorConfigGambleMatch(SectorConfig.Size.medium, CurrShipCount, CurrShipCost);
                GameControlClient.CreateGame(gic, sc);
            }
            else if (c is TrackBar)
            {
                CopyLabels();
            }
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
        }

        public void Draw(Camera2D cam, GameTime gameTime)
        {
            XnaWfWrapperInstance.Draw();
        }

        #endregion

        public void Init()
        {
            MinShipCost = (int) ShipInstanceShell.GetMinCostShip();
            MaxShipCost = (int) ShipInstanceShell.GetMaxCostShip();
            PlayerCredits = GameControlClient.playerShipClass.Credits;

            gambletrackbar.Minimum = MinShipCost;
            gambletrackbar.Maximum = MaxShipCost;
            CurrShipCost = MinShipCost;
            gambletrackbar.Value = CurrShipCost;

            shipcounttrackbar.Minimum = 1;
            shipcounttrackbar.Maximum = (int) Math.Ceiling(PlayerCredits/(double) MinShipCost);
            CurrShipCount = 1;
            shipcounttrackbar.Value = CurrShipCount;

            CopyLabels();
        }

        private void CopyLabels()
        {
            minlabel.Text = MinShipCost.ToString();
            maxlabel.Text = MaxShipCost.ToString();
            playercreditslabel.Text = PlayerCredits.ToString();

            CurrShipCost = gambletrackbar.Value;
            gamblecost.Text = CurrShipCost.ToString();

            CurrShipCount = shipcounttrackbar.Value;
            shipcountlabel.Text = CurrShipCount.ToString();

            int tc = (CurrShipCost*CurrShipCount);
            totalcostlabel.Text = tc.ToString();

            //info
            const float basemod = 1.1f;
            var basebonus = (int) (CurrShipCost*basemod);
            destroybonuslabel.Text = basebonus.ToString();

            double gambperc = Shared.mapRange(CurrShipCost, MinShipCost, MaxShipCost, 1f, 2f);
            float diff = CurrShipCount*(float) (gambperc);
            difficultylabel.Text = ((int) diff).ToString();

            var diffcap = (int) diff;
            if (diffcap > 10)
                diffcap = 10;
            if (diffcap < 1)
                diffcap = 1;

            var colourp = (float) Shared.mapRange(diffcap, 1, 10, 0, 1);
            var inv = (float) Shared.mapRange(diffcap, 1, 10, 1, 0);

            var c = Color.FromArgb((int) (255*colourp), (int) (255*inv), 0);

            difficultytextlabel.ForeColor = c;
            difficultylabel.ForeColor = c;

            if (diff < 2)
                difficultytextlabel.Text = "Beginner";
            else if (diff < 4)
                difficultytextlabel.Text = "Easy";
            else if (diff < 6)
                difficultytextlabel.Text = "Medium";
            else if (diff < 8)
                difficultytextlabel.Text = "Hard";
            else if (diff < 10)
                difficultytextlabel.Text = "Extra Hard";
            else
                difficultytextlabel.Text = "Pro";

            var modbonus = (int) (basebonus*diff);
            difficultybonuslabel.Text = modbonus.ToString();
            int totalbonus = modbonus*CurrShipCount;
            totalbonuslabel.Text = totalbonus.ToString();

            creategamebutton.Enabled = tc <= PlayerCredits;
        }
    }
}