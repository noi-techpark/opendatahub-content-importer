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

            //Let's find out for which languages there is a name
            foreach (var name in ltsevent.name)
            {
                if (!String.IsNullOrEmpty(name.Value))
                    eventv1.HasLanguage.Add(name.Key);
            }

            //Classification
            eventv1.ClassificationRID = ltsevent.classification.rid;
            
            //Topics
            foreach (var category in ltsevent.categories)
            {
                eventv1.TopicRIDs.Add(category.rid);
            }

            //Detail Information

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


            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return eventv1;
        }
    }
}
