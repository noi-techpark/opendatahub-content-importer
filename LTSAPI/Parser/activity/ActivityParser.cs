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

            odhactivitypoi.LocationInfo = new LocationInfoLinked();

            //Tourism Organization
            odhactivitypoi.TourismorganizationId = ltsactivity.tourismOrganization != null ? ltsactivity.tourismOrganization.rid : null;

            //District Id
            odhactivitypoi.LocationInfo.DistrictInfo = ltsactivity.district != null ? new DistrictInfoLinked() { Id = ltsactivity.district.rid } : null;

            //Rating
            if (ltsactivity.rating != null)
            {
                odhactivitypoi.Ratings = new Ratings()
                {
                    Difficulty = ltsactivity.rating.difficulty != null ? ltsactivity.rating.difficulty.ToString() : null,
                    Landscape = ltsactivity.rating.landscape != null ? ltsactivity.rating.landscape.ToString() : null,
                    Experience = ltsactivity.rating.experience != null ? ltsactivity.rating.experience.ToString() : null,
                    Stamina = ltsactivity.rating.stamina != null ? ltsactivity.rating.stamina.ToString() : null,
                    Technique = ltsactivity.rating.technique != null ? ltsactivity.rating.technique.ToString() : null
                };

                if (ltsactivity.rating.difficulty != null)
                    odhactivitypoi.Difficulty = ltsactivity.rating.difficulty != null ? ltsactivity.rating.difficulty.ToString() : null;
            }

            //GetoData
            if(ltsactivity.geoData != null)
            {
                odhactivitypoi.AltitudeDifference = ltsactivity.geoData.altitudeDifference != null ? ltsactivity.geoData.altitudeDifference.difference : null;
                odhactivitypoi.AltitudeHighestPoint = ltsactivity.geoData.altitudeDifference != null ? ltsactivity.geoData.altitudeDifference.max : null;
                odhactivitypoi.AltitudeLowestPoint = ltsactivity.geoData.altitudeDifference != null ? ltsactivity.geoData.altitudeDifference.min : null;

                odhactivitypoi.AltitudeSumDown = ltsactivity.geoData.distance != null ? ltsactivity.geoData.distance.sumDown : null;
                odhactivitypoi.AltitudeSumUp = ltsactivity.geoData.distance != null ? ltsactivity.geoData.distance.sumUp : null;
                odhactivitypoi.DistanceDuration = ltsactivity.geoData.distance != null ? TimeStringToHours(ltsactivity.geoData.distance.duration) : null; 

                odhactivitypoi.DistanceLength = ltsactivity.geoData.distance != null ? ltsactivity.geoData.distance.length : null;

                odhactivitypoi.Exposition = ltsactivity.geoData.exposition != null ? ltsactivity.geoData.exposition.Select(x => x.value).ToList() : null;

                //Create GPSInfo like before
                odhactivitypoi.GpsInfo = ParserHelper.GetGpsInfoForActivityPoi(ltsactivity.geoData.positions);                

                if (ltsactivity.geoData.gpsTracks != null)
                {
                    foreach (var ltsgpstrack in ltsactivity.geoData.gpsTracks)
                    {
                        if (ltsgpstrack != null)
                        {
                            if (odhactivitypoi.GpsTrack == null)
                                odhactivitypoi.GpsTrack = new List<GpsTrack>();

                            GpsTrack gpstrack = new GpsTrack();
                            gpstrack.Id = ltsgpstrack.rid;                            
                            gpstrack.Type = ParserHelper.GetGpxTrackType(ltsgpstrack.file.url);
                            gpstrack.GpxTrackDesc = ParserHelper.GetGpxTrackDescription(ltsgpstrack.file.url);
                            gpstrack.GpxTrackUrl = ltsgpstrack.file.url;
                            gpstrack.Format = "gpx";

                            odhactivitypoi.GpsTrack.Add(gpstrack);
                        }
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
                detail.Title = ltsactivity.name != null ? ltsactivity.name[language] : null;
                detail.BaseText = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language) : null;
                detail.IntroText = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language) : null;
                detail.ParkingInfo = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language) : null;
                detail.GetThereText = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language) : null;
                
                detail.AdditionalText = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "routeDescription").FirstOrDefault()?.description.GetValue(language) : null;
                detail.PublicTransportationInfo = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "publicTransport").FirstOrDefault()?.description.GetValue(language) : null;
                detail.AuthorTip = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "authorTip").FirstOrDefault()?.description.GetValue(language) : null;
                detail.SafetyInfo = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "safetyInstructions").FirstOrDefault()?.description.GetValue(language) : null;
                detail.EquipmentInfo = ltsactivity.descriptions != null ? ltsactivity.descriptions.Where(x => x.type == "equipment").FirstOrDefault()?.description.GetValue(language) : null;

                odhactivitypoi.Detail.TryAddOrUpdate(language, detail);
            }

            //Contact Information
            foreach (var language in odhactivitypoi.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.Language = language;
                contactinfo.CompanyName = ltsactivity.contact != null && ltsactivity.contact.address != null ? ltsactivity.contact.address.name.GetValue(language) : null;
                contactinfo.Address = ltsactivity.contact != null && ltsactivity.contact.address != null ? ltsactivity.contact.address.street.GetValue(language) : null;
                contactinfo.City = ltsactivity.contact != null && ltsactivity.contact.address != null ? ltsactivity.contact.address.city.GetValue(language) : null;
                contactinfo.CountryCode = ltsactivity.contact != null && ltsactivity.contact.address != null ? ltsactivity.contact.address.country : null;
                contactinfo.CountryName = ParserHelper.GetCountryName(language);
                contactinfo.ZipCode = ltsactivity.contact != null && ltsactivity.contact.address != null ? ltsactivity.contact.address.postalCode : null;
                contactinfo.Email = ltsactivity.contact != null ? ltsactivity.contact.email : null;
                contactinfo.Phonenumber = ltsactivity.contact != null ? ltsactivity.contact.phone : null;
                contactinfo.Url = ltsactivity.contact != null ? ltsactivity.contact.website : null;

                if (ltsactivity.location != null && ltsactivity.location.ContainsKey(language))
                {
                    contactinfo.Area = ltsactivity.location[language];
                }

                odhactivitypoi.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Opening Schedules
            List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();

            if (ltsactivity.openingSchedules != null)
            {
                foreach (var operationschedulelts in ltsactivity.openingSchedules)
                {
                    OperationSchedule operationschedule = new OperationSchedule();
                    operationschedule.Start = Convert.ToDateTime(operationschedulelts.validFrom);
                    operationschedule.Stop = Convert.ToDateTime(operationschedulelts.validTo);
                    operationschedule.Type = ParserHelper.ParseOperationScheduleType(operationschedulelts.type);
                    operationschedule.OperationscheduleName = operationschedulelts.name;
                    operationschedule.OperationscheduleName.RemoveNullValues();

                    if (operationschedulelts.openingTimes != null && operationschedulelts.openingTimes.Count() > 0)
                    {
                        operationschedule.OperationScheduleTime = new List<OperationScheduleTime>();
                        foreach (var openingtimelts in operationschedulelts.openingTimes)
                        {
                            OperationScheduleTime openingtime = new OperationScheduleTime();
                            openingtime.Start = TimeSpan.Parse(openingtimelts.startTime);
                            openingtime.End = TimeSpan.Parse(openingtimelts.endTime);
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

                    operationschedulelist.Add(operationschedule);
                }
                odhactivitypoi.OperationSchedule = operationschedulelist;
            }
            //Tags
            //Fill also LTSTags, and the Tags object for the TINs
            if (ltsactivity.tags != null && ltsactivity.tags.Count() > 0)
            {
                if (odhactivitypoi.TagIds == null)
                    odhactivitypoi.TagIds = new List<string>();

                if (odhactivitypoi.Tags == null)
                    odhactivitypoi.Tags = new List<Tags>();

                if (odhactivitypoi.LTSTags == null)
                    odhactivitypoi.LTSTags = new List<LTSTagsLinked>();

                foreach (var tag in ltsactivity.tags)
                {
                    odhactivitypoi.TagIds.Add(tag.rid);

                    Tags tagdata = new Tags();
                    tagdata.Id = tag.rid;


                    LTSTagsLinked ltstag = new LTSTagsLinked();                    
                    ltstag.LTSRID = tag.rid;

                    if(tag.properties != null && tag.properties.Count() > 0)
                    {
                        ltstag.LTSTins = new List<LTSTins>();
                        tagdata.TagEntry = new Dictionary<string, string>();

                        foreach (var tin in tag.properties)
                        {
                            ltstag.LTSTins.Add(new LTSTins() { LTSRID = tin.rid });                            
                        }

                        tagdata.TagEntry.TryAddOrUpdate("tagproperties", String.Join(",", tag.properties.Select(x => x.rid).ToList()));
                    }

                    odhactivitypoi.LTSTags.Add(ltstag);
                    odhactivitypoi.Tags.Add(tagdata);
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
                    imagepoi.ImageTitle.RemoveNullValues();
                    imagepoi.CopyRight = image.copyright;
                    imagepoi.License = image.license;
                    imagepoi.ImageSource = "lts";

                    imagepoi.ImageUrl = image.url;
                    imagepoi.IsInGallery = true;

                    imagepoi.Height = image.heightPixel;
                    imagepoi.Width = image.widthPixel;
                    imagepoi.ValidFrom = image.applicableStartDate;
                    imagepoi.ValidTo = image.applicableEndDate;
                    imagepoi.ListPosition = image.order;

                    //image.ismainimage?

                    imagegallerylist.Add(imagepoi);
                }
            }

            odhactivitypoi.ImageGallery = imagegallerylist;
            odhactivitypoi.ImageGallery.AddImageTagsToGallery();

            //Videos
            if (ltsactivity.videos != null && ltsactivity.videos.Count() > 0)
            {
                odhactivitypoi.VideoItems = new Dictionary<string, ICollection<VideoItems>>();

                List<VideoItems> allvideoitems = new List<VideoItems>();
                List<string> videolanguages = new List<string>();

                foreach (var videos in ltsactivity.videos)
                {
                    videolanguages = videos.url != null ? videos.url.Where(x => x.Value != null).Select(x => x.Key).ToList() :
                        videos.name != null ? videos.name.Where(x => x.Value != null).Select(x => x.Key).ToList() :
                        videos.name != null ? videos.name.Where(x => x.Value != null).Select(x => x.Key).ToList() : new List<string>();

                    foreach (var lang in videolanguages)
                    {
                        VideoItems videoitem = new VideoItems();
                        videoitem.Active = videos.isActive;
                        videoitem.Bitrate = null;
                        videoitem.CopyRight = videos.copyright;
                        videoitem.Definition = null;
                        videoitem.Duration = null;
                        videoitem.Height = null;
                        videoitem.Language = lang;
                        videoitem.License = videos.license;
                        videoitem.LicenseHolder = null;
                        videoitem.Name = videos.genre.rid;
                        videoitem.Resolution = null;
                        videoitem.StreamingSource = videos.url != null ? videos.url[lang] : null;
                        videoitem.Url = videos.htmlSnippet != null ? videos.htmlSnippet[lang] : null;
                        videoitem.VideoDesc = null;
                        videoitem.VideoSource = videos.source;
                        videoitem.VideoTitle = videos.name != null ? videos.name[lang] : null;
                        videoitem.VideoType = videos.genre.rid;
                        videoitem.Width = null;

                        allvideoitems.Add(videoitem);
                    }
                }
                foreach (var lang in videolanguages)
                {
                    odhactivitypoi.VideoItems.TryAddOrUpdate(lang, allvideoitems.Where(x => x.Language == lang).ToList());
                }
            }



            //Areas
            if (ltsactivity.areas != null)
            {
                odhactivitypoi.AreaId = new HashSet<string>();

                foreach (var area in ltsactivity.areas)
                {
                    odhactivitypoi.AreaId.Add(area.rid);
                }
            }            

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
            odhactivitypoi.LiftAvailable = ltsactivity.hasLift;

            //Novelty
            if (ltsactivity.novelty != null)
            {
                foreach(var novelty in ltsactivity.novelty)
                {
                    if(!String.IsNullOrEmpty(novelty.Value))
                    {
                        if (odhactivitypoi.AdditionalPoiInfos == null)
                            odhactivitypoi.AdditionalPoiInfos = new Dictionary<string, AdditionalPoiInfos>();

                        odhactivitypoi.AdditionalPoiInfos.TryAddOrUpdate(novelty.Key, new AdditionalPoiInfos() { Language = novelty.Key, Novelty = novelty.Value });
                    }
                }
            }


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

            if(!String.IsNullOrEmpty(ltsactivity.code))
                ltsmapping.Add("code", ltsactivity.code);

            if (!String.IsNullOrEmpty(ltsactivity.specificNumberCode))
            {
                ltsmapping.Add("specificNumberCode", ltsactivity.specificNumberCode);
                odhactivitypoi.Number = ltsactivity.specificNumberCode;
            }
            if(ltsactivity.order != null)
                ltsmapping.Add("order", ltsactivity.order.ToString());
            
            if(ltsactivity.mountainBike.isPermitted != null)
                ltsmapping.Add("mountainBike.isPermitted", ltsactivity.mountainBike.isPermitted.ToString());
            if (ltsactivity.mountainBike.officialWayNumber != null)
            {
                ltsmapping.Add("mountainBike.officialWayNumber", ltsactivity.mountainBike.officialWayNumber.ToString());
                odhactivitypoi.WayNumber = ltsactivity.mountainBike.officialWayNumber;
            }
                
            
            if(ltsactivity.tourismOrganization != null)
                ltsmapping.Add("tourismOrganization", ltsactivity.tourismOrganization.rid);

            if (ltsactivity.district != null && !String.IsNullOrEmpty(ltsactivity.district.rid))
                ltsmapping.Add("district", ltsactivity.district.rid);

            if (ltsactivity.rating != null && ltsactivity.rating.viaFerrataTechnique != null)
                ltsmapping.Add("rating.viaFerrataTechnique", ltsactivity.rating.viaFerrataTechnique);
            if (ltsactivity.rating != null && ltsactivity.rating.scaleUIAATechnique != null)
                ltsmapping.Add("rating.scaleUIAATechnique", ltsactivity.rating.scaleUIAATechnique);
            if (ltsactivity.rating != null && ltsactivity.rating.singletrackScale != null)
                ltsmapping.Add("rating.singletrackScale", ltsactivity.rating.singletrackScale);

            if (ltsactivity.liftPointCard != null && ltsactivity.liftPointCard.pointsSingleTripUp != null)
                ltsmapping.Add("liftPointCard.pointsSingleTripUp", ltsactivity.liftPointCard.pointsSingleTripUp.ToString());
            if (ltsactivity.liftPointCard != null && ltsactivity.liftPointCard.pointsSingleTripDown != null)
                ltsmapping.Add("liftPointCard.pointsSingleTripDown", ltsactivity.liftPointCard.pointsSingleTripDown.ToString());
            
            if (ltsactivity.liftType != null)
                ltsmapping.Add("liftType", ltsactivity.liftType);
            if (ltsactivity.liftCapacityType != null)
                ltsmapping.Add("liftCapacityType", ltsactivity.liftCapacityType);

            if (ltsactivity.minRopeLength != null && !string.IsNullOrEmpty(ltsactivity.minRopeLength))
                ltsmapping.Add("minRopeLength", ltsactivity.minRopeLength);
            if (ltsactivity.quantityQuickDraws != null && !string.IsNullOrEmpty(ltsactivity.quantityQuickDraws))
                ltsmapping.Add("quantityQuickDraws", ltsactivity.quantityQuickDraws);
            if (ltsactivity.snowType != null)
                ltsmapping.Add("snowType", ltsactivity.snowType.rid);
            if (ltsactivity.snowPark != null)
            {
                if(ltsactivity.snowPark.hasPipe != null)
                    ltsmapping.Add("snowPark.hasPipe", ltsactivity.snowPark.hasPipe.ToString());
                if (ltsactivity.snowPark.linesNumber != null)
                    ltsmapping.Add("snowPark.linesNumber", ltsactivity.snowPark.linesNumber.ToString());
                if (ltsactivity.snowPark.jumpsNumber != null)
                    ltsmapping.Add("snowPark.jumpsNumber", ltsactivity.snowPark.jumpsNumber.ToString());
                if (ltsactivity.snowPark.isInground != null)
                    ltsmapping.Add("snowPark.isInground", ltsactivity.snowPark.isInground.ToString());
                if (ltsactivity.snowPark.hasArtificiallySnow != null)
                    ltsmapping.Add("snowPark.hasArtificiallySnow", ltsactivity.snowPark.hasArtificiallySnow.ToString());
                if (ltsactivity.snowPark.hasBoarderCross != null) 
                    ltsmapping.Add("snowPark.hasBoarderCross", ltsactivity.snowPark.hasBoarderCross.ToString());                
            }

            ltsmapping.Add("isReadOnly", ltsactivity.isReadOnly.ToString());
            ltsmapping.Add("hasCopyright", ltsactivity.hasCopyright.ToString());

            if (ltsactivity.favouriteFor != null && !string.IsNullOrEmpty(ltsactivity.favouriteFor))
                ltsmapping.Add("favouriteFor", ltsactivity.favouriteFor);

            odhactivitypoi.Mapping.TryAddOrUpdate("lts", ltsmapping);

            //IDM Favorite over Area, to check if this favouriteFor is usable?
            if (ltsactivity.areas != null && ltsactivity.areas.Where(x => x.rid == "EEDD568AC5B14A9DB6BED6C2592483BF").Count() > 0)
                odhactivitypoi.Highlight = true;
            else
                odhactivitypoi.Highlight = false;
            

            //Take the German Shortname if available otherwise use the first available
            odhactivitypoi.Shortname = odhactivitypoi.Detail != null && odhactivitypoi.Detail.Count() > 0 ?
                        odhactivitypoi.Detail.ContainsKey("de") && String.IsNullOrEmpty(odhactivitypoi.Detail["de"].Title) ? odhactivitypoi.Detail["de"].Title :
                    odhactivitypoi.Detail.FirstOrDefault().Value.Title
                    : null;

            //Resort HasLanguage
            odhactivitypoi.HasLanguage = odhactivitypoi.HasLanguage.OrderBy(x => x).ToList();

            //Sync Information
            odhactivitypoi.SyncSourceInterface = "activitydata";
            odhactivitypoi.SyncUpdateMode = "full";

            return odhactivitypoi;
        }

        public static double? TimeStringToHours(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return null;

            if (TimeSpan.TryParse(timeString, out TimeSpan timeSpan))
            {
                return Math.Round(timeSpan.TotalHours, 2);
            }
            else
                return null;
        }        
    }

}
