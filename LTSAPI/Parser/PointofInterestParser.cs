using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTSAPI.Parser
{    
    public class PointofInterestParser
    {
        public static ODHActivityPoiLinked ParseLTSPointofInterest(
            JObject poilts, bool reduced
            )
        {
            try
            {
                LTSPointofInterest poiltsdetail = poilts.ToObject<LTSPointofInterest>();

                return ParseLTSPointofInterest(poiltsdetail.data, reduced);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ODHActivityPoiLinked ParseLTSPointofInterest(
            LTSPointofInterestData poi,
            bool reduced)
        {
            ODHActivityPoiLinked odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = poi.rid;
            odhactivitypoi._Meta = new Metadata() { Id = odhactivitypoi.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            odhactivitypoi.Source = "lts";

            odhactivitypoi.LastChange = poi.lastUpdate;

            //Tourism Organization

            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return odhactivitypoi;
        }
    }

}
