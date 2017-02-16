﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

namespace Terratype.Providers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LeafletV1 : Models.Provider
    {
        [JsonProperty]
        public override string Id
        {
            get
            {
                return "Terratype.LeafletV1";
            }
        }

        public override string Name
        {
            get
            {
                return "Leaflet V1";
            }
        }

        public override string Description
        {
            get
            {
                return "terratypeLeafletV1_description";            //  Value in language file
            }
        }

        public override string ReferenceUrl
        {
            get
            {
                return "http://leafletjs.com/index.html";
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

        public override void GetHtml(HtmlTextWriter writer, int mapId, Models.Model model, string labelId = null, int? height = null, string language = null)
        {
            throw new NotImplementedException();
        }

    }
}
