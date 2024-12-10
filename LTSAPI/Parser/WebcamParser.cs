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
    public class WebcamParser
    {
        public static WebcamInfoLinked ParseLTSWebcam(
            JObject webcamlts, bool reduced
            )
        {
            try
            {
                LTSWebcam ltswebcam = webcamlts.ToObject<LTSWebcam>();

                return ParseLTSWebcam(ltswebcam.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static WebcamInfoLinked ParseLTSWebcam(
            LTSWebcamData ltswebcam, 
            bool reduced)
        {
            WebcamInfoLinked webcam = new WebcamInfoLinked();

            webcam.Id = ltswebcam.rid;
            webcam._Meta = new Metadata() { Id = webcam.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            webcam.Source = "lts";

            webcam.LastChange = ltswebcam.lastUpdate;

            //Tourism Organization

            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return webcam;
        }
    }

}
