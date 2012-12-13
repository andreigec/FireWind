using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.Client.ClientScreens
{
	public class MenuOptions
	{
		public MenuOptions Parent;
		public string Text { get; private set; }
		private List<MenuOptions> children = new List<MenuOptions>();
		public bool hasValueText = false;
		public string valueText;

		public List<string> cached;
		public bool cachedirty = true;

		public MenuOptions(String text, bool hasValueText, MenuOptions parent = null)
		{
			this.hasValueText = hasValueText;
			Text = text;
			Parent = parent;
		}
		/*public MenuOptions getNode(String nodeText)
		{
			return Parent.getNodeTextAux(nodeText);
		}
		*/
		public String getNodeText(String nodeText)
		{
			var l = Parent.getNodeTextAux(nodeText);
			if (l!=null)
				return l.Text;
			return null;
		}

		public String getNodeValueText(String nodeText)
		{
			var l = Parent.getNodeTextAux(nodeText);
			if (l!=null&&l.hasValueText)
				return l.valueText;
			return null;
		}

		private MenuOptions getNodeTextAux(String nodeText)
		{
			if (Text != null&&Text.Equals(nodeText))
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
				foreach (var c in this.children)
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
			MenuOptions t = Parent;
			while (t != null)
			{
				if (t == m)
					return true;
				t = t.Parent;
			}
			return false;
		}

		public static void traverse(ref MenuOptions mo, bool down, bool enter)
		{
			var p = mo.Parent;
			if (p == null)
				return;

			//get index of this menuoption
			int index = 0;
			bool found = false;
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

			if (enter)
			{
				mo = mo.children[index];
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
	}
}
