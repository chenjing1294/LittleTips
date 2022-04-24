using System;
using System.Management;

namespace Assistant.Utils
{
    public static class Id
    {
        public static string GetBoisId()
        {
            string id = null;
            bool find = false;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_BIOS");
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementBaseObject baseObject in collection)
                {
                    object serial = baseObject["SerialNumber"];
                    id = serial.ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        find = true;
                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return find ? id.Trim() : "null";
        }

        public static string GetCpuId()
        {
            string id = null;
            bool find = false;
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementBaseObject baseObject in collection)
                {
                    object serial = baseObject["ProcessorId"];
                    id = serial.ToString();
                    if (!string.IsNullOrEmpty(id))
                    {
                        find = true;
                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return find ? id : "null";
        }

        public static string GetMacId()
        {
            string id = null;
            bool find = false;
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementBaseObject baseObject in collection)
                {
                    if ((bool) baseObject["IPEnabled"])
                    {
                        object serial = baseObject["MacAddress"];
                        id = serial.ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            find = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return find ? id : "null";
        }

        /**
         * 获取电脑的唯一标识符=主板序号|CPU序号|MAC序号
         */
        public static string GetIdentifier()
        {
            return $"{GetBoisId()}|{GetCpuId()}|{GetMacId()}";
        }

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}