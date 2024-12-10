using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTSAPI
{
    #region Common 
    public class LTSData<T>
    {
        public bool success { get; set; }
        public T data { get; set; }
    }

    public class GenericLTSRidResult
    {
        public string rid { get; set; }
    }

    public class LTSDistrict
    {
        public string rid { get; set; }
    }

    public class LTSAccoContact
    {
        public LTSAddress address { get; set; }
        public string email { get; set; }
        public string fax { get; set; }
        public string phone { get; set; }
        public string type { get; set; }
        public string website { get; set; }
    }

    public class LTSEventContact
    {
        public LTSAddress address { get; set; }
        public IDictionary<string, string>? email { get; set; }
        public string phone { get; set; }
        public IDictionary<string, string>? website { get; set; }
    }

    public class LTSGastronomyContact
    {
        public string type { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string email { get; set; }
        public LTSAddress address { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
    }

    public class LTSAddress
    {
        public Dictionary<string, string>? city { get; set; }
        public string country { get; set; }
        public Dictionary<string, string>? name { get; set; }
        public Dictionary<string, string>? name2 { get; set; }
        public string postalCode { get; set; }
        public Dictionary<string, string>? street { get; set; }
    }


    public class LTSImage
    {
        public DateTime? applicableEndDate { get; set; }
        public DateTime? applicableStartDate { get; set; }

        public string copyright { get; set; }
        public int heightPixel { get; set; }
        public bool isActive { get; set; }
        public string license { get; set; }
        public int? order { get; set; }
        public string rid { get; set; }
        public string url { get; set; }
        public int widthPixel { get; set; }

        public Dictionary<string, string>? name { get; set; }
        public bool? isMainImage { get; set; }
    }

    public class LTSPosition
    {
        public int altitude { get; set; }
        public float[] coordinates { get; set; }
        public string type { get; set; }
    }

    public class LTSCategory
    {
        public string rid { get; set; }
    }

    public class LTSAreamap
    {
        public string coordinateX { get; set; }
        public string coordinateY { get; set; }
        public string number { get; set; }
        public string routeColor { get; set; }
    }

    public class LTSDescription
    {
        public Dictionary<string, string>? description { get; set; }
        public string type { get; set; }
    }

    #endregion

    #region Accommodation

    public class LTSAcco : LTSData<LTSAccoData>
    {
        public bool success { get; set; }
        public LTSAccoData data { get; set; }
    }

    public class LTSAccoData
    {
        public LTSRateplan[] ratePlans { get; set; }
        public LTSType type { get; set; }
        public string rid { get; set; }
        public LTSAddressgroup[] addressGroups { get; set; }
        public LTSAmenity[] amenities { get; set; }
        public LTSAreamap areaMap { get; set; }
        public LTSCategory category { get; set; }
        public LTSAccoContact[] contacts { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public LTSDistrict district { get; set; }
        public LTSGallery[] galeries { get; set; }
        public bool hasApartments { get; set; }
        public bool hasDorms { get; set; }
        public bool hasPitches { get; set; }
        public bool hasRooms { get; set; }
        public string hgvId { get; set; }
        public int id { get; set; }
        public LTSImage[] images { get; set; }
        public bool isAccommodation { get; set; }
        public bool isActive { get; set; }
        public bool isBookable { get; set; }
        public bool isCamping { get; set; }
        public bool isSuedtirolInfoActive { get; set; }
        public bool isTourismOrganizationMember { get; set; }
        public string? cinCode { get; set; }
        public DateTime lastUpdate { get; set; }
        public DateTime lastUpdatePrices { get; set; }
        public DateTime lastUpdateAvailability { get; set; }
        public LTSMealplan[] mealPlans { get; set; }
        public LTSOverview overview { get; set; }
        public LTSPosition position { get; set; }
        public string representationMode { get; set; }
        public LTSReview[] reviews { get; set; }
        public LTSRoomgroup[] roomGroups { get; set; }
        public LTSSeason[] seasons { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public int trustyouScore { get; set; }
        public LTSSuedtirolguestpass suedtirolGuestPass { get; set; }
        public LTSAccessibility accessibility { get; set; }
    }

    public class LTSType
    {
        public string rid { get; set; }
    }

    public class LTSOverview
    {
        public LTSCamping camping { get; set; }
        public string luggageServiceEndTime { get; set; }
        public string luggageServiceStartTime { get; set; }
        public LTSParkingspaces parkingSpaces { get; set; }

        public string checkInStartTime { get; set; }
        public string checkInEndTime { get; set; }
        public string checkOutStartTime { get; set; }
        public string checkOutEndTime { get; set; }
        public string receptionEndTime { get; set; }
        public string receptionStartTime { get; set; }
        public string roomServiceEndTime { get; set; }
        public string roomServiceStartTime { get; set; }
    }

    public class LTSCamping
    {
        public int capacityPersons { get; set; }
        public int dishwashingSpaces { get; set; }
        public int laundrySpaces { get; set; }
        public int pitches { get; set; }
        public int showers { get; set; }
        public int toilets { get; set; }
    }

    public class LTSParkingspaces
    {
        public int garage { get; set; }
        public int outdoor { get; set; }
    }

    public class LTSTourismorganization
    {
        public string rid { get; set; }
    }

    public class LTSSuedtirolguestpass
    {
        public bool isActive { get; set; }
        public LTSCardtype[] cardTypes { get; set; }
    }

    public class LTSCardtype
    {
        public string rid { get; set; }
    }

    public class LTSAccessibility
    {
        public Dictionary<string, string>? website { get; set; }
        public Dictionary<string, string>? description { get; set; }
    }

    public class LTSRateplan
    {
        public string chargeType { get; set; }
        public string code { get; set; }
        public LTSDescription[] descriptions { get; set; }

        public bool iusSpecialOffer { get; set; }
        public DateTime lastUpdate { get; set; }
        public Dictionary<string, string>? name { get; set; }
        public string rid { get; set; }
        public string? visibility { get; set; }
    }

    public class LTSAddressgroup
    {
        public string rid { get; set; }
    }

    public class LTSAmenity
    {
        public string rid { get; set; }
    }

    public class LTSGallery
    {
        public LTSImage[] images { get; set; }
        public bool isActive { get; set; }
        public int order { get; set; }
        public string rid { get; set; }
    }

    public class LTSMealplan
    {
        public string rid { get; set; }
    }

    public class LTSReview
    {
        public string id { get; set; }
        public bool isActive { get; set; }
        public float rating { get; set; }
        public int? reviewsQuantity { get; set; }
        public string status { get; set; }
        public string type { get; set; }
    }

    public class LTSRoomgroup
    {
        public LTSAmenity[] amenities { get; set; }
        public int baths { get; set; }
        public string classification { get; set; }
        public string code { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public int diningRooms { get; set; }
        public LTSImage[] images { get; set; }
        public bool isActive { get; set; }
        public DateTime lastUpdate { get; set; }
        public int livingRooms { get; set; }
        public float minAmountPerPersonPerDay { get; set; }
        public float minAmountPerUnitPerDay { get; set; }
        public Dictionary<string, string>? name { get; set; }
        public LTSOccupancy occupancy { get; set; }
        public string rid { get; set; }
        public int roomQuantity { get; set; }
        public LTSRoom[] rooms { get; set; }
        public int sleepingRooms { get; set; }
        public float squareMeters { get; set; }
        public int toilets { get; set; }
        public string type { get; set; }
    }

    public class LTSOccupancy
    {
        public int max { get; set; }
        public int min { get; set; }
        public int minAdults { get; set; }
        public int standard { get; set; }
    }

    public class LTSRoom
    {
        public LTSAvailability availability { get; set; }
        public string code { get; set; }
        public DateTime lastUpdate { get; set; }
        public string rid { get; set; }
    }

    public class LTSAvailability
    {
        public bool isBlocked { get; set; }
        public DateTime lastUpdate { get; set; }
        public string status { get; set; }
        public string statusStartDate { get; set; }
    }

    public class LTSSeason
    {
        public string endDate { get; set; }
        public string startDate { get; set; }
    }

    #endregion

    #region Event

    public class LTSEvent : LTSData<LTSEventData>
    {
        public new LTSEventData data { get; set; }
    }

    public class LTSEventData
    {
        public string rid { get; set; }
        public LTSCategory[] categories { get; set; }
        public LTSClassification classification { get; set; }
        public LTSEventContact contact { get; set; }
        public DateTime createdAt { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public LTSDistrict[] districts { get; set; }
        public LTSImage[] images { get; set; }
        public bool isActive { get; set; }
        public bool isRegistrationRequired { get; set; }
        public bool isTicketRequired { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string>? location { get; set; }
        public IDictionary<string, string>? meetingPoint { get; set; }
        public IDictionary<string, string>? name { get; set; }
        public LTSOrganizer organizer { get; set; }
        public LTSPeriod[] periods { get; set; }
        public LTSPosition position { get; set; }
        public LTSPublishersetting[] publisherSettings { get; set; }
        public IDictionary<string, string>? registration { get; set; }
        public LTSShopconfiguration shopConfiguration { get; set; }
        public LTSEventTag[] tags { get; set; }
        public IDictionary<string, string>? urlAlias { get; set; }
        public LTSUrl[] urls { get; set; }
    }

    public class LTSClassification
    {
        public string rid { get; set; }
    }

    public class LTSOrganizer
    {
        public string rid { get; set; }
    }

    public class LTSShopconfiguration
    {
        public IDictionary<string, string>? bookingUrl { get; set; }
        public bool isActive { get; set; }
    }



    public class LTSPeriod
    {
        public string rid { get; set; }
        public IDictionary<string, string>? cancellationDescription { get; set; }
        public IDictionary<string, string>? guide { get; set; }
        public IDictionary<string, string>? description { get; set; }
        public IDictionary<string, string>? registrationWithin { get; set; }
        public double? minAmount { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string entranceTime { get; set; }
        public bool isActive { get; set; }
        public bool isEachDayOwnEvent { get; set; }
        public bool isCancelled { get; set; }
        public int minParticipants { get; set; }
        public int maxParticipants { get; set; }
        public object[] openingHours { get; set; }
        public LTSTicketsale ticketSale { get; set; }
        public LTSVariant[] variants { get; set; }
        public LTSDay[] days { get; set; }
    }

    public class LTSTicketsale
    {
        public bool isActive { get; set; }
        public object onlineSaleUntil { get; set; }
        public int onlineContingent { get; set; }
    }

    public class LTSVariant
    {
        public string rid { get; set; }
    }

    public class LTSDay
    {
        public string rid { get; set; }
        public string startDate { get; set; }
        public string startTime { get; set; }
        public LTSEventAvailability availability { get; set; }
    }

    public class LTSEventAvailability
    {
        public int calculatedAvailability { get; set; }
        public bool isLowAvailability { get; set; }
        public bool isSoldOut { get; set; }
    }

    public class LTSPublishersetting
    {
        public int importanceRate { get; set; }
        public string publicationStatus { get; set; }
        public LTSPublisher publisher { get; set; }
    }

    public class LTSPublisher
    {
        public string rid { get; set; }
    }

    public class LTSEventTag
    {
        public string rid { get; set; }
    }

    public class LTSUrl
    {
        public string type { get; set; }
        public IDictionary<string, string>? url { get; set; }
    }

    #endregion

    #region Accommodation Availablity

    public class LTSAvailabilitySearchResult
    {
        public bool success { get; set; }
        public LTSPaging paging { get; set; }
        public LTSAvailabilitySearchResultset resultSet { get; set; }
        public LTSAvailabilitySearchData[] data { get; set; }
    }

    public class LTSPaging
    {
        public int resultsQuantity { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int pagesQuantity { get; set; }
    }

    public class LTSAvailabilitySearchResultset
    {
        public string rid { get; set; }
        public float lowestMinAmount { get; set; }
        public float highestMaxAmount { get; set; }
    }

    public class LTSAvailabilitySearchData
    {
        public GenericLTSRidResult accommodation { get; set; }
        public float minAmount { get; set; }
        public LTSAvailabilitySearchRoomoption[] roomOptions { get; set; }
    }

    public class LTSAvailabilitySearchRoomoption
    {
        public GenericLTSRidResult roomGroup { get; set; }
        public int availableRooms { get; set; }
        public int bookableRooms { get; set; }
        public LTSAvailabilitySearchRate[] rates { get; set; }
    }

    public class LTSAvailabilitySearchRate
    {
        public int roomOptionId { get; set; }
        public bool isBookable { get; set; }
        public bool isSpecialOffer { get; set; }
        public GenericLTSRidResult ratePlan { get; set; }
        public float? allInclusiveAmount { get; set; }
        public float? bedAndBreakfastAmount { get; set; }
        public float? fullBoardAmount { get; set; }
        public float? halfBoardAmount { get; set; }
        public float? roomOnlyAmount { get; set; }
    }

    #endregion

    #region Gastronomy

    public class LTSGastronomy : LTSData<LTSGastronomyData>
    {
        public new LTSGastronomyData data { get; set; }
    }

    public class LTSGastronomyData
    {
        public string rid { get; set; }
        public int id { get; set; }
        public DateTime lastUpdate { get; set; }
        public bool isActive { get; set; }
        public string representationMode { get; set; }
        public LTSDistrict district { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public LTSPosition position { get; set; }
        public int maxSeatingCapacity { get; set; }
        public LTSAreamap areaMap { get; set; }
        public LTSCategory[] categories { get; set; }
        public LTSGastronomyContact[] contacts { get; set; }
        public IDictionary<string, string> description { get; set; }
        public LTSImage[] images { get; set; }
        public LTSFacility[] facilities { get; set; }
        public LTSOpeningschedule[] openingSchedules { get; set; }
        public LTSCeremonyseatingcapacity[] ceremonySeatingCapacities { get; set; }
        public LTSDishrate[] dishRates { get; set; }
    }

    public class Image
    {
        public string rid { get; set; }
        public string url { get; set; }
        public int widthPixel { get; set; }
        public int heightPixel { get; set; }
        public int order { get; set; }
        public bool isMainImage { get; set; }
        public bool isCurrentMainImage { get; set; }
        public string applicableStartDate { get; set; }
        public string applicableEndDate { get; set; }
        public string copyright { get; set; }
        public string license { get; set; }
        public IDictionary<string, string>? name { get; set; }
    }

    public class LTSFacility
    {
        public string rid { get; set; }
    }

    public class LTSOpeningschedule
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        public bool isOpen { get; set; }
        public bool isMondayOpen { get; set; }
        public bool isTuesdayOpen { get; set; }
        public bool isWednesdayOpen { get; set; }
        public bool isThursdayOpen { get; set; }
        public bool isFridayOpen { get; set; }
        public bool isSaturdayOpen { get; set; }
        public bool isSundayOpen { get; set; }
        public LTSOpeningtime[] openingTimes { get; set; }
    }

    public class LTSOpeningtime
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string type { get; set; }
    }

    public class LTSCeremonyseatingcapacity
    {
        public LTSCeremony ceremony { get; set; }
        public int maxSeatingCapacity { get; set; }
    }

    public class LTSCeremony
    {
        public string rid { get; set; }
    }

    public class LTSDishrate
    {
        public LTSDish dish { get; set; }
        public float minAmount { get; set; }
        public float maxAmount { get; set; }
    }

    public class LTSDish
    {
        public string rid { get; set; }
    }


    #endregion

    #region Activities

    public class LTSActivity : LTSData<LTSActivityData>
    {
        public new LTSActivityData data { get; set; }
    }

    public class LTSActivityData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public string code { get; set; }
        public string specificNumberCode { get; set; }
        public int? order { get; set; }
        public LTSMountainbike mountainBike { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public LTSRating rating { get; set; }
        public IDictionary<string, string> location { get; set; }
        public LTSLiftpointcard liftPointCard { get; set; }
        public LTSArea[] areas { get; set; }
        public bool isIlluminated { get; set; }
        public bool hasRental { get; set; }
        public bool? hasLift { get; set; }
        public bool? isPossibleClimbByFeet { get; set; }
        public bool? hasBikeTransport { get; set; }
        public string minRopeLength { get; set; }
        public string quantityQuickDraws { get; set; }
        public bool isActive { get; set; }
        public bool isOpen { get; set; }
        public bool isPrepared { get; set; }
        public bool hasCopyright { get; set; }
        public bool isReadOnly { get; set; }
        public string favouriteFor { get; set; }
        public bool isPossibleRunToValley { get; set; }
        public object snowType { get; set; }
        public LTSSnowpark snowPark { get; set; }
        public IDictionary<string, string> novelty { get; set; }
        public LTSActivityOpeningschedule[] openingSchedules { get; set; }
        public LTSEventContact contact { get; set; }
        public LTSGeodata geoData { get; set; }
        public LTSTag[] tags { get; set; }
        public LTSImage[] images { get; set; }
        public LTSVideo[] videos { get; set; }
    }

    public class LTSMountainbike
    {
        public bool? isPermitted { get; set; }
        public int? officialWayNumber { get; set; }
    }

    public class LTSRating
    {
        public int? stamina { get; set; }
        public int? experience { get; set; }
        public int? landscape { get; set; }
        public int? difficulty { get; set; }
        public int? technique { get; set; }
        public string? viaFerrataTechnique { get; set; }
        public string? scaleUIAATechnique { get; set; }
        public string? singletrackScale { get; set; }
    }

    public class LTSLiftpointcard
    {
        public int? pointsSingleTripUp { get; set; }
        public int? pointsSingleTripDown { get; set; }
    }

    public class LTSSnowpark
    {
        public bool isInground { get; set; }
        public bool hasPipe { get; set; }
        public bool hasBoarderCross { get; set; }
        public bool hasArtificiallySnow { get; set; }
        public int jibsNumber { get; set; }
        public LTSJumpsnumber jumpsNumber { get; set; }
        public LTSLinesnumber linesNumber { get; set; }
    }

    public class LTSJumpsnumber
    {
        public int blu { get; set; }
        public int red { get; set; }
        public int black { get; set; }
    }

    public class LTSLinesnumber
    {
        public int blu { get; set; }
        public int red { get; set; }
        public int black { get; set; }
    }

    public class LTSGeodata
    {
        public LTSAltitudedifference altitudeDifference { get; set; }
        public LTSDistance distance { get; set; }
        public LTSExposition[] exposition { get; set; }
        public LTSPositionExtended[] positions { get; set; }
        public LTSGpstrack[] gpsTracks { get; set; }
    }

    public class LTSAltitudedifference
    {
        public int difference { get; set; }
        public int? max { get; set; }
        public int? min { get; set; }
    }

    public class LTSDistance
    {
        public int length { get; set; }
        public string duration { get; set; }
        public int? sumUp { get; set; }
        public int? sumDown { get; set; }
    }

    public class LTSExposition
    {
        public string value { get; set; }
    }

    public class LTSPositionExtended : LTSPosition
    {
        public GenericLTSRidResult category { get; set; }
    }

    public class LTSGpstrack
    {
        public string rid { get; set; }
        public LTSFile file { get; set; }
    }

    public class LTSFile
    {
        public float filesize { get; set; }
        public string url { get; set; }
    }

    public class LTSArea
    {
        public string rid { get; set; }
    }

    public class LTSActivityOpeningschedule
    {
        public string rid { get; set; }
        public string type { get; set; }
        public string validFrom { get; set; }
        public string validTo { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSOpeningschedule[] openingTimes { get; set; }
    }

    public class LTSTag
    {
        public string rid { get; set; }
        public GenericLTSRidResult[] properties { get; set; }
    }

    public class LTSVideo
    {
        public string rid { get; set; }
        public IDictionary<string, string> name { get; set; }
        public string source { get; set; }
        public GenericLTSRidResult genre { get; set; }
        public bool isActive { get; set; }
        public IDictionary<string, string> url { get; set; }
        public IDictionary<string, string> htmlSnippet { get; set; }
        public string copyright { get; set; }
        public string license { get; set; }
    }

    #endregion

    #region Pois

    public class LTSPointofInterest : LTSData<LTSPointofInterestData>
    {
        public new LTSPointofInterestData data { get; set; }
    }

    public class LTSPointofInterestData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public bool isTourismOrganizationMember { get; set; }
        public string code { get; set; }
        public LTSDistrict district { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public IDictionary<string, string> location { get; set; }
        public LTSArea[] areas { get; set; }
        public bool isActive { get; set; }
        public bool isOpen { get; set; }
        public bool isReadOnly { get; set; }
        public bool hasFreeEntry { get; set; }
        public string favouriteFor { get; set; }
        public bool hasCopyright { get; set; }
        public LTSActivityOpeningschedule[] openingSchedules { get; set; }
        public LTSEventContact contact { get; set; }
        public LTSGeodata position { get; set; }
        public LTSTag[] tags { get; set; }
        public LTSImage[] images { get; set; }
        public LTSVideo[] videos { get; set; }
        public IDictionary<string, string> novelty { get; set; }
        public GenericLTSRidResult[] beacons { get; set; }
    }

    #endregion

    #region Venues

    #endregion

    #region Webcams

    #endregion

    #region WeatherSnow

    #endregion

    #region SuedtirolGuestpass

    #endregion

    #region Tags

    #endregion








}
