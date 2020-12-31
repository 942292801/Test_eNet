using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;

namespace eNet编辑器
{
    /// <summary>
    /// TCP UDP工具类
    /// </summary>
    class ToolsUtil
    {
       
  
        /// <summary>
        /// 把IP地址某位转换成十六进制 并补0
        /// </summary>
        /// <param name="str">IP地址</param>
        /// <param name="nub">转换第几位</param>
        /// <returns></returns>
        public static string GetIPstyle(string str, int nub)
        {
            Regex reg = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
            Match match = reg.Match(str);
            string str2 = Convert.ToInt32(match.Groups[nub].Value).ToString("X2");
            return str2;
        }

        /// <summary>
        /// 获取SET;指令内容;{}对象IP地址
        /// </summary>
        /// <param name="content">发送指令内容</param>
        /// <param name="rowindex">设备端口号</param>
        /// <returns></returns>
        public static string getObjSet(string content,int rowindex,TreeNode tn)
        {

            string obj = getIP(tn) + ".0." + getID(tn) + "." + (rowindex+1).ToString();
            string msg = "SET;" + content + ";{" + obj + "};\r\n";
            return msg;
        }

        /// <summary>
        /// 获取Set;内容;{ip.type.id.port};/r/n 指令
        /// </summary>
        /// <param name="content"></param>
        /// <param name="address"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string getObjSet(string content, string address, string ip)
        {
            try
            {
                string ipLast = ip.Split('.')[3];
                string[] adds = address.Split('.');
                string type = adds[1];
                string id = adds[2];
                string port = adds[3];
                string msg = string.Format("SET;{0};{{{1}.{2}.{3}.{4}}};\r\n", content, ipLast, type, id, port);
                return msg;
            }
            catch {
                return "";
            }
        }

        /// <summary>
        /// 十进制字符串转换为16进制字符串 前面补0如 9 09 a 0a
        /// </summary>
        /// <param name="PortIP"></param>
        /// <returns></returns>
        public static string strtohexstr(string PortIP)
        {
            PortIP = Convert.ToInt32(PortIP).ToString("X2");
            return PortIP;
        }

        /// <summary>
        /// 从Treeview选中中获取IP地址的最后一位
        /// </summary>
        /// <returns>正常返回IP  否则为null</returns>
        public static  string getIP(TreeNode tn)
        {
            if (tn != null)
            {
                try
                {
                    string[] strip;
                    if (tn.Parent != null)
                    {
                        strip = tn.Parent.Text.Split(' ');
                    }
                    else
                    {
                        strip = tn.Text.Split(' ');
                    }             
                    Regex reg = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
                    Match match = reg.Match(strip[0]);
                    string ip = match.Groups[4].Value;
                    return ip; 
                }
                catch {
                    return null;
                }
               
            }
            return null;
        }

    


        /// <summary>
        /// 从Treeview选中中获取ID地址
        /// </summary>
        /// <returns>正常返回ID  否则返回null</returns>
        public static string getID(TreeNode tn)
        {
            if (tn != null)
            {
                try
                {
                    string[] strid = tn.Text.Split(' ');
                    string id = Regex.Replace(strid[0], @"[^\d]*", "");

                    //strid[0].Substring(2, strid[0].Length - 2);
                    return id;
                }
                catch
                {
                    return null;
                }
                
            }
            return null;
        }

       
        /// <summary>
        /// 把FE000000地址转换为当前IP地址的地址AA00000
        /// </summary>
        /// <param name="ip">0.0.0.0</param>
        /// <param name="nub">第几位IP</param>
        /// <param name="Address">OBJ地址 FE000101</param>
        /// <returns></returns>
        public static string getHexIP(string ip, int nub,string Address)
        {
            try
            {
                string hexIp = String.Format("{0}{1}", ToolsUtil.GetIPstyle(ip, nub), Address.Substring(2, 6));
                return hexIp;
            }
            catch {
                return "";
            }

        }

        /// <summary>
        /// 十六进制地址加一  按照类型不同分开加
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        public static string HexAddressAddOne(string add)
        {
            if (add == "" || add == null)
            {
                return "";
            }
            switch (add.Substring(2, 2))
            {
                case "00":
                    add = add.Substring(0, 6) + ToolsUtil.strtohexstr((Convert.ToInt32(add.Substring(6, 2), 16) + 1).ToString());
                    break;
                default:
                    string hexnum = ToolsUtil.strtohexstr((Convert.ToInt32(add.Substring(4, 4), 16) + 1).ToString());
                    while (hexnum.Length < 4)
                    {
                        hexnum = hexnum.Insert(0, "0");
                    }
                    add = add.Substring(0, 4) + hexnum;
                    break;
            }
            return add;
        }


        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取第一个可用的端口号
        /// </summary>
        /// <returns></returns>
        public static int GetFreePort(int min = 1024, string address = null)
        {
            int freePort = -1;
            Random random = new Random();
            int[] freePorts = GetInUsedPort(address)
                .Where(x => x >= (min = min <= 0 ? 1 : min))
                .ToArray();
            while (freePort < 0)
            {
                freePort = random.Next(min, 65536);
                foreach (var item in freePorts)
                {
                    if (freePort == item)
                        freePort = -1;
                }
            }
            return freePort;
        }

