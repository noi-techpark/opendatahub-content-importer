﻿// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;
using GenericHelper;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace LTSAPI.Parser
{
    public class AccommodationParser
    {
        public static AccommodationLinked ParseLTSAccommodation(
            JObject accomodationdetail, bool reduced, 
            IDictionary<string, XDocument> xmlfiles,
            IDictionary<string, JArray> jsonfiles)
        {
            string accoid = "";

            try
            {
                accoid = accomodationdetail != null ? accomodationdetail["data"]["rid"].ToString() : "";

                LTSAcco accoltsdetail = accomodationdetail.ToObject<LTSAcco>();

                if (accoltsdetail != null && accoltsdetail.data != null)
                    return ParseLTSAccommodation(accoltsdetail.data, reduced, xmlfiles, jsonfiles);
                else
                {
                    Console.WriteLine(JsonConvert.SerializeObject(new { operation = "accommodation.parse", id = accoid, source = "lts", success = false, error = true, exception = "Data could not be retrieved from the Source" }));

                    return null;
                }                
            }
            catch(Exception ex)
            {
                //Generate Log Response on Error
                Console.WriteLine(JsonConvert.SerializeObject(new { operation = "accommodation.parse", id = accoid, source = "lts", success = false, error = true, exception = ex.Message }));

                return null;
            }          
        }

        public static AccommodationV2 ParseLTSAccommodation(
            LTSAccoData accommodation, 
            bool reduced,
            IDictionary<string, XDocument> xmlfiles,
            IDictionary<string, JArray> jsonfiles)
        {
            //if the xml files are null reload ít

            AccommodationV2 accommodationlinked = new AccommodationV2();

            accommodationlinked.Id = accommodation.rid;
            accommodationlinked._Meta = new Metadata() { Id = accommodationlinked.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "accommodation", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            accommodationlinked.Source = "lts";

            //Find out all languages the accommodation has, by using contacts.address.name
            var haslanguage = accommodation.contacts.Where(x => x.type == "main").FirstOrDefault().address.name.Where(x => !String.IsNullOrEmpty(x.Value)).Select(x => x.Key).ToList();

            accommodationlinked.HasLanguage = haslanguage;

            //General Data
            accommodationlinked.Active = accommodation.isActive;
            accommodationlinked.TourismVereinId = accommodation.tourismOrganization.rid;

            accommodationlinked.Representation = GetRepresentationmode(accommodation.representationMode);


            if (accommodation.isSuedtirolInfoActive)
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

            AccoProperties accoproperties = new AccoProperties();
            accoproperties.HasApartment = accommodation.hasApartments;
            accoproperties.HasDorm = accommodation.hasDorms;
            accoproperties.HasPitches = accommodation.hasPitches;
            accoproperties.HasRoom = accommodation.hasRooms;
            accoproperties.HasApartment = accommodation.hasApartments;
            accoproperties.IsBookable = accommodation.isBookable;
            accoproperties.IsAccommodation = accommodation.isAccommodation;
            accoproperties.IsCamping = accommodation.isCamping;
            accoproperties.TVMember = accommodation.isTourismOrganizationMember;

            accommodationlinked.AccoProperties = accoproperties;

            //Overview
            if (accommodation.overview != null)
            {
                AccoOverview accooverview = new AccoOverview();

                //int.TryParse(accommodation.data.overview.singl, out int singleroomqty);
                //accooverview.SingleRooms = singleroomqty;

                //int.TryParse(hoteloverview.Attribute("A4Dbr").Value, out int doubleroomqty);
                //accooverview.DoubleRooms = doubleroomqty;

                //int.TryParse(hoteloverview.Attribute("A4Tbr").Value, out int tripleroomqty);
                //accooverview.TripleRooms = tripleroomqty;

                //int.TryParse(hoteloverview.Attribute("A4Qbr").Value, out int quadrupleroomqty);
                //accooverview.QuadrupleRooms = quadrupleroomqty;

                //accooverview.TotalRooms = singleroomqty + doubleroomqty + tripleroomqty + quadrupleroomqty;

                //int.TryParse(hoteloverview.Attribute("A4App").Value, out int apartmentqty);
                //accooverview.Apartments = apartmentqty;

                //int.TryParse(hoteloverview.Attribute("A4BAp").Value, out int apartmentbedqty);
                //accooverview.ApartmentBeds = apartmentbedqty;

                //int.TryParse(hoteloverview.Attribute("A4ToP").Value, out int maxpersonqty);
                //accooverview.MaxPersons = maxpersonqty;

                //--> REMOVED BY LTS
               
                accooverview.OutdoorParkings = accommodation.overview.parkingSpaces.outdoor;
                accooverview.GarageParkings = accommodation.overview.parkingSpaces.garage;

                //TO check
                TimeSpan.TryParse(accommodation.overview.checkInStartTime, out TimeSpan checkinfrom);
                accooverview.CheckInFrom = checkinfrom;

                TimeSpan.TryParse(accommodation.overview.checkInEndTime, out TimeSpan checkinto);
                accooverview.CheckInTo = checkinto;

                //When Checkout Property is ready on LTS Side
                TimeSpan.TryParse(accommodation.overview.checkOutStartTime, out TimeSpan checkoutfrom);
                accooverview.CheckOutFrom = checkoutfrom;

                //When Checkout Property is ready on LTS Side
                TimeSpan.TryParse(accommodation.overview.checkOutEndTime, out TimeSpan checkoutto);
                accooverview.CheckOutTo = checkoutto;

                TimeSpan.TryParse(accommodation.overview.receptionStartTime, out TimeSpan receptionopenfrom);
                accooverview.ReceptionOpenFrom = receptionopenfrom;

                TimeSpan.TryParse(accommodation.overview.receptionEndTime, out TimeSpan receptionopento);
                accooverview.ReceptionOpenTo = receptionopento;

                TimeSpan.TryParse(accommodation.overview.roomServiceStartTime, out TimeSpan roomservicefrom);
                accooverview.RoomServiceFrom = roomservicefrom;

                TimeSpan.TryParse(accommodation.overview.roomServiceEndTime, out TimeSpan roomserviceto);
                accooverview.RoomServiceTo = roomserviceto;

                TimeSpan.TryParse(accommodation.overview.luggageServiceStartTime, out TimeSpan baggageservicefrom);
                accooverview.BaggageServiceFrom = baggageservicefrom;

                TimeSpan.TryParse(accommodation.overview.luggageServiceEndTime, out TimeSpan baggageserviceto);
                accooverview.BaggageServiceTo = baggageserviceto;

                
                accooverview.CampingUnits = accommodation.overview.camping.pitches;

                accooverview.CampingWashrooms = accommodation.overview.camping.laundrySpaces;
               
                accooverview.CampingDouches = accommodation.overview.camping.showers;

                accooverview.CampingToilettes = accommodation.overview.camping.toilets;

                accooverview.CampingWashingstands = accommodation.overview.camping.dishwashingSpaces;

                accooverview.ApartmentRoomSize = accommodation.overview.camping.capacityPersons; //? to check

                accommodationlinked.AccoOverview = accooverview;
            }

            //Mapping

            //Add LTS Id as Mapping
            var ltsriddict = new Dictionary<string, string>() { { "rid", accommodation.rid } };
            //Add LTS A0R_ID as Mapping     
            ltsriddict.TryAddOrUpdate("a0r_id", accommodation.id.ToString());
            ltsriddict.TryAddOrUpdate("representationMode", accommodation.representationMode);
            //Add Cin Code
            if(!string.IsNullOrEmpty(accommodation.cinCode))
                ltsriddict.TryAddOrUpdate("cincode", accommodation.cinCode);

            //Add Mapping 

            accommodationlinked.Mapping.TryAddOrUpdate("lts", ltsriddict);
                      
            //Add HGV Mapping if present and delete it if no more present
            if (!String.IsNullOrEmpty(accommodation.hgvId))
            {
                var hgviddict = new Dictionary<string, string>() { { "id", accommodation.hgvId } };
                accommodationlinked.Mapping.TryAddOrUpdate("hgv", hgviddict);

                accommodationlinked.HgvId = accommodation.hgvId;

                //Adding POS Info
                accommodationlinked.AccoBookingChannel =
                [
                    new AccoBookingChannel() { BookingId = accommodation.hgvId, Id = "hgv", Portalname = "HGV Booking", Pos1ID = "2" },
                ];
            }
            else
            {
                if (accommodationlinked.Mapping.ContainsKey("hgv"))
                    accommodationlinked.Mapping.Remove("hgv");

                accommodationlinked.HgvId = null;

                //Remove POS Info
                if(accommodationlinked.AccoBookingChannel != null)
                    accommodationlinked.AccoBookingChannel = null;
            }

            //List used to add certain ids to the features list
            List<string> additionalfeaturestoadd = new List<string>();

            //Accommodation Type
            var mytype = xmlfiles["AccoTypes"].Root.Elements("AccoType").Where(x => x.Attribute("RID").Value == accommodation.type.rid).FirstOrDefault().Attribute("SmgType").Value;
            accommodationlinked.AccoTypeId = mytype;
            additionalfeaturestoadd.Add(accommodation.type.rid);

            //Accommodation Category
            var mycategory = xmlfiles["AccoCategories"].Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == accommodation.category.rid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("T1Des").Value;
            accommodationlinked.AccoCategoryId = accommodation.category.rid;
            additionalfeaturestoadd.Add(accommodation.category.rid);  //to check


            //Board Infos
            List<string> accoboardings = new List<string>();

            foreach (var myboardelement in accommodation.mealPlans)
            {
                additionalfeaturestoadd.Add(myboardelement.rid);

                var myboard = xmlfiles["Boards"].Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == myboardelement.rid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("T1Des").Value;

                if (myboard != null)
                    accoboardings.Add(myboard);
            }
            accommodationlinked.BoardIds = accoboardings.ToList();

            //Accommodation Features
            List<AccoFeatureLinked> featurelist = new List<AccoFeatureLinked>();

            foreach (var tin in accommodation.amenities)
            {
                var myfeature = jsonfiles["Features"].Where(x => x["rid"].Value<string>() == tin.rid).FirstOrDefault();

                if (myfeature != null)
                {
                    var myfeatureparsed = myfeature["name"]["en"].Value<string>();

                    if (myfeatureparsed != null)
                    {                        
                        //Getting HGV ID if available

                        string hgvamenityid = "";

                        //var myamenity = roomamenitylist.Root.Elements("amenity").Elements("ltsrid").Where(x => x.Value == tinrid).FirstOrDefault();

                        var myamenity = xmlfiles["RoomAmenities"].Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == tin.rid).FirstOrDefault();

                        if (myamenity != null)
                            hgvamenityid = myamenity.Element("hgvid").Value;

                        featurelist.Add(new AccoFeatureLinked() { Id = tin.rid, Name = myfeatureparsed, HgvId = hgvamenityid });
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

            //GuestPass (Adding to features, to check if we should add it to Tags)
            var guestpass = accommodation.suedtirolGuestPass;
            if(guestpass != null && guestpass.isActive)
            {                
                foreach (var cardtype in guestpass.cardTypes)
                {
                    //? this needed
                    additionalfeaturestoadd.Add(cardtype.rid);
                    //Adding Guestcard To Tags
                    accommodationlinked.TagIds.Add(cardtype.rid);
                }

                //Compatiblity features to Tags             
                //Check if cardtypes should be assigned
                foreach (var guestcard in GetGuestCardIdMapping())
                {
                    if (accommodation.amenities != null && accommodation.amenities.Select(x => x.rid).Contains(guestcard.Key))
                        accommodationlinked.SmgTags.TryAddOrUpdateOnList(guestcard.Value);
                    else
                        accommodationlinked.SmgTags.TryRemoveOnList(guestcard.Value);
                }
            }

            //IF guestcard active Add Tag "guestcard" else remove it if present
            if (guestpass.isActive == false)
            {
                accommodationlinked.SmgTags.TryRemoveOnList("guestcard");
                accommodationlinked.TagIds.TryRemoveOnList("guestcard");
            }
            else
            {
                accommodationlinked.SmgTags.TryAddOrUpdateOnList("guestcard");
                accommodationlinked.TagIds.TryAddOrUpdateOnList("guestcard");
            }


            //Address Groups (Adding to features)
            foreach (var addressgroup in accommodation.addressGroups)
                additionalfeaturestoadd.Add(addressgroup.rid);

            //TO Check adding Address Groups as Marketinggroups
            accommodationlinked.MarketingGroupIds = new List<string>();
            foreach (var addressgroup in accommodation.addressGroups)
            {
                accommodationlinked.MarketingGroupIds.Add(addressgroup.rid);
            }

            foreach (var featuretoadd in additionalfeaturestoadd)
            {
                var myfeature = xmlfiles["Features"].Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == featuretoadd).FirstOrDefault();

                if (myfeature != null)
                {
                    var myfeatureparsed = myfeature.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault();

                    if (myfeatureparsed != null)
                    {
                        var myfeatureparsed2 = myfeatureparsed.Attribute("T1Des").Value;

                        //Getting HGV ID if available

                        string hgvamenityid = "";

                        //var myamenity = roomamenitylist.Root.Elements("amenity").Elements("ltsrid").Where(x => x.Value == tinrid).FirstOrDefault();

                        var myamenity = xmlfiles["RoomAmenities"].Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == featuretoadd).FirstOrDefault();

                        if (myamenity != null)
                            hgvamenityid = myamenity.Element("hgvid").Value;

                        if (myfeatureparsed2 != null)
                            featurelist.Add(new AccoFeatureLinked() { Id = featuretoadd, Name = myfeatureparsed2, HgvId = hgvamenityid });
                    }
                    else
                    {
                        //TODO Add here the guestcard logic

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

            if (accommodation.contacts != null)
            {
                var contactinfo = accommodation.contacts.Where(x => x.type == "main").FirstOrDefault();

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

                        mydetail.Firstname = contactinfo.address.name2[lang];
                        mydetail.Lastname = contactinfo.address.name2[lang];

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

                    mydetail.Longdesc = accommodation.descriptions.Where(x => x.type == "longDescription").FirstOrDefault()?.description[lang];
                    mydetail.Shortdesc = accommodation.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description[lang];

                    accommodationlinked.AccoDetail.TryAddOrUpdate(lang, mydetail);
                }
            }
            //GPS Info
            if(accommodation.position != null)
            {
                accommodationlinked.GpsInfo = new List<GpsInfo>();

                GpsInfo mygps = new GpsInfo();
                mygps.Longitude = accommodation.position.coordinates[0];
                mygps.Latitude = accommodation.position.coordinates[1];
                mygps.Altitude = accommodation.position.altitude;
                mygps.Gpstype = "position";
                mygps.AltitudeUnitofMeasure = "m";

                accommodationlinked.GpsInfo.Add(mygps);
            }

            //Images (Main Images with ValidFrom)
            List<ImageGallery> imagegallerylist = new List<ImageGallery>();

            if (accommodation.images != null)
            {
                foreach (var image in accommodation.images)
                {
                    ImageGallery mainimage = new ImageGallery();

                    //DE
                    mainimage.ImageUrl = image.url;
                    mainimage.IsInGallery = true;
                    mainimage.ImageSource = "lts";
                    mainimage.Height = image.heightPixel;
                    mainimage.Width = image.widthPixel;
                    mainimage.ValidFrom = image.applicableStartDate;
                    mainimage.ValidTo = image.applicableEndDate;
                    mainimage.ListPosition = 0;
                    //Not offered
                    //mainimagede.ImageDesc.TryAddOrUpdate("de", theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").Count() > 0 ? theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "DE").FirstOrDefault().Attribute("A9Nam").Value : "");
                    //mainimagede.ImageDesc.TryAddOrUpdate("it", theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "IT").Count() > 0 ? theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "IT").FirstOrDefault().Attribute("A9Nam").Value : "");
                    //mainimagede.ImageDesc.TryAddOrUpdate("en", theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").Count() > 0 ? theimage.Elements("DataLng").Where(x => x.Attribute("LngID").Value == "EN").FirstOrDefault().Attribute("A9Nam").Value : "");

                    mainimage.CopyRight = image.copyright;
                    mainimage.License = image.license;

                    //New Check date and give Image Tag
                    if (mainimage.ValidFrom != null && mainimage.ValidTo != null)
                    {
                        List<string> imagetaglist = new List<string>();

                        //Date is set 
                        var checkbegindate = ((DateTime)mainimage.ValidFrom).Date;
                        var checkenddate = ((DateTime)mainimage.ValidTo).Date;

                        var summer = new DateTime(mainimage.ValidFrom.Value.Year, 7, 15).Date;
                        var winter = new DateTime(mainimage.ValidTo.Value.Year, 1, 15).Date;

                        //check if date is into 15.07
                        if (summer >= checkbegindate && summer <= checkenddate)
                            imagetaglist.Add("Summer");

                        //check if date is into 15.01
                        if (winter >= checkbegindate && winter <= checkenddate)
                            imagetaglist.Add("Winter");

                        mainimage.ImageTags = imagetaglist;
                    }

                    imagegallerylist.Add(mainimage);
                }
            }
            //Galleries

            if (accommodation.galeries != null)
            {
                foreach (var gallery in accommodation.galeries.Where(x => x.isActive))
                {
                    foreach (var image in gallery.images)
                    {
                        ImageGallery mygallery = new ImageGallery();

                        mygallery.ImageUrl = image.url;
                        mygallery.ListPosition = image.order;
                        mygallery.IsInGallery = image.isActive;
                        mygallery.ImageSource = "lts";
                        mygallery.Height = image.heightPixel;
                        mygallery.Width = image.widthPixel;

                        mygallery.CopyRight = image.copyright;
                        mygallery.License = image.license;

                        //TODO Image Descriptions not in Interface?

                        imagegallerylist.Add(mygallery);

                    }
                }
            }
            accommodationlinked.ImageGallery = imagegallerylist.ToList();

            //Reorder Image Gallery
            if (accommodationlinked.ImageGallery.Count() > 0)
            {
                accommodationlinked.ImageGallery = accommodationlinked.ImageGallery.OrderBy(x => x.ListPosition).ToList();
            }

            //District
            accommodationlinked.DistrictId = accommodation.district.rid;

            //Reviews
            var trustyourating = accommodation.reviews != null ? accommodation.reviews.Where(x => x.type == "trustyou").FirstOrDefault() : null;
            if(trustyourating != null)
            {
                var review = new DataModel.Review();

                review.ReviewId = trustyourating.id;
                review.Results = trustyourating.reviewsQuantity != null ? trustyourating.reviewsQuantity.Value : 0;

                review.Score = trustyourating.rating;
                review.Active = trustyourating.isActive;

                review.State = trustyourating.status;
                review.StateInteger = GetTrustYouState(trustyourating.status);

                review.Provider = "trustyou";

                if(accommodationlinked.Review == null)
                    accommodationlinked.Review = new Dictionary<string, DataModel.Review>();

                accommodationlinked.Review.TryAddOrUpdate("trustyou", review);
            }
            
            //Accessibility Independent Data
            IndependentData independentdata = new IndependentData();

            var independentrating = accommodation.reviews != null ? accommodation.reviews.Where(x => x.type == "independent").FirstOrDefault() : null;

            if(independentrating != null)
            {
                independentdata.Enabled = independentrating.isActive;
                independentdata.IndependentRating = Convert.ToInt32(independentrating.rating);
                
                foreach (var lang in haslanguage)
                {
                    IndependentDescription independentdetail = new IndependentDescription();
                    independentdetail.Language = lang;
                    independentdetail.BacklinkUrl = accommodation.accessibility.website[lang];
                    independentdetail.Description = accommodation.accessibility.website[lang];

                    independentdata.IndependentDescription.TryAddOrUpdate(lang, independentdetail);
                }
            }

            //Seasons
            List<OperationSchedule> operationschedules = new List<OperationSchedule>();
            if (accommodation.seasons != null)
            {
                foreach (var season in accommodation.seasons)
                {                   
                    OperationSchedule schedule = new OperationSchedule();
                    schedule.Start = Convert.ToDateTime(season.startDate);
                    schedule.Stop = Convert.ToDateTime(season.endDate);
                    schedule.Type = "1";
                    schedule.OperationscheduleName = new Dictionary<string, string>() {
                        { "de","" },
                        { "it","" },
                        { "en","" }
                    };

                    operationschedules.Add(schedule);
                }
            }
            accommodationlinked.OperationSchedule = operationschedules;

            //Rate Plans
            List<RatePlan> rateplans = new List<RatePlan>();

            if (accommodation.ratePlans != null)
            {
                foreach(var rateplanlts in accommodation.ratePlans)
                {
                    RatePlan rateplan = new RatePlan();
                    rateplan.RatePlanId = rateplanlts.rid;
                    rateplan.Name = rateplanlts.name;

                    //rateplan.Description = rateplanlts.descriptions;

                    rateplan.LongDescription = rateplanlts.descriptions.Where(x => x.type == "longDescription").FirstOrDefault()?.description;
                    rateplan.ShortDescription = rateplanlts.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description;

                    rateplan.Visibility = rateplanlts.visibility;
                    rateplan.Code = rateplanlts.code;
                    rateplan.ChargeType = rateplanlts.chargeType;
                    rateplan.LastUpdate = rateplanlts.lastUpdate;

                    rateplans.Add(rateplan);
                }
            }
            accommodationlinked.RatePlan = rateplans;

            //Take the German Shortname if available otherwise use the first available
            accommodationlinked.Shortname = accommodationlinked.AccoDetail != null && accommodationlinked.AccoDetail.Count() > 0 ?
                        accommodationlinked.AccoDetail.ContainsKey("de") && String.IsNullOrEmpty(accommodationlinked.AccoDetail["de"].Name) ? accommodationlinked.AccoDetail["de"].Name :
                    accommodationlinked.AccoDetail.FirstOrDefault().Value.Name
                    : null;

            //Resort HasLanguage
            accommodationlinked.HasLanguage = accommodationlinked.HasLanguage.OrderBy(x => x).ToList();            


            //Special Operations for IDM
            //Special (Mapping Features to Marketinggroup) (B79228E62B5A4D14B2BF35E7B79B8580 ) + 2 (B5757D0688674594955606382A5E126C)  + 3 (31F741E8D6D8444A9BB571A2DF193F69
            MapFeaturetoMarketingGroup(accommodationlinked, "B79228E62B5A4D14B2BF35E7B79B8580");
            MapFeaturetoMarketingGroup(accommodationlinked, "B5757D0688674594955606382A5E126C");
            MapFeaturetoMarketingGroup(accommodationlinked, "31F741E8D6D8444A9BB571A2DF193F69");

            UpdateSpecialFeatures(accommodationlinked);
            //tracesource.TraceEvent(TraceEventType.Information, 0, A0RID + " Ausstattunginformation created");
            UpdateThemes(accommodationlinked, xmlfiles["Wine"], xmlfiles["City"], xmlfiles["NearSkiArea"], xmlfiles["Mediterranean"], xmlfiles["Dolomites"], xmlfiles["Alpine"]);
            //tracesource.TraceEvent(TraceEventType.Information, 0, A0RID + " Themeinformation created");
            UpdateBadges(accommodationlinked, xmlfiles["Vinum"]);
            //tracesource.TraceEvent(TraceEventType.Information, 0, A0RID + " Badgeinformation created");
            UpdateAusstattungToSmgTags(accommodationlinked);
            //tracesource.TraceEvent(TraceEventType.Information, 0, A0RID + " Ausstattunginformation created");

            //IF Badge Behindertengerecht is present add it as Tag NEW: Additional Check is done 
            UpdateBadgesToSmgTags(accommodationlinked, "Behindertengerecht", "barrier-free");

            //TODO Create AccoRoomInfo
            //TODO OwnerRID

            return accommodationlinked;
        }

        public static IEnumerable<AccommodationRoomV2> ParseLTSAccommodationRoom(
            LTSAccoData accommodation,
            bool reduced,
            IDictionary<string, XDocument> xmlfiles,
            IDictionary<string, JArray> jsonfiles)
        {
            List<AccommodationRoomV2> roomlist = new List<AccommodationRoomV2>();

            foreach (var accoroom in accommodation.roomGroups)
            {
                AccommodationRoomV2 room = new AccommodationRoomV2();

                room.A0RID = accoroom.rid;
                //room.Id = accoroom.; TO CHECK
                room.Roomtype = GetRoomTypeFromType(accoroom.type);  //GetRoomType() not needed, type is room/apartment

                if (accoroom.isActive)
                {
                    room.Active = true;
                    //adding room.Active
                    room.PublishedOn.TryAddOrUpdateOnList("idm-marketplace");
                }
                else
                {
                    room.Active = false;
                    room.PublishedOn.TryRemoveOnList("idm-marketplace");
                }

                room.RoomtypeInt = GetRoomType(room.Roomtype);
                room.RoomClassificationCodes = AlpineBitsHelper.GetRoomClassificationCode(room.Roomtype);

                //NEU
                room.Source = "lts";
                room.LTSId = accoroom.rid;
                room.HGVId = "";

                room.RoomCode = accoroom.code;
                room.PriceFrom = accoroom.minAmountPerPersonPerDay;  //TO CHECK IF THIS IS THE RIGHT field

                ////Room Numbers
                room.RoomNumbers = accoroom.rooms.Select(x => x.code).ToList();
                room.Roommax = accoroom.occupancy.max;
                room.Roommin = accoroom.occupancy.min;
                room.Roomstd = accoroom.occupancy.standard;
                //room.Roomminadults = accoroom.occupancy.minAdults; new field

                room.RoomQuantity = accoroom.roomQuantity;

                room.HasLanguage = accoroom.name.Keys.ToList();

                room.Shortname = accoroom.name["de"];

                //TODO

                //baths
                //diningRooms
                //livingRooms
                //sleepingRooms
                //toilets


                //classification (room, apartment?)
                
                //lastUpdate

                //minAmountPerPersonPerDay
                //minAmountPerUnitPerDay
                //rooms.availability
                //squareMeters
                //type

                //Amenities parsing                
                List<AccoFeatureLinked> featurelist = new List<AccoFeatureLinked>();

                //Features
                foreach (var amenity in accoroom.amenities)
                {
                    var myfeature = xmlfiles["Features"].Root.Elements("Data").Where(x => x.Attribute("T0RID").Value == amenity.rid).FirstOrDefault().Elements("DataLng").Where(x => x.Attribute("LngID").Value.ToUpper() == "EN").FirstOrDefault().Attribute("T1Des").Value;

                    //HGV ID Feature + OTA Code
                    string hgvamenityid = "";
                    string otacodes = "";

                    var myamenity = xmlfiles["RoomAmenities"].Root.Elements("amenity").Where(x => x.Element("ltsrid").Value == amenity.rid).FirstOrDefault();

                    if (myamenity != null)
                    {
                        hgvamenityid = myamenity.Element("hgvid").Value;
                        otacodes = myamenity.Element("ota_codes") != null ? myamenity.Element("ota_codes").Value : "";
                    }

                    List<int> amenitycodes = null;

                    if (!String.IsNullOrEmpty(otacodes))
                    {
                        var otacodessplittet = otacodes.Split(',').ToList();
                        amenitycodes = new List<int>();

                        foreach (var otacodesplit in otacodessplittet)
                        {
                            amenitycodes.Add(Convert.ToInt32(otacodesplit));
                        }
                    }

                    if (myfeature != null)
                        featurelist.Add(new AccoFeatureLinked() { Id = amenity.rid, Name = myfeature, HgvId = hgvamenityid, OtaCodes = otacodes, RoomAmenityCodes = amenitycodes });
                }
                room.Features = featurelist.ToList();

                List<AccoDetail> myroomdetailslist = new List<AccoDetail>();

                //Room Details Parsing            
                foreach (string lang in room.HasLanguage)
                {
                    AccoRoomDetail mydetail = new AccoRoomDetail();

                    mydetail.Language = lang;
                    mydetail.Name = accoroom.name[lang];

                    mydetail.Longdesc = accoroom.descriptions.Where(x => x.type == "longDescription").FirstOrDefault()?.description[lang];
                    mydetail.Shortdesc = accoroom.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description[lang];

                    room.AccoRoomDetail.TryAddOrUpdate(lang, mydetail);
                }

                //Image Parsing
                List<ImageGallery> imagegallerylist = new List<ImageGallery>();
                foreach (var image in accoroom.images)
                {
                    ImageGallery mainimage = new ImageGallery();

                    mainimage.ImageUrl = image.url;
                    mainimage.Height = image.heightPixel;
                    mainimage.Width = image.widthPixel;
                    mainimage.ImageSource = "lts";
                    mainimage.ListPosition = image.order;

                    mainimage.CopyRight = image.copyright;
                    mainimage.License = image.license;

                    mainimage.ImageTitle = image.name;

                    imagegallerylist.Add(mainimage);
                }

                room.ImageGallery = imagegallerylist.ToList();
                room.LastChange = DateTime.Now;

                roomlist.Add(room);
            }

            return roomlist;
        }

        

        //Special Mapping etc...
        private static void MapFeaturetoMarketingGroup(AccommodationV2 myacco, string featureid)
        {
            if (myacco.Features != null && myacco.Features.Count > 0 && myacco.Features.Select(x => x.Id).ToList().Contains(featureid))
            {
                if (myacco.MarketingGroupIds == null)
                    myacco.MarketingGroupIds = new List<string>();

                myacco.MarketingGroupIds.Add(featureid);
            }
        }

        //Update Badge Information
        private static void UpdateBadges(AccommodationV2 myacco, XDocument myvinumlist)
        {
            myacco.BadgeIds = new List<string>();

            if (myacco.MarketingGroupIds != null)
            {

                //badge hinzufügen            
                var badge1 = myacco.MarketingGroupIds.Where(x => x == "8C40CB6844F14E4A821F4EBF8231A4E8").Count();
                if (badge1 > 0)
                    myacco.BadgeIds.Add("Wellnesshotel");

                var badge2 = myacco.MarketingGroupIds.Where(x => x == "A0BF7E47F4AC224EB089327BE4725C8B").Count();
                if (badge2 > 0)
                    myacco.BadgeIds.Add("Familienhotel");

                var badge3 = myacco.MarketingGroupIds.Where(x => x == "F2CAAF48AC1C4EE88342FB4E59610A68").Count();
                if (badge3 > 0)
                    myacco.BadgeIds.Add("Bikehotel");

                var badge4 = myacco.MarketingGroupIds.Where(x => x == "3CC3D40C8CAC46B7928CE76C9D7A6FF6").Count();
                if (badge4 > 0)
                    myacco.BadgeIds.Add("Bauernhof");

                var badge5 = myacco.MarketingGroupIds.Where(x => x == "476007E6DF974CFC98BBBEDD8787EC81").Count();
                if (badge5 > 0)
                    myacco.BadgeIds.Add("Behindertengerecht");

                var badge6 = myacco.MarketingGroupIds.Where(x => x == "3EA6116A6103498799B642C9C56D8301").Count();
                if (badge6 > 0)
                    myacco.BadgeIds.Add("Wanderhotel");

                var badge7 = myacco.MarketingGroupIds.Where(x => x == "4796D94E3AD54135973DF8574E52679E").Count();
                if (badge7 > 0)
                {
                    myacco.BadgeIds.Add("Südtirol Privat");
                }


                //Badge 8 Vinum Hotels

                //XDocument myvinumlist = XDocument.Load(xmldir + "Vinum.xml");
                //In Weinliste schauen
                var isinwinelist = myvinumlist.Root.Elements("Hotel").Where(x => x.Value.ToUpper() == myacco.Id.ToUpper()).FirstOrDefault();
                if (isinwinelist != null)
                {
                    myacco.BadgeIds.Add("Vinumhotel");
                }

                //Badge 9,10,11 Nachhaltigkeitslabel Südtirol Level 1 (B79228E62B5A4D14B2BF35E7B79B8580 ) + 2 (B5757D0688674594955606382A5E126C)  + 3 (31F741E8D6D8444A9BB571A2DF193F69 )
                var badge9 = myacco.MarketingGroupIds.Where(x => x == "B79228E62B5A4D14B2BF35E7B79B8580").Count();
                if (badge9 > 0)
                {
                    myacco.BadgeIds.Add("SustainabilityLevel1");
                }
                var badge10 = myacco.MarketingGroupIds.Where(x => x == "B5757D0688674594955606382A5E126C").Count();
                if (badge10 > 0)
                {
                    myacco.BadgeIds.Add("SustainabilityLevel2");
                }
                var badge11 = myacco.MarketingGroupIds.Where(x => x == "31F741E8D6D8444A9BB571A2DF193F69").Count();
                if (badge11 > 0)
                {
                    myacco.BadgeIds.Add("SustainabilityLevel3");
                }

                Console.WriteLine("Badge Table info added!");
            }
        }

        //Update Theme Information
        private static void UpdateThemes(AccommodationV2 myacco, XDocument mywinelist, XDocument mycitylist, XDocument myskiarealist, XDocument mymediterranenlist, XDocument dolomiteslist, XDocument alpinelist)
        {
            myacco.ThemeIds = new List<string>();

            //Theme 1
            //Gourmet 
            //Feature = 46AD7938616B4D4882A006BEF3B199A4 
            //Feature = F0A385D0E8E44944AFCA3893712A1420
            //Feature = 2FA54F6F350748AE9CD1A389A5C9EDD9
            //Feature = C0E761D71CC44F4C80D75FF68ED72C55 
            //Feature = 6797D594C7BF4C7AA6D384B234EC7C44
            //Feature = E5775068F5644E92B7CF94BDFCDA5175
            //Feature = 1FFD5352501542BF8BCB24B7BF75CF4F
            //Feature = 5060F78090604B2E97A96D86B97D2E0B
            if (myacco.Features != null)
            {
                var gourmet = myacco.Features.Where(x => x.Id == "46AD7938616B4D4882A006BEF3B199A4" ||
                    x.Id == "F0A385D0E8E44944AFCA3893712A1420" ||
                    x.Id == "2FA54F6F350748AE9CD1A389A5C9EDD9" ||
                    x.Id == "C0E761D71CC44F4C80D75FF68ED72C55" ||
                    x.Id == "6797D594C7BF4C7AA6D384B234EC7C44" ||
                    x.Id == "E5775068F5644E92B7CF94BDFCDA5175" ||
                    x.Id == "1FFD5352501542BF8BCB24B7BF75CF4F" ||
                    x.Id == "5060F78090604B2E97A96D86B97D2E0B");

                if (gourmet.Count() > 0)
                {
                    myacco.ThemeIds.Add("Gourmet");
                }
            }

            //Theme 2
            //In der Höhe 
            //Altitude > 1000            
            var altitude = myacco.Altitude;

            if (altitude != null)
            {
                int altitudeint = Convert.ToInt32(altitude);

                if (altitudeint > 1000)
                {
                    myacco.ThemeIds.Add("In der Höhe");
                }
            }


            //Theme 3
            //Regionale Wellness- und Heilanwendungen
            //Feature = B5CFA063BEEB4631B7A0DE836030E2ED UND 
            //Feature = E72CE3544DA2475E97B9C034DA6F1595 UND
            //( Feature = D417529377CB430389E07787D8A3A483 ODER
            //  Feature = 5E57209D17244BA09A0400A498E549AE ) UND
            //( Feature = 7BCAF604E17B46F2A2C6CAE70C5B621F ODER
            //  Feature = B2103635BD224E64A812FD2BF53C8DCA ) UND
            if (myacco.Features != null)
            {
                var regionalewellness = myacco.Features.Where(x =>
                    x.Id == "B5CFA063BEEB4631B7A0DE836030E2ED" ||
                    x.Id == "E72CE3544DA2475E97B9C034DA6F1595" ||
                    x.Id == "D417529377CB430389E07787D8A3A483" ||
                    x.Id == "5E57209D17244BA09A0400A498E549AE" ||
                    x.Id == "7BCAF604E17B46F2A2C6CAE70C5B621F" ||
                    x.Id == "B2103635BD224E64A812FD2BF53C8DCA");

                bool wellness1 = false;
                bool wellness2 = false;
                bool wellness3 = false;
                bool wellness4 = false;
                bool wellness5 = false;
                bool wellness6 = false;

                if (regionalewellness.Count() > 0)
                {
                    foreach (var mywellness in regionalewellness)
                    {
                        if (mywellness.Id == "B5CFA063BEEB4631B7A0DE836030E2ED")
                            wellness1 = true;
                        if (mywellness.Id == "E72CE3544DA2475E97B9C034DA6F1595")
                            wellness2 = true;
                        if (mywellness.Id == "D417529377CB430389E07787D8A3A483")
                            wellness3 = true;
                        if (mywellness.Id == "5E57209D17244BA09A0400A498E549AE")
                            wellness4 = true;
                        if (mywellness.Id == "7BCAF604E17B46F2A2C6CAE70C5B621F")
                            wellness5 = true;
                        if (mywellness.Id == "B2103635BD224E64A812FD2BF53C8DCA")
                            wellness6 = true;
                    }

                    if (wellness1 && wellness2 && (wellness3 || wellness4) && (wellness5 || wellness6))
                    {
                        myacco.ThemeIds.Add("Regionale Wellness");
                    }
                }
            }

            //Theme 4
            //Biken
            //(Feature = 8068941DF6F34B9D955965062614A3C2 AND 
            //Feature = 349A4D98B26B448A908679142C3394D6 AND
            //Feature = BF108AD2B62042DF9FEAD4E865E11E75) OR
            //Feature = 05988DB63E5146E481C95279FB285C6A (Bett & Bike) OR 5F22AD3E93D54E99B7E6F97719A47153 (Bett & Bike Sport) OR
            //Badge Biken
            if (myacco.Features != null)
            {
                var biken = myacco.Features.Where(x =>
                    x.Id == "8068941DF6F34B9D955965062614A3C2" ||
                    x.Id == "349A4D98B26B448A908679142C3394D6" ||
                    x.Id == "BF108AD2B62042DF9FEAD4E865E11E75" ||
                    x.Id == "05988DB63E5146E481C95279FB285C6A" ||
                    x.Id == "5F22AD3E93D54E99B7E6F97719A47153");

                bool biken1 = false;
                bool biken2 = false;
                bool biken3 = false;
                bool biken4 = false;
                bool biken5 = false;
                bool bikenbadge = false;

                if (myacco.MarketingGroupIds.Where(x => x == "F2CAAF48AC1C4EE88342FB4E59610A68").Count() > 0)
                    bikenbadge = true;


                if (biken.Count() > 0)
                {
                    foreach (var mybiken in biken)
                    {
                        if (mybiken.Id == "8068941DF6F34B9D955965062614A3C2")
                            biken1 = true;
                        if (mybiken.Id == "349A4D98B26B448A908679142C3394D6")
                            biken2 = true;
                        if (mybiken.Id == "BF108AD2B62042DF9FEAD4E865E11E75")
                            biken3 = true;
                        if (mybiken.Id == "05988DB63E5146E481C95279FB285C6A")
                            biken4 = true;
                        if (mybiken.Id == "5F22AD3E93D54E99B7E6F97719A47153")
                            biken5 = true;
                    }
                }

                if ((biken1 && biken2 && biken3) || biken4 || biken5 || bikenbadge)
                {
                    myacco.ThemeIds.Add("Biken");
                }
            }
            //Theme 5
            //Familie
            //Feature = 8B808C230FE34263BE3787680DA253C7 UND 
            //Feature = 36C354DC30F14DD7B1CCFEE78E82132C UND
            //Feature = 188A9BADC0324C10B0013F108CE5EA5C UND
            if (myacco.Features != null)
            {
                var familie = myacco.Features.Where(x =>
                    x.Id == "8B808C230FE34263BE3787680DA253C7" ||
                    x.Id == "36C354DC30F14DD7B1CCFEE78E82132C" ||
                    x.Id == "188A9BADC0324C10B0013F108CE5EA5C");

                bool familie1 = false;
                bool familie2 = false;
                bool familie3 = false;

                if (familie.Count() > 0)
                {
                    foreach (var myfamilie in familie)
                    {
                        if (myfamilie.Id == "8B808C230FE34263BE3787680DA253C7")
                            familie1 = true;
                        if (myfamilie.Id == "36C354DC30F14DD7B1CCFEE78E82132C")
                            familie2 = true;
                        if (myfamilie.Id == "188A9BADC0324C10B0013F108CE5EA5C")
                            familie3 = true;
                    }

                    if (familie1 && familie2 && familie3)
                    {
                        myacco.ThemeIds.Add("Familie");
                    }
                }
            }

            //Theme 6
            //Wandern
            //Feature = 0A6193AD6EBC4BC18E83D7CEEEF53E45 UND                         
            //Feature = 42E4EFB64AD14393BC28DBC20F273B9D UND
            if (myacco.Features != null)
            {
                var wandern = myacco.Features.Where(x =>
                    x.Id == "0A6193AD6EBC4BC18E83D7CEEEF53E45" ||
                    x.Id == "42E4EFB64AD14393BC28DBC20F273B9D");

                bool wandern1 = false;
                bool wandern2 = false;

                if (wandern.Count() > 0)
                {
                    foreach (var mywandern in wandern)
                    {
                        if (mywandern.Id == "0A6193AD6EBC4BC18E83D7CEEEF53E45")
                            wandern1 = true;
                        if (mywandern.Id == "42E4EFB64AD14393BC28DBC20F273B9D")
                            wandern2 = true;
                    }

                    if (wandern1 && wandern2)
                    {
                        myacco.ThemeIds.Add("Wandern");
                    }
                }
            }
            //Theme 7
            //Wein
            //Feature = 0A6193AD6EBC4BC18E83D7CEEEF53E45 UND                         
            //Feature = 42E4EFB64AD14393BC28DBC20F273B9D UND

            var weinaltitude = myacco.Altitude;

            if (weinaltitude != null)
            {
                int weinaltitudeint = Convert.ToInt32(weinaltitude);

                if (weinaltitudeint <= 900)
                {
                    //XDocument mywinelist = XDocument.Load(xmldir + "Wine.xml");

                    //In Weinliste schauen
                    var isinwinelist = mywinelist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).FirstOrDefault();

                    if (isinwinelist != null)
                    {
                        myacco.ThemeIds.Add("Wein");
                    }
                }
            }


            //Theme 8
            //Städtisches Flair
            //in Liste

            var cityaltitude = myacco.Altitude;

            if (cityaltitude != null)
            {
                int cityaltitudeint = Convert.ToInt32(cityaltitude);

                if (cityaltitudeint <= 1100)
                {
                    //XDocument mycitylist = XDocument.Load(xmldir + "City.xml");
                    //In Liste schauen
                    var isincitieslist = mycitylist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).FirstOrDefault();

                    if (isincitieslist != null)
                    {
                        myacco.ThemeIds.Add("Städtisches Flair");
                    }
                }
            }


            //Theme 9
            //Skigebiete
            //in Liste
            //XDocument myskiarealist = XDocument.Load(xmldir + "NearSkiArea.xml");
            //In Liste schauen
            var isinskiarealist = myskiarealist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isinskiarealist > 0)
            {
                myacco.ThemeIds.Add("Am Skigebiet");
            }


            //Theme 10
            //Mediterranes Südtirol
            //in Liste
            //XDocument mymediterranenlist = XDocument.Load(xmldir + "Mediterranean.xml");
            //In Liste schauen
            var isinmediterranlist = mymediterranenlist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isinmediterranlist > 0)
            {
                myacco.ThemeIds.Add("Mediterran");
            }


            //Theme 11
            //In den Dolomiten
            //in Liste
            //XDocument dolomiteslist = XDocument.Load(xmldir + "Dolomites.xml");
            //In Liste schauen
            var isindolomitenlist = dolomiteslist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isindolomitenlist > 0)
            {
                myacco.ThemeIds.Add("Dolomiten");
            }

            //Theme 12
            //Alpines Südtirol
            //in Liste

            //XDocument alpinelist = XDocument.Load(xmldir + "Alpine.xml");
            //In Liste schauen
            var isinalpinelist = alpinelist.Root.Elements("Fraction").Where(x => x.Value == myacco.DistrictId).Count();

            if (isinalpinelist > 0)
            {
                myacco.ThemeIds.Add("Alpin");
            }


            //Thema 13
            //Kleine Betriebe
            //Units > 0 und klianer < 20

            //NOT MORE USED
            //if (myacco.Units > 0)
            //{
            //    if (myacco.Units < 20)
            //    {
            //        myacco.ThemeIds.Add("Kleine Betriebe");
            //    }
            //}            


            //Theme 14
            //Hütten & Berggasthöfe            
            if (myacco.AccoTypeId == "Mountain")
            {
                myacco.ThemeIds.Add("Hütten und Berggasthöfe");
            }


            //Theme 15
            //Bäuerliche Welten            
            if (myacco.AccoTypeId == "Farm")
            {
                myacco.ThemeIds.Add("Bäuerliche Welten");
            }


            //Theme 16
            //Bonus Vacanze
            if (myacco.Features != null)
            {
                var balance = myacco.Features.Where(x => x.Id == "D448B037F37843B3B49C15CAFBBC5669").Count();

                if (balance > 0)
                {
                    myacco.ThemeIds.Add("Bonus Vacanze");
                }
            }


            //Thema 17 Christkindlmarkt

            List<string> tvtoassign = new List<string>();
            tvtoassign.Add("5228229451CA11D18F1400A02427D15E"); //Bozen
            tvtoassign.Add("5228229751CA11D18F1400A02427D15E"); //Brixen
            tvtoassign.Add("5228229851CA11D18F1400A02427D15E"); //Bruneck            
            tvtoassign.Add("522822FF51CA11D18F1400A02427D15E"); //Sterzing
            tvtoassign.Add("522822BE51CA11D18F1400A02427D15E"); //Meran

            if (tvtoassign.Contains(myacco.TourismVereinId.ToUpper()))
                myacco.ThemeIds.Add("Christkindlmarkt");


            //Thema 18 Nachhaltigkeit NEU

            List<string> sustainabilityodhtagtocheck = new List<string>();
            sustainabilityodhtagtocheck.Add("bio hotels südtirol"); //Bozen
            sustainabilityodhtagtocheck.Add("ecolabel hotels"); //Brixen
            sustainabilityodhtagtocheck.Add("gstc hotels"); //Bruneck            
            sustainabilityodhtagtocheck.Add("klimahotel"); //Sterzing

            //check if one of this odhtags is assigned

            var sustainabilityfeaturecheck = myacco.MarketingGroupIds != null ? myacco.MarketingGroupIds.Where(x => x == "3EA6116A6103498799B642C9C56D8301").Count() : 0;
            var sustainabilitytagintersection = myacco.SmgTags != null && myacco.SmgTags.Count > 0 ? sustainabilityodhtagtocheck.Intersect(myacco.SmgTags).Count() : 0;

            if (sustainabilityfeaturecheck > 0 || sustainabilitytagintersection > 0)
                myacco.ThemeIds.Add("Sustainability");


            Console.WriteLine("Thema hinzugefügt");
            Console.WriteLine("weiter....");
        }

        //Update SpecialFeatures Information
        private static void UpdateSpecialFeatures(AccommodationV2 myacco)
        {
            myacco.SpecialFeaturesIds = new List<string>();

            if (myacco.Features != null)
            {
                //SpecialFeature 1
                //Ruhig gelegen SpecialFeatures = B6BD3F6011E5488DBF802B0C58F87AA1 

                var ruhiggelegen = myacco.Features.Where(x => x.Id == "B6BD3F6011E5488DBF802B0C58F87AA1").Count();
                if (ruhiggelegen > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Ruhig gelegen");
                }

                //SpecialFeature 2
                //Tagung möglich SpecialFeatures = FF81E4F50465484883DBF40CFB82BB0C UND 3101F60F0A594C0B9BBC8F4E2D7A2919             
                bool tagung1condition = false;
                bool tagung2condition = false;

                var tagung = myacco.Features.Where(x => x.Id == "FF81E4F50465484883DBF40CFB82BB0C" || x.Id == "3101F60F0A594C0B9BBC8F4E2D7A2919");
                if (tagung != null)
                {
                    foreach (var mytagung in tagung)
                    {
                        if (mytagung.Id == "FF81E4F50465484883DBF40CFB82BB0C")
                            tagung1condition = true;
                        if (mytagung.Id == "3101F60F0A594C0B9BBC8F4E2D7A2919")
                            tagung2condition = true;
                        if (tagung1condition && tagung2condition)
                        {
                            myacco.SpecialFeaturesIds.Add("Tagung");
                        }
                    }
                }

                //SpecialFeature 3
                //Schwimmbad SpecialFeatures = 7BCAF604E17B46F2A2C6CAE70C5B621F ODER B2103635BD224E64A812FD2BF53C8DCA
                var schwimmbad = myacco.Features.Where(x => x.Id == "7BCAF604E17B46F2A2C6CAE70C5B621F" || x.Id == "B2103635BD224E64A812FD2BF53C8DCA").Count();
                if (schwimmbad > 0)
                {                    
                    myacco.SpecialFeaturesIds.Add("Schwimmbad");                 
                }

                //SpecialFeature 4
                //Sauna SpecialFeatures = D417529377CB430389E07787D8A3A483 ODER 5E57209D17244BA09A0400A498E549AE
                var sauna = myacco.Features.Where(x => x.Id == "D417529377CB430389E07787D8A3A483" || x.Id == "5E57209D17244BA09A0400A498E549AE").Count();
                if (sauna > 0)
                {
                        myacco.SpecialFeaturesIds.Add("Sauna");                    
                }

                //SpecialFeature 5
                //Garage SpecialFeatures = D579D1C8EA8445018CA5BB6DABEA0C26
                var garage = myacco.Features.Where(x => x.Id == "D579D1C8EA8445018CA5BB6DABEA0C26").Count();
                if (garage > 0)
                {
                    //foreach (var mygarage in garage)
                    //{
                    myacco.SpecialFeaturesIds.Add("Garage");
                    //}
                }

                //SpecialFeature 6
                //Abholservice SpecialFeatures = 60F2408993E249F9A847F1B28C5B11E8
                var abholservice = myacco.Features.Where(x => x.Id == "60F2408993E249F9A847F1B28C5B11E8").Count();
                if (abholservice > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Abholservice");
                }

                //SpecialFeature 7
                //Wlan SpecialFeatures = 700A920BE6D6426CBF3EC623C2E922C2 OR 098EB30324EA492DBD99F323AE20A621
                var wlan = myacco.Features.Where(x => x.Id == "700A920BE6D6426CBF3EC623C2E922C2" || x.Id == "098EB30324EA492DBD99F323AE20A621").Count();
                if (wlan > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Wlan");
                }


                //SpecialFeature 8
                //Barrierefrei SpecialFeatures = B7E9EE4A91544849B69D5A5564DDCDFB            
                var barriere = myacco.Features.Where(x => x.Id == "B7E9EE4A91544849B69D5A5564DDCDFB").Count();
                if (barriere > 0)
                {
                    myacco.SpecialFeaturesIds.Add("Barrierefrei");
                }

                //SpecialFeature 9
                //Allergikerküche SpecialFeatures = 71A7D4A821F7437EA1DC05CEE9655A5A OR 11A6BEA7EEFC4716BDF8FBD5E15C0CFB
                var allergiker = myacco.Features.Where(x => x.Id == "71A7D4A821F7437EA1DC05CEE9655A5A" || x.Id == "11A6BEA7EEFC4716BDF8FBD5E15C0CFB").Count();
                if (allergiker > 0)
                {                    
                    myacco.SpecialFeaturesIds.Add("Allergikerküche");                 
                }

                //SpecialFeature 10
                //Kleine Haustiere SpecialFeatures = D9DCDD52FE444818AAFAB0E02FD92D91 OR FC80F2ECCE5A40AA8EDE458CBECC3D45         
                var kleinehaustiere = myacco.Features.Where(x => x.Id == "D9DCDD52FE444818AAFAB0E02FD92D91" || x.Id == "FC80F2ECCE5A40AA8EDE458CBECC3D45").Count();
                if (kleinehaustiere > 0)
                {                    
                    myacco.SpecialFeaturesIds.Add("Kleine Haustiere");                 
                }

                //SpecialFeature 11
                //Gruppenfreundlich SpecialFeatures = 828CA68E3ABC4BA69587ACCB728E8858 OR BBBE370E1A9547B09D27AE0D94C066A3 OR B7C49ECF3CE1470EBA17F34D10D163A1       
                var gruppenfreundlich = myacco.Features.Where(x => x.Id == "828CA68E3ABC4BA69587ACCB728E8858" || x.Id == "BBBE370E1A9547B09D27AE0D94C066A3" || x.Id == "B7C49ECF3CE1470EBA17F34D10D163A1").Count();
                if (gruppenfreundlich > 0)
                {                    
                    myacco.SpecialFeaturesIds.Add("Gruppenfreundlich");                 
                }

                ////Special Case Covid 19 --> obsolete?
                //var bonusvacanze = myacco.Features.Where(x => x.Id == "D448B037F37843B3B49C15CAFBBC5669").Count();

                //if (bonusvacanze > 0)
                //{
                //    //Füge SMGTag Balance hinzu
                //    if (myacco.SmgTags == null)
                //        myacco.SmgTags = new List<string>();

                //    if (!myacco.SmgTags.Contains("bonus vacanze"))
                //        myacco.SmgTags.Add("bonus vacanze");
                //}
                //if (bonusvacanze == 0)
                //{
                //    if (myacco.SmgTags != null)
                //    {
                //        if (myacco.SmgTags.Contains("bonus vacanze"))
                //            myacco.SmgTags.Remove("bonus vacanze");
                //    }
                //}

                //Specialcase guestcard if one of this guestcards is set
                //"035577098B254201A865684EF050C851",bozencardplus
                //"CEE3703E4E3B44E3BD1BEE3F559DD31C",rittencard
                //"C7758584EFDE47B398FADB6BDBD0F198",klausencard
                //"C3C7ABEB0F374A0F811788B775D96AC0",brixencard
                //"3D703D2EA16645BD9EA3273069A0B918",almencardplus
                //"D02AE2F641A4496AB1D2C4871475293D",activecard
                //"DA4CAD333B8D45448AAEA9E966C68380",winepass
                //"500AEFA8868748899BEC826B5E81951C",ultentalcard
                //"DE13880FA929461797146596FA3FFC07",merancard   --> REMOVED
                //"49E9FF69F86846BD9915A115988C5484",vinschgaucard
                //"FAEB6769EC564CBF982D454DCEEBCB27",algundcard
                //"3FD7253E3F6340E1AF642EA3DE005128",holidaypass
                //"24E475F20FF64D748EBE7033C2DBC3A8",valgardenamobilcard
                //"056486AFBEC4471EA32B3DB658A96D48",dolomitimobilcard
                //"8192350ABF6B41DA89B255B340003991",suedtirolguestpass
                //"3CB7D42AD51C4E2BA061CF9838A3735D",holidaypass3zinnen
                //"9C8140EB332F46E794DFDDB240F9A9E4",mobilactivcard
                //"6ACF61213EA347C6B1EB409D4A473B6D",dolomiti_museumobilcard
                //new C414648944CE49D38506D176C5B58486 merancard_allyear
                //New 99803FF36D51415CAFF64183CC26F736 sarntalcard

                //B69F991C1E45422B9D457F716DEAA82B,suedtirolguestpass_passeiertalpremium
                //F4D3B02B107843C894ED517FC7DC8A39,suedtirolguestpass_mobilcard
                //895C9B57E0D54B449C82F035538D4A79,suedtirolguestpass_museumobilcard

                var guestcard = myacco.Features.Where(x => x.Id == "035577098B254201A865684EF050C851" || x.Id == "CEE3703E4E3B44E3BD1BEE3F559DD31C" || x.Id == "C7758584EFDE47B398FADB6BDBD0F198" ||
                                                           x.Id == "C3C7ABEB0F374A0F811788B775D96AC0" || x.Id == "3D703D2EA16645BD9EA3273069A0B918" || x.Id == "D02AE2F641A4496AB1D2C4871475293D" ||
                                                           x.Id == "DA4CAD333B8D45448AAEA9E966C68380" || x.Id == "500AEFA8868748899BEC826B5E81951C" ||
                                                           x.Id == "49E9FF69F86846BD9915A115988C5484" || x.Id == "FAEB6769EC564CBF982D454DCEEBCB27" || x.Id == "3FD7253E3F6340E1AF642EA3DE005128" ||
                                                           x.Id == "24E475F20FF64D748EBE7033C2DBC3A8" || x.Id == "056486AFBEC4471EA32B3DB658A96D48" || x.Id == "8192350ABF6B41DA89B255B340003991" ||
                                                           x.Id == "3CB7D42AD51C4E2BA061CF9838A3735D" || x.Id == "9C8140EB332F46E794DFDDB240F9A9E4" || x.Id == "C414648944CE49D38506D176C5B58486" ||
                                                           x.Id == "6ACF61213EA347C6B1EB409D4A473B6D" || x.Id == "99803FF36D51415CAFF64183CC26F736" ||
                                                           x.Id == "B69F991C1E45422B9D457F716DEAA82B" || x.Id == "F4D3B02B107843C894ED517FC7DC8A39" || x.Id == "895C9B57E0D54B449C82F035538D4A79").Count();

                if (guestcard > 0)
                {
                    myacco.SpecialFeaturesIds.Add("guestcard");
                }
            }
        }

        private static void UpdateAusstattungToSmgTags(AccommodationV2 myacco)
        {
            RemoveTagIf("035577098B254201A865684EF050C851", "bozencardplus", myacco);
            RemoveTagIf("CEE3703E4E3B44E3BD1BEE3F559DD31C", "rittencard", myacco);
            RemoveTagIf("C7758584EFDE47B398FADB6BDBD0F198", "klausencard", myacco);
            RemoveTagIf("C3C7ABEB0F374A0F811788B775D96AC0", "brixencard", myacco);
            //RemoveTagIf("455984E79EE6437B8D01793895AFDBE6", "almencardplus", myacco);
            RemoveTagIf("3D703D2EA16645BD9EA3273069A0B918", "almencardplus", myacco);
            RemoveTagIf("D02AE2F641A4496AB1D2C4871475293D", "activecard", myacco);
            RemoveTagIf("DA4CAD333B8D45448AAEA9E966C68380", "winepass", myacco);
            RemoveTagIf("500AEFA8868748899BEC826B5E81951C", "ultentalcard", myacco);
            RemoveTagIf("DE13880FA929461797146596FA3FFC07", "merancard", myacco);
            RemoveTagIf("49E9FF69F86846BD9915A115988C5484", "vinschgaucard", myacco);
            RemoveTagIf("FAEB6769EC564CBF982D454DCEEBCB27", "algundcard", myacco);
            RemoveTagIf("3FD7253E3F6340E1AF642EA3DE005128", "holidaypass", myacco);
            RemoveTagIf("24E475F20FF64D748EBE7033C2DBC3A8", "valgardenamobilcard", myacco);

            //Renamed
            //RemoveTagIf("056486AFBEC4471EA32B3DB658A96D48", "vilnoessdolomitimobilcard", myacco);
            RemoveTagIf("056486AFBEC4471EA32B3DB658A96D48", "dolomitimobilcard", myacco);

            RemoveTagIf("9C8140EB332F46E794DFDDB240F9A9E4", "mobilactivcard", myacco);
            //NEU
            RemoveTagIf("8192350ABF6B41DA89B255B340003991", "suedtirolguestpass", myacco);
            RemoveTagIf("3CB7D42AD51C4E2BA061CF9838A3735D", "holidaypass3zinnen", myacco);
            RemoveTagIf("19ABB47430F64287BEA96237A2E99899", "seiseralm_balance", myacco);
            RemoveTagIf("D1C1C206AA0B4025A98EE83C2DBC2DFA", "workation", myacco);
            //new
            RemoveTagIf("C414648944CE49D38506D176C5B58486", "merancard_allyear", myacco);
            RemoveTagIf("6ACF61213EA347C6B1EB409D4A473B6D", "dolomiti_museumobilcard", myacco);
            //new 15.01
            RemoveTagIf("99803FF36D51415CAFF64183CC26F736", "sarntalcard", myacco);

            //new 09.04.24
            RemoveTagIf("B69F991C1E45422B9D457F716DEAA82B", "suedtirolguestpass_passeiertal_premium", myacco); //Südtirol Guest Pass Passeiertal Premium
            RemoveTagIf("F4D3B02B107843C894ED517FC7DC8A39", "suedtirolguestpass_mobilcard", myacco); //Südtirol Guest Pass Mobilcard
            RemoveTagIf("895C9B57E0D54B449C82F035538D4A79", "suedtirolguestpass_museumobilcard", myacco); //Südtirol Alto Adige Guest Pass+museumobil Card


            List<string> guestcardlist = new List<string>()
            {
                "035577098B254201A865684EF050C851",
                "CEE3703E4E3B44E3BD1BEE3F559DD31C",
                "C7758584EFDE47B398FADB6BDBD0F198",
                "C3C7ABEB0F374A0F811788B775D96AC0",
                "3D703D2EA16645BD9EA3273069A0B918",
                "D02AE2F641A4496AB1D2C4871475293D",
                "DA4CAD333B8D45448AAEA9E966C68380",
                "500AEFA8868748899BEC826B5E81951C",
                "49E9FF69F86846BD9915A115988C5484",
                "FAEB6769EC564CBF982D454DCEEBCB27",
                "3FD7253E3F6340E1AF642EA3DE005128",
                "24E475F20FF64D748EBE7033C2DBC3A8",
                "056486AFBEC4471EA32B3DB658A96D48",
                "8192350ABF6B41DA89B255B340003991",
                "3CB7D42AD51C4E2BA061CF9838A3735D",
                "9C8140EB332F46E794DFDDB240F9A9E4",
                "C414648944CE49D38506D176C5B58486",
                "6ACF61213EA347C6B1EB409D4A473B6D",
                "99803FF36D51415CAFF64183CC26F736",
                "B69F991C1E45422B9D457F716DEAA82B",
                "F4D3B02B107843C894ED517FC7DC8A39",
                "895C9B57E0D54B449C82F035538D4A79"
            };

            RemoveTagIf(guestcardlist, "guestcard", myacco);

            //NEW
            RemoveTagIf("05988DB63E5146E481C95279FB285C6A", "accomodation bed bike", myacco);
            RemoveTagIf("5F22AD3E93D54E99B7E6F97719A47153", "accomodation bett bike sport", myacco);
        }

        private static void UpdateBadgesToSmgTags(AccommodationV2 myacco, string badgename, string tagname)
        {
            if (myacco.BadgeIds != null && myacco.BadgeIds.Count() > 0)
            {
                if (myacco.BadgeIds.Select(x => x.ToLower()).Contains(badgename.ToLower()))
                {
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Add(tagname);
                }
                else
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains(tagname))
                            myacco.SmgTags.Remove(tagname);
                    }
                }
            }
            else
            {
                if (myacco.SmgTags != null)
                {
                    if (myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Remove(tagname);
                }
            }
        }

        private static void RemoveTagIf(string featureId, string tagname, AccommodationV2 myacco)
        {
            if (myacco.Features != null)
            {
                var property = myacco.Features.Where(x => x.Id == featureId).Count();

                if (property > 0)
                {
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Add(tagname);
                }
                if (property == 0)
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains(tagname))
                            myacco.SmgTags.Remove(tagname);
                    }
                }
            }
            if (myacco.SmgTags != null)
            {
                if (myacco.SmgTags.Contains(tagname))
                    myacco.SmgTags.Remove(tagname);
            }
        }

        private static void RemoveTagIf(List<string> featurelist, string tagname, AccommodationV2 myacco)
        {
            if (myacco.Features != null)
            {
                var property = myacco.Features.Where(x => featurelist.Contains(x.Id)).Count();

                if (property > 0)
                {
                    if (myacco.SmgTags == null)
                        myacco.SmgTags = new List<string>();

                    if (!myacco.SmgTags.Contains(tagname))
                        myacco.SmgTags.Add(tagname);
                }
                if (property == 0)
                {
                    if (myacco.SmgTags != null)
                    {
                        if (myacco.SmgTags.Contains(tagname))
                            myacco.SmgTags.Remove(tagname);
                    }
                }
            }
            if (myacco.SmgTags != null)
            {
                if (myacco.SmgTags.Contains(tagname))
                    myacco.SmgTags.Remove(tagname);
            }
        }

        //Refers to LTS SendData B0Typ Type of room: 0=undefined; 1=room; 2=apartment; 3=camp site (Stellplatz); 4=caravan for rent; 5=tent area(Zeltplatz); 6=Bungalow; 7=camp(Schlaflager)

        private static string GetRoomType(int roomtype)
        {
            string myroomtype = "";

            switch (roomtype)
            {
                case 0:
                    myroomtype = "undefined";
                    break;
                case 1:
                    myroomtype = "room";
                    break;
                case 2:
                    myroomtype = "apartment";
                    break;
                case 3:
                    myroomtype = "pitch";
                    break;
                case 4:
                    myroomtype = "dorm";
                    break;

                    //case "3":
                    //    myroomtype = "campsite";
                    //    break;
                    //case "4":
                    //    myroomtype = "caravan";
                    //    break;
                    //case "5":
                    //    myroomtype = "tentarea";
                    //    break;
                    //case "6":
                    //    myroomtype = "bungalow";
                    //    break;
                    //case "7":
                    //    myroomtype = "camp";
                    //    break;
            }

            return myroomtype;
        }

        //Converts Room Type of the new Interface to B0Typ
        private static int GetRoomType(string roomtype)
        {
            switch (roomtype)
            {
                case "undefined": return 0;                    
                case "room": return 1;                    
                case "apartment": return 2;
                case "pitch": return 3;
                case "dorm": return 4;
                default: return 0;
            }
        }

        private static string GetRoomTypeFromClassification(string classification)
        {            
            switch (classification)
            {
                case "room": return "room";
                case "apartment": return "apartment";
                case "holidayHome": return "apartment";
                case "tent": return "pitch";
                case "pitch": return "pitch";
                case "dorm": return "dorm";
                default: return "undefined";
            }
        }

        private static string GetRoomTypeFromType(string roomtype)
        {
            //roomtypes used before
            //room
            //apartment
            //pitch
            //dorm
            //caravan
            //campsite
            //undefined
           
            switch (roomtype)
            {
                case "room": return "room";
                case "apartment": return "apartment";
                case "pitches": return "pitch";
                case "restingPlaces": return "dorm";
                default: return "undefined";
            }
        }

        private static int GetRepresentationmode(string representationmode)
        {
            //According to old LTS Documentation Representation Mode (0 = don’t show; 1 = minimal displaying; 2 = complete displaying)
            switch (representationmode)
            {
                case "full": return 2;
                case "minimal": return 1;
                case "none": return 0;
                default: return 0;
            }
        }

        private static int GetTrustYouState(string trustyoustate)
        {
            //According to old LTS Documentation State (0=not rated, 1=do not display, 2=display)
            switch (trustyoustate)
            {
                case "rated": return 2;
                case "underValued": return 1;
                case "notRated": return 0;
                default: return 0;
            }            
        }

        private static IDictionary<string, string> GetGuestCardIdMapping()
        {
            return new Dictionary<string, string>()
            {
                {"035577098B254201A865684EF050C851","bozencardplus" },
                {"CEE3703E4E3B44E3BD1BEE3F559DD31C","rittencard" },
                {"C7758584EFDE47B398FADB6BDBD0F198","klausencard" },
                {"C3C7ABEB0F374A0F811788B775D96AC0","brixencard" },
                {"3D703D2EA16645BD9EA3273069A0B918","almencardplus" },
                {"D02AE2F641A4496AB1D2C4871475293D","activecard" },
                {"DA4CAD333B8D45448AAEA9E966C68380","winepass" },
                {"500AEFA8868748899BEC826B5E81951C","ultentalcard" },
                {"DE13880FA929461797146596FA3FFC07","merancard" },
                {"49E9FF69F86846BD9915A115988C5484","vinschgaucard" },
                {"FAEB6769EC564CBF982D454DCEEBCB27","algundcard" },
                {"3FD7253E3F6340E1AF642EA3DE005128","holidaypass" },
                {"24E475F20FF64D748EBE7033C2DBC3A8","valgardenamobilcard" },
                {"056486AFBEC4471EA32B3DB658A96D48","dolomitimobilcard" },
                {"9C8140EB332F46E794DFDDB240F9A9E4","mobilactivcard" },
                {"8192350ABF6B41DA89B255B340003991","suedtirolguestpass" },
                {"3CB7D42AD51C4E2BA061CF9838A3735D","holidaypass3zinnen" },
                {"19ABB47430F64287BEA96237A2E99899","seiseralm_balance" },
                {"D1C1C206AA0B4025A98EE83C2DBC2DFA","workation" },
                {"C414648944CE49D38506D176C5B58486","merancard_allyear" },
                {"6ACF61213EA347C6B1EB409D4A473B6D","dolomiti_museumobilcard" },
                {"99803FF36D51415CAFF64183CC26F736","sarntalcard" },
                {"B69F991C1E45422B9D457F716DEAA82B","suedtirolguestpass_passeiertal_premium" },
                {"F4D3B02B107843C894ED517FC7DC8A39","suedtirolguestpass_mobilcard" },
                {"895C9B57E0D54B449C82F035538D4A79","suedtirolguestpass_museumobilcard" },
                {"742AA043BD5847C79EE93EEADF0BE0D2","natzschabscard" },
                {"05988DB63E5146E481C95279FB285C6A","accomodation bed bike" },
                {"5F22AD3E93D54E99B7E6F97719A47153","accomodation bett bike sport" },
            };
        }
    }

}
