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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTSAPI.Parser
{
    public class GastronomyParser
    {
        public static ODHActivityPoiLinked ParseLTSGastronomy(
            JObject ltsdata, bool reduced,
            IDictionary<string, JArray>? jsonfiles
            )
        {
            string dataid = "";
            try
            {
                LTSGastronomy gastroltsdetail = ltsdata.ToObject<LTSGastronomy>();

                dataid = gastroltsdetail.data.rid;

                return ParseLTSGastronomy(gastroltsdetail.data, reduced, jsonfiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "gastronomy.parse", id = dataid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }
        }

        public static ODHActivityPoiLinked ParseLTSGastronomy(
            LTSGastronomyData ltsgastronomy,
            bool reduced,
            IDictionary<string, JArray>? jsonfiles,
            bool additionalcontactsync = false)
        {
            ODHActivityPoiLinked gastronomy = new ODHActivityPoiLinked();

            gastronomy.Id = ltsgastronomy.rid;
            gastronomy._Meta = new Metadata() { Id = gastronomy.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            gastronomy.Source = "lts";            

            gastronomy.LastChange = ltsgastronomy.lastUpdate;
            gastronomy.Active = ltsgastronomy.isActive;

            gastronomy.HasLanguage = new List<string>();

            //Let's find out for which languages there is a name (To check if gastronomies without name exists)
            if (ltsgastronomy.contacts.Any(x => x.type != null))
            {
                foreach (var gcontacts in ltsgastronomy.contacts)
                {
                    foreach (var desc in gcontacts.address.name)
                    {
                        if (!String.IsNullOrEmpty(desc.Value) && !gastronomy.HasLanguage.Contains(desc.Key))
                            gastronomy.HasLanguage.Add(desc.Key);
                    }
                }
            }

            gastronomy.LocationInfo = new LocationInfoLinked();
            
            //Tourism Organization, District
            gastronomy.TourismorganizationId = ltsgastronomy.tourismOrganization != null ? ltsgastronomy.tourismOrganization.rid : null;
            gastronomy.LocationInfo.DistrictInfo = ltsgastronomy.district != null && !String.IsNullOrEmpty(ltsgastronomy.district.rid) ? new DistrictInfoLinked() { Id = ltsgastronomy.district.rid } : null;

            
            gastronomy.MaxSeatingCapacity = ltsgastronomy.maxSeatingCapacity;

            //Categories, Dishrates, Facilities, CeremonyCodes all to Tags
            if (gastronomy.TagIds == null)
                gastronomy.TagIds = new List<string>();

            //Prefill also Tags with the TagEntry
            if (gastronomy.Tags == null)
                gastronomy.Tags = new List<Tags>();

            //Categories
            if (ltsgastronomy.categories != null)
            {
                foreach (var category in ltsgastronomy.categories)
                {
                    if (category != null && !String.IsNullOrEmpty(category.rid))
                    {
                        var categorycode = jsonfiles != null && jsonfiles["CategoryCodes"] != null ? jsonfiles["CategoryCodes"].Where(x => x["Id"].Value<string>() == category.rid).FirstOrDefault() : null;

                        if (gastronomy.CategoryCodes == null)
                            gastronomy.CategoryCodes = new List<CategoryCodesLinked>();

                        //to check categorycode has the shortname inside
                        gastronomy.CategoryCodes.Add(new CategoryCodesLinked()
                        {
                            Id = category.rid,
                            Shortname = categorycode != null && categorycode["TagName"] != null ? categorycode["TagName"]["de"].Value<string>() : ""
                        });
                        gastronomy.TagIds.Add(category.rid);

                        //Add Tags
                        gastronomy.Tags.Add(new Tags() { Id = category.rid });
                    }
                }
            }
            //Dishrates
            if (ltsgastronomy.dishRates != null)
            {
                 foreach (var dishcode in ltsgastronomy.dishRates)
                 {
                    if (dishcode != null && dishcode.dish != null && !String.IsNullOrEmpty(dishcode.dish.rid))
                    {
                        var dishrate = jsonfiles != null && jsonfiles["DishRates"] != null ? jsonfiles["DishRates"].Where(x => x["Id"].Value<string>() == dishcode.dish.rid).FirstOrDefault() : null;

                        if (gastronomy.DishRates == null)
                            gastronomy.DishRates = new List<DishRatesLinked>();

                        //to check categorycode has the shortname inside
                        gastronomy.DishRates.Add(new DishRatesLinked()
                        {
                            Id = dishcode.dish.rid,
                            MaxAmount = dishcode.maxAmount,
                            MinAmount = dishcode.minAmount,
                            CurrencyCode = "EUR",
                            Shortname = dishrate != null && dishrate["TagName"] != null ? dishrate["TagName"]["de"].Value<string>() : ""
                        });
                        //gastronomy.DishRates.Add(new DishRatesV2() { Id = dishcode.dish.rid, MaxAmount = dishcode.maxAmount, MinAmount = dishcode.minAmount });
                        gastronomy.TagIds.Add(dishcode.dish.rid);

                        //Add Tags
                        gastronomy.Tags.Add(new Tags() { Id = dishcode.dish.rid, TagEntry = new Dictionary<string, string>() { { "MaxAmount", dishcode.maxAmount.ToString() }, { "MinAmount", dishcode.minAmount.ToString() }, { "CurrencyCode", "EUR" } } });
                    }                        
                }
            }
            //Facilities
            if (ltsgastronomy.facilities != null)
            {
                foreach (var facility in ltsgastronomy.facilities)
                {
                    if (facility != null && !String.IsNullOrEmpty(facility.rid))
                    {
                        var facilitycode = jsonfiles != null && jsonfiles["Facilities"] != null ? jsonfiles["Facilities"].Where(x => x["Id"].Value<string>() == facility.rid).FirstOrDefault() : null;

                        if (gastronomy.Facilities == null)
                            gastronomy.Facilities = new List<FacilitiesLinked>();

                        //to check categorycode has the shortname inside
                        gastronomy.Facilities.Add(new FacilitiesLinked()
                        {
                            Id = facility.rid,
                            Shortname = facilitycode != null && facilitycode["TagName"] != null ? facilitycode["TagName"]["de"].Value<string>() : ""
                        });
                        gastronomy.TagIds.Add(facility.rid);

                        //Add Tags
                        gastronomy.Tags.Add(new Tags() { Id = facility.rid });
                    }
                }
            }
            //CeremonyCodes
            if (ltsgastronomy.ceremonySeatingCapacities != null)
            {
                foreach (var ceremonycode in ltsgastronomy.ceremonySeatingCapacities)
                {
                    if (ceremonycode != null && ceremonycode.ceremony != null && !String.IsNullOrEmpty(ceremonycode.ceremony.rid))
                    {
                        var capacityceremony = jsonfiles != null && jsonfiles["CapacityCeremonies"] != null ? jsonfiles["CapacityCeremonies"].Where(x => x["Id"].Value<string>() == ceremonycode.ceremony.rid).FirstOrDefault() : null;

                        if (gastronomy.CapacityCeremony == null)
                            gastronomy.CapacityCeremony = new List<CapacityCeremonyLinked>();

                        //to check categorycode has the shortname inside
                        gastronomy.CapacityCeremony.Add(new CapacityCeremonyLinked()
                        {
                            Id = ceremonycode.ceremony.rid,
                            MaxSeatingCapacity = ceremonycode.maxSeatingCapacity,
                            Shortname = capacityceremony != null && capacityceremony["TagName"] != null ? capacityceremony["TagName"]["de"].Value<string>() : ""
                        });
                        //gastronomy.CategoryCodes.Add(new CapacityCeremonyV2() { Id = ceremonycode.ceremony.rid, MaxSeatingCapacity = ceremonycode.maxSeatingCapacity });
                        gastronomy.TagIds.Add(ceremonycode.ceremony.rid);

                        //Add Tags
                        gastronomy.Tags.Add(new Tags() { Id = ceremonycode.ceremony.rid, TagEntry = new Dictionary<string, string>() { { "MaxSeatingCapacity", ceremonycode.maxSeatingCapacity.ToString() } } });
                    }                       
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
                    contactinfo.CountryName = ParserHelper.GetCountryName(language);
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
                    contactinfo.CountryName = ParserHelper.GetCountryName(language);
                    contactinfo.ZipCode = ltsgastronomy.contacts.FirstOrDefault().postalCode != null ? ltsgastronomy.contacts.FirstOrDefault().address.postalCode : null;
                    contactinfo.Email = ltsgastronomy.contacts.FirstOrDefault().email != null ? ltsgastronomy.contacts.FirstOrDefault().email : null;
                    contactinfo.Phonenumber = ltsgastronomy.contacts.FirstOrDefault().phone != null ? ltsgastronomy.contacts.FirstOrDefault().phone : null;
                    contactinfo.Url = ltsgastronomy.contacts.FirstOrDefault().website != null ? ltsgastronomy.contacts.FirstOrDefault().website : null;
                }

                gastronomy.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Add AdditionalContact
            if (additionalcontactsync)
            {
                if (ltsgastronomy.contacts.Where(x => x.type != "restaurant").Count() > 0)
                {
                    gastronomy.AdditionalContact = new Dictionary<string, List<AdditionalContact>>();


                    foreach (var language in gastronomy.HasLanguage)
                    {
                        List<AdditionalContact> additionalcontactlist = new List<AdditionalContact>();

                        foreach (var ltsgastronomycontact in ltsgastronomy.contacts.Where(x => x.type != null && x.type != "restaurant"))
                        {
                            //Import only if one of the props is not null
                            if (
                                (ltsgastronomycontact.address.name != null && ltsgastronomycontact.address.name[language] != null) ||
                                (ltsgastronomycontact.address.street != null && ltsgastronomycontact.address.street[language] != null) ||
                                (ltsgastronomycontact.address.city != null && ltsgastronomycontact.address.city[language] != null) ||
                                ltsgastronomycontact.address.country != null ||
                                ltsgastronomycontact.postalCode != null ||
                                ltsgastronomycontact.email != null ||
                                ltsgastronomycontact.website != null
                                )
                            {
                                AdditionalContact additionalcontact = new AdditionalContact();

                                additionalcontact.Type = ltsgastronomycontact.type;

                                ContactInfos contactinfoadditional = new ContactInfos();

                                contactinfoadditional.Language = language;

                                contactinfoadditional.CompanyName = ltsgastronomycontact.address.name != null ? ltsgastronomycontact.address.name.GetValue(language) : null;
                                contactinfoadditional.Address = ltsgastronomycontact.address.street != null ? ltsgastronomycontact.address.street.GetValue(language) : null;
                                contactinfoadditional.City = ltsgastronomycontact.address.city != null ? ltsgastronomycontact.address.city.GetValue(language) : null;
                                contactinfoadditional.CountryCode = ltsgastronomycontact.address.country != null ? ltsgastronomycontact.address.country : null;
                                contactinfoadditional.CountryName = ParserHelper.GetCountryName(language);
                                contactinfoadditional.ZipCode = ltsgastronomycontact.postalCode != null ? ltsgastronomycontact.address.postalCode : null;
                                contactinfoadditional.Email = ltsgastronomycontact.email != null ? ltsgastronomycontact.email : null;
                                contactinfoadditional.Phonenumber = ltsgastronomycontact.phone != null ? ltsgastronomycontact.phone : null;
                                contactinfoadditional.Url = ltsgastronomycontact.website != null ? ltsgastronomycontact.website : null;

                                var cnamedict = ltsgastronomycontact.address.name2 != null ? ltsgastronomycontact.address.name2 : null;
                                if(cnamedict != null)
                                {
                                    var cname = cnamedict.GetValue(language);
                                    if(cname != null)
                                        contactinfoadditional.Givenname = cname;
                                }

                                additionalcontact.ContactInfos = contactinfoadditional;
                                additionalcontactlist.Add(additionalcontact);
                            }
                        }

                        if (additionalcontactlist.Count > 0)
                            gastronomy.AdditionalContact.TryAddOrUpdate(language, additionalcontactlist);
                    }
                }
            }

            //Detail Information
            foreach (var language in gastronomy.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Language = language;

                if (ltsgastronomy.contacts.Where(x => x.type == "restaurant").Count() > 0)
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

                                //TODO PARSE type
                                openingtime.Timecode = ParseOperationScheduleType(openingtimelts.type);
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
                    imagepoi.ImageTitle.RemoveNullValues();
                    imagepoi.ImageSource = "lts";

                    imagepoi.CopyRight = image.copyright;
                    imagepoi.License = image.license;

                    imagepoi.ImageUrl = image.url;
                    imagepoi.IsInGallery = true;

                    imagepoi.Height = image.heightPixel;
                    imagepoi.Width = image.widthPixel;
                    imagepoi.ValidFrom = image.applicableStartDate;
                    imagepoi.ValidTo = image.applicableEndDate;
                    imagepoi.ListPosition = image.order;

                    //could be added to ImageTag
                    //"isMainImage": true,
                    //"isCurrentMainImage": true,

                    imagegallerylist.Add(imagepoi);
                }
            }

            gastronomy.ImageGallery = imagegallerylist;
            gastronomy.ImageGallery.AddImageTagsToGallery();

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

            if (ltsgastronomy.representationMode != null)
                ltsmapping.Add("representationMode", ltsgastronomy.representationMode);

            if (ltsgastronomy.district != null && !String.IsNullOrEmpty(ltsgastronomy.district.rid))
                ltsmapping.Add("district", ltsgastronomy.district.rid);
            if (ltsgastronomy.tourismOrganization != null && !String.IsNullOrEmpty(ltsgastronomy.tourismOrganization.rid))
                ltsmapping.Add("tourismOrganization", ltsgastronomy.tourismOrganization.rid);

            if (ltsgastronomy.maxSeatingCapacity > 0)
                ltsmapping.Add("maxSeatingCapacity", ltsgastronomy.maxSeatingCapacity.ToString());

            //Adding also as PoiProperty ?



            //Adding the Location to the Contactinfo
            //ltsmapping.Add("location_de", ltspoi.location["de"]);
            //ltsmapping.Add("location_it", ltspoi.location["it"]);
            //ltsmapping.Add("location_en", ltspoi.location["en"]);

            gastronomy.Mapping.TryAddOrUpdate("lts", ltsmapping);


            //add the IDM Tags
            gastronomy.SmgTags = new List<string>();
            gastronomy.SmgTags.Add("gastronomy");

            gastronomy.SyncSourceInterface = "gastronomicdata";
            gastronomy.SyncUpdateMode = "full";

            //Take the German Shortname if available otherwise use the first available
            gastronomy.Shortname = gastronomy.Detail != null && gastronomy.Detail.Count() > 0 ? 
                        gastronomy.Detail.ContainsKey("de") && String.IsNullOrEmpty(gastronomy.Detail["de"].Title) ? gastronomy.Detail["de"].Title :
                    gastronomy.Detail.FirstOrDefault().Value.Title 
                    : null;

            //Resort HasLanguage
            gastronomy.HasLanguage = gastronomy.HasLanguage.OrderBy(x => x).ToList();

            return gastronomy;
        }

        private static int ParseOperationScheduleType(string openingtimestype)
        {
            switch (openingtimestype)
            {
                case "general":
                    return 1;
                case "meals":
                    return 2;
                case "pizza":
                    return 3;
                case "snacks":
                    return 4;
                default:
                    return 1;

            }
        }
    }
}
