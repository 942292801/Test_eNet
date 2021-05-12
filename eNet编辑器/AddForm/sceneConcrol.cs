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

namespace eNet编辑器.AddForm
{
    public partial class sceneConcrol : Form
    {
        public sceneConcrol()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 地址
        /// </summary>
        private string ip;

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        private string opt;

        public string Opt
        {
            get { return opt; }
            set { opt = value; }
        }

        private string ver;

        /// <summary>
        /// 操作名称
        /// </summary>
        public string Ver
        {
            get { return ver; }
            set { ver = value; }
        }
    
        private string objType;

        public string ObjType
        {
            get { return objType; }
            set { objType = value; }
        }
        
        //getObj用的point  只限equipment用
        private DataJson.PointInfo point = null;

        internal DataJson.PointInfo Point
        {
            get { return point; }
            set { point = value; }
        }

        /// <summary>
        /// 存放type.ini里面的commod信息
        /// </summary>
        private List<data> comdsList = new List<data>();
        class data
        {
            public string[] list { get; set; }
        }

        //存放当前dev表中读取到的 ID+设备
        private Dictionary<int, string> devDic;

        //链路类型 2=类型,设备,0    存放 设备 0
        private Dictionary<string, string> linkDic;


        private void sceneConcrol_Load(object sender, EventArgs e)
        {
            try
            {
                //CbVersion初始化 获取INI的COMMONS信息
                CbVersionIni();
                //初始化加载ini里面的链路类型
                LinkDicIni();
                //信息展示还原
                InfoFormat();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region 初始化 和 version更新

        /// <summary>
        ///  CbVersion初始化 获取INI的COMMONS信息
        /// </summary>
        private void CbVersionIni()
        {
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            //bool Flag = false;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                //找到类型一致
                if (type == objType)
                {
                    string tmp = "";
                    //循环读取command 1= 2= 3= 信息
                    for (int i = 1; i < 30; i++)
                    {
                        tmp = IniConfig.GetValue(file.FullName, "command", i.ToString());
                        if (tmp == "")
                        {
                            break;
                        }
                        data a = new data();
                        a.list = tmp.Split(',');
                        //把command命令存放到信息
                        comdsList.Add(a);
                    }
                    //2.1 cb1 加载 type.ini信息的头一个信息 
                    foreach (data comd in comdsList)
                    {
                        cbVersion.Items.Add(comd.list[0]);
                    }
                    cbVersion.Items.Add("赋值");
                    //默认加载cbversion第一个选项信息
                    cbVersion.SelectedIndex = 0;
                    break;
                }//类型一致

            }
        }

        /// <summary>
        /// 初始化加载ini里面的链路类型
        /// </summary>
        private void LinkDicIni()
        {
            linkDic = IniHelper.ReadTypesAddress2Info();
        }


        private void InfoFormat()
        {
            if (string.IsNullOrEmpty(ver) && string.IsNullOrEmpty(opt))
            {
                cbVersion.SelectedIndex = 0;
                return;
            }
            if (ver == "赋值")
            {
                FuzhiStyle();
            }
            else
            {
                CommonStyle();
            }

        }

        private void CommonStyle()
        {
            //是否为状态操作
            bool isStatusCtrl = true;
            for (int i = 0; i < cbVersion.Items.Count; i++)
            {
                if (cbVersion.Items[i].ToString() == ver)
                {
                    cbVersion.SelectedItem = cbVersion.Items[i];
                    isStatusCtrl = false;
                    break;
                }
            }
            if (isStatusCtrl)
            {
                cbVersion.SelectedItem = "状态操作";
                //状态操作
                cb1.Text = DataChange.HexStringToString(opt.Substring(0, 2));
                cb2.Text = DataChange.HexStringToString(opt.Substring(2, 2));
                cb3.Text = DataChange.HexStringToString(opt.Substring(4, 2));
                string tmp4 = DataChange.HexStringToString(opt.Substring(6, 2));
                for (int i = 0; i < cb4.Items.Count; i++)
                {
                    if (cb4.Items[i].ToString().Contains(tmp4))
                    {
                        cb4.SelectedItem = cb4.Items[i];
                        return;
                    }
                }
            }
            else
            {
                //普通commons指令
                string tmp1 = DataChange.HexStringToString(opt.Substring(0, 2));
                for (int j = 0; j < cb1.Items.Count; j++)
                {
                    if (cb1.Items[j].ToString().Contains(tmp1))
                    {
                        cb1.Text = cb1.Items[j].ToString();
                        break;
                    }
                }
                string tmp2 = DataChange.HexStringToString(opt.Substring(2, 2));
                for (int j = 0; j < cb2.Items.Count; j++)
                {
                    if (cb2.Items[j].ToString().Contains(tmp2))
                    {
                        cb2.Text = cb2.Items[j].ToString();
                        break;
                    }
                }
                string tmp3 = DataChange.HexStringToString(opt.Substring(4, 2));
                for (int j = 0; j < cb3.Items.Count; j++)
                {
                    if (cb3.Items[j].ToString().Contains(tmp3))
                    {
                        cb3.Text = cb3.Items[j].ToString();
                        break;
                    }
                }
                string tmp4 = DataChange.HexStringToString(opt.Substring(6, 2));
                for (int j = 0; j < cb4.Items.Count; j++)
                {
                    if (cb4.Items[j].ToString().Contains(tmp4))
                    {
                        cb4.Text = cb4.Items[j].ToString();
                        break;
                    }
                }

            }
            
        }

        /// <summary>
        /// 赋值地址还原
        /// </summary>
        private void FuzhiStyle()
        {
            //选中赋值
            cbVersion.SelectedIndex = cbVersion.Items.Count-1;
            //十进制信息内容
            string typeval = DataChange.HexStringToString(opt.Substring(2, 2));
            string id = DataChange.HexStringToString(opt.Substring(4, 2));
            string port = DataChange.HexStringToString(opt.Substring(6, 2));
            foreach (var item in linkDic)
            {
                if (item.Value == typeval)
                {
                    cb2.SelectedItem = item.Key;
                    break;
                }
            }
            switch (linkDic[cb2.SelectedItem.ToString()])
            {
                //设备  面板按键 感应输入
                case "0":
                case "248":
                case "250":
                    //addNumForDevList();
                    cb3.Text = id;
                    cb4.Text = port;
                    break;

                //场景 定时 面板感应  逻辑
                case "16":
                case "32":
                case "48":
                case "64":
                    cb4.Text = (Convert.ToInt32(id) * 256 + Convert.ToInt32(port)).ToString();
                    break;

                //虚拟端口
                case "251":
                    cb3.Text = id;
                    cb4.Text = port;
                    cb3.Enabled = false;
                    break;
            }
            
      

        }


      

        #endregion

        #region  Version选项改变
        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            VersionChange();
        }

