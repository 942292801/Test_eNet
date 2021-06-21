using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace eNet编辑器
{
    /// <summary>
    /// 操作项目缓存数据的工具类
    /// </summary>
    class DataListHelper
    {
        /// <summary>
        /// 刷新左栏所有树状图  在form中调用 
        /// </summary>
        public static event Action UpdateTreeView;

        //public static event Action<string> AppTxtShow;
        //public static event Action<string> ClearTxtShow;

        #region 操作DeviceList的管理工具

        /// <summary>
        /// 新建网关
        /// </summary>
        /// <param name="infos">新IP + 设备型号 例：（192.168.1.111+空格+GW100） </param>
        public static void newGateway(string ip, string master)
        {
            try
            {
                if (FileMesege.DeviceList == null)
                {
                    FileMesege.DeviceList = new List<DataJson.Device>();
                }
                DataJson.Device dl = new DataJson.Device();
                dl.ip = ip;
                dl.master = master;
                dl.area1 = "";
                dl.area2 = "";
                dl.area3 = "";
                dl.area4 = "";
                dl.gateway = "";
                dl.mac = "";
                dl.mask = "";
                dl.name = "";
                dl.sn = "";
                dl.ver = "";
                //创建成功
                FileMesege.DeviceList.Add(dl);
                Console.WriteLine("添加网关成功：" + ip + "数量：" + FileMesege.DeviceList.Count());

                //网关排序
                GatewaySort();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
          
          
        }


        /// <summary>
        /// 修改网关 
        /// </summary>
        /// <param name="infos">新网关IP+Version + 旧IP + 旧Version</param>
        public static void changeGateway(string ip, string master, string oldip)
        {
            //Device修改IP
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == oldip)
                {
                    dev.ip = ip;
                    dev.master = master;
                    break;
                }
            }
            //Scene修改IP
            foreach (DataJson.Scene scene in FileMesege.sceneList)
            {
                if (scene.IP == oldip)
                {
                    scene.IP = ip;
                    scene.Dev = master;
                  
                    break;
                }
            }

            //timer修改IP
            foreach (DataJson.Timer timer in FileMesege.timerList)
            {
                if (timer.IP == oldip)
                {
                    timer.IP = ip;
                    timer.Dev = master;
                    
                    break;
                }
            }
            /////////////////////////////后期完整数据 //////////////////
            //panel修改IP
            foreach (DataJson.Panel panel in FileMesege.panelList)
            {
                if (panel.IP == oldip)
                {
                    panel.IP = ip;
                    panel.Dev = master;

                    break;
                }
            }
            //感应修改IP
            foreach (DataJson.Sensor sensor in FileMesege.sensorList)
            {
                if (sensor.IP == oldip)
                {
                    sensor.IP = ip;
                    sensor.Dev = master;

                    break;
                }
            }
            //逻辑修改IP
            foreach (DataJson.Logic logic in FileMesege.logicList)
            {
                if (logic.IP == oldip)
                {
                    logic.IP = ip;
                    logic.Dev = master;

                    break;
                }
            }
            /////////////////////////////后期完整数据 //////////////////
            changePointIP(ip,oldip);
            GatewaySort();
            UpdateTreeView();
        }

   

        /// <summary>
        /// 删除网关
        /// </summary>
        /// <param name="infos">新IP + 设备型号 例：（192.168.1.111+空格+GW100）</param>
        public static void delGateway(string ip, string master)
        {
            
            if (FileMesege.DeviceList == null)
            {
                return;
            }
            
           

            if (FileMesege.sceneList != null)
            {
                //Scenes删除IP
                foreach (DataJson.Scene scene in FileMesege.sceneList)
                {
                    if (scene.IP == ip)
                    {
                        FileMesege.sceneList.Remove(scene);
                        break;
                    }
                }
            }

            if (FileMesege.timerList != null)
            {
                //定时删除IP
                foreach (DataJson.Timer timer in FileMesege.timerList)
                {
                    if (timer.IP == ip)
                    {
                        FileMesege.timerList.Remove(timer);
                        break;
                    }
                }
            }

            if (FileMesege.panelList != null)
            {
                //面板删除IP
                foreach (DataJson.Panel panel in FileMesege.panelList)
                {
                    if (panel.IP == ip)
                    {
                        FileMesege.panelList.Remove(panel);
                        break;
                    }
                }
            }
            if (FileMesege.sensorList != null)
            {
                //感应删除IP
                foreach (DataJson.Sensor sensor in FileMesege.sensorList)
                {
                    if (sensor.IP == ip)
                    {
                        FileMesege.sensorList.Remove(sensor);
                        break;
                    }
                }
            }

            if (FileMesege.logicList != null)
            {
                //逻辑删除IP
                foreach (DataJson.Logic logic in FileMesege.logicList)
                {
                    if (logic.IP == ip)
                    {
                        FileMesege.logicList.Remove(logic);
                        break;
                    }
                }
            }
            //、、、、、、、、、、、、、、、、、、/后期数据完善
            delPointIP(ip);

            //循环寻找当前IP匹配数据
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == ip)
                {
                    //删除网关节点
                    FileMesege.DeviceList.Remove(dev);
                    break;
                }
            }
            //刷新所有Tree的节点
            UpdateTreeView();
            
        }

   
        /// <summary>
        /// 新建设备
        /// </summary>
        /// <param name="infos">新IP + 设备号 +设备型号 例：（192.168.1.111+空格+2 + 空格 + ET1000）</param>
        public static void newDevice(DataJson.Device dev, string id,string version)
        {
            try {
                DataJson.Module md = new DataJson.Module();
                md.id = Convert.ToInt32(id);
                md.device = version;
                md.area1 = "";
                md.area2 = "";
                md.area3 = "";
                md.area4 = "";
                md.name = "";
                md.sn = "";
                md.ver = "";
                addPortType(md.devPortList, version);
                //插入新设备信息
                dev.module.Add(md);
                //按ID号排序
                DeviceSort(dev);
                AddDevPanel(dev.ip, Convert.ToInt32(id), version);
                //刷新所有Tree的节点
                UpdateTreeView();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
           
        }

        /// <summary>
        /// 新建设备
        /// </summary>
        /// <param name="infos">新IP + 设备号 +设备型号 例：（192.168.1.111+空格+2 + 空格 + ET1000）</param>
        public static void newDevice(string ip, string id, string version)
        {
            try {
                foreach (DataJson.Device dev in FileMesege.DeviceList)
                {

                    if (dev.ip == ip)
                    {
                        DataJson.Module md = new DataJson.Module();
                        md.id = Convert.ToInt32(id);
                        md.device = version;
                        md.area1 = "";
                        md.area2 = "";
                        md.area3 = "";
                        md.area4 = "";
                        md.name = "";
                        md.sn = "";
                        md.ver = "";
                        addPortType(md.devPortList, version);
                        //插入新设备信息
                        dev.module.Add(md);
                        //按ID号排序
                        DeviceSort(dev);
                        AddDevPanel(dev.ip, Convert.ToInt32(id), version);
                        //Console.WriteLine("设备添加成功:" + id + "   " + version);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
           


        }


        /// <summary>
        /// 修改设备
        /// </summary> 
        /// <param name="infos">新IP + 新ID号 +新型号 + 旧ID号 +旧型号例：（192.168.1.111+空格+2 + 空格 + ET1000+ 空格+1 + 空格 + ETXXXXX）</param>
        public static void changeDevice(string ip, string id, string version, string oldid, string oldVersion)
        {
            //修改某IP下某ID 型号的设备信息
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == ip)
                {
                    foreach (DataJson.Module m in dev.module)
                    {
                        if (m.id.ToString() == oldid && m.device == oldVersion)
                        {

                            m.id = Convert.ToInt32(id);
                            m.device = version;
                            addPortType(m.devPortList, version);
                            //按ID号排序
                            DeviceSort(dev);
                            int tmpid = Convert.ToInt32(id);
                            int tmpOldid = Convert.ToInt32(oldid);
                            if (id != oldid && version == oldVersion)
                            {
                                ChangeDevPanel(ip, tmpid, version, tmpOldid);
                                //修改信息
                                changeSceneInfoByid(dev.ip, tmpid, tmpOldid);
                                changeTimerInfoByid(dev.ip, tmpid, tmpOldid);
                                changePanelInfoByid(dev.ip, tmpid, tmpOldid);
                                //changeSensorInfoByid(dev.ip, tmpOldid);//还需要完善
                                //changeLogicInfoByid(dev.ip, tmpOldid);//还需要完善
                            }
                            else
                            {
                                //修改型号
                                ChangeDevIDVerPanel(dev.ip, tmpid, version, tmpOldid);
                                //删除信息
                                removeSceneInfoByid(dev.ip, tmpOldid);
                                removeTimerInfoByid(dev.ip, tmpOldid);
                                removePanelInfoByid(dev.ip, tmpOldid);
                                //removeSensorInfoByid(dev.ip, tmpOldid);//还需要完善
                                //removeLogicInfoByid(dev.ip, tmpOldid);//还需要完善
                            }

                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 修改设备
        /// </summary> 
        /// <param name="infos">新IP + 新ID号 +新型号 + 旧ID号 +旧型号例：（192.168.1.111+空格+2 + 空格 + ET1000+ 空格+1 + 空格 + ETXXXXX）</param>
        public static void changeDevice(DataJson.Device dev, string id, string version, string oldid, string oldVersion)
        {
       
            foreach (DataJson.Module m in dev.module)
            {
                if (m.id.ToString() == oldid )//&& m.device == oldVersion)
                {

                    m.id = Convert.ToInt32(id);
                    m.device = version;
                    addPortType(m.devPortList, version);
                    //按ID号排序
                    DeviceSort(dev);
                    int tmpid = Convert.ToInt32(id);
                    int tmpOldid = Convert.ToInt32(oldid);
                    if (id != oldid && version == oldVersion)
                    {
                        //只修改ID号
                        ChangeDevPanel(dev.ip, tmpid, version, tmpOldid);
                        //修改信息
                        changeSceneInfoByid(dev.ip, tmpid, tmpOldid);
                        changeTimerInfoByid(dev.ip, tmpid, tmpOldid);
                        changePanelInfoByid(dev.ip, tmpid, tmpOldid);
                        //changeSensorInfoByid(dev.ip, tmpOldid);//还需要完善
                        //changeLogicInfoByid(dev.ip, tmpOldid);//还需要完善
                    }
                    else
                    {
                        //修改型号
                        ChangeDevIDVerPanel(dev.ip, tmpid, version, tmpOldid);
                        //删除信息
                        removeSceneInfoByid(dev.ip, tmpOldid);
                        removeTimerInfoByid(dev.ip, tmpOldid);
                        removePanelInfoByid(dev.ip, tmpOldid);
                        //removeSensorInfoByid(dev.ip, tmpOldid);//还需要完善
                        //removeLogicInfoByid(dev.ip, tmpOldid);//还需要完善
                    }
                    //刷新所有Tree的节点
                    UpdateTreeView();
                    break;
                }
            }
      
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        public static void delDevice(string ip, string id, string version)
        {
            //修改某IP下某ID 型号的设备信息
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == ip)
                {
                    foreach (DataJson.Module m in dev.module)
                    {
                        if (m.id.ToString() == id)
                        {
                            int tmpid = Convert.ToInt32(id);
                            dev.module.Remove(m);
                            delPointID(ip,id);
                            DelDevPanel(ip, tmpid);
                            removeSceneInfoByid(ip, tmpid);
                            removeTimerInfoByid(ip, tmpid);
                            removePanelInfoByid(ip, tmpid);
                            removeSensorInfoByid(ip, tmpid);//还需要完善
                            removeLogicInfoByid(ip, tmpid);//还需要完善
                            //刷新所有Tree的节点
                            UpdateTreeView();
                            break;
                        }
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// 添加端口的类型 
        /// </summary>
        /// <param name="portList"></param>
        /// <param name="version"></param>
        public static void addPortType( List<DataJson.DevPort> portList,string version)
        {
            string filepath = IniHelper.findDevicesDisplay(version);
            //获取全部Section下的Key
            List<string> list = IniConfig.ReadKeys("ports", filepath);
            string[] value ;
            portList.Clear();
            //循环添加行信息
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == "0")
                {
                    continue;
                }
                //获取类型版本类型版本
                value = IniConfig.GetValue(filepath, "ports", list[i]).Split(',');
                if (value.Length<2)
                {
                    break;
                }
                DataJson.DevPort portInfo = new DataJson.DevPort();
                portInfo.portID = Convert.ToInt32( list[i]);
                portInfo.portType = value[0];
                portInfo.portInterface = value[1];
                //portInfo.portContent =???
                portList.Add(portInfo);
              
            }
        }


        /// <summary>
        /// DeviceList设备网关重新按照IP排序
        /// </summary>
        public static void GatewaySort()
        {

            FileMesege.DeviceList.Sort(delegate(DataJson.Device x, DataJson.Device y)
            {
                string[] xip = x.ip.Split('.');
                string[] yip = y.ip.Split('.');
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

        }

        /// <summary>
        /// DeviceList在当前IP列 设备按照ID重新排列顺序
        /// </summary>
        /// <param name="dev"></param>
        public static void DeviceSort(DataJson.Device dev )
        {
           
            dev.module.Sort(delegate(DataJson.Module x, DataJson.Module y)
            {
                return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
            });
           

        }

        /// <summary>
        /// 在deviceList寻找某个IP下的ID的设备 否则返回空
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="strid"></param>
        /// <returns></returns>
        public static DataJson.Module findDeviceByIP_ID(string ip, int id)
        {
      
            //修改某IP下某ID 型号的设备信息
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == ip)
                {
                    foreach (DataJson.Module m in dev.module)
                    {
                        if (m.id  == id )
                        {

                            return m;
                        }
                    }
                    break;
                }
            }
            return null;
        }


        public static DataJson.DevPort findPortByModule_Port(DataJson.Module md, int port)
        {

            foreach (DataJson.DevPort dp in md.devPortList)
            {
                if (dp.portID == port)
                {
                    return dp;
                }
            }
            return null;
        }




        #endregion


        #region 操作PointList的管理工具


        /// <summary>
        /// 新建point节点
        /// </summary>
        /// <param name="address">十六进制6位数字</param>
        /// <param name="ip">IP</param>
        /// <param name="sections">四位区域位置 一\\二\\ \\ 或 一\\二\\三\\四</param>
        /// <param name="typeName">类型 TYPES文件下name的名称</param>
        public static void newPoint(string address, string ip, string[] sections, string typeName ,List<DataJson.PointInfo> list)
        {
            foreach (DataJson.PointInfo e in list)
            {
                //循环判断 NameList中是否存在该节点
                if (address == e.address && e.ip == ip)
                {
                    e.area1 = sections[0];
                    e.area2 = sections[1];
                    e.area3 = sections[2];
                    e.area4 = sections[3];
                    //存在
                    //更新NameList里面的类型信息
                    e.type = IniHelper.findTypesIniTypebyName(typeName);

                    return;

                }

            }
            //不存在 插入新信息
            DataJson.PointInfo eq = new DataJson.PointInfo();
            eq.pid = DataChange.randomNum();
            eq.area1 = sections[0];
            eq.area2 = sections[1];
            eq.area3 = sections[2];
            eq.area4 = sections[3];
            //eq.name = "";
            eq.ip = ip;
            eq.address = address;
            eq.objType = "";
            eq.value = "";
            eq.type = IniHelper.findTypesIniTypebyName(typeName);
            list.Add(eq);

        }



        /// <summary>
        /// 网关更改 修改Point点的某个IP的所有信息
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="oldIP"></param>
        public static void changePointIP(string IP,string oldIP)
        {
            string hexIP = ToolsUtil.GetIPstyle(IP, 4);
            foreach (DataJson.PointInfo p in FileMesege.PointList.equipment)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //p.address = string.Format("{0}{1}",hexIP,p.address.Substring(2,6));
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0], IP.Split('.')[3]);
                }
            }
            foreach (DataJson.PointInfo p in FileMesege.PointList.scene)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //p.address = string.Format("{0}{1}", hexIP, p.address.Substring(2, 6));
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0], IP.Split('.')[3]);
                }
            }
            foreach (DataJson.PointInfo p in FileMesege.PointList.timer)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //p.address = string.Format("{0}{1}", hexIP, p.address.Substring(2, 6));
                    //修改定时名称的后缀
                    //p.name =  p.name.Replace(Regex.Replace(p.name, @"[^\d]*", ""), "")+IP.Split('.')[3];
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0] , IP.Split('.')[3]);
                }
            }
            foreach (DataJson.PointInfo p in FileMesege.PointList.link)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //p.address = string.Format("{0}{1}", hexIP, p.address.Substring(2, 6));
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0], IP.Split('.')[3]);
                }
            }
            foreach (DataJson.PointInfo p in FileMesege.PointList.virtualport)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //address的为 254开头 254.251.3.X格式 所以不修改 
                    //p.address = string.Format("{0}{1}", hexIP, p.address.Substring(2, 6));
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0], IP.Split('.')[3]);
                }
            }
            foreach (DataJson.PointInfo p in FileMesege.PointList.logic)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //p.address = string.Format("{0}{1}", hexIP, p.address.Substring(2, 6));
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0], IP.Split('.')[3]);
                }
            }
            /*
            foreach (DataJson.PointInfo p in FileMesege.PointList.localvar)
            {
                if (p.ip == oldIP)
                {
                    p.ip = IP;
                    //address的为 254开头 254.249.X.X格式 (场景格式)所以不修改 
                    //p.address = string.Format("{0}{1}", hexIP, p.address.Substring(2, 6));
                    p.name = string.Format("{0}@{1}", p.name.Split('@')[0], IP.Split('.')[3]);
                }
            }*/

        }


        /// <summary>
        /// 设备ID更改 修改Ponin点的某个IP下的某个ID所有点信息
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="id">新ID地址</param>
        /// <param name="oldid">旧ID地址</param>
        public static void changePointID(string IP, string id, string oldid)
        {
            id = ToolsUtil.strtohexstr(id);
            oldid = ToolsUtil.strtohexstr(oldid);
            foreach (DataJson.PointInfo p in FileMesege.PointList.equipment)
            {
                if (p.ip == IP && p.address.Substring(4,2) == oldid)
                {
                    p.address =  string.Format("{0}{1}{2}", p.address.Substring(0, 4),id,p.address.Substring(6,2));
                }
            }
        }

        /// <summary>
        /// 网关删除 删除Point的某个IP的所有信息
        /// </summary>
        /// <param name="IP"></param>
        public static void delPointIP(string IP)
        {
            //设备
            DataJson.PointInfo dellist = FileMesege.PointList.equipment.Find(mach=> mach.ip.Equals(IP));
            while (dellist != null)
            {
                FileMesege.PointList.equipment.Remove(dellist);
                dellist = FileMesege.PointList.equipment.Find(mach => mach.ip == IP);
            }
            //场景
            DataJson.PointInfo dellistscene = FileMesege.PointList.scene.Find(mach => mach.ip.Equals(IP));
            while (dellistscene != null)
            {
                FileMesege.PointList.scene.Remove(dellistscene);
                dellistscene = FileMesege.PointList.scene.Find(mach => mach.ip == IP);
            }
            //定时
            DataJson.PointInfo dellisttimer = FileMesege.PointList.timer.Find(mach => mach.ip.Equals(IP));
            while (dellisttimer != null)
            {
                FileMesege.PointList.timer.Remove(dellisttimer);
                dellisttimer = FileMesege.PointList.timer.Find(mach => mach.ip == IP);
            }
            //面板 感应
            DataJson.PointInfo dellistlink = FileMesege.PointList.link.Find(mach => mach.ip.Equals(IP));
            while (dellistlink != null)
            {
                FileMesege.PointList.link.Remove(dellistlink);
                dellistlink = FileMesege.PointList.link.Find(mach => mach.ip == IP);
            }

            //虚拟端口
            DataJson.PointInfo dellistVirtualport = FileMesege.PointList.virtualport.Find(mach => mach.ip.Equals(IP));
            while (dellistVirtualport != null)
            {
                FileMesege.PointList.virtualport.Remove(dellistVirtualport);
                dellistVirtualport = FileMesege.PointList.virtualport.Find(mach => mach.ip == IP);
            }

            //逻辑
            DataJson.PointInfo dellistLogic = FileMesege.PointList.logic.Find(mach => mach.ip.Equals(IP));
            while (dellistLogic != null)
            {
                FileMesege.PointList.logic.Remove(dellistLogic);
                dellistLogic = FileMesege.PointList.logic.Find(mach => mach.ip == IP);
            }
            /*
            //内部变量
            DataJson.PointInfo dellistVar = FileMesege.PointList.localvar.Find(mach => mach.ip.Equals(IP));
            while (dellistVar != null)
            {
                FileMesege.PointList.localvar.Remove(dellistVar);
                dellistVar = FileMesege.PointList.localvar.Find(mach => mach.ip == IP);
            }*/
        }


        /// <summary>
        /// 设备删除 删除Point点的信息某个IP下某个ID所有点信息
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="oldid">旧ID地址</param>
        public static void delPointID(string IP, string id)
        {
            id = ToolsUtil.strtohexstr(id);
            List<DataJson.PointInfo> dellist = FileMesege.PointList.equipment.FindAll(mach => mach.ip == IP && mach.address.Substring(4,2) == id);
            foreach (DataJson.PointInfo p in dellist)
            {
                FileMesege.PointList.equipment.Remove(p);
            }

        }

        /// <summary>
        /// 通过区域名称  删除某一个区域的点位 包括删除场景、定时、虚拟变量、逻辑、感应的有关信息
        /// </summary>
        /// <param name="area1"></param>
        public static void DelPointByArea(string area1){
            //设备
            FileMesege.PointList.equipment.RemoveAll(point=>point.area1 == area1);
            //虚拟变量
            FileMesege.PointList.virtualport.RemoveAll(point => point.area1 == area1);
            //场景
            List<DataJson.PointInfo> dellist = FileMesege.PointList.scene.FindAll(point => point.area1 == area1);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Scene scene in FileMesege.sceneList) {
                    scene.scenes.RemoveAll(scenes => scenes.pid == p.pid);
                }
                FileMesege.PointList.scene.Remove(p);
            }
            //定时
            dellist = FileMesege.PointList.timer.FindAll(point => point.area1 == area1);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Timer timer in FileMesege.timerList)
                {
                    timer.timers.RemoveAll(timers => timers.pid == p.pid);
                }
                FileMesege.PointList.timer.Remove(p);
            }
            //感应和 面板
            dellist = FileMesege.PointList.link.FindAll(point => point.area1 == area1);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Sensor sensor in FileMesege.sensorList)
                {
                    sensor.sensors.RemoveAll(sensors => sensors.pid == p.pid);
                }
                foreach (DataJson.Panel panel in FileMesege.panelList)
                {
                    panel.panels.RemoveAll(panels => panels.pid == p.pid);
                }
                FileMesege.PointList.link.Remove(p);
            }
            //逻辑
            dellist = FileMesege.PointList.logic.FindAll(point => point.area1 == area1);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Logic logic in FileMesege.logicList)
                {
                    logic.logics.RemoveAll(logics => logics.pid == p.pid);
                }
                FileMesege.PointList.logic.Remove(p);
            }

        }

        public static void DelPointByArea(string area1, string area2)
        {
            //设备
            FileMesege.PointList.equipment.RemoveAll(point => point.area1 == area1 && point.area2 == area2);
            //虚拟变量
            FileMesege.PointList.virtualport.RemoveAll(point => point.area1 == area1 && point.area2 == area2);
            //场景
            List<DataJson.PointInfo> dellist = FileMesege.PointList.scene.FindAll(point => point.area1 == area1 && point.area2 == area2);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Scene scene in FileMesege.sceneList)
                {
                    scene.scenes.RemoveAll(scenes => scenes.pid == p.pid);
                }
                FileMesege.PointList.scene.Remove(p);
            }
            //定时
            dellist = FileMesege.PointList.timer.FindAll(point => point.area1 == area1 && point.area2 == area2);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Timer timer in FileMesege.timerList)
                {
                    timer.timers.RemoveAll(timers => timers.pid == p.pid);
                }
                FileMesege.PointList.timer.Remove(p);
            }
            //感应和面板
            dellist = FileMesege.PointList.link.FindAll(point => point.area1 == area1 && point.area2 == area2);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Sensor sensor in FileMesege.sensorList)
                {
                    sensor.sensors.RemoveAll(sensors => sensors.pid == p.pid);
                }
                foreach (DataJson.Panel panel in FileMesege.panelList)
                {
                    panel.panels.RemoveAll(panels => panels.pid == p.pid);
                }
                FileMesege.PointList.link.Remove(p);
            }
            //逻辑
            dellist = FileMesege.PointList.logic.FindAll(point => point.area1 == area1 && point.area2 == area2);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Logic logic in FileMesege.logicList)
                {
                    logic.logics.RemoveAll(logics => logics.pid == p.pid);
                }
                FileMesege.PointList.logic.Remove(p);
            }
        }

        public static void DelPointByArea(string area1, string area2, string area3)
        {
            //设备
            FileMesege.PointList.equipment.RemoveAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3);
            //虚拟变量
            FileMesege.PointList.virtualport.RemoveAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3);
            //场景
            List<DataJson.PointInfo> dellist = FileMesege.PointList.scene.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Scene scene in FileMesege.sceneList)
                {
                    scene.scenes.RemoveAll(scenes => scenes.pid == p.pid);
                }
                FileMesege.PointList.scene.Remove(p);
            }
            //定时
            dellist = FileMesege.PointList.timer.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Timer timer in FileMesege.timerList)
                {
                    timer.timers.RemoveAll(timers => timers.pid == p.pid);
                }
                FileMesege.PointList.timer.Remove(p);
            }
            //感应和面板
            dellist = FileMesege.PointList.link.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Sensor sensor in FileMesege.sensorList)
                {
                    sensor.sensors.RemoveAll(sensors => sensors.pid == p.pid);
                }
                foreach (DataJson.Panel panel in FileMesege.panelList)
                {
                    panel.panels.RemoveAll(panels => panels.pid == p.pid);
                }
                FileMesege.PointList.link.Remove(p);
            }
            //逻辑
            dellist = FileMesege.PointList.logic.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Logic logic in FileMesege.logicList)
                {
                    logic.logics.RemoveAll(logics => logics.pid == p.pid);
                }
                FileMesege.PointList.logic.Remove(p);
            }
        }

        public static void DelPointByArea(string area1, string area2, string area3, string area4)
        {
            //设备
            FileMesege.PointList.equipment.RemoveAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3 && point.area4 == area4);
            //虚拟变量
            FileMesege.PointList.virtualport.RemoveAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3 && point.area4 == area4);
            //场景
            List<DataJson.PointInfo> dellist = FileMesege.PointList.scene.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3 && point.area4 == area4);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Scene scene in FileMesege.sceneList)
                {
                    scene.scenes.RemoveAll(scenes => scenes.pid == p.pid);
                }
                FileMesege.PointList.scene.Remove(p);
            }
            //定时
            dellist = FileMesege.PointList.timer.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3 && point.area4 == area4);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Timer timer in FileMesege.timerList)
                {
                    timer.timers.RemoveAll(timers => timers.pid == p.pid);
                }
                FileMesege.PointList.timer.Remove(p);
            }
            //感应和面板
            dellist = FileMesege.PointList.link.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3 && point.area4 == area4);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Sensor sensor in FileMesege.sensorList)
                {
                    sensor.sensors.RemoveAll(sensors => sensors.pid == p.pid);
                }
                foreach (DataJson.Panel panel in FileMesege.panelList)
                {
                    panel.panels.RemoveAll(panels => panels.pid == p.pid);
                }
                FileMesege.PointList.link.Remove(p);
            }
            //逻辑
            dellist = FileMesege.PointList.logic.FindAll(point => point.area1 == area1 && point.area2 == area2 && point.area3 == area3 && point.area4 == area4);
            foreach (DataJson.PointInfo p in dellist)
            {
                foreach (DataJson.Logic logic in FileMesege.logicList)
                {
                    logic.logics.RemoveAll(logics => logics.pid == p.pid);
                }
                FileMesege.PointList.logic.Remove(p);
            }
        }


        /// <summary>
        /// 加载选中区域的point节点  返回section--name---type 参数为point表的点
        /// </summary>
        /// <param name="pointlist"></param>
        /// <returns></returns>
        public static List<string> GetPointNodeBySectionName(List<DataJson.PointInfo> pointlist)
        {
            List<string> infoList = new List<string>();
            if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy))
            {
                foreach (DataJson.PointInfo eq in pointlist)
                {
                    infoList.Add(dealSection(eq));
                }
                return infoList;
            }
            
           
            //区域
            string[] sections = FileMesege.sectionNodeCopy.Split('\\');
            if (sections[0] == "查看所有区域")
            {
                foreach (DataJson.PointInfo eq in pointlist)
                {
                    infoList.Add(dealSection(eq));
                }
            }
            else
            {
                foreach (DataJson.PointInfo eq in pointlist)
                {

                    // 1 2 3 4 用于区分点击区域 区分加载搜索信息 例如东区 把东区所有信息加载进来 
                    if (string.IsNullOrEmpty(sections[1]) && eq.area1 == sections[0])
                    {
                        //添加信息
                        infoList.Add(dealSection(eq));
                    }
                    // 1 2 3 0
                    else if (string.IsNullOrEmpty(sections[2]) && eq.area1 == sections[0] && eq.area2 == sections[1])
                    {
                        //添加信息
                        infoList.Add(dealSection(eq));
                    }
                    // 1 2 0 0
                    else if (string.IsNullOrEmpty(sections[3]) && eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2])
                    {
                        //添加信息
                        infoList.Add(dealSection(eq));
                    }
                    //1 0 0 0
                    else if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3])
                    {
                        //添加信息
                        infoList.Add(dealSection(eq));
                    }

                }
            }
            

            return infoList;
        }

        //添加 section --- name---type 格式 或者section --- name
        public static string dealSection(DataJson.PointInfo eq)
        {
            string tmp = eq.area1;
            if (!string.IsNullOrEmpty(eq.area2))
            {
                 tmp = string.Format("{0}\\{1}", tmp, eq.area2);
                if (!string.IsNullOrEmpty(eq.area3))
                {
                    tmp = string.Format("{0}\\{1}", tmp, eq.area3);
                    if (!string.IsNullOrEmpty(eq.area4))
                    {
                        tmp = string.Format("{0}\\{1}", tmp, eq.area4);
                    }
                }
            }
            string type = IniHelper.findTypesIniNamebyType(eq.type);
            if (string.IsNullOrEmpty(type))
            {
                return string.Format("{0}---{1}", tmp, eq.name);
            }
            return string.Format("{0}---{1}---{2}", tmp, eq.name, type);
        }

        /// <summary>
        /// 处理title点位信息section---name---type或者section --- name  获取area1 area12 area3 area4 name  type（可以无）
        /// </summary>
        /// <returns></returns>
        public static List<string> dealPointInfo(string section_name)
        {
            List<string> infoList = new List<string>();
            if (string.IsNullOrEmpty(section_name))
            {
                return infoList;
            }
            string[] modes = section_name.Replace("---", "?").Split('?');
            string[] section = modes[0].Split('\\');
            infoList = section.ToList();
            while (infoList.Count < 4)
            {
                infoList.Add("");
            }
            infoList.Add(modes[1]);
            if (modes.Length > 2)
            {
                //当type不为空的时候
                infoList.Add(modes[2]);
            }
            return infoList;
        }

        /// <summary>
        /// 根据1.0_switch类型 和 address 获取该点位 返回DataJson.PointInfo 否则 null (type可以为空)
        /// </summary>
        /// <param name="type">type为空则搜索全部list表的</param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static DataJson.PointInfo findPointByType_address(string type, string address,string ip)
        {
            try
            {
                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(ip))
                {
                    return null;
                }
                string tmp = address.Substring(0, 2);
                if (address != "FFFFFFFF" && tmp != "FE")
                {
                    //XXFFFFFF 把XX代替 192.168.1.XX
                    address = "FE" + address.Substring(2, 6);
                    string[] ips = ip.Split('.');
                    ip = string.Format("{0}.{1}.{2}.{3}", ips[0], ips[1], ips[2],Convert.ToInt32(tmp, 16));
                     
                }
                //区域加名称
                DataJson.PointInfo pointInfo = null;
                //如果type为空就搜索全表
                if (string.IsNullOrEmpty(type))
                {
                    if (FileMesege.PointList.equipment != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.equipment, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }
                    if (FileMesege.PointList.timer != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.timer, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }
                    if (FileMesege.PointList.scene != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.scene, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }
                    if (FileMesege.PointList.link != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.link, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }
                    if (FileMesege.PointList.virtualport != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.virtualport, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }
                    if (FileMesege.PointList.logic != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.logic, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }
                    /*
                    if (FileMesege.PointList.localvar != null)
                    {
                        pointInfo = findPointByList_add(FileMesege.PointList.localvar, address, ip);
                        if (pointInfo != null)
                        {
                            return pointInfo;
                        }
                    }*/
                    return pointInfo;
                }
                // version_type 分割成
                switch (type.Split('_')[1])
                {
                    case "scene":
                        pointInfo = findPointByList_add(FileMesege.PointList.scene, address, ip);

                        break;

                    case "timer":
                        pointInfo = findPointByList_add(FileMesege.PointList.timer, address, ip);

                        break;

                    case "panel":
                        pointInfo = findPointByList_add(FileMesege.PointList.link, address, ip);

                        break;
                    case "sensor":
                        pointInfo = findPointByList_add(FileMesege.PointList.link, address, ip);

                        break;
                    case "virtualport":
                        pointInfo = findPointByList_add(FileMesege.PointList.virtualport, address, ip);

                        break;
                    case "logic":
                        pointInfo = findPointByList_add(FileMesege.PointList.logic, address, ip);

                        break;
                        /*
                    case "localvar":
                        pointInfo = findPointByList_add(FileMesege.PointList.localvar, address, ip);

                        break;*/
                    //为设备00 扫equipment
                    default:
                        pointInfo = findPointByList_add(FileMesege.PointList.equipment, address, ip);

                        break;
                }

                return pointInfo;
            }
            catch {
                return findPointByList_add(FileMesege.PointList.equipment, address, ip); 
            }
            
        }

        /// <summary>
        /// 根据PointList划分类型 和 address 获取该点位 返回DataJson.PointInfo 否则 null
        /// </summary>
        /// <param name="Jsonlist"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static DataJson.PointInfo findPointByList_add(List<DataJson.PointInfo> Jsonlist, string address, string ip)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(ip))
            {
                return null;
            }
            if (Jsonlist == null)
            {
                return null;
            }
            foreach (DataJson.PointInfo eq in Jsonlist)
            {
                if (eq.address == address && eq.ip == ip)
                {
                    return eq;

                }
            }
            return null;
        }

        /// <summary>
        /// 根据PointList划分类型 和 pid 获取该点位 返回DataJson.PointInfo 否则 null
        /// </summary>
        /// <param name="pid">pid号</param>
        /// <param name="list">Polist的类型</param>
        /// <returns></returns>
        public static DataJson.PointInfo findPointByPid(int pid, List<DataJson.PointInfo> list)
        {
            if (list == null)
            {
                return null;
            }
            foreach (DataJson.PointInfo point in list)
            {
                if (point.pid == pid)
                {

                    return point;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据PointList全表 pid 获取该点位 返回DataJson.PointInfo 否则 null
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static DataJson.PointInfo findPointByPid(int pid)
        {
            try
            {
                if (FileMesege.PointList == null)
                {
                    return null;
                }
                if (FileMesege.PointList.equipment != null)
                {
                    
                    foreach (DataJson.PointInfo point in FileMesege.PointList.equipment)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }

                if (FileMesege.PointList.scene != null)
                {
                    foreach (DataJson.PointInfo point in FileMesege.PointList.scene)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }

                if (FileMesege.PointList.link != null)
                {
                    foreach (DataJson.PointInfo point in FileMesege.PointList.link)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }

                if (FileMesege.PointList.timer != null)
                {
                    foreach (DataJson.PointInfo point in FileMesege.PointList.timer)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }

                if (FileMesege.PointList.virtualport != null)
                {
                    foreach (DataJson.PointInfo point in FileMesege.PointList.virtualport)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }

                if (FileMesege.PointList.logic != null)
                {
                    foreach (DataJson.PointInfo point in FileMesege.PointList.logic)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }
                /*
                if (FileMesege.PointList.localvar != null)
                {
                    foreach (DataJson.PointInfo point in FileMesege.PointList.localvar)
                    {
                        if (point.pid == pid)
                        {

                            return point;
                        }
                    }
                }*/
            }
            catch{
                return null;
            }
            return null;
        }

        /// <summary>
        /// 根据区域名称 和名称 搜索 PointList 全表中寻找point点 失败返回null
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DataJson.PointInfo findPointBySectionName(string section, string name)
        { 
            //区域
            List<string> sections = section.Split(' ').ToList();
            while (sections.Count != 4)
            {
                sections.Add("");
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }
           
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2] && eq.area4 == sections[3] && eq.name.Split('@')[0] == name)
                {
                    return eq;
                }
            }*/
            
            return null;
        }

        /// <summary>
        /// 根据PointList的 地域和名称 section---name---type或者section --- name  获取area1 area12 area3 area4 name  type（可以无）获取point点 配合dealPointInfo（）用
        /// </summary>
        /// <param name="section_name"></param>
        /// <returns></returns>
        public static DataJson.PointInfo findPointBySection_name(List<string> section_name)
        {
            if (section_name.Count < 5)
            {
                return null;
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    return eq;
                }
            }*/
            return null;
        }

        #endregion


        #region 操作sceneList的管理工具

        /// <summary>
        /// 在选中节点的基础上 按IP和定时号ID 寻找scenesList表中是scenes
        /// </summary>
        /// <returns></returns>
        public static DataJson.scenes getScenesInfoList()
        {
            if (FileMesege.sceneSelectNode == null || FileMesege.sceneSelectNode.Parent == null)
            {
                return null;
            }

            string ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
            string[] timerNodetxt = FileMesege.sceneSelectNode.Text.Split(' ');
            int timerNum = Convert.ToInt32(Regex.Replace(timerNodetxt[0], @"[^\d]*", ""));
            return getSceneInfoList(ip, timerNum);
        }

        /// <summary>
        /// 获取某个scenes列表中对应ID号的scenesInfo 否则返回空
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.sceneInfo getSceneInfo(DataJson.scenes ses, int id)
        {
            foreach (DataJson.sceneInfo info in ses.sceneInfo)
            {
                if (info.id == id)
                {
                    return info;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某个IP点 某个场景的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">场景号</param>
        /// <returns></returns>
        public static DataJson.scenes getSceneInfoList(string ip, int num)
        {
            try
            {
                if (FileMesege.sceneList == null)
                {
                    FileMesege.sceneList = new List<DataJson.Scene>();
                }
                foreach (DataJson.Scene scIP in FileMesege.sceneList)
                {
                    if (scIP.IP == ip)
                    {
                        foreach (DataJson.scenes sc in scIP.scenes)
                        {
                            if (sc.id == num)
                            {
                                return sc;
                            }
                        }

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }


        public static DataJson.scenes getSceneInfoListByPid(string ip, int pid)
        {
            try
            {
                if (FileMesege.sceneList == null)
                {
                    FileMesege.sceneList = new List<DataJson.Scene>();
                }
                foreach (DataJson.Scene scIP in FileMesege.sceneList)
                {
                    if (scIP.IP == ip)
                    {
                        foreach (DataJson.scenes sc in scIP.scenes)
                        {
                            if (sc.pid == pid)
                            {
                                return sc;
                            }
                        }

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
           
        }

        /// <summary>
        /// 获取某个IP点 的所有scene
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public static DataJson.Scene getSceneList(string ip)
        {
            try
            {
                if (FileMesege.sceneList == null)
                {
                    FileMesege.sceneList = new List<DataJson.Scene>();
                }
                foreach (DataJson.Scene scIP in FileMesege.sceneList)
                {
                    if (scIP.IP == ip)
                    {
                        return scIP;

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 删除场景中所有匹配的PID号
        /// </summary>
        /// <param name="pid"></param>
        public static void reMoveAllSceneByPid(int pid)
        {
            if (FileMesege.sceneList == null)
            {
                return;
            }
            for(int i =0 ;i <FileMesege.sceneList.Count;i++)
            {
                FileMesege.sceneList[i].scenes.RemoveAll(scene=>scene.pid ==pid);
            }
           
        }


        /// <summary>
        /// 修改场景某设备ID下的所有信息修改为新的ID
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="oldid"></param>
        public static void changeSceneInfoByid(string ip, int id,int oldid)
        {
            if (FileMesege.sceneList == null)
            {
                return;
            }
            string hexID =  id.ToString("X2");
            string hexOLDID = "00" + oldid.ToString("X2");
            foreach (DataJson.Scene scIP in FileMesege.sceneList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.scenes sc in scIP.scenes)
                    {
                        foreach (DataJson.sceneInfo sceneInfo in sc.sceneInfo) {
                            
                            if (sceneInfo.address.Length == 8 && sceneInfo.address.Substring(2, 4) == hexOLDID) {
                                //修改新ID
                                sceneInfo.address = sceneInfo.address.Substring(0, 4) + hexID + sceneInfo.address.Substring(6, 2);
                            }
                        }

                        
                    }
                }
            }
        }

        /// <summary>
        /// 删除场景某设备ID下的所有信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        public static void removeSceneInfoByid(string ip,int id)
        {
            if (FileMesege.sceneList == null)
            {
                return;
            }
            string hexID =  "00"+id.ToString("X2");
            foreach (DataJson.Scene scIP in FileMesege.sceneList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.scenes sc in scIP.scenes)
                    {
                        sc.sceneInfo.RemoveAll(item=> item.address.Length == 8 && item.address.Substring(2,4) == hexID );
                        //排序
                        for (int i = 0; i < sc.sceneInfo.Count; i++)
                        {
                            sc.sceneInfo[i].id = i + 1;
                        }
                    }
                }
            }
        }

        #endregion


        #region 操作timerList的管理工具

        /// <summary>
        /// 在选中节点的基础上 按IP和定时号ID 寻找timerList表中是timers
        /// </summary>
        /// <returns></returns>
        public static DataJson.timers getTimersInfoList()
        {
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return null;
            }

            string ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
            string[] timerNodetxt = FileMesege.timerSelectNode.Text.Split(' ');
            int timerNum = Convert.ToInt32(Regex.Replace(timerNodetxt[0], @"[^\d]*", ""));
            return getTimersInfoList(ip, timerNum);
        }

        /// <summary>
        /// 获取某个Timers列表中对应ID号的TiInfo 否则返回空
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.timersInfo getTimerInfo(DataJson.timers tms, int id)
        {
            
            foreach (DataJson.timersInfo info in tms.timersInfo)
            {
                if (info.id == id)
                {
                    return info;
                }
            }
            return null;
        }


        /// <summary>
        /// 获取某个IP点 某个定时的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">定时号</param>
        /// <returns></returns>
        public static DataJson.timers getTimersInfoList(string ip, int num)
        {
            try
            {
                if (FileMesege.timerList == null)
                {
                    FileMesege.timerList = new List<DataJson.Timer>();
                }
                foreach (DataJson.Timer tmIP in FileMesege.timerList)
                {
                    if (tmIP.IP == ip)
                    {
                        foreach (DataJson.timers tms in tmIP.timers)
                        {
                            if (tms.id == num)
                            {
                                return tms;
                            }
                        }

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
           
        }

        /// <summary>
        /// 获取某个IP点 的所有scene
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public static DataJson.Timer getTimerList(string ip)
        {
            try
            {
                if (FileMesege.timerList == null)
                {
                    FileMesege.timerList = new List<DataJson.Timer>();
                }
                foreach (DataJson.Timer tmlIP in FileMesege.timerList)
                {
                    if (tmlIP.IP == ip)
                    {
                        return tmlIP;

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 删除定时中所有匹配的PID号
        /// </summary>
        /// <param name="pid"></param>
        public static void reMoveAllTimersByPid(int pid)
        {
            if (FileMesege.timerList == null)
            {
                return;
            }
            for (int i = 0; i < FileMesege.timerList.Count; i++)
            {
                FileMesege.timerList[i].timers.RemoveAll(timers => timers.pid == pid);
            }

        }


        /// <summary>
        /// 修改定时某设备ID下的所有信息修改为新的ID
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="oldid"></param>
        public static void changeTimerInfoByid(string ip, int id, int oldid)
        {
            if (FileMesege.timerList == null)
            {
                return;
            }
            string hexID = id.ToString("X2");
            string hexOLDID = "00" + oldid.ToString("X2");
            foreach (DataJson.Timer scIP in FileMesege.timerList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.timers sc in scIP.timers)
                    {
                        foreach (DataJson.timersInfo sceneInfo in sc.timersInfo)
                        {

                            if (sceneInfo.address.Length == 8 && sceneInfo.address.Substring(2, 4) == hexOLDID)
                            {
                                //修改新ID
                                sceneInfo.address = sceneInfo.address.Substring(0, 4) + hexID + sceneInfo.address.Substring(6, 2);
                            }
                        }


                    }
                }
            }
        }

        /// <summary>
        /// 删除定时某设备ID下的所有信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        public static void removeTimerInfoByid(string ip, int id)
        {
            if (FileMesege.timerList == null)
            {
                return;
            }
            string hexID = "00" + id.ToString("X2");
            foreach (DataJson.Timer scIP in FileMesege.timerList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.timers sc in scIP.timers)
                    {
                        sc.timersInfo.RemoveAll(item => item.address.Length == 8 && item.address.Substring(2, 4) == hexID);
                        //排序
                        for (int i = 0; i < sc.timersInfo.Count; i++)
                        {
                            sc.timersInfo[i].id = i + 1;
                        }
                    }
                }
            }
        }

        #endregion


        #region 操作panelList的管理工具
        /// <summary>
        /// 在选中节点的基础上 按IP和定时号ID 寻找panelList表中是panels
        /// </summary>
        /// <returns></returns>
        public static DataJson.panels getPanelsInfoListByNode()
        {
            try
            {
                if (FileMesege.panelSelectNode == null || FileMesege.panelSelectNode.Parent == null)
                {
                    return null;
                }

                string ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                string[] panelNodetxt = FileMesege.panelSelectNode.Text.Split(' ');
                int panelNum = Convert.ToInt32(Regex.Replace(panelNodetxt[0], @"[^\d]*", ""));
                return getPanelsInfoList(ip, panelNum);
            }
            catch {
                return null;
            }
            
        }

        /// <summary>
        /// 获取某个Panels列表中对应ID号的panelsInfo 否则返回空
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.panelsInfo getPanelInfoByid(DataJson.panels pls, int id)
        {
            try
            {
                foreach (DataJson.panelsInfo info in pls.panelsInfo)
                {
                    if (info.id == id)
                    {
                        return info;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }


        /// <summary>
        /// 获取某个IP点 某个面板的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">面板号</param>
        /// <returns></returns>
        public static DataJson.panels getPanelsInfoList(string ip, int num)
        {
            try
            {
                foreach (DataJson.Panel plIp in FileMesege.panelList)
                {
                    if (plIp.IP == ip)
                    {
                        foreach (DataJson.panels pls in plIp.panels)
                        {
                            if (pls.id == num)
                            {
                                return pls;
                            }
                        }

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 获取某个IP点 的所有scene
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public static DataJson.Panel getPanelList(string ip)
        {
            try
            {
                foreach (DataJson.Panel plIP in FileMesege.panelList)
                {
                    if (plIP.IP == ip)
                    {
                        return plIP;

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 删除面板中所有匹配的PID号
        /// </summary>
        /// <param name="pid"></param>
        public static void reMoveAllPanelsByPid(int pid)
        {

            if (FileMesege.panelList == null)
            {
                return;
            }
            for (int i = 0; i < FileMesege.panelList.Count; i++)
            {
                FileMesege.panelList[i].panels.RemoveAll(panels => panels.pid == pid);
            }

        }

        /// <summary>
        /// 添加设备时候 添加面板
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        private static void AddDevPanel(string ip,int id,string version)
        {
            string keyVal = "";
            string path = Application.StartupPath + "\\devices\\";
            keyVal = IniConfig.GetValue(path + version + ".ini", "input", "key");
            if (string.IsNullOrEmpty(keyVal) || keyVal == "null" )
            {
                return;
            }
            //获取面板号
            string num = id.ToString("X4");
            string address = "FE30" + num;
            if (FileMesege.panelList == null)
            {
                FileMesege.panelList = new List<DataJson.Panel>();

            }
            //该IP在timerList里面是否存在  
            if (!FileMesege.panelList.Exists(x => x.IP == ip))
            {
                //不存在新建List
                DataJson.Panel pl = new DataJson.Panel();
                pl.IP = ip;
                pl.Dev = "GW100A";
                pl.panels = new List<DataJson.panels>();
                FileMesege.panelList.Add(pl);

            }

            int randomNum = DataChange.randomNum();
            foreach (DataJson.Panel pl in FileMesege.panelList)
            {
                //节点的IP相等 进入创建 不会存在相同ID号新建信息
                if (pl.IP == ip)
                {
                    DataJson.panels pls = new DataJson.panels();
                    pls.id = id;
                    pls.pid = randomNum;
                    //pls.keyNum = 0;
                    pls.panelsInfo = new List<DataJson.panelsInfo>();

                    //添加point点
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.area1 = "";
                    eq.area2 = "";
                    eq.area3 = "";
                    eq.area4 = "";
                    eq.address = address;
                    eq.pid = randomNum;
                    eq.ip = ip;
                    eq.name = IniConfig.GetValue(path + version + ".ini", "define", "note").Split('&')[0]+id.ToString()+"@"+ip.Split('.')[3];
                    eq.type = IniHelper.findTypesIniTypebyName("面板");
                    eq.objType = "";
                    eq.value = "";
                    if (FileMesege.PointList == null)
                    {
                        FileMesege.PointList= new DataJson.Point();
                    }
                    foreach (DataJson.PointInfo pi in FileMesege.PointList.link)
                    {
                        if (pi.area1 == "" && pi.area2 == "" && pi.area3 == "" && pi.area4 == "" && pi.name == eq.name)
                        {
                            MessageBox.Show("该名称已存在，请更换名称");
                            return;
                        }

                    }
                    //根据keyval添加按键地址
                    setKeyNum(pls,keyVal,ip,id);
                    pl.panels.Add(pls);
                    FileMesege.PointList.link.Add(eq);
                    //排序
                    pl.panels.Sort(delegate (DataJson.panels x, DataJson.panels y)
                    {
                        return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
                    });

                    break;
                }

            }
        }

        /// <summary>
        /// 设置当前面板为 几键面板 成功返回true
        /// </summary>
        /// <param name="keyNum"></param>
        /// <returns></returns>
        public static void setKeyNum(DataJson.panels pls, string keyVal,string ip,int id)
        {
            
            int keyNum = 0;
            if (keyVal.Contains("-"))
            {
                string[] infos = keyVal.Split('-');
                int j = Convert.ToInt32(infos[1]);
                keyNum = j;

            }
            else if (keyVal.Contains(","))
            {
                string[] infos = keyVal.Split(',');
             
                keyNum = Convert.ToInt32(infos[infos.Length-1]);
            }
            else
            {
                keyNum = Convert.ToInt32(keyVal);
            }
            if (keyNum > 255 || keyNum <= 0)
            {
                return ;
            }
            //循环每一页 
            string ipLast = ToolsUtil.strtohexstr(ip.Split('.')[3]);
            string tmpNum = "";
            string idHex = id.ToString("X2");

            //是否存在该键值ID
            bool isExit = false;
            List<DataJson.panelsInfo> delPanel = new List<DataJson.panelsInfo>();
            HashSet<int> numList = new HashSet<int>();
            int start = 0;
            int end = 0;
            //循环新建每页内容
            for (int pageNum = 0; pageNum < 10; pageNum++)
            {
                //循环清理多余的按键
                delPanel.Clear();
                numList.Clear();
                start = pageNum * 256 + keyNum;
                end = pageNum * 256 + 256;
                foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
                {
                    if (plInfo.id > start && plInfo.id <=end)
                    {
                        //删除的
                        delPanel.Add(plInfo);
                    }
                    if (plInfo.id > pageNum * 256 && plInfo.id <= end)
                    {
                        //保留的
                        numList.Add(plInfo.id);
                    }
                }

                for (int i = 0; i < delPanel.Count; i++)
                {
                    //删除id大于面板按键值
                    pls.panelsInfo.Remove(delPanel[i]);
                }
                List<int> list = numList.ToList<int>();
                //添加面板按键和地址
                for (int i = 1; i <= keyNum; i++)
                {
                    isExit = false;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (pageNum * 256 + i == list[j])
                        {
                            isExit = true;
                            break;
                        }
                    }
                    if (!isExit)
                    {
                        //添加改id的按键
                        DataJson.panelsInfo plInfo = new DataJson.panelsInfo();
                        plInfo.id = pageNum * 256 + i;
                       /* if (pageNum == 0)
                        {
                            plInfo.id =  i;

                        }
                        else
                        {
                            plInfo.id = pageNum * 256 + i +1;
                        }*/
                        plInfo.pid = 0;
                        tmpNum = plInfo.id.ToString("X4");
                        plInfo.keyAddress = string.Format("FE{0}{1}",  idHex, tmpNum);
                        //plInfo.keyAddress = string.Format("{0}{1}{2}", ipLast, idHex, tmpNum);
                        plInfo.objAddress = "";
                        plInfo.objType = "";
                        plInfo.opt = 255;
                        plInfo.showAddress = "";
                        plInfo.showMode = "";
                        pls.panelsInfo.Add(plInfo);

                    }
                }
            }
            
            //排序
            pls.panelsInfo.Sort(delegate (DataJson.panelsInfo x, DataJson.panelsInfo y)
            {

                return x.id.CompareTo(y.id);
            });

            return ;
        }



        /// <summary>
        /// 修改设备ID的时候 修改面板信息 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="version"></param>
        /// <param name="oldid"></param>
        private static void ChangeDevPanel(string ip, int id,string version,int oldid)
        {
            string keyVal = "";
            string path = Application.StartupPath + "\\devices\\";
            keyVal = IniConfig.GetValue(path + version + ".ini", "input", "key");
            if (string.IsNullOrEmpty(keyVal) || keyVal == "null")
            {
                return;
            }
            //获取该节点IP地址面板下的 面板信息对象
            DataJson.panels pls = DataListHelper.getPanelsInfoList(ip, oldid);
            if (pls == null)
            {
                return;
            }
            //获取面板号
            string num = id.ToString("X4");
            //获取IP最后一位
            string address = "FE30" + num;
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                //修改当前的point点信息
                if (pls.pid == eq.pid)
                {
                    
                    pls.id = id;
                   /* eq.area1 = "";
                    eq.area2 = "";
                    eq.area3 = "";
                    eq.area4 = "";*/
                    eq.address = address;
                    eq.name = IniConfig.GetValue(path + version + ".ini", "define", "note").Split('&')[0] + id.ToString() + "@" + ip.Split('.')[3]; ;
                    break;
                }

            }
            string idHex = id.ToString("X2");
            //修改键地址
            foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
            {
                if (!string.IsNullOrEmpty(plInfo.keyAddress))
                {
                    plInfo.keyAddress = plInfo.keyAddress.Remove(2,2);
                    plInfo.keyAddress = plInfo.keyAddress.Insert(2, idHex);
                }
            }
            foreach (DataJson.Panel pl in FileMesege.panelList)
            {
                if (pl.IP == ip)
                {
                    //排序
                    pl.panels.Sort(delegate (DataJson.panels x, DataJson.panels y)
                    {
                        return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
                    });
                    break;
                }
            }
         
        }

        private static void ChangeDevIDVerPanel(string ip, int id, string version, int oldid)
        {
            string keyVal = "";
            string path = Application.StartupPath + "\\devices\\";
            keyVal = IniConfig.GetValue(path + version + ".ini", "input", "key");
            if (string.IsNullOrEmpty(keyVal) || keyVal == "null")
            {
                DelDevPanel(ip,id);
                return ;
            }
            //获取该节点IP地址面板下的 面板信息对象
            DataJson.panels pls = DataListHelper.getPanelsInfoList(ip, oldid);
            if (pls == null)
            {
                AddDevPanel(ip, id, version);
                return;
            }
            //获取面板号
            string num = id.ToString("X4");
            //获取IP最后一位
            string address = "FE30" + num;
            
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                //修改当前的point点信息
                if (pls.pid == eq.pid)
                {
                    pls.id = id;
                    eq.area1 = "";
                    eq.area2 = "";
                    eq.area3 = "";
                    eq.area4 = "";
                    eq.address = address;
                    eq.name = IniConfig.GetValue(path + version + ".ini", "define", "note").Split('&')[0] + id.ToString() + "@" + ip.Split('.')[3]; ;
                    break;
                }

            }
            //检查配对的ID
            setKeyNum(pls, keyVal, ip, id);

            string idHex = id.ToString("X2");
            //修改键地址
            foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
            {
                if (!string.IsNullOrEmpty(plInfo.keyAddress))
                {
                    plInfo.keyAddress = plInfo.keyAddress.Remove(2, 2);
                    plInfo.keyAddress = plInfo.keyAddress.Insert(2, idHex);
                }
            }
            foreach (DataJson.Panel pl in FileMesege.panelList)
            {
                if (pl.IP == ip)
                {
                    //排序
                    pl.panels.Sort(delegate (DataJson.panels x, DataJson.panels y)
                    {
                        return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
                    });
                    break;
                }
            }

        }




        /// <summary>
        /// 删除设备的时候 删除面板
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        private static void DelDevPanel(string ip, int id)
        {
            
            foreach (DataJson.Panel pl in FileMesege.panelList)
            {
                //进入IP同一个
                if (pl.IP == ip)
                {
                    foreach (DataJson.panels pls in pl.panels)
                    {
                        //当面板号一样
                        if (pls.id == id)
                        {
                            //移除pointList 中地址
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
                            {
                                //获取address与IP地址相同的对象
                                if (eq.pid == pls.pid)
                                {
                                    //移除Namelist 的对象
                                    FileMesege.PointList.link.Remove(eq);
                                    break;
                                }
                            }
                            //移除panellist的对象
                            pl.panels.Remove(pls);
                            return;
                        }
                    }

                }

            }//IP FOREACH
        }


        /// <summary>
        /// 修改面板某设备ID下的所有信息修改为新的ID
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="oldid"></param>
        public static void changePanelInfoByid(string ip, int id, int oldid)
        {
            if (FileMesege.panelList == null)
            {
                return;
            }
            string hexID = id.ToString("X2");
            string hexOLDID = "00" + oldid.ToString("X2");
            foreach (DataJson.Panel scIP in FileMesege.panelList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.panels sc in scIP.panels)
                    {
                        foreach (DataJson.panelsInfo sceneInfo in sc.panelsInfo)
                        {

                            if (sceneInfo.objAddress.Length == 8 && sceneInfo.objAddress.Substring(2, 4) == hexOLDID)
                            {
                                //修改新ID
                                sceneInfo.objAddress = sceneInfo.objAddress.Substring(0, 4) + hexID + sceneInfo.objAddress.Substring(6, 2);
                            }
                            if (sceneInfo.showAddress.Length == 8 && sceneInfo.showAddress.Substring(2, 4) == hexOLDID)
                            {
                                //修改新ID
                                sceneInfo.showAddress = sceneInfo.showAddress.Substring(0, 4) + hexID + sceneInfo.showAddress.Substring(6, 2);
                            }

                        }


                    }
                }
            }
        }

        /// <summary>
        /// 删除面板某设备ID下的所有信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        public static void removePanelInfoByid(string ip, int id)
        {
            if (FileMesege.panelList == null)
            {
                return;
            }
            string hexID = "00" + id.ToString("X2");
            foreach (DataJson.Panel scIP in FileMesege.panelList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.panels sc in scIP.panels)
                    {
                        foreach (DataJson.panelsInfo plsInfo in sc.panelsInfo) {
                            if (plsInfo.objAddress.Length == 8 && plsInfo.objAddress.Substring(2, 4) == hexID) {
                                plsInfo.objAddress = "";
                                plsInfo.objType = "";
                                plsInfo.opt = 255;
                                plsInfo.pid = 0;
                                plsInfo.showAddress = "";
                                plsInfo.showMode = "";
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region 操作sensorList的管理工具


        /// <summary>
        /// 在选中节点的基础上 按IP和定时号ID 寻找sensorList表中是panels
        /// </summary>
        /// <returns></returns>
        public static DataJson.sensors getSensorInfoListByNode()
        {
            if (FileMesege.sensorSelectNode == null || FileMesege.sensorSelectNode.Parent == null)
            {
                return null;
            }

            string ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
            string[] panelNodetxt = FileMesege.sensorSelectNode.Text.Split(' ');
            int panelNum = Convert.ToInt32(Regex.Replace(panelNodetxt[0], @"[^\d]*", ""));
            return getSensorsInfoList(ip, panelNum);
        }

        /// <summary>
        /// 获取某个Sensors列表中对应ID号的sensorsInfo 否则返回空
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.sensorsInfo getSensorInfo(DataJson.sensors srs, int id)
        {
            try
            {
                foreach (DataJson.sensorsInfo info in srs.sensorsInfo)
                {
                    if (info.id == id)
                    {
                        return info;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
           
        }


        /// <summary>
        /// 获取某个IP点 某个感应的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">感应号</param>
        /// <returns></returns>
        public static DataJson.sensors getSensorsInfoList(string ip, int num)
        {
            try
            {
                foreach (DataJson.Sensor srIp in FileMesege.sensorList)
                {
                    if (srIp.IP == ip)
                    {
                        foreach (DataJson.sensors srs in srIp.sensors)
                        {
                            if (srs.id == num)
                            {
                                return srs;
                            }
                        }

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 获取某个IP点 的所有sensor
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public static DataJson.Sensor getSensorList(string ip)
        {
            try
            {
                foreach (DataJson.Sensor sr in FileMesege.sensorList)
                {
                    if (sr.IP == ip)
                    {
                        return sr;

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// 删除感应中所有匹配的PID号
        /// </summary>
        /// <param name="pid"></param>
        public static void reMoveAllSensorsByPid(int pid)
        {
            if (FileMesege.sensorList == null)
            {
                return;
            }
            for (int i = 0; i < FileMesege.sensorList.Count; i++)
            {
                FileMesege.sensorList[i].sensors.RemoveAll(sensors => sensors.pid == pid);
            }

        }

        /// <summary>
        /// 删除感应某设备ID下的所有信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        public static void removeSensorInfoByid(string ip, int id)
        {
            if (FileMesege.sensorList == null)
            {
                return;
            }
            string hexID = "00" + id.ToString("X2");
            foreach (DataJson.Sensor scIP in FileMesege.sensorList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.sensors sc in scIP.sensors)
                    {
                        foreach (DataJson.sensorsInfo srsInfo in sc.sensorsInfo)
                        {
                            if (srsInfo.objAddress.Length == 8 && srsInfo.objAddress.Substring(2, 4) == hexID)
                            {
                                srsInfo.keyAddress = "";
                                srsInfo.objAddress = "";
                                srsInfo.objType = "";
                                srsInfo.opt = "";
                                srsInfo.optName = "";
                                srsInfo.pid = 0;
                                srsInfo.fbmode = 0;
                            }
                        }
                        /*sc.sensorsInfo.RemoveAll(item => item.objAddress.Substring(2, 4) == hexID);
                        //排序
                        for (int i = 0; i < sc.sensorsInfo.Count; i++)
                        {
                            sc.sensorsInfo[i].id = i + 1;
                        }*/
                    }
                }
            }
        }

        #endregion


        #region 操作logicList的管理工具


        /// <summary>
        /// 在选中节点的基础上 按IP和定时号ID 寻找logicList表中logics
        /// </summary>
        /// <returns></returns>
        public static DataJson.logics getLogicInfoListByNode()
        {
            if (FileMesege.logicSelectNode == null || FileMesege.logicSelectNode.Parent == null)
            {
                return null;
            }

            string ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            string[] logicNodetxt = FileMesege.logicSelectNode.Text.Split(' ');
            int logicNum = Convert.ToInt32(Regex.Replace(logicNodetxt[0], @"[^\d]*", ""));
            return getLogicsInfoList(ip, logicNum);
        }

        /// <summary>
        /// 获取某个Logics列表中对应ID号的logicsInfo 否则返回空
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.logicsInfo getLogicInfo(DataJson.logics lgs, int id)
        {
            try
            {
                foreach (DataJson.logicsInfo info in lgs.logicsInfo)
                {
                    if (info.id == id)
                    {
                        return info;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
           
        }


        /// <summary>
        /// 获取某个IP点 某个逻辑的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">逻辑号</param>
        /// <returns></returns>
        public static DataJson.logics getLogicsInfoList(string ip, int num)
        {
            try
            {
                foreach (DataJson.Logic lgIp in FileMesege.logicList)
                {
                    if (lgIp.IP == ip)
                    {
                        foreach (DataJson.logics lgs in lgIp.logics)
                        {
                            if (lgs.id == num)
                            {
                                return lgs;
                            }
                        }

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
           
        }

        /// <summary>
        /// 获取某个IP点 的所有logic
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <returns></returns>
        public static DataJson.Logic getLogicList(string ip)
        {
            try
            {
                foreach (DataJson.Logic lg in FileMesege.logicList)
                {
                    if (lg.IP == ip)
                    {
                        return lg;

                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
            
        }


        /// <summary>
        /// 获取新ID在当前List的序号 
        /// </summary>
        /// <returns></returns>
        public static int getLogicInfoID(DataJson.logics lgs)
        {
            HashSet<int> hasharry = new HashSet<int>();
            foreach(DataJson.logicsInfo logsInfo in lgs.logicsInfo )
            {
                hasharry.Add( logsInfo.id);
            }
            //哈希表 同一个区域的所有序号都在里面
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
                    return i+1;
                }
            }
            return arry[arry.Count - 1] + 1;
        }

        /// <summary>
        /// 删除逻辑中所有匹配的PID号
        /// </summary>
        /// <param name="pid"></param>
        public static void reMoveAllLogicsByPid(int pid)
        {
            if (FileMesege.logicList == null)
            {
                return;
            }
            for (int i = 0; i < FileMesege.logicList.Count; i++)
            {
                FileMesege.logicList[i].logics.RemoveAll(logics => logics.pid == pid);
            }

        }

        /// <summary>
        /// 根据tabName 获取当前框的LogicInfo 否则返回null
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public static DataJson.logicsInfo findLogicInfoByTabName(string tabName)
        {
            DataJson.logics lgs = DataListHelper.getLogicInfoListByNode();
            if (lgs == null)
            {
                return null;
            }
            int id = Validator.GetNumber(tabName);
            if (id == -1)
            {
                return null;
            }
            //删除逻辑框信息内容
            foreach (DataJson.logicsInfo logicInfo in lgs.logicsInfo)
            {
                if (logicInfo.id == id)
                {
                    return logicInfo;

                }
            }
            return null;
        }


        /// <summary>
        /// 根据DGV表中的ID号获取该行的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.sceneInfo getLogicSceneInfo(int id, List<DataJson.sceneInfo> list)
        {
            if (list != null)
            {
                foreach (DataJson.sceneInfo sceneInfo in list)
                {
                    if (sceneInfo.id == id)
                    {
                        return sceneInfo;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 根据DGV表中的ID号获取该行的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.ConditionInfo getLogicConditionInfo(int id, List<DataJson.ConditionInfo> list)
        {
            if (list != null)
            {
                foreach (DataJson.ConditionInfo conditionInfo in list)
                {
                    if (conditionInfo.id == id)
                    {
                        return conditionInfo;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 根据DGV表中的ID号ABCDEF...  获取该行的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.VoiceItem getLogicVoiceItem(string id, List<DataJson.VoiceItem> list)
        {
            if (list != null)
            {
                foreach (DataJson.VoiceItem voiceItem in list)
                {
                    if (voiceItem.letter == id)
                    {
                        return voiceItem;
                    }
                }
            }
            return null;

        }


        /// <summary>
        /// 根据DGV表中的ID号获取该行的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataJson.VoiceIfItem getLogicVoiceIfItem(int id, List<DataJson.VoiceIfItem> list)
        {
            if (list != null)
            {
                foreach (DataJson.VoiceIfItem voiceIfItem in list)
                {
                    if (voiceIfItem.id == id)
                    {
                        return voiceIfItem;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 删除逻辑某设备ID下的所有信息          还需要完善
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        public static void removeLogicInfoByid(string ip, int id)
        {
            if (FileMesege.logicList == null)
            {
                return;
            }
            /*string hexID = "00" + id.ToString("X2");
            foreach (DataJson.Logic scIP in FileMesege.logicList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.logics sc in scIP.logics)
                    {
                        sc.logicsInfo.RemoveAll(item => item.content.Substring(2, 4) == hexID);
                        //排序
                        for (int i = 0; i < sc.logicsInfo.Count; i++)
                        {
                            sc.logicsInfo[i].id = i + 1;
                        }
                    }
                }
            }*/
        }

        #endregion


    }
}
