using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using eNet编辑器.Properties;

namespace eNet编辑器.LogicForm
{
    public partial class LogicCompareAddress : Form
    {
        public LogicCompareAddress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 传入地址 十六进制
        /// </summary>
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }



        //英语对象类型
        private string objType;

        public string ObjType
        {
            get { return objType; }
            set { objType = value; }
        }

        
        //英文比较对象类型
        private string compareType;

        public string CompareType
        {
            get { return compareType; }
            set { compareType = value; }
        }

        /// <summary>
        /// 返回 类型（英文4.0_scene）
        /// </summary>
        private string rtType;

        public string RtType
        {
            get { return rtType; }
            set { rtType = value; }
        }

        /// <summary>
        /// 存放当前dev表中读取到的 ID+设备
        /// </summary>
        List<string> devName = new List<string>();

        private string ip = "";


        /// <summary>
        /// types 下data的每一项数据内容
        /// </summary>
        List<string> Keylist = new List<string>();

        /// <summary>
        /// types里 data的数据段 1-8所有信息
        /// </summary>
        private List<DataInfo> dataList = new List<DataInfo>();

        public class DataInfo
        {
            public string readWrite { get; set; }
            public string dataType { get; set; }
            public string dataBitInset { get; set; }//区域
            public string dataValScope { get; set; }//范围
            public string dataName { get; set; }
        }

        /// <summary>
        /// types里 data的format发送格式
        /// </summary>
        private string format = "";

        string objTypeCNName = "";

        /// <summary>
        /// address下 2 的内容（ 类型，xx,0 ）
        /// </summary>
        List<string> TypeList = null;

        LinkType linkType = LinkType.Dev;

        /// <summary>
        /// 链路的类型
        /// </summary>
        public enum LinkType
        {
            /// <summary>
            /// 设备类型
            /// </summary>
            Dev = 1,
            /// <summary>
            /// 虚拟端口类型
            /// </summary>
            Var = 2,
            /// <summary>
            /// 场景 定时 面板 类型
            /// </summary>
            Com = 3,
            /// <summary>
            /// 局部变量
            /// </summary>
            localVar=4

        }


        private void LogicCompareAddress_Load(object sender, EventArgs e)
        {
            try
            {
                //读取Address类型
                ReadIniAddressType();
                //读取IP
                GetIp();
                //读取data下的信息
                GetStateConten();
                cbVersion.Items.Add(Resources.StateControl);
                cbVersion.Items.Add(Resources.Give);
                cbVersion.SelectedIndex = 0;

                

                //数据还原
                if (!string.IsNullOrEmpty(address))
                {
                    Label[] lbs = { lb1, lb2, lb3, lb4 };
                    ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                   
                    if(address.Substring(0,2) == "FE"&& !string.IsNullOrEmpty(compareType))
                    {
                        //状态操作
                        cbVersion.SelectedIndex = 1;
                        typeChange(IniHelper.findIniLinkTypeByAddress(address));
                        //地址开头 且类型不为空  则为赋值
                        string[] infos = DgvMesege.addressTransform(address).Split('.');
                        if (linkType == LinkType.Com)
                        {
                            //不为设备类型
                            cb1.Text = infos[0];
                            //将超256 的号数操作
                            cb4.Text = (Convert.ToInt32(infos[2]) * 256 + Convert.ToInt32(infos[3])).ToString();
                            findComNum();
                        }
                        else if (linkType == LinkType.Dev)
                        {
                            addNumForDevList();
                            cb1.Text = infos[0];
                            cb3.Text = infos[2];
                            cb4.Text = infos[3];
                            findPort(infos[2]);
                        }
                        else if (linkType == LinkType.Var)
                        {
                            //虚拟端口
                            cb1.Text = infos[0];
                            cb3.Text = infos[2];
                            cb4.Text = infos[3];
                            findVarNum();
                            cb3.Enabled = false;
                        }
                        else if (linkType == LinkType.localVar)
                        {
                            //局部变量
                            cb1.Text = infos[0];
                            int localvarNum = Convert.ToInt32(infos[2]) * 256 + Convert.ToInt32(infos[3]);
                            double tmp = Math.Ceiling((double)localvarNum / 8);
                            cb3.Text = Convert.ToInt32(tmp).ToString();
                            //将超256 的号数操作
                            cb4.Text = localvarNum.ToString();
                        }
                    }
                    else
                    {
                        //赋值
                        //地址转换为二进制
                        string binVal = DataChange.HexString2BinString(address).Replace(" ", "");
                        //获取中文类型名
                        objTypeCNName = IniHelper.findTypesIniNamebyType(ObjType);
                        for (int i = 0; i < 4; i++)
                        {
     
                            foreach (DataInfo item in dataList)
                            {
                                //找到名称相同 且 为可写
                                if (item.dataName == lbs[i].Text)// && item.readWrite.Contains("w"))
                                {
                                    
                                    string val = DataChange.getBinBit(binVal, item.dataBitInset);
                                    //时间特殊处理
                                    if (objTypeCNName == Resources.Date || objTypeCNName == Resources.Time)
                                    {
                                        if (val == "255")
                                        {
                                            cbs[i].Text = Resources.Ignore;
                                            break;
                                        }
                                     
                                    }
                                
                                        //对应的选择框值 替换 相对应的位置
                                        cbs[i].Text = val;
                                    
                                    
                                    break;
                                }
                            }
                            
                        }
                    }
                }
                 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
        }

        #region 读取Address所有类型 和 获取当前IP
        /// <summary>
        /// 读取ini里面的Address类型 设备场景。。。
        /// </summary>
        private void ReadIniAddressType()
        {
            //记录全部cb2的类型

            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");

            HashSet<string> nameList = new HashSet<string>();
            string tmp = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                tmp = IniConfig.GetValue(file.FullName, "address", "2");
                if (!string.IsNullOrEmpty(tmp))
                {
                    nameList.Add(tmp);
                }

            }
            TypeList = nameList.ToList<string>();
           
        
        }

