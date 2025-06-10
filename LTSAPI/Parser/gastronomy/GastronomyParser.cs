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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTSAPI.Parser
{
    public class GastronomyParser
    {
        public static ODHActivityPoiLinked ParseLTSGastronomy(
            JObject ltsdata, bool reduced
            )
        {
            string dataid = "";
            try
            {
                LTSGastronomy gastroltsdetail = ltsdata.ToObject<LTSGastronomy>();

                dataid = gastroltsdetail.data.rid;

                return ParseLTSGastronomy(gastroltsdetail.data, reduced);
            }
            catch(Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "gastronomy.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static ODHActivityPoiLinked ParseLTSGastronomy(
            LTSGastronomyData ltsgastronomy, 
            bool reduced)
        {
            ODHActivityPoiLinked gastronomy = new ODHActivityPoiLinked();

            gastronomy.Id = ltsgastronomy.rid;
            gastronomy._Meta = new Metadata() { Id = gastronomy.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            gastronomy.Source = "lts";

            gastronomy.LastChange = ltsgastronomy.lastUpdate;
            gastronomy.Active = ltsgastronomy.isActive;

            gastronomy.HasLanguage = new List<string>();

            //Let's find out for which languages there is a name (To check if gastronomies without name exists)
            foreach (var desc in ltsgastronomy.contacts.FirstOrDefault().address.name)
            {
                if (!String.IsNullOrEmpty(desc.Value))
                    gastronomy.HasLanguage.Add(desc.Key);
            }

            gastronomy.LocationInfo = new LocationInfoLinked();

            //Tourism Organization, District
            gastronomy.TourismorganizationId = ltsgastronomy.tourismOrganization != null ? ltsgastronomy.tourismOrganization.rid : null;
            gastronomy.LocationInfo.DistrictInfo = ltsgastronomy.district != null && !String.IsNullOrEmpty(ltsgastronomy.district.rid) ? new DistrictInfoLinked() { Id = ltsgastronomy.district.rid } : null;

            gastronomy.MaxSeatingCapacity = ltsgastronomy.maxSeatingCapacity;

            //Categories, Dishrates, Facilities, CeremonyCodes all to Tags
            if(gastronomy.TagIds == null)
                gastronomy.TagIds = new List<string>();

            //Categories
            if (ltsgastronomy.categories != null)
            {
                foreach (var category in ltsgastronomy.categories)
                {
                    if (gastronomy.CategoryCodes == null)
                        gastronomy.CategoryCodes = new List<CategoryCodesLinked>();

                    //to check categorycode has the shortname inside
                    gastronomy.CategoryCodes.Add(new CategoryCodesLinked() { Id = category.rid });
                    gastronomy.TagIds.Add(category.rid);
                }
            }
            //Dishrates
            if (ltsgastronomy.dishRates != null)
            {
                foreach (var dishcode in ltsgastronomy.dishRates)
                {
                    if (gastronomy.DishRates == null)
                        gastronomy.DishRates = new List<DishRatesLinked>();

                    //to check categorycode has the shortname inside
                    gastronomy.DishRates.Add(new DishRatesLinked() { Id = dishcode.dish.rid });
                    //gastronomy.DishRates.Add(new DishRatesV2() { Id = dishcode.dish.rid, MaxAmount = dishcode.maxAmount, MinAmount = dishcode.minAmount });
                    gastronomy.TagIds.Add(dishcode.dish.rid);
                }
            }
            //Facilities
            if (ltsgastronomy.facilities != null)
            {
                foreach (var facility in ltsgastronomy.facilities)
                {
                    if (gastronomy.Facilities == null)
                        gastronomy.Facilities = new List<FacilitiesLinked>();

                    //to check categorycode has the shortname inside
                    gastronomy.Facilities.Add(new FacilitiesLinked() { Id = facility.rid });
                    gastronomy.TagIds.Add(facility.rid);
                }
            }
            //CeremonyCodes
            if (ltsgastronomy.ceremonySeatingCapacities != null)
            {
                foreach (var ceremonycode in ltsgastronomy.ceremonySeatingCapacities)
                {
                    if (gastronomy.CapacityCeremony == null)
                        gastronomy.CapacityCeremony = new List<CapacityCeremonyLinked>();

                    //to check categorycode has the shortname inside
                    gastronomy.CapacityCeremony.Add(new CapacityCeremonyLinked() { Id = ceremonycode.ceremony.rid });
                    //gastronomy.CategoryCodes.Add(new CapacityCeremonyV2() { Id = ceremonycode.ceremony.rid, MaxSeatingCapacity = ceremonycode.maxSeatingCapacity });
                    gastronomy.TagIds.Add(ceremonycode.ceremony.rid);
                }
            }
            
            //Contact Information
            //array here why?
            foreach (var language in gastronomy.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.Language = language;

                if (ltsgastronomy.contacts.Any(x => x.type != null))
                {
                    contactinfo.CompanyName = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().address.name.GetValue(language);
                    contactinfo.Address = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().address.street.GetValue(language);
                    contactinfo.City = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().address.city.GetValue(language);
                    contactinfo.CountryCode = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().address.country;
                    contactinfo.ZipCode = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().address.postalCode;
                    contactinfo.Email = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().email;
                    contactinfo.Phonenumber = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().phone;
                    contactinfo.Url = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().website;
                }
                //on opendata no type is passed
                else
                {
                    contactinfo.CompanyName = ltsgastronomy.contacts.FirstOrDefault().address.name != null ? ltsgastronomy.contacts.FirstOrDefault().address.name.GetValue(language) : null;
                    contactinfo.Address = ltsgastronomy.contacts.FirstOrDefault().address.street != null ? ltsgastronomy.contacts.FirstOrDefault().address.street.GetValue(language) : null;
                    contactinfo.City = ltsgastronomy.contacts.FirstOrDefault().address.city != null ? ltsgastronomy.contacts.FirstOrDefault().address.city.GetValue(language) : null;
                    contactinfo.CountryCode = ltsgastronomy.contacts.FirstOrDefault().address.country != null ? ltsgastronomy.contacts.FirstOrDefault().address.country : null;
                    contactinfo.ZipCode = ltsgastronomy.contacts.FirstOrDefault().postalCode != null ? ltsgastronomy.contacts.FirstOrDefault().address.postalCode : null;
                    contactinfo.Email = ltsgastronomy.contacts.FirstOrDefault().email != null ? ltsgastronomy.contacts.FirstOrDefault().email : null;
                    contactinfo.Phonenumber = ltsgastronomy.contacts.FirstOrDefault().phone != null ? ltsgastronomy.contacts.FirstOrDefault().phone : null;
                    contactinfo.Url = ltsgastronomy.contacts.FirstOrDefault().website != null ? ltsgastronomy.contacts.FirstOrDefault().website : null;
                }

                gastronomy.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Detail Information
            foreach (var language in gastronomy.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Language = language;

                if(ltsgastronomy.contacts.Where(x => x.type == "restaurant").Count() > 0)
                    detail.Title = ltsgastronomy.contacts.Where(x => x.type == "restaurant").FirstOrDefault().address.name.GetValue(language);
                //on opendata no type is passed
                else
                    detail.Title = ltsgastronomy.contacts.FirstOrDefault().address.name.GetValue(language);

                if (ltsgastronomy.description != null)
                    detail.BaseText = ltsgastronomy.description.GetValue(language);
            
                gastronomy.Detail.TryAddOrUpdate(language, detail);
            }

            //Opening Schedules
            if (ltsgastronomy.openingSchedules != null)
            {
                List<OperationSchedule> operationschedulelist = new List<OperationSchedule>();
                foreach (var operationschedulelts in ltsgastronomy.openingSchedules)
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

                            //TODO PARSE type
                            //openingtime.State = 2;
                            //openingtime.Timecode = 1;

                            operationschedule.OperationScheduleTime.Add(openingtime);
                        }
                    }
                }
                gastronomy.OperationSchedule = operationschedulelist;
            }
            
            //Images
            //Images (Main Images with ValidFrom)
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (ltsgastronomy.images != null)
            {
                foreach (var image in ltsgastronomy.images)
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

                    //"isMainImage": true,
                    //"isCurrentMainImage": true,

                    imagegallerylist.Add(imagepoi);
                }
            }

            gastronomy.ImageGallery = imagegallerylist;


            //Videos

            //Position
            if (ltsgastronomy.position != null && ltsgastronomy.position.coordinates.Length == 2)
            {
                if (gastronomy.GpsInfo == null)
                    gastronomy.GpsInfo = new List<GpsInfo>();

                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = ltsgastronomy.position.coordinates[1];
                gpsinfo.Longitude = ltsgastronomy.position.coordinates[0];
                gpsinfo.Altitude = ltsgastronomy.position.altitude;
                gpsinfo.AltitudeUnitofMeasure = "m";

                gastronomy.GpsInfo.Add(gpsinfo);
            }

            //Custom Fields
            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsgastronomy.rid);
            ltsmapping.Add("id", ltsgastronomy.id.ToString());

            if(ltsgastronomy.representationMode != null)
                ltsmapping.Add("representationMode", ltsgastronomy.representationMode);
          
            if (ltsgastronomy.district != null && !String.IsNullOrEmpty(ltsgastronomy.district.rid))
                ltsmapping.Add("district", ltsgastronomy.district.rid);
            if (ltsgastronomy.tourismOrganization != null && !String.IsNullOrEmpty(ltsgastronomy.tourismOrganization.rid))
                ltsmapping.Add("tourismOrganization", ltsgastronomy.tourismOrganization.rid);

            //Adding the Location to the Contactinfo
            //ltsmapping.Add("location_de", ltspoi.location["de"]);
            //ltsmapping.Add("location_it", ltspoi.location["it"]);
            //ltsmapping.Add("location_en", ltspoi.location["en"]);

            gastronomy.Mapping.TryAddOrUpdate("lts", ltsmapping);


            //TODO add the IDM Tags
            gastronomy.SmgTags = new List<string>();
            gastronomy.SmgTags.Add("gastronomy");

            gastronomy.SyncSourceInterface = "gastronomicdata";
            gastronomy.SyncUpdateMode = "full";

            return gastronomy;
        }
    }

}
