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
    }
}
