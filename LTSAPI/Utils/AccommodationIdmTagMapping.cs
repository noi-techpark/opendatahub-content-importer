// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using GenericHelper;

namespace LTSAPI.Utils
{
    /// <summary>
    /// Static mapping of the IDM Accommodation Types, Badges, Themes and SpecialFeatures (accommodationtypes table, German keys used in
    /// AccoTypeId, BadgeIds, ThemeIds, SpecialFeaturesIds) to the Tags with Source idm.
    /// Tag Id = {tagtype}:{english key}
    /// The Tags are created in the Content Api by DataModify AccommodationTypesToTags which uses this same mapping
    /// </summary>
    public static class AccommodationIdmTagMapping
    {
        //accommodationtypes table Type → Tag Type
        //Board and Category are not converted, they are already present as LTS Tags (accommodationmealplans, accommodationcategory)
        //Type uses the same Tag Type as the LTS Accommodation Types, distinguished by Source (idm/lts).
        public static readonly IReadOnlyDictionary<string, string> TagTypes = new Dictionary<string, string>()
        {
            { "Type", "accommodationtypes" },
            { "Badge", "accommodationbadge" },
            { "Theme", "accommodationtheme" },
            { "SpecialFeature", "accommodationspecialfeature" },
        };

        //English Tag key per accommodationtypes {Type}|{Key} (case insensitive), Tag Id = {tagtype}:{englishkey}
        //null = not converted on purpose
        public static readonly IReadOnlyDictionary<string, string?> TagKeys = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            //Type
            { "Type|hotelpension", "hotel-boardinghouse" },
            { "Type|bedbreakfast", "bed-and-breakfast" },
            { "Type|farm", "farm-holiday" },
            { "Type|camping", "camping" },
            { "Type|youth", "youth-hostel" },
            { "Type|mountain", "mountain-refuge" },
            { "Type|apartment", "apartment" },
            { "Type|notdefined", "not-defined" },
            //Badge
            { "Badge|wellnesshotel", "wellness-hotel" },
            { "Badge|familienhotel", "family-hotel" },
            { "Badge|bikehotel", "bike-hotel" },
            { "Badge|bauernhof", "red-rooster-farm" },
            { "Badge|wanderhotel", "hiking-hotel" },
            { "Badge|südtirol privat", "private-rooms" },
            { "Badge|vinumhotel", "wine-hotel" },
            { "Badge|sustainabilitylevel1", "sustainability-level-1" },
            { "Badge|sustainabilitylevel2", "sustainability-level-2" },
            { "Badge|sustainabilitylevel3", "sustainability-level-3" },
            //Theme ("alle" is only a filter pseudo entry)
            { "Theme|alle", null },
            { "Theme|gourmet", "gourmet" },
            { "Theme|in der höhe", "at-altitude" },
            { "Theme|regionale wellness", "regional-wellness" },
            { "Theme|biken", "biking" },
            { "Theme|familie", "family" },
            { "Theme|wandern", "hiking" },
            { "Theme|wein", "wine" },
            { "Theme|städtisches flair", "urban" },
            { "Theme|am skigebiet", "ski-resort" },
            { "Theme|mediterran", "mediterranean" },
            { "Theme|dolomiten", "dolomites" },
            { "Theme|alpin", "alpine" },
            { "Theme|hütten und berggasthöfe", "huts-and-mountain-inns" },
            { "Theme|bäuerliche welten", "rural-life" },
            { "Theme|christkindlmarkt", "christmas-markets" },
            { "Theme|kleine betriebe", "small-and-charming" },
            //"bonus vacanze" is obsolete, no Tag is created and it is not assigned to TagIds
            //{ "Theme|bonus vacanze", "bonus-vacanze" },
            { "Theme|bonus vacanze", null },
            { "Theme|sustainability", "sustainability" },
            //SpecialFeature (guestcard already exists as Tag "guestcard" maintained by the Guestpass cardtypes import and is already assigned)
            { "SpecialFeature|gruppenfreundlich", "group-friendly" },
            { "SpecialFeature|tagung", "meeting-rooms" },
            { "SpecialFeature|schwimmbad", "swimming-pool" },
            { "SpecialFeature|sauna", "sauna" },
            { "SpecialFeature|garage", "garage" },
            { "SpecialFeature|abholservice", "pick-up-service" },
            { "SpecialFeature|wlan", "wifi" },
            { "SpecialFeature|barrierefrei", "barrier-free" },
            { "SpecialFeature|allergikerküche", "allergy-menu" },
            { "SpecialFeature|kleine haustiere", "pets-welcome" },
            { "SpecialFeature|guestcard", null },
        };

        /// <summary>
        /// Returns the Tag Id for a accommodationtypes Type (Type, Badge, Theme, SpecialFeature) and Key, null if there is no Tag
        /// </summary>
        public static string? GetTagId(string type, string key)
        {
            if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(key))
                return null;

            if (!TagTypes.TryGetValue(type, out var tagtype))
                return null;

            if (!TagKeys.TryGetValue(type + "|" + key, out var englishkey) || englishkey == null)
                return null;

            return tagtype + ":" + englishkey;
        }

        /// <summary>
        /// Adds the idm Tags for AccoTypeId, BadgeIds, ThemeIds and SpecialFeaturesIds to TagIds
        /// </summary>
        public static void AssignTags(AccommodationV2 accommodation)
        {
            if (accommodation == null)
                return;

            void AddTags(string type, IEnumerable<string>? keys)
            {
                if (keys == null)
                    return;

                foreach (var key in keys)
                {
                    var tagid = GetTagId(type, key);
                    if (tagid == null)
                        continue;

                    if (accommodation.TagIds == null)
                        accommodation.TagIds = new List<string>();

                    accommodation.TagIds.TryAddOrUpdateOnList(tagid);
                }
            }

            AddTags("Type", accommodation.AccoTypeId != null ? new List<string>() { accommodation.AccoTypeId } : null);
            AddTags("Badge", accommodation.BadgeIds);
            AddTags("Theme", accommodation.ThemeIds);
            AddTags("SpecialFeature", accommodation.SpecialFeaturesIds);
        }
    }
}
