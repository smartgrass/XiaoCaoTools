using System;
namespace XiaoCao
{
    public static class DateTimeUtil
    {
        /// <summary>
        /// 时间戳（毫秒）
        /// </summary>
        /// <returns></returns>
        public static long TimeStamp
        {
            get
            {
                DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return (long)(DateTime.UtcNow - startTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// 转化时间戳
        /// </summary>
        /// <param name="timeStamp">时间戳（毫秒）</param>
        /// <returns>UTC DateTime</returns>
        public static DateTime ConvertToDateTime(long timeStamp)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return startTime.AddMilliseconds(timeStamp);
        }
    }
}