using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Gaia.Infrastructure.Newtonsoft
{
    public static class SettingsFactory
    {
        public static JsonSerializerSettings DefaultSettings => new JsonSerializerSettings
        {


            Converters = new List<JsonConverter>(new []
            {
                (JsonConverter) null
            })
        };

        private static KeyValuePair<Type, JsonSerializerSettings>[] _CustomSettingsMaps => new[]
        {

        };

        public static KeyValuePair<Type, JsonSerializerSettings>[] CustomSettingsMaps => _CustomSettingsMaps.ToArray();

    }
}
