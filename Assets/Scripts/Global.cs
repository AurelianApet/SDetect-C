using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

public static class Global {
    public static int uid = -1;
    public static bool is_reset = true;
    public static string DOMAIN = "http://ec2-18-191-117-43.us-east-2.compute.amazonaws.com/sdetect/";
    public static string login_save_api = "loginApi.php";
    public static string signup_api = "signupApi.php";
    public static string setHistory_api = "setHistoryApi.php";
    public static string getHistory_api = "getHistoryApi.php";
    public static string delHistory_api = "DelHistoryApi.php";
    public static List<sLogo> SLogo;
    public static string google_key = "AIzaSyCcUZpErk_6r6IeLCM2dtG4pJbDHgd-yHE";
    public static string google_service_api = "https://vision.googleapis.com/v1/images:annotate?key=" + google_key;
    public static string google_host_name = "https://vision.googleapis.com";

    public static string logograb_api = "https://api.logograb.com/detect";
    public static string logograb_api_key = "";

    public static string imageContent = "";
    public static bool isProcess = false;

    public static string[] history = { };
    public static string username;
    public static string curScene;
    public static string GetBase64ContentForImage(string imagePath)
    {
        byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
        string base64String = Convert.ToBase64String(imageBytes);
        return base64String;
    }
    public static float ratio;

