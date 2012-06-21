namespace Project.View.Client.ClientScreens
{
    public class keyboardPress
    {
        public bool AllowKeyDownOnly;
        public int TimeSincePress;
        public int Timeout = 100;

        public void ShapeTimeSincePress()
        {
            if (TimeSincePress > Timeout)
                TimeSincePress = Timeout;
        }
    }
}