using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hackland.AccessControl.Web.Extensions
{
    public static class ObjectExtensions
    {
        //todo: caching
        private static bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public static TConvert ConvertTo<TConvert>(this object entity) where TConvert : new()
        {
            var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            var convert = new TConvert();

            foreach (var entityProperty in entityProperties)
            {
                var property = entityProperty;
                var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                {
                    object value = entityProperty.GetValue(entity);
                    if (entityProperty.PropertyType == convertProperty.PropertyType)
                    {
                        convertProperty.SetValue(convert, Convert.ChangeType(value, convertProperty.PropertyType));
                    }
                    else
                    {
                        //destination is nullable, source is not, underlying type matches
                        if(IsNullable(convertProperty.PropertyType) && 
                            !IsNullable(entityProperty.PropertyType) &&
                            Nullable.GetUnderlyingType(convertProperty.PropertyType) == entityProperty.PropertyType
                        )
                        {
                            Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
                            convertProperty.SetValue(convert, safeValue);
                        }
                    }
                }
            }

            return convert;
        }
    }
}
