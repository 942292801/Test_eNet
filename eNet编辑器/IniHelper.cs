using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eNet编辑器
{
    /// <summary>
    /// 读取ini配置的操作工具
    /// </summary>
    class IniHelper
    {
        #region 读取ini内容
        /// <summary>
        /// 根据类型的名称 返回 ini名称  例如 开关 返回 1.0_switch 否则返回空""
        /// </summary>
        /// <param name="typeName">开关</param>
        /// <returns>1.0_switch types下的文件名1.0_switch 去掉后缀</returns>
        public static string findTypesIniTypebyName(string typeName)
        {
            //获取当前设备的类型
            //方法一 最快捷方法 根据 类型名循环所有参数 来获取1.0_switch
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            if (string.IsNullOrEmpty(typeName))
            {
                return "";
            }
            //bool Flag = false;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                //DGV类型名称和Types.Iin Name 相同的类型名称
                if (type == typeName)
                {
                    return file.Name.Replace(".ini", "");
                }
            }

            return "";


        }


        /// <summary>
        /// 根据1.0_switch,开关  打开ini获取对象的name
        /// </summary>
        /// <param name="type">1.0_switch,开关  or 1.0_switch </param>
        /// <returns>开关 对应types文件下define_name的名字</returns>
        public static string findTypesIniNamebyType(string type)
        {
            try
            {
                string[] str = type.Split(',');
                string filepath = Application.StartupPath + "\\types\\" + str[0] + ".ini";
                if (File.Exists(filepath))
                {
                    return IniConfig.GetValue(filepath, "define", "name");
                }

                return "";
            }
            catch {
                return "";
            }

        }

        /// <summary>
        /// Objs文件夹里 文件名tv_ir 返回  电视（define的name）
        /// </summary>
        /// <param name="objType">tv_ir</param>
        /// <returns>电视 obj下的文件名</returns>
        public static string findObjsDefineName_ByType(string objType)
        {
            string filepath = string.Format("{0}\\objs\\{1}.ini", Application.StartupPath, objType);
            if (File.Exists(filepath))
            {
                return IniConfig.GetValue(filepath, "define", "name");
            }
            return "";
        }



        /// <summary>
        /// Objs文件夹里 例如电视（define的name） 返回 tv_ir(ini文件名称)
        /// </summary>
        /// <param name="objName">电视</param>
        /// <returns>tv_ir</returns>
        public static string findObjsFileNae_ByName(string objName)
        {
            //获取当前设备的类型
            //方法一 最快捷方法 根据 类型名循环所有参数 来获取1.0_switch
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//objs");
            string type = "";
            if (string.IsNullOrEmpty(objName))
            {
                return "";
            }
            //bool Flag = false;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                //DGV类型名称和Types.Iin Name 相同的类型名称
                if (type == objName)
                {
                    return file.Name.Replace(".ini", "");
                }
            }

            return "";
        }

        /// <summary>
        /// Objs文件夹里 例如寻找 电视.value1文件 返回value里面的name的信息
        /// </summary>
        /// <param name="objName">电视 中文文件名</param>
        /// <param name="value">value1 section名</param>
        /// <returns>红外遥控 value1下的name</returns>
        public static string findObjValueName_ByVal(string objName, string value)
        {
            //获取value1下的name
            string objType = findObjsFileNae_ByName(objName);
            string filepath = string.Format("{0}\\objs\\{1}.ini", Application.StartupPath, objType);
            string tmp = IniConfig.GetValue(filepath, value, "name");
            return tmp;
        }

        /// <summary>
        /// Objs文件夹里 返回value的type类型 参数全部为中文名
        /// </summary>
        /// <param name="objName">电视 中文文件名</param>
        /// <param name="value">value1 name名</param>
        /// <returns>红外遥控 value1下的name</returns>
        public static string findObjValueType_ByVal(string objName, string value)
        {
            //获取value1下的name
            string objType = findObjsFileNae_ByName(objName);
            value = findObjSectionValue_ByValName(objName, value);
            string filepath = string.Format("{0}\\objs\\{1}.ini", Application.StartupPath, objType);
            string tmp = IniConfig.GetValue(filepath, value, "type");
            return tmp;
        }

        /// <summary>
        /// Objs文件夹里 返回value的type类型 参数全部为英文 
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string findObjValueType_ByobjTypeValue(string objType,string value)
        {
            string filepath = string.Format("{0}\\objs\\{1}.ini", Application.StartupPath, objType);
            string tmp = IniConfig.GetValue(filepath, value, "type");
            return tmp;
        }

        /// <summary>
        /// objs文件  返回section名称 value1 -value8  参数为中文文件名 value下name名
        /// </summary>
        /// <param name="objName">中文文件名</param>
        /// <param name="valueName">value中文名</param>
        /// <returns>返回section 名称 value1 -value8</returns>
        public static string findObjSectionValue_ByValName(string objName, string valueName)
        {
            //获取value1下的name
            string objType = findObjsFileNae_ByName(objName);
            string filepath = string.Format("{0}\\objs\\{1}.ini", Application.StartupPath, objType);
            List<string> section =  IniConfig.ReadSections(filepath);
            for (int i = 0; i < section.Count; i++)
            {
                if (valueName == IniConfig.GetValue(filepath, section[i], "name"))
                {
                    return section[i];
                }
            }
            return "";
        }

        
  

        /// <summary>
        /// 根据地址address寻找 该地址的设备端口类型
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string findIniTypesByAddress(string ip, string address)
        {
            try
            {
                string addressType = address.Substring(2, 2);
                if (addressType != "00")
                {
                    switch (addressType)
                    {
                        //这个类型现在地址确认不了类型
                        case "10":
                            return"4.0_scene";
                        case "20":
                            return "5.0_time";
                        case "30":
                            //objType = "6.0_group";
                            return "";
                        default:
                            return "";
                    }
                }
                int id = Convert.ToInt32(address.Substring(4, 2), 16);
                string port = Convert.ToInt32(address.Substring(6, 2), 16).ToString();
                DataJson.Device gws = FileMesege.DeviceList.Find(gw => gw.ip == ip);
                foreach (DataJson.Module md in gws.module)
                {
                    if (md.id == id)
                    {
                        string filepath = Application.StartupPath + "\\devices\\" + md.device + ".ini";
                        return IniConfig.GetValue(filepath, "ports", port); ;
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }


        //public static 

        #endregion
    }
}
