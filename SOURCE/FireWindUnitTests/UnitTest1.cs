using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Model;
using Model.Instances;

namespace FireWindUnitTests
{
	[TestClass]
	public class UnitTest1
	{
		/*
		private Game g;
		private map m;	
			
		[TestMethod]
		public void InitBaseline()
		{
			g = new Game();
			var gdm = new GraphicsDeviceManager(g);
			//window
			gdm.PreferredBackBufferWidth = 1024;
			gdm.PreferredBackBufferHeight = 768;
			gdm.ApplyChanges();

			
			m = new map(new Camera2D(1,1,g.GraphicsDevice),new SpriteBatch(g.GraphicsDevice),g );
			
			//m.playerShip= new playerShip(null,m, g, new Vector2(400, -400));

			//ships.Add(new AIShip("SHIP2", this, maingame, new Vector2(450, -450)));//NE
			//int goangleToPlayer = spriteinstance.move.getAngleToOtherVector(dest);
			//ships.Add(new AIShip("SHIP2", this, maingame, new Vector2(450, -350)));//SE
			Exception ex = new Exception("sas");
			 
		}
		 * */

		public class angletest
		{
			public float X;
			public float Y;
			public int angle;
		}
		[TestMethod]
		public void VECTORMOVE_ANGLETEST()
		{
			float basex = 400f;
			float basey = -400f;
			int ca = 50;
			var b=new Vector2(basex,basey);

			runAngleTest(b,new angletest() { angle = 0, X = basex - ca, Y = basey });
			runAngleTest(b, new angletest() { angle = 45, X = basex - ca, Y = basey + ca });
			runAngleTest(b, new angletest() { angle = 90, X = basex, Y = basey + ca });
			runAngleTest(b, new angletest() { angle = 180, X = basex + ca, Y = basey });
			runAngleTest(b, new angletest() { angle = 270, X = basex, Y = basey - ca });
		}

		private void runAngleTest(Vector2 parent,angletest at)
		{
			VectorMove vm=new VectorMove(new Vector2(at.X,at.Y));

			float angle = vm.getAngleToOtherVector(parent);
			Assert.AreEqual(at.angle,angle,"Angles do not match, planex="+at.X+" planey="+at.Y);
		}
	}
}
