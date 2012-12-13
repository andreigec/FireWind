using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.View.Client.ClientScreens;

namespace Project.View.Client.ClientScreens
{
    public class KeyboardClass
    {
        private const int DefaultTimeoutMS = 500;

        private KeyboardState CurrentState;
        private Dictionary<Keys, keyboardPress> KeyCoolDown = new Dictionary<Keys, keyboardPress>();
        private KeyboardState OldState;
        private bool allowUnregistedKeys = true;
        private GameTime gt;

        public KeyboardClass()
        {
            KeysHold = new List<Keys>();
            KeysDown = new List<Keys>();
        }

        public List<Keys> KeysHold { get; private set; }
        public List<Keys> KeysDown { get; private set; }

        public List<Keys> GetAllKeys()
        {
            var nl = new List<Keys>();
            nl.AddRange(KeysHold);
            nl.AddRange(KeysDown);
            return nl;
        }

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

        public void AddKeyboardStandardKeyPresses(bool AToZ, bool numbers, bool dot, bool space)
        {
            allowUnregistedKeys = false;
            var toadd = new List<Keys>();
            if (AToZ)
            {
                toadd.Add(Keys.A);
                toadd.Add(Keys.B);
                toadd.Add(Keys.C);
                toadd.Add(Keys.D);
                toadd.Add(Keys.E);
                toadd.Add(Keys.F);
                toadd.Add(Keys.G);
                toadd.Add(Keys.H);
                toadd.Add(Keys.I);
                toadd.Add(Keys.J);
                toadd.Add(Keys.K);
                toadd.Add(Keys.L);
                toadd.Add(Keys.M);
                toadd.Add(Keys.N);
                toadd.Add(Keys.O);
                toadd.Add(Keys.P);
                toadd.Add(Keys.Q);
                toadd.Add(Keys.R);
                toadd.Add(Keys.S);
                toadd.Add(Keys.T);
                toadd.Add(Keys.U);
                toadd.Add(Keys.V);
                toadd.Add(Keys.W);
                toadd.Add(Keys.X);
                toadd.Add(Keys.Y);
                toadd.Add(Keys.Z);
            }
            if (numbers)
            {
                toadd.Add(Keys.D0);
                toadd.Add(Keys.D1);
                toadd.Add(Keys.D2);
                toadd.Add(Keys.D3);
                toadd.Add(Keys.D4);
                toadd.Add(Keys.D5);
                toadd.Add(Keys.D6);
                toadd.Add(Keys.D7);
                toadd.Add(Keys.D8);
                toadd.Add(Keys.D9);
            }
            if (dot)
            {
                toadd.Add(Keys.OemPeriod);
            }
            if (space)
            {
                toadd.Add(Keys.Space);
            }

            foreach (var k in toadd)
            {
                InitKey(k);
                InitialiseKeyPress(k);
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

                InitKey(k, overloadTimeout);
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
            KeyCoolDown[k].TimeSincePress += (int) gt.ElapsedGameTime.TotalMilliseconds;
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