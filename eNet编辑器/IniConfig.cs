using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace eNet编辑器
{
    class IniConfig
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key, string def, Byte[] retVal, int size, string filePath);


        //ini文件名称
        //private static string inifilename = "Config.ini";
        //获取ini文件路径
        //private static string inifilepath = Directory.GetCurrentDirectory() + "\\";

        /// <summary>
        /// 获取信息 失败返回“”
        /// </summary>
        /// <param name="inifilename">文件路径</param>
        /// <param name="section">区域</param>
        /// <param name="key">键值</param>
        /// <returns>字符串</returns>
        public static string GetValue(string inifilename, string section, string key)
        {
            StringBuilder s = new StringBuilder(1024);
            if (GetPrivateProfileString(section, key, "", s, 1024, inifilename) == 0)
            {
                return "";
            }
            return s.ToString();
        }


        public static void SetValue(string inifilename, string section, string key, string value)
        {
            try
            {
                WritePrivateProfileString(section, key, value, inifilename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取section下所以的key值  调用这个
        /// 
        /// </summary>
        /// <param name="SectionName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> ReadKeys(string SectionName ,string filePath)
        {
            return ReadKey(SectionName, filePath);
        }

        public static List<string> ReadKey(string SectionName, string iniFilename)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(SectionName, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }

        /// <summary>
        /// 获取文件下 所有section区域
        /// </summary>
        /// <param name="iniFilename"></param>
        /// <returns></returns>
        public static List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
       
   

    }
}
