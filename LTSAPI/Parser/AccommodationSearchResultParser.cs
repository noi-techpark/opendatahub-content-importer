using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LTSAPI.Parser
{
    public class AccommodationSearchResultParser
    {
        public static MssResult ParseLTSAccommodation(JObject accommodationsearchresult, int rooms)
        {
            try
            {
                LTSAvailabilitySearchResult accoltssearchresult = accommodationsearchresult.ToObject<LTSAvailabilitySearchResult>();

                return ParseLTSAccommodationSearchResult(accoltssearchresult, rooms);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static MssResult ParseLTSAccommodationSearchResult(
            LTSAvailabilitySearchResult accommodationsearchresult,
            int rooms)
        {
            return null;
        }

    }
}
