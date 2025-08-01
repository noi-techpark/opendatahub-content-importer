﻿// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using GenericHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HGVApi.Parser
{
    public class AccommodationParser
    {
        public static AccommodationV2 ParseHGVAccommodations(AccoHGV accommodation)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<AccommodationRoomV2> ParseHGVAccommodationRoom(string lang, XElement mssresponse, IDictionary<string, XDocument> xmlfiles)
        {
            var myresult = mssresponse.Elements("result").Elements("hotel");

            List<AccommodationRoomV2> myroomlist = new List<AccommodationRoomV2>();

            foreach (var myhotelresult in myresult)
            {
                myroomlist.AddRange(HGVRoomResponseParser(myhotelresult, xmlfiles, lang));
            }

            return myroomlist;
        }

        private static List<AccommodationRoomV2> HGVRoomResponseParser(XElement myresult, IDictionary<string, XDocument> xmlfiles, string language)
        {
            try
            {
                CultureInfo culturede = CultureInfo.CreateSpecificCulture("de");

                List<AccommodationRoomV2> myaccorooms = new List<AccommodationRoomV2>();

                string A0RID = myresult.Element("id_lts").Value;
                string HgvId = myresult.Element("id").Value;

                //Für jedes Zimmer vom Typ hgv
                if (myresult.Element("channel") != null)
                {
                    var mychannelresults = myresult.Elements("channel");

                    foreach (var mychannelresult in mychannelresults)
                    {
                        if (mychannelresult.Element("channel_id").Value == "hgv")
                        {
                            var myroomlist = mychannelresult.Element("room_description").Elements("room");

                            foreach (var myroom in myroomlist)
                            {
                                AccommodationRoomV2 myroomtosave = new AccommodationRoomV2();

                                string roomid = myroom.Element("room_id").Value;
                                string roomidlts = myroom.Element("room_lts_id").Value != null ? myroom.Element("room_lts_id").Value : "";

                                myroomtosave.Id = roomid + "hgv" + roomidlts;
                                myroomtosave.A0RID = A0RID.ToUpper();
                                myroomtosave.RoomCode = myroom.Element("room_code").Value;
                                myroomtosave.HGVId = roomid;
                                myroomtosave.LTSId = myroom.Element("room_lts_id").Value != null ? myroom.Element("room_lts_id").Value : "";
                                myroomtosave.Source = "hgv";

                                string roomtyp = myroom.Element("room_type").Value;
                                if (roomtyp == "1")
                                    myroomtosave.Roomtype = "room";
                                else if (roomtyp == "2")
                                    myroomtosave.Roomtype = "apartment";
                                else
                                    myroomtosave.Roomtype = "undefined";

                                myroomtosave.RoomtypeInt = Convert.ToInt32(roomtyp);
                                myroomtosave.RoomClassificationCodes = AlpineBitsHelper.GetRoomClassificationCode(myroomtosave.Roomtype);

                                string roommax = myroom.Element("occupancy").Element("max").Value;
                                string roommin = myroom.Element("occupancy").Element("min").Value;
                                string roomstd = myroom.Element("occupancy").Element("std").Value;

                                if (!String.IsNullOrEmpty(roommax))
                                    myroomtosave.Roommax = Convert.ToInt32(roommax);
                                else
                                    myroomtosave.Roommax = null;

                                if (!String.IsNullOrEmpty(roommin))
                                    myroomtosave.Roommin = Convert.ToInt32(roommin);
                                else
                                    myroomtosave.Roommin = null;

                                if (!String.IsNullOrEmpty(roomstd))
                                    myroomtosave.Roomstd = Convert.ToInt32(roomstd);
                                else
                                    myroomtosave.Roomstd = null;


                                string pricefrom = myroom.Element("price_from").Value;

                                if (!String.IsNullOrEmpty(pricefrom))
                                    myroomtosave.PriceFrom = Convert.ToDouble(pricefrom);
                                else
                                    myroomtosave.PriceFrom = null;


                                myroomtosave.Shortname = myroom.Element("title").Value;

                                //zimmeranzahl

                                if (myroom.Element("room_numbers") != null)
                                {
                                    var myroomcount = myroom.Element("room_numbers").Elements("number").Count();

                                    myroomtosave.RoomQuantity = myroomcount;

                                    myroomtosave.RoomNumbers = myroom.Element("room_numbers").Elements("number").Select(x => x.Value).ToList();
                                }

                                //Roomdetail

                                AccoRoomDetail mydetail = new AccoRoomDetail();

                                mydetail.Language = language;
                                mydetail.Name = myroom.Element("title").Value;
                                mydetail.Shortdesc = myroom.Element("description").Value;
                                mydetail.Longdesc = myroom.Element("description").Value;

                                myroomtosave.AccoRoomDetail.TryAddOrUpdate(language, mydetail);

                                //Features     

                                if (myroom.Element("features_view") != null)
                                {
                                    var myfeatures = myroom.Element("features_view").Elements("feature");

                                    List<AccoFeatureLinked> myroomfeatureslist = new List<AccoFeatureLinked>();
                                    foreach (var feature in myfeatures)
                                    {
                                        AccoFeatureLinked myroomfeature = new AccoFeatureLinked();

                                        string amenityid = feature.Element("id").Value;

                                        myroomfeature.HgvId = amenityid;
                                        myroomfeature.Name = feature.Element("title").Value;

                                        //LTS Feature ID + OTA Code                                        
                                        string ltsamenityid = "";
                                        string otacodes = "";

                                        var myamenity = xmlfiles["RoomAmenities"].Root.Elements("amenity").Where(x => x.Element("hgvid").Value == amenityid).FirstOrDefault();

                                        if (myamenity != null)
                                        {
                                            ltsamenityid = myamenity.Element("ltsrid").Value;
                                            otacodes = myamenity.Element("ota_codes") != null ? myamenity.Element("ota_codes").Value : "";
                                        }

                                        myroomfeature.Id = ltsamenityid;
                                        myroomfeature.OtaCodes = otacodes;

                                        if (!String.IsNullOrEmpty(otacodes))
                                        {
                                            var otacodessplittet = otacodes.Split(',').ToList();
                                            List<int> amenitycodes = new List<int>();

                                            foreach (var otacodesplit in otacodessplittet)
                                            {
                                                amenitycodes.Add(Convert.ToInt32(otacodesplit));
                                            }

                                            myroomfeature.RoomAmenityCodes = amenitycodes;
                                        }


                                        myroomfeatureslist.Add(myroomfeature);
                                    }

                                    myroomtosave.Features = myroomfeatureslist.ToList();
                                }
                                //Pictures

                                if (myroom.Element("pictures") != null)
                                {
                                    var myroompictures = myroom.Element("pictures").Elements("picture");

                                    int i = 0;

                                    List<ImageGallery> mypictureslist = new List<ImageGallery>();
                                    foreach (var picture in myroompictures)
                                    {
                                        ImageGallery mainimage = new ImageGallery();

                                        mainimage.ImageUrl = picture.Element("url").Value;
                                        mainimage.ImageName = picture.Element("title").Value != null ? picture.Element("title").Value : "";
                                        mainimage.Height = 0;
                                        mainimage.Width = 0;
                                        mainimage.ImageSource = "HGV";
                                        mainimage.ListPosition = i;

                                        mypictureslist.Add(mainimage);
                                        i++;
                                    }

                                    myroomtosave.ImageGallery = mypictureslist.ToList();
                                }

                                myroomtosave.LastChange = DateTime.Now;

                                myaccorooms.Add(myroomtosave);
                            }
                        }
                    }

                }

                return myaccorooms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