        /// <summary>
        /// 读取网关IP 
        /// </summary>
        /// <param name="selectSumNode">选中子节点</param>
        private void GetIp()
        {

            try
            {
                switch (FileMesege.formType)
                {
                    ////////////////////////////////////还需要后续添加选中节点参数
                    case "name":

                        break;

                    case "point":
                        break;
                    case "scene":
                        ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "timer":
                        ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "panel":
                        ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "sensor":
                        ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "logic":
                        ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
                        break;

                    default: break;
                }


            }
            catch
            {

            }

        }

        /// <summary>
        /// 读取data下的信息
        /// </summary>
        private void GetStateConten()
        {
            if (string.IsNullOrEmpty(objType))
            {
                return;
            }
            string filepath = string.Format("{0}//types//{1}.ini", Application.StartupPath, objType);
            //获取全部Section下的Key
            Keylist = IniConfig.ReadKeys("data", filepath);
            //顺便 读取format 数据格式
            format = IniConfig.GetValue(filepath, "data", "format").Replace(" ","");
            string[] infos = null;
            for (int i = 0; i < Keylist.Count; i++)
            {
                //获取类型下data的数据  rw,uint8,0-1,0-1,亮度(%)
                infos = IniConfig.GetValue(filepath, "data", Keylist[i]).Split(',');
                //读取的value的格式不规范 直接退出不作处理 排除format
                if (infos.Length != 5)
                {
                    continue;
                }
                DataInfo dataItem = new DataInfo();
                dataItem.readWrite = infos[0];
                dataItem.dataType = infos[1];
                dataItem.dataBitInset = infos[2];
                dataItem.dataValScope = infos[3];
                dataItem.dataName = infos[4];
                dataList.Add(dataItem);


            }
        }

        #endregion

