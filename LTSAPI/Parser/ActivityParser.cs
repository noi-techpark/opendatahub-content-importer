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
    public class ActivityParser
    {
        public static ODHActivityPoiLinked ParseLTSActivity(
            JObject activitylts, bool reduced
            )
        {
            try
            {
                LTSActivity activityltsdetail = activitylts.ToObject<LTSActivity>();

                return ParseLTSActivity(activityltsdetail.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static ODHActivityPoiLinked ParseLTSActivity(
            LTSActivityData activity, 
            bool reduced)
        {
            ODHActivityPoiLinked odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = activity.rid;
            odhactivitypoi._Meta = new Metadata() { Id = odhactivitypoi.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            odhactivitypoi.Source = "lts";

            odhactivitypoi.LastChange = activity.lastUpdate;

            //Tourism Organization
            odhactivitypoi.TourismorganizationId = activity.tourismOrganization.rid;

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
