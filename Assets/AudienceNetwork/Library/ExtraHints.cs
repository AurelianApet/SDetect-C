using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using AudienceNetwork.Utility;
using System.Collections.ObjectModel;

namespace AudienceNetwork
{
    [Serializable]
    public class ExtraHints
    {
        public struct Keyword
        {
            public const String ACCESSORIES = "accessories";
            public const String ART_HISTORY = "art_history";
            public const String AUTOMOTIVE = "automotive";
            public const String BEAUTY = "beauty";
            public const String BIOLOGY = "biology";
            public const String BOARD_GAMES = "board_games";
            public const String BUSINESS_SOFTWARE = "business_software";
            public const String BUYING_SELLING_HOMES = "buying_selling_homes";
            public const String CATS = "cats";
            public const String CELEBRITIES = "celebrities";
            public const String CLOTHING = "clothing";
            public const String COMIC_BOOKS = "comic_books";
            public const String DESKTOP_VIDEO = "desktop_video";
            public const String DOGS = "dogs";
            public const String EDUCATION = "education";
            public const String EMAIL = "email";
            public const String ENTERTAINMENT = "entertainment";
            public const String FAMILY_PARENTING = "family_parenting";
            public const String FASHION = "fashion";
            public const String FINE_ART = "fine_art";
            public const String FOOD_DRINK = "food_drink";
            public const String FRENCH_CUISINE = "french_cuisine";
            public const String GOVERNMENT = "government";
            public const String HEALTH_FITNESS = "health_fitness";
            public const String HOBBIES = "hobbies";
            public const String HOME_GARDEN = "home_garden";
            public const String HUMOR = "humor";
            public const String INTERNET_TECHNOLOGY = "internet_technology";
            public const String LARGE_ANIMALS = "large_animals";
            public const String LAW = "law";
            public const String LEGAL_ISSUES = "legal_issues";
            public const String LITERATURE = "literature";
            public const String MARKETING = "marketing";
            public const String MOVIES = "movies";
            public const String MUSIC = "music";
            public const String NEWS = "news";
            public const String PERSONAL_FINANCE = "personal_finance";
            public const String PETS = "pets";
            public const String PHOTOGRAPHY = "photography";
            public const String POLITICS = "politics";
            public const String REAL_ESTATE = "real_estate";
            public const String ROLEPLAYING_GAMES = "roleplaying_games";
            public const String SCIENCE = "science";
            public const String SHOPPING = "shopping";
            public const String SOCIETY = "society";
            public const String SPORTS = "sports";
            public const String TECHNOLOGY = "technology";
            public const String TELEVISION = "television";
            public const String TRAVEL = "travel";
            public const String VIDEO_COMPUTER_GAMES = "video_computer_games";
        };

        private const int KEYWORDS_MAX_COUNT = 5;

        public List<String> keywords;
        public String extraData;
        public String contentURL;

        internal AndroidJavaObject getAndroidObject()
        {
            AndroidJavaObject builderExtraHintsAndroid = new AndroidJavaObject("com.facebook.ads.ExtraHints$Builder");
            if (builderExtraHintsAndroid != null)
            {
                if (this.keywords != null)
                {
                    AndroidJavaClass androidKeywordEnum = new AndroidJavaClass("com.facebook.ads.ExtraHints$Keyword");
                    AndroidJavaObject[] androidKeywordArray = androidKeywordEnum.CallStatic<AndroidJavaObject[]>("values");
                    AndroidJavaObject list = new AndroidJavaObject("java.util.ArrayList");
                    int currentCount = 0;
                    foreach (String keyword in this.keywords)
                    {
                        if (currentCount == KEYWORDS_MAX_COUNT) {
                            break;
                        }

                        foreach (AndroidJavaObject obj in androidKeywordArray)
                        {
                            if (obj.Call<string>("toString").ToLower() == keyword)
                            {
                                list.Call<bool>("add", obj);
                                currentCount++;
                                break;
                            }
                        }
                    }

                    builderExtraHintsAndroid = builderExtraHintsAndroid
                        .Call<AndroidJavaObject>("keywords", list);

                }
                if (this.extraData != null)
                {
                    builderExtraHintsAndroid = builderExtraHintsAndroid
                        .Call<AndroidJavaObject>("extraData", this.extraData);
                }
                if (this.contentURL != null)
                {
                    builderExtraHintsAndroid = builderExtraHintsAndroid
                        .Call<AndroidJavaObject>("contentUrl", this.contentURL);
                }
            }

            return builderExtraHintsAndroid.Call<AndroidJavaObject>("build");
        }
    }
}
