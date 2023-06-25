using System.Reflection;

namespace RepositoryUOWDomain.Shared.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum genericEnum)
    {
        MemberInfo[] memberInfo = genericEnum.GetType().GetMember(genericEnum.ToString());
        if ((memberInfo is {Length: > 0}))
        {
            var enumAttributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (enumAttributes.Any()) return ((System.ComponentModel.DescriptionAttribute)enumAttributes.ElementAt(0)).Description;
        }
        return genericEnum.ToString();
    }
}