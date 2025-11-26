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
    public class VenueParser
    {
        public static VenueFlattened ParseLTSVenueFlattened(
            JObject venuelts, bool reduced
            )
        {
            try
            {
                LTSVenue ltsvenue = venuelts.ToObject<LTSVenue>();

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

        public static VenueV2 ParseLTSVenue(
            JObject venuelts, bool reduced
            )
        {
            try
            {
                LTSVenue ltsvenue = venuelts.ToObject<LTSVenue>();

                return ParseLTSVenue(ltsvenue.data, reduced);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static VenueV2 ParseLTSVenue(
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

            venue.LocationInfo = new LocationInfoLinked();


            //District and Tourism Organization
            //venue.LocationInfo.TvInfo = ltsvenue.tourismOrganization != null ? new TvInfoLinked() { Id = ltsvenue.tourismOrganization.rid } : null;
            venue.LocationInfo.DistrictInfo = ltsvenue.district != null && !String.IsNullOrEmpty(ltsvenue.district.rid) ? new DistrictInfoLinked() { Id = ltsvenue.district.rid } : null;

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

                if(ltsvenue.location != null)
                {
                    if (!String.IsNullOrEmpty(ltsvenue.location.GetValue(language)))
                        contactinfo.Area = ltsvenue.location.GetValue(language);
                }

                venue.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            if (ltsvenue.name != null)
                venue.Shortname = ltsvenue.name.Where(x => !String.IsNullOrEmpty(x.Value)).FirstOrDefault().Value;


            //Opening Schedules            
            if (ltsvenue.openingSchedules != null)
            {
                List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();

                foreach (var operationschedulelts in ltsvenue.openingSchedules)
                {
                    OperationSchedule operationschedule = new OperationSchedule();
                    operationschedule.Start = Convert.ToDateTime(operationschedulelts.startDate);
                    operationschedule.Stop = Convert.ToDateTime(operationschedulelts.endDate);
                    operationschedule.Type = "1";
                    //operationschedule.OperationscheduleName = operationschedulelts.name;
                    //"isOpen": true

                    if (operationschedulelts.openingTimes != null)
                    {
                        operationschedule.OperationScheduleTime = new List<OperationScheduleTime>();

                        //If there are openingtimes given
                        if (operationschedulelts.openingTimes.Count() > 0)
                        {
                            foreach (var openingtimelts in operationschedulelts.openingTimes)
                            {
                                OperationScheduleTime openingtime = new OperationScheduleTime();
                                openingtime.Start = TimeSpan.Parse(openingtimelts.startTime);
                                openingtime.End = TimeSpan.Parse(openingtimelts.endTime);
                                openingtime.Monday = operationschedulelts.isMondayOpen;
                                openingtime.Tuesday = operationschedulelts.isTuesdayOpen;
                                openingtime.Wednesday = operationschedulelts.isWednesdayOpen;
                                openingtime.Thursday = operationschedulelts.isThursdayOpen;
                                openingtime.Friday = operationschedulelts.isFridayOpen;
                                openingtime.Saturday = operationschedulelts.isSaturdayOpen;
                                openingtime.Sunday = operationschedulelts.isSundayOpen;
                                openingtime.Timecode = 1;

                                if (operationschedulelts.isOpen)
                                    openingtime.State = 2;
                                else
                                    openingtime.State = 1;

                                operationschedule.OperationScheduleTime.Add(openingtime);
                            }
                        }
                        else
                        {
                            OperationScheduleTime openingtime = new OperationScheduleTime();
                            openingtime.Start = TimeSpan.Parse("00:00:00");
                            openingtime.End = TimeSpan.Parse("23:59:59");
                            openingtime.Monday = operationschedulelts.isMondayOpen;
                            openingtime.Tuesday = operationschedulelts.isTuesdayOpen;
                            openingtime.Wednesday = operationschedulelts.isWednesdayOpen;
                            openingtime.Thursday = operationschedulelts.isThursdayOpen;
                            openingtime.Friday = operationschedulelts.isFridayOpen;
                            openingtime.Saturday = operationschedulelts.isSaturdayOpen;
                            openingtime.Sunday = operationschedulelts.isSundayOpen;

                            //TODO PARSE type
                            openingtime.Timecode = 1;
                            if (operationschedulelts.isOpen)
                                openingtime.State = 2;
                            else
                                openingtime.State = 1;

                            operationschedule.OperationScheduleTime.Add(openingtime);
                        }
                    }
                    operationschedulelist.Add(operationschedule);
                }

                venue.OperationSchedule = operationschedulelist;
            }


            //Tags Add Categories here
            if (ltsvenue.category != null)
            {
                if (!String.IsNullOrEmpty(ltsvenue.category.rid))
                {
                    if (venue.TagIds == null)
                        venue.TagIds = new List<string>();

                    venue.TagIds.Add("VEN" + ltsvenue.category.rid);
                }
            }

            //Images
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (ltsvenue.images != null)
            {
                foreach (var image in ltsvenue.images)
                {
                    ImageGallery imagepoi = new ImageGallery();

                    imagepoi.ImageName = image.rid;                    
                    imagepoi.ImageSource = "lts";

                    imagepoi.CopyRight = image.copyright;
                    imagepoi.License = image.license;

                    imagepoi.ImageUrl = image.url;
                    imagepoi.IsInGallery = true;

                    imagepoi.Height = image.heightPixel;
                    imagepoi.Width = image.widthPixel;
                    
                    imagepoi.ListPosition = image.order;

                    imagegallerylist.Add(imagepoi);
                }
            }

            venue.ImageGallery = imagegallerylist;
            venue.ImageGallery.AddImageTagsToGallery();


            //Videos

            //Halls

            if (ltsvenue.halls != null)
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
                        if(venueroomdetail.TagIds == null)
                            venueroomdetail.TagIds = new List<string>();

                        foreach(var feature in ltshall.features)
                        {
                            venueroomdetail.TagIds.Add("VEN" + feature.rid);
                        }
                    }

                    //Add Features as Tags
                    if (ltshall.purposesOfUse != null)
                    {
                        if (venueroomdetail.TagIds == null)
                            venueroomdetail.TagIds = new List<string>();

                        //if (venueroomdetail.Tags != null)
                        //    venueroomdetail.Tags = new List<Tags>();

                        foreach (var purposesofuse in ltshall.purposesOfUse)
                        {
                            if (!String.IsNullOrEmpty(purposesofuse.type))
                            {
                                venueroomdetail.TagIds.Add(GetIDPurposeOfUse(purposesofuse.type));

                                //Problem the ID is not here, and we cannot 
                                venueroomdetail.Tags.Add(new Tags() { Id = GetIDPurposeOfUse(purposesofuse.type), Source = "lts", Name = purposesofuse.type, TagEntry = new Dictionary<string, string>() { { "maxCapacity", purposesofuse.maxCapacity.ToString() } } });
                            }
                        }
                    }

                    venueroomdetail.Placement = ltshall.placement;

                    //Images halls
                    //Images
                    List<ImageGallery> hallimagegallerylist = new List<ImageGallery>();

                    if (ltshall.images != null)
                    {
                        foreach (var image in ltshall.images)
                        {
                            ImageGallery imagehall = new ImageGallery();

                            imagehall.ImageName = image.rid;
                            imagehall.ImageSource = "lts";

                            imagehall.CopyRight = image.copyright;
                            imagehall.License = image.license;

                            imagehall.ImageUrl = image.url;
                            imagehall.IsInGallery = true;

                            imagehall.Height = image.heightPixel;
                            imagehall.Width = image.widthPixel;

                            imagehall.ListPosition = image.order;

                            hallimagegallerylist.Add(imagehall);
                        }
                    }

                    venueroomdetail.ImageGallery = hallimagegallerylist;                    


                    venue.RoomDetails.Add(venueroomdetail);
                }
            }

            //Custom Fields
            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsvenue.rid);

            if (ltsvenue.tourismOrganization != null && !String.IsNullOrEmpty(ltsvenue.tourismOrganization.rid))
                ltsmapping.Add("tourismOrganization", ltsvenue.tourismOrganization.rid);

            if (!String.IsNullOrEmpty(ltsvenue.accommodation?.rid))
                ltsmapping.Add("accommodation", ltsvenue.accommodation.rid.ToString());

            if (ltsvenue.district != null && !String.IsNullOrEmpty(ltsvenue.district.rid))
                ltsmapping.Add("district", ltsvenue.district.rid);

            venue.Mapping.TryAddOrUpdate("lts", ltsmapping);

            return venue;
        }

        //TO REMOVE when this info comes from LTS Interface
        public static string GetIDPurposeOfUse(string purposeofUse)
        {
            switch (purposeofUse)
            {
                case "audience": return "VENE4AA0F13DDF1410489F237FF6B181831";
                case "board": return "VEN3D1C707DCA794BBFBAA76ECA49465865";
                case "class": return "VEN0536C64E86B44FC2A2FBA77B6948993D";
                case "cocktail": return "VEN6E29B13DB4B04860B97DC993E203B1CF";
                case "galaDinner": return "VENDB7A4015D38543C5B8971362BDE1986A";
                case "horseshoe": return "VENDF70389DB7C54D57BCA0778AB7AA9C73";
                case "theaterSetup": return "VEN1A7079E1D02E48BE906FEE5CA73B534E";
                case "parliamentarySetup": return "VENA060940F647545F595C4017AA4D79CD0";
                case "circleSetup": return "VENC9296B94D05D4B6E9FFDF363B96842FB";
                default:  return "";
            }
        }
    }

}
