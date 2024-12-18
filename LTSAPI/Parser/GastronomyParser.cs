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
    public class GastronomyParser
    {
        public static ODHActivityPoiLinked ParseLTSGastronomy(
            JObject activitylts, bool reduced
            )
        {
            try
            {
                LTSGastronomy gastroltsdetail = activitylts.ToObject<LTSGastronomy>();

                return ParseLTSGastronomy(gastroltsdetail.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static ODHActivityPoiLinked ParseLTSGastronomy(
            LTSGastronomyData ltsgastronomy, 
            bool reduced)
        {
            ODHActivityPoiLinked gastronomy = new ODHActivityPoiLinked();

            gastronomy.Id = ltsgastronomy.rid;
            gastronomy._Meta = new Metadata() { Id = gastronomy.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            gastronomy.Source = "lts";

            gastronomy.LastChange = ltsgastronomy.lastUpdate;

            //Tourism Organization

            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return gastronomy;
        }
    }

}
