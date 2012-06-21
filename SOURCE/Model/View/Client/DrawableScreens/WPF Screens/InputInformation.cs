using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.Model.mapInfo;
using Project.View.Client.Cameras;
using Project.View.Client.ClientScreens;
using XNA_Winforms_Wrapper;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Point = System.Drawing.Point;

namespace Project.View.Client.DrawableScreens.Pop_Up_Screens
{
    public partial class InputInformation : Form, IScreenControls
    {
        private string labelname;
        private string textboxname;

        private static KeyboardClass kbc;
        public static XNA_WF_Wrapper XnaWfWrapperInstance;

        private TextBox selected;
        private int CaretLocation;

        //for return
        public delegate void AcceptInputItemsDelegate(bool OKPressed);

        private AcceptInputItemsDelegate retfunc;

        public InputInformation(IEnumerable<RequiredItem> RI, AcceptInputItemsDelegate retfunc)
        {
            InitializeComponent();

            this.retfunc = retfunc;

            if (XnaWfWrapperInstance == null)
            {
                XnaWfWrapperInstance = new XNA_WF_Wrapper();
            }
            XnaWfWrapperInstance.Init(this);

            //dynamically get field names
            var ifa=new InputFieldWFControl();
            labelname = ifa.label1.Name;
            textboxname = ifa.textBox1.Name;
            ifa = null;

            int x = 0;
            int y = 0;
            
            foreach (var ri in RI)
            {
                var newp = new InputFieldWFControl(ri.showText,"");

                //set general properties
                newp.Location = new Point(x, y);
                ri.p = newp.Controls[0] as Panel;
                ri.p.Tag = ri;
                
                Controls.Add(newp);

                //move
                y += newp.Size.Height + 10;
            }

            //set y of ok and cancel buttons
            okbutton.Location = new Point(okbutton.Location.X, y);
            cancelbutton.Location = new Point(cancelbutton.Location.X, y);
        }

        public void InitOKButton(bool ok)
        {
            if (ok)
            {
                okbutton.Enabled = true;
            }
            else
            {
                okbutton.Enabled = false;
            }
        }

        public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
        {
            if (kbc.KeysDown.Count == 0)
                return;

            var p = kbc.KeysDown;

            if (selected != null && CaretLocation != -1)
            {
                foreach (var pp in p)
                {
                    AddTextboxText(selected, pp);
                }
            }
        }


        private void AddTextboxText(TextBox tb, Keys press)
        {
            var kbp2 = Shared.GetCharFromKeys(press);

            string add = kbp2.ToString();

            if (press == Keys.Left)
            {
                SetCaretLocation(tb, --CaretLocation);
            }
            else if (press == Keys.Right)
            {
                SetCaretLocation(tb, ++CaretLocation);
            }
            else if (add == "\b" && CaretLocation > 0)
            {
                tb.Text = tb.Text.Remove(--CaretLocation, 1);
                SetCaretLocation(tb, CaretLocation);
            }
            else if (kbp2 >= 32 && kbp2 <= 126)
            {
                tb.Text = tb.Text.Insert(CaretLocation++, add);
                SetCaretLocation(tb, CaretLocation);
            }
        }

        private void SetTextBox(TextBox tb)
        {
            //if no selected textbox, we need to set caret the 0
            if (selected != tb)
            {
                if (selected != null)
                {
                    SetCaretLocation(tb, -1);
                }

                selected = tb;
                SetCaretLocation(tb, 0);
                //set keyboard
                kbc.ClearKeyTimeout();
                var ri = tb.Tag as RequiredItem;
                if (ri != null)
                {
                    kbc.AddKeyboardStandardKeyPresses(ri.allowletters, ri.allownumbers, false, false);
                }
            }
            else
            {
                var mouse = Mouse.GetState();
                //get the click location to update the caret
                var pos = new Vector2(mouse.X, mouse.Y);
                var loc = XnaWfWrapperInstance.GetClickLocation(pos, tb);

                SetCaretLocation(tb, loc);

                selected = tb;
            }
        }

        private void SetCaretLocation(TextBox tb, int caretloc)
        {
            //remove the current caret if any
            tb.Text = tb.Text.Replace("|", "");

            if (caretloc > tb.Text.Length)
                caretloc = tb.Text.Length;

            if (caretloc <= 0)
                caretloc = 0;

            //add the caret if wanted
            if (caretloc != -1 && caretloc <= tb.Text.Length)
            {
                tb.Text = tb.Text.Insert(caretloc, "|");
            }
            CaretLocation = caretloc;
        }

        public void returnstuff(List<RequiredItem> ril = null)
        {
            if (ril!=null)
            {
                foreach (var ri in ril)
                {
                    var tb = ri.p.Controls[textboxname] as TextBox;
                    if (tb == null)
                        continue;
                    //remove the caret
                    tb.Text = tb.Text.Replace("|", "");
                    //set the changed text
                    ri.SetThis.Value = tb.Text;
                }  
            }

            if (ril != null && ril.Count > 0)
                retfunc(true);
            else
            {
                retfunc(false);
            }

            GameControlClient.ExitPopupScreen<InputInformation>();
        }

        public void MouseUpdate(GameTime gt, MouseClass mc)
        {
            var c = XnaWfWrapperInstance.MouseUpdate(gt);
            if (c == null)
                return;

            if (mc.ButtonsPressed() == false)
                return;

            if (c == okbutton)
            {
                var rets = new List<RequiredItem>();
                foreach (var cc in Controls)
                {
                    if (cc is InputFieldWFControl&&((InputFieldWFControl)cc).Controls[0] is Panel)
                    {
                        var p = (cc as InputFieldWFControl).Controls[0] as Panel;
                        
                        rets.Add(p.Tag as RequiredItem);
                    }
                }
                returnstuff(rets);
            }

            else if (c == cancelbutton)
            {
                returnstuff();
            }
            else
            {
                if (c == null)
                {
                    if (selected != null)
                    {
                        SetCaretLocation(selected, -1);
                    }

                    selected = null;
                    CaretLocation = -1;
                    return;
                }

                if (c is TextBox)
                {
                    SetTextBox(c as TextBox);
                }
            }
        }

        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            InputInformation.kbc = kbc;

            // throw new NotImplementedException();
        }



        public void Draw(Camera2D cam, GameTime gameTime)
        {
            bool ok = true;
            //update ok button based on if all text boxes are filled as required
            foreach (var p1 in Controls)
            {
                if (p1 is Panel)
                {
                    var p = p1 as Panel;
                    var ri = p.Tag as RequiredItem;
                    var tb = p.Controls[textboxname];

                    if (ri.optional == false)
                    {
                        if (string.IsNullOrEmpty(tb.Text) || string.IsNullOrWhiteSpace(tb.Text) ||
                            tb.Text.Length == 1 && tb.Text[0].Equals('|'))
                            ok = false;
                        break;
                    }
                }
            }

            InitOKButton(ok);

            XnaWfWrapperInstance.Draw(true);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
