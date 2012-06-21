using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Client.ClientScreens
{
	public class keyboardPress
	{
		public int Timeout = 100;
		public int TimeSincePress = 0;
		public bool AllowKeyDownOnly = false;

		public void ShapeTimeSincePress()
		{
			if (TimeSincePress > Timeout)
				TimeSincePress = Timeout;
		}
	}
}
