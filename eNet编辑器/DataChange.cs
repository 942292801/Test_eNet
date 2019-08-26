using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace eNet编辑器
{
    /// <summary>
    /// 数据操作工具
    /// </summary>
    class DataChange
    {

        //判断是否为十六进制
        public static bool IsHexadecimal(string str)
        {
            Regex reg = new Regex(@"^[0-9A-Fa-f]+$");
            return reg.IsMatch(str);
        }

        /// <summary>
        /// 十进制字符串转 16进制byte
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {

            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += "";
            //hexString = hexString.Insert(hexString.Length, "0");

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// byte转十六进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes, int l)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < l; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 十进制字符串转十六进制字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StringToHexString(string s, Encoding encode)
        {
            s = s.Replace(" ", "");
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += " " + Convert.ToString(b[i], 16);
            }
            return result;
        }

        //十进制字符串转十六进制字符串
        public static string HexStringToString(string hexStr)
        {
            return Convert.ToInt64(hexStr, 16).ToString();
        }

        /// <summary>
        /// 十六进制字符串转二进制字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexString2BinString(string hexString)
        {
            string result = string.Empty;
            foreach (char c in hexString)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                // 去掉格式串中的空格，即可去掉每个4位二进制数之间的空格，
                result += string.Format("{0:d4} ", v2);
            }
            return result;
        }

        /// <summary>
        /// 字符串 反转排序  123 变为 321
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Reversal(string input)
        {
            string result = "";
            for (int i = input.Length - 1; i >= 0; i--)
            {
                result += input[i];
            }
            return result;
        }

        /// <summary>
        /// 根据位数替换数据
        /// </summary>
        /// <param name="content">16进制数据格式00 00 00 00</param>
        /// <param name="val">10进制替换数据</param>
        /// <param name="bitSite">替换的位置</param>
        /// <returns></returns>
        public static string replaceStr(string content, string val, string bitSite)
        {
            //十进制转换为十六进制字符串
            val = Convert.ToInt32(val).ToString("X");
            //二进制内容
            string contentBin = DataChange.HexString2BinString(content.Replace(" ", "")).Replace(" ", "");
            string valBin = DataChange.HexString2BinString(val.Replace(" ", "")).Replace(" ", "");
            //反转二进制数据
            contentBin = DataChange.Reversal(contentBin);
            valBin = DataChange.Reversal(valBin);
            if (bitSite.Contains("-"))
            {
                string[] infos = bitSite.Split('-');
                int end = Convert.ToInt32(infos[1]);
                int start = Convert.ToInt32(infos[0]);
                //移除需要替换的字符
                contentBin = contentBin.Remove(start, end - start + 1);
                //补齐32bit数据
                int j = 32 - valBin.Length - contentBin.Length;
                for (int i = j; i > 0; i--)
                {
                    valBin=  valBin + "0";
                }
                //插入新字符
                contentBin = contentBin.Insert(start, valBin);


            }
            else
            {
                //移除需要替换的字符
                contentBin = contentBin.Remove(Convert.ToInt32(bitSite), 1);
                //插入新字符
                contentBin = contentBin.Insert(Convert.ToInt32(bitSite), valBin);

            }
            //再次反转二进制数值 并转换为十进制
            return Convert.ToInt32(DataChange.Reversal(contentBin), 2).ToString("X");

        }

        
        /// <summary>
        /// 产生随机5位数
        /// </summary>
        /// <returns></returns>
        public static int randomNum()
        {
            Random r = new Random(int.Parse(DateTime.Now.ToString("HHmmssfff")));
            int num = r.Next(10000, 99999);//随机生成一个5位整数
            
            return num;
           
           
        }

        /// <summary>
        /// 计算表中ID序号 (用于dgv添加按钮 增加补缺ID)
        /// </summary>
        /// <param name="hasharry"></param>
        /// <returns></returns>
        public static int polishId(HashSet<int> hasharry)
        {
            try
            {
                /*
                List<int> arry = hasharry.ToList<int>();
                arry.Sort();
                return arry[arry.Count - 1] + 1;*/
                List<int> arry = hasharry.ToList<int>();
                arry.Sort();

                if (arry.Count == 0)
                {
                    //该区域节点前面数字不存在
                    return 1;
                }
                //哈希表 不存在序号 直接返回
                for (int i = 0; i < arry.Count; i++)
                {
                    if (arry[i] != i + 1)
                    {
                        return i + 1;
                    }
                }
                return arry[arry.Count - 1] + 1;
            }
            catch
            {
                return 1;
            }
        }


    }
}
