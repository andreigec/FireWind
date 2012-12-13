using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project.Model;
using Project.View.Client.Cameras;

namespace Project.View.Client.ClientScreens
{
    public class MenuOptionsCreate
    {
        public int EditCaret;
        public bool EditingCurrent;
        public MenuOptions currentOption;
        public string currentValueEdit;
        public MenuOptions rootOptions;

        public static void RegisterKeyboardValueEntry(KeyboardClass kbc, bool letters, bool numbers, bool dot, bool space)
        {
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyPress(Keys.Left);
            kbc.InitialiseKeyPress(Keys.Right);
            kbc.InitialiseKeyPress(Keys.Escape);
            kbc.InitialiseKeyPress(Keys.Enter);
            kbc.InitialiseKeyHold(Keys.Delete, 50);
            kbc.InitialiseKeyHold(Keys.Back, 50);
            kbc.AddKeyboardStandardKeyPresses(letters, numbers, dot, space);
        }


        public void Draw(Camera2D cam, GameTime gameTime)
        {
            if (currentOption == null)
                return;

            cam.spriteBatch.Begin();
            var ypos = 0;
            var l = currentOption.createDisplayList();
            foreach (var s in l)
            {
                //draw current
                var c = Color.Green;
                if (currentOption.Text.Equals(s))
                    c = Color.Yellow;
                var s2 = s;
                var nl = currentOption.getNodeValueText(s);

                if (nl != null)
                {
                    s2 += ":";
                    if (EditingCurrent && currentOption.Text.Equals(s))
                    {
                        s2 += currentValueEdit.Substring(0, EditCaret);
                        s2 += "|";
                        s2 += currentValueEdit.Substring(EditCaret);
                    }
                    else
                    {
                        s2 += nl;
                    }

                    cam.DrawString(s2, c, new Vector2 { X = 0, Y = ypos });
                }
                else
                    cam.DrawString(s2, c, new Vector2 { X = 0, Y = ypos });

                ypos += 20;
            }
            cam.spriteBatch.End();
        }

        public void StartEditingCurrent()
        {
            EditingCurrent = true;
            EditCaret = 0;
            currentValueEdit = currentOption.valueText;
            currentOption.cachedirty = true;
        }

        public bool HandleKey(KeyboardClass kbc)
        {
            if (EditingCurrent)
            {
                if (kbc.CanUseKey(Keys.Escape))
                {
                    RegisterKeyboardKeys(kbc);
                    currentOption.cachedirty = true;
                    EditingCurrent = false;
                }
                else if (kbc.CanUseKey(Keys.Enter))
                {
                    RegisterKeyboardKeys(kbc);
                    currentOption.cachedirty = true;
                    currentOption.valueText = currentValueEdit;
                    EditingCurrent = false;
                }
                //move
                else if (kbc.CanUseKey(Keys.Left))
                {
                    EditCaret--;
                    if (EditCaret < 0)
                        EditCaret = 0;
                }
                else if (kbc.CanUseKey(Keys.Right))
                {
                    EditCaret++;
                    if (EditCaret > currentValueEdit.Length)
                        EditCaret = currentValueEdit.Length;
                }
                //delete
                else if (kbc.CanUseKey(Keys.Back))
                {
                    if (EditCaret != 0)
                    {
                        currentValueEdit = currentValueEdit.Remove(EditCaret - 1, 1);
                        EditCaret -= 1;
                    }
                }
                else if (kbc.CanUseKey(Keys.Delete))
                {
                    if (EditCaret != currentValueEdit.Length)
                        currentValueEdit = currentValueEdit.Remove(EditCaret, 1);
                }
                //valueText
                else if (kbc.KeysDown.Count != 0)
                {
                    var kbp = kbc.KeysDown[0];
                    if (kbc.CanUseKey(kbp))
                    {
                        var kbp2 = Shared.GetCharFromKeys(kbp);
                        currentValueEdit = currentValueEdit.Insert(EditCaret, kbp2.ToString());
                        EditCaret++;
                    }
                }
            }
            //choose an option
            else if (currentOption.isLeaf() && currentOption.Parent.hasValueText &&
                     currentOption.Parent.options)
            {
                var str = currentOption.Text;
                traverse(ref currentOption, false, -1);
                currentOption.valueText = str;
            }

            else if (kbc.CanUseKey(Keys.Down))
            {
                traverse(ref currentOption, true, 0);
            }
            else if (kbc.CanUseKey(Keys.Up))
            {
                traverse(ref currentOption, false, 0);
            }
            //only handle escape if the menu can go to the parent when its not root
            else if (kbc.CanUseKey(Keys.Escape) && currentOption != null && currentOption.Parent != rootOptions)
            {
                traverse(ref currentOption, false, -1);
            }
            else
                return false;
            return true;
        }

