using System.ComponentModel;

namespace ModelGenerator.Enum
{
    public static class Global
    {
        public static string DescriptionAttribute<T>(this T source)
        {
            var fi = source.GetType().GetField(source.ToString());
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            return source.ToString();
        }
    }
}
