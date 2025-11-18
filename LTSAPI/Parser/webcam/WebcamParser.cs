// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using GenericHelper;
using LTSAPI.Utils;

namespace LTSAPI.Parser
{
    public class WebcamInfoParser
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
            webcam._Meta = new Metadata() { Id = webcam.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "webcam", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            webcam.Source = "lts";

            webcam.LastChange = ltswebcam.lastUpdate;
            
            webcam.Active = ltswebcam.isActive;
            webcam.AreaIds = ltswebcam.areas != null ? ltswebcam.areas.Select(x => x.rid).ToList() : null;

            //Webcam Details
            webcam.WebCamProperties = new WebcamProperties();
            webcam.WebCamProperties.StreamUrl = ltswebcam.streamUrl;
            webcam.WebCamProperties.PreviewUrl = ltswebcam.previewUrl;
            webcam.WebCamProperties.WebcamUrl = ltswebcam.url;

            webcam.WebcamId = ltswebcam.rid;

            webcam.FirstImport =
                webcam.FirstImport == null ? DateTime.Now : webcam.FirstImport;

            //Let's find out for which languages there is a name
            if (ltswebcam.name != null)
            {
                foreach (var name in ltswebcam.name)
                {
                    if (!String.IsNullOrEmpty(name.Value))
                        webcam.HasLanguage.Add(name.Key);
                }
            }

            //Detail Information
            foreach (var language in webcam.HasLanguage)
            {
                Detail detail = new Detail();
                detail.Language = language;

                detail.Title = ltswebcam.name.GetValue(language);

                webcam.Detail.TryAddOrUpdate(language, detail);
            }

            //Position
            if (ltswebcam.position != null && ltswebcam.position.coordinates.Length == 2)
            {
                if (webcam.GpsInfo == null)
                    webcam.GpsInfo = new List<GpsInfo>();

                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = ltswebcam.position.coordinates[1];
                gpsinfo.Longitude = ltswebcam.position.coordinates[0];
                gpsinfo.Altitude = ltswebcam.position.altitude;
                gpsinfo.AltitudeUnitofMeasure = "m";

                webcam.GpsInfo.Add(gpsinfo);
            }

            //Images
            if (!String.IsNullOrEmpty(ltswebcam.url))
            {
                List<ImageGallery> imagegallerylist = new List<ImageGallery>();

                ImageGallery imagepoi = new ImageGallery();

                imagepoi.ImageTitle = ltswebcam.name;                
                imagepoi.ImageUrl = ltswebcam.url;

                imagegallerylist.Add(imagepoi);
                webcam.ImageGallery = imagegallerylist;
            }

            //Videos

            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltswebcam.rid);       
            
            if(ltswebcam.tourismOrganization != null && !String.IsNullOrEmpty(ltswebcam.tourismOrganization.rid))
                ltsmapping.Add("tourismOrganization", ltswebcam.tourismOrganization.rid);

            if (ltswebcam.isReadOnly != null)
                ltsmapping.Add("isReadOnly", ltswebcam.isReadOnly.ToString());
            if (ltswebcam.isOutOfOrder != null)
                ltsmapping.Add("isOutOfOrder", ltswebcam.isOutOfOrder.ToString());
            if (ltswebcam.hasCopyright != null)
                ltsmapping.Add("hasCopyright", ltswebcam.hasCopyright.ToString());

            webcam.Mapping.TryAddOrUpdate("lts", ltsmapping);

            return webcam;
        }
    }

}
