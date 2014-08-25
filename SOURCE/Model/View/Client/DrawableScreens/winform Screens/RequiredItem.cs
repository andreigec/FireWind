using System.Windows.Forms;
using Project.Model.Networking;

namespace Project.View.Client.DrawableScreens.Pop_Up_Screens
{
    public class RequiredItem
    {
        #region InputType enum

        public enum InputType
        {
            String
        }

        #endregion

        public Wrapper<object> SetThis;

        //for string
        public bool allowletters;
        public bool allownumbers;
        public InputType inputType;
        public bool optional;
        public Panel p;
        public string showText;


        private RequiredItem()
        {
        }

        public static RequiredItem GetRIString(Wrapper<object> SetThis, string ShowInfoText = "",
                                               bool OptionalValue = false,
                                               bool allowlettersI = true, bool allownumbersI = false)
        {
            var ri = new RequiredItem
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