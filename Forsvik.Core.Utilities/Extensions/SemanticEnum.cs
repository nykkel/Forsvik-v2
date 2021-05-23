using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Forsvik.Core.Utilities.Extensions
{
    public class SemanticEnum
    {
        public Type Type { get; set; }
        public string Name { get; private set; }

        private SemanticEnum(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.PropertyType.IsEnum)
                throw new Exception("Not an Enum");

            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
        }

        public SemanticEnum(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        internal static SemanticEnum FromPropertyInfo(PropertyInfo propertyInfo)
        {
            return new SemanticEnum(propertyInfo);
        }

        public override bool Equals(object instance)
        {
            var target = instance as SemanticEnum;

            if (target == null)
                return false;

            var names = Enum.GetNames(Type);
            var targetNames = Enum.GetNames(target.Type);

            if (targetNames.Any(enumName => !names.Contains(enumName)))
                return false;

            if (names.Length != targetNames.Length)
                return false;

            var enumerator = Enum.GetValues(Type).GetEnumerator();
            var targetEnumerator = Enum.GetValues(target.Type).GetEnumerator();

            while (enumerator.MoveNext())
            {
                targetEnumerator.MoveNext();
                if ((int)enumerator.Current != (int)targetEnumerator.Current)
                    return false;
            }

            return Name.Split('.').Last().Equals(target.Name.Split('.').Last());
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Type.ToString().Split('.').Last().GetHashCode();
        }
    }
}

