using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTSAPI.Utils
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

        public static Dictionary<string,string> GetGpxTrackDescription(string gpxurl)
        {
            if (gpxurl.EndsWith("original"))
                return new Dictionary<string, string>() { { "de", "Datei zum herunterladen" }, { "it", "scaricare dato" }, { "en", "File to download" } };
            else if (gpxurl.EndsWith("reduced"))
                return new Dictionary<string, string>() { { "de", "Übersicht" }, { "it", "compendio" }, { "en", "overview" } };
            else
                return new Dictionary<string, string>();
        }

        public static string GetGpxTrackType(string gpxurl)
        {
            if (gpxurl.EndsWith("original"))
                return "detailed";
            else if (gpxurl.EndsWith("reduced"))
                return "overview";
            else
                return "";
        }

        #endregion

        #region Accommodation

        #endregion
    }
}
