using System;
using System.Collections.Generic;
using Project.Model;

namespace Project
{
    public abstract partial class SectorConfig
    {
        public abstract List<string> SerialiseCreate();

        public static SectorConfig DeserialiseCreate(List<string> args)
        {
            var type = ((SectorType) (int.Parse(Shared.PopFirstListItem(args))));

            if (type == SectorType.Colosseum)
                return SectorConfigColosseum.DeserialiseCreateF(args);
            
            if (type == SectorType.GambleMatch)
                return SectorConfigGambleMatch.DeserialiseCreateF(args);

            return null;
        }
    }

    public partial class SectorConfigMission
    {
        public override List<string> SerialiseCreate()
        {
            throw new NotImplementedException();
        }
    }

    public partial class SectorConfigGambleMatch
    {
        public override List<string> SerialiseCreate()
        {
            var ret = new List<String>();
            ret.Add(SectorType.GambleMatch.ToString("d"));
            //start
            ret.Add(size.ToString("d"));
            ret.Add(shipcount.ToString());
            ret.Add(shipcost.ToString());
            return ret;
        }

        public static SectorConfig DeserialiseCreateF(List<string> args)
        {
            var size = (Size) int.Parse(Shared.PopFirstListItem(args));
            var count = int.Parse(Shared.PopFirstListItem(args));
            var cost = int.Parse(Shared.PopFirstListItem(args));

            var scc = new SectorConfigGambleMatch(size,count,cost);
            return scc;
        }
    }

    public partial class SectorConfigColosseum
    {
        public override List<string> SerialiseCreate()
        {
            var ret = new List<String>();
            ret.Add(SectorType.Colosseum.ToString("d"));
            //start
            ret.Add(size.ToString("d"));
            return ret;
        }

        public static SectorConfig DeserialiseCreateF(List<string> args)
        {
            var size = (Size) int.Parse(Shared.PopFirstListItem(args));
            var scc = new SectorConfigColosseum(size);
            return scc;
        }
    }
}