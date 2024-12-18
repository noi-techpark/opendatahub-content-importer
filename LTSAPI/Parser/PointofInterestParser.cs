using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTSAPI.Utils;
using GenericHelper;
using Microsoft.OpenApi.Any;

namespace LTSAPI.Parser
{    
    public class PointofInterestParser
    {
        public static ODHActivityPoiLinked ParseLTSPointofInterest(
            JObject poilts, bool reduced
            )
        {
            try
            {
                LTSPointofInterest poiltsdetail = poilts.ToObject<LTSPointofInterest>();

                return ParseLTSPointofInterest(poiltsdetail.data, reduced);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ODHActivityPoiLinked ParseLTSPointofInterest(
            LTSPointofInterestData ltspoi,
            bool reduced)
        {
            ODHActivityPoiLinked odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = ltspoi.rid;
            odhactivitypoi._Meta = new Metadata() { Id = odhactivitypoi.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            odhactivitypoi.Source = "lts";

            odhactivitypoi.LastChange = ltspoi.lastUpdate;

            //Tourism Organization
            odhactivitypoi.TourismorganizationId = ltspoi.tourismOrganization != null ? ltspoi.tourismOrganization.rid : null;
            odhactivitypoi.LocationInfo = new LocationInfoLinked();

            //Area Id
            if(ltspoi.areas != null)
            {
                odhactivitypoi.AreaId = new HashSet<string>();
                foreach (var area in ltspoi.areas)
                {
                    odhactivitypoi.AreaId.Add(area.rid);
                }
            }     

            odhactivitypoi.LocationInfo.DistrictInfo = new DistrictInfoLinked() { Id = ltspoi.district.rid };

            odhactivitypoi.HasLanguage = new List<string>();
            odhactivitypoi.Detail = new Dictionary<string, Detail>();

            //Let's find out for which languages there is a name
            foreach (var name in ltspoi.name)
            {
                if (!String.IsNullOrEmpty(name.Value))
                    odhactivitypoi.HasLanguage.Add(name.Key);
            }

            //Detail Information
            foreach (var language in odhactivitypoi.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Title = ltspoi.name[language];
                detail.BaseText = ltspoi.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language);
                detail.IntroText = ltspoi.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language);
                detail.ParkingInfo = ltspoi.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language);
                detail.GetThereText = ltspoi.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language);
                detail.IntroText = ltspoi.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language);

                detail.AdditionalText = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.PublicTransportationInfo = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.AuthorTip = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.SafetyInfo = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.EquipmentInfo = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);

                odhactivitypoi.Detail.TryAddOrUpdate(language, detail);
            }

            //Contact Information
            foreach (var language in odhactivitypoi.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.CompanyName = ltspoi.contact.address.name.GetValue(language);
                contactinfo.Address = ltspoi.contact.address.street.GetValue(language);
                contactinfo.City = ltspoi.contact.address.city.GetValue(language);
                contactinfo.CountryCode = ltspoi.contact.address.country;
                contactinfo.ZipCode = ltspoi.contact.address.postalCode;
                contactinfo.Email = ltspoi.contact.email;
                contactinfo.Phonenumber = ltspoi.contact.phone;
                contactinfo.Url = ltspoi.contact.website;

                odhactivitypoi.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Position
            if (ltspoi.position != null && ltspoi.position.coordinates.Length == 2)
            {
                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = ltspoi.position.coordinates[1];
                gpsinfo.Longitude = ltspoi.position.coordinates[0];
                gpsinfo.Altitude = ltspoi.position.altitude;
                gpsinfo.AltitudeUnitofMeasure = "m";

                odhactivitypoi.GpsInfo.Add(gpsinfo);
            }

            //Tags
            if(ltspoi.tags != null && ltspoi.tags.Count() > 0)
            {
                if (odhactivitypoi.Tags == null)
                    odhactivitypoi.Tags = new List<Tags>();

                foreach (var tag in ltspoi.tags)
                {
                    odhactivitypoi.TagIds.Add(tag.rid);
                }
            }
            
            //Images
            //Images (Main Images with ValidFrom)
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (ltspoi.images != null)
            {
                foreach (var image in ltspoi.images)
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

            //Properties
            odhactivitypoi.Active = ltspoi.isActive;
            odhactivitypoi.HasFreeEntrance = ltspoi.hasFreeEntry;
            odhactivitypoi.IsOpen = ltspoi.isOpen;
            odhactivitypoi.IsPrepared = null;
            odhactivitypoi.IsWithLigth = null;
            odhactivitypoi.HasRentals = null;
            odhactivitypoi.FeetClimb = null;
            odhactivitypoi.CopyrightChecked = ltspoi.hasCopyright;

            //ltspoi.isReadOnly ???
            
            //Beacons
            if(ltspoi.beacons != null)
            {
                odhactivitypoi.ChildPoiIds = new List<string>();
                foreach (var beacon in ltspoi.beacons)
                {
                    odhactivitypoi.ChildPoiIds.Add(beacon.rid);
                }
            }

            //Videos
            if(ltspoi.videos != null)
            {
                odhactivitypoi.VideoItems = new Dictionary<string, ICollection<VideoItems>>();

                List<VideoItems> allvideoitems = new List<VideoItems>();
                List<string> videolanguages = new List<string>();

                foreach(var videos in ltspoi.videos)
                {
                    foreach(var lang in videos.url.Keys)
                    {
                        videolanguages.Add(lang);
                        VideoItems videoitem = new VideoItems();
                        videoitem.Active = videos.isActive;
                        videoitem.VideoTitle = videos.name[lang];
                        videoitem.Bitrate = null;
                        videoitem.CopyRight = videos.copyright;
                        videoitem.Definition = null;
                        videoitem.Duration = null;
                        videoitem.Height = null;
                        videoitem.Language = lang;
                        videoitem.License = videos.license;
                        videoitem.LicenseHolder = null;
                        videoitem.Name = null;
                        videoitem.Resolution = null;
                        videoitem.StreamingSource = null;
                        videoitem.Url = null;
                        videoitem.VideoDesc = null;
                        videoitem.VideoSource = null;
                        videoitem.VideoTitle = null;
                        videoitem.VideoType = null;
                        videoitem.Width = null;

                        allvideoitems.Add(videoitem);
                    }                    
                }
                foreach(var lang in videolanguages)
                {
                    odhactivitypoi.VideoItems.TryAddOrUpdate(lang, allvideoitems.Where(x => x.Language == lang).ToList());
                }
            }


            //Opening Schedules
            List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();            
            foreach (var operationschedulelts in ltspoi.openingSchedules)
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


            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltspoi.rid);
            ltsmapping.Add("code", ltspoi.code);
            ltsmapping.Add("favouriteFor", ltspoi.favouriteFor);
            ltsmapping.Add("location_de", ltspoi.location["de"]);
            ltsmapping.Add("location_it", ltspoi.location["it"]);
            ltsmapping.Add("location_en", ltspoi.location["en"]);


            return odhactivitypoi;
        }
    }

}
