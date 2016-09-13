namespace journals.commons.Util {
    public static class StringExtensions {

        public static byte[] GetBytes(this string str) {
            if (str == null) {
                return null;
            }
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes) {
            if (bytes == null) {
                return null;
            }
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


        public static string Fmt(this string str, params object[] parameters) {
            return string.Format(str, parameters);
        }
    }
}
