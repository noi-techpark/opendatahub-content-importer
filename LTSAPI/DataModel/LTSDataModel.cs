using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

    public class LTSActivityPoiContact
    {
        public LTSAddress address { get; set; }
        public string? email { get; set; }
        public string phone { get; set; }
        public string? website { get; set; }
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
        public Dictionary<string, string>? complement { get; set; }
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
        public int? altitude { get; set; }
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
        //Unique identifier of the Event
        public string rid { get; set; }
        //Array Unique identifier of the category of the Event
        public LTSCategory[] categories { get; set; }
        public LTSClassification classification { get; set; }
        public LTSEventContact contact { get; set; }
        //Date and time of creation
        public DateTime createdAt { get; set; }
        //Localised description of the Event related to the type
        public LTSDescription[] descriptions { get; set; }
        //Unique identifier of the district array
        public LTSDistrict[] districts { get; set; }
        public LTSImage[] images { get; set; }
        //Defines if the Event is active
        public bool isActive { get; set; }
        //Defines if the registration is required for participation
        public bool isRegistrationRequired { get; set; }
        //Defines if a ticket is required for participation
        public bool isTicketRequired { get; set; }
        //Defines if the Event is included in Suedtirol Guest Pass
        public bool? isIncludedInSuedtirolGuestPass { get; set; }

        public string? eventLanguage { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //Localised name of the location of the Event
        public IDictionary<string, string>? location { get; set; }
        //Localised name of the meeting point of the Event
        public IDictionary<string, string>? meetingPoint { get; set; }
        //Localised name of the Event
        public IDictionary<string, string>? name { get; set; }
        public LTSOrganizer organizer { get; set; }
        public LTSPeriod[] periods { get; set; }
        public LTSPosition position { get; set; }
        public LTSPublishersetting[] publisherSettings { get; set; }
        //Localised description of the registration to the Event
        public IDictionary<string, string>? registration { get; set; }
        public LTSShopconfiguration shopConfiguration { get; set; }
        public LTSEventTag[] tags { get; set; }
        //Localised URL alias of the Event website
        public IDictionary<string, string>? urlAlias { get; set; }
        public LTSUrl[] urls { get; set; }

        public LTSVariant[] variants { get; set; }
    }

    public class LTSClassification
    {
        //Unique identifier of the classification of the Event
        public string rid { get; set; }
    }

    public class LTSOrganizer
    {
        //Unique identifier of the organizer of the Event
        public string rid { get; set; }
    }

    public class LTSShopconfiguration
    {
        //Localised URL of the booking website
        public IDictionary<string, string>? bookingUrl { get; set; }
        //Defines if the shop configuration is active
        public bool isActive { get; set; }
    }

    public class LTSPeriod
    {
        //Unique identifier of the period
        public string rid { get; set; }
        //Localised description of the cancellation
        public IDictionary<string, string>? cancellationDescription { get; set; }
        //Localised guide of the Event
        public IDictionary<string, string>? guide { get; set; }
        //Localised description of the period of the Event
        public IDictionary<string, string>? description { get; set; }
        //Localised description about the registration deadline
        public IDictionary<string, string>? registrationWithin { get; set; }
        //Lower bound of prices for the period
        public double? minAmount { get; set; }
        //Start date of the period
        public DateTime startDate { get; set; }
        //End date of the period
        public DateTime endDate { get; set; }
        //Start time of the period
        public string startTime { get; set; }
        //End time of the period
        public string endTime { get; set; }
        //Entrance time to the Event in the period
        public string entranceTime { get; set; }
        //Defines if the period is active
        public bool isActive { get; set; }
        //Defines if there exist one sellable Event for each day of the period
        public bool isEachDayOwnEvent { get; set; }
        //"Defines if the period is cancelled
        public bool isCancelled { get; set; }
        //Minimum number of participants allowed for the period
        public int minParticipants { get; set; }
        //Maximum number of participants allowed for the period
        public int maxParticipants { get; set; }
        public LTSPeriodOpeningTime[]? openingHours { get; set; }
        public LTSTicketsale ticketSale { get; set; }
        public LTSPeriodVariant[] variants { get; set; }
        public LTSDay[] days { get; set; }
    }

    public class LTSTicketsale
    {
        //Defines if the ticket sale is active for the period
        public bool isActive { get; set; }
        //Number of minutes before the Event before which ticket sale is allowed
        public int onlineSaleUntil { get; set; }
        //Maximum number of tickets sellable online
        public int onlineContingent { get; set; } 
    }

    public class LTSVariant
    {
        //Localised name of the variant of the Event
        public IDictionary<string, string>? name { get; set; }
        //Sorting number of the variant
        public int order { get; set; }
        //Price of the variant in euros
        public double? price { get; set; }
        //Unique identifier of the variant of the Event
        public string rid { get; set; }
        public LTSVariantCategory[] variantCategory { get; set; }
    }

    public class LTSVariantCategory
    {
        //Unique identifier of the category of the variant
        public string rid { get; set; }
    }

    public class LTSPeriodVariant
    {
        //Unique identifier of the variant of the period
        public string rid { get; set; }
    }

    public class LTSPeriodOpeningTime
    {
        //Starting time of the first daily opening
        public string startTime1 { get; set; }
        //Starting time of the second daily opening
        public string startTime2 { get; set; }
        //End time of the first daily opening
        public string endTime1 { get; set; }
        //End time of the second daily opening
        public string endTime2 { get; set; }
        //Entrance time of the first daily opening
        public string entranceTime1 { get; set; }
        //Entrance time of the second daily opening
        public string entranceTime2 { get; set; }
        //Defines if the Event is open on Mondays
        public bool isMondayOpen { get; set; }
        //Defines if the Event is open on Saturdays
        public bool isSaturdayOpen { get; set; }
        //Defines if the Event is open on Sundays
        public bool isSundayOpen { get; set; }
        //Defines if the Event is open on Thursdays
        public bool isThursdayOpen { get; set; }
        //Defines if the Event is open on Tuesdays
        public bool isTuesdayOpen { get; set; }
        //Defines if the Event is open on Wednesdays
        public bool isWednesdayOpen { get; set; }
        //Defines if the Event is open on Fridays
        public bool isFridayOpen { get; set; }
    }

    public class LTSDay
    {
        //Unique identifier of the day of the period
        public string rid { get; set; }
        //Starting date of the day
        public string startDate { get; set; }
        //Starting time of the day
        public string startTime { get; set; }
        public LTSEventAvailability availability { get; set; }
    }

    public class LTSEventAvailability
    {
        //The ticket availability of the relative day
        public int calculatedAvailability { get; set; }
        //Defines if the availability is low for the relative day
        public bool isLowAvailability { get; set; }
        //Defines if all tickets for the relative day are sold out
        public bool isSoldOut { get; set; }

        public LTSVariantAvailability[]? variants { get; set; }
    }

    public class LTSVariantAvailability
    {
        //Unique identifier of the variant of the Event
        public string rid { get; set; }
        //The variant availability of the relative day
        public int calculatedAvailability { get; set; }
        //Defines if the availability is low for the relative day
        public bool isLowAvailability { get; set; }
    }

    public class LTSPublishersetting
    {
        //Ranking of the publisher
        public int importanceRate { get; set; }
        //Status of the Event publication by the publisher (see enums list for possible values in the schema definition) suggestedForPublication,approved,rejected
        public string publicationStatus { get; set; }  //suggestedForPublication,approved,rejected
        public LTSPublisher publisher { get; set; }
    }

    public class LTSPublisher
    {
        //Unique identifier of the publisher of the publisher settings
        public string rid { get; set; }
    }

    public class LTSEventTag
    {
        //Unique identifier of the tag of the Event
        public string rid { get; set; }
    }

    public class LTSUrl
    {
        //Type of url (see enums list for possible values in the schema definition) "pdf1","url1","video1","facebook","twitter","pdf2","pdf3","url2","url3","video2","video3","instagram","tikTok"
        public string type { get; set; }
        //Localised URL of the Event related to the type
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
        public GenericLTSRidResult snowType { get; set; }
        public LTSSnowpark snowPark { get; set; }
        public IDictionary<string, string> novelty { get; set; }
        public LTSActivityOpeningschedule[] openingSchedules { get; set; }
        public LTSActivityPoiContact contact { get; set; }
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
        public LTSActivityPoiContact contact { get; set; }
        public LTSPosition position { get; set; }
        public LTSTag[] tags { get; set; }
        public LTSImage[] images { get; set; }
        public LTSVideo[] videos { get; set; }
        //public IDictionary<string, string> novelty { get; set; }

        //Works only with object because it can contain null / [] / Dictionary<string,string>()
        public object novelty { get; set; }
        public GenericLTSRidResult[] beacons { get; set; }
    }

    #endregion

    #region Venues

    public class LTSVenue : LTSData<LTSVenueData>
    {
        public new LTSVenueData data { get; set; }
    }

    public class LTSVenueData
    {
        public string rid { get; set; }
        public GenericLTSRidResult accommodation { get; set; }
        public LTSEventContact contact { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public LTSHall[] halls { get; set; }
        public bool isActive { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSOpeningtime[] openingTimes { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public LTSPosition position { get; set; }
    }

    public class LTSHall
    {
        public LTSDescription[] descriptions { get; set; }
        public LTSHallDimension dimension { get; set; }
        public GenericLTSRidResult[] features { get; set; }
        public IDictionary<string, string> name { get; set; }
        public string placement { get; set; }
        public LTSHallPurposesofuse[] purposesOfUse { get; set; }
        public string rid { get; set; }
    }

    public class LTSHallDimension
    {
        public int? doorHeightInCentimeters { get; set; }
        public int? doorWidthInCentimeters { get; set; }
        public int? roomDepthInMeters { get; set; }
        public int? roomHeightInCentimeters { get; set; }
        public int? roomWidthInMeters { get; set; }
        public int? squareMeters { get; set; }
    }

    public class LTSHallPurposesofuse
    {
        public int maxCapacity { get; set; }
        public string type { get; set; }
    }

    #endregion

    #region Webcams

    public class LTSWebcam : LTSData<LTSWebcamData>
    {
        public new LTSWebcamData data { get; set; }
    }

    public class LTSWebcamData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public string url { get; set; }
        public string previewUrl { get; set; }
        public string streamUrl { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public bool isActive { get; set; }
        public bool hasCopyright { get; set; }
        public bool isOutOfOrder { get; set; }
        public bool isReadOnly { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSArea[] areas { get; set; }
        public LTSPosition position { get; set; }
    }

    #endregion

    #region WeatherSnow

    public class LTSWeatherSnows
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public bool isActive { get; set; }
        public bool isOutOfOrder { get; set; }
        public bool isReadOnly { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSArea[] areas { get; set; }
        public LTSWeatherSnowsConditions conditions { get; set; }
        public LTSPosition position { get; set; }
    }

    public class LTSWeatherSnowsConditions
    {
        public float? temperature { get; set; }
        public LTSSnow snow { get; set; }
        public LTSWeatherforecasts weatherForecasts { get; set; }
    }

    public class LTSSnow
    {
        public int? height { get; set; }
        public DateTime? lastEvent { get; set; }
        public int? lastEventHeight { get; set; }
    }

    public class LTSWeatherforecasts
    {
        public int? regionId { get; set; }
        public LTSWeatherForecast[] forecasts { get; set; }
    }

    public class LTSWeatherForecast
    {
        public string rid { get; set; }
        public string date { get; set; }
        public int iconId { get; set; }
        public IDictionary<string, string> description { get; set; }
    }

    #endregion

    #region SuedtirolGuestpass

    public class LTSGuestcardTypes
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public bool isActive { get; set; }
    }

    #endregion

    #region Tags

    public class LTSTags : LTSData<LTSTagsData>
    {
        public new LTSTagsData data { get; set; }
    }

    public class LTSTagsData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public string code { get; set; }
        public string entityType { get; set; }
        public int level { get; set; }
        public string mainTagRid { get; set; }
        public string parentTagRid { get; set; }
        public bool isActive { get; set; }
        public bool isSelectable { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }
        public GenericLTSRidResult[] properties { get; set; }
    }

    #endregion

    #region Categories

    public class LTSAccommodationCategoryData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        public int order { get; set; }
    }

    public class LTSAccommodationMealplanData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        public string additionalInfo { get; set; }
        public int otaCode { get; set; }
    }

    public class LTSAccommodationRateplanData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        public string alpineBitsRatePlanId { get; set; }
        public string chargeType { get; set; }
        public bool areChildrenAllowed { get; set; }
    }

    public class LTSAccommodationTypeData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        public int order { get; set; }
    }

    public class LTSAccommodationAmenityData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        public string type { get; set; }
    }

    public class LTSEventCategoryData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }

        public LTSClassification classification { get; set; }
    }

    public class LTSEventClassificationData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }

        //public IDictionary<string, string> description { get; set; }
        public string code { get; set; }
    }

    public class LTSEventTagData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }
        public bool isActive { get; set; }
        public string code { get; set; }
    }

    public class LTSGastronomyCeremonyCodeData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
    }

    public class LTSGastronomyCategoryData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
    }

    public class LTSGastronomyDishData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
    }

    public class LTSGastronomyFacilityData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        public string group { get; set; }
    }

    public class LTSGuestcardData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public bool isActive { get; set; }
    }

    public class LTSVenueCategoryData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public bool? isHotel { get; set; }
        public int? minimalHallNumber { get; set; }
        public int? minimalSurfaceInSquareMeters { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
    }

    #endregion




















}