        /// <summary>
        /// Move around the menu
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="down"></param>
        /// <param name="levelChange">0 to stay on same level, 1 to go into an option, -1 to come out of an option</param>
        public void traverse(ref MenuOptions mo, bool down, int levelChange)
        {
            var p = mo.Parent;
            if (p == null)
                return;

            //get index of this menuoption
            var index = 0;
            var found = false;
            for (; index < p.children.Count; index++)
            {
                if (p.children[index].Text == mo.Text)
                {
                    found = true;
                    break;
                }
            }
            if (found == false)
                return;

            if (levelChange == 1)
            {
                //on enter, go to the first item
                if (mo.isLeaf() == false)
                {
                    //var pare = mo.Parent;
                    mo = mo.children[0];
                    //mo.Parent = pare;
                    mo.cachedirty = true;
                    return;
                }

                //leaf and not handled = value
                if (mo.hasValueText)
                {
                }
            }

            //come out of option
            if (levelChange == -1)
            {
                mo = mo.Parent;
                mo.cachedirty = true;
                return;
            }

            if (down)
                index++;
            else
                index--;

            if (index < 0)
                index = p.children.Count - 1;
            else if (index == p.children.Count)
                index = 0;

            mo = p.children[index];
        }


        public void RegisterKeyboardKeys(KeyboardClass kbc)
        {
            kbc.ClearKeyTimeout();
            kbc.InitialiseKeyPress(Keys.Down);
            kbc.InitialiseKeyPress(Keys.Up);
            kbc.InitialiseKeyPress(Keys.Enter);
            kbc.InitialiseKeyPress(Keys.Escape);
        }
    }

    public class MenuOptions
    {
        public MenuOptions Parent;
        public object Tag;

        public List<string> cached;
        public bool cachedirty = true;
        public bool hasValueText;

        public bool options;
        public string valueText;


        public MenuOptions(String text, bool hasValueText, MenuOptions parent = null)
        {
            children = new List<MenuOptions>();
            this.hasValueText = hasValueText;
            Text = text;
            Parent = parent;
        }

        public string Text { get; private set; }
        public List<MenuOptions> children { get; private set; }

        public MenuOptions getNode(String nodeText)
        {
            var l = getNodeTextAux(nodeText);
            if (l != null)
                return l;
            return null;
        }

        public String getNodeText(String nodeText)
        {
            var l = Parent.getNodeTextAux(nodeText);
            if (l != null)
                return l.Text;
            return null;
        }

        public String getNodeValueText(String nodeText)
        {
            var l = Parent.getNodeTextAux(nodeText);
            if (l != null && l.hasValueText)
                return l.valueText;
            return null;
        }

        private MenuOptions getNodeTextAux(String nodeText)
        {
            if (Text != null && Text.Equals(nodeText))
                return this;

            foreach (var c in children)
            {
                var n = c.getNodeTextAux(nodeText);
                if (n != null)
                    return n;
            }
            return null;
        }

        public bool isLeaf()
        {
            return children == null || children.Count == 0;
        }

        public List<string> createDisplayList()
        {
            if (Parent.cachedirty)
            {
                var l = Parent.createDisplayListAux(1);
                Parent.cached = l;
                Parent.cachedirty = false;
            }
            return Parent.cached;
        }

        private List<string> createDisplayListAux(int levels)
        {
            var o = new List<string>();

            if (string.IsNullOrEmpty(Text) == false)
            {
                o.Add(Text);
            }

            if (levels > 0)
            {
                foreach (var c in children)
                    o.AddRange(c.createDisplayListAux(levels - 1));
            }

            return o;
        }

        public MenuOptions getFirst()
        {
            if (children == null || children.Count == 0)
                return null;
            return children[0];
        }

        public MenuOptions addChild(String text, bool hasValueText)
        {
            var mo = new MenuOptions(text, hasValueText, this);
            children.Add(mo);
            return mo;
        }

        public void addChild(MenuOptions mo)
        {
            mo.Parent = this;
            children.Add(mo);
        }

        public bool hasParentNode(MenuOptions m)
        {
            var t = Parent;
            while (t != null)
            {
                if (t == m)
                    return true;
                t = t.Parent;
            }
            return false;
        }
    }
}