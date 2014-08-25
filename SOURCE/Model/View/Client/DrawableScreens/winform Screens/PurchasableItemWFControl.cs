using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Project.Model;
using Project.Model.Server.XML_Load;

namespace Project.View.Client.DrawableScreens.WPF_Screens
{
    public partial class PurchasableItemWFControl : UserControl
    {
        /// <summary>
        /// attributes which arent compared usefully
        /// </summary>
        private readonly Dictionary<Type, List<String>> ignoreAttributes;

        /// <summary>
        /// a list of types/field names where lower values are better than higher values
        /// </summary>
        private readonly Dictionary<Type, List<String>> reverseAttributes;

        private IPurchasable BuyPart;
        private IPurchasable EquippedPart;

        private int col1X;
        private int col2X;

        public PurchasableItemWFControl()
        {
            InitializeComponent();

            //items which are compared in revese
            reverseAttributes = new Dictionary<Type, List<string>>();
            var names = new List<string>();
            names.Add(Shared.GetPropertyName((weapon s) => s.EnergyPerShot));
            names.Add(Shared.GetPropertyName((weapon s) => s.CoolDown));
            reverseAttributes.Add(typeof (weapon), names);

            //items which arent usefully compared
            ignoreAttributes = new Dictionary<Type, List<string>>();
            names = new List<string>();
            names.Add(Shared.GetPropertyName((weapon s) => s.AngleOfFire));
            names.Add(Shared.GetPropertyName((weapon s) => s.IsBeam));
            ignoreAttributes.Add(typeof (weapon), names);
        }

        public bool Init(IPurchasable buyPart, IPurchasable equippedPart, bool hideCosts = false)
        {
            Controls.Clear();
            InitializeComponent();
            itemname.Text = "";
            typelabel.Text = "";
            costtext.Text = "";
            playermoney.Text = "";

            int cred = GameControlClient.playerShipClass.Credits;
            if (hideCosts == false)
            {
                costlabel.Text = Localisation.CostLabel;
                playermoneylabel.Text = Localisation.PlayerCreditLabel;
                playermoney.Text = cred.ToString();
            }

            if (buyPart == null && equippedPart == null)
            {
                itemname.Text = "No Item";
                return true;
            }

            if (buyPart != null && equippedPart != null && buyPart.GetType() != equippedPart.GetType())
            {
                return false;
            }

            BuyPart = buyPart;
            EquippedPart = equippedPart;

            if (BuyPart != null)
            {
                itemname.Text = BuyPart.name;
                typelabel.Text = BuyPart.GetType().Name;

                if (BuyPart.Cost <= cred)
                {
                    costtext.ForeColor = Color.Green;
                }
                else
                {
                    costtext.ForeColor = Color.Red;
                }

                costtext.Text = BuyPart.Cost.ToString();
            }

            col1X = 150;
            col2X = 250;

            int y = 0;
            AddItemRows(ref y);
            AddHeaderRow(ref y);

            if (hideCosts)
            {
                costlabel.Text = "";
                costtext.Text = "";
                playermoney.Text = "";
                playermoneylabel.Text = "";
            }

            return true;
        }


        public void AddHeaderRow(ref int yvalue)
        {
            var pic = new PurchasableItemWFControlItem(col1X, col2X);
            pic.Location = new Point(5, yvalue);

            pic.Name = "headrow";
            pic.statname.Text = Localisation.StatName;
            pic.statname.BackColor = Color.Black;
            pic.statname.ForeColor = Color.Orange;

            if (EquippedPart != null)
            {
                pic.equippeditemstat.Text = Localisation.EquippedItem;
                pic.equippeditemstat.BackColor = Color.Black;
                pic.equippeditemstat.ForeColor = Color.Orange;
            }

            if (BuyPart != null)
            {
                pic.newitemstat.Text = Localisation.ThisItem;
                pic.newitemstat.BackColor = Color.Black;
                pic.newitemstat.ForeColor = Color.Orange;
            }

            pic.Dock = DockStyle.Top;

            statspanel.Controls.Add(pic);
            yvalue += pic.Size.Height + 5;
        }

        public void AddItemRow(ref int yvalue, FieldInfo f)
        {
            object po = null;
            if (BuyPart != null)
                po = f.GetValue(BuyPart);

            object eqo = null;
            if (EquippedPart != null)
                eqo = f.GetValue(EquippedPart);

            if (po == null && eqo == null)
                return;

            Type ty = f.FieldType;
            //no non primitives
            if (ty.IsPrimitive == false)
                return;

            int equipcompare = 0;

            if (BuyPart != null && EquippedPart != null)
            {
                if (ty == typeof (int) || ty == typeof (Int16) || ty == typeof (Int32) || ty == typeof (Int64)
                    || ty == typeof (float) || ty == typeof (double))
                {
                    double v1 = double.Parse(po.ToString());
                    double v2 = double.Parse(eqo.ToString());

                    //some attributes like cooldown are better when lower, show as such
                    equipcompare = v2.CompareTo(v1);
                }

                else if (ty == typeof (bool))
                {
                    bool v1 = bool.Parse(po.ToString());
                    bool v2 = bool.Parse(eqo.ToString());
                    equipcompare = v2.CompareTo(v1);
                }

                //change attributes
                Type pt = BuyPart.GetType();

                bool ignore = (ignoreAttributes.ContainsKey(pt) && ignoreAttributes[pt].Contains(f.Name));
                if (ignore)
                    equipcompare = 0;

                bool reverse = equipcompare != 0 &&
                               (reverseAttributes.ContainsKey(pt) && reverseAttributes[pt].Contains(f.Name));
                if (reverse)
                {
                    if (equipcompare == -1)
                        equipcompare = 1;
                    else if (equipcompare == 1)
                        equipcompare = -1;
                }
            }

            var pic = new PurchasableItemWFControlItem(col1X, col2X);
            pic.Location = new Point(5, yvalue);

            pic.Name = "sp" + statspanel.Controls.Count.ToString();
            pic.statname.Text = f.Name;
            pic.statname.ForeColor = Color.Orange;
            pic.statname.BackColor = Color.DarkSlateGray;

            if (eqo == null)
                pic.equippeditemstat.Text = "";
            else
                pic.equippeditemstat.Text = eqo.ToString();

            if (po == null)
                pic.newitemstat.Text = "";
            else
                pic.newitemstat.Text = po.ToString();

            if (eqo != null && po != null)
            {
                Color badc = Color.DarkRed;
                Color goodc = Color.DarkGreen;
                Color okc = Color.Teal;

                if (equipcompare == -1)
                {
                    pic.equippeditemstat.BackColor = badc;
                    pic.newitemstat.BackColor = goodc;
                }
                else if (equipcompare == 0)
                {
                    pic.equippeditemstat.BackColor = okc;
                    pic.newitemstat.BackColor = okc;
                }
                else
                {
                    pic.equippeditemstat.BackColor = goodc;
                    pic.newitemstat.BackColor = badc;
                }
            }

            pic.Dock = DockStyle.Top;

            statspanel.Controls.Add(pic);
            yvalue += pic.Size.Height + 5;
        }

        public void AddItemRows(ref int yvalue)
        {
            //get stats
            Type t;
            if (BuyPart != null)
                t = BuyPart.GetType();
            else if (EquippedPart != null)
                t = EquippedPart.GetType();
            else
                return;

            FieldInfo[] ff = t.GetFields();


            //add values
            foreach (FieldInfo f in ff.Reverse())
            {
                AddItemRow(ref yvalue, f);
            }
        }

        public void AddControl(Control c, int row, int rc)
        {
        }
    }
}