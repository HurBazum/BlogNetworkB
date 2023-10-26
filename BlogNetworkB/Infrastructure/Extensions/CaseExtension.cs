namespace BlogNetworkB.Infrastructure.Extensions
{
    public static class CaseExtension
    {
        public static string UpFirstLowOther(this string str)
        {
            str = str.ToLower();
            var chars = str.ToCharArray();
            chars[0] = char.ToUpper(chars[0]);
            str = string.Join("", chars);
            return str;
        }
    }
}