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
    public class MeasuringpointParser
    {
        public static MeasuringpointV2 ParseLTSMeasuringpoint(
            JObject weathersnowlts, bool reduced
            )
        {
            try
            {
                LTSWeatherSnows ltsweathersnow = weathersnowlts.ToObject<LTSWeatherSnows>();

                return ParseLTSMeasuringpoint(ltsweathersnow.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static MeasuringpointV2 ParseLTSMeasuringpoint(
            LTSWeatherSnowsData ltsweathersnow, 
            bool reduced)
        {
            MeasuringpointV2 measuringpoint = new MeasuringpointV2();

            measuringpoint.Id = ltsweathersnow.rid;

            measuringpoint._Meta = new Metadata() { Id = measuringpoint.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "measuringpoint", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            measuringpoint.Source = "lts";
            measuringpoint.Active = ltsweathersnow.isActive;
            measuringpoint.LastChange = ltsweathersnow.lastUpdate;
            measuringpoint.LastUpdate = ltsweathersnow.lastUpdate;            

            //Position
            if (ltsweathersnow.position != null && ltsweathersnow.position.coordinates.Length == 2)
            {
                if (measuringpoint.GpsInfo == null)
                    measuringpoint.GpsInfo = new List<GpsInfo>();

                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = ltsweathersnow.position.coordinates[1];
                gpsinfo.Longitude = ltsweathersnow.position.coordinates[0];
                gpsinfo.Altitude = ltsweathersnow.position.altitude;
                gpsinfo.AltitudeUnitofMeasure = "m";

                measuringpoint.GpsInfo.Add(gpsinfo);
            }

            measuringpoint.AreaIds = ltsweathersnow.areas != null ? ltsweathersnow.areas.Select(x => x.rid).ToList() : null;
            
            measuringpoint.Shortname = ltsweathersnow.name.FirstOrDefault().Value;

            measuringpoint.HasLanguage = new List<string>();


            if (ltsweathersnow.name != null)
            {
                foreach (var desc in ltsweathersnow.name)
                {
                    if (!String.IsNullOrEmpty(desc.Value) && !measuringpoint.HasLanguage.Contains(desc.Key))
                        measuringpoint.HasLanguage.Add(desc.Key);
                }

                //Detail Information
                foreach (var language in measuringpoint.HasLanguage)
                {
                    DetailGeneric detail = new DetailGeneric();
                    detail.Language = language;

                    detail.Title = ltsweathersnow.name.ContainsKey(language) ? ltsweathersnow.name.GetValue(language) : null;

                    measuringpoint.Detail.TryAddOrUpdate(language, detail);
                }
            }

            if (ltsweathersnow.conditions != null)
            {
                measuringpoint.Temperature = ltsweathersnow.conditions.temperature != null ? ltsweathersnow.conditions.temperature.ToString() :null;
                measuringpoint.SnowHeight = ltsweathersnow.conditions.snow != null && ltsweathersnow.conditions.snow.height != null ? ltsweathersnow.conditions.snow.height.ToString() : null;
                measuringpoint.LastSnowDate = ltsweathersnow.conditions.snow != null && ltsweathersnow.conditions.snow.lastEvent != null ? ltsweathersnow.conditions.snow.lastEvent.Value : null;
                measuringpoint.newSnowHeight = ltsweathersnow.conditions.snow != null && ltsweathersnow.conditions.snow.lastEventHeight != null ? ltsweathersnow.conditions.snow.lastEventHeight.ToString() : null;

                if (ltsweathersnow.conditions.weatherForecasts != null)
                {
                    measuringpoint.WeatherObservation = new List<WeatherObservation>();
                    
                    foreach (var ltsweatherforecast in ltsweathersnow.conditions.weatherForecasts.forecasts)
                    {
                        WeatherObservation observation = new WeatherObservation();

                        observation.Id = ltsweatherforecast.rid;
                        observation.IconID = ltsweatherforecast.iconId.ToString();
                        observation.Date = Convert.ToDateTime(ltsweatherforecast.date);
                        observation.WeatherStatus = (Dictionary<string, string>)ltsweatherforecast.description;

                        measuringpoint.WeatherObservation.Add(observation);
                    }
                }
            }
            
            

            ////Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsweathersnow.rid);     
            
            if(ltsweathersnow.tourismOrganization != null && !String.IsNullOrEmpty(ltsweathersnow.tourismOrganization.rid))
                ltsmapping.Add("tourismOrganization", ltsweathersnow.tourismOrganization.rid);

            if(ltsweathersnow.isReadOnly != null)
                ltsmapping.Add("isReadOnly", ltsweathersnow.isReadOnly.ToString());
            if (ltsweathersnow.isOutOfOrder != null)
                ltsmapping.Add("isOutOfOrder", ltsweathersnow.isOutOfOrder.ToString());

            if (ltsweathersnow.conditions?.weatherForecasts?.regionId != null)
                ltsmapping.Add("weatherForecasts.regionId", ltsweathersnow.conditions.weatherForecasts.regionId.ToString());


            measuringpoint.Mapping.TryAddOrUpdate("lts", ltsmapping);

            return measuringpoint;
        }
    }

}
