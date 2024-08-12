using System.Net;
using System.Text;
using System.Xml.Linq;

namespace HGVApi
{
    public class HGVRequest
    {
        public static async Task<XElement> GetMssRoomlistAsync(string lang, string hotelid, string hotelidofchannel, XElement roomdetails, XDocument roomamenities, string source, string version)
        {
            try
            {
                //TODO add this from config
                MssRequest mssRequest = new MssRequest(null);

                XDocument myrequest = mssRequest.BuildRoomlistPostData(roomdetails, hotelid, hotelidofchannel, lang, source, version);
                var myresponses = mssRequest.RequestRoomListAsync(myrequest);

                await Task.WhenAll(myresponses);

                Task<string> roomresponsecontent = myresponses.Result.Content.ReadAsStringAsync();

                await Task.WhenAll(roomresponsecontent);

                XElement fullresponse = XElement.Parse(roomresponsecontent.Result);
                
                return fullresponse;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

    public enum MSSFunction
    {
        getHotelList,
        getSpecialList,
        getRoomList,
        getLocationList
    }

    public class HGVCredentials
    {
        public string serviceurl { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class MssRequest
    {
        string baseurl;
        HGVCredentials credentials;
        public MssRequest(HGVCredentials _credentials)
        {
            this.baseurl = _credentials.serviceurl; // @"http://www.easymailing.eu/mss/mss_service.php";
            this.credentials = _credentials;
        }



        public async Task<HttpResponseMessage> RequestHotelListAsync(XDocument request)
        {
            try
            {
                HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
                myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
                string serviceurl = baseurl + "?function=" + MSSFunction.getHotelList + "&mode=1";
                var myresponse = await myclient.PostAsync(serviceurl, new StringContent(request.ToString(), Encoding.UTF8, "text/xml"));

                return myresponse;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

        public async Task<HttpResponseMessage> RequestSpecialListAsync(XDocument request)
        {
            try
            {
                HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
                myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
                string serviceurl = baseurl + "?function=" + MSSFunction.getSpecialList + "&mode=1";
                var myresponse = await myclient.PostAsync(serviceurl, new StringContent(request.ToString(), Encoding.UTF8, "text/xml"));

                return myresponse;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

        public async Task<HttpResponseMessage> RequestRoomListAsync(XDocument request)
        {
            try
            {
                HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
                myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
                string serviceurl = baseurl + "?function=" + MSSFunction.getRoomList + "&mode=1";

                var myresponse = await myclient.PostAsync(serviceurl, new StringContent(request.ToString(), Encoding.UTF8, "text/xml"));

                return myresponse;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }        

        public async Task<HttpResponseMessage> RequestLocationListAsync(XDocument request)
        {
            try
            {
                HttpClient myclient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
                myclient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
                string serviceurl = baseurl + "?function=" + MSSFunction.getLocationList + "&mode=1";

                var myresponse = await myclient.PostAsync(serviceurl, new StringContent(request.ToString(), Encoding.UTF8, "text/xml"));

                return myresponse;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }


        //Build the Post Data

        public XDocument BuildPostData(XElement idlist, XElement channel, XElement roomlist, DateTime arrival, DateTime departure, XElement offerdetails, XElement hoteldetails, XElement type, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getHotelList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        idlist.Elements("id"),
                        new XElement("id_ofchannel", "lts"),
                        new XElement("search_offer",
                            channel.Elements("channel_id"),
                            new XElement("arrival", String.Format("{0:yyyy-MM-dd}", arrival)),
                            new XElement("departure", String.Format("{0:yyyy-MM-dd}", departure)),
                            new XElement("service", service),
                            roomlist.Elements("room"),
                            type
                            )
                        ),
                        new XElement("options",
                            offerdetails,
                            hoteldetails
                            ),
                        new XElement("order"),
                        new XElement("logging",
                            new XElement("step")
                            )
                    )
                );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildPostData(XElement channel, XElement roomlist, DateTime arrival, DateTime departure, XElement offerdetails, XElement hoteldetails, XElement type, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getHotelList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        new XElement("search_offer",
                            channel,
                            new XElement("arrival", String.Format("{0:yyyy-MM-dd}", arrival)),
                            new XElement("departure", String.Format("{0:yyyy-MM-dd}", departure)),
                            new XElement("service", service),
                            roomlist.Elements("room"),
                            type
                            )
                        ),
                        new XElement("options",
                            offerdetails,
                            hoteldetails
                            ),
                        new XElement("order"),
                        new XElement("logging",
                            new XElement("step")
                            )
                    )
                );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildPostData2(XElement channels, XElement roomlist, DateTime arrival, DateTime departure, XElement offerdetails, XElement hoteldetails, XElement type, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getHotelList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        new XElement("search_offer",
                            channels.Elements("channel_id"),
                            new XElement("arrival", String.Format("{0:yyyy-MM-dd}", arrival)),
                            new XElement("departure", String.Format("{0:yyyy-MM-dd}", departure)),
                            new XElement("service", service),
                            roomlist.Elements("room"),
                            type
                            )
                        ),
                        new XElement("options",
                            offerdetails,
                            hoteldetails
                            ),
                        new XElement("order"),
                        new XElement("logging",
                            new XElement("step")
                            )
                    )
                );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildBaseSearchPostData(XElement offerdetails, XElement hoteldetails, string lang, string idofchannel, string version, string source)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getHotelList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        new XElement("id_ofchannel", idofchannel)),
                    new XElement("options",
                        offerdetails,
                        hoteldetails
                        ),
                    new XElement("order"),
                    new XElement("logging",
                        new XElement("step")
                        )
                    )
                );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildSpecialPostData(XElement offerid, XElement roomlist, DateTime arrival, DateTime departure, XElement specialdetails, int typ, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getSpecialList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        new XElement("search_special",
                            offerid.Elements("offer_id"),
                            new XElement("date_from", String.Format("{0:yyyy-MM-dd}", arrival)),
                            new XElement("date_to", String.Format("{0:yyyy-MM-dd}", departure)),
                            new XElement("typ", typ),
                            new XElement("validity",
                                new XElement("valid", "0"),
                                new XElement("offers", "0"),
                                new XElement("service", service),
                                roomlist.Elements("room")
                                )
                            )
                        ),
                        new XElement("options",
                            specialdetails
                            ),
                        new XElement("order",
                                new XElement("field", "date"),
                                new XElement("dir", "asc")
                            )
                        )
                    );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildRoomlistPostData(XElement roomdetails, string hotelid, string idofchannel, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getRoomList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        new XElement("id", hotelid),
                        new XElement("id_ofchannel", idofchannel)
                       ),
                        new XElement("options",
                            roomdetails
                            )
                        )
                    );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildSpecialPostDatawithPremium(XElement offerid, XElement roomlist, DateTime arrival, DateTime departure, XElement specialdetails, int typ, int premium, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getSpecialList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        )),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        new XElement("search_special",
                            offerid.Elements("offer_id"),
                            new XElement("date_from", String.Format("{0:yyyy-MM-dd}", arrival)),
                            new XElement("date_to", String.Format("{0:yyyy-MM-dd}", departure)),
                            new XElement("typ", typ),
                            new XElement("premium", premium),
                            new XElement("validity",
                                new XElement("valid", "0"),
                                new XElement("offers", "0"),
                                new XElement("service", service),
                                roomlist.Elements("room")
                                )
                            )
                        ),
                        new XElement("options",
                            specialdetails
                            ),
                        new XElement("order",
                                new XElement("field", "date"),
                                new XElement("dir", "asc")
                            )
                        )
                    );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildSpecialPostDataCheckAvailability(XElement idlist, XElement offerid, XElement roomlist, DateTime arrival, DateTime departure, XElement specialdetails, XElement hoteldetails, int typ, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getSpecialList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        ), new XElement("result_id")),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        idlist.Elements("id"),
                        //new XElement("id_ofchannel", "lts"),
                        new XElement("search_special",
                            //new XElement("date_from", String.Format("{0:yyyy-MM-dd}", arrival)),
                            //new XElement("date_to", String.Format("{0:yyyy-MM-dd}", departure)),
                            offerid.Elements("offer_id"),
                            new XElement("typ", typ),
                            new XElement("validity",
                                new XElement("valid", "0"),
                                new XElement("offers", "1"),
                                new XElement("arrival", String.Format("{0:yyyy-MM-dd}", arrival)),
                                new XElement("departure", String.Format("{0:yyyy-MM-dd}", departure)),
                                new XElement("service", service),
                                roomlist.Elements("room")
                                )
                            )
                        ),
                        new XElement("options",
                            specialdetails,
                            hoteldetails
                            ),
                        new XElement("order",
                                new XElement("field", "date"),
                                new XElement("dir", "asc")
                            )
                        )
                    );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }
        
        public XDocument BuildSpecialPostDataCheckAvailabilitywithPremium(XElement idlist, XElement offerid, XElement roomlist, DateTime arrival, DateTime departure, XElement specialdetails, XElement hoteldetails, int typ, int premium, int service, string lang, string source, string version)
        {
            XElement myroot =
                new XElement("root",
                new XElement("version", version + ".0"),
                new XElement("header",
                    new XElement("credentials",
                        new XElement("user", credentials.username),
                        new XElement("password", credentials.password),
                        new XElement("source", source)
                        ),
                    new XElement("method", "getSpecialList"),
                    new XElement("paging",
                        new XElement("start", "0"),
                        new XElement("limit", "0")
                        ), new XElement("result_id")),
                new XElement("request",
                    new XElement("search",
                        new XElement("lang", lang),
                        idlist.Elements("id"),
                        //new XElement("id_ofchannel", "lts"),
                        new XElement("search_special",
                            //new XElement("date_from", String.Format("{0:yyyy-MM-dd}", arrival)),
                            //new XElement("date_to", String.Format("{0:yyyy-MM-dd}", departure)),
                            offerid.Elements("offer_id"),
                            new XElement("typ", typ),
                            new XElement("premium", premium),
                            new XElement("validity",
                                new XElement("valid", "0"),
                                new XElement("offers", "1"),
                                new XElement("arrival", String.Format("{0:yyyy-MM-dd}", arrival)),
                                new XElement("departure", String.Format("{0:yyyy-MM-dd}", departure)),
                                new XElement("service", service),
                                roomlist.Elements("room")
                                )
                            )
                        ),
                        new XElement("options",
                            specialdetails,
                            hoteldetails
                            ),
                        new XElement("order",
                                new XElement("field", "date"),
                                new XElement("dir", "asc")
                            )
                        )
                    );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public XDocument BuildLocationListPostData(string rootid, string typ, string source, string version)
        {
            //<request>
            //  <!-- Search Parameter: -->
            //  <search>
            //    <!-- optional: 1..n Root of Locations: 1=Suedtirol  -->
            //    <root_id>1</root_id>
            //    <!-- optional: Typ of location(reg->region, com->community, cit=>location, vir=>virtual location)  -->
            //    <!-- <typ></typ>  -->
            //  </search>
            //  <!-- No order available -->
            //  <options>
            //    <location_details>0</location_details>
            //  </options>
            //</request>

            XElement myroot =
               new XElement("root",
               new XElement("version", version + ".0"),
               new XElement("header",
                   new XElement("credentials",
                       new XElement("user", credentials.username),
                       new XElement("password", credentials.password),
                       new XElement("source", source)
                       ),
                   new XElement("method", "getLocationList"),
                   new XElement("paging",
                       new XElement("start", "0"),
                       new XElement("limit", "0")
                       ), new XElement("result_id")),
               new XElement("request",
                   new XElement("search",
                       new XElement("root_id", rootid),
                       new XElement("typ", typ)
                       ),
                       new XElement("options",
                           new XElement("location_details", 0)
                           )
                       )
                   );

            XDocument encodedDoc8 = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                myroot
            );

            return encodedDoc8;
        }

        public static XElement BuildRoomData(List<HGVRoom> myroompropertys)
        {
            XElement myroomlist = new XElement("roomlist");

            foreach (var room in myroompropertys)
            {
                XElement roomroot = new XElement("room");

                foreach (var myperson in room.Person)
                {
                    roomroot.Add(new XElement("person", myperson));
                }
                roomroot.Add(
                    new XElement("room_seq", room.RoomSeq),
                    new XElement("room_type", room.RoomType)
                    );

                myroomlist.Add(roomroot);
            }

            return myroomlist;
        }

        public static XElement BuildIDList(List<string> A0RIdList)
        {
            XElement myidlist = new XElement("idlist");

            foreach (string a0rid in A0RIdList)
            {
                myidlist.Add(new XElement("id", a0rid));
            }

            return myidlist;
        }

        public static XElement BuildOfferIDList(List<string> offeridlist)
        {
            XElement myidlist = new XElement("idlist");

            foreach (string offerid in offeridlist)
            {
                myidlist.Add(new XElement("offer_id", offerid));
            }

            return myidlist;
        }

        public static XElement BuildType(string typ)
        {
            XElement typxelement = new XElement("typ", typ);

            return typxelement;
        }


        public static XElement BuildChannelList(List<string> channels)
        {
            XElement mychannellist = new XElement("channellist");

            foreach (string channel in channels)
            {
                mychannellist.Add(new XElement("channel_id", channel));
            }

            return mychannellist;
        }

        public static XElement BuildChannelList(string channel)
        {
            XElement mychannellist = new XElement("channellist");
            mychannellist.Add(new XElement("channel_id", channel));

            return mychannellist;
        }

        public static XElement BuildChannelList(string[] channels)
        {
            XElement mychannellist = new XElement("channellist");

            foreach (string channel in channels)
            {
                mychannellist.Add(new XElement("channel_id", channel));
            }

            return mychannellist;
        }
    }

    public class HGVRoom
    {
        private string roomSeq;
        public string RoomSeq
        {
            get { return roomSeq; }
            set { roomSeq = value; }
        }

        private string roomType;
        public string RoomType
        {
            get { return roomType; }
            set { roomType = value; }
        }

        private List<string> person;
        public List<string> Person
        {
            get { return person; }
            set { person = value; }
        }
    }

}
