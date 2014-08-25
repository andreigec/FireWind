using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Project.Model;
using Project.Model.Instances;
using Project.Model.Server.XML_Load;
using Project.Networking;
using Project.XML_Load;

namespace Project
{
    public static class loadXML
    {
        public const string buildingI = "BUILDING_";
        public const string buildingF = "BUILDINGS";
        public const string userships = "UserShips";
        private static Game parentGame;
        public static Dictionary<string, SpriteDraw> loadedSprites;
        public static Dictionary<string, shipBase> loadedShips;
        public static Dictionary<string, weapon> loadedWeapons;
        public static Dictionary<string, building> loadedBuildings;
        public static Dictionary<string, PlayerShipClass> loadedPlayerShips;
        public static Dictionary<string, ShipPart> loadedShipParts;

        public static void initXML(Game g)
        {
            if (loadedSprites != null)
                return;
            parentGame = g;
            loadedSprites = new Dictionary<string, SpriteDraw>();
            loadFastSprites(ref loadedSprites, "SPRITESFASTLOAD");
            loadBaseSprite(ref loadedSprites, "SPRITES");
            loadBaseSprite(ref loadedSprites, "BUILDINGSPRITES");
            loadPartSprite(ref loadedSprites, "PARTICLES");
            loadedWeapons = loadWeapon("WEAPONS");
            loadedShips = loadShips("SHIPS");
            loadedBuildings = loadBuildings(buildingF);
            loadedShipParts = loadShipParts();
        }

        public static void LoadPlayerShips()
        {
            loadPlayerShips(ref loadedPlayerShips, userships);
        }

        private static void loadPlayerShips(ref Dictionary<string, PlayerShipClass> loadedPlayerShips, string dir)
        {
            loadedPlayerShips = new Dictionary<string, PlayerShipClass>();
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                string reg = file;

                if (String.IsNullOrEmpty(name))
                {
                    Manager.FireLogEvent("Error parsing player ship:" + file, SynchMain.MessagePriority.Low, true, -1,
                                         null);
                    continue;
                }
                PlayerShipClass sec = PlayerShipClass.LoadPlayerShipClassFile(reg);
                sec.PlayerShip.instanceOwner = new InstanceOwner();
                sec.Name = name;

                foreach (ShipInstance s in sec.SupportCraft)
                {
                    s.instanceOwner = new InstanceOwner();
                }
                loadedPlayerShips.Add(name, sec);
            }
        }

        private static void loadFastSprites(ref Dictionary<string, SpriteDraw> dict, String path)
        {
            Dictionary<string, SpriteBaseFastLoad> ret = loadXMLOB<SpriteBaseFastLoad>(path);
            int subcount = 0;
            int imgcount = 0;
            foreach (var v in ret)
            {
                var sb = new SpriteBase();
                sb.FrameHeight = v.Value.FrameHeight;
                sb.FrameWidth = v.Value.FrameWidth;
                sb.name = v.Key;
                sb.path = v.Value.path;
                sb.loadTexture2D(parentGame, sb.path);

                int x = sb.image.Width/sb.FrameWidth;
                int y = sb.image.Height/sb.FrameHeight;
                int c = x*y;
                c /= v.Value.framesPerImage;
                sb.columnCount = x;
                for (int a = 0; a < c; a++)
                {
                    sb.sprites.Add(v.Value.frameName + imgcount,
                                   new SpriteAnimation
                                       {
                                           startImageCount = subcount,
                                           endImageCount = subcount + v.Value.framesPerImage - 1
                                       });
                    subcount += v.Value.framesPerImage;
                    imgcount++;
                }

                dict.Add(sb.name, sb);
            }
        }

        public static void loadBaseSprite(ref Dictionary<string, SpriteDraw> dict, String path)
        {
            Dictionary<string, SpriteBase> ret = loadXMLOB<SpriteBase>(path);

            foreach (var v in ret)
                loadBaseSprite(ref dict, v);
        }

        private static void loadBaseSprite(ref Dictionary<string, SpriteDraw> dict, KeyValuePair<String, SpriteBase> sb)
        {
            sb.Value.loadTexture2D(parentGame, sb.Value.path);
            dict.Add(sb.Key, sb.Value);
        }

