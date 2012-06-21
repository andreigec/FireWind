using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Model.Client.ClientScreens;
using Model.Networking;
using System.Resources;

namespace Model.ClientScreens
{
	public class MainScreen : drawableType
	{
		private static MenuOptions currentOption;
		private static MenuOptions rootOptions;
 
		static MainScreen()
		{
			rootOptions = new MenuOptions(null, false);

			//1p
			rootOptions.addChild(singleplayer, false);
			//mp
			var mp = rootOptions.addChild(multiplayer, false);
			//LS
			var ls=mp.addChild(listenserver, true);
			//3003, 3001, "127.0.0.1");s
			ls.addChild(tcpport, true).valueText = "3000";
			ls.addChild(udpport, true).valueText = "3001";
			ls.addChild(servername, true).valueText = "Test Server Name";
			ls.addChild(maxplayers, true).valueText = "8";
			ls.addChild(creategame, true);

			//client
			var cl=mp.addChild(client, false);
			cl.addChild(tcpport, true).valueText = "3000";
			cl.addChild(udpport, true).valueText = "3001";
			cl.addChild(ipaddress, true).valueText = "127.0.0.1";
			//exit
			rootOptions.addChild(exit, false);
			currentOption = rootOptions.getFirst();
		}

		public void Draw(Camera2D cam, GameTime gameTime)
		{
			cam.spriteBatch.Begin();
			int ypos = 0;
			var l = currentOption.createDisplayList();
			foreach (var s in l)
			{
				//draw current
				Color c = Color.Yellow;
				if (currentOption.Text.Equals(s))
					c = Color.Green;
				String s2 = s;
				var nl = currentOption.getNodeValueText(s);
				if (nl!=null)
				{
					s2 += ":" + nl;
					cam.DrawString(s2, c, new Vector2() { X = 0, Y = ypos });
				}
				else
				cam.DrawString(s2, c, new Vector2() { X = 0, Y = ypos });
				ypos += 20;
			}
			
			cam.spriteBatch.End();
		}

		public void RegisterKeyboardKeys(KeyboardClass kbc)
		{
			kbc.InitialiseKeyPress(Keys.Down);
			kbc.InitialiseKeyPress(Keys.Up);
			kbc.InitialiseKeyPress(Keys.Enter);
			kbc.InitialiseKeyPress(Keys.Escape);
		}

		public void KeyboardUpdate(GameTime gt, KeyboardClass kbc)
		{
			if (kbc.CanUseKey(Keys.Down))
			{
				MenuOptions.traverse(ref currentOption,true, false);
			}
			else if (kbc.CanUseKey(Keys.Up))
				MenuOptions.traverse(ref currentOption,false, false);

			else if (kbc.CanUseKey(Keys.Escape))
			{
				if (currentOption.Parent!=rootOptions)
				currentOption = currentOption.Parent;
			}

			else if (kbc.CanUseKey(Keys.Enter))
			{
				if (currentOption.isLeaf())
				{
					switch (currentOption.Text)
					{
						case singleplayer:
							var gic2 = new GameInitConfig();
							GameControlClient.CreateGame(gic2);
							break;

						case creategame:
							//get variables
							var tcp1 = currentOption.getNodeValueText(tcpport);
							if (tcp1 == null)
								return;
							int tcp2 = int.Parse(tcp1);

							var udp1 = currentOption.getNodeValueText(udpport);
							if (udp1 == null)
								return;
							int udp2 = int.Parse(udp1);

							var sn1 = currentOption.getNodeValueText(servername);
							if (sn1 == null)
								return;

							var mp1 = currentOption.getNodeValueText(maxplayers);
							if (mp1==null)
								return;
							int mp2 = int.Parse(mp1);

							var gic = new GameInitConfig(GameInitConfig.ServerType.ListenServer, sn1, mp2, udp2, tcp2);
							GameControlClient.CreateGame(gic);
							break;

						case client:
							var ctcp1 = currentOption.getNodeValueText(tcpport);
							if (ctcp1 == null)
								return;
							int ctcp2 = int.Parse(ctcp1);

							var cudp1 = currentOption.getNodeValueText(udpport);
							if (cudp1 == null)
								return;
							int cudp2 = int.Parse(cudp1);

							var ip1 = currentOption.getNodeValueText(ipaddress);
							if (ip1==null)
								return;
							IPAddress ip2 =null;
							try
							{
								ip2=IPAddress.Parse(ip1);
							}
							catch (Exception)
							{

								return;
							}
							String ip3 = ip2.ToString();
							if (string.IsNullOrEmpty(ip3))
								return;

							var gic1 = new GameInitConfig(cudp2,ctcp2,ip3);
							GameControlClient.CreateGame(gic1);
							break;

						case exit:
							GameControlClient.EndGame();
							break;
						default:
							break;
					}
				}
				else
				{
					MenuOptions.traverse(ref currentOption, false, true);
				}
				return;
			}
		}
	}
}