        /// <summary>
        /// 获取正在使用中的端口
        /// </summary>
        /// <param name="address">指定IP地址,默认全部地址</param>
        /// <returns></returns>
        public static int[] GetInUsedPort(string address = null)
        {
            List<IPEndPoint> localEP = new List<IPEndPoint>();
            List<int> localPort = new List<int>();
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            localEP.AddRange(ipGlobalProperties.GetActiveTcpListeners());
            localEP.AddRange(ipGlobalProperties.GetActiveUdpListeners());
            localEP.AddRange(ipGlobalProperties.GetActiveTcpConnections().Select(item => item.LocalEndPoint));
            foreach (var item in localEP.Distinct())
            {
                if (address == null || item.Address.ToString() == address)
                    localPort.Add(item.Port);
            }
            localPort.Sort();
            return localPort.Distinct().ToArray();
        }



        /// <summary>
        /// 延时毫秒
        /// </summary>
        /// <param name="milliSecond"></param>
        public static void DelayMilli(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                Application.DoEvents();//可执行某无聊的操作
            }
        }


        /// <summary>
        /// 写入日志 在运行目录下创建logs文件夹每分钟来个
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLog(string text)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = System.IO.Path.Combine(path
                , "Logs\\");

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string fileFullName = System.IO.Path.Combine(path
                , string.Format("{0}.log", DateTime.Now.ToString("yyyyMMdd-HHmm")));


                using (StreamWriter output = System.IO.File.AppendText(fileFullName))
                {
                    output.WriteLine(text);

                    output.Close();
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
           
        }

        /// <summary>
        /// 普通的对象可以克隆 有object的不行
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CloneObject(object obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter(null,
                  new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(memStream, obj);
                memStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(memStream);
            }
        }


        public static string AddBlank(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            string tmp = content.Replace(" ","");
            return Regex.Replace(tmp, @".{2}", "$0 ").Trim();

        }

        /// <summary>
        /// 计算字符串中子串出现的次数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="substring">子串</param>
        /// <returns></returns>
        public static int SubstringCount(string str, string substring)
        {
            if (str.Contains(substring))
            {
                string strReplaced = str.Replace(substring, "");
                return (str.Length - strReplaced.Length) / substring.Length;
            }
            return 0;
        }



        /// <summary>
        /// 根据设备列表添加IP地址的到ComboBox里面
        /// </summary>
        /// <param name="cb"></param>
        public static void CBGetIp(ComboBox cb)
        {
            if (FileMesege.DeviceList == null)
            {
                return;
            }
            cb.Items.Clear();
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                cb.Items.Add(d.ip);
            }
        }


        /// <summary>
        /// 判断1-9 或 1,2,3  或数字（链路类型）,再添加到ComboBox里面
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        public static void CBDealNumFormat(ComboBox cb, string info)
        {
            cb.Items.Clear();
            if (info.Contains("-"))
            {
                string[] infos = info.Split('-');
                int start = Convert.ToInt32(infos[0]);
                int end = Convert.ToInt32(infos[1]);
                if (end - start > 100)
                {
                    end = start + 100;
                }
                for (int i = start; i < end; i++)
                {
                    cb.Items.Add(i.ToString());

                }
              
            }
            else if (info.Contains(","))
            {
                string[] infos = info.Split(',');

                for (int i = 0; i < infos.Length; i++)
                {
                    cb.Items.Add(infos[i]);
                }
            }
            else
            {
                cb.Items.Add(info);

            }

        }


        /// <summary>
        /// 把IP地址排序
        /// </summary>
        /// <param name="hsList"></param>
        /// <returns></returns>
        public static List<string> IPSort(HashSet<string> hsList)
        {
            List<string> list = new List<string>(hsList);

            list.Sort(delegate (string x, string y)
            {
                string[] xip = x.Split('.');
                string[] yip = y.Split('.');
                if (xip[0] == yip[0])
                {
                    if (xip[1] == yip[1])
                    {
                        if (xip[2] == yip[2])
                        {
                            return Convert.ToInt32(xip[3]).CompareTo(Convert.ToInt32(yip[3]));
                        }
                        return Convert.ToInt32(xip[2]).CompareTo(Convert.ToInt32(yip[2]));
                    }
                    return Convert.ToInt32(xip[1]).CompareTo(Convert.ToInt32(yip[1]));
                }
                return Convert.ToInt32(xip[0]).CompareTo(Convert.ToInt32(yip[0]));
            });

            return list;
        }


        public static String ByteToString(byte[] src)
        {
            int len = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] != 0)
                {
                    len++;
                }
                else
                {
                    break;
                }

            }
            byte[] dest = new byte[len];
            if (len > 0)
            {
                Array.Copy(src, dest, len);
            }
            return Encoding.Default.GetString(dest);
        }

    }
}
