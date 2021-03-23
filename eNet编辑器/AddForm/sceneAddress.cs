using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace eNet编辑器.AddForm
{
    public partial class sceneAddress : Form
    {
        public sceneAddress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选择按钮的地址
        /// </summary>
        private string ip;

        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        /// <summary>
        /// 传入地址
        /// </summary>
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }


        private string nowType;

        /// <summary>
        /// 对象类型（中文）
        /// </summary>
        public string NowType
        {
            get { return nowType; }
            set { nowType = value; }
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



        //存放当前dev表中读取到的 ID+设备
        private Dictionary<int, string> devDic;

        //链路类型 2=类型,设备,0 
        private Dictionary<string, string> linkDic;

        private bool isFirstIni;

  

      
        private void sceneAddress_Load(object sender, EventArgs e)
        {
            try
            {
                isFirstIni = true;
                LinkDicIni();
                //IP地址初始化
                ToolsUtil.CBGetIp(cb1);
                InfoFormat();
                isFirstIni = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
        }



        #region 初始化和还原展示数据

        /// <summary>
        /// 获取加载ini里面的链路的所有类型
        /// </summary>
        private void LinkDicIni()
        {
            //记录全部cb2的类型
            linkDic = IniHelper.ReadTypesAddress2Info();
            if (linkDic == null)
            {
                return;
            }
            foreach (var item in linkDic)
            {
                cb2.Items.Add(item.Key);

            }
        }

        /// <summary>
        /// 还原地址的信息 
        /// </summary>
        private void InfoFormat()
        {
            if (address == "255.255.255.255")
            {
                cb1.Text = ip;
                if (cb2.Items.Count > 0)
                {
                    cb2.SelectedIndex = 0;

                }
                return;
            }
            else if (string.IsNullOrEmpty(address))
            {
                cb1.Text = ip;
                cb2.SelectedIndex = 0;
                return;
            }
            //IP还原
            string[] addInfos = address.Split('.');
            for (int i = 0; i < cb1.Items.Count; i++)
            {
                if (cb1.Items[i].ToString().Split('.')[3] == addInfos[0])
                {
                    cb1.SelectedItem = cb1.Items[i];
                    break;
                }
            }

            //类型还原
            if (string.IsNullOrEmpty(nowType))
            {
                //没有类型 默认选中为设备
                cb2.SelectedIndex = 0;
                return;
            }
            string fileName = IniHelper.findTypesIniTypebyName(nowType);
            if (string.IsNullOrEmpty(fileName))
            {
                //没有类型 默认选中为设备
                cb2.SelectedIndex = 0;
                return;
            }

            string path = string.Format("{0}//types//{1}.ini", Application.StartupPath, fileName);
            string[] address_1 = IniConfig.GetValue(path, "address", "1").Split(',');
            lb1.Text = address_1[0];

            string[] address_2 = IniConfig.GetValue(path, "address", "2").Split(',');
            lb2.Text = address_2[0];
            cb2.SelectedItem = address_2[1];
            switch (linkDic[cb2.SelectedItem.ToString()])
            {
                //设备  面板按键 感应输入
                case "0":
                case "248":
                case "250":
                    //addNumForDevList();
                    cb3.Text = addInfos[2];
                    cb4.Text = addInfos[3];
                    break;

                //场景 定时 面板感应  逻辑
                case "16":
                case "32":
                case "48":
                case "64":
                    cb4.Text = (Convert.ToInt32(addInfos[2]) * 256 + Convert.ToInt32(addInfos[3])).ToString();
                    break;

                //虚拟端口
                case "251":
                    cb3.Text = addInfos[2];
                    cb4.Text = addInfos[3];
                    cb3.Enabled = false;
                    break;
            }
            
        }

        #endregion

        #region cb的内容更改

        /// <summary>
        /// IP选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cb1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb2.Items.Clear();
            if (cb1.Text == ip)
            {
                //非跨主机
                if (linkDic == null)
                {
                    return;
                }
                foreach (var item in linkDic)
                {
                    cb2.Items.Add(item.Key);

                }
                if (!string.IsNullOrEmpty(cb2.Text))
                {
                    cb2.SelectedItem = cb2.Text;
                }
            }
            else
            {

                //跨主机只显示场景
                foreach (var item in linkDic)
                {
                    if (item.Value == "16")
                    {
                        cb2.Items.Add(item.Key);
                    }

                }
                cb2.SelectedIndex = 0;

            }
            if (!isFirstIni)
            {
                FindTotalNum();

            }
        }




        private void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //设置读取的格式
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
        }


        private void Cb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FindTotalNum();

        }

        #endregion

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
                       /* string tmpip = ip;
                        if (!cb1.Text.Contains("254"))
                        {
                            string[] ips = ip.Split('.');
                            tmpip = string.Format("{0}.{1}.{2}.{3}", ips[0], ips[1], ips[2], cb1.Text);
                        }
                        DataJson.Scene scene = DataListHelper.getSceneList(tmpip);*/
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

        private void btnDecid_Click(object sender, EventArgs e)
        {
            try {
                string newAddress = "";
                if (!string.IsNullOrEmpty(cb1.Text) && !string.IsNullOrEmpty(cb2.Text))
                {
                    if (ip == cb1.Text)
                    {
                        newAddress = "FE" + ToolsUtil.strtohexstr(linkDic[cb2.SelectedItem.ToString()]);
                    }
                    else
                    {
                        newAddress = ToolsUtil.strtohexstr(cb1.Text.Split('.')[3]) + ToolsUtil.strtohexstr(linkDic[cb2.SelectedItem.ToString()]);
                    }

                    switch (linkDic[cb2.SelectedItem.ToString()])
                    {
                        //设备  面板按键 感应输入
                        case "0":
                        case "248":
                        case "250":
                            if (string.IsNullOrEmpty(cb3.Text) || string.IsNullOrEmpty(cb4.Text))
                            {
                                this.DialogResult = DialogResult.No;
                                return;
                            }
                            newAddress = newAddress + ToolsUtil.strtohexstr(cb3.Text) + ToolsUtil.strtohexstr(cb4.Text);
                            break;
                        //场景 定时 面板 感应编组  逻辑
                        case "16":
                        case "32":
                        case "48":
                        case "64":
                            if ( string.IsNullOrEmpty(cb4.Text))
                            {
                                this.DialogResult = DialogResult.No;
                                return;
                            }
                            newAddress = newAddress + Convert.ToInt32(cb4.Text).ToString("X4");
                            break;

                        //虚拟端口
                        case "251":
                            if (string.IsNullOrEmpty(cb3.Text) || string.IsNullOrEmpty(cb4.Text))
                            {
                                this.DialogResult = DialogResult.No;
                                return;
                            }
                            newAddress = newAddress + ToolsUtil.strtohexstr(cb3.Text) + ToolsUtil.strtohexstr(cb4.Text);
                            break;
                    }
                
                }

                if (newAddress.Length == 8)
                {
                    this.address = newAddress;
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                this.DialogResult = DialogResult.No;
            }
            catch
            {
                this.DialogResult = DialogResult.No;
            }
               
            
        
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

        private void sceneAddress_Paint(object sender, PaintEventArgs e)
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

        private void plInfoTitle_MouseDown_1(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void plInfoTitle_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }










        #endregion

      
    }
}
