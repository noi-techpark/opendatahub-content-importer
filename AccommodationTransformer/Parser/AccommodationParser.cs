using DataModel;
using Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            //Find out all languages the accommodation has, by using contacts.address.name
            var haslanguage = accommodation.data.contacts.Where(x => x.type == "main").FirstOrDefault().address.name.Where(x => !String.IsNullOrEmpty(x.Value)).Select(x => x.Key).ToList();

            accommodationlinked.HasLanguage = haslanguage;

            //General Data
            accommodationlinked.Active = accommodation.data.isActive;
            accommodationlinked.IsBookable = accommodation.data.isBookable;
            accommodationlinked.IsAccommodation = accommodation.data.isAccommodation;
            accommodationlinked.IsCamping = accommodation.data.isCamping;
            accommodationlinked.TVMember = accommodation.data.isTourismOrganizationMember;
            accommodationlinked.TourismVereinId = accommodation.data.tourismOrganization.rid;

            //"representationMode": "full",
            //accommodationlinked.Representation = accommodation.data.representationMode

           
            if (accommodation.data.isSuedtirolInfoActive)
            {
                accommodationlinked.SmgActive = true;
                if (accommodationlinked.PublishedOn == null)
                    accommodationlinked.PublishedOn = new List<string>();

                accommodationlinked.PublishedOn.TryAddOrUpdateOnList("idm-marketplace");
            }                
            else
            {
                accommodationlinked.SmgActive = false;

                if(accommodationlinked.PublishedOn != null)
                    accommodationlinked.PublishedOn.TryRemoveOnList("idm-marketplace");
            }
                
           
            //accommodationlinked.IsGastronomy = fehlt

            accommodationlinked.HasApartment = accommodation.data.hasApartments;
            //accommodationlinked.HasDorm = accommodation.data.hasDorms;  --> to integrate
            //accommodationlinked.HasPitches = accommodation.data.hasPitches;  --> to integrate
            accommodationlinked.HasRoom = accommodation.data.hasRooms;
            accommodationlinked.HasApartment = accommodation.data.hasApartments;
            accommodationlinked.HasApartment = accommodation.data.hasApartments;

            //Mapping

            //Add LTS Id as Mapping
            var ltsriddict = new Dictionary<string, string>() { { "rid", accommodation.data.rid } };
            //Add LTS A0R_ID as Mapping     
            ltsriddict.TryAddOrUpdate("a0r_id", accommodation.data.id.ToString());
            accommodationlinked.Mapping.TryAddOrUpdate("lts", ltsriddict);

            //Add HGV Mapping if present and delete it if no more present
            if (!String.IsNullOrEmpty(accommodation.data.hgvId))
            {
                var hgviddict = new Dictionary<string, string>() { { "id", accommodation.data.hgvId } };
                accommodationlinked.Mapping.TryAddOrUpdate("hgv", hgviddict);
            }
            else
            {
                if (accommodationlinked.Mapping.ContainsKey("hgv"))
                    accommodationlinked.Mapping.Remove("hgv");
            }

            //List used to add certain ids to the features list
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

            List<AccoDetail> myaccodetailslist = new List<AccoDetail>();

            var contactinfo = accommodation.data.contacts.Where(x => x.type == "main").FirstOrDefault();
            
            foreach (string lang in haslanguage)
            {
                AccoDetail mydetail = new AccoDetail();

                if (contactinfo != null)
                {
                    //De Adress
                    mydetail.Language = lang;

                    mydetail.CountryCode = contactinfo.address.country;
                    mydetail.City = contactinfo.address.city[lang];
                    mydetail.Email = contactinfo.email;
                    mydetail.Name = contactinfo.address.name[lang];

                    mydetail.Firstname = contactinfo.address.name2[lang]; ;
                    mydetail.Lastname = contactinfo.address.name2[lang]; ;

                    if (lang == "de")
                        accommodationlinked.Shortname = contactinfo.address.name[lang];

                    mydetail.Street = contactinfo.address.street[lang];

                    mydetail.Fax = contactinfo.fax;

                    //NO MORE PRESENT ON INTERFACE
                    //mydetail.Firstname = ""
                    //mydetail.Lastname = "";
                    //mydetail.Mobile = "";
                    //mydetail.Vat = "";
                    //mydetail.NameAddition = "";


                    mydetail.Phone = contactinfo.phone;
                    mydetail.Zip = contactinfo.address.postalCode;
                    mydetail.Website = contactinfo.website;
                }

                mydetail.Longdesc = accommodation.data.descriptions.Where(x => x.type == "longDescription").FirstOrDefault()?.description[lang];
                mydetail.Shortdesc = accommodation.data.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description[lang];
                
                accommodationlinked.AccoDetail.TryAddOrUpdate(lang, mydetail);
            }

            //Address Groups

            //Amenities

            //GPS Info

            //Images

            //Galleries

            //District
            accommodationlinked.DistrictId = accommodation.data.district.rid;

            return accommodationlinked;
        }
    }
}
