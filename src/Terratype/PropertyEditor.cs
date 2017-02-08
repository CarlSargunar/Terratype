﻿using ClientDependency.Core;
using System.Collections.Generic;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors;

namespace Terratype
{
    [PropertyEditor("Terratype", "Terratype", "/App_Plugins/Terratype/views/editor.html?cache=1.0.3", ValueType = PropertyEditorValueTypes.Text, Group = "Map", Icon = "icon-map-location")]
	[PropertyEditorAsset(ClientDependencyType.Javascript, "/App_Plugins/Terratype/scripts/terratype.js?cache=1.0.2")]
    public class TerratypePropertyEditor : PropertyEditor
	{
		protected override PreValueEditor CreatePreValueEditor()
		{
			return new TerratypePreValueEditor();
		}

		public TerratypePropertyEditor()
		{
            _defaultPreVals = new Dictionary<string, object>
            {
                { "definition", "{ \"config\": {\"height\": 400, \"debug\": 0, \"icon\": {\"id\":\"redmarker\"}}}" }
            };
		}

        private IDictionary<string, object> _defaultPreVals;
		public override IDictionary<string, object> DefaultPreValues
		{
			get { return _defaultPreVals; }
			set { _defaultPreVals = value; }
		}

		internal class TerratypePreValueEditor : PreValueEditor
		{
			[PreValueField("definition", "Config", "/App_Plugins/Terratype/views/config.html?cache=1.0.3", Description = "", HideLabel = true)]
            public Models.Model Definition { get; set; }

        }
	}
}
