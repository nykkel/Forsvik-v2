using Forsvik.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Forsvik.Core.Utilities.Extensions
{
    public static class ReflectionExtensions
    {
        public static object SemanticCopy(this object target, object source)
        {
            // Copy plain properties
            target
                .GetType()
                .GetSemanticProperties(true)
                .Intersect(source.GetType().GetSemanticProperties())
                .Do(property => target.SetPropertyValue(property, source.GetPropertyValue(property)));

            // Copy Enums regardless of namespace
            target
                .GetType()
                .GetSemanticEnums()
                .Intersect(source.GetType().GetSemanticEnums())
                .Do(property => target.SetEnumPropertyValue(property, source.GetEnumPropertyValue(property)));

            return target;
        }     
        
        public static T SemanticCopy<T>(this T target, object source)
        {
            if (source == null) return target;

            // Copy plain properties
            target
                .GetType()
                .GetSemanticProperties(true)
                .Intersect(source.GetType().GetSemanticProperties())
                .Do(property => target.SetPropertyValue(property, source.GetPropertyValue(property)));

            // Copy Enums regardless of namespace
            target
                .GetType()
                .GetSemanticEnums()
                .Intersect(source.GetType().GetSemanticEnums())
                .Do(property => target.SetEnumPropertyValue(property, source.GetEnumPropertyValue(property)));

            return target;
        }

        internal static IEnumerable<SemanticEnum> GetSemanticEnums(this Type type)
        {
            return type.GetProperties().Where(item => item.PropertyType.IsEnum).Select(SemanticEnum.FromPropertyInfo);
        }

        public static IEnumerable<SemanticProperty> GetSemanticProperties(this Type type, bool requireSetter=false)
        {
            return type.GetProperties()
                .Where(item => !requireSetter || item.GetSetMethod() != null)
                .Select(SemanticProperty.FromPropertyInfo);
        }

        public static object GetPropertyValue(this object instance, SemanticProperty property)
        {
            return instance.GetType().GetProperty(property.Name, property.Type).GetValue(instance);
        }

        public static object GetEnumPropertyValue(this object instance, SemanticEnum property)
        {
            return instance.GetType().GetProperty(property.Name).GetValue(instance);
        }

        public static object GetPropertyValue(this object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName).GetValue(instance);
        }

        public static object SetEnumPropertyValue(this object instance, SemanticEnum property, object value)
        {
            instance.GetType().GetProperty(property.Name, property.Type).SetValue(instance, (int)value);
            return instance;
        }

        public static object SetPropertyValue(this object instance, SemanticProperty property, object value)
        {
            instance.GetType().GetProperty(property.Name, property.Type).SetValue(instance, value);
            return instance;
        }

        public static void SetValue(this PropertyInfo propertyInfo, object instance, object value)
        {
            propertyInfo.SetValue(instance, value, null);
        }

        public static object GetValue(this PropertyInfo propertyInfo, object instance)
        {
            return propertyInfo.GetValue(instance, null);
        }

    }
}
