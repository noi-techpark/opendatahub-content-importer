using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTSAPI.Parser
{
    public static class ParserHelper
    {
        #region Generic

        public static string ParseOperationScheduleType(string type)
        {
            switch (type)
            {
                case "annual": return "2";
                case "bestSeason": return "3";
                case "standard": return "1";
                default: return "1";
            }
        }

        #endregion


        #region Accommodation

        #endregion



    }
}
