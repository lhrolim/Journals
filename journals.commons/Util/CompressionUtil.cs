using System.IO;
using System.IO.Compression;

namespace journals.commons.Util {
    public class CompressionUtil {


        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static byte[] Compress(byte[] plainData) {
            if (plainData == null) {
                return null;
            }
            byte[] compressesData = null;
            using (var outputStream = new MemoryStream()) {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress)) {
                    zip.Write(plainData, 0, plainData.Length);
                }
                //Dont get the MemoryStream data before the GZipStream is closed 
                //since it doesn’t yet contain complete compressed data.
                //GZipStream writes additional data including footer information when its been disposed
                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }

        public static string GetB64PartOnly(string content) {
            var indexOf = content.IndexOf(',');
            if (indexOf == -1) {
                return content;
            }
            var base64String = content.Substring(indexOf + 1);
            return base64String;
        }

        public static byte[] Decompress(byte[] zippedData) {
            if (zippedData == null) {
                return null;
            }
            byte[] decompressedData = null;
            using (var outputStream = new MemoryStream()) {
                using (var inputStream = new MemoryStream(zippedData)) {
                    using (var zip = new GZipStream(inputStream, CompressionMode.Decompress)) {
                        zip.CopyTo(outputStream);
                    }
                }
                decompressedData = outputStream.ToArray();
            }

            return decompressedData;
        }




    }
}