        /// <summary>
        /// Version选项改变时 更新全部的lb信息 和cb信息
        /// </summary>
        /// <param name="indexs">version索引</param>
        private void VersionChange()
        {
            int indexs = cbVersion.SelectedIndex;
            if (comdsList.Count > indexs)
            {
                cb1.DropDownStyle = ComboBoxStyle.DropDown;
                //正常加载type.ini的command命令
                //2.2 cb2-3 lb1-4按读取分割的顺序添加 信息 
                lb1.Text = comdsList[indexs].list[1];
                lb2.Text = comdsList[indexs].list[3];
                lb3.Text = comdsList[indexs].list[5];
                lb4.Text = comdsList[indexs].list[7];
                //2.3 cb2-3 的内容需要判断 1-100 表示有1-100个内容选择 enable打开
                CommondsDeal(cb1, comdsList[indexs].list[2]);
                CommondsDeal(cb2, comdsList[indexs].list[4]);
                CommondsDeal(cb3, comdsList[indexs].list[6]);
                CommondsDeal(cb4, comdsList[indexs].list[8]);
            }
            else
            {
                ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                //清除信息
                for (int i = 0; i < 4; i++)
                {
                    cbs[i].Items.Clear();
                    cbs[i].Enabled = true;
                    cbs[i].Text = "";
                }
                cb1.Items.Clear();
                cb1.Items.Add(ip);
                cb1.DropDownStyle = ComboBoxStyle.DropDownList;
                if (cb1.Items.Count > 0)
                {
                    cb1.SelectedIndex = 0;

                }
                lb1.Text = "网关";
                foreach (var item in linkDic)
                {
                    cb2.Items.Add(item.Key);

                }
                if (cb2.Items.Count > 0)
                {
                    cb2.SelectedIndex = 0;

                }
                lb2.Text = "类型";

            }
        }

