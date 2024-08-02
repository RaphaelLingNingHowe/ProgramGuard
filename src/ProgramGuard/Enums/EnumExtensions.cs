using System.ComponentModel;
using System.Reflection;

namespace ProgramGuard.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            // 获取字段信息
            var field = value.GetType().GetField(value.ToString());

            // 如果字段信息为 null，返回枚举值的字符串表示
            if (field == null)
                return value.ToString();

            // 获取 DescriptionAttribute
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            // 返回 Description 或默认字符串表示
            return attribute?.Description ?? value.ToString();
        }
    }
}
