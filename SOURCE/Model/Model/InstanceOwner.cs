using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Project.Model.Instances
{
    public class InstanceOwner
    {
        #region ControlType enum

        public enum ControlType
        {
            NoPlayer,
            JustOwner,
            SameFaction
        }

        #endregion

        //someone is currently controlling this item
        public long FactionOwner = -1;
        public long PlayerOwnerID = -1;
        //who can control this item? given player control possible
        public ControlType PossibleControl = ControlType.NoPlayer;

        public InstanceOwner()
        {
        }

        public InstanceOwner(long playerOwnerID, long factionOwner, ControlType possibleControl,
                             long currentControlID = -1)
        {
            CurrentControlID = currentControlID;
            PlayerOwnerID = playerOwnerID;
            FactionOwner = factionOwner;
            PossibleControl = possibleControl;
        }

        [XmlIgnore]
        public long CurrentControlID { get; private set; }

        public List<String> SerialiseCreate()
        {
            var ret = new List<string>();
            ret.Add(CurrentControlID.ToString());
            ret.Add(PlayerOwnerID.ToString());
            ret.Add(FactionOwner.ToString());
            ret.Add(PossibleControl.ToString("d"));
            return ret;
        }

        public static InstanceOwner DeserialiseCreate(List<String> args)
        {
            var ccid = long.Parse(Shared.PopFirstListItem(args));
            var owid = long.Parse(Shared.PopFirstListItem(args));
            var fact = long.Parse(Shared.PopFirstListItem(args));
            var ct = (ControlType) int.Parse(Shared.PopFirstListItem(args));

            var ret = new InstanceOwner(owid, fact, ct, ccid);
            return ret;
        }

        public bool PlayerCanControl(long playerID, long factionID, bool ignorecurrentcontrol = false)
        {
            //being controlled
            if ((CurrentControlID != -1 && ignorecurrentcontrol == false) ||
                //cant control
                (PossibleControl == ControlType.NoPlayer) ||
                //not the owner
                (PossibleControl == ControlType.JustOwner && PlayerOwnerID != playerID) ||
                //not the same faction
                (PossibleControl == ControlType.SameFaction && factionID != FactionOwner))
                return false;
            return true;
        }

        public bool ChangeControl(long playerID, long factionID)
        {
            if (PlayerCanControl(playerID, factionID) == false)
                return false;

            CurrentControlID = playerID;
            return true;
        }

        /*
		public void ResetOwner(bool removePlayerOwner)
		{
			CurrentControlID = -1;
			if (removePlayerOwner)
			{
				PlayerOwnerID = -1;
			}
		}
		*/

        public void ReleaseControl()
        {
            CurrentControlID = -1;
        }

        public static InstanceOwner ReturnNoControl()
        {
            return new InstanceOwner(-1, -1, ControlType.NoPlayer, -1);
        }

        public bool BeingControlled()
        {
            return CurrentControlID != -1;
        }

        public bool BeingControlledBy(long id)
        {
            return CurrentControlID == id;
        }
    }
}