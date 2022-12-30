﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType.Recipe;
//
//    var recipe = Recipe.FromJson(jsonString);

namespace QuickType.Recipe
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using R = Newtonsoft.Json.Required;
    using N = Newtonsoft.Json.NullValueHandling;

    public partial class RecipeJson
    {
        [J("name")] public string Name { get; set; }
        [J("formula")] public FormulaItem[] Formula { get; set; }
    }

    public partial class FormulaItem
    {
        [J("name")] public string Name { get; set; }
        [J("quantity")] public long Quantity { get; set; }
    }

    public partial class RecipeJson
    {
        public static RecipeJson[] FromJson(string json) => JsonConvert.DeserializeObject<RecipeJson[]>(json, QuickType.Recipe.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RecipeJson[] self) => JsonConvert.SerializeObject(self, QuickType.Recipe.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