        #region commom命令处理
        /// <summary>
        /// commom命令处理 
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void CommondsDeal(ComboBox cb, string info)
        {

            cb.Items.Clear();
            cb.Enabled = true;
            if (info.Contains("\\"))
            {
                dealXiegan(cb, info);
            }
            else if (info.Contains(":"))
            {
                dealMaohao(cb, info);
            }
            else if (info.Contains("-"))
            {
                dealHenggan(cb, info);
            }
            else if (info == "getObj")
            {
                if (point != null && !string.IsNullOrEmpty(point.objType) && !string.IsNullOrEmpty(point.value))
                {
                    dealGetObj(cb, info);

                }
                else
                {
                    cb.Items.Add("点位：对象和参数未赋值 ");
                    cb.Enabled = false;
                }

            }
            else
            {
                cb.Items.Add(info);
                cb.Enabled = false;
            }
            try
            {
                //初始化索引
                cb.SelectedIndex = 0;
            }
            catch
            {
            }


        }

        /// <summary>
        /// 斜杆
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealXiegan(ComboBox cb, string info)
        {
            string[] parents = info.Split('\\');
            for (int l = 0; l < parents.Length; l++)
            {
                if (parents[l].Contains(':'))
                {
                    dealMaohao(cb, parents[l]);
                }
                else if (parents[l].Contains('-'))
                {
                    dealHenggan(cb, parents[l]);
                }
                else
                {
                    cb.Items.Add(parents[l]);
                }
            }
        }

        /// <summary>
        /// 冒号
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealMaohao(ComboBox cb, string info)
        {
            string[] parents = info.Split(':');
            if (parents[0].Contains("-"))
            {
                string[] child = parents[0].Split('-');
                int j = Convert.ToInt32(child[1]);
                for (int i = Convert.ToInt32(child[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString() + parents[1]);
                }
            }
            else
            {
                cb.Items.Add(info);
            }

        }

        /// <summary>
        /// 横杆
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealHenggan(ComboBox cb, string info)
        {
            string[] child = info.Split('-');
            int j = Convert.ToInt32(child[1]);
            if (j > 100)
            {
                j = 100;
            }
            for (int i = Convert.ToInt32(child[0]); i <= j; i++)
            {
                cb.Items.Add(i.ToString());
            }
        }

        private void dealGetObj(ComboBox cb, string info)
        {
            string filepath = string.Format("{0}//objs//{1}.ini", Application.StartupPath, point.objType);
            List<string> keys = IniConfig.ReadKeys(point.value, filepath);

            for (int i = 2; i < keys.Count; i++)
            {
                CommondsDeal(cb, IniConfig.GetValue(filepath, point.value, keys[i]));
            }
        }

        #endregion

        #endregion

        #region CB2选项改变
        private void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVersion.SelectedIndex < comdsList.Count)
            {
                //选中不为赋值
                return;
            }
            GetStyleByType();
        }

