
namespace Booski.Lib.Utilities {
    public static class DateUtilities {
        public static string GetJsDateTime(DateTime date) {
            string jsDateTimeFomat = "yyyy-MM-dd\\THH:mm:ss.000zzz";
            return date.ToString(jsDateTimeFomat);
        }
    }
}