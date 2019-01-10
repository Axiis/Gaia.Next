using System;
using System.Linq;
using System.Reflection;
using Axis.Jupiter;

using Axis.Luna.Extensions;
using Axis.Proteus.Ioc;

namespace Gaia.Data.EFCore
{
    public static class ModelTransformExporter
    {
        /// <summary>
        /// Return all model transforms in this assembly
        /// </summary>
        /// <returns></returns>
        public static ModelTransform[] ExportModelTransforms(IServiceResolver resolver)
        => Assembly
            .GetExecutingAssembly()
            .GetExportedTypes()
            .Where(type => !type.IsInterface)
            .Where(type => type.Extends(typeof(ModelTransform)))
            .Select(type => Activator.CreateInstance(type, resolver))
            .Cast<ModelTransform>()
            .ToArray();

        private static bool Extends(this Type type, Type baseType) => type.BaseTypes().Contains(baseType);
    }
}
