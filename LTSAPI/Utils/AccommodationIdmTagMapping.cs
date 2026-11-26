// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using GenericHelper;

namespace LTSAPI.Utils
{
    /// <summary>
    /// Static mapping of the IDM Accommodation Types, Categories, Boards, Badges, Themes and SpecialFeatures (accommodationtypes table, keys used in
    /// AccoTypeId, AccoCategoryId, BoardIds, BadgeIds, ThemeIds, SpecialFeaturesIds) to the Tags with Source idm.
    /// Tag Id = {tagtype}:{english key}
    /// These Tags are valid for Accommodations of all sources (lts, discoverswiss, ...), LTS Accommodations additionally have the LTS Tags (RIDs) for Type, Category and Board.
    /// The Tags are created in the Content Api by DataModify AccommodationTypesToTags which uses this same mapping
    /// </summary>
    public static class AccommodationIdmTagMapping
    {
        //accommodationtypes table Type → Tag Type
        //Type, Category and Board use the same Tag Types as the LTS Tags, distinguished by Source (idm/lts).
        //The LTS syncs only deactivate Tags with Source lts, so the idm Tags are not affected
        public static readonly IReadOnlyDictionary<string, string> TagTypes = new Dictionary<string, string>()
        {
            { "Type", "accommodationtypes" },
            { "Category", "accommodationcategory" },
            { "Board", "accommodationmealplans" },
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
            { "Type|swisslodge", "swiss-lodge" },
            //Category
            { "Category|not categorized", "not-categorized" },
            { "Category|1star", "1-star" },
            { "Category|1flower", "1-flower" },
            { "Category|1sun", "1-sun" },
            { "Category|2stars", "2-stars" },
            { "Category|2flowers", "2-flowers" },
            { "Category|2suns", "2-suns" },
            { "Category|3stars", "3-stars" },
            { "Category|3flowers", "3-flowers" },
            { "Category|3suns", "3-suns" },
            { "Category|3sstars", "3s-stars" },
            { "Category|4stars", "4-stars" },
            { "Category|4flowers", "4-flowers" },
            { "Category|4suns", "4-suns" },
            { "Category|4sstars", "4s-stars" },
            { "Category|5stars", "5-stars" },
            { "Category|5suns", "5-suns" },
            { "Category|5flowers", "5-flowers" },
            //Board ("all boards" is only a filter pseudo entry)
            { "Board|without board", "without-board" },
            { "Board|breakfast", "breakfast" },
            { "Board|half board", "half-board" },
            { "Board|full board", "full-board" },
            { "Board|all inclusive", "all-inclusive" },
            { "Board|all boards", null },
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
        /// Returns the Tag Id for a accommodationtypes Type (Type, Category, Board, Badge, Theme, SpecialFeature) and Key, null if there is no Tag
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
        /// True if the Tag Id is a idm Accommodation Tag of this mapping ({tagtype}:{englishkey}), LTS Tags (RIDs) and all other Tags return false
        /// </summary>
        public static bool IsIdmTagId(string? tagid)
        {
            if (String.IsNullOrEmpty(tagid))
                return false;

            return TagTypes.Values.Any(tagtype => tagid.StartsWith(tagtype + ":", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Sets the idm Tags for AccoTypeId, AccoCategoryId, BoardIds, BadgeIds, ThemeIds and SpecialFeaturesIds on TagIds.
        /// Previously assigned idm Tags which are no longer valid are removed, all other TagIds are not touched
        /// </summary>
        public static void AssignTags(AccommodationV2 accommodation)
        {
            if (accommodation == null)
                return;

            var tagidstoassign = new List<string>();

            void AddTags(string type, IEnumerable<string>? keys)
            {
                if (keys == null)
                    return;

                foreach (var key in keys)
                {
                    var tagid = GetTagId(type, key);
                    if (tagid != null)
                        tagidstoassign.Add(tagid);
                }
            }

            AddTags("Type", accommodation.AccoTypeId != null ? new List<string>() { accommodation.AccoTypeId } : null);
            AddTags("Category", accommodation.AccoCategoryId != null ? new List<string>() { accommodation.AccoCategoryId } : null);
            AddTags("Board", accommodation.BoardIds);
            AddTags("Badge", accommodation.BadgeIds);
            AddTags("Theme", accommodation.ThemeIds);
            AddTags("SpecialFeature", accommodation.SpecialFeaturesIds);

            //Remove the idm Tags which are no longer valid
            if (accommodation.TagIds != null)
            {
                var toremove = accommodation.TagIds.Where(x => IsIdmTagId(x) && !tagidstoassign.Contains(x)).ToList();
                foreach (var tagid in toremove)
                    accommodation.TagIds.Remove(tagid);
            }

            if (tagidstoassign.Count == 0)
                return;

            if (accommodation.TagIds == null)
                accommodation.TagIds = new List<string>();

            accommodation.TagIds.TryAddOrUpdateOnList(tagidstoassign);
        }
    }
}
