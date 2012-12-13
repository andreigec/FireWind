using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Model
{
	public class map
	{
		private int width=10000;
		private int height=1000;
		private bool wrap = true;
		public List<ship> ships=new List<ship>( );

		public void addShip()
		{
			//texture2d = Content.Load<Texture2D>("Sprites/Characters/Character1/animate");
			ships.Add(new ship() {instance = new SpriteInstance(new Game() ) {basesprite = loadXML.loadedSprites[0]}});
		}

	}
}
