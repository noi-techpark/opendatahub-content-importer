using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public static AccommodationLinked ParseLTSAccommodation(AccoLTS accommodation, 
            bool reduced,
            XDocument mytypes,
            XDocument mycategories,
            XDocument myboards,
            XDocument myfeatures,
            XDocument mybookingchannels,
            XDocument myvinumlist,
            XDocument mywinelist,
            XDocument mycitylist,
            XDocument myskiarealist,
            XDocument mymediterranenlist,
            XDocument dolomiteslist,
            XDocument alpinelist,
            XDocument roomamenitylist)
        {
            AccommodationLinked accommodationlinked = new AccommodationLinked();

            accommodationlinked.Id = accommodation.data.rid;
            accommodationlinked._Meta = new Metadata() { Id = accommodationlinked.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "accommodation", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            accommodationlinked.Source = "LTS";

            List<string> additionalfeaturestoadd = new List<string>();


            //Accommodation Type
            var mytype = mytypes.Root.Elements("AccoType").Where(x => x.Attribute("RID").Value == accommodation.data.type.rid).FirstOrDefault().Attribute("SmgType").Value;
            accommodationlinked.AccoTypeId = mytype;
            additionalfeaturestoadd.Add(accommodation.data.type.rid);

            //Accommodation Category
            var mycategory = mycategories.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == accommodation.data.category.rid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("T1Des").Value;
            accommodationlinked.AccoCategoryId = accommodation.data.category.rid;

            //Board Infos
            List<string> accoboardings = new List<string>();

            foreach (var myboardelement in accommodation.data.mealPlans)
            {
                additionalfeaturestoadd.Add(myboardelement.rid);

                var myboard = myboards.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == myboardelement.rid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("T1Des").Value;

                if (myboard != null)
                    accoboardings.Add(myboard);
            }
            accommodationlinked.BoardIds = accoboardings.ToList();

            //Accommodation Features
            List<AccoFeatureLinked> featurelist = new List<AccoFeatureLinked>();

            foreach (var tin in accommodation.data.amenities)
            {
                var myfeature = myfeatures.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == tin.rid).FirstOrDefault();

                if (myfeature != null)
                {
                    var myfeatureparsed = myfeature.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault();

                    if (myfeatureparsed != null)
                    {
                        var myfeatureparsed2 = myfeatureparsed.Attribute("T1Des").Value;

                        //Getting HGV ID if available

                        string hgvamenityid = "";

                        //var myamenity = roomamenitylist.Root.Elements("amenity").Elements("ltsrid").Where(x => x.Value == tinrid).FirstOrDefault();

                        var myamenity = roomamenitylist.Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == tin.rid).FirstOrDefault();

                        if (myamenity != null)
                            hgvamenityid = myamenity.Element("hgvid").Value;


                        if (myfeatureparsed2 != null)
                            featurelist.Add(new AccoFeatureLinked() { Id = tin.rid, Name = myfeatureparsed2, HgvId = hgvamenityid });
                    }
                    else
                    {
                        //TODO Error Log
                        //tracesource.TraceEvent(TraceEventType.Error, 0, A0RID + " Error Tin: " + featuretoadd + " not found");
                    }
                }
                else
                {
                    //tracesource.TraceEvent(TraceEventType.Error, 0, A0RID + " Error Tin: " + tinrid + " not found");
                }
            }

            foreach (var featuretoadd in additionalfeaturestoadd)
            {
                var myfeature = myfeatures.Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == featuretoadd).FirstOrDefault();

                if (myfeature != null)
                {
                    var myfeatureparsed = myfeature.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault();

                    if (myfeatureparsed != null)
                    {
                        var myfeatureparsed2 = myfeatureparsed.Attribute("T1Des").Value;

                        //Getting HGV ID if available

                        string hgvamenityid = "";

                        //var myamenity = roomamenitylist.Root.Elements("amenity").Elements("ltsrid").Where(x => x.Value == tinrid).FirstOrDefault();

                        var myamenity = roomamenitylist.Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == featuretoadd).FirstOrDefault();

                        if (myamenity != null)
                            hgvamenityid = myamenity.Element("hgvid").Value;

                        if (myfeatureparsed2 != null)
                            featurelist.Add(new AccoFeatureLinked() { Id = featuretoadd, Name = myfeatureparsed2, HgvId = hgvamenityid });
                    }
                    else
                    {
                       //tracesource.TraceEvent(TraceEventType.Error, 0, A0RID + " Error Tin: " + featuretoadd + " not found");
                    }
                }
                else
                {
                    //tracesource.TraceEvent(TraceEventType.Error, 0, A0RID + " Error Tin: " + featuretoadd + " not found");
                }
            }

            accommodationlinked.Features = featurelist.ToList();

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
