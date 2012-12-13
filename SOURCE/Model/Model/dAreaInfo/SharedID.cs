using System.Xml.Serialization;

namespace Project.Model.mapInfo
{
    public class SetID
    {
        #region IDAction enum

        public enum IDAction
        {
            DontSet,
            SetNew,
            SetForce
        }

        #endregion

        public IDAction action;
        public long forceID = -1;

        private SetID()
        {
        }

        public static SetID CreateNoID()
        {
            var ret = new SetID();
            ret.action = IDAction.DontSet;
            return ret;
        }

        public static SetID CreateSetNew()
        {
            var ret = new SetID();
            ret.action = IDAction.SetNew;
            return ret;
        }

        public static SetID CreateSetForce(long newid)
        {
            var ret = new SetID();
            ret.action = IDAction.SetForce;
            ret.forceID = newid;
            return ret;
        }

        public static implicit operator IDAction(SetID s)
        {
            return s.action;
        }
    }

    /// <summary>
    /// ships,buildings,weapons,maps,sectors,regions
    /// </summary>
    public class SharedID
    {
        private static long lastID = -1;
        [XmlIgnore] public long ID = -1;
        /*
		public static void ResetLastID()
		{
			lastID = -1;
		}
		*/

        public void setID(SetID cfg)
        {
            if (cfg == SetID.IDAction.DontSet)
                return;
            if (cfg == SetID.IDAction.SetNew)
                ID = ++lastID;
            else if (cfg == SetID.IDAction.SetForce)
            {
                ID = cfg.forceID;
            }
        }

        public long getID()
        {
            return ID;
        }
    }
}