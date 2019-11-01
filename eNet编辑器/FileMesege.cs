using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;


namespace eNet编辑器
{
    class FileMesege
    {
        
        // 静态跨窗口数据传输  txtGateway.Text + " " +cbVersion.Text + " " +txtinfo.Text 跨窗口传输数据
        public static string info = "";
        /// <summary>
        /// 打开工程文件夹当前路径
        /// </summary>
        public static string filePath = "";
        /// <summary>
        /// 打开工程文件夹当前路径的文件名称
        /// </summary>
        public static string fileName = "";
        //工程临时缓存数据 默认路径
        public static string TmpFilePath = Application.StartupPath + "\\protmp";
        //treesection临时存放数据处 区域
        public static TreeNode sectionNode = null;
        //四个区域点 空的区域也用\\补齐 分割\\
        public static string sectionNodeCopy = "";//备份 不清除
        //点位区域 英文ini文件名称 tv_ir
        public static string objType = "";
        //treetitle名字临时存放 对象表
        public static string titleinfo = "";
        public static string titlePointSection = "";//存放 点位的名称 section//section---name
        //逻辑表的Tab的信息名称 用于DataListHelper.findLogicInfoByTabName函数
        public static string LogicTabName = "";
        public static int LogicSelectedTabIndex = 0;

        //左栏 树状图treename节点选中临时存放
        public static TreeNode tnselectNode = null;

        //左栏 树状图treename节点选中临时存放
        public static TreeNode sceneSelectNode = null;
        //左栏 树状图treename节点选中临时存放
        public static TreeNode timerSelectNode = null;

        //左栏 树状图treepanel节点选中临时存放
        public static TreeNode panelSelectNode = null;

        //左栏 树状图treeSensor节点选中临时存放
        public static TreeNode sensorSelectNode = null;

        //左栏 树状图treeVar节点选中临时存放
        public static TreeNode varSelectNode = null;

        //左栏 树状图treeLogic节点选中临时存放
        public static TreeNode logicSelectNode = null;
        /// <summary>
        /// form下按钮的选择命名、场景。。。。  默认为命名 用来设置treetitle的显示功能 name,point,scene,timer,panel,sensor,logic,virtualport
        /// </summary>
        public static string formType = "name";
        /// <summary>
        /// form下面cbType的索引号
        /// </summary>
        public static int cbTypeIndex = 0;

        //undo redo
        public static CommandManager cmds = new CommandManager();

        //快捷键的复制 粘贴副本
        public static DataJson.Module copyDevice = null;
        public static DataJson.PointInfo copyPoint = null;
        public static DataJson.sceneInfo copyScene = null;
        public static DataJson.timersInfo copyTimer = null;
        public static DataJson.panelsInfo copyPanel = null;
        public static DataJson.sensorsInfo copySensor = null;
        public static DataJson.sceneInfo copyLogicScene = null;
        public static DataJson.SceneItem copyLogicSceneItem = null;

        public static List<DataJson.Device> DeviceList;//工程设备的保存记录
        public static List<DataJson.Area1> AreaList;//
        public static DataJson.Point PointList;//Title表的设备信息
        public static DataJson.Serial serialList;//命名在线设备型号及版本序列号数据文件
        public static List<DataJson.Scene> sceneList;//场景
        public static List<DataJson.Timer> timerList;
        public static List<DataJson.Panel> panelList;//面板
        public static List<DataJson.Sensor> sensorList;//感应
        public static List<DataJson.Logic> logicList;//逻辑

        //设置窗口恢复上一次信息状态
        public static DataJson.PortDimmer portDimmer = null;

        public static DataJson.PortDimmer portDali = null;

