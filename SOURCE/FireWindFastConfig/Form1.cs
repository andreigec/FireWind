using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CSHARPLIBRARY;
using Project;
using Project.Model;
using Project.XML_Load;

namespace FireWindFastConfig
{
	public partial class Form1 : Form
	{

		#region licensing

		private const string AppTitle = "FireWind Fast Config";
		private const double AppVersion = 0.1;
		private const String HelpString = "";

		private const String UpdatePath = "http://www.andreigec.net/games/firewind/config.zip";
		private const String VersionPath = "http://www.andreigec.net/games/firewind/configversion.txt";
		private const String ChangelogPath = "http://www.andreigec.net/games/firewind/configchangelog.txt";

		private readonly String OtherText =
			@"©" + DateTime.Now.Year +
			@" Andrei Gec (http://www.andreigec.net)

Licensed under GNU LGPL (http://www.gnu.org/)

Zip Assets © SharpZipLib (http://www.sharpdevelop.net/OpenSource/SharpZipLib/)
";
		#endregion
		public Form1()
		{
			InitializeComponent();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

			Licensing.CreateLicense(this, HelpString, AppTitle, AppVersion, OtherText, VersionPath, UpdatePath, ChangelogPath, menuStrip1);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var lsb = new List<SpriteBase>();
			
			int b2 = 0;
			int a2 = 0;
			for (int p = 1; p < 4; p++)
			{
				for (int a = 16; a < 32; a++)
				{
					//
					var sb = new SpriteBase();
					sb.name = "BUILDING_" + a2;
					sb.path = "GFX/Buildings";
					if (p != 1)
						sb.path += p;
					sb.columnCount = 8;
					sb.FrameHeight = 64;
					sb.FrameWidth = 64;
                    
                    sb.sprites = new SerializableDictionary<string, SpriteAnimation>();
					for (int b = 0; b < 4; b++)
					{
						sb.sprites.Add("PHASE_" + b, new SpriteAnimation() {startImageCount = b2, endImageCount = b2});
						b2++;
					}
                     
					lsb.Add(sb);
					a2++;
				}
				b2 = 0;
			}

			
            serialiseFile(lsb,"building.xml");
		}

        private static void serialiseFile<T>(List<T> obj, String outputFilename)
        {
            var f = serialiseText(obj);
            var sw = new StreamWriter(outputFilename);
            sw.Write(f);
            sw.Close();
        }

        private static string serialiseText<T>(T obj)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            var ser = new XmlSerializer(obj.GetType());
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            ser.Serialize(writer, obj);
            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            return sb.ToString();
        }

		private void button2_Click(object sender, EventArgs e)
		{
			var lsb = new List<building>();

			for (int a = 0; a < 48; a++)
			{
				var b = new building();
				b.name = b.SpriteName = "BUILDING_" + a;
				b.MaxArmour = 1000;
				for (int c = 0; c < 4; c++)
				{
					b.ImagesIN.Add(.25f*(c+1),"PHASE_"+(3-c));
				}
				lsb.Add(b);
			}
            Shared.serialiseFile(lsb, "buildingphase.xml");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var lsb = new List<SpriteBase>();
			
			
			for (int a=0;a<64;a++)
			{
				//
				var sb = new SpriteBase();
				sb.name = "ICON_" + a;
				sb.path = "GFX/Icons";
				sb.columnCount = 8;
				sb.FrameHeight = 32;
				sb.FrameWidth = 32;
                sb.sprites = new SerializableDictionary<string, SpriteAnimation>();
					sb.sprites.Add("IMG_" + a, new SpriteAnimation() { startImageCount = a, endImageCount = a });
					lsb.Add(sb);	
			}
            Shared.serialiseFile(lsb, "icon.xml");
		}

		private void button4_Click(object sender, EventArgs e)
		{
			var l = new List<SpriteBaseFastLoad>();
			SpriteBaseFastLoad s = new SpriteBaseFastLoad();
			s.FrameHeight = 32;
			s.FrameWidth = 32;
			s.framesPerImage = 1;
			s.path = "GFX/Icons";
			l.Add(s);

            Shared.serialiseFile(l, "icon2.xml");

		}
	}
}

