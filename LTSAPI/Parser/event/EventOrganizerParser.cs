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
    public class EventOrganizerParser
    {
        public static IDictionary<string, ContactInfos> ParseLTSEventOrganizer(
           JObject eventorganizerlts
           )
        {
            string organizerid = "";

            try
            {
                organizerid = eventorganizerlts != null ? eventorganizerlts["data"]["rid"].ToString() : "";

                LTSEventOrganizer eventorganizerltsdetail = eventorganizerlts.ToObject<LTSEventOrganizer>();

                if (eventorganizerltsdetail != null && eventorganizerltsdetail.data != null)
                    return ParseLTSEventOrganizer(eventorganizerltsdetail.data);                
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(new { operation = "event.organizer.parse", id = organizerid, source = "lts", success = false, error = true, exception = "Data could not be retrieved from the Source" }));

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "event.organizer.parse", id = organizerid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }
        }

        public static IDictionary<string, ContactInfos> ParseLTSEventOrganizer(
            LTSEventOrganizerData ltseventorganizer)
        {
            //To check what are we doing with the isActive?


            Dictionary<string, ContactInfos> eventorganizer = new Dictionary<string, ContactInfos>();

            //Let's find out for which languages there is a name
            foreach (var name in ltseventorganizer.name)
            {
                if (name.Value != null)
                {
                    ContactInfos contactinfo = new ContactInfos();
                    contactinfo.Language = name.Key;
                    contactinfo.CompanyName = name.Value;

                    if (ltseventorganizer.contact != null)
                    {
                        if (ltseventorganizer.contact.address != null)
                        {                            
                            contactinfo.Address = ltseventorganizer.contact.address.street != null ? ltseventorganizer.contact.address.street.GetValue(name.Key) : null;
                            contactinfo.City = ltseventorganizer.contact.address.city != null ? ltseventorganizer.contact.address.city.GetValue(name.Key) : null;
                            contactinfo.CountryCode = ltseventorganizer.contact.address.country;
                            contactinfo.ZipCode = ltseventorganizer.contact.address.postalCode;
                        }
                        contactinfo.Email = ltseventorganizer.contact.email;
                        contactinfo.Phonenumber = ltseventorganizer.contact.phone;
                        contactinfo.Url = ltseventorganizer.contact.website;
                        contactinfo.Faxnumber = ltseventorganizer.contact.fax;
                    }


                    eventorganizer.Add(name.Key, contactinfo);
                }
            }
            
            return eventorganizer;
        }
    } 
}