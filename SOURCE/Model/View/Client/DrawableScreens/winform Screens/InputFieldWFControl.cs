﻿using System;
using System.Windows.Forms;

namespace Project.View.Client.DrawableScreens.Pop_Up_Screens
{
    public partial class InputFieldWFControl : UserControl
    {
        public InputFieldWFControl()
        {
            InitializeComponent();
        }
        
        public InputFieldWFControl(String labeltext = "DL", String deftextboxtext = "DT")
        {
            InitializeComponent();
            Controls[0].Controls[label1.Name].Text = labeltext;
            Controls[0].Controls[textBox1.Name].Text = deftextboxtext;
        }
    }
}