        /// <summary>
        /// 根据类型获取cb3 cb4的样式
        /// </summary>
        private void GetStyleByType()
        {
            string cbType = cb2.SelectedItem.ToString();
            if (string.IsNullOrEmpty(cbType))
            {
                return;
            }
            string inipath = IniHelper.FindPathByAddressType(cbType);
            if (string.IsNullOrEmpty(inipath))
            {
                return;
            }
            string[] address_3 = IniConfig.GetValue(inipath, "address", "3").Split(',');

            switch (linkDic[cbType])
            {
                //设备  面板按键 感应输入
                case "0":
                case "248":
                case "250":
                    lb3.Text = address_3[0];
                    ToolsUtil.CBDealNumFormat(cb3, address_3[1]);
                    cb3.Enabled = true;
                    addNumForDevList();
                    break;
                //场景 定时 面板 感应编组  逻辑
                case "16":
                case "32":
                case "48":
                case "64":
                    lb3.Text = "";
                    cb3.Text = "";
                    cb3.Enabled = false;
                    break;
                //虚拟端口
                case "251":
                    lb3.Text = address_3[0];
                    cb3.Text = address_3[1];
                    cb3.Enabled = false;
                    break;
            }

            string[] address_4 = IniConfig.GetValue(inipath, "address", "4").Split(',');
            lb4.Text = address_4[0];
            ToolsUtil.CBDealNumFormat(cb4, address_4[1]);
            FindTotalNum();
            if (cb3.Items.Count > 0)
            {
                cb3.SelectedIndex = 0;
            }
        }
        #endregion

