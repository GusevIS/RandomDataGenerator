using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataGenerator.Reflection;

namespace DataGenerator.Sources
{
    public class EnumSource : DataSourceBase
    {
        public override bool TryMap(IMappingContext mappingContext)
        {
            var memberType = mappingContext?.MemberMapping?.MemberAccessor?.MemberType;
            if (memberType == null)
                return false;

            return memberType.GetTypeInfo().IsEnum == true;
        }

        public override object NextValue(IGenerateContext generateContext)
        {
            var values = Enum.GetValues(generateContext.MemberType);
            var index = RandomGenerator.Current.Next(values.Length);

            return values.GetValue(index);
        }
    }
}
