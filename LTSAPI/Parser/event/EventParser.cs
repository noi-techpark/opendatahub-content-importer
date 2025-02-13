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

namespace LTSAPI.Parser
{
    public class EventParser
    {
        public static (List<EventV2>, VenueV2) ParseLTSEvent(
            JObject activitylts, bool reduced
            )
        {
            try
            {
                LTSEvent eventltsdetail = activitylts.ToObject<LTSEvent>();

                return ParseLTSEvent(eventltsdetail.data, reduced);
            }
            catch(Exception ex)
            {           
                return (null, null);
            }          
        }

        public static (List<EventV2>, VenueV2) ParseLTSEvent(
            LTSEventData activity, 
            bool reduced)
        {
            EventV2 eventv2 = new EventV2();

            eventv2.Id = activity.rid;
            eventv2._Meta = new Metadata() { Id = eventv2.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "event", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            eventv2.Source = "lts";

            eventv2.LastChange = activity.lastUpdate;
            
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
           JObject activitylts, bool reduced
           )
        {
            try
            {
                LTSEvent eventltsdetail = activitylts.ToObject<LTSEvent>();

                return ParseLTSEventV1(eventltsdetail.data, reduced);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EventLinked ParseLTSEventV1(
            LTSEventData ltsevent,
            bool reduced)
        {
            EventLinked eventv1 = new EventLinked();

            eventv1.Id = ltsevent.rid;
            eventv1._Meta = new Metadata() { Id = eventv1.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "event", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            eventv1.Source = "lts";

            eventv1.LastChange = ltsevent.lastUpdate;
            eventv1.FirstImport = ltsevent.createdAt;

            eventv1.Active = ltsevent.isActive;

            if (eventv1.TagIds == null)
                eventv1.TagIds = new List<string>();

            //Let's find out for which languages there is a name
            foreach (var name in ltsevent.name)
            {
                if (!String.IsNullOrEmpty(name.Value))
                    eventv1.HasLanguage.Add(name.Key);
            }

            
            //Topics
            foreach (var category in ltsevent.categories)
            {
                if(eventv1.TopicRIDs == null)
                    eventv1.TopicRIDs = new List<string>();

                eventv1.TopicRIDs.Add(category.rid);
                eventv1.TagIds.Add(category.rid);
            }

            //Tags
            foreach (var tag in ltsevent.tags)
            {
                eventv1.TagIds.Add(tag.rid);
            }

            //Districts
            foreach(var dist in ltsevent.districts)
            {
                if(eventv1.DistrictIds == null)
                    eventv1.DistrictIds = new List<string>();

                eventv1.DistrictIds.Add(dist.rid);
            }

            //Detail Information
            foreach (var language in eventv1.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Language = language;
                detail.Title = ltsevent.name[language];
                detail.BaseText = ltsevent.descriptions.Where(x => x.type == "longDescription").FirstOrDefault()?.description.GetValue(language);
                detail.IntroText = ltsevent.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language);
                detail.ParkingInfo = ltsevent.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language);
                detail.GetThereText = ltsevent.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language);
                
                detail.AdditionalText = ltsevent.descriptions.Where(x => x.type == "routeDescription").FirstOrDefault()?.description.GetValue(language);
                detail.PublicTransportationInfo = ltsevent.descriptions.Where(x => x.type == "publicTransport").FirstOrDefault()?.description.GetValue(language);
                detail.AuthorTip = ltsevent.descriptions.Where(x => x.type == "authorTip").FirstOrDefault()?.description.GetValue(language);
                detail.SafetyInfo = ltsevent.descriptions.Where(x => x.type == "safetyInstructions").FirstOrDefault()?.description.GetValue(language);
                detail.EquipmentInfo = ltsevent.descriptions.Where(x => x.type == "equipment").FirstOrDefault()?.description.GetValue(language);
                
                eventv1.Detail.TryAddOrUpdate(language, detail);

                //EventDescAdditional eventdescadditional = new EventDescAdditional();
                //eventdescadditional.


                //eventv1.EventDescAdditional.TryAddOrUpdate(language, eventdescadditional);
                //To check how to include this descriptions
                //serviceDescription
                //whatToBring
                //cancellationModality


            }





            //Contact Information
            foreach (var language in eventv1.HasLanguage)
            {
                ContactInfos contactinfo = new ContactInfos();

                contactinfo.Language = language;
                contactinfo.CompanyName = ltsevent.contact.address.name.GetValue(language);
                contactinfo.Address = ltsevent.contact.address.street.GetValue(language);
                contactinfo.City = ltsevent.contact.address.city.GetValue(language);
                contactinfo.CountryCode = ltsevent.contact.address.country;
                contactinfo.ZipCode = ltsevent.contact.address.postalCode;
                contactinfo.Email = ltsevent.contact.email.GetValue(language);
                contactinfo.Phonenumber = ltsevent.contact.phone;
                contactinfo.Url = ltsevent.contact.website.GetValue(language);

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
                    eventdate.Begin = TimeSpan.Parse(period.startTime);

                    eventdate.To = Convert.ToDateTime(period.endDate);
                    eventdate.End = TimeSpan.Parse(period.endTime);

                    eventdate.Entrance = TimeSpan.Parse(period.entranceTime);

                    eventdate.MaxPersons = period.maxParticipants;
                    eventdate.MinPersons = period.minParticipants;
                    eventdate.DayRID = period.rid;
                    eventdate.PriceFrom = period.minAmount;

                    eventdate.Cancelled = period.isCancelled;
                    
                    //days
                    foreach(var day in period.days)
                    {
                        if (eventdate.EventCalculatedDay == null)
                            eventdate.EventCalculatedDay = new EventDateCalculatedDay();   //Wrong this should be a Array

                        EventDateCalculatedDay eventcalculatedday = new EventDateCalculatedDay();
                        eventcalculatedday.Day = Convert.ToDateTime(day.startDate);
                        eventcalculatedday.Begin = TimeSpan.Parse(day.startTime);
                        eventcalculatedday.CDayRID = day.rid;
                        
                        //day.availability

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

                    //days --> calculated day?

                    //openingHours
                    //ticketSale.isActive
                    //ticketSale.onlineContingent
                    //ticketSale.onlineSaleUntil
                    //variants
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
                    //eventpublisher.Publish = publishersetting.publicationStatus TO CHECK use this or convert it to the int value

                    eventv1.EventPublisher.Add(eventpublisher);
                }
            }

            eventv1.EventProperty = new DataModel.EventProperty();

            //Classification
            eventv1.EventProperty.EventClassificationId = ltsevent.classification.rid;
            eventv1.EventProperty.RegistrationRequired = ltsevent.isRegistrationRequired;
            eventv1.EventProperty.EventOrganizerId = ltsevent.organizer.rid;
            eventv1.EventProperty.TicketRequired = ltsevent.isTicketRequired;
            eventv1.EventProperty.IncludedInSuedtirolGuestPass = ltsevent.isIncludedInSuedtirolGuestPass;


            //meetingPoint   --> Dictionary with string fields
            //location   --> Dictionary with string fields
            //registration   --> Infos about registration Dictionary with string fields

            foreach(var language in eventv1.HasLanguage)
            {
                EventAdditionalInfos eventAdditionalInfos = new EventAdditionalInfos();
                eventAdditionalInfos.Language = language;
                eventAdditionalInfos.MeetingPoint = ltsevent.meetingPoint != null && ltsevent.meetingPoint.ContainsKey(language) ? ltsevent.meetingPoint[language] : null;
                eventAdditionalInfos.Location = ltsevent.location != null && ltsevent.location.ContainsKey(language) ? ltsevent.location[language] : null;
                eventAdditionalInfos.Registration = ltsevent.registration != null && ltsevent.registration.ContainsKey(language) ? ltsevent.registration[language] : null;

                //Maybe rename Mplace, Reg and add Here also cancellationModality, serviceDescription, whatToBring

                eventv1.EventAdditionalInfos.TryAddOrUpdate("language", eventAdditionalInfos);
            }



            //variants  --> array with object { name: Dictionary string, order int, price double, rid string, variantCategory.rid string}
            if (ltsevent.variants != null)
            {
                foreach (var variant in ltsevent.variants)
                {
                    //Include EventPrice?
                }
            }

            

            //shopConfiguration   ---> bookingurl Dictionary, isActive field
            //urls

            if(ltsevent.urls != null)
            {
                foreach(var url in ltsevent.urls)
                {
                    
                    //eventBooking.BookingUrl = ltsevent.shopConfiguration.bookingUrl
                    //urlAlias      --> Dictionary with string fields

                }
            }

         

            



            //Custom Fields
            //Mapping
            var ltsmapping = new Dictionary<string, string>();
            ltsmapping.Add("rid", ltsevent.rid);
            ltsmapping.Add("organizer", ltsevent.organizer.rid);
            ltsmapping.Add("isRegistrationRequired", ltsevent.isRegistrationRequired.ToString());
            ltsmapping.Add("isTicketRequired", ltsevent.isTicketRequired.ToString());

            eventv1.Mapping.TryAddOrUpdate("lts", ltsmapping);

            return eventv1;
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