        #region CB3选项改变
        private void cb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVersion.SelectedIndex < comdsList.Count)
            {
                //选中不为赋值
                return;
            }
            FindTotalNum();
          

        }

        #region 搜索获取工程真实的设备号数

        /// <summary>
        /// 加载网关的设备到cb3里面
        /// </summary>
        private void addNumForDevList()
        {
            devDic = new Dictionary<int, string>();
            devDic.Clear();
            cb3.Items.Clear();
            foreach (DataJson.Device devip in FileMesege.DeviceList)
            {
                if (devip.ip == ip)
                {
                    foreach (DataJson.Module md in devip.module)
                    {
                        if (!devDic.ContainsKey(md.id))
                        {
                            devDic.Add(md.id, md.device);

                        }
                        cb3.Items.Add(md.id);
                    }

                    break;
                }
            }
        }




        /// <summary>
        /// 根据选中的信息查找端口号 或者 场景 定时的号码
        /// </summary>
        private void FindTotalNum()
        {
            if (cb2.SelectedItem == null)
            {
                return;
            }
            cb4.Items.Clear();
            cb4.Text = string.Empty;
            switch (linkDic[cb2.SelectedItem.ToString()])
            {
                //设备  面板按键 感应输入
                case "0":
                case "248":
                case "250":
                    if (cb3.SelectedItem != null)
                    {
                        findPort(Convert.ToInt32(cb3.Text));

                    }
                    break;

                //场景 定时 面板 感应编组  逻辑
                case "16":
                case "32":
                case "48":
                case "64":
                    findNum();
                    break;

                //虚拟端口
                case "251":
                    findVarNum();
                    cb3.Enabled = false;
                    break;
            }
            if (cb4.Items.Count > 0)
            {
                cb4.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// 寻找该设备的端口数目
        /// </summary>
        /// <param name="id"></param>
        private void findPort(int id)
        {
            try
            {
                foreach (var item in devDic)
                {
                    if (item.Key == id)
                    {
                        //string filepath = IniHelper.findDevicesDisplay(item.Value);
                        string filepath = Application.StartupPath + "//devices//" + item.Value + ".ini";
                        //获取全部Section下的Key
                        List<string> list = IniConfig.ReadKeys("ports", filepath);
                        //循环添加行信息
                        for (int j = 0; j < list.Count; j++)
                        {
                            cb4.Items.Add(list[j]);
                        }
                        return;
                    }
                }

            }
            catch
            {

            }


        }

        /// <summary>
        /// 查找当前ip的存在场景 定时 面板 号
        /// </summary>
        private void findNum()
        {
            try
            {
                //////////////////////////////////后期还需要添加
                switch (cb2.SelectedItem.ToString())
                {
                    case "场景":
                        DataJson.Scene scene = DataListHelper.getSceneList(cb1.Text);
                        foreach (DataJson.scenes scenes in scene.scenes)
                        {
                            cb4.Items.Add(scenes.id);
                        }
                        break;
                    case "定时":
                        DataJson.Timer timer = DataListHelper.getTimerList(ip);

                        foreach (DataJson.timers tms in timer.timers)
                        {
                            cb4.Items.Add(tms.id);
                        }
                        break;
                    case "面板":
                        DataJson.Panel panel = DataListHelper.getPanelList(ip);

                        foreach (DataJson.panels pls in panel.panels)
                        {
                            cb4.Items.Add(pls.id);
                        }
                        break;
                    case "感应编组":
                        DataJson.Sensor sensor = DataListHelper.getSensorList(ip);

                        foreach (DataJson.sensors srs in sensor.sensors)
                        {
                            cb4.Items.Add(srs.id);
                        }
                        break;
                    case "逻辑":
                        DataJson.Logic logic = DataListHelper.getLogicList(ip);

                        foreach (DataJson.logics lgs in logic.logics)
                        {
                            cb4.Items.Add(lgs.id);
                        }
                        break;

                    default:
                        break;
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

        #endregion

        #endregion



        #region 确认
        private void btnDecid_Click(object sender, EventArgs e)
        {
            try
            {
                if (comdsList.Count > cbVersion.SelectedIndex)
                {
                    string cb1Num = dealNum(cb1.Text);
                    string cb2Num = dealNum(cb2.Text);
                    string cb3Num = dealNum(cb3.Text);
                    string cb4Num = dealNum(cb4.Text);
                    if (string.IsNullOrEmpty(cb1Num) || string.IsNullOrEmpty(cb2Num) || string.IsNullOrEmpty(cb3Num) || string.IsNullOrEmpty(cb4Num))
                    {
                        this.DialogResult = DialogResult.No;
                        return;
                    }
                    this.opt = ToolsUtil.strtohexstr(cb1Num) + ToolsUtil.strtohexstr(cb2Num) + ToolsUtil.strtohexstr(cb3Num) + ToolsUtil.strtohexstr(cb4Num);
                    if (cb4.Text.Split(':').Length > 1)
                    {
                        if (cbVersion.Text == "状态操作")
                        {
                            //当为getObj的操作的时候
                            this.ver = cb4.Text.Split(':')[1];
                        }
                        else
                        {
                            this.ver = cbVersion.Text;
                        }

                    }
                    else
                    {

                        this.ver = cbVersion.Text;
                    }
                }
                else
                {
                    //赋值
                    string newobj = "";
                    if (cb1.Text != "" && cb2.Text != "")
                    {
                        foreach (var item in linkDic)
                        {
                            if (cb2.Text == item.Key)
                            {
                                newobj = "FE" + ToolsUtil.strtohexstr(item.Value);
                                break;
                            }

                        }
                       /* for (int i = 0; i < TypeList.Count; i++)
                        {
                            if (TypeList[i].Split(',')[1] == cb2.Text)
                            {
                                newobj = ToolsUtil.strtohexstr(cb1.Text) + ToolsUtil.strtohexstr(TypeList[i].Split(',')[2]);
                                break;
                            }
                        }*/

                        if (cb3.Text != "" && cb4.Text != "")
                        {
                            //设备
                            newobj = newobj + ToolsUtil.strtohexstr(cb3.Text) + ToolsUtil.strtohexstr(cb4.Text);
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
                    if (newobj.Length != 8)
                    {

                        this.DialogResult = DialogResult.No;
                        return;
                    }
                    this.opt = newobj;


                    this.ver = cbVersion.Text;


                }
                if (string.IsNullOrEmpty(this.ver))
                {
                    this.DialogResult = DialogResult.No;
                    return;
                }
                this.DialogResult = DialogResult.OK;

            }
            catch
            {
                this.opt = "";
                this.ver = "";
                this.DialogResult = DialogResult.No;
            }
    
        }

        /// <summary>
        /// 提取 21秒 或 255 ：0.1秒 前面的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string dealNum(string str)
        {
            string tmp = "";
            if (str.Contains(":"))
            {
                tmp = Regex.Replace(str.Split(':')[0], @"[^\d]*", "");
            }
            else
            {
                tmp = Regex.Replace(str, @"[^\d]*", "");
            }
            return tmp;
        }

        #endregion



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
        private void sceneConcrol_Paint(object sender, PaintEventArgs e)
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
