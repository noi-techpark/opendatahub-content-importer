// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LTSAPI.Parser
{
    public class AccommodationSearchResultParser
    {
        public static MssResult ParseLTSAccommodation(JObject accommodationsearchresult, int rooms)
        {
            try
            {
                LTSAvailabilitySearchResult accoltssearchresult = accommodationsearchresult.ToObject<LTSAvailabilitySearchResult>();

                return ParseLTSAccommodationSearchResult(accoltssearchresult, rooms);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static MssResult ParseLTSAccommodationSearchResult(
            LTSAvailabilitySearchResult accommodationsearchresult,
            int rooms)
        {
            try
            {
                if (accommodationsearchresult != null)
                {
                  
                    CultureInfo culturede = CultureInfo.CreateSpecificCulture("de");

                    MssResult result = new MssResult();
                    result.ResultId = accommodationsearchresult.resultSet.rid;
                    //min amount max amount
                    result.bookableHotels = accommodationsearchresult.data != null ? accommodationsearchresult.data.Count() : 0;
                    result.CheapestChannel = "lts";
                    result.Cheapestprice = 0;

                    if (accommodationsearchresult.data != null)
                    {
                        foreach (var offerdetail in accommodationsearchresult.data)
                        {
                            MssResponseShort lcsresponseshort = new MssResponseShort();

                            lcsresponseshort.A0RID = offerdetail.accommodation.rid;
                            lcsresponseshort.HotelId = 0;
                            lcsresponseshort.ChannelID = "lts";

                            //Check Günstigstes Angebot
                            List<CheapestOffer> CheapestOffer_ws = new List<CheapestOffer>();
                            List<CheapestOffer> CheapestOffer_bb = new List<CheapestOffer>();
                            List<CheapestOffer> CheapestOffer_hb = new List<CheapestOffer>();
                            List<CheapestOffer> CheapestOffer_fb = new List<CheapestOffer>();
                            List<CheapestOffer> CheapestOffer_ai = new List<CheapestOffer>();

                            //Für jedes Zimmerangebot
                            foreach (var roomdetail in offerdetail.roomOptions)
                            {
                                foreach (var roomdetailoffer in roomdetail.rates)
                                {
                                    RoomDetails myroomdetail = new RoomDetails();

                                    myroomdetail.RoomSeq = roomdetailoffer.roomOptionId;
                                    myroomdetail.RoomId = roomdetail.roomGroup.rid;
                                    myroomdetail.OfferId = roomdetailoffer.ratePlan.rid; // TODO change to rommdetailoffer.RID
                                    myroomdetail.Price_ai = roomdetailoffer.allInclusiveAmount;
                                    myroomdetail.Price_bb = roomdetailoffer.bedAndBreakfastAmount;
                                    myroomdetail.Price_fb = roomdetailoffer.fullBoardAmount;
                                    myroomdetail.Price_hb = roomdetailoffer.halfBoardAmount;
                                    myroomdetail.Price_ws = roomdetailoffer.roomOnlyAmount;
                                    myroomdetail.TotalPrice = 0;
                                    myroomdetail.TotalPriceString = "";
                                    //myroomdetail.Roomtitle = roomdetail.Name;  //available with more details??
                                    //myroomdetail.Roomdesc = roomdetailoffer.
                                    myroomdetail.Roomfree = roomdetail.availableRooms;
                                    myroomdetail.Roommax = 0;
                                    myroomdetail.Roommin = 0;
                                    myroomdetail.Roomstd = 0;
                                    //NOT found the roomtype?
                                    //myroomdetail.Roomtype = roomdetail.Genre != null ? Convert.ToInt32(roomdetail.Genre) : 0;  //roomdetail.Genre;
                                                                                                                               //myroomdetail.Roomtype = 0;

                                    if (roomdetailoffer.allInclusiveAmount != null && roomdetailoffer.allInclusiveAmount != 0)
                                    {
                                        var mycheapestofferai = new CheapestOffer() { RoomId = roomdetail.roomGroup.rid, Price = roomdetailoffer.allInclusiveAmount ?? 0, RoomSeq = (int)roomdetailoffer.roomOptionId, RoomFree = roomdetail.availableRooms };
                                        CheapestOffer_ai.Add(mycheapestofferai);
                                    }
                                    if (roomdetailoffer.bedAndBreakfastAmount != null && roomdetailoffer.bedAndBreakfastAmount != 0)
                                    {
                                        var mycheapestofferbb = new CheapestOffer() { RoomId = roomdetail.roomGroup.rid, Price = roomdetailoffer.bedAndBreakfastAmount ?? 0, RoomSeq = (int)roomdetailoffer.roomOptionId, RoomFree = roomdetail.availableRooms };
                                        CheapestOffer_bb.Add(mycheapestofferbb);
                                    }
                                    if (roomdetailoffer.halfBoardAmount != null && roomdetailoffer.halfBoardAmount != 0)
                                    {
                                        var mycheapestofferhb = new CheapestOffer() { RoomId = roomdetail.roomGroup.rid, Price = roomdetailoffer.halfBoardAmount ?? 0, RoomSeq = (int)roomdetailoffer.roomOptionId, RoomFree = roomdetail.availableRooms };
                                        CheapestOffer_hb.Add(mycheapestofferhb);
                                    }
                                    if (roomdetailoffer.fullBoardAmount != null && roomdetailoffer.fullBoardAmount != 0)
                                    {
                                        var mycheapestofferfb = new CheapestOffer() { RoomId = roomdetail.roomGroup.rid, Price = roomdetailoffer.fullBoardAmount ?? 0, RoomSeq = (int)roomdetailoffer.roomOptionId, RoomFree = roomdetail.availableRooms };
                                        CheapestOffer_fb.Add(mycheapestofferfb);
                                    }
                                    if (roomdetailoffer.roomOnlyAmount != null && roomdetailoffer.roomOnlyAmount != 0)
                                    {
                                        var mycheapestofferws = new CheapestOffer() { RoomId = roomdetail.roomGroup.rid, Price = roomdetailoffer.roomOnlyAmount ?? 0, RoomSeq = (int)roomdetailoffer.roomOptionId, RoomFree = roomdetail.availableRooms };
                                        CheapestOffer_ws.Add(mycheapestofferws);
                                    }


                                    lcsresponseshort.RoomDetails.Add(myroomdetail);
                                }
                            }


                            //Getting cheapest offer
                            var cheapestofferobj_ai = RoomCalculationHelper.CalculateCheapestRooms(CheapestOffer_ai, rooms, "ai");
                            lcsresponseshort.CheapestOffer_ai = cheapestofferobj_ai != null ? cheapestofferobj_ai.Price : 0;
                            if (cheapestofferobj_ai != null && cheapestofferobj_ai.Price > 0)
                                lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_ai);

                            var cheapestofferobj_bb = RoomCalculationHelper.CalculateCheapestRooms(CheapestOffer_bb, rooms, "bb");
                            lcsresponseshort.CheapestOffer_bb = cheapestofferobj_bb != null ? cheapestofferobj_bb.Price : 0;
                            if (cheapestofferobj_bb != null && cheapestofferobj_bb.Price > 0)
                                lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_bb);

                            var cheapestofferobj_hb = RoomCalculationHelper.CalculateCheapestRooms(CheapestOffer_hb, rooms, "hb");
                            lcsresponseshort.CheapestOffer_hb = cheapestofferobj_hb != null ? cheapestofferobj_hb.Price : 0;
                            if (cheapestofferobj_hb != null && cheapestofferobj_hb.Price > 0)
                                lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_hb);

                            var cheapestofferobj_fb = RoomCalculationHelper.CalculateCheapestRooms(CheapestOffer_fb, rooms, "fb");
                            lcsresponseshort.CheapestOffer_fb = cheapestofferobj_fb != null ? cheapestofferobj_fb.Price : 0;
                            if (cheapestofferobj_fb != null && cheapestofferobj_fb.Price > 0)
                                lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_fb);

                            var cheapestofferobj_ws = RoomCalculationHelper.CalculateCheapestRooms(CheapestOffer_ws, rooms, "ws");
                            lcsresponseshort.CheapestOffer_ws = cheapestofferobj_ws != null ? cheapestofferobj_ws.Price : 0;
                            if (cheapestofferobj_ws != null && cheapestofferobj_ws.Price > 0)
                                lcsresponseshort.CheapestOfferDetail.Add(cheapestofferobj_ws);

                            //Cheapest Offer General
                            if (lcsresponseshort.CheapestOffer == 0)
                            {
                                List<double> cheapestoffertotal = new List<double>();
                                if (lcsresponseshort.CheapestOffer_ai > 0)
                                    cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_ai);
                                if (lcsresponseshort.CheapestOffer_bb > 0)
                                    cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_bb);
                                if (lcsresponseshort.CheapestOffer_fb > 0)
                                    cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_fb);
                                if (lcsresponseshort.CheapestOffer_hb > 0)
                                    cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_hb);
                                if (lcsresponseshort.CheapestOffer_ws > 0)
                                    cheapestoffertotal.Add((double)lcsresponseshort.CheapestOffer_ws);

                                if (cheapestoffertotal.Count > 0)
                                {
                                    var cheapestofferdouble = cheapestoffertotal.OrderBy(x => x).FirstOrDefault();
                                    lcsresponseshort.CheapestOffer = cheapestofferdouble;
                                    lcsresponseshort.CheapestOfferString = String.Format(culturede, "{0:0,0.00}", cheapestofferdouble);
                                }

                            }

                            //Add only if there is a valid Offer (enough Roomfree etc..)
                            if (lcsresponseshort.CheapestOfferDetail != null && lcsresponseshort.CheapestOfferDetail.Count > 0)
                                result.MssResponseShort.Add(lcsresponseshort);
                            else
                            {

                            }
                        }
                    }


                    return result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

    public class AccommodationSearchUtilities
    {
        public static List<LTSAvailabilitySearchRequestRoomoption> RoomstringtoAvailabilitySearchRequestRoomoption(string roominfo)
        {
            if (!String.IsNullOrEmpty(roominfo) && roominfo != "null")
            {
                //roominfo has format 1Z-1P-18 oder 1Z-2P-18.18,1Z-1P-18
                List<LTSAvailabilitySearchRequestRoomoption> myroominfo = new List<LTSAvailabilitySearchRequestRoomoption>();

                var zimmerinfos = roominfo.Split('|');
                int roomseq = 1;

                foreach (var zimmerinfo in zimmerinfos)
                {
                    List<int> mypersons = new List<int>();

                    var myspittetzimmerinfo = zimmerinfo.Split('-');

                    var mypersoninfo = myspittetzimmerinfo[1].Split(',');
                    foreach (string s in mypersoninfo)
                    {
                        if(int.TryParse(s, out int sint))
                            mypersons.Add(sint);
                    }

                    var myroom = new LTSAvailabilitySearchRequestRoomoption();
                    myroom.id = roomseq;

                    int.TryParse(myspittetzimmerinfo[0], out int roomtype);

                    if(roomtype != 0)
                        myroom.typeBitmask = roomtype;

                    myroom.guests = mypersons.Count;
                    
                    myroom.guestAges = mypersons;

                    //var myroom = new Tuple<string, string, List<string>>(roomseq.ToString(), myspittetzimmerinfo[0].Substring(0), mypersons);

                    myroominfo.Add(myroom);
                    roomseq++;
                }

                return myroominfo;
            }
            else
            {
                List<LTSAvailabilitySearchRequestRoomoption> myroominfostd = new List<LTSAvailabilitySearchRequestRoomoption>();
                myroominfostd.Add(
                    new LTSAvailabilitySearchRequestRoomoption()
                    {
                        id = 1,                        
                        guests = 2,
                        guestAges = new List<int>() { 18, 18 },
                    }
                );

                return myroominfostd;
            }
        }
    }
}
