// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType.Materials;
//
//    var material = Material.FromJson(jsonString);

namespace QuickType.Materials
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using R = Newtonsoft.Json.Required;
    using N = Newtonsoft.Json.NullValueHandling;

    public partial class MaterialJson
    {
        [J("name")] public string Name { get; set; }
        [J("description")] public string Description { get; set; }
        [J("sortorder")] public long Sortorder { get; set; }
        [J("rarity", NullValueHandling = N.Ignore)][JsonConverter(typeof(ParseStringConverter))] public long? Rarity { get; set; }
        [J("category")] public Category Category { get; set; }
        [J("materialtype")] public string Materialtype { get; set; }
        [J("source")] public string[] Source { get; set; }
        [J("images")] public Images Images { get; set; }
        [J("url", NullValueHandling = N.Ignore)] public Url Url { get; set; }
        [J("version")] public Version Version { get; set; }
        [J("dropdomain", NullValueHandling = N.Ignore)] public string Dropdomain { get; set; }
        [J("daysofweek", NullValueHandling = N.Ignore)] public Daysofweek[] Daysofweek { get; set; }
        [J("dupealias", NullValueHandling = N.Ignore)] public string Dupealias { get; set; }
    }

    public partial class Images
    {
        [J("redirect")] public Uri Redirect { get; set; }
        [J("fandom", NullValueHandling = N.Ignore)] public Uri Fandom { get; set; }
        [J("nameicon")] public string Nameicon { get; set; }
    }

    public partial class Url
    {
        [J("fandom")] public Uri Fandom { get; set; }
    }

    public enum Category { Adsorbate, AvatarMaterial, Consume, Exchange, ExpFruit, FishBait, FishRod, ItemVirtual, NoticeAddHp, WeaponExpStone, Wood };

    public enum Daysofweek { 周一, 周三, 周二, 周五, 周六, 周四, 周日 };

    public enum Version { Empty, The10, The26, The27, The28, The30, The31, The32, The33 };

    public partial class MaterialJson
    {
        public static MaterialJson[] FromJson(string json) => JsonConvert.DeserializeObject<MaterialJson[]>(json, QuickType.Materials.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MaterialJson[] self) => JsonConvert.SerializeObject(self, QuickType.Materials.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CategoryConverter.Singleton,
                DaysofweekConverter.Singleton,
                VersionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Category) || t == typeof(Category?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ADSORBATE":
                    return Category.Adsorbate;
                case "AVATAR_MATERIAL":
                    return Category.AvatarMaterial;
                case "CONSUME":
                    return Category.Consume;
                case "EXCHANGE":
                    return Category.Exchange;
                case "EXP_FRUIT":
                    return Category.ExpFruit;
                case "FISH_BAIT":
                    return Category.FishBait;
                case "FISH_ROD":
                    return Category.FishRod;
                case "ITEM_VIRTUAL":
                    return Category.ItemVirtual;
                case "NOTICE_ADD_HP":
                    return Category.NoticeAddHp;
                case "WEAPON_EXP_STONE":
                    return Category.WeaponExpStone;
                case "WOOD":
                    return Category.Wood;
            }
            throw new Exception("Cannot unmarshal type Category");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Category)untypedValue;
            switch (value)
            {
                case Category.Adsorbate:
                    serializer.Serialize(writer, "ADSORBATE");
                    return;
                case Category.AvatarMaterial:
                    serializer.Serialize(writer, "AVATAR_MATERIAL");
                    return;
                case Category.Consume:
                    serializer.Serialize(writer, "CONSUME");
                    return;
                case Category.Exchange:
                    serializer.Serialize(writer, "EXCHANGE");
                    return;
                case Category.ExpFruit:
                    serializer.Serialize(writer, "EXP_FRUIT");
                    return;
                case Category.FishBait:
                    serializer.Serialize(writer, "FISH_BAIT");
                    return;
                case Category.FishRod:
                    serializer.Serialize(writer, "FISH_ROD");
                    return;
                case Category.ItemVirtual:
                    serializer.Serialize(writer, "ITEM_VIRTUAL");
                    return;
                case Category.NoticeAddHp:
                    serializer.Serialize(writer, "NOTICE_ADD_HP");
                    return;
                case Category.WeaponExpStone:
                    serializer.Serialize(writer, "WEAPON_EXP_STONE");
                    return;
                case Category.Wood:
                    serializer.Serialize(writer, "WOOD");
                    return;
            }
            throw new Exception("Cannot marshal type Category");
        }

        public static readonly CategoryConverter Singleton = new CategoryConverter();
    }

    internal class DaysofweekConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Daysofweek) || t == typeof(Daysofweek?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "周一":
                    return Daysofweek.周一;
                case "周三":
                    return Daysofweek.周三;
                case "周二":
                    return Daysofweek.周二;
                case "周五":
                    return Daysofweek.周五;
                case "周六":
                    return Daysofweek.周六;
                case "周四":
                    return Daysofweek.周四;
                case "周日":
                    return Daysofweek.周日;
            }
            throw new Exception("Cannot unmarshal type Daysofweek");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Daysofweek)untypedValue;
            switch (value)
            {
                case Daysofweek.周一:
                    serializer.Serialize(writer, "周一");
                    return;
                case Daysofweek.周三:
                    serializer.Serialize(writer, "周三");
                    return;
                case Daysofweek.周二:
                    serializer.Serialize(writer, "周二");
                    return;
                case Daysofweek.周五:
                    serializer.Serialize(writer, "周五");
                    return;
                case Daysofweek.周六:
                    serializer.Serialize(writer, "周六");
                    return;
                case Daysofweek.周四:
                    serializer.Serialize(writer, "周四");
                    return;
                case Daysofweek.周日:
                    serializer.Serialize(writer, "周日");
                    return;
            }
            throw new Exception("Cannot marshal type Daysofweek");
        }

        public static readonly DaysofweekConverter Singleton = new DaysofweekConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class VersionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Version) || t == typeof(Version?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return Version.Empty;
                case "1.0":
                    return Version.The10;
                case "2.6":
                    return Version.The26;
                case "2.7":
                    return Version.The27;
                case "2.8":
                    return Version.The28;
                case "3.0":
                    return Version.The30;
                case "3.1":
                    return Version.The31;
                case "3.2":
                    return Version.The32;
                case "3.3":
                    return Version.The33;
            }
            throw new Exception("Cannot unmarshal type Version");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Version)untypedValue;
            switch (value)
            {
                case Version.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case Version.The10:
                    serializer.Serialize(writer, "1.0");
                    return;
                case Version.The26:
                    serializer.Serialize(writer, "2.6");
                    return;
                case Version.The27:
                    serializer.Serialize(writer, "2.7");
                    return;
                case Version.The28:
                    serializer.Serialize(writer, "2.8");
                    return;
                case Version.The30:
                    serializer.Serialize(writer, "3.0");
                    return;
                case Version.The31:
                    serializer.Serialize(writer, "3.1");
                    return;
                case Version.The32:
                    serializer.Serialize(writer, "3.2");
                    return;
                case Version.The33:
                    serializer.Serialize(writer, "3.3");
                    return;
            }
            throw new Exception("Cannot marshal type Version");
        }

        public static readonly VersionConverter Singleton = new VersionConverter();
    }
}
