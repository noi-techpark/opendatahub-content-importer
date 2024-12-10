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
    public class EventParser
    {
        public static EventV2 ParseLTSEvent(
            JObject activitylts, bool reduced
            )
        {
            try
            {
                LTSEvent eventltsdetail = activitylts.ToObject<LTSEvent>();

                return ParseLTSEvent(eventltsdetail.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static EventV2 ParseLTSEvent(
            LTSEventData activity, 
            bool reduced)
        {
            EventV2 eventv1 = new EventV2();

            eventv1.Id = activity.rid;
            eventv1._Meta = new Metadata() { Id = eventv1.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "event", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            eventv1.Source = "lts";

            eventv1.LastChange = activity.lastUpdate;

            //Tourism Organization

            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return eventv1;
        }
    }

}
