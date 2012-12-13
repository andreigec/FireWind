using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Project.Model.Server.XML_Load;

namespace Project.Model
{
    public class weaponslot : IPurchasable
    {
        //shot cooldown
        [XmlIgnore] public double LastShotTimeStamp;
        public weapon w;

        public weaponslot()
        {
        }

        public weaponslot(weapon w)
        {
            this.w = w;
        }

        #region ILoadXMLBase Members

        [XmlIgnore]
        [ContentSerializerIgnore]
        public string name
        {
            get { return w.name; }
            set { }
        }

        #endregion

        #region IPurchasable Members

        [XmlIgnore]
        [ContentSerializerIgnore]
        public int Cost
        {
            get { return GetCost(); }
            set { Cost = value; }
        }

        #endregion

        public int GetCost()
        {
            return w.Cost;
        }

        public void fire(GameTime gt, map m, ShipInstance si)
        {
            //test cooldown between shots
            var canshoot = Shared.TimeSinceElapsed(ref LastShotTimeStamp, gt, w.CoolDown);
            if (canshoot == false)
                return;

            //test energy
            if (si.CurrentEnergy >= w.EnergyPerShot)
            {
                GameControlClient.fireShot(m, si, w);
                si.CurrentEnergy -= w.EnergyPerShot;
            }
        }
    }
}