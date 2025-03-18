using DataImportHelper;
using GenericHelper;
using LTSAPI;
using LTSAPI.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class TestRequests
    {
        public static async Task TestAccommodationSingle(Settings settings)
        {
            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

            DataImport dataimport = new DataImport(settings);

            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };

            await dataimport.ImportLTSAccommodationSingle("1DF9C17D6F24E87E4EDAAB3408588A6C");
            await dataimport.ImportLTSAccommodationSingle("525B5D14566741D3B5910721027B5ED7");

            var ltsacco = await ltsapi.AccommodationDetailRequest("525B5D14566741D3B5910721027B5ED7", null);
        }

        public static async Task TestAvailabilitySearch(Settings settings)
        {
            Stopwatch watch = Stopwatch.StartNew();

            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

            //https://tourism.api.opendatahub.bz.it/v1/Accommodation?pagenumber=1&pagesize=10000&arrival=2025-02-03&departure=2025-02-10&roominfo=0-18%2C18&language=de&availabilitychecklanguage=de&availabilitycheck=true&removenullvalues=false&bookablefilter=true&fields=Id%2CMssResponseShort&idfilter=FF050E6BE1F245FAD5E61495E17522D2&bokfilter=lts

            LTSAvailabilitySearchRequestBody body = new LTSAvailabilitySearchRequestBody()
            {
                accommodationRids = new List<string>() { "FF050E6BE1F245FAD5E61495E17522D2" },
                //marketingGroupRids = new List<string>() { "" },
                startDate = "2025-02-03",
                endDate = "2025-02-10",
                paging = new LTSAvailabilitySearchRequestPaging() { pageNumber = 1, pageSize = 10000 },
                cacheLifeTimeInSeconds = 300,
                onlySuedtirolInfoActive = true,
                roomOptions = new List<LTSAvailabilitySearchRequestRoomoption>() { new LTSAvailabilitySearchRequestRoomoption() { id = 1, guests = 2, guestAges = new List<int>() { 18, 18 } } }
            };


            var ltsavailablilitysearch = await ltsapi.AccommodationAvailabilitySearchRequest(null, body);

            var parsedavailabilitysearch = ltsavailablilitysearch[0].ToObject<LTSAvailabilitySearchResult>();

            var mssresult = AccommodationSearchResultParser.ParseLTSAccommodation(ltsavailablilitysearch.FirstOrDefault(), 1);

            //var testlts = await ltsapi.AccommodationDetailRequest("2657B7CBCb85380B253D2fBE28AF100E", null);

            Console.WriteLine(parsedavailabilitysearch.success);

            watch.Stop();

            Console.WriteLine("elapsed time: " + watch.ElapsedMilliseconds);

            Console.ReadLine();
        }
    }
}
