using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Project.Model.Networking;

namespace Project.View.Client.DrawableScreens.Pop_Up_Screens
{

    public class RequiredItem
    {
        public enum InputType
        {
            String
        }

        public InputType inputType;
        public Wrapper<object> SetThis;
        public bool optional;
        public string showText;
        public Panel p;

        //for string
        public bool allowletters;
        public bool allownumbers;


        private RequiredItem()
        {

        }

        public static RequiredItem GetRIString(Wrapper<object> SetThis, string ShowInfoText = "",
                                               bool OptionalValue = false,
                                               bool allowlettersI = true, bool allownumbersI = false)
        {
            var ri = new RequiredItem()
            {
                showText = ShowInfoText,
                inputType = InputType.String,
                allowletters = allowlettersI,
                allownumbers = allownumbersI,
                optional = OptionalValue,
                SetThis = SetThis
            };
            return ri;
        }

    }

}
