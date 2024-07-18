using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccommodationTransformer.Parser
{
    public class AccoLTS
    {
        public bool success { get; set; }
        public AccoData data { get; set; }
    }

    public class AccoData
    {
        public Rateplan[] ratePlans { get; set; }
        public Type type { get; set; }
        public string rid { get; set; }
        public Addressgroup[] addressGroups { get; set; }
        public Amenity[] amenities { get; set; }
        public Areamap areaMap { get; set; }
        public Category category { get; set; }
        public Contact[] contacts { get; set; }
        public Description1[] descriptions { get; set; }
        public District district { get; set; }
        public Galery[] galeries { get; set; }
        public bool hasApartments { get; set; }
        public bool hasDorms { get; set; }
        public bool hasPitches { get; set; }
        public bool hasRooms { get; set; }
        public string hgvId { get; set; }
        public int id { get; set; }
        public Image1[] images { get; set; }
        public bool isAccommodation { get; set; }
        public bool isActive { get; set; }
        public bool isBookable { get; set; }
        public bool isCamping { get; set; }
        public bool isSuedtirolInfoActive { get; set; }
        public bool isTourismOrganizationMember { get; set; }
        public DateTime lastUpdate { get; set; }
        public DateTime lastUpdatePrices { get; set; }
        public DateTime lastUpdateAvailability { get; set; }
        public Mealplan[] mealPlans { get; set; }
        public Overview overview { get; set; }
        public Position position { get; set; }
        public string representationMode { get; set; }
        public Review[] reviews { get; set; }
        public Roomgroup[] roomGroups { get; set; }
        public Season[] seasons { get; set; }
        public Tourismorganization tourismOrganization { get; set; }
        public int trustyouScore { get; set; }
        public Suedtirolguestpass suedtirolGuestPass { get; set; }
        public Accessibility accessibility { get; set; }
    }

    public class Type
    {
        public string rid { get; set; }
    }

    public class Areamap
    {
        public string coordinateX { get; set; }
        public string coordinateY { get; set; }
        public string number { get; set; }
        public string routeColor { get; set; }
    }

    public class Category
    {
        public string rid { get; set; }
    }

    public class District
    {
        public string rid { get; set; }
    }

    public class Overview
    {
        public Camping camping { get; set; }
        public string luggageServiceEndTime { get; set; }
        public string luggageServiceStartTime { get; set; }
        public Parkingspaces parkingSpaces { get; set; }

        public string checkInStartTime { get; set; }
        public string checkInEndTime { get; set; }
        public string checkOutStartTime { get; set; }
        public string checkOutEndTime { get; set; }
        public string receptionEndTime { get; set; }
        public string receptionStartTime { get; set; }
        public string roomServiceEndTime { get; set; }
        public string roomServiceStartTime { get; set; }
    }

    public class Camping
    {
        public int capacityPersons { get; set; }
        public int dishwashingSpaces { get; set; }
        public int laundrySpaces { get; set; }
        public int pitches { get; set; }
        public int showers { get; set; }
        public int toilets { get; set; }
    }

    public class Parkingspaces
    {
        public int garage { get; set; }
        public int outdoor { get; set; }
    }

    public class Position
    {
        public int altitude { get; set; }
        public float[] coordinates { get; set; }
        public string type { get; set; }
    }

    public class Tourismorganization
    {
        public string rid { get; set; }
    }

    public class Suedtirolguestpass
    {
        public bool isActive { get; set; }
        public Cardtype[] cardTypes { get; set; }
    }

    public class Cardtype
    {
        public string rid { get; set; }
    }

    public class Accessibility
    {
        public Dictionary<string,string> website { get; set; }
        public Dictionary<string, string> description { get; set; }
    }       

    public class Rateplan
    {
        public string chargeType { get; set; }
        public string code { get; set; }
        public object[] descriptions { get; set; }
        public DateTime lastUpdate { get; set; }
        public Dictionary<string, string> name { get; set; }
        public string rid { get; set; }
    }

    public class Addressgroup
    {
        public string rid { get; set; }
    }

    public class Amenity
    {
        public string rid { get; set; }
    }

    public class Contact
    {
        public Address address { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string phone { get; set; }
        public string type { get; set; }
        public string website { get; set; }
    }

    public class Address
    {
        public Dictionary<string, string> city { get; set; }
        public string country { get; set; }
        public Dictionary<string, string> name { get; set; }
        public Dictionary<string, string> name2 { get; set; }
        public string postalCode { get; set; }
        public Dictionary<string, string> street { get; set; }
    }

    public class Description1
    {
        public Dictionary<string, string> description { get; set; }
        public string type { get; set; }
    }
   

    public class Galery
    {
        public Image[] images { get; set; }
        public bool isActive { get; set; }
        public int order { get; set; }
        public string rid { get; set; }
    }

    public class Image
    {
        public string copyright { get; set; }
        public int heightPixel { get; set; }
        public bool isActive { get; set; }
        public string license { get; set; }
        public int order { get; set; }
        public string rid { get; set; }
        public string url { get; set; }
        public int widthPixel { get; set; }
    }

    public class Image1
    {
        public DateTime applicableEndDate { get; set; }
        public DateTime applicableStartDate { get; set; }
        public string copyright { get; set; }
        public int heightPixel { get; set; }
        public string license { get; set; }
        public string rid { get; set; }
        public string url { get; set; }
        public int widthPixel { get; set; }
    }

    public class Mealplan
    {
        public string rid { get; set; }
    }

    public class Review
    {
        public string id { get; set; }
        public bool isActive { get; set; }
        public float rating { get; set; }
        public int? reviewsQuantity { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class Roomgroup
    {
        public Amenity1[] amenities { get; set; }
        public int baths { get; set; }
        public string classification { get; set; }
        public string code { get; set; }
        public object[] descriptions { get; set; }
        public int diningRooms { get; set; }
        public Image2[] images { get; set; }
        public bool isActive { get; set; }
        public DateTime lastUpdate { get; set; }
        public int livingRooms { get; set; }
        public float minAmountPerPersonPerDay { get; set; }
        public float minAmountPerUnitPerDay { get; set; }
        public Dictionary<string, string> name { get; set; }
        public Occupancy occupancy { get; set; }
        public string rid { get; set; }
        public int roomQuantity { get; set; }
        public Room[] rooms { get; set; }
        public int sleepingRooms { get; set; }
        public float squareMeters { get; set; }
        public int toilets { get; set; }
        public string type { get; set; }
    }
    
    public class Occupancy
    {
        public int max { get; set; }
        public int min { get; set; }
        public int minAdults { get; set; }
        public int standard { get; set; }
    }

    public class Amenity1
    {
        public string rid { get; set; }
    }

    public class Image2
    {
        public string copyright { get; set; }
        public int heightPixel { get; set; }
        public string license { get; set; }
        public Dictionary<string,string> name { get; set; }
        public int order { get; set; }
        public string rid { get; set; }
        public string url { get; set; }
        public int widthPixel { get; set; }
    }    

    public class Room
    {
        public Availability availability { get; set; }
        public string code { get; set; }
        public DateTime lastUpdate { get; set; }
        public string rid { get; set; }
    }

    public class Availability
    {
        public bool isBlocked { get; set; }
        public DateTime lastUpdate { get; set; }
        public string status { get; set; }
        public string statusStartDate { get; set; }
    }

    public class Season
    {
        public string endDate { get; set; }
        public string startDate { get; set; }
    }

}