        /// <summary>
        /// 窗体关闭发生事件
        /// </summary>
        /// <param name="e"></param>
        public  void formclosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("若未保存工程，窗口关闭后，数据即将丢失！是否现在关闭窗口", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

        #region 项目的新建 打开 保存 另存为
        //初始化 清空所有值 
        public static void iniPath()
       {
           // 静态跨窗口数据传输  txtGateway.Text + " " +cbVersion.Text + " " + 旧的（修改前）   txtGateway.Text + " " +cbVersion.Text  跨窗口传输数据
            info = "";
            //左栏 树状图treename节点选中临时存放
            tnselectNode = null;
            //左栏 树状图treename节点选中临时存放
            sceneSelectNode = null;
            //左栏 树状图treetimer节点选中临时存放
            timerSelectNode = null;
            //左栏 树状图treepanel节点选中临时存放
            panelSelectNode = null;
            //左栏 树状图treesensor节点选中临时存放
            sensorSelectNode = null;
            //工程设备的保存记录
            DeviceList = null;
            //工程位置树状图
            AreaList = null;
            //Title表的设备信息
            PointList = null;           
            //命名在线设备文件
            serialList = null;
            //场景
            sceneList = null;
            //定时
            timerList = null;
            //绑定
            panelList = null;
            //感应
            sensorList = null;
            //undo redo
            cmds = new CommandManager();
            filePath = "";
            fileName = "";
             sectionNode = null;
            //四个区域点 补齐 分割\\
            sectionNodeCopy = "";//备份 不清除
            
            //点位区域 英文ini文件名称 tv_ir
            objType = "";
            titleinfo = "";
            titlePointSection = "";//存放 点位的名称 section//section---name
       }

        public bool newfile()
        {
            //IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", filePath);
            iniPath();
            return true;            
            
        }

        public bool openfile()
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                string historyPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                if (historyPath != "")
                {
                    //设置此次默认目录为上一次选中目录  
                    op.InitialDirectory = historyPath.Split(',')[0];
                    
                }  
                op.Title = "请打开工程文件";
                op.Filter = "项目文件（*.yc）|*.yc|压缩文件（*.zip）|*.zip|All files(*.*)|*.*";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    if (readProject(op.FileName))
                    {
                        if (!historyPath.Contains(filePath))
                        {
                            //添加打开过的地址
                            IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", filePath + "," + historyPath);
                        }
                        return true;
                    }

                }
                return false;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }  

        }

        /// <summary>
        /// 读取需要打开的文件项目
        /// </summary>
        /// <param name="openFilePath"></param>
        /// <returns></returns>
        public bool readProject(string openFilePath)
        {
            string localFilePath = openFilePath.ToString(); //获得文件路径 
            string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
            //读取的后缀不为.cy
            if (fileNameExt.Split('.')[1] != "yc")
            {
                return false;
            }
            tmpPathClear();
            
            try
            {
                string msg;
                ZipHelper ziphelp = new ZipHelper();
                bool isUnzip = ziphelp.UnZipFile(localFilePath, TmpFilePath, out msg);
                if(!isUnzip)
                {
                    //MessageBox.Show("解析文件失败！", "提示");
                    return false;
                }
     
            }
            catch
            {
                return false;
            }
            
            if (!System.IO.File.Exists(TmpFilePath + "\\pro\\area.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\device.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\point.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\scene.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\panel.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\timer.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\sensor.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\logic.json")
                )
            {
                //MessageBox.Show("打开文件失败！", "提示");
                return false;
            }
            iniPath();
            DeviceList = JsonConvert.DeserializeObject<List<DataJson.Device>>(File.ReadAllText(TmpFilePath + "\\pro\\device.json"));
            AreaList = JsonConvert.DeserializeObject<List<DataJson.Area1>>(File.ReadAllText(TmpFilePath + "\\pro\\area.json"));
            PointList = JsonConvert.DeserializeObject<DataJson.Point>(File.ReadAllText(TmpFilePath + "\\pro\\point.json"));
            sceneList = JsonConvert.DeserializeObject<List<DataJson.Scene>>(File.ReadAllText(TmpFilePath + "\\pro\\scene.json"));
            panelList = JsonConvert.DeserializeObject<List<DataJson.Panel>>(File.ReadAllText(TmpFilePath + "\\pro\\panel.json"));
            timerList = JsonConvert.DeserializeObject<List<DataJson.Timer>>(File.ReadAllText(TmpFilePath + "\\pro\\timer.json"));
            sensorList = JsonConvert.DeserializeObject<List<DataJson.Sensor>>(File.ReadAllText(TmpFilePath + "\\pro\\sensor.json"));
            logicList = JsonConvert.DeserializeObject<List<DataJson.Logic>>(File.ReadAllText(TmpFilePath + "\\pro\\logic.json"));
            filePath = localFilePath;
            fileName = fileNameExt;
            return true;
        }

        public bool savefile()
        {
            try 
            {
                string localFilePath = filePath;
                string fileNameExt = fileName;
                //数据在缓存文件夹中
                if (string.IsNullOrEmpty(filePath))
                {
                    
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "请选择保存路径";
                    //设置文件类型 
                    sfd.Filter = "项目文件（*.yc）|*.yc|压缩文件（*.zip）|*.zip|All files(*.*)|*.*";
                    //设置默认文件类型显示顺序 
                    sfd.FilterIndex = 1;
                    //保存对话框是否记忆上次打开的目录 
                    sfd.RestoreDirectory = true;

                    //设置默认的文件名

                    sfd.FileName = "ProjectFile";// in wpf is  sfd.FileName = "YourFileName";

                    string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                    if (newPath != "")
                    {
                        //设置此次默认目录为上一次选中目录  
                        sfd.InitialDirectory = newPath.Split(',')[0];

                    } 
                    //点了保存按钮进入 
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        
                        localFilePath = sfd.FileName.ToString(); //获得文件路径 
                        fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                        string historyPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                        if (!historyPath.Contains(localFilePath))
                        {
                            //添加打开过的地址
                            IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", localFilePath + "," + historyPath);
                        }

                    }
                    else
                    {
                        //不做操作 退出
                        return false;
                    }
                    
                }
                filePath = localFilePath;
                fileName = fileNameExt;
                writeAlltoPro(TmpFilePath);
                string[] fileList = {TmpFilePath+"//pro",TmpFilePath+"//objs"};
                try
                {
                    string msg;
                    ZipHelper zipHelper = new ZipHelper();
                    //压缩到选中路径
                    zipHelper.ZipFolder(TmpFilePath, localFilePath, out msg);
                }
                catch
                {
                    return false;
                }
                return true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }  

             
        }

        public bool othersavefile()
        {
            try
            {
                string localFilePath = "";
                string fileNameExt = "";
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "请选择保存路径";
                //设置文件类型 
                sfd.Filter = "项目文件（*.yc）|*.yc|压缩文件（*.zip）|*.zip|All files(*.*)|*.*";
                //设置默认文件类型显示顺序 
                sfd.FilterIndex = 1;
                //保存对话框是否记忆上次打开的目录 
                sfd.RestoreDirectory = true;

                //设置默认的文件名

                sfd.FileName = "ProjectFile";// in wpf is  sfd.FileName = "YourFileName";
                string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                if (newPath != "")
                {
                    //设置此次默认目录为上一次选中目录  
                    sfd.InitialDirectory = newPath.Split(',')[0];

                }
                //点了保存按钮进入 
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    localFilePath = sfd.FileName.ToString(); //获得文件路径 
                    fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                    string historyPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                    if (!historyPath.Contains(localFilePath))
                    {
                        //添加打开过的地址
                        IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", localFilePath + "," + historyPath);
                    }
                    writeAlltoPro(TmpFilePath);
                    string[] fileList = { TmpFilePath + "//pro", TmpFilePath + "//objs" };
                    try
                    {

                        string msg;
                        ZipHelper zipHelper = new ZipHelper();
                        //压缩到选中路径
                        zipHelper.ZipFolder(TmpFilePath, localFilePath, out msg);
     
                    }
                    catch
                    {
                        return false;
                    }

                    return true;

                }
                else
                {
                    //不做操作 退出
                    return false;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }  
                       
        }

        /// <summary>
        /// 格式化JSON文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }


        /// <summary>
        /// 把缓存数据写进文档 补充说明：工程保存写入文件
        /// </summary>
        /// <param name="path">Pro文件路径</param>
        private void writeAlltoPro(string path)
        {
            tmpPathClear();
            File.WriteAllText(path + "\\pro\\point.json", ConvertJsonString(JsonConvert.SerializeObject(PointList)));
            File.WriteAllText(path + "\\pro\\area.json", ConvertJsonString(JsonConvert.SerializeObject(AreaList)));
            File.WriteAllText(path + "\\pro\\device.json", ConvertJsonString(JsonConvert.SerializeObject(DeviceList)));
            File.WriteAllText(path + "\\pro\\scene.json", ConvertJsonString(JsonConvert.SerializeObject(sceneList)));
            File.WriteAllText(path + "\\pro\\panel.json", ConvertJsonString(JsonConvert.SerializeObject(panelList)));
            File.WriteAllText(path + "\\pro\\timer.json", ConvertJsonString(JsonConvert.SerializeObject(timerList)));
            File.WriteAllText(path + "\\pro\\sensor.json", ConvertJsonString(JsonConvert.SerializeObject(sensorList)));
            File.WriteAllText(path + "\\pro\\logic.json", ConvertJsonString(JsonConvert.SerializeObject(logicList)));
        }

        /// <summary>
        /// 清除protmp缓存的文件
        /// </summary>
        /// <param name="path"></param>
        public void tmpPathClear()
        {
            try
            {
                
                if (System.IO.Directory.Exists(TmpFilePath))
                {
                    Directory.Delete(TmpFilePath, true);
                }
                /*
                if (System.IO.Directory.Exists(TmpFilePath + "\\objs"))
                {
                    Directory.Delete(TmpFilePath + "\\objs", true);
                }
                if (System.IO.Directory.Exists(TmpFilePath + "\\pro"))
                {
                    Directory.Delete(TmpFilePath + "\\pro", true);
                }*/
                Directory.CreateDirectory(TmpFilePath + "\\objs");
                Directory.CreateDirectory(TmpFilePath + "\\pro");
            }
            catch { 
                
            }
        }

        #endregion


        #region 编译 下载
        /// <summary>
        /// 把该ip名称的文件夹删除 再重新建立该文件夹 并把旧IP.zip包删除
        /// </summary>
        /// <param name="ip"></param>
        public bool ObjDirClearByIP(string ip)
        {
            try
            {
                string tmpPath = string.Format("{0}\\objs\\{1}", TmpFilePath, ip);
                if (System.IO.Directory.Exists(tmpPath))
                {
                    Directory.Delete(tmpPath, true);
                }
       
                File.Delete(tmpPath + ".zip");
                Directory.CreateDirectory(tmpPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region 点位 位置 设备列表
        /// <summary>
        /// 在point.json总文件中抽取该网关ip的信息并写进 所有点位信息 成功：返回json字符串 失败：返回""
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string getPointJsonByIP(string ip)
        {
            try
            {
                //找到该ip的目录 然后在List表中把该ip的节点抽离出来并写进去该文件夹
                DataJson.Point gwPoint = new DataJson.Point();
                if (PointList != null)
                {
                    if (PointList.equipment != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.equipment)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.equipment.Add(point);
                            }
                        }
                    }
                    if (PointList.scene != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.scene)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.scene.Add(point);
                            }
                        }
                    }
                    if (PointList.timer != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.timer)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.timer.Add(point);
                            }
                        }
                    }
                    if (PointList.link != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.link)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.link.Add(point);
                            }
                        }
                    }
                    if (PointList.virtualport != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.virtualport)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.virtualport.Add(point);
                            }
                        }
                    }
                    if (PointList.logic != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.logic)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.logic.Add(point);
                            }
                        }
                    }
                    if (PointList.localvar != null)
                    {
                        foreach (DataJson.PointInfo point in PointList.localvar)
                        {
                            if (point.ip == ip)
                            {
                                gwPoint.localvar.Add(point);
                            }
                        }
                    }
                }
                //File.WriteAllText(string.Format("{0}\\objs\\{1}\\point.json", TmpFilePath, ip), ConvertJsonString(JsonConvert.SerializeObject(gwPoint)));
                return JsonConvert.SerializeObject(gwPoint);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return "";
            }
        }

        /// <summary>
        /// 把area.json总文件写入该ip  成功：返回json字符串 失败：返回""
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string getAreaJsonByIP(string ip)
        {
            try
            {
               
                if (AreaList == null)
                {
                    AreaList = new List<DataJson.Area1>();
                }
                //File.WriteAllText(string.Format("{0}\\objs\\{1}\\area.json", TmpFilePath, ip), ConvertJsonString(JsonConvert.SerializeObject(AreaList)));
                return JsonConvert.SerializeObject(AreaList);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return "";
            }
        }

        /// <summary>
        /// 把device.json总文件中抽取该网关ip的设备信息并写进 所有点位信息  成功：返回json字符串 失败：返回""
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string getDeviceJsonByIP(string ip)
        {
            try
            {

                if (DeviceList == null)
                {
                    DeviceList = new List<DataJson.Device>();
                }
                DataJson.Device tmp = new DataJson.Device();
                foreach (DataJson.Device info in DeviceList)
                {
                    if (info.ip == ip)
                    {
                        tmp = info;
                        break;
                    }
                }
                //File.WriteAllText(string.Format("{0}\\objs\\{1}\\device.json", TmpFilePath, ip), ConvertJsonString(JsonConvert.SerializeObject(tmp)));
                return JsonConvert.SerializeObject(tmp);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return "";
            }
        }
        #endregion

        #region 场景
        /// <summary>
        /// 把scene.json该ip地址的下的场景 全部抽离 产生s1.json,s2.json,sX.json
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool getSceneJsonByIP(string ip)
        {
            try
            {
                DataJson.Scene sc = DataListHelper.getSceneList(ip);

                //获取该IP下的所有场景
                if( sc == null || sc.scenes.Count == 0)
                {
                    return true;
                }
                foreach (DataJson.scenes scs in sc.scenes)
                {
                    //场景信息为空
                    if (scs.sceneInfo.Count == 0)
                    {
                        continue;
                    }
                    DataJson.Sn sn = new DataJson.Sn();
                    sn.action = new List<DataJson.Scenenumber>();
                    //把有效的对象操作 放到SN对象里面
                    foreach (DataJson.sceneInfo info in scs.sceneInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(info.opt) || info.address == "FFFFFFFF")
                        {
                            
                            continue;
                        }
                        DataJson.Scenenumber sb = new DataJson.Scenenumber();

                        sb.num = info.id;
                        sb.obj = info.address;
                        sb.val = info.opt;
                        sb.optname = info.optName;
                        sb.delay = info.delay;
                        sn.action.Add(sb);

                    }
                    if (sn.action.Count > 0)
                    {
                        File.WriteAllText(string.Format("{0}\\objs\\{1}\\s{2}.json", TmpFilePath, ip, scs.id), ConvertJsonString(JsonConvert.SerializeObject(sn)));
                    }
                    

                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }
        #endregion

        #region 定时
        /// <summary>
        /// 把timer.json该ip地址的下的定时 全部抽离 产生t1.json,t2.json,tX.json
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool getTimerJsonByIP(string ip)
        {
            try
            {
                DataJson.Timer tm = DataListHelper.getTimerList(ip);

                //获取该IP下的所有定时
                if (tm == null || tm.timers.Count == 0)
                {
                    return true;
                }
                foreach (DataJson.timers tms in tm.timers)
                {
                    //定时信息为空
                    if (tms.timersInfo.Count == 0)
                    {
                        continue;
                    }
                    DataJson.Tn tn = new DataJson.Tn();
                    tn.timer = new List<DataJson.Timernumber>();
                    if (string.IsNullOrEmpty(tms.dates))
                    {
                        continue;
                    }
                    //把有效的对象操作 放到SN对象里面
                    foreach (DataJson.timersInfo tmInfo in tms.timersInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(tmInfo.opt) || string.IsNullOrEmpty(tmInfo.shortTime) || tmInfo.address == "FFFFFFFF")
                        {
                            continue;
                        }
                        int hour = 255;
                        int min = 255;
                        if (tmInfo.shortTime == "日出时间")
                        {
                            hour = 254;
                            min = 254;
                        }
                        else if (tmInfo.shortTime == "日落时间")
                        {
                            hour = 253;
                            min = 253;
                        }
                        else
                        {
                            string tmpHour = tmInfo.shortTime.Split(':')[0];
                            string tmpMin = tmInfo.shortTime.Split(':')[1];
                            if (!tmpHour.Contains("*"))
                            {
                                hour = Convert.ToInt32(tmpHour);
                            }
                            if (!tmpMin.Contains("*"))
                            {
                                min = Convert.ToInt32(tmpMin);
                            }

                        }

                        string[] dates = tms.dates.Split(',');
                        if (tms.dates.Contains("/"))
                        {
                            //自定义日期
                            for (int i = 0; i < dates.Length; i++)
                            {
                                DataJson.Timernumber sb = new DataJson.Timernumber();

                                sb.num = tmInfo.id;
                                sb.obj = tmInfo.address;
                                sb.data = tmInfo.opt;
                                sb.optname = tmInfo.optName;
                                sb.hour = hour;
                                sb.min = min;
                                string[] ymd = dates[i].Split('/');
                                if (ymd[0].Contains("*"))
                                {
                                    //年为255
                                    sb.year = 255;
                                }
                                else
                                {
                                    sb.year = Convert.ToInt32(ymd[0]);
                                }
                                if (ymd[1].Contains("*"))
                                {
                                    //月为255
                                    sb.mon = 255;
                                }
                                else
                                {
                                    sb.mon = Convert.ToInt32(ymd[1]);
                                }
                                //这个是日
                                sb.date = Convert.ToInt32(ymd[2]);
                                //这个是周
                                sb.day = 255;
                                tn.timer.Add(sb);
                            }
                        }
                        else
                        {
                            //星期一到日 0-7
                            for (int i = 0; i < dates.Length; i++)
                            {
                                DataJson.Timernumber sb = new DataJson.Timernumber();
                                sb.num = tmInfo.id;
                                sb.obj = tmInfo.address;
                                sb.data = tmInfo.opt;
                                sb.optname = tmInfo.optName;
                                sb.hour = hour;
                                sb.min = min;
                                sb.year = 255;
                                sb.mon = 255;
                                sb.date = 255;
                                sb.day = Convert.ToInt32(dates[i]);
                                tn.timer.Add(sb);
                            }
                        }

                    }
                    if (tn.timer.Count > 0)
                    {
                        File.WriteAllText(string.Format("{0}\\objs\\{1}\\t{2}.json", TmpFilePath, ip, tms.id), ConvertJsonString(JsonConvert.SerializeObject(tn)));
                    }


                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }
        #endregion

        #region 面板
        /// <summary>
        /// 把panel.json该ip地址的下的面板 全部抽离 产生k1.json,k2.json,kX.json 1-999号
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool getPanelJsonByIP(string ip)
        {
            try
            {
                DataJson.Panel pl = DataListHelper.getPanelList(ip);

                //获取该IP下的所有场景
                if (pl == null || pl.panels.Count == 0)
                {
                    return true;
                }
                foreach (DataJson.panels pls in pl.panels)
                {
                    //场景信息为空
                    if (pls.panelsInfo.Count == 0)
                    {
                        continue;
                    }
                    DataJson.Kn kn = new DataJson.Kn();
                    kn.key = new List<DataJson.Keynumber>();

                    //把有效的对象操作 放到kN对象里面
                    foreach (DataJson.panelsInfo plInfo in pls.panelsInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(plInfo.keyAddress) 
                            || string.IsNullOrEmpty(plInfo.objAddress)
                            || plInfo.opt == 0 
                            || string.IsNullOrEmpty(plInfo.objType)
                            || plInfo.objAddress == "FFFFFFFF"
                            )
                        {
                            continue;
                        }

                        DataJson.Keynumber keyInfo = new DataJson.Keynumber();
                        keyInfo.num = plInfo.id;
                        keyInfo.key = "00" + plInfo.keyAddress.Substring(2, 6);
                        keyInfo.obj = plInfo.objAddress;
                        keyInfo.mode = plInfo.opt;
                        if (string.IsNullOrEmpty(plInfo.showAddress))
                        {
                            keyInfo.fback = "00000000";
                        }
                        else
                        {
                            keyInfo.fback = "FE" + plInfo.showAddress.Substring(2, 6);
                        }
                        //显示模式
                        keyInfo.fbmode = getShowMode(plInfo.showMode);

                        kn.key.Add(keyInfo);



                    }
                    if (kn.key.Count > 0)
                    {
                        File.WriteAllText(string.Format("{0}\\objs\\{1}\\k{2}.json", TmpFilePath, ip, pls.id), ConvertJsonString(JsonConvert.SerializeObject(kn)));
                    }


                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        public static int getShowMode(string mode)
        {
            switch (mode)
            {
                case "同步":
                    return 1;
                case "反显":
                    return 128;
                case "图形按键":
                    return 2;
                case "图形滑动条":
                    return 4;
                case "图形图标":
                    return 5;
                case "图形数值1":
                    return 80;
                case "图形数值0.1":
                    return 81;
                case "图形数值0.01":
                    return 82;
                case "图形数值0.001":
                    return 83;
                case "图形数值0.0001":
                    return 84;
                default: return 255;
            }
        }
        #endregion

        #region 感应
        /// <summary>
        /// 把sensor.json该ip地址的下的感应 全部抽离 产生k1001.json,k1002.json,kX.json 1001-1999号
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool getSensorJsonByIP(string ip)
        {
            try
            {
                DataJson.Sensor sr = DataListHelper.getSensorList(ip);

                //获取该IP下的所有场景
                if (sr == null || sr.sensors.Count == 0)
                {
                    return true;
                }
                foreach (DataJson.sensors srs in sr.sensors)
                {
                    //场景信息为空
                    if (srs.sensorsInfo.Count == 0)
                    {
                        continue;
                    }
                     DataJson.Kn kn = new DataJson.Kn();
                    kn.key = new List<DataJson.Keynumber>();

                    //把有效的对象操作 放到kN对象里面
                    foreach (DataJson.sensorsInfo srInfo in srs.sensorsInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(srInfo.keyAddress)
                            || string.IsNullOrEmpty(srInfo.objAddress)
                            || string.IsNullOrEmpty(srInfo.opt)
                            || string.IsNullOrEmpty(srInfo.objType)
                            || srInfo.fbmode == 0
                            || srInfo.objAddress == "FFFFFFFF"
                            )
                        {
                            continue;
                        }

                        DataJson.Keynumber keyInfo = new DataJson.Keynumber();
                        keyInfo.num = srInfo.id;
                        keyInfo.key = "00" + srInfo.keyAddress.Substring(2, 6);
                        keyInfo.obj = srInfo.objAddress;
                        keyInfo.mode = 255;
                        keyInfo.fback = srInfo.opt;

                        keyInfo.fbmode = srInfo.fbmode;

                        kn.key.Add(keyInfo);
                    }
                    if (kn.key.Count > 0)
                    {
                        File.WriteAllText(string.Format("{0}\\objs\\{1}\\k{2}.json", TmpFilePath, ip, srs.id), ConvertJsonString(JsonConvert.SerializeObject(kn)));
                    }


                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }
        #endregion

        #region 逻辑
        public bool getLogicJsonByIp(string ip)
        {
            try
            {
                DataJson.Logic lg = DataListHelper.getLogicList(ip);

                //获取该IP下的所有逻辑
                if (lg == null || lg.logics.Count == 0)
                {
                    return true;
                }
                foreach (DataJson.logics lgs in lg.logics)
                {
                    //逻辑的信息为空
                    if (lgs.logicsInfo.Count == 0)
                    {
                        continue;
                    }
                    
                    //逻辑模块
                    DataJson.Lc lc = new DataJson.Lc();
                    lc.trigger = new List<DataJson.TriggerNumber>();
                    //把有效的对象操作 放到lc对象里面
                    foreach (DataJson.logicsInfo info in lgs.logicsInfo)
                    {
                        //确保有信息
                        if (string.IsNullOrEmpty(info.content))
                        {
                            continue;
                        }
                      
                        switch(info.modelType)
                        {
                            case "SceneDeal":
                                //提取场景处理的逻辑
                                SceneDeal(lc,info,lgs.id);
                                break;
                            case "ConditionDeal":
                                //提取多条件处理的逻辑
                                ConditionDeal(lc, info);
                                break;
                            case "VoiceDeal":
                                //提取表达式处理的逻辑
                                VoiceDeal(lc, info);
                                break;
                            default:
                                break;
                        }
                        
            

                    }
                    //判断有没有num块存在
                    if (lc.trigger.Count > 0)
                    {
                        
                        //File.WriteAllText(string.Format("{0}\\objs\\{1}\\l{2}.json", TmpFilePath, ip, lgs.id), ConvertJsonString(JsonConvert.SerializeObject(lc)));
                        //暂时输出到某个目录下面
                        File.WriteAllText(string.Format("C:\\Users\\Administrator\\Desktop\\工程obj\\tmp\\l{0}.json", lgs.id), ConvertJsonString(JsonConvert.SerializeObject(lc)));
                    }
                    

                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        #region 场景处理提取SWITCH和CASE

        private void SceneDeal(DataJson.Lc lc, DataJson.logicsInfo LogicInfo,int LogicID)
        {
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(LogicInfo.content);
            //计算场景数量是50的多小倍
            int j = (logicSceneContent.SceneItemInfo.Count - logicSceneContent.SceneItemInfo.Count % 50) / 50;
            if (j > 0)
            {
                #region 场景数量超出50的范围
                //最后一个item的switch存放所有变量
                string lastSwitch = "";
                //场景数量超出50的范围
                for (int i = 0; i <= j; i++)
                {
                    DataJson.TriggerNumber total = new DataJson.TriggerNumber();
                    total.num = i + 1;
                    total.attr = LogicInfo.attr;
                    total.modelType = LogicInfo.modelType;
                    //填写switch
                    for (int k = i * 50; k < i * 50 + 50; k++)
                    {
                        if (k >= logicSceneContent.SceneItemInfo.Count)
                        {
                            //超出索引范围退出
                            break;
                        }
                        //确保有信息 
                        if (string.IsNullOrEmpty(logicSceneContent.SceneItemInfo[k].state) || logicSceneContent.SceneItemInfo[k].address == "FFFFFFFF")
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(total.@switch))
                        {
                            total.@switch = string.Format("({0}=={1})", logicSceneContent.SceneItemInfo[k].address, logicSceneContent.SceneItemInfo[k].state);
                        }
                        else
                        {
                            total.@switch = string.Format("{0}&&({1}=={2})", total.@switch, logicSceneContent.SceneItemInfo[k].address, logicSceneContent.SceneItemInfo[k].state);
                        }
                    }
                    string localVarAddress = string.Format("FEF9{0}", ((LogicID-1) * 8 + total.num).ToString("X4"));
                    lastSwitch = string.Format("{0}&&{1}", lastSwitch, localVarAddress);
                    DataJson.ConditionCase cas1 = new DataJson.ConditionCase();
                    cas1.@case = "1";
                    cas1.obj = localVarAddress;
                    cas1.data = "00000001";
                    cas1.delay = 0;
                    total.conditionCase.Add(cas1);

                    DataJson.ConditionCase cas2 = new DataJson.ConditionCase();
                    cas2.@case = "*";
                    cas2.obj = localVarAddress;
                    cas2.data = "00000000";
                    cas2.delay = 0;
                    total.conditionCase.Add(cas2);

                    //当switch项内容不为空且 case的内容不为空
                    if (total.conditionCase.Count > 0 && !string.IsNullOrEmpty(total.@switch))
                    {
                        lc.trigger.Add(total);
                    }

                }//for
                if (lc.trigger.Count > 0)
                { 
                    //添加最后一项
                    DataJson.TriggerNumber total = new DataJson.TriggerNumber();
                    total.num = j+2;
                    total.attr = LogicInfo.attr;
                    total.modelType = LogicInfo.modelType;
                    total.@switch = lastSwitch.Substring(2, lastSwitch.Length-2);

                    DataJson.PointInfo point = DataListHelper.findPointByPid(logicSceneContent.pid);
                    if(point == null)
                    {
                        //清空所有内容
                        lc.trigger.Clear();
                        return;
                    }
                    DataJson.ConditionCase cas1 = new DataJson.ConditionCase();
                    cas1.@case = "1";
                    cas1.obj = point.address;
                    cas1.data = "10000001";
                    cas1.delay = 0;
                    total.conditionCase.Add(cas1);

                    DataJson.ConditionCase cas2 = new DataJson.ConditionCase();
                    cas2.@case = "*";
                    cas2.obj = point.address;
                    cas2.data = "10000000";
                    cas2.delay = 0;
                    total.conditionCase.Add(cas2);

                    //当switch项内容不为空且 case的内容不为空
                    if (total.conditionCase.Count > 0 && !string.IsNullOrEmpty(total.@switch))
                    {
                        lc.trigger.Add(total);
                    }

                }
                #endregion
            }
            else
            {
                #region 场景数量不超50范围
                //场景数量不超50范围
                DataJson.TriggerNumber total = new DataJson.TriggerNumber();
                total.num = 1;
                total.attr = LogicInfo.attr;
                total.modelType = LogicInfo.modelType;
                //填写switch
                for (int k = 0; k < logicSceneContent.SceneItemInfo.Count; k++)
                {
            
                    //确保有信息 
                    if (string.IsNullOrEmpty(logicSceneContent.SceneItemInfo[k].state) || logicSceneContent.SceneItemInfo[k].address == "FFFFFFFF")
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(total.@switch))
                    {
                        total.@switch = string.Format("({0}=={1})", logicSceneContent.SceneItemInfo[k].address, logicSceneContent.SceneItemInfo[k].state);
                    }
                    else
                    {
                        total.@switch = string.Format("{0}&&({1}=={2})", total.@switch, logicSceneContent.SceneItemInfo[k].address, logicSceneContent.SceneItemInfo[k].state);
                    }
                }
                //填写case
                DataJson.PointInfo point = DataListHelper.findPointByPid(logicSceneContent.pid);
                if (point == null)
                {
                    //清空所有内容
                    lc.trigger.Clear();
                    return;
                }
                DataJson.ConditionCase cas1 = new DataJson.ConditionCase();
                cas1.@case = "1";
                cas1.obj = point.address;
                cas1.data = "10000001";
                cas1.delay = 0;
                total.conditionCase.Add(cas1);

                DataJson.ConditionCase cas2 = new DataJson.ConditionCase();
                cas2.@case = "*";
                cas2.obj = point.address;
                cas2.data = "10000000";
                cas2.delay = 0;
                total.conditionCase.Add(cas2);

                //当switch项内容不为空且 case的内容不为空
                if (total.conditionCase.Count > 0 && !string.IsNullOrEmpty(total.@switch))
                {
                    lc.trigger.Add(total);
                }
                #endregion
            }
           
 

        }

        #endregion

        #region 多条件处理提取SWITCH和CASE

        private void ConditionDeal(DataJson.Lc lc, DataJson.logicsInfo LogicInfo)
        {
            
            DataJson.ConditionContent logicConditionContent = JsonConvert.DeserializeObject<DataJson.ConditionContent>(LogicInfo.content);            
            //场景数量不超50范围
            DataJson.TriggerNumber total = new DataJson.TriggerNumber();
            total.num = LogicInfo.id;
            total.attr = LogicInfo.attr;
            total.modelType = LogicInfo.modelType;
            //填写switch
            for (int k = 0; k < logicConditionContent.conditionInfo.Count; k++)
            {

                //确保有信息 
                if (string.IsNullOrEmpty(logicConditionContent.conditionInfo[k].compareobjAddress) 
                    || string.IsNullOrEmpty(logicConditionContent.conditionInfo[k].objAddress)
                    || string.IsNullOrEmpty(logicConditionContent.conditionInfo[k].operation)
                    || logicConditionContent.conditionInfo[k].objAddress == "FFFFFFFF"
                    || logicConditionContent.conditionInfo[k].compareobjAddress == "FFFFFFFF"
                    || logicConditionContent.conditionInfo[k].a == 0
                    || logicConditionContent.conditionInfo[k].c == 0
                    )
                {
                    continue;
                }
                
                if (string.IsNullOrEmpty(total.@switch))
                {
                    
                    total.@switch = string.Format("({0}{1}{2})",
                        PowerAdd(logicConditionContent.conditionInfo[k].a, logicConditionContent.conditionInfo[k].b, logicConditionContent.conditionInfo[k].objAddress),
                        logicConditionContent.conditionInfo[k].operation,
                        PowerAdd(logicConditionContent.conditionInfo[k].c, logicConditionContent.conditionInfo[k].d, logicConditionContent.conditionInfo[k].compareobjAddress)
                        
                        );
                }
                else
                {
                    total.@switch = string.Format("{0}&&({1}{2}{3})",
                        total.@switch,
                        PowerAdd(logicConditionContent.conditionInfo[k].a, logicConditionContent.conditionInfo[k].b, logicConditionContent.conditionInfo[k].objAddress),
                        logicConditionContent.conditionInfo[k].operation,
                        PowerAdd(logicConditionContent.conditionInfo[k].c, logicConditionContent.conditionInfo[k].d, logicConditionContent.conditionInfo[k].compareobjAddress)
                        );
                }
            }
            //填写case
            if (string.IsNullOrEmpty(total.@switch))
            {
                //清空所有内容
                lc.trigger.Clear();
                return;
            }
            //trueDo
            for (int k = 0; k < logicConditionContent.trueDo.Count; k++)
            {
                if (string.IsNullOrEmpty(logicConditionContent.trueDo[k].address)
                    || logicConditionContent.trueDo[k].address == "FFFFFFFF"
                    || string.IsNullOrEmpty(logicConditionContent.trueDo[k].opt)
                )
                {
                    continue;
                }
                DataJson.ConditionCase cas1 = new DataJson.ConditionCase();
                cas1.@case = "1";
   
                
                cas1.obj = logicConditionContent.trueDo[k].address;
                cas1.data = logicConditionContent.trueDo[k].opt;
                cas1.optname = logicConditionContent.trueDo[k].optName;
                cas1.delay = logicConditionContent.trueDo[k].delay;
                total.conditionCase.Add(cas1);
            }
            //falseDo
            for (int k = 0; k < logicConditionContent.falseDo.Count; k++)
            {
                if (string.IsNullOrEmpty(logicConditionContent.falseDo[k].address)
                    || logicConditionContent.falseDo[k].address == "FFFFFFFF"
                    || string.IsNullOrEmpty(logicConditionContent.falseDo[k].opt)
                )
                {
                    continue;
                }
                DataJson.ConditionCase cas1 = new DataJson.ConditionCase();
         
                cas1.@case = "*";
                cas1.obj = logicConditionContent.falseDo[k].address;
                cas1.data = logicConditionContent.falseDo[k].opt;
                cas1.optname = logicConditionContent.falseDo[k].optName;
                cas1.delay = logicConditionContent.falseDo[k].delay;
                total.conditionCase.Add(cas1);
            }
            //当switch项内容不为空且 case的内容不为空
            if (total.conditionCase.Count > 0 && !string.IsNullOrEmpty(total.@switch))
            {
                lc.trigger.Add(total);
            }
            
        }

        /// <summary>
        /// 地址加权 a为乘 b为加
        /// </summary>
        /// <returns></returns>
        private string PowerAdd(int a,int b,string address)
        {
            string tmp = "";
            if (a == 1)
            {
                if (b == 0)
                {
                    //1 0
                    tmp = address;
                }
                else
                { 
                    //1 x
                    tmp = string.Format("({0}+{1})", address, b);
                }
            }
            else
            {
                if (b == 0)
                {
                    //0 1
                    tmp = "";
                }
                else
                {
                    //x x
                    tmp = string.Format("({0}*{1}+{2})",a, address, b);
                }
            }
            return tmp;

        }

        #endregion


        #region 表达式处理提取SWITCH和CASE

        private void VoiceDeal(DataJson.Lc lc, DataJson.logicsInfo LogicInfo)
        {
            
            DataJson.VoiceContent logicVoiceContent = JsonConvert.DeserializeObject<DataJson.VoiceContent>(LogicInfo.content);
            //场景数量不超50范围
            DataJson.TriggerNumber total = new DataJson.TriggerNumber();
            total.num = LogicInfo.id;
            total.attr = LogicInfo.attr;
            total.modelType = LogicInfo.modelType;

            //填写switch  
            if (string.IsNullOrEmpty(logicVoiceContent.voice))
            {
                //清空所有内容
                lc.trigger.Clear();
                return;
            }
            char[] voiceChar = logicVoiceContent.voice.ToCharArray();
    
          
            for(int i = 0;i < voiceChar.Length;i++)
            {
                if (Validator.IsEnglishCharacter(voiceChar[i].ToString()))
                {
                     DataJson.VoiceItem voiceitem = logicVoiceContent.voiceItem.Find(obj => obj.letter == voiceChar[i].ToString());
                     if (voiceitem == null || string.IsNullOrEmpty(voiceitem.address) || voiceitem.address == "FFFFFFFF")
                     {
                         //地址为空 直接退出
                         total.@switch = "";
                         break;
                     }
                     total.@switch = string.Format("{0}{1}", total.@switch, voiceitem.address);
                }
                else
                {
                    total.@switch = string.Format("{0}{1}", total.@switch , voiceChar[i]);
                }
            }
            

            //填写case
            if (string.IsNullOrEmpty(total.@switch))
            {
                //清空所有内容
                lc.trigger.Clear();
                return;
            }
            //表达式是否有赋值到局部变量或者虚拟变量
            if (!string.IsNullOrEmpty(logicVoiceContent.voiceGive))
            {
                DataJson.ConditionCase cas1 = new DataJson.ConditionCase();

                cas1.@case = "#";
                cas1.obj = logicVoiceContent.voiceGive;
                cas1.data = "FE000000";
                cas1.optname = "";
                cas1.delay = 0;
                total.conditionCase.Add(cas1);
            }

            //表达判断项
            for (int k = 0; k < logicVoiceContent.voiceIfItem.Count; k++)
            {
                if (string.IsNullOrEmpty(logicVoiceContent.voiceIfItem[k].result)
                    || string.IsNullOrEmpty(logicVoiceContent.voiceIfItem[k].address)
                    || logicVoiceContent.voiceIfItem[k].address == "FFFFFFFF"
                    || string.IsNullOrEmpty(logicVoiceContent.voiceIfItem[k].opt)

                )
                {
                    continue;
                }
                DataJson.ConditionCase cas1 = new DataJson.ConditionCase();

                cas1.@case = logicVoiceContent.voiceIfItem[k].result;
                cas1.obj = logicVoiceContent.voiceIfItem[k].address;
                cas1.data = logicVoiceContent.voiceIfItem[k].opt;
                cas1.optname = logicVoiceContent.voiceIfItem[k].optName;
                cas1.delay = logicVoiceContent.voiceIfItem[k].delay;
                total.conditionCase.Add(cas1);
            }
            //当switch项内容不为空且 case的内容不为空
            if (total.conditionCase.Count > 0 && !string.IsNullOrEmpty(total.@switch))
            {
                lc.trigger.Add(total);
            }


        }

        #endregion

        #endregion

        #endregion


        //end
    }
}