        #region 总栏 状态操作或者赋值
        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearForm();
            if (cbVersion.SelectedIndex == 0)
            {
                //状态内容
                StateMode();
            }
            else
            {
                //赋值
                GiveMode();
            }
        }

        /// <summary>
        /// 清空界面
        /// </summary>
        private void clearForm()
        {
            Label[] lbs = { lb1, lb2, lb3, lb4 };
            ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
            for (int i = 0; i < 4; i++)
            {
                lbs[i].Text = "";
                cbs[i].Text = "";
                cbs[i].Items.Clear();
                cbs[i].Enabled = true;
            }
        }

        
        /// <summary>
        /// 状态界面
        /// </summary>
        private void StateMode()
        {
            Label[] lbs = { lb1,lb2,lb3,lb4};
            ComboBox[] cbs = {cb1,cb2,cb3,cb4 };
            for (int i = 0; i < 4; i++)
            {
                if (i < dataList.Count)
                {
                    lbs[i].Text = dataList[i].dataName;
                    //解析取值区域范围
                    DataChange.dealInfoNum(cbs[i], dataList[i].dataValScope);
                    if (string.IsNullOrEmpty(objTypeCNName))
                    {
                        //获取中文类型名
                        objTypeCNName = IniHelper.findTypesIniNamebyType(ObjType);
                    }
                    //时间日期特殊处理
                    if (objTypeCNName == Resources.Date || objTypeCNName == Resources.Time)
                    {
                        cbs[i].Items.Add(Resources.Ignore);
                    }
                }
                else
                {
                    cbs[i].Enabled = false;
                
                }
            }
         
       
        }

        private void GiveMode()
        {
           
            for (int i = 0; i < TypeList.Count; i++)
            {
                if (TypeList[i].Split(',')[1] == "按键" || TypeList[i].Split(',')[1] == "感应输入")
                {
                    continue;
                }
                cb2.Items.Add(TypeList[i].Split(',')[1]);
            }
            cb2.Items.Add("局部变量");
            //这里选择类型有问题
            cb2.Text = objType;
            cb2.SelectedIndex = 0;

        }
        #endregion

        #region 类型框改变
        private void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVersion.SelectedIndex == 0)
            {
                return;
            }
            else
            {
                typeChange(cb2.Text);
                if (linkType == LinkType.Dev)
                {
                    addNumForDevList();

                }
                else if (linkType == LinkType.Com)
                {
                    findComNum();
                }
                else if (linkType == LinkType.Var)
                {
                    findVarNum();
                }
            }


        }

        /// <summary>
        /// 当cb2类型改变cb3 cb4的格式  TypeList的格式
        /// </summary>
        /// <param name="cbType"></param>
        private void typeChange(string cbType)
        {
            if (string.IsNullOrEmpty(cbType))
            {
                return;
            }
            if (cbType == Resources.LocalVar)
            {
                localVarMode();
                return;
            }
  

            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            string tmp = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                tmp = IniConfig.GetValue(file.FullName, "address", "2");
                if (string.IsNullOrEmpty(tmp))
                {
                    continue;
                }
                type = tmp.Split(',')[1];
                //找到类型一致
                if (type == cbType)
                {
                    //记录当前类型 场景 定时 面板 人感 编组
                    rtType = file.Name.Replace(".ini", "");
                    //读取【address】1-4信息
                    string[] infos = IniConfig.GetValue(file.FullName, "address", "1").Split(',');
                    cb1.Items.Clear();
                    lb1.Text = infos[0];
                    cb1.Text = infos[1];
                    cb1.Items.Add(infos[1]);
                    infos = IniConfig.GetValue(file.FullName, "address", "2").Split(',');
                    lb2.Text = infos[0];
                    cb2.Text = infos[1];
                    infos = IniConfig.GetValue(file.FullName, "address", "3").Split(',');
                    if (infos[0] == "3&4")
                    {
                        //场景
                        lb3.Text = "";
                        cb3.Text = "";
                        cb3.Enabled = false;
                        linkType = LinkType.Com;
                    }
                    else if (infos[0] == Resources.Device)
                    {
                        //设备
                        lb3.Text = infos[0];
                        //cb3.Text = infos[1];
                        DataChange.dealInfoNum(cb3, infos[1]);
                        cb3.Enabled = true;
                        linkType = LinkType.Dev;
                    }
                    else if (infos[0] == Resources.Fixation)
                    {
                        //虚拟端口
                        lb3.Text = infos[0];
                        cb3.Text = infos[1];
                        cb3.Enabled = false;
                        linkType = LinkType.Var;

                    }
                    infos = IniConfig.GetValue(file.FullName, "address", "4").Split(',');
                    lb4.Text = infos[0];
                    cb4.Text = "";
                    DataChange.dealInfoNum(cb4, infos[1]);
                    cb4.Enabled = true;


                    break;
                }
                //不存在该个类型
                rtType = "";
            }///foreach 

        }


        #endregion



        private void cb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVersion.SelectedIndex == 0)
            {
                return;
            }
            else
            {
                if (cb3.Enabled && linkType == LinkType.Dev)
                {
                    findPort(cb3.Text);
                }
                else if (linkType == LinkType.localVar)
                {
                    findLogicNum(cb3.Text);
                }
            }
             
            
        }

        #region 局部变量模式
        /// <summary>
        /// 窗体框信息更新为局部变量信息
        /// </summary>
        private void localVarMode()
        {
            try
            {
                linkType = LinkType.localVar;
                lb3.Text = Resources.LogicNum;
                lb4.Text = Resources.LocalVarNum;
                cb2.Text = Resources.LocalVar;
                cb3.Text = "";
                cb4.Text = "";
                cb3.Enabled = true;
                cb4.Enabled = true;
                rtType = "15.0_LocalVariable";
                cb3.Items.Clear();
                foreach (DataJson.Logic lg in FileMesege.logicList)
                {
                    //  添加该网关IP的子节点
                    if (lg.IP == ip)
                    {
                        foreach (DataJson.logics lgs in lg.logics)
                        {
                            cb3.Items.Add(lgs.id);

                        }

                    }
                }

            }
            catch
            {
            }
        }

        #endregion

        #region 搜索加载文本的设备号 场景号 虚拟端口号 局部变量号等

        /// <summary>
        /// 加载网关的设备到cb3里面
        /// </summary>
        private void addNumForDevList()
        {
            foreach (DataJson.Device devip in FileMesege.DeviceList)
            {
                if (devip.ip == ip)
                {
                    cb3.Items.Clear();
                    foreach (DataJson.Module md in devip.module)
                    {
                        devName.Add(md.id + " " + md.device);
                        cb3.Items.Add(md.id);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 寻找该设备的端口数目
        /// </summary>
        /// <param name="id"></param>
        private void findPort(string id)
        {
            if (devName == null)
            {
                return;
            }
            for (int i = 0; i < devName.Count; i++)
            {
                if (devName[i].Split(' ')[0] == id)
                {
                    string filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, devName[i].Split(' ')[1]);
                    //获取全部Section下的Key
                    List<string> list = IniConfig.ReadKeys("ports", filepath);
                    cb4.Items.Clear();
                    //循环添加行信息
                    for (int j = 0; j < list.Count; j++)
                    {

                        cb4.Items.Add(list[j]);
                    }
                    break;
                }
            }

        }

        /// <summary>
        /// 查找当前ip的存在场景 定时 面板 号
        /// </summary>
        private void findComNum()
        {
            try
            {
                //后期还需完整
                cb4.Items.Clear();
                if (cb2.Text == Resources.Scene)
                {
                    string tmpip = ip;
                    if (cb1.Text != "254")
                    {
                        string[] ips = ip.Split('.');
                        tmpip = string.Format("{0}.{1}.{2}.{3}", ips[0], ips[1], ips[2], cb1.Text);
                    }
                    DataJson.Scene scene = DataListHelper.getSceneList(tmpip);

                    foreach (DataJson.scenes scenes in scene.scenes)
                    {
                        cb4.Items.Add(scenes.id);
                    }
                }
                else if (cb2.Text == Resources.Timer)
                {
                    DataJson.Timer timer = DataListHelper.getTimerList(ip);

                    foreach (DataJson.timers tms in timer.timers)
                    {
                        cb4.Items.Add(tms.id);
                    }
                }
                else if (cb2.Text == Resources.Panel)
                {
                    DataJson.Panel panel = DataListHelper.getPanelList(ip);

                    foreach (DataJson.panels pls in panel.panels)
                    {
                        cb4.Items.Add(pls.id);
                    }
                }
                else if (cb2.Text == Resources.Sensor)
                {
                    DataJson.Sensor sensor = DataListHelper.getSensorList(ip);

                    foreach (DataJson.sensors srs in sensor.sensors)
                    {
                        cb4.Items.Add(srs.id);
                    }
                }
                else if (cb2.Text == Resources.Logic)
                {
                    DataJson.Logic logic = DataListHelper.getLogicList(ip);

                    foreach (DataJson.logics lgs in logic.logics)
                    {
                        cb4.Items.Add(lgs.id);
                    }
                }
                else if (cb2.Text == Resources.LocalVar)
                {
                    DataJson.Logic logic = DataListHelper.getLogicList(ip);

                    foreach (DataJson.logics lgs in logic.logics)
                    {
                        for (int i = (lgs.id - 1) * 16 + 1; i <= lgs.id * 16; i++)
                        {
                            cb4.Items.Add(i);
                        }

                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 查找当前ip的存在虚拟端口
        /// </summary>
        private void findVarNum()
        {
            try
            {
                cb4.Items.Clear();
                foreach (DataJson.PointInfo point in FileMesege.PointList.virtualport)
                {
                    if (point.ip == ip)
                    {

                        cb4.Items.Add(Convert.ToInt32(point.address.Substring(6, 2), 16));
                    }
                }

            }
            catch { }
        }

        /// <summary>
        /// 局部变量 加载逻辑号
        /// </summary>
        /// <param name="logicID"></param>
        private void findLogicNum(string logicID)
        {
            int id = Validator.GetNumber(logicID);
            if (id == -1)
            {
                return;
            }
            cb4.Items.Clear();
            for (int i = (id - 1) * 8 + 1; i <= id * 8; i++)
            {
                cb4.Items.Add(i);
            }
        }

        #endregion



        private void btnDecid_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbVersion.SelectedIndex == 0)
                {
                    //状态操作
                    DecidState();
                }
                else
                {
                    //赋值
                    DecidGive();
                }
            }
            catch 
            { this.DialogResult = DialogResult.No; }
        }

        /// <summary>
        /// 状态值
        /// </summary>
        private void DecidState()
        {
            if (string.IsNullOrEmpty(format))
            {
                //组合信息为空
                this.DialogResult = DialogResult.No;
                return;
            }
            string content = format;
            //是否有修改了有效信息
            bool isContent = false;
            Label[] lbs = { lb1, lb2, lb3, lb4 };
            ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
            for (int i = 0; i < 4; i++)
            {
                //当信息不为空
                if (!string.IsNullOrEmpty(cbs[i].Text))
                {
                    foreach (DataInfo item in dataList)
                    {
                        //找到名称相同 且 为可写
                        if (item.dataName == lbs[i].Text )// && item.readWrite.Contains("w"))
                        {
                            if (cbs[i].Text == Resources.Ignore)
                            {
                                //对应的选择框值 替换 相对应的位置
                                content = DataChange.replaceStr(content, "255", item.dataBitInset);
                            }
                            else
                            {
                                //对应的选择框值 替换 相对应的位置
                                content = DataChange.replaceStr(content, cbs[i].Text, item.dataBitInset);
                            }
                            
                            isContent = true;
                            break;
                        }
                    }
                }
            }
            if (isContent)
            {
                this.address = content;
                this.DialogResult = DialogResult.OK;
                return;
            }
            else
            {
                this.DialogResult = DialogResult.No;
                return;
            }
        
        }

        /// <summary>
        /// 赋值
        /// </summary>
        private void DecidGive()
        {
            string newobj = "";
            if (cb1.Text != "" && cb2.Text != "")
            {

                for (int i = 0; i < TypeList.Count; i++)
                {
                    //获取类型的编码
                    if (TypeList[i].Split(',')[1] == cb2.Text)
                    {
                        newobj = ToolsUtil.strtohexstr(cb1.Text) + ToolsUtil.strtohexstr(TypeList[i].Split(',')[2]);
                        break;
                    }
                }

                if (cb3.Text != "" && cb4.Text != "")
                {
                    if (cb2.Text == Resources.LocalVar)
                    {
                        //局部变量
                        string tmp = ToolsUtil.strtohexstr(cb4.Text);
                        while (tmp.Length < 4)
                        {
                            tmp = tmp.Insert(0, "0");
                        }
                        newobj = ToolsUtil.strtohexstr(cb1.Text) + ToolsUtil.strtohexstr("249") + tmp;
                    }
                    else
                    {
                        //设备
                        newobj = newobj + ToolsUtil.strtohexstr(cb3.Text) + ToolsUtil.strtohexstr(cb4.Text);
                    }

                }
                else if (cb3.Text == "" && cb4.Text != "")
                {
                    //非设备类
                    string tmp = ToolsUtil.strtohexstr(cb4.Text);
                    while (tmp.Length < 4)
                    {
                        tmp = tmp.Insert(0, "0");
                    }
                    newobj = newobj + tmp;
                }
            }
            if (newobj.Length == 8)
            {
                this.address = newobj;
                this.DialogResult = DialogResult.OK;
                return;
            }
            this.DialogResult = DialogResult.No;
        }

        #region 窗体样色


        #region 窗体样色2
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        #endregion
        private void LogicCompareAddress_Paint(object sender, PaintEventArgs e)
        {
            
            Rectangle myRectangle = new Rectangle(0, 0, this.Width, this.Height);
            //ControlPaint.DrawBorder(e.Graphics, myRectangle, Color.Blue, ButtonBorderStyle.Solid);//画个边框 
            ControlPaint.DrawBorder(e.Graphics, myRectangle,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid
            );
        }

        private Point mPoint;

    

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void plInfoTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void plInfoTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        #endregion



     

        

      
    }
}
