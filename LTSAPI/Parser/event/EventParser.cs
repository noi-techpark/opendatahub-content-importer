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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTSAPI.Parser
{
    public class EventParser
    {
        public static (List<EventV2>, VenueV2) ParseLTSEvent(
            JObject eventlts, bool reduced
            )
        {
            try
            {
                LTSEvent eventltsdetail = eventlts.ToObject<LTSEvent>();

                return ParseLTSEvent(eventltsdetail.data, reduced);
            }
            catch(Exception ex)
            {           
                return (null, null);
            }          
        }

        public static (List<EventV2>, VenueV2) ParseLTSEvent(
            LTSEventData eventlts, 
            bool reduced)
        {
            EventV2 eventv2 = new EventV2();

            eventv2.Id = eventlts.rid;
            eventv2._Meta = new Metadata() { Id = eventv2.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "event", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            eventv2.Source = "lts";

            eventv2.LastChange = eventlts.lastUpdate;
            
            //Detail Information

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return (new List<EventV2>() { eventv2 }, new VenueV2());
        }

        public static EventLinked ParseLTSEventV1(
           JObject eventlts, bool reduced
           )
        {
            string eventid = "";

            try
            {
                eventid = eventlts != null ? eventlts["data"]["rid"].ToString() : "";

                LTSEvent eventltsdetail = eventlts.ToObject<LTSEvent>();

                if (eventltsdetail != null && eventltsdetail.data != null)
                    return ParseLTSEventV1(eventltsdetail.data, reduced);                
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(new { operation = "event.parse", id = eventid, source = "lts", success = false, error = true, exception = "Data could not be retrieved from the Source" }));

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "event.parse", id = eventid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }
        }

        public static EventLinked ParseLTSEventV1(
            LTSEventData ltsevent,
            bool reduced)
        {
            EventLinked eventv1 = new EventLinked();

            eventv1.Id = ltsevent.rid;
            //if(reduced)
            //    eventv1.Id = eventv1.Id + "_REDUCED";

            eventv1._Meta = new Metadata() { Id = eventv1.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "event", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            eventv1.Source = "lts";

            eventv1.LastChange = ltsevent.lastUpdate;
            eventv1.FirstImport = ltsevent.createdAt;

            eventv1.Active = ltsevent.isActive != null ? ltsevent.isActive.Value : false;

            if (eventv1.TagIds == null)
                eventv1.TagIds = new List<string>();
            if(eventv1.HasLanguage == null)
                eventv1.HasLanguage = new List<string>();

            //Let's use the lts eventLanguage object
            eventv1.HasLanguage = ltsevent.eventLanguages;

            //Topics
            if (ltsevent.categories != null)
            {
                foreach (var category in ltsevent.categories)
                {
                    if (eventv1.TopicRIDs == null)
                        eventv1.TopicRIDs = new List<string>();

                    eventv1.TopicRIDs.Add(category.rid);
                    eventv1.TagIds.Add(category.rid);
                }
            }            

            //Tags
            if (ltsevent.tags != null)
            {
                foreach (var tag in ltsevent.tags)
                {
                    eventv1.TagIds.Add(tag.rid);
                }
            }

            //Classification
            if (ltsevent.classification != null && !String.IsNullOrEmpty(ltsevent.classification.rid))
                eventv1.TagIds.Add(ltsevent.classification.rid);

            //Districts
            if (ltsevent.districts != null)
            {
                foreach (var dist in ltsevent.districts)
                {
                    if (eventv1.DistrictIds == null)
                        eventv1.DistrictIds = new List<string>();

                    eventv1.DistrictIds.Add(dist.rid);
                }
            }

            //At the first District for LocationInfo
            eventv1.DistrictId = eventv1.DistrictIds != null && eventv1.DistrictIds.Count > 0 ? eventv1.DistrictIds.FirstOrDefault() : null;

            //Detail Information
            foreach (var language in eventv1.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Language = language;
                detail.Title = ltsevent.name[language];

                if (ltsevent.descriptions != null)
                {
                    detail.BaseText = ltsevent.descriptions.Where(x => x.type == "longDescription").FirstOrDefault()?.description.GetValue(language);
                    detail.IntroText = ltsevent.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language);
                    detail.ParkingInfo = ltsevent.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language);
                    detail.GetThereText = ltsevent.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language);

                    detail.AdditionalText = ltsevent.descriptions.Where(x => x.type == "routeDescription").FirstOrDefault()?.description.GetValue(language);
                    detail.PublicTransportationInfo = ltsevent.descriptions.Where(x => x.type == "publicTransport").FirstOrDefault()?.description.GetValue(language);
                    detail.AuthorTip = ltsevent.descriptions.Where(x => x.type == "authorTip").FirstOrDefault()?.description.GetValue(language);
                    detail.SafetyInfo = ltsevent.descriptions.Where(x => x.type == "safetyInstructions").FirstOrDefault()?.description.GetValue(language);
                    detail.EquipmentInfo = ltsevent.descriptions.Where(x => x.type == "equipment").FirstOrDefault()?.description.GetValue(language);
                }
                eventv1.Detail.TryAddOrUpdate(language, detail);

                //EventAdditionalInfos eventadditionalinfos = new EventAdditionalInfos();
                //eventadditionalinfos.Language = language;
                //eventadditionalinfos.


                //eventv1.EventAdditionalInfos.TryAddOrUpdate(language, eventadditionalinfos);



            }

            //Contact Information
            foreach (var language in eventv1.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.Language = language;
                if (ltsevent.contact != null)
                {
                    if (ltsevent.contact.address != null)
                    {
                        contactinfo.CompanyName = ltsevent.contact.address.name.GetValue(language);
                        contactinfo.Address = ltsevent.contact.address.street.GetValue(language);
                        contactinfo.City = ltsevent.contact.address.city.GetValue(language);
                        contactinfo.CountryCode = ltsevent.contact.address.country;
                        contactinfo.ZipCode = ltsevent.contact.address.postalCode;
                    }
                    contactinfo.Email = ltsevent.contact.email.GetValue(language);
                    contactinfo.Phonenumber = ltsevent.contact.phone;
                    contactinfo.Url = ltsevent.contact.website.GetValue(language);
                }
                eventv1.ContactInfos.TryAddOrUpdate(language, contactinfo);
            }

            //Position
            if (ltsevent.position != null && ltsevent.position.coordinates.Length == 2)
            {
                if (eventv1.GpsInfo == null)
                    eventv1.GpsInfo = new List<GpsInfo>();

                GpsInfo gpsinfo = new GpsInfo();
                gpsinfo.Gpstype = "position";
                gpsinfo.Latitude = ltsevent.position.coordinates[1];
                gpsinfo.Longitude = ltsevent.position.coordinates[0];
                gpsinfo.Altitude = ltsevent.position.altitude;
                gpsinfo.AltitudeUnitofMeasure = "m";

                eventv1.GpsInfo.Add(gpsinfo);
            }

            //Images
            //Images (Main Images with ValidFrom)
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (ltsevent.images != null)
            {
                foreach (var image in ltsevent.images)
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
                    imagepoi.ImageSource = "lts";

                    imagegallerylist.Add(imagepoi);
                }
            }

            eventv1.ImageGallery = imagegallerylist;

            //periods
            eventv1.EventDate = new List<EventDate>();

            if (ltsevent.periods != null)
            {
                foreach (var period in ltsevent.periods)
                {
                    EventDate eventdate = new EventDate();
                    eventdate.Active = period.isActive;

                    eventdate.From = Convert.ToDateTime(period.startDate);
                    eventdate.Begin = period.startTime != null ? TimeSpan.Parse(period.startTime) : null;

                    eventdate.To = Convert.ToDateTime(period.endDate);
                    eventdate.End = period.endTime != null ? TimeSpan.Parse(period.endTime) : null;

                    eventdate.Entrance = period.entranceTime != null ? TimeSpan.Parse(period.entranceTime) : null; 

                    eventdate.MaxPersons = period.maxParticipants;
                    eventdate.MinPersons = period.minParticipants;
                    eventdate.DayRID = period.rid;
                    eventdate.PriceFrom = period.minAmount;

                    eventdate.IsCancelled = period.isCancelled;

                    eventdate.Cancelled = eventdate.IsCancelled == true ? "1" : "0";

                    //days
                    foreach (var day in period.days)
                    {
                        if (eventdate.EventCalculatedDays == null)
                            eventdate.EventCalculatedDays = new List<EventDateCalculatedDay>();

                        EventDateCalculatedDay eventcalculatedday = new EventDateCalculatedDay();
                        eventcalculatedday.Day = Convert.ToDateTime(day.startDate);
                        eventcalculatedday.Begin = TimeSpan.Parse(day.startTime);
                        eventcalculatedday.CDayRID = day.rid;

                        if(day.availability != null)
                        {
                            eventcalculatedday.AvailabilityCalculatedValue = day.availability.calculatedAvailability;
                            eventcalculatedday.AvailabilityLow = day.availability.isLowAvailability;
                            eventcalculatedday.SoldOut = day.availability.isSoldOut;

                            if(day.availability.variants != null)
                            {
                                foreach(var variant in day.availability.variants)
                                {
                                    if(eventcalculatedday.EventDateCalculatedDayVariant == null)
                                        eventcalculatedday.EventDateCalculatedDayVariant = new List<EventDateCalculatedDayVariant>();

                                    EventDateCalculatedDayVariant eventcvariant = new EventDateCalculatedDayVariant();
                                    eventcvariant.VariantRID = variant.rid;
                                    eventcvariant.AvailabilityCalculatedValue = variant.calculatedAvailability;
                                    eventcvariant.AvailabilityLow = variant.isLowAvailability;

                                    eventcalculatedday.EventDateCalculatedDayVariant.Add(eventcvariant);
                                }
                            }
                        }

                        
                        eventdate.EventCalculatedDays.Add(eventcalculatedday);
                    }

                    eventdate.SingleDays = period.isEachDayOwnEvent; //TO CHECK

                    //EventDateAdditionalInfo
                    eventdate.EventDateAdditionalInfo = new Dictionary<string, EventDateAdditionalInfo>();

                    //RegistrationWithhin
                    if (period.registrationWithin != null)
                    {
                        foreach(var registrationwithhinelement in period.registrationWithin)
                        {
                            if (!eventdate.EventDateAdditionalInfo.ContainsKey(registrationwithhinelement.Key))
                                eventdate.EventDateAdditionalInfo.TryAdd(registrationwithhinelement.Key, new EventDateAdditionalInfo() { Language = registrationwithhinelement.Key });

                            eventdate.EventDateAdditionalInfo[registrationwithhinelement.Key].RegistrationWithin = registrationwithhinelement.Value;
                        }
                    }
                    //Description
                    if (period.description != null)
                    {
                        foreach (var descriptionelement in period.description)
                        {
                            if (!eventdate.EventDateAdditionalInfo.ContainsKey(descriptionelement.Key))
                                eventdate.EventDateAdditionalInfo.TryAdd(descriptionelement.Key, new EventDateAdditionalInfo() { Language = descriptionelement.Key });

                            eventdate.EventDateAdditionalInfo[descriptionelement.Key].Description = descriptionelement.Value;
                        }
                    }
                    //Guide
                    if (period.guide != null)
                    {
                        foreach (var guidelement in period.guide)
                        {
                            if (!eventdate.EventDateAdditionalInfo.ContainsKey(guidelement.Key))
                                eventdate.EventDateAdditionalInfo.TryAdd(guidelement.Key, new EventDateAdditionalInfo() { Language = guidelement.Key });

                            eventdate.EventDateAdditionalInfo[guidelement.Key].Guide = guidelement.Value;
                        }
                    }
                    //CancellationDescription
                    if (period.cancellationDescription != null)
                    {
                        foreach (var cancellationdescelement in period.cancellationDescription)
                        {
                            if (!eventdate.EventDateAdditionalInfo.ContainsKey(cancellationdescelement.Key))
                                eventdate.EventDateAdditionalInfo.TryAdd(cancellationdescelement.Key, new EventDateAdditionalInfo() { Language = cancellationdescelement.Key });

                            eventdate.EventDateAdditionalInfo[cancellationdescelement.Key].CancellationDescription = cancellationdescelement.Value;
                        }
                    }

                    //TicketSale
                    if(period.ticketSale != null)
                    {
                        eventdate.EventDateTicketInfo = new EventDateTicketInfo();
                        eventdate.EventDateTicketInfo.Active = period.ticketSale.isActive;
                        eventdate.EventDateTicketInfo.OnlineContingent = period.ticketSale.onlineContingent;
                        eventdate.EventDateTicketInfo.OnlineSaleUntil = period.ticketSale.onlineSaleUntil;
                    }

                    //openingHours
                    if (period.openingHours != null)
                    {
                        foreach(var openinghours in period.openingHours)                        
                        {
                            if (eventdate.EventDateOpeningInfo == null)
                                eventdate.EventDateOpeningInfo = new List<EventDateOpeningHour>();

                            EventDateOpeningHour eventdateopeninghour = new EventDateOpeningHour();
                            eventdateopeninghour.Entrance1 = TimeSpan.Parse(openinghours.entranceTime1);
                            eventdateopeninghour.Entrance2 = TimeSpan.Parse(openinghours.entranceTime2);
                            eventdateopeninghour.Begin1 = TimeSpan.Parse(openinghours.startTime1);
                            eventdateopeninghour.Begin2 = TimeSpan.Parse(openinghours.startTime2);
                            eventdateopeninghour.End1 = TimeSpan.Parse(openinghours.endTime1);
                            eventdateopeninghour.End2 = TimeSpan.Parse(openinghours.endTime2);
                            eventdateopeninghour.MondayOpen = openinghours.isMondayOpen;
                            eventdateopeninghour.TuesdayOpen = openinghours.isTuesdayOpen;
                            eventdateopeninghour.WednesdayOpen = openinghours.isWednesdayOpen;
                            eventdateopeninghour.ThursdayOpen = openinghours.isThursdayOpen;
                            eventdateopeninghour.FridayOpen = openinghours.isFridayOpen;
                            eventdateopeninghour.SaturdayOpen = openinghours.isSaturdayOpen;
                            eventdateopeninghour.SundayOpen = openinghours.isSundayOpen;

                            eventdate.EventDateOpeningInfo.Add(eventdateopeninghour);
                        }
                    }

                    //variants
                    if (period.variants != null)
                    {
                        foreach (var variant in period.variants)
                        {
                            if (eventdate.EventVariantIDs == null)
                                eventdate.EventVariantIDs = new List<string>();

                            eventdate.EventVariantIDs.Add(variant.rid);
                        }
                    }

                    eventv1.EventDate.Add(eventdate);
                }
            }
                     
            //publisherSettings
            eventv1.EventPublisher = new List<EventPublisher>();
            if (ltsevent.publisherSettings != null)
            {
                foreach (var publishersetting in ltsevent.publisherSettings)
                {
                    EventPublisher eventpublisher = new EventPublisher();
                    eventpublisher.PublisherRID = publishersetting.publisher.rid;
                    eventpublisher.Ranc = publishersetting.importanceRate;
                    eventpublisher.Publish = ParsePublisherStatus(publishersetting.publicationStatus);

                    eventv1.EventPublisher.Add(eventpublisher);
                }
            }

            eventv1.EventProperty = new DataModel.EventProperty();

            //Classification
            eventv1.EventProperty.EventClassificationId = ltsevent.classification != null ? ltsevent.classification.rid : "";
            eventv1.EventProperty.RegistrationRequired = ltsevent.isRegistrationRequired;
            eventv1.EventProperty.EventOrganizerId = ltsevent.organizer != null ? ltsevent.organizer.rid : "";
            eventv1.EventProperty.TicketRequired = ltsevent.isTicketRequired;
            eventv1.EventProperty.IncludedInSuedtirolGuestPass = ltsevent.isIncludedInSuedtirolGuestPass;


            //meetingPoint   --> Dictionary with string fields
            //location   --> Dictionary with string fields
            //registration   --> Infos about registration Dictionary with string fields
            foreach(var language in eventv1.HasLanguage)
            {
                EventAdditionalInfos eventAdditionalInfos = new EventAdditionalInfos();

                if (eventv1.EventAdditionalInfos != null && eventv1.EventAdditionalInfos.ContainsKey(language))
                    eventAdditionalInfos = eventv1.EventAdditionalInfos[language];

                eventAdditionalInfos.Language = language;
                eventAdditionalInfos.MeetingPoint = ltsevent.meetingPoint != null && ltsevent.meetingPoint.ContainsKey(language) ? ltsevent.meetingPoint[language] : null;
                eventAdditionalInfos.Location = ltsevent.location != null && ltsevent.location.ContainsKey(language) ? ltsevent.location[language] : null;
                eventAdditionalInfos.Registration = ltsevent.registration != null && ltsevent.registration.ContainsKey(language) ? ltsevent.registration[language] : null;

                //To check how to include this descriptions
                //serviceDescription
                //whatToBring
                //cancellationModality

                eventAdditionalInfos.ServiceDescription = ltsevent.descriptions != null && ltsevent.descriptions.Where(x => x.type == "serviceDescription").Count() > 0 ? ltsevent.descriptions.Where(x => x.type == "serviceDescription").FirstOrDefault()?.description.GetValue(language) : null;
                eventAdditionalInfos.WhatToBring = ltsevent.descriptions != null && ltsevent.descriptions.Where(x => x.type == "whatToBring").Count() > 0 ? ltsevent.descriptions.Where(x => x.type == "whatToBring").FirstOrDefault()?.description.GetValue(language) : null;
                eventAdditionalInfos.CancellationModality = ltsevent.descriptions != null && ltsevent.descriptions.Where(x => x.type == "cancellationModality").Count() > 0 ? ltsevent.descriptions.Where(x => x.type == "cancellationModality").FirstOrDefault()?.description.GetValue(language) : null;


                eventv1.EventAdditionalInfos.TryAddOrUpdate(language, eventAdditionalInfos);
            }



            //variants  --> array with object { name: Dictionary string, order int, price double, rid string, variantCategory.rid string}
            if (ltsevent.variants != null)
            {
                //Include EventPrice?

                foreach (var variant in ltsevent.variants)
                {
                    if(eventv1.EventVariants == null)
                        eventv1.EventVariants = new List<EventVariant>();

                    EventVariant eventvariant = new EventVariant();
                    eventvariant.Price = variant.price;
                    eventvariant.VariantId = variant.rid;
                    eventvariant.Order = variant.order;

                    eventvariant.Name = new Dictionary<string, string>();
                    foreach(var variantname in variant.name)
                    {
                        if(variantname.Value != null)
                            eventvariant.Name.TryAddOrUpdate(variantname.Key, variantname.Value);
                    }                        

                    if (variant.variantCategory != null)
                    {
                        eventvariant.VariantCategoryId = variant.variantCategory.rid;
                    }

                    eventv1.EventVariants.Add(eventvariant);
                }
            }

            
            eventv1.EventUrls = new List<EventUrls>();

            //shopConfiguration   ---> bookingurl Dictionary, isActive field
            if (ltsevent.shopConfiguration != null && ltsevent.shopConfiguration.bookingUrl != null)
            {
                EventUrls eventurl = new EventUrls();

                eventurl.Url = new Dictionary<string, string>();
                foreach (var url in ltsevent.shopConfiguration.bookingUrl)
                {
                    //Maybe align also with Haslanguage
                    if (url.Value != null)
                        eventurl.Url.TryAddOrUpdate(url.Key, url.Value);
                }


                eventurl.Type = "bookingUrl";
                eventurl.Active = ltsevent.shopConfiguration.isActive;

                eventv1.EventUrls.Add(eventurl);

                //Compatibility
                //eventBooking.BookingUrl = ltsevent.shopConfiguration.bookingUrl
                eventv1.EventBooking = new EventBooking();
                eventv1.EventBooking.BookingUrl = new Dictionary<string, EventBookingDetail>();
                foreach (var bookingurl in ltsevent.shopConfiguration.bookingUrl)
                {
                    //Maybe align also with Haslanguage
                    if (bookingurl.Value != null)
                        eventv1.EventBooking.BookingUrl.TryAddOrUpdate(bookingurl.Key, new EventBookingDetail() { Url = bookingurl.Value });
                }
            }


            //urlAlias      --> Dictionary with string fields
            if (ltsevent.urlAlias != null)
            {
                EventUrls eventurl = new EventUrls();

                eventurl.Url = new Dictionary<string, string>();
                foreach (var url in ltsevent.urlAlias)
                {
                    //Maybe align also with Haslanguage
                    if(url.Value != null)
                        eventurl.Url.TryAddOrUpdate(url.Key, url.Value);
                }
                
                eventurl.Type = "urlAlias";
                eventurl.Active = true;

                eventv1.EventUrls.Add(eventurl);
            }

            //urls
            if (ltsevent.urls != null)
            {
                foreach(var url in ltsevent.urls)
                {
                    EventUrls eventurl = new EventUrls();
                    eventurl.Url = url.url;
                    eventurl.Type = url.type;
                    eventurl.Active = true;

                    eventv1.EventUrls.Add(eventurl);
                }
            }

            //Custom Fields
            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsevent.rid);
            if(ltsevent.organizer != null)
                ltsmapping.Add("organizer_rid", ltsevent.organizer.rid);

            if (ltsevent.classification != null)
                ltsmapping.Add("classification_rid", ltsevent.classification.rid);

            eventv1.Mapping.TryAddOrUpdate("lts", ltsmapping);


            eventv1.Shortname = eventv1.Detail.FirstOrDefault().Value.Title;

            //Check if success for parsing should be logged
            //Console.WriteLine(JsonConvert.SerializeObject(new { operation = "event.parse", id = ltsevent.rid, source = "lts", success = true, error = false));

            return eventv1;
        }

        private static int ParsePublisherStatus(string status)
        {            
            switch (status)
            {
                case "suggestedForPublication": return 1;
                case "approved": return 2;
                case "rejected": return 3;                
                default: return 0;   
            }
        }
    } 
}


//TO ADD
//events/variantcategories
//events/categories
//events/tags
//events/classifications
//events/dates  ???
//events/organizers