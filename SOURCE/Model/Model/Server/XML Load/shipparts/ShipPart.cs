using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Project.Model.Server.XML_Load;

namespace Project
{
    public class SlotLocation
    {
        public enum SlotLocationEnum
        {
            None,
            ShipHullPart,
            ShipWingsPart,
            ShipEnginePart,
            ShipBoosterPart,
            ShipGeneratorPart,
            ShipBatteryPart,
            LeftWeapon,
            RightWeapon,
            ShipModel
        }

        public static List<Type> GetTypeList()
        {
            var ee = Enum.GetValues(typeof(SlotLocationEnum)).Cast<SlotLocationEnum>();
            var el= ee.Select(EnumToType).ToList();
            //no null entries
            el.RemoveAll(s => s == null);
            //remove duplicates
            var e2=el.Distinct().ToList();
            return e2;
        }

        public static List<string> GetEnumStringList(bool useTypeNames=false)
        {
            if (useTypeNames)
            {
                return GetTypeList().Select(x => x.Name).ToList();
            }
            else
            {
                var e = Enum.GetValues(typeof(SlotLocationEnum)).Cast<SlotLocationEnum>();
                return e.Select(s => s.ToString()).ToList();                
            }
        }

        public static Type EnumToType(SlotLocationEnum sl)
        {
            switch (sl)
            {
                case SlotLocationEnum.None:
                    return null;

                case SlotLocationEnum.ShipHullPart:
                    return typeof(ShipHullPart);

                case SlotLocationEnum.ShipWingsPart:
                    return typeof(ShipWingsPart);

                case SlotLocationEnum.ShipEnginePart:
                    return typeof(ShipEnginePart);

                case SlotLocationEnum.ShipBoosterPart:
                    return typeof(ShipBoosterPart);

                case SlotLocationEnum.ShipGeneratorPart:
                    return typeof(ShipGeneratorPart);

                case SlotLocationEnum.ShipBatteryPart:
                    return typeof(ShipBatteryPart);

                case SlotLocationEnum.LeftWeapon:
                    return typeof(weapon);

                case SlotLocationEnum.RightWeapon:
                    return typeof(weapon);

                case SlotLocationEnum.ShipModel:
                    return typeof(shipBase);

                default:
                    throw new ArgumentOutOfRangeException("sl");
            }
        }
    }

    

    [Serializable]
    [XmlInclude(typeof (ShipWingsPart))]
    [XmlInclude(typeof (ShipEnginePart))]
    [XmlInclude(typeof (ShipHullPart))]
    [XmlInclude(typeof (ShipBoosterPart))]
    [XmlInclude(typeof (ShipGeneratorPart))]
    [XmlInclude(typeof (ShipBatteryPart))]
    public class ShipPart :  IPurchasable
    {
        #region ILoadXMLBase Members

        public String name { get; set; }

        #endregion

        #region IPurchasable Members

        [XmlIgnore]
        public int Cost { get; set; }

        #endregion

        public SlotLocation.SlotLocationEnum ShipPartToEnum()
        {
            if (this is ShipHullPart)
                return SlotLocation.SlotLocationEnum.ShipHullPart;
            if (this is ShipWingsPart)
                return SlotLocation.SlotLocationEnum.ShipWingsPart;
            if (this is ShipEnginePart)
                return SlotLocation.SlotLocationEnum.ShipEnginePart;
            if (this is ShipBoosterPart)
                return SlotLocation.SlotLocationEnum.ShipBoosterPart;
            if (this is ShipGeneratorPart)
                return SlotLocation.SlotLocationEnum.ShipGeneratorPart;
            if (this is ShipBatteryPart)
                return SlotLocation.SlotLocationEnum.ShipBatteryPart;

            return SlotLocation.SlotLocationEnum.None;
        }

        public static ShipPart addPart(string partname)
        {
            var sm = loadXML.loadedShipParts[partname];
            if (sm is ShipHullPart)
                return sm as ShipHullPart;
            if (sm is ShipWingsPart)
                return sm as ShipWingsPart;
            if (sm is ShipEnginePart)
                return sm as ShipEnginePart;
            if (sm is ShipBoosterPart)
                return sm as ShipBoosterPart;
            if (sm is ShipGeneratorPart)
                return sm as ShipGeneratorPart;
            if (sm is ShipBatteryPart)
                return sm as ShipBatteryPart;
            return null;
        }
    }

    /// <summary>
    /// Increases turn stat
    /// </summary>
    public class ShipWingsPart : ShipPart
    {
        [XmlIgnore] public double MaxTurnModifier;
    }

    /// <summary>
    /// Increases Speed and accel stat
    /// </summary>
    public class ShipEnginePart : ShipPart
    {
        [XmlIgnore] public double AccelModifier;
        [XmlIgnore] public double BrakeModifier;

        [XmlIgnore] public double MaxAccelModifier;
        [XmlIgnore] public double MaxSpeedModifier;
    }

    public class ShipGeneratorPart : ShipPart
    {
        [XmlIgnore] public double EnergyRegenSpeedModifier;
    }

    public class ShipHullPart : ShipPart
    {
        [XmlIgnore] public double MaxArmourModifier;
    }

    public class ShipBoosterPart : ShipPart
    {
        [XmlIgnore] public double MaxBoosterMultiplierModifier;
        [XmlIgnore] public double maxBoosterTimeModifier;
    }

    public class ShipBatteryPart : ShipPart
    {
        [XmlIgnore] public double MaxEnergyModifier;

        [XmlIgnore] public double MaxShieldModifier;
    }
}