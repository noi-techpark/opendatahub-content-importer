// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        //Email address of the contact
        public string email { get; set; }
        //Fax number of the contact
        public string fax { get; set; }
        //Phone number of the contact
        public string phone { get; set; }
        //Type of the contact (see enums list for possible values in the schema definition) ("main","invoicing","private")
        public string type { get; set; }
        //URL of the contact website
        public string website { get; set; }
    }

    public class LTSVenueContact
    {
        public LTSAddress? address { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? website { get; set; }
    }

    public class LTSEventContact
    {
        public LTSAddress? address { get; set; }
        public IDictionary<string, string>? email { get; set; }
        public string? phone { get; set; }
        public IDictionary<string, string>? website { get; set; }
    }

    public class LTSEventOrganizerContact
    {
        public LTSAddress? address { get; set; }
        public string? email { get; set; }
        public string phone { get; set; }
        public string? website { get; set; }
        public string? fax { get; set; }
    }

    public class LTSActivityPoiContact
    {
        public LTSAddress? address { get; set; }
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

    public class LTSTourismOrganizationContact
    {
        public string type { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fax { get; set; }
        public string website { get; set; }
        public string email { get; set; }
        public LTSAddress address { get; set; }
    }

    public class LTSAddress
    {
        //Localised name of the city of the contact address
        public Dictionary<string, string>? city { get; set; }
        public Dictionary<string, string>? complement { get; set; }
        //Country code of the contact address. The API utilizes the international standards for Country codes as specified in ISO 3166-1 alpha-2
        public string country { get; set; }
        //Localised name of the Accommodation in the contact address
        public Dictionary<string, string>? name { get; set; }
        //Localised name of the owner of the Accommodation in the contact address
        public Dictionary<string, string>? name2 { get; set; }
        //Postal code of the contact address
        public string postalCode { get; set; }
        //Localised name of the street of the contact address
        public Dictionary<string, string>? street { get; set; }
    }


    public class LTSImage
    {
        //End date of image season (in mm-dd format)
        public DateTime? applicableEndDate { get; set; }
        //Start date of image season (in mm-dd format)
        public DateTime? applicableStartDate { get; set; }
        //Copyright holder of the image of the gallery
        public string copyright { get; set; }
        //Height of the image of the gallery in pixel
        public int? heightPixel { get; set; }
        //Defines if the image of the gallery is active
        public bool isActive { get; set; }
        //License of the image of the gallery (see enums list for possible values in the schema definition) ("lts","cc0")
        public string license { get; set; }
        //Sorting number of the image of the gallery
        public int? order { get; set; }
        //Unique identifier of the image of the gallery
        public string rid { get; set; }
        //URL of the image of the gallery
        public string url { get; set; }
        //Width of the image of the gallery in pixel
        public int? widthPixel { get; set; }
        //Localised name of the image
        public Dictionary<string, string>? name { get; set; }
        public bool? isMainImage { get; set; }
    }

    public class LTSPosition
    {
        //Altitude of the position in metres
        public int? altitude { get; set; }
        //represents the geographical coordinates of a position. The first value in this pair corresponds to the longitude, expressed in decimal degrees, while the second value represents the latitude, also in decimal degrees.
        public float[] coordinates { get; set; }
        //Type of the position (see enums list for possible values in the schema definition) (point)
        public string type { get; set; }
    }

    public class LTSCategory
    {
        //Unique identifier of the category
        public string rid { get; set; }
    }

    public class LTSAreamap
    {
        //X coordinate of the Accommodation on the local area map
        public string coordinateX { get; set; }
        //Y coordinate of the Accommodation on the local area map
        public string coordinateY { get; set; }
        //Number of the Accommodation on the local area map
        public string number { get; set; }
        //Color of the route to the Accommodation
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
        //Unique identifier of the Accommodation (A0RID)
        public string rid { get; set; }
        //List of unique identifier of the marketing group(s)
        public LTSMarketinggroup[] marketingGroups { get; set; }
        //Unique identifier of the address group
        public LTSAddressgroup[] addressGroups { get; set; }
        //Unique identifier of the amenity
        public LTSAmenity[] amenities { get; set; }
        public LTSAreamap areaMap { get; set; }
        //Unique identifier of the category
        public LTSCategory category { get; set; }
        public LTSAccoContact[] contacts { get; set; }
        //Localised description of the Accommodation related to the type. Type of the description (see enums list for possible values in the schema definition) (longDescription,shortDescription)
        public LTSDescription[] descriptions { get; set; }
        //Unique identifier of the district
        public LTSDistrict district { get; set; }
        public LTSGallery[] galeries { get; set; }
        //Defines if the Accommodation has apartments
        public bool hasApartments { get; set; }
        //Defines if the Accommodation has dorms
        public bool hasDorms { get; set; }
        //Defines if the Accommodation has pitches
        public bool hasPitches { get; set; }
        //Defines if the Accommodation has rooms
        public bool hasRooms { get; set; }
        //Unique identification number of the Accommodation in the HGV platform
        public string hgvId { get; set; }
        //Unique identification number of the Accommodation (A0R_ID)
        public int? id { get; set; }
        public LTSImage[] images { get; set; }
        //"Defines if the Accommodation is an accommodation
        public bool isAccommodation { get; set; }
        //Defines if the Accommodation is active
        public bool isActive { get; set; }
        //Defines if the Accommodation is bookable
        public bool isBookable { get; set; }
        //Defines if the Accommodation is a camping
        public bool isCamping { get; set; }
        //Defines if the Accommodation is member of IDM
        public bool isSuedtirolInfoActive { get; set; }
        //Defines if the Accommodation is member of an tourism organization
        public bool isTourismOrganizationMember { get; set; }
        //CIN Code of Accommodation
        public string? cinCode { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //Date and time of last change of the prices
        public DateTime lastUpdatePrices { get; set; }
        //Date and time of last availability change
        public DateTime lastUpdateAvailability { get; set; }
        public LTSMealplan[] mealPlans { get; set; }
        public LTSOverview overview { get; set; }
        public LTSPosition position { get; set; }
        //Representation mode of the Accommodation (see enums list for possible values in the schema definition) ("full","minimal","none")
        public string representationMode { get; set; }
        public LTSReview[] reviews { get; set; }
        public LTSRoomgroup[] roomGroups { get; set; }
        public LTSSeason[] seasons { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        //Rating of the Accommodation on TrustYou platform
        public int? trustyouScore { get; set; }
        public LTSSuedtirolguestpass suedtirolGuestPass { get; set; }
        public LTSAccessibility accessibility { get; set; }
    }

    public class LTSType
    {
        //Unique identifier of the Accommodation establishment type
        public string rid { get; set; }
    }

    public class LTSMarketinggroup
    {
        //List of unique identifier of the marketing group(s)
        public string rid { get; set; }
    }

    public class LTSOverview
    {
        public LTSCamping camping { get; set; }
        //End time of luggage service
        public string luggageServiceEndTime { get; set; }
        //Start time of luggage service
        public string luggageServiceStartTime { get; set; }
        public LTSParkingspaces parkingSpaces { get; set; }
        //Check-in allowed from time
        public string checkInStartTime { get; set; }
        //Check-in allowed to time
        public string checkInEndTime { get; set; }
        //Check-out allowed from time
        public string checkOutStartTime { get; set; }
        //Check-out allowed to time
        public string checkOutEndTime { get; set; }
        //Reception closing time
        public string receptionEndTime { get; set; }
        //Reception opening time
        public string receptionStartTime { get; set; }
        //End time for room service
        public string roomServiceEndTime { get; set; }
        //Start time for room service
        public string roomServiceStartTime { get; set; }
    }

    public class LTSCamping
    {
        //Maximum occupancy of the campsite
        public int? capacityPersons { get; set; }
        //Number of dishwashing spaces on the campsite
        public int? dishwashingSpaces { get; set; }
        //Number of washrooms on the campsite
        public int? laundrySpaces { get; set; }
        //Number of pitches on the campsite
        public int? pitches { get; set; }
        //Number of showers on the campsite
        public int? showers { get; set; }
        //Number of toilets on the campsite
        public int? toilets { get; set; }
    }

    public class LTSParkingspaces
    {
        //Number of garage parking spaces
        public int? garage { get; set; }
        //Number of outdoor parking spaces
        public int? outdoor { get; set; }
    }

    public class LTSTourismorganization
    {
        //Unique identifier of the tourism organization
        public string rid { get; set; }
    }

    public class LTSSuedtirolguestpass
    {
        //is accommodation active for GuestPass
        public bool isActive { get; set; }
        public LTSCardtype[] cardTypes { get; set; }
    }

    public class LTSCardtype
    {
        //Guest Pass type assigned to accommodation. Only one is allowed
        public string rid { get; set; }
    }

    public class LTSAccessibility
    {
        //URL of the website providing information about the accommodation's accessibility
        public Dictionary<string, string>? website { get; set; }
        //Localised description of the accommodation's accessibility
        public Dictionary<string, string>? description { get; set; }
    }

    public class LTSRateplan
    {
        //Type of the price of the ratePlan ("perPersonPerNight","perRoomPerNight","undefined")
        public string chargeType { get; set; }
        //AlpineBits RatePlanId
        public string alpineBitsRatePlanId { get; set; }
        //OTA code of the ratePlan
        public string code { get; set; }
        //Localised description of the ratePlan related to the type
        //Type of the description of the ratePlan (see enums list for possible values in the schema definition) longDescription,plainTextLongDescription
        public LTSDescription[] descriptions { get; set; }
        //Defines if the rate is a special offer or comes from pricelist
        public bool iusSpecialOffer { get; set; }
        //Date and time of last ratePlan change
        public DateTime lastUpdate { get; set; }
        //Localised name of the ratePlan
        public Dictionary<string, string>? name { get; set; }
        //Unique identifier of the ratePlan
        public string rid { get; set; }
        //Level of visibility of the ratePlan ("notActive","visible","visibleAndBookable","undefined")
        public string? visibility { get; set; }
    }

    public class LTSAddressgroup
    {
        //Unique identifier of the address group
        public string rid { get; set; }
    }

    public class LTSAmenity
    {
        //Unique identifier of the amenity
        //Unique identifier of the amenity of the room group
        public string rid { get; set; }
    }

    public class LTSGallery
    {
        public LTSImage[] images { get; set; }
        //Defines if the gallery is active
        public bool isActive { get; set; }
        //Sorting number of the gallery
        public int? order { get; set; }
        //Unique identifier of the gallery
        public string rid { get; set; }
    }

    public class LTSMealplan
    {
        //Unique identifier of the meal plan
        public string rid { get; set; }
    }

    public class LTSReview
    {
        //Unique identification number of the review platform
        public string id { get; set; }
        //"Defines if reviews are active
        public bool isActive { get; set; }
        //Rating of the Accommodation
        public float? rating { get; set; }
        //Number of reviews for the Accommodation
        public int? reviewsQuantity { get; set; }
        //Defines the status of reviews (see enums list for possible values in the schema definition) ("notRated","underValued","rated")
        public string status { get; set; }
        //Defines the type of review platform (see enums list for possible values in the schema definition) ("independent","trustyou")
        public string type { get; set; }
    }

    public class LTSRoomgroup
    {
        public LTSAmenity[] amenities { get; set; }
        //Number of bathrooms of the room group
        public int? baths { get; set; }
        //Classification of the room group (see enums list for possible values in the schema definition) ("room","apartment","mobileHome","holidayHome","bungalow","pitch","tent","dorm","pitchOrTent")
        public string classification { get; set; }
        //Code of the room group
        public string code { get; set; }
        //Localised description of the room group related to the type. Type of the description (see enums list for possible values in the schema definition) ("shortDescription","longDescription")
        public LTSDescription[] descriptions { get; set; }
        //Number of dining rooms
        public int? diningRooms { get; set; }
        public LTSImage[] images { get; set; }
        //Defines if the room group is active
        public bool isActive { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //Number of living rooms
        public int? livingRooms { get; set; }
        //Min Amount (price from) per person per day for this roomgroup based on standard prices
        public float? minAmountPerPersonPerDay { get; set; }
        //Min Amount (price from) per unit per day for this roomgroup based on standard prices
        public float? minAmountPerUnitPerDay { get; set; }
        //Localised name of the room group
        public Dictionary<string, string>? name { get; set; }
        public LTSOccupancy occupancy { get; set; }
        //Unique identifier of the room group
        public string rid { get; set; }
        //Number of rooms
        public int? roomQuantity { get; set; }
        public LTSRoom[] rooms { get; set; }
        //Number of sleeping rooms
        public int? sleepingRooms { get; set; }
        //Size in square metres
        public float? squareMeters { get; set; }
        //Number of toilets
        public int? toilets { get; set; }
        //Type of the room group (see enums list for possible values in the schema definition) ("apartment","pitches","restingPlaces","room","undefined")
        public string type { get; set; }
    }

    public class LTSOccupancy
    {
        //Maximum room occupancy
        public int? max { get; set; }
        //Minimum room occupancy
        public int? min { get; set; }
        //Minimum adult occupancy of the room
        public int? minAdults { get; set; }
        //Standard room occupancy
        public int? standard { get; set; }
    }

    public class LTSRoom
    {
        public LTSAvailability availability { get; set; }
        //String code of the room
        public string code { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //Unique identifier of the room
        public string rid { get; set; }
    }

    public class LTSAvailability
    {
        //Defined if the room is blocked
        public bool isBlocked { get; set; }
        //Date and time of last change of the availability
        public DateTime lastUpdate { get; set; }
        //Availability status of the room of every day
        public string status { get; set; }
        //"Date and time Date and time of the first availability status character
        public string statusStartDate { get; set; }
    }

    public class LTSSeason
    {
        //End date of the season
        public string endDate { get; set; }
        //Start date of the season
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
        public LTSCategory[]? categories { get; set; }
        public LTSClassification? classification { get; set; }
        public LTSEventContact? contact { get; set; }
        //Date and time of creation
        public DateTime? createdAt { get; set; }
        //Localised description of the Event related to the type
        public LTSDescription[]? descriptions { get; set; }
        //Unique identifier of the district array
        public LTSDistrict[]? districts { get; set; }
        public LTSImage[]? images { get; set; }
        //Defines if the Event is active
        public bool? isActive { get; set; }

        public bool? isBookable { get; set; }


        //Defines if the registration is required for participation
        public bool? isRegistrationRequired { get; set; }
        //Defines if a ticket is required for participation
        public bool? isTicketRequired { get; set; }
        //Defines if the Event is included in Suedtirol Guest Pass
        public bool? isIncludedInSuedtirolGuestPass { get; set; }

    
        public List<string>? eventLanguages { get; set; }
        //Date and time of last change
        public DateTime? lastUpdate { get; set; }
        //Localised name of the location of the Event
        public IDictionary<string, string>? location { get; set; }
        //Localised name of the meeting point of the Event
        public IDictionary<string, string>? meetingPoint { get; set; }
        //Localised name of the Event
        public IDictionary<string, string>? name { get; set; }
        public LTSOrganizer? organizer { get; set; }
        public LTSPeriod[]? periods { get; set; }
        public LTSPosition? position { get; set; }
        public LTSPublishersetting[]? publisherSettings { get; set; }
        //Localised description of the registration to the Event
        public IDictionary<string, string>? registration { get; set; }
        public LTSShopconfiguration? shopConfiguration { get; set; }
        public LTSEventTag[]? tags { get; set; }
        //Localised URL alias of the Event website
        public IDictionary<string, string>? urlAlias { get; set; }
        public LTSUrl[]? urls { get; set; }

        public LTSVariant[]? variants { get; set; }
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
        public bool? isActive { get; set; }

        public DateTime? bookableStartDate { get; set; }
        public DateTime? bookableEndDate { get; set; }
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
        public string? startTime { get; set; }
        //End time of the period
        public string? endTime { get; set; }
        //Entrance time to the Event in the period
        public string? entranceTime { get; set; }
        //Defines if the period is active
        public bool? isActive { get; set; }
        //Defines if there exist one sellable Event for each day of the period

        public bool? isBookable { get; set; }
        //Defines if there exist one sellable Event for each day of the period
        public bool? isEachDayOwnEvent { get; set; }
        //"Defines if the period is cancelled
        public bool? isCancelled { get; set; }
        //Minimum number of participants allowed for the period
        public int? minParticipants { get; set; }
        //Maximum number of participants allowed for the period
        public int? maxParticipants { get; set; }
        public LTSPeriodOpeningTime[]? openingHours { get; set; }
        public LTSTicketsale? ticketSale { get; set; }
        public LTSPeriodVariant[]? variants { get; set; }
        public LTSDay[]? days { get; set; }
    }

    public class LTSTicketsale
    {
        //Defines if the ticket sale is active for the period
        public bool? isActive { get; set; }
        //Number of minutes before the Event before which ticket sale is allowed
        public int? onlineSaleUntil { get; set; }
        //Maximum number of tickets sellable online
        public int? onlineContingent { get; set; }
    }

    public class LTSVariant
    {
        //Localised name of the variant of the Event
        public IDictionary<string, string>? name { get; set; }
        //Sorting number of the variant
        public int? order { get; set; }
        //Price of the variant in euros
        public double? price { get; set; }
        //Unique identifier of the variant of the Event
        public string rid { get; set; }
        //Unique identifier of the category of the variant
        public LTSVariantCategory? variantCategory { get; set; }

        //Type of implemented ticket validation (see enums list for possible values in the schema definition) ("none","single","multiple","inOutSingle","inOutMultiple")
        //public string? ticketValidationType { get; set; }
        //Maximum number of possible validations
        //public int? maxValidationQuantity { get; set; }

        //Validity type of the ticket ("appointment","period","days","fixed")
        //public string? ticketValidity { get; set; }
        //Number of validity days
        //public int? ticketValidityDays { get; set; }
        //Defines if the variant category is standard
        public bool? isStandard { get; set; }
        //Unique identifier of the tax rate of the Event
        public GenericLTSRidResult? taxRate { get; set; }
        //Defines if the tickets are ignored for availability calculation
        public bool? isIgnoredInAvailability { get; set; }
        //Defines if children tickets are hidden
        //public bool? isGhostTicket { get; set; }
        //Unique identifier of the combined sale of the variant
        public List<GenericLTSRidResult>? combinedSales { get; set; }
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
        public string? startTime1 { get; set; }
        //Starting time of the second daily opening
        public string? startTime2 { get; set; }
        //End time of the first daily opening
        public string? endTime1 { get; set; }
        //End time of the second daily opening
        public string? endTime2 { get; set; }
        //Entrance time of the first daily opening
        public string? entranceTime1 { get; set; }
        //Entrance time of the second daily opening
        public string? entranceTime2 { get; set; }
        //Defines if the Event is open on Mondays
        public bool? isMondayOpen { get; set; }
        //Defines if the Event is open on Saturdays
        public bool? isSaturdayOpen { get; set; }
        //Defines if the Event is open on Sundays
        public bool? isSundayOpen { get; set; }
        //Defines if the Event is open on Thursdays
        public bool? isThursdayOpen { get; set; }
        //Defines if the Event is open on Tuesdays
        public bool? isTuesdayOpen { get; set; }
        //Defines if the Event is open on Wednesdays
        public bool? isWednesdayOpen { get; set; }
        //Defines if the Event is open on Fridays
        public bool? isFridayOpen { get; set; }
    }

    public class LTSDay
    {
        //Unique identifier of the day of the period
        public string rid { get; set; }
        //Starting date of the day
        public string? startDate { get; set; }
        //Starting time of the day
        public string? startTime { get; set; }
        public LTSEventAvailability? availability { get; set; }
    }

    public class LTSEventAvailability
    {
        //The ticket availability of the relative day
        public int? calculatedAvailability { get; set; }
        //Defines if the availability is low for the relative day
        public bool? isLowAvailability { get; set; }
        //Defines if all tickets for the relative day are sold out
        public bool? isSoldOut { get; set; }

        public LTSVariantAvailability[]? variants { get; set; }
    }

    public class LTSVariantAvailability
    {
        //Unique identifier of the variant of the Event
        public string rid { get; set; }
        //The variant availability of the relative day
        public int? calculatedAvailability { get; set; }
        //Defines if the availability is low for the relative day
        public bool? isLowAvailability { get; set; }
    }

    public class LTSPublishersetting
    {
        //Ranking of the publisher
        public int? importanceRate { get; set; }
        //Status of the Event publication by the publisher (see enums list for possible values in the schema definition) suggestedForPublication,approved,rejected
        public string? publicationStatus { get; set; }  //suggestedForPublication,approved,rejected
        public LTSPublisher? publisher { get; set; }

        //public bool hasWritingPermission { get; set; }
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
        public string? type { get; set; }
        //Localised URL of the Event related to the type
        public IDictionary<string, string>? url { get; set; }
    }
    
    public class LTSEventOrganizer : LTSData<LTSEventOrganizerData>
    {
        public new LTSEventOrganizerData data { get; set; }
    }

    public class LTSEventOrganizerData
    {
        //Unique identifier of the Organizer
        public string rid { get; set; }
        //Date and time of last change
        public DateTime? lastUpdate { get; set; }
        //Defines if the Organizer is active
        public bool? isActive { get; set; }
        public LTSEventOrganizerContact? contact { get; set; }
        public IDictionary<string, string>? name { get; set; }
    }


    public class LTSVariantCategoryData
    {
        //Unique identifier of the category of the variant
        public string rid { get; set; }
        //Sorting number of the VariantCategory
        public int? order { get; set; }
        //Date and time of last change
        public DateTime? lastUpdate { get; set; }
        //Defines if the VariantCategory is active
        public bool isActive { get; set; }
        //Localised name of the VariantCategory
        public IDictionary<string, string>? name { get; set; }
        //Localised description of the VariantCategory
        public IDictionary<string, string>? description { get; set; }
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
        public int? id { get; set; }
        public DateTime lastUpdate { get; set; }
        public bool isActive { get; set; }
        //Representation mode of the Gastronomy (full, none)
        public string representationMode { get; set; }
        public LTSDistrict district { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public LTSPosition position { get; set; }
        public int? maxSeatingCapacity { get; set; }
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
        public int? widthPixel { get; set; }
        public int? heightPixel { get; set; }
        public int? order { get; set; }
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
        //The opening time of the Gastronomy in the specified days for the specified type
        public string startTime { get; set; }
        //The closing time of the Gastronomy on the specified days for the specified type
        public string endTime { get; set; }
        //Type of opening times of the Gastronomy ("general","meals","pizza","snacks")
        public string type { get; set; }
    }

    public class LTSCeremonyseatingcapacity
    {
        public LTSCeremony ceremony { get; set; }
        public int? maxSeatingCapacity { get; set; }
    }

    public class LTSCeremony
    {
        public string rid { get; set; }
    }

    public class LTSDishrate
    {
        public LTSDish dish { get; set; }
        public float? minAmount { get; set; }
        public float? maxAmount { get; set; }
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

        public string? liftType { get; set; }
        public string? liftCapacityType { get; set; }

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
        public IDictionary<string, string>? novelty { get; set; }
        public LTSActivityOpeningschedule[] openingSchedules { get; set; }
        public LTSActivityPoiContact contact { get; set; }
        public LTSGeodata geoData { get; set; }
        public LTSTag[] tags { get; set; }
        public LTSImage[] images { get; set; }
        public LTSVideo[] videos { get; set; }

        public LTSDistrict district { get; set; }
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
        public bool? isInground { get; set; }
        public bool? hasPipe { get; set; }
        public bool? hasBoarderCross { get; set; }
        public bool? hasArtificiallySnow { get; set; }
        public int? jibsNumber { get; set; }
        public LTSJumpsnumber jumpsNumber { get; set; }
        public LTSLinesnumber linesNumber { get; set; }
    }

    public class LTSJumpsnumber
    {
        public int? blu { get; set; }
        public int? red { get; set; }
        public int? black { get; set; }
    }

    public class LTSLinesnumber
    {
        public int? blu { get; set; }
        public int? red { get; set; }
        public int? black { get; set; }
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
        public int? difference { get; set; }
        public int? max { get; set; }
        public int? min { get; set; }
    }

    public class LTSDistance
    {
        public int? length { get; set; }
        public string? duration { get; set; }
        public int? sumUp { get; set; }
        public int? sumDown { get; set; }
    }

    public class LTSExposition
    {
        public string? value { get; set; }
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
        public float? filesize { get; set; }
        public string? url { get; set; }
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
        public LTSOpeningtimeActivityPoi[] openingTimes { get; set; }
    }

    public class LTSOpeningtimeActivityPoi
    {
        public string startTime { get; set; }
        public string endTime { get; set; }
        public bool isMondayOpen { get; set; }
        public bool isTuesdayOpen { get; set; }
        public bool isWednesdayOpen { get; set; }
        public bool isThursdayOpen { get; set; }
        public bool isFridayOpen { get; set; }
        public bool isSaturdayOpen { get; set; }
        public bool isSundayOpen { get; set; }
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
        public IDictionary<string, string>? novelty { get; set; }
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
        public GenericLTSRidResult category { get; set; }
        public GenericLTSRidResult district { get; set; }
        public LTSVenueContact contact { get; set; }
        public LTSDescription[] descriptions { get; set; }
        public LTSHall[] halls { get; set; }
        public bool isActive { get; set; }
        public DateTime lastUpdate { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSOpeningschedule[] openingSchedules { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        public LTSPosition position { get; set; }

        public LTSImage[] images { get; set; }

        public IDictionary<string, string> location { get; set; }
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

        public LTSImage[] images { get; set; }
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
        public int? maxCapacity { get; set; }
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
        public bool? hasCopyright { get; set; }
        public bool? isOutOfOrder { get; set; }
        public bool? isReadOnly { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSArea[] areas { get; set; }
        public LTSPosition position { get; set; }
    }

    #endregion

    #region WeatherSnow

    public class LTSWeatherSnows : LTSData<LTSWeatherSnowsData>
    {
        public new LTSWeatherSnowsData data { get; set; }
    }

    public class LTSWeatherSnowsData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }
        public LTSTourismorganization tourismOrganization { get; set; }
        //Defines if the WeatherSnow is active
        public bool isActive { get; set; }
        //Defines if the WeatherSnow is out of order
        public bool isOutOfOrder { get; set; }
        //Defines if the WeatherSnow can only be read
        public bool isReadOnly { get; set; }
        public IDictionary<string, string> name { get; set; }
        public LTSArea[] areas { get; set; }
        public LTSWeatherSnowsConditions conditions { get; set; }
        public LTSPosition position { get; set; }
    }

    public class LTSWeatherSnowsConditions
    {
        //Measured temperature in Celsius degrees
        public float? temperature { get; set; }
        public LTSSnow snow { get; set; }
        public LTSWeatherforecasts weatherForecasts { get; set; }
    }

    public class LTSSnow
    {
        //Actual snow height in centimeters
        public int? height { get; set; }
        //Date and time of last observation of snow
        public DateTime? lastEvent { get; set; }
        //Snow height of last event in centimeters
        public int? lastEventHeight { get; set; }
    }

    public class LTSWeatherforecasts
    {
        //Unique identification number of the region of the forecasts
        public int? regionId { get; set; }
        public LTSWeatherForecast[] forecasts { get; set; }
    }

    public class LTSWeatherForecast
    {
        //Unique identifier of the forecast
        public string rid { get; set; }
        //Date of the forecast
        public string date { get; set; }
        //Unique identification number of the icon of the forecast
        public int? iconId { get; set; }
        //Localised description of the forecast
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
        public int? level { get; set; }
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
        public int? order { get; set; }
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
        public int? otaCode { get; set; }
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
        public int? order { get; set; }
    }

    public class LTSAccommodationAmenityData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public IDictionary<string, string> name { get; set; }
        public IDictionary<string, string> description { get; set; }

        //public bool isActive { get; set; }
        public string code { get; set; }
        //Type of the Amenity (see enums list for possible values in the schema definition) (title, option)
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

    public class LTSVenueHallFeatureData
    {
        public string rid { get; set; }
        public DateTime lastUpdate { get; set; }

        public string code { get; set; }

        public IDictionary<string, string> name { get; set; }        
    }

    #endregion

    #region TourismOrganization

    public class LTSTourismOrganizationData
    {
        //Unique identifier of the Country
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //tourismOrganization-tourismFederation-touristOfficeMerano-touristOfficeBolzano
        public string type { get; set; }
        //String code of the TourismOrganization
        public string code { get; set; }
        //Unique identifier of the municipality
        public GenericLTSRidResult parentTourismOrganization { get; set; }

        public LTSSuedtirolguestpass suedtirolGuestPass { get; set; }

        public LTSTourismOrganizationContact[] contacts { get; set; }

        //Defines if the TourismOrganization is active
        public bool isActive { get; set; }
    }

    #endregion

    #region Municipality

    public class LTSMunicipalityData
    {
        //Unique identifier of the Municipality
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the Municipality
        public string code { get; set; }
        //Localised name of the Municipality
        public IDictionary<string, string> name { get; set; }
        public bool isActive { get; set; }
    }

    #endregion

    #region District

    public class LTSDistrictData
    {
        //Unique identifier of the District
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the Municipality
        public string code { get; set; }
        //Unique identifier of the municipality
        public GenericLTSRidResult municipality { get; set; }
        //Localised name of the Municipality
        public IDictionary<string, string> name { get; set; }
        public bool isActive { get; set; }
    }

    #endregion

    #region Countries

    public class LTSCountryData
    {
        //Unique identifier of the Country
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the Country
        public string code { get; set; }
        //Code of the country following ISO 639-2/T standards
        public string isoCode2 { get; set; }        
        //Localised name of the Municipality
        public IDictionary<string, string> name { get; set; }
        public bool isActive { get; set; }
    }

    #endregion

    #region Salutation

    public class LTSSalutiationData
    {
        //Unique identifier of the Country
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the Salutation
        public string code { get; set; }       
        public LTSSalutationNamePrefixes[] namePrefixes { get; set; }
    }

    public class LTSSalutationNamePrefixes
    {
        //Type of the prefix of the Salutation (standard, letter, address) 
        public string type { get; set; }
        //Localised name of the prefix of the Salutation related to the type
        public IDictionary<string, string> name { get; set; }
    }

    #endregion

    #region Areas

    public class LTSAreaData
    {
        //Unique identifier of the Area
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the geographic position of the Area
        public string geographicCode { get; set; }
        //Unique identifier of the category of the Area
        public GenericLTSRidResult category { get; set; }
        //String code of the Area
        public string code { get; set; }
        //Defines if the Area is active
        public bool isActive { get; set; }
        //Localised name of the Area
        public IDictionary<string, string> name { get; set; }
    }

    #endregion

    #region AreaCategory

    public class LTSAreaCategoryData
    {
        //Unique identifier of the Area
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //Localised name of the Category
        public IDictionary<string, string> name { get; set; }
    }

    #endregion

    #region VideoGenre

    public class LTSVideoGenreData
    {
        //Unique identifier of the VideoGenre
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the VideoGenre
        public string code { get; set; }
        //Localised name of the VideoGenre
        public IDictionary<string, string> name { get; set; }
    }

    #endregion

    #region TaxRate

    #endregion

    #region Amenity

    #endregion

    #region AddressGroup

    public class LTSAddressGroupData
    {
        //Unique identifier of the AddressGroup
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //String code of the AddressGroup
        public string code { get; set; }
        //Localised name of the AddressGroup
        public IDictionary<string, string> name { get; set; }
    }

    #endregion

    #region PositionCategory

    public class LTSPositionCategoryData
    {
        //Unique identifier of the Category
        public string rid { get; set; }
        //Date and time of last change
        public DateTime lastUpdate { get; set; }
        //Localised name of the Category
        public IDictionary<string, string> name { get; set; }
    }

    #endregion
















}
