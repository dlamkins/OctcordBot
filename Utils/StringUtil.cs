namespace OctcordBot {
    public static class StringUtil {

        public static string TruncateLength(string value, int maxLength) {
            if (string.IsNullOrEmpty(value)) return "";

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

    }
}
