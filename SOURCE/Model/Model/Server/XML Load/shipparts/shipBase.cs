using System;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Project.Model;
using Project.Model.Server.XML_Load;

namespace Project
{
    public enum shipAnimations
    {
        FLY,
        FLIP
    }

    public class shipBase : IPurchasable
    {
        [XmlIgnore] [ContentSerializerIgnore] public SpriteDraw basesprite;

        [XmlIgnore] public string spriteName;

        #region IPurchasable Members

        public String name { get; set; }

        [XmlIgnore]
        public int Cost { get; set; }

        #endregion

        #region stats
        //size overload
        [XmlIgnore] private double width;
		[XmlIgnore]
		private double height;
        //armour
        [XmlIgnore] public double BaseAccelIncrement;
        [XmlIgnore] public double BaseBrakeIncrement;
        [XmlIgnore] public double BaseEnergyRegenSpeed;
        [XmlIgnore] public double BaseMaxAccel;
        [XmlIgnore] public double BaseMaxArmour;
        [XmlIgnore] public double BaseMaxBoosterMultiplier;
        [XmlIgnore] public double BaseMaxBoosterTime;

        //other
        [XmlIgnore] public double BaseMaxEnergy;
        [XmlIgnore] public double BaseMaxShield;
        [XmlIgnore] public double BaseMaxSpeed;
        [XmlIgnore] public double BaseMaxSupportCraft;
        [XmlIgnore] public float BaseMaxTurnSpeed;

        #endregion stats

        //<
    }


    public interface ShipBaseItems
    {
        SerializableDictionary<SlotLocation.SlotLocationEnum, ShipPart> Parts { get; set; }
        SerializableDictionary<SlotLocation.SlotLocationEnum, weapon> Slots { get; set; }
        shipBase shipModel { get; set; }
    }

    /// <summary>
    /// Dynamic methods to get the values of attributes modified by parts/slots
    /// </summary>
    public static class ShipBaseItemsMIXIN
    {
        #region dynamic

        public static int GetRebuildCost(ShipBaseItems sbi)
        {
            return 500;
        }

        public static int GetTotalItemsCost(ShipBaseItems sbi)
        {
            int cost = 0;

            if (sbi.Parts != null && sbi.Parts.Count > 0)
                cost += sbi.Parts.Sum(s => s.Value.Cost);

            if (sbi.Slots != null && sbi.Slots.Count > 0)
                cost += sbi.Slots.Sum(s => s.Value.Cost);

            if (sbi.shipModel != null)
                cost += sbi.shipModel.Cost;

            return cost;
        }

        public static string GetShipModelName(ShipBaseItems sbi)
        {
            return sbi.shipModel.name;
        }

        public static double GetBrakeIncr(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseBrakeIncrement;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipEnginePart))
                m = ((ShipEnginePart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipEnginePart]).BrakeModifier;

            return s*m;
        }

        public static double GetAccelIncr(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseAccelIncrement;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipEnginePart))
                m = ((ShipEnginePart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipEnginePart]).AccelModifier;

            return s*m;
        }

        public static double GetMaxAccel(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseMaxAccel;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipEnginePart))
                m = ((ShipEnginePart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipEnginePart]).MaxAccelModifier;

            return s*m;
        }

        public static double GetMaxSpeed(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseMaxSpeed;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipEnginePart))
                m = ((ShipEnginePart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipEnginePart]).MaxSpeedModifier;

            return s*m;
        }

        public static double GetMinSpeed(ShipBaseItems sbi)
        {
            return 10;
        }

        public static double GetMaxArmour(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseMaxArmour;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipHullPart))
                m = ((ShipHullPart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipHullPart]).MaxArmourModifier;

            return s*m;
        }

        public static double GetMaxEnergy(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseMaxEnergy;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipBatteryPart))
                m = ((ShipBatteryPart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipBatteryPart]).MaxEnergyModifier;

            return (s*m);
        }

        public static double GetMaxShield(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseMaxShield;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipBatteryPart))
                m = ((ShipBatteryPart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipBatteryPart]).MaxShieldModifier;

            return (s*m);
        }

        public static double GetEnergyRegen(ShipBaseItems sbi)
        {
            double s = sbi.shipModel.BaseEnergyRegenSpeed;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipGeneratorPart))
                m =
                    ((ShipGeneratorPart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipGeneratorPart]).
                        EnergyRegenSpeedModifier;

            return (s*m);
        }

        public static double GetMaxTurnSpeed(ShipBaseItems sbi)
        {
            float s = sbi.shipModel.BaseMaxTurnSpeed;
            double m = 1;
            if (sbi.Parts.ContainsKey(SlotLocation.SlotLocationEnum.ShipWingsPart))
                m = ((ShipWingsPart) sbi.Parts[SlotLocation.SlotLocationEnum.ShipWingsPart]).MaxTurnModifier;

            return s*m;
        }

        #endregion dynamic
    }
}