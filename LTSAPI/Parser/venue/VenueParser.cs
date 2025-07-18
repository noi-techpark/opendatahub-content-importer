using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using GenericHelper;
using static System.Net.Mime.MediaTypeNames;

namespace LTSAPI.Parser
{
    public class VenueParser
    {
        public static VenueV2 ParseLTSVenueV2(
            JObject webcamlts, bool reduced
            )
        {
            try
            {
                LTSVenue ltsvenue = webcamlts.ToObject<LTSVenue>();

                return ParseLTSVenueV2(ltsvenue.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static VenueV2 ParseLTSVenueV2(
            LTSVenueData ltsvenue, 
            bool reduced)
        {
            VenueV2 venue = new VenueV2();

            venue.Id = ltsvenue.rid;
            venue._Meta = new Metadata() { Id = venue.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            venue.Source = "lts";

            venue.LastChange = ltsvenue.lastUpdate;

            //Tourism Organization

            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return venue;
        }

        public static VenueLinked ParseLTSVenueV1(
            JObject webcamlts, bool reduced
            )
        {
            try
            {
                LTSVenue ltsvenue = webcamlts.ToObject<LTSVenue>();

                return ParseLTSVenueV1(ltsvenue.data, reduced);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static VenueLinked ParseLTSVenueV1(
            LTSVenueData ltsvenue,
            bool reduced)
        {
            VenueLinked venue = new VenueLinked();

            venue.Id = ltsvenue.rid;
            venue._Meta = new Metadata() { Id = venue.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            venue.Source = "lts";

            venue.LastChange = ltsvenue.lastUpdate;

            //Tourism Organization

            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return venue;
        }
    }

}