    public static string []plogos = new string[] {"World Wildlife Fund", "Nike Classic", "Coca-Cola", "Pepsi-Cola", "Apple", "Disney Entertainment", "Adidas", "Puma", "MTV", "McDonald`s", "Mickey", "Stussy", "Federal Express", "I Love NY", "Star Wars", "Atari", "Cartoon Network", "Subway", "Nintendo 64", "Shell", "National Basketball Association (NBA)", "BP", "Woolmark", "Paramount", "NBC", "Starbucks Coffee", "Air Jordan", "20th Century Fox", "Michelin", "Red Bull", "Lacoste", "Sony Electronics", "Parker Brothers", "Virgin", "MSN", "Hallmark", "Johnnie Walker", "MasterCard", "Oakland Athletics", "Tic Tac", "Bic", "RCA Entertainment", "Caterpillar", "Lego", "Xerox", "Swatch", "Yellow Pages", "Seven Up", "Del Monte", "Carlsberg", "Kellogg`s", "NASA", "Bacardi", "Nike", "Ralph Lauren Polo", "Scuderia Ferrari", "Corona", "Campbell`s", "Rolex", "CNN", "Le Tour de France", "Sun Microsystems", "Barbie", "Post-it", "Adobe", "Ferrari Automobilia", "IBM", "Good Year", "Sheba", "Fila", "Sega", "San Francisco 49ers", "General Electric", "Batman", "Motorola", "Hewlett-Packard", "Macromedia", "Playboy", "Chanel", "Unicef", "Vodafone", "Toyota", "Ford", "SMC Smart", "Oakley", "Der Grune Punkt", "Durex", "Texaco", "Linux", "Nestle", "Robe di Kappa", "United Parcel Service", "Land Rover", "Pirelli", "Calvin Klein", "Dialog", "SeaWorld", "Reebok", "Martini", "Walmart", "Royal Dutch Shell", "Microsoft", "Google", "Samsung", "Ford", "Honda", "Tesco", "Petro China", "Exxon Mobil", "Volkswagen", "Colgate", "Mazda", "Batman", "Subway", "Motorola", "Roxy", "Lacoste", "Verizon", "National Geographic", "Firefox", "Nissan", "BMW", "Hitachi", "Nestle", "Siemens", "Boeing", "Amazon", "Proceter & Gamble", "Hyundai", "Panasonic", "Mitsubishi", "3M", "Museum of London", "Pepsi", "Unilever", "Bentley", "MasterCard", "Dell", "Goldman Sachs", "Yamaha", "BBC", "Baskins Robbins", "Picasa", "FedEx", "Audi", "Sony VIAO", "AG Low", "Dove", "LG", "Cisco", "Toblerone", "Burger King", "Ubuntu", "Sun Microsystems", "Domino", "Hershey`s Kisses", "Carrefour", "Wikipedia", "Lodon Symphony Orchestra", "Facebook Places", "Circus of Magazines", "Eighty 20`s", "Beats", "My Fonts", "Yoga Australia", "Mister Cutts", "Twitter", "Walt Disney", "Goodwill", "Gillette", "Kolner Zoo", "Continental", "Pittsburgh Zoo", "Philadelphia Eagle", "British Heart Foundation", "British Blind Sport", "The Bronx Zoo", "San Diego Zoo", "Tostitos", "VIA Rail Canada", "Mobil", "Formula", "Galaries Lafayette", "The Guild of Food Writers", "Jack in the Box", "Bipolar", "Killed Productions", "General Electric", "Facebook", "AT&T", "LOUIS VUITTON", "Oracle", "Intel", "Wal-Mart", "Verizon", "American Express", "Mercedes-Benz", "Budweiser", "Marlboro", "SAP", "Visa", "Nescafe", "ESPN", "H&M", "L`Oreal", "Hewlett-Packard", "HSBC", "Home Depot", "Frito-Lay", "Audi", "UPS", "Gucci", "Accenture", "IKEA", "Siemens", "Wells Fargo", "Fox", "Pampers", "Ebay", "Hermes", "Starbucks", "MasterCard", "Danone", "Cartier", "J.P.Morgan", "Caterpillar", "Zara", "Kellogg`s", "Kraft", "Colgate", "Chevrolet", "Coach", "Hyundai", "Lexus", "Volkswagen", "Tomson Reuters", "Santander", "John Deere", "Chase", "Bank Of America", "Canon", "Prada", "Nissan", "Red Bull", "Philips", "Porsche", "FedEx", "Citi", "Boeing", "adidas", "chanel", "RBC", "Allianz", "Goldman Sachs", "Ralph Lauren", "Heineken", "Exxon Mobil", "Target", "Hershey`s", "LEGO", "Lancome", "KIA Motors", "Sprite", "MTV", "Estee Lauder"};
    public static string []slogos = new string[] { "worldwildlifefund", "nikeclassic", "cocacola", "pepsicola", "apple", "disneyentertainment", "adidas", "puma", "mtv", "mcdonald", "mickey", "stussy", "federalexpress", "iloveny", "starwars", "atari", "cartoonnetwork", "subway", "nintendo64", "shell", "nationalbasketballassociationnba", "bp", "woolmark", "paramount", "nbc", "starbuckscoffee", "airjordan", "20thcenturyfox", "michelin", "redbull", "lacoste", "sonyelectronics", "parkerbrothers", "virgin", "msn", "hallmark", "johnniewalker", "mastercard", "oaklandathletics", "tictac", "bic", "rcaentertainment", "caterpillar", "lego", "xerox", "swatch", "yellowpages", "sevenup", "delmonte", "carlsberg", "kellogg`s", "nasa", "bacardi", "nike", "ralphlaurenpolo", "scuderiaferrari", "corona", "campbells", "rolex", "cnn", "letourdefrance", "sunmicrosystems", "barbie", "postit", "adobe", "ferrariautomobilia", "ibm", "goodyear", "sheba", "Fila", "sega", "sanfrancisco49ers", "generalelectric", "batman", "motorola", "hewlettpackard", "macromedia", "playboy", "chanel", "unicef", "vodafone", "toyota", "ford", "smcsmart", "oakley", "dergrunepunkt", "durex", "texaco", "linux", "nestle", "robedikappa", "unitedparcelservice", "landrover", "pirelli", "calvinklein", "dialog", "seaWorld", "reebok", "martini", "walmart", "royaldutchshell", "microsoft", "google", "samsung", "ford", "honda", "tesco", "petrochina", "exxonmobil", "volkswagen", "colgate", "mazda", "batman", "subway", "motorola", "roxy", "lacoste", "verizon", "nationalgeographic", "firefox", "nissan", "bmw", "hitachi", "nestle", "siemens", "boeing", "amazon", "procetergamble", "hyundai", "panasonic", "mitsubishi", "3m", "museumoflondon", "pepsi", "unilever", "bentley", "masterCard", "dell", "goldmansachs", "yamaha", "bbc", "baskinsrobbins", "picasa", "fedEx", "audi", "sonyviao", "aglow", "dove", "lg", "cisco", "toblerone", "burgerking", "ubuntu", "sunmicrosystems", "domino", "hersheyskisses", "arrefour", "wikipedia", "lodonsymphonyorchestra", "facebookplaces", "circusofmagazines", "eighty20s", "beats", "myfonts", "yogaaustralia", "mistercutts", "twitter", "waltdisney", "goodwill", "gillette", "kolnerzoo", "continental", "pittsburghzoo", "philadelphiaeagle", "britishheartfoundation", "britishblindsport", "thebronxzoo", "sandiegozoo", "tostitos", "viarailcanada", "mobil", "formula", "galarieslafayette", "theguildoffoodwriters", "jackinthebox", "bipolar", "killedproductions", "generalelectric", "facebook", "att", "louisvuitton", "oracle", "intel", "walmart", "verizon", "americanexpress", "mercedesbenz", "budweiser", "marlboro", "sap", "visa", "nescafe", "espn", "hm", "loreal", "hewlettpackard", "hsbc", "homedepot", "fritolay", "audi", "ups", "gucci", "accenture", "ikea", "siemens", "wellsfargo", "fox", "pampers", "ebay", "hermes", "starbucks", "mastercard", "danone", "cartier", "jpmorgan", "caterpillar", "zara", "kelloggs", "kraft", "colgate", "chevrolet", "coach", "hyundai", "lexus", "volkswagen", "Tomson Reuters", "santander", "johndeere", "chase", "bankofamerica", "canon", "prada", "nissan", "redbull", "philips", "porsche", "fedEx", "citi", "boeing", "adidas", "chanel", "rbc", "allianz", "goldmansachs", "ralphlauren", "heineken", "exxonmobil", "target", "hersheys", "lego", "lancome", "kiamotors", "sprite", "mtv", "esteelauder" };
}

public class sLogo
{
    public string logo_name;
    public string logo_url;
    public logoPos logo_pos;
    public sLogo(string name, string url, float x, float y)
    {
        this.logo_name = name;
        this.logo_url = url;
        this.logo_pos = new logoPos(x, y);
    }
}

public class logoPos
{
    public float X;
    public float Y;

    public logoPos(float x, float y)
    {
        this.X = x;
        this.Y = y;
    }
}
