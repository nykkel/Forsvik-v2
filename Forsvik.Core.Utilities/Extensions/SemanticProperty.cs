using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Forsvik.Core.Utilities.Extensions
{
    public class SemanticProperty
    {
        public Type Type { get; private set; }
        public string Name { get; private set; }

        private SemanticProperty(PropertyInfo propertyInfo)
        {
            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
        }

        public SemanticProperty(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        internal static SemanticProperty FromPropertyInfo(PropertyInfo propertyInfo)
        {
            return new SemanticProperty(propertyInfo);
        }

        public override bool Equals(object instance)
        {
            var target = instance as SemanticProperty;

            if (target == null)
                return false;

            if (Type != target.Type)
                return false;

            return Name.Equals(target.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Type.GetHashCode();
        }
    }
}
