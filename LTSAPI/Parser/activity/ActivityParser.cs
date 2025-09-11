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
using LTSAPI.Utils;

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
            LTSActivityData ltsactivity, 
            bool reduced)
        {
            ODHActivityPoiLinked odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = ltsactivity.rid;
            odhactivitypoi._Meta = new Metadata() { Id = odhactivitypoi.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            odhactivitypoi.Source = "lts";

            odhactivitypoi.LastChange = ltsactivity.lastUpdate;

            //Tourism Organization
            odhactivitypoi.TourismorganizationId = ltsactivity.tourismOrganization != null ? ltsactivity.tourismOrganization.rid : null;

            //Rating
            if (ltsactivity.rating != null)
            {
                odhactivitypoi.Ratings = new Ratings()
                {
                    Difficulty = ltsactivity.rating.difficulty.ToString(),
                    Landscape = ltsactivity.rating.landscape.ToString(),
                    Experience = ltsactivity.rating.experience.ToString(),
                    Stamina = ltsactivity.rating.stamina.ToString(),
                    Technique = ltsactivity.rating.technique.ToString()
                };
            }

            //GetoData
            if(ltsactivity.geoData != null)
            {
                odhactivitypoi.AltitudeDifference = ltsactivity.geoData.altitudeDifference.difference;
                odhactivitypoi.AltitudeHighestPoint = ltsactivity.geoData.altitudeDifference.max;
                odhactivitypoi.AltitudeLowestPoint = ltsactivity.geoData.altitudeDifference.min;

                odhactivitypoi.AltitudeSumDown = ltsactivity.geoData.distance.sumDown;
                odhactivitypoi.AltitudeSumUp = ltsactivity.geoData.distance.sumUp;
                //odhactivitypoi.DistanceDuration = ltsactivity.geoData.distance.duration; //TODO Convert To Double

                odhactivitypoi.DistanceLength = ltsactivity.geoData.distance.length;

                odhactivitypoi.Exposition = ltsactivity.geoData.exposition != null ? ltsactivity.geoData.exposition.Select(x => x.value).ToList() : null;

                foreach(var position in ltsactivity.geoData.positions)
                {
                    if (position != null && position.coordinates.Length == 2)
                    {
                        if (odhactivitypoi.GpsInfo == null)
                            odhactivitypoi.GpsInfo = new List<GpsInfo>();

                        GpsInfo gpsinfo = new GpsInfo();
                        gpsinfo.Gpstype = "position";  //TODO transforms the positions.category from lts to opendatahub gpstype
                        gpsinfo.Latitude = position.coordinates[1];
                        gpsinfo.Longitude = position.coordinates[0];
                        gpsinfo.Altitude = position.altitude;
                        gpsinfo.AltitudeUnitofMeasure = "m";

                        odhactivitypoi.GpsInfo.Add(gpsinfo);
                    }
                }

                foreach (var ltsgpstrack in ltsactivity.geoData.gpsTracks)
                {
                    if (ltsgpstrack != null)
                    {
                        if (odhactivitypoi.GpsTrack == null)
                            odhactivitypoi.GpsTrack = new List<GpsTrack>();

                        GpsTrack gpstrack = new GpsTrack();
                        gpstrack.Type = ParserHelper.GetGpxTrackType(ltsgpstrack.file.url);
                        gpstrack.GpxTrackDesc = ParserHelper.GetGpxTrackDescription(ltsgpstrack.file.url);
                        gpstrack.GpxTrackUrl = ltsgpstrack.file.url;
                        gpstrack.Format = "gpx";
              
                        odhactivitypoi.GpsTrack.Add(gpstrack);
                    }
                }
            }

            odhactivitypoi.HasLanguage = new List<string>();
            odhactivitypoi.Detail = new Dictionary<string, Detail>();

            //Let's find out for which languages there is a name
            foreach (var name in ltsactivity.name)
            {
                if (!String.IsNullOrEmpty(name.Value))
                    odhactivitypoi.HasLanguage.Add(name.Key);
            }

            //Detail Information
            foreach (var language in odhactivitypoi.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Language = language;
                detail.Title = ltsactivity.name[language];
                detail.BaseText = ltsactivity.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language);
                detail.IntroText = ltsactivity.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language);
                detail.ParkingInfo = ltsactivity.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language);
                detail.GetThereText = ltsactivity.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language);
                
                detail.AdditionalText = ltsactivity.descriptions.Where(x => x.type == "routeDescription").FirstOrDefault()?.description.GetValue(language);
                detail.PublicTransportationInfo = ltsactivity.descriptions.Where(x => x.type == "publicTransport").FirstOrDefault()?.description.GetValue(language);
                detail.AuthorTip = ltsactivity.descriptions.Where(x => x.type == "authorTip").FirstOrDefault()?.description.GetValue(language);
                detail.SafetyInfo = ltsactivity.descriptions.Where(x => x.type == "safetyInstructions").FirstOrDefault()?.description.GetValue(language);
                detail.EquipmentInfo = ltsactivity.descriptions.Where(x => x.type == "equipment").FirstOrDefault()?.description.GetValue(language);

                odhactivitypoi.Detail.TryAddOrUpdate(language, detail);
            }

            //Contact Information
            foreach (var language in odhactivitypoi.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.Language = language;
                contactinfo.CompanyName = ltsactivity.contact.address.name.GetValue(language);
                contactinfo.Address = ltsactivity.contact.address.street.GetValue(language);
                contactinfo.City = ltsactivity.contact.address.city.GetValue(language);
                contactinfo.CountryCode = ltsactivity.contact.address.country;
                contactinfo.ZipCode = ltsactivity.contact.address.postalCode;
                contactinfo.Email = ltsactivity.contact.email;
                contactinfo.Phonenumber = ltsactivity.contact.phone;
                contactinfo.Url = ltsactivity.contact.website;

                if (ltsactivity.location != null && ltsactivity.location.ContainsKey(language))
                {
                    contactinfo.Area = ltsactivity.location[language];
                }

                odhactivitypoi.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Opening Schedules
            List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();
            foreach (var operationschedulelts in ltsactivity.openingSchedules)
            {
                OperationSchedule operationschedule = new OperationSchedule();
                operationschedule.Start = Convert.ToDateTime(operationschedulelts.validFrom);
                operationschedule.Stop = Convert.ToDateTime(operationschedulelts.validTo);
                operationschedule.Type = ParserHelper.ParseOperationScheduleType(operationschedulelts.type);
                operationschedule.OperationscheduleName = operationschedulelts.name;

                if (operationschedulelts.openingTimes != null)
                {
                    operationschedule.OperationScheduleTime = new List<OperationScheduleTime>();
                    foreach (var openingtimelts in operationschedulelts.openingTimes)
                    {
                        OperationScheduleTime openingtime = new OperationScheduleTime();
                        openingtime.Start = TimeSpan.Parse(openingtimelts.startDate);
                        openingtime.End = TimeSpan.Parse(openingtimelts.endDate);
                        openingtime.Monday = openingtimelts.isMondayOpen;
                        openingtime.Tuesday = openingtimelts.isTuesdayOpen;
                        openingtime.Wednesday = openingtimelts.isWednesdayOpen;
                        openingtime.Thursday = openingtimelts.isThursdayOpen;
                        openingtime.Friday = openingtimelts.isFridayOpen;
                        openingtime.Saturday = openingtimelts.isSaturdayOpen;
                        openingtime.Sunday = openingtimelts.isSundayOpen;
                        openingtime.State = 2;
                        openingtime.Timecode = 1;

                        operationschedule.OperationScheduleTime.Add(openingtime);
                    }
                }
            }
            odhactivitypoi.OperationSchedule = operationschedulelist;

            //Tags
            if (ltsactivity.tags != null && ltsactivity.tags.Count() > 0)
            {
                if (odhactivitypoi.TagIds == null)
                    odhactivitypoi.TagIds = new List<string>();

                foreach (var tag in ltsactivity.tags)
                {
                    odhactivitypoi.TagIds.Add(tag.rid);
                }
            }

            //Images
            //Images (Main Images with ValidFrom)
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (ltsactivity.images != null)
            {
                foreach (var image in ltsactivity.images)
                {
                    ImageGallery imagepoi = new ImageGallery();

                    imagepoi.ImageName = image.rid;
                    imagepoi.ImageTitle = image.name;
                    imagepoi.CopyRight = image.copyright;
                    imagepoi.License = image.license;

                    imagepoi.ImageUrl = image.url;
                    imagepoi.IsInGallery = true;

                    imagepoi.Height = image.heightPixel;
                    imagepoi.Width = image.widthPixel;
                    imagepoi.ValidFrom = image.applicableStartDate;
                    imagepoi.ValidTo = image.applicableEndDate;
                    imagepoi.ListPosition = image.order;

                    //image.ismainimage?

                    ////New Check date and give Image Tag
                    //if (imagepoi.ValidFrom != null && imagepoi.ValidTo != null)
                    //{
                    //    List<string> imagetaglist = new List<string>();

                    //    //Date is set 
                    //    var checkbegindate = ((DateTime)mainimage.ValidFrom).Date;
                    //    var checkenddate = ((DateTime)mainimage.ValidTo).Date;

                    //    var summer = new DateTime(mainimage.ValidFrom.Value.Year, 7, 15).Date;
                    //    var winter = new DateTime(mainimage.ValidTo.Value.Year, 1, 15).Date;

                    //    //check if date is into 15.07
                    //    if (summer >= checkbegindate && summer <= checkenddate)
                    //        imagetaglist.Add("Summer");

                    //    //check if date is into 15.01
                    //    if (winter >= checkbegindate && winter <= checkenddate)
                    //        imagetaglist.Add("Winter");

                    //    mainimage.ImageTags = imagetaglist;
                    //}

                    imagegallerylist.Add(imagepoi);
                }
            }

            odhactivitypoi.ImageGallery = imagegallerylist;

            //Videos

            //Custom Fields
            odhactivitypoi.Active = ltsactivity.isActive;
            odhactivitypoi.RunToValley = ltsactivity.isPossibleRunToValley;
            odhactivitypoi.IsPrepared = ltsactivity.isPrepared;
            odhactivitypoi.HasRentals = ltsactivity.hasRental;
            odhactivitypoi.IsWithLigth = ltsactivity.isIlluminated;
            odhactivitypoi.IsOpen = ltsactivity.isOpen;
            odhactivitypoi.FeetClimb = ltsactivity.isPossibleClimbByFeet;
            odhactivitypoi.BikeTransport = ltsactivity.hasBikeTransport;
            odhactivitypoi.CopyrightChecked = ltsactivity.hasCopyright;


            //code
            //specificNumberCode
            //order
            //mountainBike.isPermitted
            //mountainBike.officialWayNumber
            //rating.viaFerrataTechnique
            //rating.scaleUIAATechnique
            //rating.singletrackScale
            //liftPointCard.pointsSingleTripUp
            //liftPointCard.pointsSingleTripDown
            //hasLift
            //minRopeLength
            //quantityQuickDraws
            //isReadOnly
            //snowType
            //snowPark

            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsactivity.rid);
            ltsmapping.Add("code", ltsactivity.code);
            ltsmapping.Add("specificNumberCode", ltsactivity.specificNumberCode);
            ltsmapping.Add("order", ltsactivity.order.ToString());
            
            if(ltsactivity.mountainBike.isPermitted != null)
                ltsmapping.Add("mountainBike.isPermitted", ltsactivity.mountainBike.isPermitted.ToString());
            if (ltsactivity.mountainBike.officialWayNumber != null)
                ltsmapping.Add("mountainBike.officialWayNumber", ltsactivity.mountainBike.officialWayNumber.ToString());
            
            if(ltsactivity.tourismOrganization != null)
                ltsmapping.Add("tourismOrganization", ltsactivity.tourismOrganization.rid);

            if (ltsactivity.rating != null && ltsactivity.rating.viaFerrataTechnique != null)
                ltsmapping.Add("rating.viaFerrataTechnique", ltsactivity.rating.viaFerrataTechnique);
            if (ltsactivity.rating != null && ltsactivity.rating.scaleUIAATechnique != null)
                ltsmapping.Add("rating.scaleUIAATechnique", ltsactivity.rating.scaleUIAATechnique);
            if (ltsactivity.rating != null && ltsactivity.rating.singletrackScale != null)
                ltsmapping.Add("rating.singletrackScale", ltsactivity.rating.singletrackScale);

            if (ltsactivity.liftPointCard.pointsSingleTripUp != null)
                ltsmapping.Add("liftPointCard.pointsSingleTripUp", ltsactivity.liftPointCard.pointsSingleTripUp.ToString());
            if (ltsactivity.liftPointCard.pointsSingleTripDown != null)
                ltsmapping.Add("liftPointCard.pointsSingleTripDown", ltsactivity.liftPointCard.pointsSingleTripDown.ToString());

            if (ltsactivity.hasLift != null)
                ltsmapping.Add("hasLift", ltsactivity.hasLift.ToString());
            if (ltsactivity.minRopeLength != null)
                ltsmapping.Add("minRopeLength", ltsactivity.minRopeLength);
            if (ltsactivity.quantityQuickDraws != null)
                ltsmapping.Add("quantityQuickDraws", ltsactivity.quantityQuickDraws);
            if (ltsactivity.snowType != null)
                ltsmapping.Add("snowType", ltsactivity.snowType.rid);
            if (ltsactivity.snowPark != null)
            {
                ltsmapping.Add("snowPark.hasPipe", ltsactivity.snowPark.hasPipe.ToString());
                ltsmapping.Add("snowPark.linesNumber", ltsactivity.snowPark.linesNumber.ToString());
                ltsmapping.Add("snowPark.jumpsNumber", ltsactivity.snowPark.jumpsNumber.ToString());
                ltsmapping.Add("snowPark.isInground", ltsactivity.snowPark.isInground.ToString());
                ltsmapping.Add("snowPark.hasArtificiallySnow", ltsactivity.snowPark.hasArtificiallySnow.ToString());
                ltsmapping.Add("snowPark.hasBoarderCross", ltsactivity.snowPark.hasBoarderCross.ToString());
                ltsmapping.Add("snowPark.hasPipe", ltsactivity.snowPark.hasPipe.ToString());                
            }

            ltsmapping.Add("isReadOnly", ltsactivity.isReadOnly.ToString());
            ltsmapping.Add("hasCopyright", ltsactivity.hasCopyright.ToString());
            ltsmapping.Add("favouriteFor", ltsactivity.favouriteFor);

            odhactivitypoi.Mapping.TryAddOrUpdate("lts", ltsmapping);

            return odhactivitypoi;
        }
    }

}
