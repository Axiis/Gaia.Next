using Axis.Luna.Common;
using Axis.Luna.Extensions;
using Axis.Pollux.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaia.Services.AccessDescriptors
{
    using Abac = Axis.Pollux.Authorization.Abac;

    public abstract class PropertyCollectionDescriptor : IDataAccessDescriptor
    {
        public IEnumerable<DataAttribute> DataDescriptors()
        {
            return this
                .GetType()
                .GetProperties()
                .Select(prop =>
                {
                    var value = prop.GetValue(this);
                    var type = prop.PropertyType.CommonType();
                    var data =
                        type == CommonDataType.CSV ? ToCsvString(value) :
                        type == CommonDataType.JsonObject ? Serialize(value) :
                        value?.ToString();

                    return new DataAttribute
                    {
                        Name = $"{Abac.Models.ResourceAttributes.DataDescriptorPrefix}.{prop.Name}",
                        Type =  type,
                        Data = data
                    };
                })
                .ToArray();
        }

        private static string ToCsvString(object obj)
        {
            return (obj as System.Collections.IEnumerable)
                .Cast<object>()
                .ToCSV()
                .UnwrapFrom("[", "]");
        }

        private static string Serialize(object obj)
        {
            return obj?.ToString(); //for now
        }
    }
}
