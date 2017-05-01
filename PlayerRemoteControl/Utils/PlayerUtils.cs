using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerRemoteControl {

    /// <summary>
    /// collection of helper methods for controlling the player
    /// </summary>
    public static class PlayerUtils {

        /// <summary>
        /// normalize given timestamp to hh:mm:ss
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static String NormalizeTimestamp(String timestamp) {
            String hh = "";
            if (timestamp.Length < 6) {
                hh += "00:";
            }
            return hh + timestamp;
        }

        /// <summary>
        /// converts a normalized timestamp to seconds
        /// </summary>
        /// <param name="timestamp">video timestamp in hh:mm:ss format</param>
        /// <returns>number of seconds, or -1 if error occured</returns>
        public static int ConvertTimestampToSeconds(String normalizedTimestamp) {

            String[] breakup = normalizedTimestamp.Split(':');

            if (breakup.Length == 3) {
                try {
                    int hh = Int32.Parse(breakup[0]) * 60 * 60;
                    int mm = Int32.Parse(breakup[1]) * 60;
                    int ss = Int32.Parse(breakup[2]);

                    return hh + mm + ss;
                } catch (Exception ex) {
                    return -1;
                }
            }

            return -1;
        }

        /// <summary>
        /// get the file name for the item
        /// </summary>
        /// <param name="fullPath">the full path of the item</param>
        /// <returns></returns>
        public static String getFileName(String fullPath) {

            if (String.IsNullOrEmpty(fullPath)) {
                return String.Empty;
            }

            int lastIndex = fullPath.LastIndexOf('\\');

            if (lastIndex <= 0) {
                return fullPath;
            }
            return fullPath.Substring(lastIndex + 1);
        }


        /// <summary>
        /// perform in-place shuffle of array using fischer-yates algo
        /// </summary>
        /// <param name="arr">the array to be shuffled</param>
        public static void shuffleArrayInPlace(object[] arr) {

            int j = arr.Length-1;
            Random rng = new Random();
            while (j > 0) {

                var temp = arr[j];
                int randomIndex = rng.Next(j);
                arr[j] = arr[randomIndex];
                arr[randomIndex] = temp;
                j--;
            }
        }
    }
}
