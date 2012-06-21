using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Model.Client.ClientScreens;

namespace Model.Client.ClientScreens
{
	public class KeyboardClass
	{
		private bool allowUnregistedKeys = true;
		private const int DefaultTimeoutMS = 500;

		private Dictionary<Keys, keyboardPress> KeyCoolDown = new Dictionary<Keys, keyboardPress>();

		private KeyboardState CurrentState;
		private KeyboardState OldState;
		private GameTime gt;

		private List<Keys> KeysHold = new List<Keys>();
		private List<Keys> KeysDown = new List<Keys>();

		public void ClearKeyTimeout()
		{
			KeyCoolDown = new Dictionary<Keys, keyboardPress>();
		}

		private void InitKey(Keys k, int timeout = DefaultTimeoutMS)
		{
			if (KeyCoolDown.ContainsKey(k) == false)
			{
				KeyCoolDown[k] = new keyboardPress {Timeout = timeout};
			}
		}

		public void InitialiseKeyPress(Keys k)
		{
			allowUnregistedKeys = false;
			InitKey(k);
			KeyCoolDown[k].AllowKeyDownOnly = true;
		}

		public void InitialiseKeyHold(Keys k, int timeout = DefaultTimeoutMS)
		{
			allowUnregistedKeys = false;
			InitKey(k);

			KeyCoolDown[k].Timeout = timeout;
			KeyCoolDown[k].TimeSincePress = 0;
		}

		public void InitialiseAllKeys()
		{
			allowUnregistedKeys = true;
		}

		public bool KeysPressed()
		{
			return (KeysDown.Count > 0 || KeysHold.Count > 0);
		}

		public bool CanUseKey(Keys k, int overloadTimeout = -1)
		{
			//key not used at all
			if (CurrentState.IsKeyDown(k) == false)
				return false;

			//key not registered in keycooldown = dont use
			if (KeyCoolDown.ContainsKey(k) == false)
			{
				if (allowUnregistedKeys == false)
					return false;

				InitKey(k,overloadTimeout);
			}

			var k2 = KeyCoolDown[k];

			if (overloadTimeout == -1)
				overloadTimeout = k2.Timeout;

			//press only?
			if (k2.AllowKeyDownOnly)
			{
				return KeysDown.Contains(k);
			}

			if (k2.TimeSincePress >= overloadTimeout)
			{
				k2.TimeSincePress -= overloadTimeout;
				return true;
			}
			return false;
		}

		public void UpdateKeys(KeyboardState kbs, GameTime gtIN)
		{
			gt = gtIN;
			CurrentState = kbs;
			KeysHold = new List<Keys>();
			KeysDown = new List<Keys>();
			
			foreach (var k in CurrentState.GetPressedKeys())
			{
				InitKey(k);

				UpdateKeyTimings(k);

				UpdateKeysHold(k);
				UpdateKeysHoldKeysDown(k);
			}
		}

		public void SwitchStates()
		{
			OldState = CurrentState;
		}

		private void UpdateKeyTimings(Keys k)
		{
			KeyCoolDown[k].TimeSincePress += (int)gt.ElapsedGameTime.TotalMilliseconds;
			KeyCoolDown[k].ShapeTimeSincePress();
		}

		private void UpdateKeysHold(Keys k)
		{
			if (OldState.IsKeyDown(k))
				KeysHold.Add(k);
		}

		private void UpdateKeysHoldKeysDown(Keys k)
		{
			if (OldState.IsKeyDown(k) == false)
				KeysDown.Add(k);
		}
	}
}
