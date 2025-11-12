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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using GenericHelper;
using static System.Net.Mime.MediaTypeNames;
using LTSAPI.Utils;

namespace LTSAPI.Parser
{
    public class VenueParser
    {
        public static VenueFlattened ParseLTSVenueFlattened(
            JObject webcamlts, bool reduced
            )
        {
            try
            {
                LTSVenue ltsvenue = webcamlts.ToObject<LTSVenue>();

                return ParseLTSVenueVenueFlattened(ltsvenue.data, reduced);
            }
            catch(Exception ex)
            {           
                return null;
            }          
        }

        public static VenueFlattened ParseLTSVenueVenueFlattened(
            LTSVenueData ltsvenue, 
            bool reduced)
        {
            VenueFlattened venue = new VenueFlattened();

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

        public static VenueV2 ParseLTSVenueV1(
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

        public static VenueV2 ParseLTSVenueV1(
            LTSVenueData ltsvenue,
            bool reduced)
        {
            VenueV2 venue = new VenueV2();

            venue.Id = ltsvenue.rid;
            venue._Meta = new Metadata() { Id = venue.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "venue", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            venue.Source = "lts";

            venue.LastChange = ltsvenue.lastUpdate;

            venue.Active = ltsvenue.isActive;

            //Gps
            if (ltsvenue.position != null && ltsvenue.position.coordinates.Length == 2)
            {
                if (venue.GpsInfo == null)
                    venue.GpsInfo = new List<GpsInfo>();

                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = ltsvenue.position.coordinates[1];
                gpsinfo.Longitude = ltsvenue.position.coordinates[0];
                gpsinfo.Altitude = ltsvenue.position.altitude;
                gpsinfo.AltitudeUnitofMeasure = "m";

                venue.GpsInfo.Add(gpsinfo);
            }

            venue.HasLanguage = new List<string>();

            //Populate Haslanguage
            if(ltsvenue.name != null)
            { 
                foreach(var name in ltsvenue.name)
                {
                    if (!String.IsNullOrEmpty(name.Value))
                        venue.HasLanguage.Add(name.Key);
                }
            }

            //Tourism Organization
            //ltsvenue.tourismOrganization.rid;

            //Detail Information
            foreach (var language in venue.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Language = language;
                                
                detail.Title = ltsvenue.name[language];
                detail.BaseText = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language) : null;
                detail.IntroText = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language) : null;
                detail.ParkingInfo = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language) : null;
                detail.GetThereText = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language) : null;

                detail.AdditionalText = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "routeDescription").FirstOrDefault()?.description.GetValue(language) : null;
                detail.PublicTransportationInfo = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "publicTransport").FirstOrDefault()?.description.GetValue(language) : null;
                detail.AuthorTip = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "authorTip").FirstOrDefault()?.description.GetValue(language) : null;
                detail.SafetyInfo = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "safetyInstructions").FirstOrDefault()?.description.GetValue(language) : null;
                detail.EquipmentInfo = ltsvenue.descriptions != null ? ltsvenue.descriptions.Where(x => x.type == "equipment").FirstOrDefault()?.description.GetValue(language) : null;

                venue.Detail.TryAddOrUpdate(language, detail);
            }

            //Contact Information
            foreach (var language in venue.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.Language = language;
                contactinfo.CompanyName = ltsvenue.contact != null && ltsvenue.contact.address != null ? ltsvenue.contact.address.name.GetValue(language) : null;
                contactinfo.Address = ltsvenue.contact != null && ltsvenue.contact.address != null ? ltsvenue.contact.address.street.GetValue(language) : null;
                contactinfo.City = ltsvenue.contact != null && ltsvenue.contact.address != null ? ltsvenue.contact.address.city.GetValue(language) : null;
                contactinfo.CountryCode = ltsvenue.contact != null && ltsvenue.contact.address != null ? ltsvenue.contact.address.country : null;
                contactinfo.CountryName = ParserHelper.GetCountryName(language);
                contactinfo.ZipCode = ltsvenue.contact != null && ltsvenue.contact.address != null ? ltsvenue.contact.address.postalCode : null;
                contactinfo.Email = ltsvenue.contact != null ? ltsvenue.contact.email : null;
                contactinfo.Phonenumber = ltsvenue.contact != null ? ltsvenue.contact.phone : null;
                contactinfo.Url = ltsvenue.contact != null ? ltsvenue.contact.website : null;

                venue.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Opening Schedules

            //Tags  Add Categories here


            //Images

            //Videos

            //Halls

            if(ltsvenue.halls != null)
            {
                venue.RoomDetails = new List<VenueRoomDetailsV2>();

                foreach(var ltshall in ltsvenue.halls)
                {
                    VenueRoomDetailsV2 venueroomdetail = new VenueRoomDetailsV2();

                    venueroomdetail.Id = ltshall.rid;

                    //Detail Information
                    foreach (var language in venue.HasLanguage)
                    {
                        Detail detail = new Detail();

                        detail.Language = language;

                        detail.Title = ltshall.name[language];
                        detail.BaseText = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.IntroText = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.ParkingInfo = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.GetThereText = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language) : null;

                        detail.AdditionalText = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "routeDescription").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.PublicTransportationInfo = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "publicTransport").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.AuthorTip = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "authorTip").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.SafetyInfo = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "safetyInstructions").FirstOrDefault()?.description.GetValue(language) : null;
                        detail.EquipmentInfo = ltshall.descriptions != null ? ltshall.descriptions.Where(x => x.type == "equipment").FirstOrDefault()?.description.GetValue(language) : null;

                        venueroomdetail.Detail.TryAddOrUpdate(language, detail);
                    }

                    if(ltshall.dimension != null)
                    {
                        venueroomdetail.VenueRoomProperties = new VenueRoomProperties();
                        venueroomdetail.VenueRoomProperties.RoomDepthInMeters = ltshall.dimension.roomDepthInMeters;
                        venueroomdetail.VenueRoomProperties.RoomWidthInMeters = ltshall.dimension.roomWidthInMeters;
                        venueroomdetail.VenueRoomProperties.RoomHeightInCentimeters = ltshall.dimension.roomHeightInCentimeters;
                        venueroomdetail.VenueRoomProperties.SquareMeters = ltshall.dimension.squareMeters;
                        venueroomdetail.VenueRoomProperties.DoorHeightInCentimeters = ltshall.dimension.doorHeightInCentimeters;
                        venueroomdetail.VenueRoomProperties.DoorWidthInCentimeters = ltshall.dimension.doorWidthInCentimeters;                        
                    }

                    //Add Features as Tags
                    if (ltshall.features != null)
                    {
                        if(venueroomdetail.TagIds != null)
                            venueroomdetail.TagIds = new List<string>();

                        foreach(var feature in ltshall.features)
                        {
                            venueroomdetail.TagIds.Add("VEN" + feature.rid);
                        }
                    }

                    //Add Features as Tags
                    if (ltshall.purposesOfUse != null)
                    {
                        if (venueroomdetail.TagIds != null)
                            venueroomdetail.TagIds = new List<string>();

                        //if (venueroomdetail.Tags != null)
                        //    venueroomdetail.Tags = new List<Tags>();

                        foreach (var purposesofuse in ltshall.purposesOfUse)
                        {
                            venueroomdetail.TagIds.Add(purposesofuse.type);

                            //Problem the ID is not here, and we cannot 
                            //venueroomdetail.Tags.Add(new Tags() { Id = purposesofuse.type, Source = "lts", TagEntry = new Dictionary<string, string>() { { "maxCapacity", purposesofuse.maxCapacity.ToString() } } });
                        }
                    }

                    venueroomdetail.Placement = ltshall.placement;

                    venue.RoomDetails.Add(venueroomdetail);
                }
            }

            //Custom Fields
            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsvenue.rid);
            if (!String.IsNullOrEmpty(ltsvenue.tourismOrganization.rid))
                ltsmapping.Add("tourismOrganization", ltsvenue.tourismOrganization.rid);

            if (!String.IsNullOrEmpty(ltsvenue.accommodation?.rid))
                ltsmapping.Add("accommodation", ltsvenue.accommodation.rid.ToString());

            venue.Mapping.TryAddOrUpdate("lts", ltsmapping);

            return venue;
        }
    }

}
