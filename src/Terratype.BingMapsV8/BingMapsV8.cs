﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Terratype.Providers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BingMapsV8 : Models.Provider
    {
        private string UrlPath(string file, bool cache = true)
        {
            var result = "/App_Plugins/Terratype.BingMapsV8/" + file;
            if (cache)
            {
                result += "?cache=1.0.12";
            }
            return result;
        }

        [JsonProperty]
        public override string Id
        {
            get
            {
                return "Terratype.BingMapsV8";
            }
        }
        public override string Name
        {
            get
            {
                return "terratypeBingMapsV8_name";              //  Value is in language file
            }
        }

        public override string Description
        {
            get
            {
                return "terratypeBingMapsV8_description";       //  Value is in language file
            }
        }

        public override string ReferenceUrl
        {
            get
            {
                return "terratypeBingMapsV8_description";       //  Value is in language file
            }
        }

        public override bool CanSearch
        {
            get
            {
                return true;
            }
        }

        public override IDictionary<string, Type> CoordinateSystems
        {
            get
            {
                var wgs84 = new CoordinateSystems.Wgs84();

                return new Dictionary<string, Type>
                {
                    { wgs84.Id, wgs84.GetType() }
                };
            }
        }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        /// <summary>
        /// API Key from https://console.developers.google.com/apis/credentials
        /// </summary>
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }

        [JsonProperty(PropertyName = "forceHttps")]
        public bool ForceHttps { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "predefineStyling")]
        public string PredefineStyling { get; set; }

        [JsonObject(MemberSerialization.OptIn)]
        public class SearchDefinition
        {
            public enum SearchStatus { Disable = 0, Enable, Autocomplete };

            [JsonProperty(PropertyName = "enable")]
            public SearchStatus Enable { get; set; }
        }

        [JsonProperty(PropertyName = "search")]
        public SearchDefinition Search { get; set; }

        [JsonObject(MemberSerialization.OptIn)]
        public class VarietyDefinition
        {
            [JsonProperty(PropertyName = "basic")]
            public bool Basic { get; set; }

            [JsonProperty(PropertyName = "satellite")]
            public bool Satellite { get; set; }

            [JsonProperty(PropertyName = "streetView")]
            public bool StreetView { get; set; }
        }

        [JsonProperty(PropertyName = "variety")]
        public VarietyDefinition Variety { get; set; }

        [JsonObject(MemberSerialization.OptIn)]
        public class Control
        {
            [JsonProperty(PropertyName = "enable")]
            public bool Enable { get; set; }

        }

        [JsonProperty(PropertyName = "breadcrumb")]
        public Control Breadcrumb { get; set; }

        [JsonProperty(PropertyName = "dashboard")]
        public Control Dashboard { get; set; }

        [JsonProperty(PropertyName = "scale")]
        public Control Scale { get; set; }

        [JsonProperty(PropertyName = "zoomControl")]
        public Control ZoomControl { get; set; }

        [JsonObject(MemberSerialization.OptIn)]
        public class TrafficDefinition
        {
            [JsonProperty(PropertyName = "enable")]
            public bool Enable { get; set; }

            [JsonProperty(PropertyName = "legend")]
            public bool Legend { get; set; }
        }

        [JsonProperty(PropertyName = "traffic")]
        public TrafficDefinition Traffic { get; set; }

        [JsonProperty(PropertyName = "showLabels")]
        public bool ShowLabels { get; set; }

        private string BingScript(Models.Model model)
        {
            var url = new StringBuilder();

            var provider = model.Provider as BingMapsV8;

            if (provider.ForceHttps)
            {
                url.Append("https:");
            }

            url.Append(String.Equals(model.Position.Id, Terratype.CoordinateSystems.Gcj02._Id, StringComparison.InvariantCultureIgnoreCase) ?
                "//www.bing.com/" : "//www.bing.com/");

            url.Append("api/maps/mapcontrol?branch=");
            url.Append(String.IsNullOrWhiteSpace(provider.Version) ? "release" : provider.Version);
            url.Append(@"&callback=TerratypeBingMapsV8CallbackRender");
            if (!String.IsNullOrWhiteSpace(provider.Language))
            {
                url.Append(@"&mkt=");
                url.Append(provider.Language);
            }
            return url.ToString();
        }

        /// <summary>
        /// Returns the Html that renders this map
        /// </summary>
        /// <param name="model"></param>
        /// <param name="height"></param>
        /// <param name="language"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override void GetHtml(HtmlTextWriter writer, int mapId, Models.Model model, string labelId = null, int? height = null, string language = null)
        {
            const string guid = "af82089e-e9b9-4b8b-9f2a-bed92279dc6b";
            var id = nameof(Terratype) + nameof(BingMapsV8) + Guid.NewGuid().ToString();

            writer.AddAttribute("data-bingmapsv8", HttpUtility.UrlEncode(JsonConvert.SerializeObject(model), System.Text.Encoding.Default));
            writer.AddAttribute("data-map-id", "m" + mapId.ToString());
            if (labelId != null)
            {
                writer.AddAttribute("data-label-id", labelId);
            }
            writer.AddAttribute("data-id", id);
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, nameof(Terratype) + '.' + nameof(BingMapsV8));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (model.Icon != null && !HttpContext.Current.Items.Contains(guid))
            {
                HttpContext.Current.Items.Add(guid, true);
                writer.AddAttribute(HtmlTextWriterAttribute.Src, UrlPath("scripts/Terratype.BingMapsV8.Renderer.js"));
                writer.AddAttribute("defer", "");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Src, BingScript(model));
                writer.AddAttribute("defer", "");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.RenderEndTag();
            }

            writer.WriteFullBeginTag("style");

            writer.Write('#');
            writer.Write(id);
            writer.WriteLine(" .switchSlot.labelToggle {display:none;}");


            var provider = model.Provider as Terratype.Providers.BingMapsV8;
            if (provider.Variety.Basic == false || (provider.Variety.Basic == true && provider.PredefineStyling == "ordnanceSurvey"))
            {
                writer.Write('#');
                writer.Write(id);
                writer.WriteLine(" .slot.road {display:none;}");
            }
            if (provider.Variety.Basic == false || (provider.Variety.Basic == true && provider.PredefineStyling != "ordnanceSurvey"))
            {
                writer.Write('#');
                writer.Write(id);
                writer.WriteLine(" .slot.ordnanceSurvey {display:none;}");
            }
            if (provider.Variety.Satellite == false)
            {
                writer.Write('#');
                writer.Write(id);
                writer.WriteLine(" .slot.aerial {display:none;}");
            }
            if (provider.Variety.StreetView == false)
            {
                writer.Write('#');
                writer.Write(id);
                writer.WriteLine(" .slot.streetside {display:none;}");
            }
            if (provider.Variety.Basic == false && provider.Variety.Satellite == false && provider.Variety.StreetView == true)
            {
                writer.Write('#');
                writer.Write(id);
                writer.WriteLine(" .streetsideExit {display:none;}");
            }
            if (provider.Breadcrumb.Enable == false)
            {
                writer.Write('#');
                writer.Write(id);
                writer.WriteLine(" streetsideText {display:none;}");
            }
            writer.WriteEndTag("style");

            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, (height != null ? height : model.Height).ToString() + "px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, id);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}