        public static Dictionary<string, building> loadBuildings(String path)
        {
            Dictionary<string, building> ret = loadXMLOB<building>(path);
            foreach (var v in ret)
            {
                v.Value.BaseSprite = loadedSprites[v.Value.SpriteName] as SpriteBase;
                //destruct sprite list
                foreach (var s in v.Value.ImagesIN)
                {
                    v.Value.DestructFrames.Add(s.Key, v.Value.BaseSprite.sprites[s.Value]);
                }

                v.Value.ImagesIN = null;
            }
            return ret;
        }

        public static Dictionary<string, ShipPart> loadShipParts()
        {
            var ret = new Dictionary<string, ShipPart>();

            Dictionary<string, ShipBoosterPart> a = loadXMLOB<ShipBoosterPart>("BOOSTERPARTS");
            foreach (var a2 in a)
            {
                ret.Add(a2.Key, a2.Value);
            }

            Dictionary<string, ShipHullPart> b = loadXMLOB<ShipHullPart>("HULLPARTS");
            foreach (var b2 in b)
            {
                ret.Add(b2.Key, b2.Value);
            }

            Dictionary<string, ShipWingsPart> c = loadXMLOB<ShipWingsPart>("WINGPARTS");
            foreach (var c2 in c)
            {
                ret.Add(c2.Key, c2.Value);
            }

            Dictionary<string, ShipEnginePart> d = loadXMLOB<ShipEnginePart>("ENGINEPARTS");
            foreach (var d2 in d)
            {
                ret.Add(d2.Key, d2.Value);
            }

            Dictionary<string, ShipGeneratorPart> e = loadXMLOB<ShipGeneratorPart>("GENERATORPARTS");
            foreach (var e2 in e)
            {
                ret.Add(e2.Key, e2.Value);
            }

            Dictionary<string, ShipBatteryPart> f = loadXMLOB<ShipBatteryPart>("BATTERYPARTS");
            foreach (var f2 in f)
            {
                ret.Add(f2.Key, f2.Value);
            }

            return ret;
        }

        public static Dictionary<string, shipBase> loadShips(String path)
        {
            Dictionary<string, shipBase> ret = loadXMLOB<shipBase>(path);
            foreach (var v in ret)
            {
                v.Value.basesprite = loadedSprites[v.Value.spriteName];
            }
            return ret;
        }

        public static Dictionary<string, weapon> loadWeapon(String path)
        {
            Dictionary<string, weapon> ret = loadXMLOB<weapon>(path);
            foreach (var v in ret)
            {
                if (string.IsNullOrEmpty(v.Value.ParticleName) == false)
                {
                    SpriteDraw bs = loadedSprites[v.Value.ParticleName];
                    var b2 = bs as SpriteParticle;
                    v.Value.BaseSprite = b2;
                    v.Value.ParticleName = null;
                    v.Value.SpriteName = null;
                }
                else if (string.IsNullOrEmpty(v.Value.SpriteName) == false)
                    v.Value.BaseSprite = loadedSprites[v.Value.SpriteName];

                if (string.IsNullOrEmpty(v.Value.SlotPictureName) == false)
                {
                    SpriteDraw bs = loadedSprites[v.Value.SlotPictureName];
                    v.Value.SlotPictureTexture = bs;
                    v.Value.SlotPictureName = null;
                }
            }
            return ret;
        }

        public static void loadPartSprite(ref Dictionary<string, SpriteDraw> dict, String path)
        {
            Dictionary<string, SpriteParticle> ret = loadXMLOB<SpriteParticle>(path);
            foreach (var v in ret)
            {
                SpriteParticle v2 = v.Value;
                v2.initImage(parentGame);
                dict.Add(v.Key, v.Value);
            }
        }

        public static Dictionary<string, T> loadXMLOB<T>(String path) where T : ILoadXMLBase
        {
            var s = parentGame.Content.Load<List<T>>(path);
            return s.ToDictionary(s1 => s1.name);
        }
    }
}