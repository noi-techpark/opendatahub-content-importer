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
            JObject activitydetail, bool reduced
            )
        {
            try
            {
                LTSActivity accoltsdetail = activitydetail.ToObject<LTSActivity>();

                return ParseLTSActivity(accoltsdetail.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static ODHActivityPoiLinked ParseLTSActivity(
            LTSActivity activity, 
            bool reduced)
        {
            ODHActivityPoiLinked odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = activity.rid;
            odhactivitypoi._Meta = new Metadata() { Id = activity.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            odhactivitypoi.Source = "lts";

            return odhactivitypoi;
        }
    }

}
