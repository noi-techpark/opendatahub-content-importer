// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTSAPI.Utils
{
    public static class ParserHelper
    {
        #region Generic

        public static string ParseOperationScheduleType(string type)
        {
            switch (type)
            {
                case "annual": return "2";
                case "bestSeason": return "3";
                case "standard": return "1";
                default: return "1";
            }
        }

        public static Dictionary<string,string> GetGpxTrackDescription(string gpxurl)
        {
            if (gpxurl.EndsWith("original"))
                return new Dictionary<string, string>() { { "de", "Datei zum herunterladen" }, { "it", "scaricare dato" }, { "en", "File to download" } };
            else if (gpxurl.EndsWith("reduced"))
                return new Dictionary<string, string>() { { "de", "Übersicht" }, { "it", "compendio" }, { "en", "overview" } };
            else
                return new Dictionary<string, string>();
        }

        public static string GetGpxTrackType(string gpxurl)
        {
            if (gpxurl.EndsWith("original"))
                return "detailed";
            else if (gpxurl.EndsWith("reduced"))
                return "overview";
            else
                return "";
        }

        public static string GetCountryName(string language) => language switch
        {
            "de" => "Italien",
            "it" => "Italia",
            "en" => "Italy",
            "cs" => "Itálie",
            "pl" => "Włochy",
            "nl" => "Italië",
            "fr" => "Italie",
            "ru" => "Италия",
            "_" => "Italy"
        };

        #endregion

        #region Accommodation

        #endregion

        #region GPSPosition

        public static ICollection<GpsInfo> GetGpsInfoForActivityPoi(ICollection<LTSPositionExtended> ltspositionlist)
        {
            var gpspointtoadd = new List<GpsInfo>();

            if (ltspositionlist != null)
            {
                foreach (var ltsgpsinfo in ltspositionlist)
                {
                    if (ltsgpsinfo != null && ltsgpsinfo.coordinates != null && ltsgpsinfo.coordinates.Length == 2)
                    {
                        //Position
                        if (ltsgpsinfo.category.rid == "86209D0947B54253AD24E4A95FFA8689")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "position", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });

                        //Start and End
                        if (ltsgpsinfo.category.rid == "F1F85E5880CB4130816F119F4FBBFB9C")
                        {
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "startingandarrivalpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "position", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                        }

                        //Start Point
                        if (ltsgpsinfo.category.rid == "232B767B6A2B412EB209C248B2E2AB83")
                        {
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "startingpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "position", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                        }
                        //End Point
                        if (ltsgpsinfo.category.rid == "2DDE73116F1D4EFD93DB04147AB95213")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "arrivalpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });

                        //Parking
                        if (ltsgpsinfo.category.rid == "3FD7DF59CA44499B8B4B6670A4590521")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "carparking", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });

                        //not defined
                        if (ltsgpsinfo.category.rid == "DCDBC46AC6E04F60B923753EE0C145F5")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "position", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                        
                        //View Point
                        if (ltsgpsinfo.category.rid == "BE7E5B05529949D98C8FD96326F15EFC")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "viewpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                        
                        //Halfway
                        if (ltsgpsinfo.category.rid == "BBD844B7EBE540EF9DF3613B49CA020B")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "halfwaypoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });

                        //Valley Station
                        if (ltsgpsinfo.category.rid == "C0501A1C82BE409E8EE1A1F02026080D")
                        {
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "valleystationpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "position", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                        }
                        //Middle Station
                        if (ltsgpsinfo.category.rid == "875B868C33CC44C488BB70EB4FF3F9D3")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "middlestationpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                        
                        //Mountain Station
                        if (ltsgpsinfo.category.rid == "E46C49301DAC4F5FA631BFFE2F60F03F")
                            gpspointtoadd.Add(new GpsInfo() { Altitude = ltsgpsinfo.altitude, AltitudeUnitofMeasure = "m", Gpstype = "mountainstationpoint", Latitude = ltsgpsinfo.coordinates[1], Longitude = ltsgpsinfo.coordinates[0] });
                    }
                }

                //Hack if position is there twice
                if (gpspointtoadd.Where(x => x.Gpstype == "position").Count() > 1)
                {
                    var gpspointtoremove = gpspointtoadd.Where(x => x.Gpstype == "position").FirstOrDefault();

                    gpspointtoadd.Remove(gpspointtoremove);
                }
            }

            return gpspointtoadd;
        }


        #endregion
    }
}
