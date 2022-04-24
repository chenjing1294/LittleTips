using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace LittleTips.Utils
{
    public class ActiveWindow
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        /// <summary>
        /// 获取当前活动窗口的标题
        /// </summary>
        /// <returns></returns>
        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, buff, nChars) > 0)
            {
                return buff.ToString();
            }

            return null;
        }

        /// <summary>
        /// 获取当前活动窗口所属程序名
        /// </summary>
        /// <returns></returns>
        public static string GetActiveProgramName()
        {
            IntPtr hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out var procId);
            Process proc = Process.GetProcessById((int) procId);
            if (proc.MainModule != null)
                return proc.MainModule.FileName;
            else
                return string.Empty;
        }
    }
}