using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AccommodationTransformer.Parser
{
    public class AccommodationParser
    {
        public static AccommodationLinked ParseLTSAccommodation(JObject accomodationdetail, bool reduced)
        {
            AccommodationLinked accommodationlinked = new AccommodationLinked();
            
            accommodationlinked.Id = accomodationdetail["rid"].Value<string>();
            accommodationlinked._Meta = new Metadata() { Id = accommodationlinked.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "accommodation", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };           

            //Accommodation Type

            //Accommodation Category

            //Accommodation Detail

            //Address Groups

            //Amenities

            //GPS Info

            //Images

            //Galleries

            //District



            //TODO PARSE ACCOMMODATION
            var name = accomodationdetail["contacts"].Value<JArray>().FirstOrDefault()["address"]["name"].Value<JObject>();
            string namede = "";

            if (name != null)
            {
                JToken token = name["de"];
                if (token != null)
                {
                    namede = token.Value<string>();
                }
            }


            return accommodationlinked;
        }

        public static AccommodationLinked ParseLTSAccommodation(AccoLTS accomodation, bool reduced)
        {
            AccommodationLinked accommodationlinked = new AccommodationLinked();

            accommodationlinked.Id = accomodation.data.rid;
            accommodationlinked._Meta = new Metadata() { Id = accommodationlinked.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "accommodation", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };

            //Accommodation Type
            accommodationlinked.AccoTypeId = accomodation.data.type.rid;

            //Accommodation Category

            //Accommodation Detail

            //Address Groups

            //Amenities

            //GPS Info

            //Images

            //Galleries

            //District

            return accommodationlinked;
        }
    }
}
