using DataModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LTSAPI.Utils;
using GenericHelper;

namespace LTSAPI.Parser
{    
    public class PointofInterestParser
    {
        public static ODHActivityPoiLinked ParseLTSPointofInterest(
            JObject poilts, bool reduced
            )
        {
            try
            {
                LTSPointofInterest poiltsdetail = poilts.ToObject<LTSPointofInterest>();

                return ParseLTSPointofInterest(poiltsdetail.data, reduced);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static ODHActivityPoiLinked ParseLTSPointofInterest(
            LTSPointofInterestData ltspoi,
            bool reduced)
        {
            ODHActivityPoiLinked odhactivitypoi = new ODHActivityPoiLinked();

            odhactivitypoi.Id = ltspoi.rid;
            odhactivitypoi._Meta = new Metadata() { Id = odhactivitypoi.Id, LastUpdate = DateTime.Now, Reduced = reduced, Source = "lts", Type = "odhactivitypoi", UpdateInfo = new UpdateInfo() { UpdatedBy = "importer.v2", UpdateSource = "lts.interface.v2" } };
            odhactivitypoi.Source = "lts";

            odhactivitypoi.LastChange = ltspoi.lastUpdate;

            //Tourism Organization
            odhactivitypoi.TourismorganizationId = ltspoi.tourismOrganization.rid;
            odhactivitypoi.LocationInfo = new LocationInfoLinked();

            odhactivitypoi.LocationInfo.DistrictInfo = new DistrictInfoLinked() { Id = ltspoi.district.rid };

            odhactivitypoi.HasLanguage = new List<string>();
            odhactivitypoi.Detail = new Dictionary<string,Detail>();

            //Let's find out for which languages there is a name
            foreach (var name in ltspoi.name)
            {
                if (!String.IsNullOrEmpty(name.Value))
                    odhactivitypoi.HasLanguage.Add(name.Key);
            }

            //Detail Information
            foreach(var language in odhactivitypoi.HasLanguage)
            {
                Detail detail = new Detail();

                detail.Title = ltspoi.name[language];
                detail.BaseText = ltspoi.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language);
                detail.IntroText = ltspoi.descriptions.Where(x => x.type == "shortDescription").FirstOrDefault()?.description.GetValue(language);
                detail.ParkingInfo = ltspoi.descriptions.Where(x => x.type == "howToPark").FirstOrDefault()?.description.GetValue(language);
                detail.GetThereText = ltspoi.descriptions.Where(x => x.type == "howToArrive").FirstOrDefault()?.description.GetValue(language);
                detail.IntroText = ltspoi.descriptions.Where(x => x.type == "generalDescription").FirstOrDefault()?.description.GetValue(language);

                detail.AdditionalText = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.PublicTransportationInfo = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.AuthorTip = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.SafetyInfo = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);
                detail.EquipmentInfo = ltspoi.descriptions.Where(x => x.type == "").FirstOrDefault()?.description.GetValue(language);

                odhactivitypoi.Detail.TryAddOrUpdate(language, detail);
            }            

            //Contact Information

            //Opening Schedules

            //Tags

            //Images

            //Videos

            //Custom Fields


            return odhactivitypoi;
        }
    }

}
