using eNet编辑器.OtherView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.Controller
{
    public partial class SetData : Form
    {


        ClientAsync5000 client5000;
        private event Action<string> tcp5000receviceDelegate;

        private DataJson.Module devModuel;
        private DataJson.DevRS3000 devRS3000;
        private string ip;
        private bool isSync = false;

        //接收标记
        private bool isTimeOut;
        public string Ip { get => ip; set => ip = value; }
        public DataJson.Module DevModuel { get => devModuel; set => devModuel = value; }

        public bool IsSync { get => isSync; set => isSync = value; }

        private BackgroundWorker backgroundWorker1;
        private BackgroundWorker backgroundWorker2;
        private PgView pgv;
        private string hexBand;
        private string rcvIP;
        private string rcvType;
        private string rcvID;
        private string rcvPort;
        private string rcvReg;

        List<string> nameCopy;
        List<string> rcvCopy ;
        List<string> sendCopy;
        List<string> pollingCopy;
        public SetData()
        {
            InitializeComponent();
        }


        private void SetData_Load(object sender, EventArgs e)
        {
            try
            {
                controlIni();
                tcp5000receviceDelegate += new Action<string>(tcp5000ReceviceDelegateMsg);
                //焦点失去
                groupBox3.MouseDown += new MouseEventHandler(groupBox1_MouseDown);
                groupBox4.MouseDown += new MouseEventHandler(groupBox1_MouseDown);
                groupBox5.MouseDown += new MouseEventHandler(groupBox1_MouseDown);
                doubleBuffered(dataGridView1);
                //链接tcp
                Connect5000Tcp(ip);
                timer2.Start();
                rcvIP = ToolsUtil.GetIPstyle(ip,4);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            
        }

    

        #region groupBox1失去焦点
        private void SuperTabControlPanel2_Click(object sender, EventArgs e)
        {
            superTabControlPanel2.Focus();
        }

        void groupBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //鼠标按下bai
            superTabControlPanel2.Focus();

        }

        #endregion

        #region 利用反射设置DataGridView的双缓冲
        private void doubleBuffered(DataGridView dataGridView)
        {
            //利用反射设置DataGridView的双缓冲
            Type dgvType = dataGridView.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, true, null);
        }
        #endregion

        #region  界面UI初始化 切换
        private void controlIni()
        {
            #region 主界面
            cbBaudRate.Items.Clear();
            cbBaudRate.Items.Add("1200");
            cbBaudRate.Items.Add("2400");
            cbBaudRate.Items.Add("4800");
            cbBaudRate.Items.Add("9600");
            cbBaudRate.Items.Add("19200");
            cbBaudRate.Items.Add("38400");
            cbBaudRate.Items.Add("57600");
            cbBaudRate.Items.Add("115200");

            cbCheck.Items.Clear();
            cbCheck.Items.Add("无校验");
            cbCheck.Items.Add("奇校验");
            cbCheck.Items.Add("偶校验");

            cbStop.Items.Clear();
            cbStop.Items.Add("1 bit");
            cbStop.Items.Add("2 bit");



            #endregion


            #region 发送界面初始化
            cbSendCheck.Items.Clear();
            cbSendCheck.Items.Add("无校验处理");
            cbSendCheck.Items.Add("校验和");
            cbSendCheck.Items.Add("LRC校验（校验和补码）");
            cbSendCheck.Items.Add("异或校验");
            cbSendCheck.Items.Add("CRC16校验（高字节在后，低字节在前）");
            cbSendCheck.Items.Add("CRC16校验（低字节在后，高字节在前）");
            cbSendCheck.Items.Add("校验和+附加值");
            cbSendCheck.Items.Add("LRC校验+附加值");
            cbSendCheck.Items.Add("异或校验+附加值");
            cbSendCheck.Items.Add("校验和xor附加值");
            cbSendCheck.Items.Add("LRC校验xor附加值");
            cbSendCheck.Items.Add("异或校验xor附加值");
            #endregion

            #region 接收界面初始化
            cbTime.Items.Clear();
            cbTime.Items.Add("不查询");
            cbTime.Items.Add("60");
            cbTime.Items.Add("100");
            cbTime.Items.Add("160");
            cbTime.Items.Add("200");
            cbTime.Items.Add("400");
            cbTime.Items.Add("500");
            cbTime.Items.Add("600");
            cbTime.Items.Add("800");
            cbTime.Items.Add("1000");
            cbTime.Items.Add("1500");
            cbTime.Items.Add("2000");
            cbTime.Items.Add("3000");
            cbTime.Items.Add("4000");
            cbTime.Items.Add("5000");

            cbRcvDealMode.Items.Clear();
            cbRcvDealMode.Items.Add("本端口查询指令应答数据");
            cbRcvDealMode.Items.Add("所有接收数据");
            for (int i = 0; i < devModuel.devPortList.Count; i++)
            {
                cbRcvDealMode.Items.Add(string.Format("端口{0}查询指令应答数据",i+1));

            }

            cbRcvKeyFeedback.Items.Clear();
            cbRcvKeyFeedback.Items.Add("禁止反馈");
            cbRcvKeyFeedback.Items.Add("格式正确，当前端口号的按键按下");
            cbRcvKeyFeedback.Items.Add("当前端口号的按键反馈处理结果");

            cbRcvFunc.Items.Clear();
            cbRcvFunc.Items.Add("无运算处理");
            cbRcvFunc.Items.Add("四则运算一");
            cbRcvFunc.Items.Add("四则运算二");
            cbRcvFunc.Items.Add("八数值匹配");
            cbRcvFunc.Items.Add("九区范围匹配");
            cbRcvFunc.Items.Add("四数值匹配");
            cbRcvFunc.Items.Add("五区范围匹配");
            cbRcvFunc.Items.Add("IEEE浮点数换算");
            cbRcvFunc.Items.Add("DLT645规范数值换算");
            cbRcvFunc.Items.Add("ASCII码格式的十六进制数据转十六进制数据");
            cbRcvFunc.Items.Add("ASCII码格式的十进制数据转十六进制数据");
            cbRcvFunc.Items.Add("十进制数据转十六进制数据");

            nameCopy = new List<string>();
            rcvCopy = new List<string>();
            sendCopy = new List<string>();
            pollingCopy = new List<string>();
            #endregion

            #region 数据加载
            if (devModuel != null && devModuel.devPortList != null)
            {
                //数据初始化
                if (string.IsNullOrEmpty(devModuel.devContent))
                {
                    devRS3000 = new DataJson.DevRS3000();
                    devRS3000.baud = 9600;
                    devRS3000.check = 0;
                    devRS3000.stopBit = 0;
                    devRS3000.pollingTime = 0;

                }
                else
                {
                    devRS3000 = JsonConvert.DeserializeObject<DataJson.DevRS3000>(devModuel.devContent);

                }

                if (devRS3000.portDatas == null || devRS3000.portDatas.Count < 1)
                {
                    for (int i = 0; i < devModuel.devPortList.Count; i++)
                    {
                        DataJson.PortData portData = new DataJson.PortData();
                        portData.id = i + 1;
                        devRS3000.portDatas.Add(portData);
                    }
                }
                else
                {
                    if (devRS3000.portDatas.Count < devModuel.devPortList.Count)
                    {
                        for (int i = devRS3000.portDatas.Count; i < devModuel.devPortList.Count + 1; i++)
                        {
                            DataJson.PortData portData = new DataJson.PortData();
                            portData.id = i + 1;
                            devRS3000.portDatas.Add(portData);
                        }
                    }
                }
                //设置串口信息
                lbTitle.Text = string.Format("串口设置 - ID：{0}   型号：{1}   版本：{2}", devModuel.id,DevModuel.device, DevModuel.ver);
                cbBaudRate.Text = devRS3000.baud.ToString();
                cbCheck.SelectedIndex = devRS3000.check;
                cbStop.SelectedIndex = devRS3000.stopBit;
                //cbTime.Text = devRS3000.pollingTime.ToString();
                //添加行 并填入信息
                int index ;
                //暂时存放发送数据 检验chk是否为旧的
                for (int i = 0; i < devModuel.devPortList.Count; i++)
                {
                    index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = i + 1;
                    dataGridView1.Rows[index].Cells[1].Value = devRS3000.portDatas[i].name;
                    string tmpCode = "";
                    tmpCode = ChkOldChange(devRS3000.portDatas[i].sendCode);
                    if (!string.IsNullOrEmpty(tmpCode))
                    {
                        devRS3000.portDatas[i].sendCode = tmpCode;
                    }
                    dataGridView1.Rows[index].Cells[2].Value = SendShowStyle(devRS3000.portDatas[i].sendCode);
                    dataGridView1.Rows[index].Cells[3].Value = RcvShowStyle(devRS3000.portDatas[i].rcvCode) ;
                    if (!string.IsNullOrEmpty(devRS3000.portDatas[i].pollingCode))
                    {
                        dataGridView1.Rows[index].Cells[3].Style.BackColor = Color.FromArgb(228, 228, 228);
                    }
                    
                }
                TableShow(1);
                SendCodeEdit(1);
            }

            #endregion

        }

        /// <summary>
        /// 界面显示
        /// </summary>
        /// <param name="i"></param>
        private void TableShow(int i)
        {
            switch (i)
            {
                case 1:
                    superTabItem1.Visible = true;
                    superTabItem2.Visible = false;
                    superTabItem3.Visible = false;
                    superTabItem4.Visible = false;
                    break;
                case 2:
                    superTabItem2.Visible = true;
                    superTabItem1.Visible = false;
                    superTabItem3.Visible = false;
                    superTabItem4.Visible = false;
                    break;
                case 3:
                    superTabItem3.Visible = true;
                    superTabItem1.Visible = false;
                    superTabItem2.Visible = false;
                    superTabItem4.Visible = false;
                    break;
                case 4:
                    superTabItem4.Visible = true;
                    superTabItem1.Visible = false;
                    superTabItem2.Visible = false;
                    superTabItem3.Visible = false;
                    break;
            }
        }


        private string ChkOldChange(string sendCode)
        {
            if (string.IsNullOrEmpty(sendCode))
            {
                return null;
            }
            string chk = sendCode.Substring(0, 2);
            switch (chk)
            {
                case "00":
                case "01":
                case "02":
                case "03":
                case "04":
                case "05":
                case "11":
                case "12":
                case "13":
                case "21":
                case "22":
                case "23":
                    sendCode = sendCode.Remove(0, 2);
                    sendCode = sendCode.Insert(0, (Convert.ToInt32(chk, 16) + 128).ToString("X2"));
                    //并用模式 插入DR1 
                    sendCode = sendCode.Insert(16, "01020304");
                    break;
                case "80":
                case "81":
                case "82":
                case "83":
                case "84":
                case "85":
                case "91":
                case "92":
                case "93":
                case "A1":
                case "A2":
                case "A3":
                    sendCode = null;
                    break;
                default:
                    sendCode = null;
                    break;
            }
            return sendCode;
        }
        #endregion

        #region  计算字符串长度
        /// <summary>
        /// 计算字符串长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="selectStr"></param>
        /// <param name="selectStart"></param>
        /// <param name="isSend">是否为发送</param>
        private void GetStrSectionLen(string str,string selectStr,int  selectStart,bool isSend)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    //总字符串为空
                    bar1.Items[1].Text = "0";
                    bar1.Items[3].Text = "0";
                    return;
                }
                List<string> list = str.Split(' ').ToList<string>();
                bar1.Items[1].Text = list.Count.ToString();
                if (string.IsNullOrEmpty(selectStr))
                {
                    //总字符串为空
                    bar1.Items[3].Text = "0";
                    return;
                }
                //当前选中位置
                selectStart = selectStart + 1;
                if (selectStr.Substring(0, 1) != " ")
                {
                    selectStart = selectStart - 1;
                }
                string beforCode = str.Substring(0, selectStart);
                if (isSend)
                {
                    selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#") * 3 - ToolsUtil.SubstringCount(beforCode, "/") - ToolsUtil.SubstringCount(beforCode, "CK") * 2;

                }
                else
                {
                    selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#") * 2 - ToolsUtil.SubstringCount(beforCode, "/") - ToolsUtil.SubstringCount(beforCode, "CK") * 2;

                }
                bar1.Items[3].Text = ((selectStart / 2) + 1).ToString();

            }
            catch {

            }
            
        }
      


        #endregion

        #region tcp5000 链接 以及处理反馈信息 发送信息

        /// <summary>
        /// 定时5秒链接tcp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (client5000 != null && !client5000.Connected())
            {
                //链接tcp
                Connect5000Tcp(ip);
            }

        }

        private void Connect5000Tcp(string ip)
        {
            try
            {

                if (client5000 != null)
                {
                    client5000.Dispoes();
                }
                pictureBox1.Image = Properties.Resources.OutLine;
                client5000 = new ClientAsync5000();
                client5000Ini();
                if (string.IsNullOrEmpty(ip))
                {
                    return;
                }
                //异步连接
                client5000.ConnectAsync(ip, 5000);
                if (client5000 != null && client5000.Connected())
                {
                    pictureBox1.Image = Properties.Resources.Online;
                   
                    return;
                }
                else
                {
                    //异步连接失败 再次连接
                    client5000.ConnectAsync(ip, 5000);
                    if (client5000 != null && client5000.Connected())
                    {
                        pictureBox1.Image = Properties.Resources.Online;
                        return;
                    }
                }

            }
            catch
            {

                return;
            }
        }



        private void client5000Ini()
        {
            client5000.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync5000.EnSocketAction>((c, enAction) =>
            {
                string key = "";

                try
                {
                    if (c.Client.Connected)
                    {
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }

                    switch (enAction)
                    {
                        case ClientAsync5000.EnSocketAction.Connect:
                            // MessageBox.Show("已经与" + key + "建立连接");
                            break;
                        case ClientAsync5000.EnSocketAction.SendMsg:

                            //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                            break;
                        case ClientAsync5000.EnSocketAction.Close:
                            //跨线程调用
                            this.Invoke(tcp5000receviceDelegate, "outline");
                            //MessageBox.Show("服务端连接关闭");
                            break;
                        case ClientAsync5000.EnSocketAction.Error:
                            //MessageBox.Show("连接发生错误,请检查网络连接");
                            //跨线程调用
                            this.Invoke(tcp5000receviceDelegate, "outline");
                            break;
                        default:
                            break;
                    }
                }
                catch { }
            });
            //信息接收处理
            client5000.Received += new Action<string, string>((key, Hexmsg) =>
            {
                try
                {

                    if (!string.IsNullOrEmpty(Hexmsg))
                    {
                        //跨线程调用
                        this.Invoke(tcp5000receviceDelegate, Hexmsg.Replace(" ",""));

                    }
                }
                catch
                {
                    //报错不处理
                }

            });

        }

      
        /// <summary>
        /// 5000端口回调函数
        /// </summary>
        /// <param name="msg"></param>
        private void tcp5000ReceviceDelegateMsg(string rcvHexMsg)
        {
            //离线处理
            if (rcvHexMsg.Contains("outline"))
            {
                if (client5000 != null)
                {
                    client5000.Dispoes();
                }
                pictureBox1.Image = Properties.Resources.OutLine;
                return;
            }
            //分割指令
            string subStr = "";
            string order = "";
            bool isHeart = false;
            List<string> orderList = new List<string>();
            for (int i = 0; i < rcvHexMsg.Length; i+=2)
            {
                subStr = rcvHexMsg.Substring(i, 2);
                if (subStr == rcvIP || subStr == "FE")
                {
                    if (!isHeart)
                    {
                        //确定为首次截取信息帧头
                        isHeart = true;
                        order = subStr;
                    }
                    else
                    {
                        order = order + subStr;
                    }

                }
                else if (isHeart && subStr == "CA")
                {
                    order = order + subStr;
                    if (i + 2 < rcvHexMsg.Length)
                    {
                        //确保不是最后的子集
                        subStr = rcvHexMsg.Substring(i + 2, 2);
                        if (subStr != rcvIP && subStr == "FE")
                        {
                            isHeart = false;
                            orderList.Add(order);
                        }
                    }
                    else
                    {
                        //字符串结尾 
                        isHeart = false;
                        orderList.Add(order);
                    }
                }
                else
                {
                    //添加 截取子串
                    order = order + subStr;
                }
            }

            string nowType = "";
            string nowID = "";
            string nowPort = "";
            string nowReg = "";
            int len ;
            int port;
            //处理指令
            for (int i = 0; i < orderList.Count; i++)
            {
                nowType = orderList[i].Substring(4, 2);
                nowID = orderList[i].Substring(6, 2);
                nowPort = orderList[i].Substring(8, 2);
                nowReg = orderList[i].Substring(10, 2);
                port = Convert.ToInt32(nowPort, 16);
                len = Convert.ToInt32(orderList[i].Substring(12, 2), 16);
                //Console.WriteLine(orderList[i]);
                if (nowType == rcvType && nowID == rcvID && nowPort == rcvPort && nowReg == rcvReg)
                {
                    //读写应答
                    isTimeOut = false;
                    
                }

                if (nowType == "91")
                {
                    //查询反馈
                    switch (nowReg)
                    {
                        case "60":
                            //接收
                            if (len > 2)
                            {
                                devRS3000.portDatas[port - 1].rcvCode = orderList[i].Substring(18, orderList[i].Length - 20);
                                dataGridView1.Rows[port - 1].Cells[3].Value = RcvShowStyle(devRS3000.portDatas[port - 1].rcvCode);
                            }
                            
                            break;
                        case "61":
                            //发送
                            if (len > 2)
                            {
                                devRS3000.portDatas[port - 1].sendCode = orderList[i].Substring(20, orderList[i].Length - 22);
                                dataGridView1.Rows[port - 1].Cells[2].Value = SendShowStyle(devRS3000.portDatas[port - 1].sendCode);
                            }
                            
                            break;
                        case "62":
                            //查询
                            if (len > 2)
                            {
                                devRS3000.portDatas[port - 1].pollingCode = orderList[i].Substring(20, orderList[i].Length - 22);

                            }
                            break;
                        case "63":
                            //发送数据定义
                            if (len > 2)
                            {
                                devRS3000.portDatas[port - 1].customCode = orderList[i].Substring(14, orderList[i].Length - 16);

                            }
                            break;
                        case "64":
                            //逻辑
                            if (len > 2)
                            {
                                devRS3000.portDatas[port - 1].logicCode = orderList[i].Substring(14, orderList[i].Length - 16);

                            }
                            break;
                        case "0C":
                            //串口参数
                            SetSerialVal(orderList[i].Substring(14, orderList[i].Length - 16));
                            cbBaudRate.Text = devRS3000.baud.ToString();
                            cbCheck.SelectedIndex = devRS3000.check;
                            cbStop.SelectedIndex = devRS3000.stopBit;
                            break;
                        case "08":
                            //查询时间
                            devRS3000.pollingTime = Convert.ToInt32(orderList[i].Substring(14, orderList[i].Length - 16), 16) * 20;
                            break;
                        default:
                            break;
                    }
                }
            }
         
        }
        #endregion

        #region 窗体边框 拉伸 最大化 最小话 还原

        #region  窗体设置
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

        private const int Guying_HTLEFT = 10;
        private const int Guying_HTRIGHT = 11;
        private const int Guying_HTTOP = 12;
        private const int Guying_HTTOPLEFT = 13;
        private const int Guying_HTTOPRIGHT = 14;
        private const int Guying_HTBOTTOM = 15;
        private const int Guying_HTBOTTOMLEFT = 0x10;
        private const int Guying_HTBOTTOMRIGHT = 17;


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
        //private const int HTCAPTION = 0x2;

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
                case 0x0084:
                    base.WndProc(ref m);
                    Point vPoint = new Point((int)m.LParam & 0xFFFF, (int)m.LParam >> 16 & 0xFFFF);
                    vPoint = PointToClient(vPoint);
                    if (vPoint.X <= 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPLEFT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMLEFT;
                        else
                            m.Result = (IntPtr)Guying_HTLEFT;
                    else if (vPoint.X >= ClientSize.Width - 5)
                        if (vPoint.Y <= 5)
                            m.Result = (IntPtr)Guying_HTTOPRIGHT;
                        else if (vPoint.Y >= ClientSize.Height - 5)
                            m.Result = (IntPtr)Guying_HTBOTTOMRIGHT;
                        else
                            m.Result = (IntPtr)Guying_HTRIGHT;
                    else if (vPoint.Y <= 5)
                        m.Result = (IntPtr)Guying_HTTOP;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)Guying_HTBOTTOM;
                    break;
                case 0x0201://鼠标左键按下的消息
                    m.Msg = 0x00A1;//更改消息为非客户区按下鼠标
                    m.LParam = IntPtr.Zero; //默认值
                    m.WParam = new IntPtr(2);//鼠标放在标题栏内
                    base.WndProc(ref m);
                    break;
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
                    base.WndProc(ref m);
                    break;
            }

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }

        private void SetData_Paint(object sender, PaintEventArgs e)
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


        [DllImport("user32.dll")]//命名空间System.Runtime.InteropServices;
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_RESTORE = 0xF120;
        public const int SC_SIZE = 0xF000;

        #endregion

        private void PlInfoTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks <= 1)
            {
                //拖动窗体
                ReleaseCapture();//释放label1对鼠标的捕捉
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
            }
            else if (e.Clicks == 2)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    //还原窗体
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_SYSCOMMAND, SC_RESTORE, 0);
                    btnBig.Tooltip = "最大化";
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    //最大化窗体
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
                    btnBig.Tooltip = "还原";
                }

            }

        }

        private void BtnSmall_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            
        }

        private void BtnBig_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                //还原窗体
                ReleaseCapture();
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_RESTORE, 0);
                btnBig.Tooltip = "最大化";
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                //最大化窗体
                ReleaseCapture();
                SendMessage(this.Handle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
                btnBig.Tooltip = "还原";
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {

            devModuel.devContent = JsonConvert.SerializeObject(devRS3000);
            timer2.Stop();
            if (client5000 != null)
            {
                client5000.Dispoes();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion

        #region 写入 读取 导入 导出 读取波特率和时间

        #region 写入 读取
        private void BtnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork_Write;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                hexBand = GetSerialVal();
                pgv = new PgView();
                pgv.setMaxValue(devRS3000.portDatas.Count *5+2);
                backgroundWorker1.RunWorkerAsync();
                pgv.ShowDialog();
                if (pgv.DialogResult == DialogResult.Cancel )
                {
                    backgroundWorker1.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void BackgroundWorker1_DoWork_Write(object sender, DoWorkEventArgs e)
        {
            int count = 0;
            string hexID = DevModuel.id.ToString("X2");
            string hexPort = "";
            string tmpCode = "";
            if (string.IsNullOrEmpty(hexBand))
            {
                MessageBox.Show("请先设置串口参数");
                return;
            }
            int pollingTime = 0;
            //查询时间写0
            pollingTime = devRS3000.pollingTime;
            devRS3000.pollingTime = 0;
            if (!TimeOrder())
            {
                return;
            }
            count++;
            backgroundWorker1.ReportProgress(count);

            ToolsUtil.DelayMilli(6000);

            foreach (DataJson.PortData portData in devRS3000.portDatas)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                hexPort = portData.id.ToString("X2");
                //1.发送代码写入 
                if (string.IsNullOrEmpty(portData.sendCode))
                {
                    tmpCode = "";
                }
                else {
                    tmpCode = hexBand + portData.sendCode;
                }
                if (!WriteOrder(hexID, hexPort, "61", tmpCode))
                {
                    return;
                }
                count++;
                backgroundWorker1.ReportProgress(count);

                //2.接收代码发送
                if (!WriteOrder(hexID, hexPort, "60", portData.rcvCode))
                {
                    return;
                }
                count++;
                backgroundWorker1.ReportProgress(count);
                //3.查询代码发送
                if (string.IsNullOrEmpty(portData.pollingCode))
                {
                    tmpCode = "";
                }
                else
                {
                    tmpCode = hexBand + portData.pollingCode;
                }
                if (!WriteOrder(hexID, hexPort, "62", tmpCode))
                {
                    return;
                }
                count++;
                backgroundWorker1.ReportProgress(count);

                //4.发送高级代码写入 
                if (!WriteOrder(hexID, hexPort, "63", portData.customCode))
                {
                    return;
                }
                count++;
                backgroundWorker1.ReportProgress(count);
                //5.逻辑代码发送
                if (!WriteOrder(hexID, hexPort, "64", portData.logicCode))
                {
                    return;
                }
                count++;
                backgroundWorker1.ReportProgress(count);

            }
            //设置查询时间
            devRS3000.pollingTime = pollingTime;
            if (!TimeOrder())
            {
                return;
            }
            count++;
            backgroundWorker1.ReportProgress(count);
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker2 = new BackgroundWorker();
                backgroundWorker2.WorkerReportsProgress = true;
                backgroundWorker2.WorkerSupportsCancellation = true;
                backgroundWorker2.DoWork += BackgroundWorker1_DoWork_Read;
                backgroundWorker2.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker2.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                //添加行 并填入信息
                dataGridView1.Rows.Clear();
                int index;
                for (int i = 0; i < devModuel.devPortList.Count; i++)
                {
                    index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = i + 1;
                    devRS3000.portDatas[i].sendCode = "";
                    devRS3000.portDatas[i].rcvCode = "";
                    devRS3000.portDatas[i].pollingCode = "";
                    devRS3000.portDatas[i].customCode = "";
                    devRS3000.portDatas[i].logicCode = "";
                }
                pgv = new PgView();
                pgv.setMaxValue(devRS3000.portDatas.Count * 5 + 4);
          
           
                backgroundWorker2.RunWorkerAsync();
                pgv.ShowDialog();
                if (pgv.DialogResult == DialogResult.Cancel)
                {
                    backgroundWorker2.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BackgroundWorker1_DoWork_Read(object sender, DoWorkEventArgs e)
        {
            int count = 0;
            string hexID = DevModuel.id.ToString("X2");
            string hexPort = "";
            int pollingTime = 0;
            //6.读取串口时间
            if (!ReadOrder(hexID, "FF", "0C"))
            {
                return;
            }

            count++;
            backgroundWorker2.ReportProgress(count);
            //7.读取接收时间
            if (!ReadOrder(hexID, "FF", "08"))
            {
                return;
            }
            count++;
            backgroundWorker2.ReportProgress(count);
            //查询时间写0
            pollingTime = devRS3000.pollingTime;
            devRS3000.pollingTime = 0;
            if (!TimeOrder())
            {
                return;
            }
            count++;
            backgroundWorker2.ReportProgress(count);
            ToolsUtil.DelayMilli(3000);
            foreach (DataJson.PortData portData in devRS3000.portDatas)
            {
                if (backgroundWorker2.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                hexPort = portData.id.ToString("X2");
                //1.发送代码写入 
                if (!ReadOrder(hexID, hexPort, "61"))
                {
                    return;
                }
                count++;
                backgroundWorker2.ReportProgress(count);
                ToolsUtil.DelayMilli(100);
                //2.接收代码发送
                if (!ReadOrder(hexID, hexPort, "60"))
                {
                    return;
                }
                count++;
                backgroundWorker2.ReportProgress(count);
                ToolsUtil.DelayMilli(100);
                //3.查询代码发送
                if (!ReadOrder(hexID, hexPort, "62"))
                {
                    return;
                }
                count++;
                backgroundWorker2.ReportProgress(count);
                ToolsUtil.DelayMilli(100);
                //4.发送数据定义代码写入 
                if (!ReadOrder(hexID, hexPort, "63"))
                {
                    return;
                }
                count++;
                backgroundWorker2.ReportProgress(count);
                ToolsUtil.DelayMilli(100);
                //5.逻辑代码发送
                if (!ReadOrder(hexID, hexPort, "64"))
                {
                    return;
                }
                count++;
                backgroundWorker2.ReportProgress(count);
                ToolsUtil.DelayMilli(100);
            }
            //设置查询时间
            devRS3000.pollingTime = pollingTime;
            if (!TimeOrder())
            {
                return;
            }
            count++;
            backgroundWorker2.ReportProgress(count);

        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (pgv != null)
                {
                    Console.WriteLine("进度框关闭");
                    pgv.Close();

                }
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }

        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("更新进度条"+ e.ProgressPercentage.ToString());
            //设置值
            pgv.setValue(e.ProgressPercentage);

        }


        private bool WriteOrder(string hexID, string hexPort, string hexReg, string hexCode)
        {
            string ord = "";
            if (string.IsNullOrEmpty(hexCode))
            {
                //组成指令
                ord = string.Format("FE0010{0}{1}{2}0100CA", hexID, hexPort, hexReg);
            }
            else
            {
                int totalLen = hexCode.Length / 2 + 1;
                if (hexReg == "63" || hexReg == "64")
                {
                    //组成指令
                    ord = string.Format("FE0010{0}{1}{2}{3}{4}CA", hexID, hexPort, hexReg,  (totalLen - 1).ToString("X2"), hexCode);
                }
                else
                {
                    //组成指令
                    ord = string.Format("FE0010{0}{1}{2}{3}{4}{5}CA", hexID, hexPort, hexReg, totalLen.ToString("X2"), (totalLen - 1).ToString("X2"), hexCode);
                }
                
            }
            //接收标记
            rcvID = hexID;
            rcvPort = hexPort;
            rcvReg = hexReg;
            rcvType = "11";
            Console.WriteLine("寄存器"+ hexReg + ":  "+  ord);
            return TimeOutSendOrder(ord);
        }

        private bool ReadOrder(string hexID, string hexPort, string hexReg)
        {
            string ord = ord = string.Format("FE0090{0}{1}{2}00CA", hexID, hexPort, hexReg);
            //接收标记
            rcvID = hexID;
            rcvPort = hexPort;
            rcvReg = hexReg;
            rcvType = "91";
            bool isRead = false;
            //第一次发送
            if (TimeOutSendOrder(ord))
            {
                //第二次发送
                if (TimeOutSendOrder(ord))
                {
                    isRead = true;
                }
            }
            return isRead;
        }
        private bool TimeOutSendOrder(string ord)
        {
            int count = 0;
            isTimeOut = true;
            //发送
            Console.WriteLine(ord + " 发送指令：" + count.ToString());
            client5000.SendHexAsync(ord);
            TimeOutHelper timeOutHelper = new TimeOutHelper();
            while (isTimeOut)
            {
                if (timeOutHelper.IsTimeout())
                {
                    count++;
                    //超过2秒没回应 重新发送资源
                    timeOutHelper = new TimeOutHelper();
                    client5000.SendHexAsync(ord);
                    //发送
                    Console.WriteLine(ord + " 发送指令：" + count.ToString());
                    //Console.WriteLine(ord + " 发送指令回应超时，重发信息次数：" + count.ToString() );
                    if (count == 3)
                    {
                        //超时发送3次 直接退出
                        count = 1;
                        //发送失败
                        //MessageBox.Show("发送回应超时："+ ord);
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region 读取串口时间 
        private void BtnReadBaudTime_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork_ReadBaudTime;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                pgv = new PgView();
                pgv.setMaxValue(2);
                //this.Enabled = false;
      
                backgroundWorker1.RunWorkerAsync();
                pgv.ShowDialog();
                if (pgv.DialogResult == DialogResult.Cancel)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackgroundWorker1_DoWork_ReadBaudTime(object sender, DoWorkEventArgs e)
        {
            int count = 0;
            string hexID = DevModuel.id.ToString("X2");
            //6.读取串口时间
            if (!ReadOrder(hexID, "FF", "0C"))
            {
                return;
            }
            count++;
            backgroundWorker1.ReportProgress(count);
            //7.读取接收时间
            if (!ReadOrder(hexID, "FF", "08"))
            {
                return;
            }
            count++;
            backgroundWorker1.ReportProgress(count);
        }

        #endregion

        #region 导入导出
        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath");
                if (System.IO.File.Exists(newPath))
                {
                    //设置此次默认目录为上一次选中目录  
                    op.InitialDirectory = newPath;

                }
                op.Title = "请打开表格文件";
                op.Filter = "CSV（*.csv）|*.csv|All files(*.*)|*.*";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = CsvHelper.OpenCSV(op.FileName);
                    //判断DataTable是否为空
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        MessageBox.Show("导入失败，文件为空");
                        return ;
                    }
                    List<DataJson.PortData> portDatas = new List<DataJson.PortData>();
                    //发送代码
                    string sendCode = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        DataJson.PortData portData = new DataJson.PortData();
                        if (Convert.IsDBNull(row[0]))
                        {
                            MessageBox.Show("导入失败，文件格式错误");
                            return;

                        }
                        else
                        {
                            portData.id = Convert.ToInt32(row[0]);
                        }
                        if (!Convert.IsDBNull(row[1]))
                        {
                            portData.name = Convert.ToString(row[1]);

                        }
                        if (!Convert.IsDBNull(row[2]))
                        {
                            sendCode = Convert.ToString(row[2]).Replace(" ", "");
                            if (!string.IsNullOrEmpty(sendCode))
                            {
                                //判断sendcode的校验位是否大于0x80 128 修改旧工程的信息
                                string chk = sendCode.Substring(0, 2);
                                switch (chk)
                                {
                                    case "00":
                                    case "01":
                                    case "02":
                                    case "03":
                                    case "04":
                                    case "05":
                                    case "11":
                                    case "12":
                                    case "13":
                                    case "21":
                                    case "22":
                                    case "23":
                                        sendCode = sendCode.Remove(0, 2);
                                        sendCode = sendCode.Insert(0, (Convert.ToInt32(chk, 16) + 128).ToString("X2"));
                                        //并用模式 插入DR1 
                                        sendCode = sendCode.Insert(16, "01020304");
                                        break;
                                    case "80":
                                    case "81":
                                    case "82":
                                    case "83":
                                    case "84":
                                    case "85":
                                    case "91":
                                    case "92":
                                    case "93":
                                    case "A1":
                                    case "A2":
                                    case "A3":
                                        break;
                                    default:
                                        MessageBox.Show("导入失败，校验类型错误");
                                        return;
                                }
                                portData.sendCode = sendCode;

                                if (!string.IsNullOrEmpty(portData.sendCode) && !Validator.IsHexadecimal(portData.sendCode))
                                {
                                    MessageBox.Show("导入失败，发送代码含非十六进制字符");
                                    return;
                                }
                            }
                           
                        }
                        if (!Convert.IsDBNull(row[3]))
                        {
                            portData.rcvCode = Convert.ToString(row[3]).Replace(" ", "");
                            if (!string.IsNullOrEmpty(portData.rcvCode) && !Validator.IsHexadecimal(portData.rcvCode))
                            {
                                MessageBox.Show("导入失败，接收代码含非十六进制字符");
                                return;
                            }

                        }
                        if (!Convert.IsDBNull(row[4]))
                        {
                            portData.pollingCode = Convert.ToString(row[4]).Replace(" ", "");
                            if (!string.IsNullOrEmpty(portData.pollingCode) && !Validator.IsHexadecimal(portData.pollingCode))
                            {
                                MessageBox.Show("导入失败，查询代码含非十六进制字符");
                                return;
                            }

                        }
                        if (!Convert.IsDBNull(row[5]))
                        {
                            portData.logicCode = Convert.ToString(row[5]).Replace(" ", "");
                            if (!string.IsNullOrEmpty(portData.logicCode) && !Validator.IsHexadecimal(portData.logicCode))
                            {
                                MessageBox.Show("导入失败，逻辑代码含非十六进制字符");
                                return;
                            }

                        }
                        if (!Convert.IsDBNull(row[6]))
                        {
                            portData.customCode = Convert.ToString(row[6]).Replace(" ", "");
                            if (!string.IsNullOrEmpty(portData.customCode) && !Validator.IsHexadecimal(portData.customCode))
                            {
                                MessageBox.Show("导入失败，逻辑代码含非十六进制字符");
                                return;
                            }

                        }
                        portDatas.Add(portData);

                    }
                    devModuel.devContent = JsonConvert.SerializeObject(portDatas);
                    devRS3000.portDatas = portDatas;
                    if (devRS3000.portDatas.Count < devModuel.devPortList.Count)
                    {
                        for (int i = devRS3000.portDatas.Count; i < devModuel.devPortList.Count + 1; i++)
                        {
                            DataJson.PortData portData = new DataJson.PortData();
                            portData.id = i + 1;
                            devRS3000.portDatas.Add(portData);
                        }
                    }
                    //dataGridView1.Rows.Clear();
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = "";
                        dataGridView1.Rows[i].Cells[2].Value = "";
                        dataGridView1.Rows[i].Cells[3].Value = "";
                    }
                    //添加行 并填入信息
                    for (int i = 0; i < devModuel.devPortList.Count; i++)
                    {
                        //index = dataGridView1.Rows.Add();
                        dataGridView1.Rows[i].Cells[0].Value = i + 1;
                        dataGridView1.Rows[i].Cells[1].Value = devRS3000.portDatas[i].name;
                        dataGridView1.Rows[i].Cells[2].Value = SendShowStyle(devRS3000.portDatas[i].sendCode);
                        dataGridView1.Rows[i].Cells[3].Value = RcvShowStyle(devRS3000.portDatas[i].rcvCode);
                    }
                    TableShow(1);
                    SendCodeEdit(1);
                    //添加打开过的地址
                    IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath", op.FileName);
                    MessageBox.Show("文件成功导入");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

            }
        }

        private void BtnOutput_Click(object sender, EventArgs e)
        {

            try
            {

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "请选择保存路径";
                //设置文件类型 
                sfd.Filter = "CSV文件（*.csv）|*.csv|JSON文件（*.json）|*.json|All files(*.*)|*.*";
                //设置默认文件类型显示顺序 
                sfd.FilterIndex = 1;
                //保存对话框是否记忆上次打开的目录 
                sfd.RestoreDirectory = true;
                //设置默认的文件名
                sfd.FileName = "RS3000Table";// in wpf is  sfd.FileName = "YourFileName";
                string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath");
                if (System.IO.File.Exists(newPath))
                {
                    //设置此次默认目录为上一次选中目录  
                    sfd.InitialDirectory = newPath;

                }
                //点了保存按钮进入 
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataTable dt = new DataTable();
                    //添加列头
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        DataColumn dc = new DataColumn(dataGridView1.Columns[i].HeaderText);
                        dt.Columns.Add(dc);
                    }

                    DataColumn dcc = new DataColumn();
                    dcc.ColumnName = "查询代码";
                    dt.Columns.Add(dcc);
                    DataColumn dc2 = new DataColumn();
                    dc2.ColumnName = "逻辑代码";
                    dt.Columns.Add(dc2);
                    DataColumn dc3 = new DataColumn();
                    dc3.ColumnName = "发送定义代码";
                    dt.Columns.Add(dc3);
                    for (int i = 0; i < devRS3000.portDatas.Count; i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = devRS3000.portDatas[i].id;
                        dr[1] = devRS3000.portDatas[i].name;
                        dr[2] = ToolsUtil.AddBlank(devRS3000.portDatas[i].sendCode);
                        dr[3] = ToolsUtil.AddBlank(devRS3000.portDatas[i].rcvCode);
                        dr[4] = ToolsUtil.AddBlank(devRS3000.portDatas[i].pollingCode);
                        dr[5] = ToolsUtil.AddBlank(devRS3000.portDatas[i].logicCode);
                        dr[6] = ToolsUtil.AddBlank(devRS3000.portDatas[i].customCode);
                        dt.Rows.Add(dr);

                    }
                    string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                    IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath", localFilePath);
                    CsvHelper.SaveCSV(dt, localFilePath);
                    MessageBox.Show("文件成功导出");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

            }
        }
        #endregion

        #endregion

        #region 发送代码显示 
        /// <summary>
        /// datagride显示代码格式
        /// </summary>
        /// <param name="sendCode"></param>
        /// <returns></returns>
        private string SendShowStyle(string sendCode)
        {
            try
            {
                if (string.IsNullOrEmpty(sendCode))
                {
                    return "";
                }
                //校验位置
                int ckSection = Convert.ToInt32(sendCode.Substring(2, 2), 16);
                //data的位置
                int data1 = Convert.ToInt32(sendCode.Substring(8, 2), 16);
                int data2 = Convert.ToInt32(sendCode.Substring(10, 2), 16);
                int data3 = Convert.ToInt32(sendCode.Substring(12, 2), 16);
                int data4 = Convert.ToInt32(sendCode.Substring(14, 2), 16);
                //数据的来源
                string dr1 = sendCode.Substring(16, 2);
                string dr2 = sendCode.Substring(18, 2);
                string dr3 = sendCode.Substring(20, 2);
                string dr4 = sendCode.Substring(22, 2);
                List<string> list = new List<string>();
                //指令数据 ab cd ef
                string code = sendCode.Substring(24, sendCode.Length - 24);
                for (int i = 0; i < sendCode.Length - 24; i = i + 2)
                {
                    list.Add(sendCode.Substring(24 + i, 2));
                }
                //data1-4 数据位置
                if (data1 != 255 && data1 <= list.Count)
                {
                    //dr1 默认值01 数据源
                    list[data1 - 1] = dr1 + "#" + list[data1 - 1];
                }
                if (data2 != 255 && data2 <= list.Count)
                {
                    list[data2 - 1] = dr2 + "#" + list[data2 - 1];
                }
                if (data3 != 255 && data3 <= list.Count)
                {
                    list[data3 - 1] = dr3 + "#" + list[data3 - 1];
                }
                if (data4 != 255 && data4 <= list.Count)
                {
                    list[data4 - 1] = dr4 + "#" + list[data4 - 1];
                }


                if (ckSection <= code.Length / 2)
                {
                    //添加ck
                    string check = sendCode.Substring(1, 1);
                    if (check == "4" || check == "5")
                    {
                        if (ckSection == 0)
                        {
                            list[ckSection] = "CK" + list[ckSection];
                            list[ckSection + 1] = "CK" + list[ckSection + 1];
                        }
                        else
                        {
                            list[ckSection - 1] = "CK" + list[ckSection - 1];
                            list[ckSection] = "CK" + list[ckSection];
                        }
                    }
                    else if (check != "0")
                    {
                        if (ckSection == 0)
                        {
                            list[ckSection] = "CK" + list[ckSection];
                        }
                        else
                        {
                            list[ckSection - 1] = "CK" + list[ckSection - 1];
                        }

                    }
                }
               
                StringBuilder sb = new StringBuilder();
                if (list != null && list.Count() > 0)
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (i < list.Count() - 1)
                        {
                            sb.Append(list[i] + " ");
                        }
                        else
                        {
                            sb.Append(list[i]);
                        }
                    }

                    return sb.ToString();

                }
                return "";
            }
            catch //(Exception ex)
            {
                //ToolsUtil.WriteLog(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.ToString());
                return "";
            }


        }



        /// <summary>
        /// 右侧区域显示数据
        /// </summary>
        /// <param name="id">行号ID</param>
        private void SendCodeEdit(int id)
        {
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            lbSendNum.Text = id.ToString();
            txtSendCode.Enabled = false;
            cbSendCheck.Enabled = false;
            txtSendStart.Enabled = false;
            txtSendEnd.Enabled = false;
            txtSendAdd.Enabled = false;
            btnSendSet.Enabled = false;
            btnSendTest.Enabled = false;
            txtSendCode.Text = "";
            cbSendCheck.Text = "";
            txtSendStart.Text = "";
            txtSendEnd.Text = "";
            txtSendAdd.Text = "";
            if (string.IsNullOrEmpty(sendCode))
            {
                return;
            }
            txtSendCode.Enabled = true;
            cbSendCheck.Enabled = true;
            txtSendStart.Enabled = true;
            txtSendEnd.Enabled = true;
            btnSendSet.Enabled = true;
            btnSendTest.Enabled = true;
            txtSendCode.Text = SendShowStyle(sendCode);
            string check = sendCode.Substring(0, 2);
            int start = Convert.ToInt32(sendCode.Substring(4, 2), 16);
            int end = Convert.ToInt32(sendCode.Substring(2, 2), 16);
            if (start != 255 && start != 0)
            {
                txtSendStart.Text = start.ToString();
            }
            else
            {
                txtSendStart.Text = "1";
            }
            
            switch (check)
            {
                case "80":
                    cbSendCheck.SelectedIndex = 0;
                    lbSendCheckSection.Text = "无";
                    break;
                case "81":
                    cbSendCheck.SelectedIndex = 1;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "82":
                    cbSendCheck.SelectedIndex = 2;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "83":
                    cbSendCheck.SelectedIndex = 3;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "84":
                    cbSendCheck.SelectedIndex = 4;
                    lbSendCheckSection.Text = string.Format("{0},{1}", end, end + 1);
                    break;
                case "85":
                    cbSendCheck.SelectedIndex = 5;
                    lbSendCheckSection.Text = string.Format("{0},{1}", end, end + 1);
                    break;
                case "91":
                    cbSendCheck.SelectedIndex = 6;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "92":
                    cbSendCheck.SelectedIndex = 7;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "93":
                    cbSendCheck.SelectedIndex = 8;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "A1":
                    cbSendCheck.SelectedIndex = 9;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "A2":
                    cbSendCheck.SelectedIndex = 10;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case "A3":
                    cbSendCheck.SelectedIndex = 11;
                    lbSendCheckSection.Text = end.ToString();
                    break;
                default:
                    cbSendCheck.SelectedIndex = 0;
                    lbSendCheckSection.Text = "无";
                    break;
            }
            if (end != 255 && end != 0)
            {
                txtSendEnd.Text = (end - 1).ToString();

            }
            else
            {
                txtSendEnd.Text = "1";
            }
            
            
            txtSendAdd.Text = sendCode.Substring(6, 2);
            //校验选中需要添加值
            if (cbSendCheck.SelectedIndex > 5)
            {
                txtSendStart.Enabled = true;
                txtSendEnd.Enabled = true;
                txtSendAdd.Enabled = true;
            }
            else if (cbSendCheck.SelectedIndex == 0)
            {
                txtSendStart.Enabled = false;
                txtSendEnd.Enabled = false;
                txtSendAdd.Enabled = false;
            }
            else
            {
                txtSendEnd.Enabled = true;
                txtSendStart.Enabled = true;
                txtSendAdd.Enabled = false;
            }
        }

        #endregion

        #region 接收代码显示 
        private string RcvShowStyle(string rcvCode)
        {
            try
            {
                if (string.IsNullOrEmpty(rcvCode))
                {
                    return "";
                }
                //屏蔽位
                string band = DataChange.HexString2BinString(rcvCode.Substring(6, 16)).Replace(" ", "");
                //提取位
                int data1 = Convert.ToInt32(rcvCode.Substring(22, 2), 16);
                int data2 = Convert.ToInt32(rcvCode.Substring(24, 2), 16);
                int data3 = Convert.ToInt32(rcvCode.Substring(26, 2), 16);
                int data4 = Convert.ToInt32(rcvCode.Substring(28, 2), 16);
                List<string> list = new List<string>();
                //指令数据 ab cd ef
                string code = rcvCode.Substring(38, rcvCode.Length - 38);
                for (int i = 0; i < rcvCode.Length - 38; i = i + 2)
                {
                    list.Add(rcvCode.Substring(38 + i, 2));
                }

                //屏蔽位
                for (int i = 0; i < list.Count; i++)
                {
                    if (band.Substring(i, 1) == "0")
                    {
                        list[i] = "/" + list[i];
                    }
                   
                }

                //数据位
                if (data1 != 255 && data1 <= list.Count)
                {
                    list[data1 - 1] = "1#" + list[data1 - 1].Replace("/","");
                }
                if (data2 != 255 && data2 <= list.Count)
                {
                    list[data2 - 1] = "2#" + list[data2 - 1].Replace("/", "");
                }
                if (data3 != 255 && data3 <= list.Count)
                {
                    list[data3 - 1] = "3#" + list[data3 - 1].Replace("/", "");
                }
                if (data4 != 255 && data4 <= list.Count)
                {
                    list[data4 - 1] = "4#" + list[data4 - 1].Replace("/", "");
                }

                StringBuilder sb = new StringBuilder();
                if (list != null && list.Count() > 0)
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (i < list.Count() - 1)
                        {
                            sb.Append(list[i] + " ");
                        }
                        else
                        {
                            sb.Append(list[i]);
                        }
                    }
                    return sb.ToString();

                }

                return "";
            }
            catch 
            {
                //ToolsUtil.WriteLog(ex.Message + "\r\n" + ex.StackTrace + "\r\n" + ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 编辑接收代码  ID为行号
        /// </summary>
        /// <param name="id"></param>
        private void RcvCodeEdit(int id)
        {
            try
            {
                string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
                lbRcvNum.Text = id.ToString();
                if (string.IsNullOrEmpty(rcvCode))
                {
                    cbTime.Enabled = false;
                    txtRcvCode.Enabled = false;
                    txtPollingCode.Enabled = false;
                    btnSaveChange.Enabled = false;
                    btnTimeSet.Enabled = false;
                    cbRcvDealMode.Enabled = false;
                    cbRcvKeyFeedback.Enabled = false;
                    btnRcvSet.Enabled = false;
                    txtRcvCode.Text = "";
                    txtPollingCode.Text = "";
                    
                    btnRcvPortSet.Enabled = false;
                    return;
                }
                else
                {
                    cbTime.Enabled = true;
                    txtRcvCode.Enabled = true;
                    txtPollingCode.Enabled = true;
                    btnSaveChange.Enabled = true;
                    btnTimeSet.Enabled = true;
                    cbRcvDealMode.Enabled = true;
                    cbRcvKeyFeedback.Enabled = true;
                    btnRcvSet.Enabled = true;
                   
                    btnRcvPortSet.Enabled = true;

                }
                if (devRS3000.pollingTime == 0)
                {
                    cbTime.SelectedIndex = 0;
                }
                else
                {
                    cbTime.Text = devRS3000.pollingTime.ToString();

                }
                //接收码
                txtRcvCode.Text = RcvShowStyle(rcvCode);
                //屏蔽位
                string band = DataChange.HexString2BinString(rcvCode.Substring(30, 8)).Replace(" ", "");
                CheckBox[] checkBoxes = {cbRcvData17, cbRcvData16, cbRcvData15, cbRcvData14, cbRcvData13, cbRcvData12, cbRcvData11, cbRcvData10,
                    cbRcvData27, cbRcvData26, cbRcvData25, cbRcvData24, cbRcvData23, cbRcvData22, cbRcvData21, cbRcvData20,
                    cbRcvData37, cbRcvData36, cbRcvData35, cbRcvData34, cbRcvData33, cbRcvData32, cbRcvData31, cbRcvData30,
                    cbRcvData47, cbRcvData46, cbRcvData45, cbRcvData44, cbRcvData43, cbRcvData42, cbRcvData41, cbRcvData40
                };
                for (int i = 0; i < checkBoxes.Length; i++)
                {
                    if (band.Substring(i, 1) == "0")
                    {
                        checkBoxes[i].Checked = false;
                    }
                    else
                    {
                        checkBoxes[i].Checked = true;
                    }
                }
                //查询设置
                int dealRcvData = Convert.ToInt32( rcvCode.Substring(2, 2),16);
                if (dealRcvData == 0)
                {
                    cbRcvDealMode.SelectedIndex = 0;
                } else if (dealRcvData == 255)
                {
                    cbRcvDealMode.SelectedIndex = 1;
                } else
                {
                    if (cbRcvDealMode.Items.Count-1 > dealRcvData)
                    {
                        cbRcvDealMode.SelectedIndex = dealRcvData + 1;

                    }
                }
                txtPollingCode.Enabled = false;
      
                //btnRcvPortSet.Enabled = false;
                if (cbRcvDealMode.SelectedIndex == 0)
                {
                    txtPollingCode.Enabled = true;
                 
                    //btnRcvPortSet.Enabled = true;
                }
                btnRcvSet.Enabled = false;
                if (cbRcvKeyFeedback.SelectedIndex == 2)
                {
                    btnRcvSet.Enabled = true;
                }
                string pollingCode = devRS3000.portDatas[id - 1].pollingCode;
                txtPollingCode.Text = ToolsUtil.AddBlank(pollingCode);
                //按键模式
                int keyFeedback = Convert.ToInt32(rcvCode.Substring(0, 2), 16);
                cbRcvKeyFeedback.SelectedIndex = keyFeedback;
            }
            catch
            {
            }
        }

        #endregion

        #region 串口参数 轮询时间设置
        //设置串口参数
        private void BtnSet_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork_Serial;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                hexBand = GetSerialVal();
                devRS3000.baud = Convert.ToInt32(cbBaudRate.Text) ;
                devRS3000.check = cbCheck.SelectedIndex;
                devRS3000.stopBit = cbStop.SelectedIndex;

                pgv = new PgView();
                pgv.setMaxValue(12);
                pgv.setCancelEnable(false);
                //this.Enabled = false;
              
                backgroundWorker1.RunWorkerAsync();
                pgv.ShowDialog();
                if (pgv.DialogResult == DialogResult.Cancel)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackgroundWorker1_DoWork_Serial(object sender, DoWorkEventArgs e)
        {
            string hexID = DevModuel.id.ToString("X2");
            if (string.IsNullOrEmpty(hexBand))
            {
                MessageBox.Show("串口设置参数值不能为空");
                return;
            }
            int count = 0;
            string ord = string.Format("FE0010{0}FF0C01{1}CA", hexID, hexBand);
            rcvID = hexID;
            rcvPort = "FF";
            rcvReg = "0C";
            rcvType = "11";
            if (TimeOutSendOrder(ord))
            {
                count++;
                backgroundWorker1.ReportProgress(count);
                ord = string.Format("FE0010{0}FFFE02ACACCA", hexID, hexBand);
                //rcvID = hexID;
                //rcvPort = "FF";
                rcvReg = "FE";
                //rcvType = "11";

                if (TimeOutSendOrder(ord))
                {
                    count++;
                    backgroundWorker1.ReportProgress(count);
                    while (count <12)
                    {
                        count++;
                        backgroundWorker1.ReportProgress(count);
                        ToolsUtil.DelayMilli(1000);
                    }

                }
            }
        }

        //设置轮询时间
        private void BtnTimeSet_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork_Time;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                if (cbTime.SelectedIndex == 0)
                {
                    devRS3000.pollingTime = 0;
                }
                else
                {
                    devRS3000.pollingTime = Convert.ToInt32(cbTime.Text);
                }
                //this.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void BackgroundWorker1_DoWork_Time(object sender, DoWorkEventArgs e)
        {
            if (TimeOrder())
            {
                MessageBox.Show("设置成功");
            }
        }

        private bool TimeOrder()
        {
            string hexID = DevModuel.id.ToString("X2");
            string hexTime = (devRS3000.pollingTime / 20).ToString("X2");
            string ord = string.Format("FE0010{0}FF0801{1}CA", hexID, hexTime);
            rcvID = hexID;
            rcvPort = "FF";
            rcvReg = "08";
            rcvType = "11";
            Console.WriteLine("轮询时间发送："+ord) ;
            return TimeOutSendOrder(ord);
        }
        

        /// <summary>
        /// 获取串口格式设置参数
        /// </summary>
        /// <returns></returns>
        private string GetSerialVal()
        {
            string tmp;
            switch (cbBaudRate.Text)
            {
                case "1200":
                    tmp = "1";
                    break;
                case "2400":
                    tmp = "2";
                    break;
                case "4800":
                    tmp = "3";
                    break;
                case "9600":
                    tmp = "4";
                    break;
                case "19200":
                    tmp = "5";
                    break;
                case "38400":
                    tmp = "6";
                    break;
                case "57600":
                    tmp = "7";
                    break;
                case "115200":
                    tmp = "8";
                    break;
                default:
                    return "";
            }
            switch (cbCheck.SelectedIndex)
            {
                case 0:
                    if (cbStop.SelectedIndex == 0)
                    {
                        tmp = tmp + "0";

                    }
                    else
                    {
                        tmp = tmp + "3";
                    }
                    break;
                case 1:
                    if (cbStop.SelectedIndex == 0)
                    {
                        tmp = tmp + "1";

                    }
                    else
                    {
                        tmp = tmp + "4";
                    }
                    break;
                case 2:
                    if (cbStop.SelectedIndex == 0)
                    {
                        tmp = tmp + "2";

                    }
                    else
                    {
                        tmp = tmp + "5";
                    }
                    break;
                default:
                    return "";
            }
            return tmp;

        }

        private void SetSerialVal(string val)
        {
            if (devRS3000 == null)
            {
                return;
            }
            if (val.Length != 2)
            {
                return;
            }
            switch (val.Substring(0,1))
            {
                case "1":
                    devRS3000.baud = 1200;
                    break;
                case "2":
                    devRS3000.baud = 2400;
                    break;
                case "3":
                    devRS3000.baud = 4800;
                    break;
                case "4":
                    devRS3000.baud = 9600;
                    break;
                case "5":
                    devRS3000.baud = 19200;
                    break;
                case "6":
                    devRS3000.baud = 38400;
                    break;
                case "7":
                    devRS3000.baud = 57600;
                    break;
                case "8":
                    devRS3000.baud = 115200;
                    break;
                default:
                    return ;
            }
            switch (val.Substring(1, 1))
            {
                case "0":
                    devRS3000.check = 0;
                    devRS3000.stopBit = 0;
                    break;
                case "1":
                    devRS3000.check = 1;
                    devRS3000.stopBit = 0;
                    break;
                case "2":
                    devRS3000.check = 2;
                    devRS3000.stopBit = 0;
                    break;
                case "3":
                    devRS3000.check = 0;
                    devRS3000.stopBit = 1;
                    break;
                case "4":
                    devRS3000.check = 1;
                    devRS3000.stopBit = 1;
                    break;
                case "5":
                    devRS3000.check = 2;
                    devRS3000.stopBit = 1;
                    break;
                default:
                    return ;
            }

        }

        #endregion

        #region 表格操作
        private bool isFirstClick = true;
        private bool isDoubleClick = false;
        private int milliseconds = 0;
        /// <summary>
        /// 行号
        /// </summary>
        private int rowCount = 0;
        /// <summary>
        /// 列号
        /// </summary>
        private int columnCount = 0;
        private int oldrowCount = 0;
        private int oldcolumnCount = 0;
        bool isClick = false;
        bool isEdit = false;
        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            DgvMesege.endDataViewCurrent(dataGridView1, e.Y, e.X);
        }

        private void DataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (isClick == true)
            {
                return;

            }
            else
            {
                //选中行号
                /*int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[rowNum].Selected = true;//选中行
                }*/
            }
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            oldrowCount = rowCount;
            oldcolumnCount = columnCount;
            rowCount = e.RowIndex;
            columnCount = e.ColumnIndex;
            // 鼠标单击.
            if (isFirstClick)
            {
                isFirstClick = false;
                doubleClickTimer.Start();
            }
            // 鼠标双击
            else
            {

                isDoubleClick = true;
            }
            if (isClick)
            {
                if (dataGridView1.SelectedCells.Count == 1 && rowCount == oldrowCount && columnCount == oldcolumnCount)
                {
                    return;

                }
                else if (oldcolumnCount == columnCount)
                {
                    return;
                }
                isClick = false;
            }
            else
            {
                isClick = true;
            }
        }

        //单双击
        private void DoubleClickTimer_Tick(object sender, EventArgs e)
        {
            milliseconds += 100;
            // 第二次鼠标点击超出双击事件间隔
            if (milliseconds >= SystemInformation.DoubleClickTime)
            {
                doubleClickTimer.Stop();
                if (isDoubleClick)
                {

                    if (rowCount >= 0 && columnCount >= 0)
                    {
                        int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);

                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "id":
                                break;
                            case "name":
                                dataGridView1.Columns[1].ReadOnly = false;
                                break;
                            case "send":
                                if (dataGridView1.Columns[2].ReadOnly)
                                {
                                    dataGridView1.Columns[2].ReadOnly = false;

                                }
                                break;
                            case "rcv":
                                if (dataGridView1.Columns[3].ReadOnly)
                                {
                                    dataGridView1.Columns[3].ReadOnly = false;

                                }
                                break;


                            default: break;
                        }
                        /*try
                        {
                            //更改内容回自动刷新到第一行
                            dataGridView1.CurrentCell = dataGridView1.Rows[rowCount].Cells[columnCount];
                        }
                        catch
                        {
                            if (dataGridView1.Rows.Count > 0)
                            {
                                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[columnCount];
                            }

                        }*/
                    }
                }
                else
                {
                    //处理单击事件操作

                    if (rowCount >= 0 && columnCount >= 0)
                    {
                        //DGV的行号
                        int id = Convert.ToInt32(dataGridView1.Rows[rowCount].Cells[0].Value);
                        switch (dataGridView1.Columns[columnCount].Name)
                        {
                            case "id":

                                break;
                            case "name":

                                break;
                            case "send":
                                if (dataGridView1.Columns[2].ReadOnly == true)
                                {
                                    SendCodeEdit(id);
                                    TableShow(1);
                                }
                                //计算长度
                                if (dataGridView1.Rows[rowCount].Cells[columnCount].Value == null || string.IsNullOrEmpty(dataGridView1.Rows[rowCount].Cells[columnCount].Value.ToString()))
                                {
                                    GetStrSectionLen(null, null, 0,true);
                                }
                                else
                                {
                                    GetStrSectionLen(dataGridView1.Rows[rowCount].Cells[columnCount].Value.ToString(), null, 0, true);
                                }
                                break;
                            case "rcv":
                                if (dataGridView1.Columns[3].ReadOnly == true)
                                {
                                    RcvCodeEdit(id);
                                    TableShow(2);
                                }
                                //计算长度
                                if (dataGridView1.Rows[rowCount].Cells[columnCount].Value == null || string.IsNullOrEmpty(dataGridView1.Rows[rowCount].Cells[columnCount].Value.ToString()))
                                {
                                    GetStrSectionLen(null, null, 0,false);
                                }
                                else
                                {
                                    GetStrSectionLen(dataGridView1.Rows[rowCount].Cells[columnCount].Value.ToString(), null, 0, false);
                                }
                                break;
                            default: break;
                        }
                        
                        if (!isEdit)
                        {
                            dataGridView1.Focus();

                        }


                    }
                }
                isFirstClick = true;
                isDoubleClick = false;
                milliseconds = 0;
            }
        }

      
        //开始编辑
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                isEdit = true;
                dataGridView1.ContextMenuStrip = null;
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                string code;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView1.Columns[columnNum].Name)
                    {

                        case "name":

                            break;
                        case "send":
                           /* //输入为大写
                            DataGridViewTextBoxEditingControl tb = dataGridView1.EditingControl as DataGridViewTextBoxEditingControl;
                            tb.CharacterCasing = CharacterCasing.Upper;
                            //输入为十六进制 和空格 退格
                            tb.KeyPress += new KeyPressEventHandler(txtNumber_KeyPress);
                            tb.KeyUp += new KeyEventHandler(txtNumber_KeyUp);*/
                            if (dataGridView1.Rows[rowNum].Cells[columnNum].Value != null && !string.IsNullOrEmpty(dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString()))
                            {
                                code = devRS3000.portDatas[rowNum].sendCode;
                                if (!string.IsNullOrEmpty(code))
                                {
                                    dataGridView1.Rows[rowNum].Cells[columnNum].Value = ToolsUtil.AddBlank(code.Substring(24, code.Length - 24));
                                }

                            }
                            break;
                        case "rcv":

                            if (dataGridView1.Rows[rowNum].Cells[columnNum].Value != null && !string.IsNullOrEmpty(dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString()))
                            {
                                code = devRS3000.portDatas[rowNum].rcvCode;
                                if (!string.IsNullOrEmpty(code))
                                {
                                    dataGridView1.Rows[rowNum].Cells[columnNum].Value = ToolsUtil.AddBlank(code.Substring(38, code.Length - 38));
                                }

                            }
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        //结束编辑机
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                isEdit = false;
                dataGridView1.ContextMenuStrip = contextMenuStrip3;
                //选中行号
                int rowNum = e.RowIndex;
                //选中列号
                int columnNum = e.ColumnIndex;
                string code;
                if (rowNum >= 0 && columnNum >= 0)
                {
                    switch (dataGridView1.Columns[columnNum].Name)
                    {

                        case "name":
                            if (dataGridView1.Rows[rowNum].Cells[columnNum].Value == null || string.IsNullOrEmpty(dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString()))
                            {
                                devRS3000.portDatas[rowNum].name = "";
                            }
                            else
                            {
                                devRS3000.portDatas[rowNum].name = dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString();
                            }
                            dataGridView1.Columns[1].ReadOnly = true;
                            break;
                        case "send":
                            if (dataGridView1.Rows[rowNum].Cells[columnNum].Value == null || string.IsNullOrEmpty(dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString()))
                            {
                                devRS3000.portDatas[rowNum].sendCode = "";
                            }
                            else
                            {
                                //替换code 
                                code = dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString().Replace(" ", "").Replace("CK","").Replace("1#", "").Replace("2#", "").Replace("3#", "").Replace("4#", "");
                                if (!string.IsNullOrEmpty(devRS3000.portDatas[rowNum].sendCode) && code == devRS3000.portDatas[rowNum].sendCode.Substring(24, devRS3000.portDatas[rowNum].sendCode.Length - 24))
                                {
                                    //显示当前代码
                                    dataGridView1.Rows[rowNum].Cells[columnNum].Value = SendShowStyle(devRS3000.portDatas[rowNum].sendCode);
                                    dataGridView1.Columns[2].ReadOnly = true;
                                    return;
                                }
                                if (code.Length % 2 == 1)
                                {
                                    //确保不为单数
                                    code = code.Substring(0, code.Length - 1);
                                }
                                if (code.Length > 98)
                                {
                                    dataGridView1.Rows[rowNum].Cells[columnNum].Value = SendShowStyle(devRS3000.portDatas[rowNum].sendCode);
                                    return;
                                }
                                if (string.IsNullOrEmpty(devRS3000.portDatas[rowNum].sendCode))
                                {
                                    //添加默认的
                                    devRS3000.portDatas[rowNum].sendCode = "800101FFFFFFFFFF01020304" + code;
                                    //devRS3000.portDatas[rowNum].sendCode = "000101FFFFFFFFFF01020304" + code;
                                }
                                else
                                {
                                    devRS3000.portDatas[rowNum].sendCode = devRS3000.portDatas[rowNum].sendCode.Substring(0, 24) + code;
                                }
                                //显示当前代码
                                dataGridView1.Rows[rowNum].Cells[columnNum].Value = SendShowStyle(devRS3000.portDatas[rowNum].sendCode);
                            }
                            dataGridView1.Columns[2].ReadOnly = true;
                            break;
                        case "rcv":
                            if (dataGridView1.Rows[rowNum].Cells[columnNum].Value == null || string.IsNullOrEmpty(dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString()))
                            {
                                devRS3000.portDatas[rowNum].rcvCode = "";
                                devRS3000.portDatas[rowNum].pollingCode = "";
                            }
                            else
                            {
                                //替换code 
                                code = dataGridView1.Rows[rowNum].Cells[columnNum].Value.ToString().Replace(" ", "").Replace("1#", "").Replace("2#", "").Replace("3#", "").Replace("4#", ""); ;
                                if (!string.IsNullOrEmpty(devRS3000.portDatas[rowNum].rcvCode) && code == devRS3000.portDatas[rowNum].rcvCode.Substring(38, devRS3000.portDatas[rowNum].rcvCode.Length - 38))
                                {
                                    //代码相同
                                    dataGridView1.Rows[rowNum].Cells[columnNum].Value = RcvShowStyle(devRS3000.portDatas[rowNum].rcvCode);
                                    dataGridView1.Columns[3].ReadOnly = true;
                                    return;
                                }
                                if (code.Length % 2 == 1)
                                {
                                    //确保不为单数
                                    code = code.Substring(0, code.Length - 1);
                                }
                                if (code.Length > 86)
                                {
                                    dataGridView1.Rows[rowNum].Cells[columnNum].Value = RcvShowStyle(devRS3000.portDatas[rowNum].rcvCode);
                                    return;
                                }
                                if (string.IsNullOrEmpty(devRS3000.portDatas[rowNum].rcvCode))
                                {
                                    //添加默认的
                                    devRS3000.portDatas[rowNum].rcvCode = "01FF00" + GetBandSection(code.Length) + "FFFFFFFFFFFFFFFF" + code;
                                }
                                else
                                {
                                    string heart = devRS3000.portDatas[rowNum].rcvCode.Substring(0, 6);
                                    string data = devRS3000.portDatas[rowNum].rcvCode.Substring(22, 16);
                                    if (code.Length == devRS3000.portDatas[rowNum].rcvCode.Length - 38)
                                    {
                                        //修改部分字符
                                        devRS3000.portDatas[rowNum].rcvCode = devRS3000.portDatas[rowNum].rcvCode.Substring(0,38) + code;

                                    }
                                    else
                                    {
                                        devRS3000.portDatas[rowNum].rcvCode = heart + GetBandSection(code.Length) + "FFFFFFFFFFFFFFFF" + code;

                                    }
                                }
                                //显示当前代码
                                dataGridView1.Rows[rowNum].Cells[columnNum].Value = RcvShowStyle(devRS3000.portDatas[rowNum].rcvCode);
                            }
                            dataGridView1.Columns[3].ReadOnly = true;
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        #region  限制输入
        private bool isHex = false;
        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
           
            if (dataGridView1.CurrentCell.ColumnIndex == 3 || dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                isHex = true;
                ((TextBox)e.Control).CharacterCasing = CharacterCasing.Upper;
                ((TextBox)e.Control).KeyPress += new KeyPressEventHandler(txtNumber_KeyPress);
                ((TextBox)e.Control).KeyUp += new KeyEventHandler(txtNumber_KeyUp);
                Console.WriteLine("十六进制 当前活动的单元格：" + dataGridView1.CurrentCell.ColumnIndex.ToString());

            }
            else if (dataGridView1.CurrentCell.ColumnIndex == 1)
            {
                isHex = false;
                ((TextBox)e.Control).CharacterCasing = CharacterCasing.Normal;
                Console.WriteLine("所有可以输入 当前活动的单元格：" + dataGridView1.CurrentCell.ColumnIndex.ToString());

            }


        }

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isHex) {
                //如果输入的不是16进制，也不是回车键、Backspace键，则取消该输入
                if (e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space && "0123456789ABCDEF".IndexOf(char.ToUpper(e.KeyChar)) < 0)
                {
                    e.Handled = true;
                }
            }
           
        }

        private void txtNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.V ))
            {
                if (Clipboard.ContainsText())
                {
                    if (isHex)
                    {
                        string tmp = Clipboard.GetText().Replace(" ", "");
                        if (Validator.IsHexadecimal(tmp))
                        {
                            ((TextBox)sender).Text = Clipboard.GetText().Trim(); //Ctrl+V 粘贴 

                        }
                    }
                    else
                    {
                        ((TextBox)sender).Text = Clipboard.GetText().Trim(); //Ctrl+V 粘贴 
                    }
                    
                }
            }
            else if(e.KeyData == (Keys.Control | Keys.C))
            {
                if(!string.IsNullOrEmpty(((TextBox)sender).SelectedText))
                {
                    Clipboard.SetText(((TextBox)sender).SelectedText);

                }

            }

        }

      

        #endregion

        #region 记录滑动条位置
        //滑动条位置
        int X_Value; // Stores position of Horizontal scroll bar
        int Y_Value; // Stores position of Vertical scroll bar
        private void DataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                X_Value = e.NewValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Y_Value = e.NewValue;
            }

        }


        #endregion


        #endregion

        #region 发送界面

        #region 菜单栏
        private void TxtSendCode_MouseUp(object sender, MouseEventArgs e)
        {
            GetStrSectionLen(txtSendCode.Text, txtSendCode.SelectedText, txtSendCode.SelectionStart,true);

        }


        private void TextBoxX1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //禁止一切输入
            e.Handled = true;
        }

        #region 本端口数据
        private void 数据1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1,"01");
        }

        private void 数据2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(2, "02");
        }

        private void 数据3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(3, "03");
        }

        private void 数据4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(4, "04");
        }
        #endregion

        #region 其他端口数据1
        private void 下一个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "11");
        }

        private void 下二个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "21");
        }

        private void 下三个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "31");
        }

        private void 下四个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "41");
        }

        private void 下五个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "51");
        }

        private void 下六个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "61");
        }

        private void 下七个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "71");
        }

        private void 下八个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "81");
        }

        private void 下九个端口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectData(1, "91");
        }
        #endregion

        #region 其他端口数据2
        private void 下一个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "12");
        }

        private void 下二个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "22");
        }

        private void 下三个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "32");
        }

        private void 下四个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "42");
        }

        private void 下五个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "52");
        }

        private void 下六个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "62");
        }

        private void 下七个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "72");
        }

        private void 下八个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "82");
        }

        private void 下九个端口ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectData(2, "92");
        }
        #endregion

        #region 其他端口数据3
        private void 下一个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "13");
        }

        private void 下二个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "23");
        }

        private void 下三个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "33");
        }
        private void 下四个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "43");
        }

        private void 下五个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "53");
        }

        private void 下六个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "63");
        }

        private void 下七个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "73");
        }

        private void 下八个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "83");
        }

        private void 下九个端口ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectData(3, "93");
        }

        #endregion

        #region 其他端口数据4
        private void 下一个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "14");
        }

        private void 下二个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "24");
        }

        private void 下三个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "34");
        }

        private void 下四个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "44");
        }

        private void 下五个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "54");
        }

        private void 下六个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "64");
        }

        private void 下七个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "74");
        }

        private void 下八个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "84");
        }

        private void 下九个端口ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectData(4, "94");
        }
        #endregion

        /// <summary>
        /// 选中数据和Dr值
        /// </summary>
        /// <param name="dataNum"></param>
        /// <param name="dr"></param>
        private void SelectData(int dataNum,string dr)
        {
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;

            if (string.IsNullOrEmpty(txtSendCode.SelectedText))
            {
                //确保选中字节为双数
                return;
            }
            if (txtSendCode.SelectedText.Contains("CK"))
            {
                return;
            }
            int selectStart = txtSendCode.SelectionStart + 1;

            //当前选中位置
            if (txtSendCode.SelectedText.Substring(0, 1) != " ")
            {
                selectStart = selectStart - 1;
            }
            string beforCode = txtSendCode.Text.Substring(0, selectStart);
            selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#")*3 - ToolsUtil.SubstringCount(beforCode, "CK") * 2;

            string dataNow =((selectStart/2)+1).ToString("X2");

            string data1 = sendCode.Substring(8, 2);
            string data2 = sendCode.Substring(10, 2);
            string data3 = sendCode.Substring(12, 2);
            string data4 = sendCode.Substring(14, 2);
            //当前选中位置
            if (dataNow == data1)
            {
                sendCode = sendCode.Remove(8, 2);
                sendCode = sendCode.Insert(8, "FF");
                sendCode = sendCode.Remove(16, 2);
                sendCode = sendCode.Insert(16, "01");
            }
            if (dataNow == data2)
            {
                sendCode = sendCode.Remove(10, 2);
                sendCode = sendCode.Insert(10, "FF");
                sendCode = sendCode.Remove(18, 2);
                sendCode = sendCode.Insert(18, "02");
            }
            if (dataNow == data3)
            {
                sendCode = sendCode.Remove(12, 2);
                sendCode = sendCode.Insert(12, "FF");
                sendCode = sendCode.Remove(20, 2);
                sendCode = sendCode.Insert(20, "03");
            }
            if (dataNow == data4)
            {
                sendCode = sendCode.Remove(14, 2);
                sendCode = sendCode.Insert(14, "FF");
                sendCode = sendCode.Remove(22, 2);
                sendCode = sendCode.Insert(22, "04");
            }
            //插入新的
            if (dataNum == 1)
            {
                sendCode = sendCode.Remove(8, 2);
                sendCode = sendCode.Insert(8, dataNow);
                sendCode = sendCode.Remove(16, 2);
                sendCode = sendCode.Insert(16, dr);
            }
            else if (dataNum == 2)
            {
                sendCode = sendCode.Remove(10, 2);
                sendCode = sendCode.Insert(10, dataNow);
                sendCode = sendCode.Remove(18, 2);
                sendCode = sendCode.Insert(18, dr);
            }
            else if (dataNum == 3)
            {
                sendCode = sendCode.Remove(12, 2);
                sendCode = sendCode.Insert(12, dataNow);
                sendCode = sendCode.Remove(20, 2);
                sendCode = sendCode.Insert(20, dr);
            }
            else if (dataNum == 4)
            {
                sendCode = sendCode.Remove(14, 2);
                sendCode = sendCode.Insert(14, dataNow);
                sendCode = sendCode.Remove(22, 2);
                sendCode = sendCode.Insert(22, dr);
            }


            devRS3000.portDatas[id - 1].sendCode = sendCode;
            txtSendCode.Text = SendShowStyle(sendCode);
            dataGridView1.Rows[id - 1].Cells[2].Value = txtSendCode.Text;

           
        }


        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            List<string> list = txtSendCode.Text.Split(' ').ToList<string>();
            string sleTxt = txtSendCode.SelectedText.Trim();
            if (string.IsNullOrEmpty(sleTxt) || sleTxt.Length != 5)
            {
                return;
            }
            if (!sleTxt.Contains('#') || sleTxt.Contains("CK"))
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == sleTxt)
                {
                    string data1 = sendCode.Substring(8, 2);
                    string data2 = sendCode.Substring(10, 2);
                    string data3 = sendCode.Substring(12, 2);
                    string data4 = sendCode.Substring(14, 2);
                    string dataNow = (i + 1).ToString("X2");
                    if (dataNow == data1)
                    {
                        sendCode = sendCode.Remove(8, 2);
                        sendCode = sendCode.Insert(8, "FF");
                        sendCode = sendCode.Remove(16, 2);
                        sendCode = sendCode.Insert(16, "01");
                    }
                    if (dataNow == data2)
                    {
                        sendCode = sendCode.Remove(10, 2);
                        sendCode = sendCode.Insert(10, "FF");
                        sendCode = sendCode.Remove(18, 2);
                        sendCode = sendCode.Insert(18, "02");
                    }
                    if (dataNow == data3)
                    {
                        sendCode = sendCode.Remove(12, 2);
                        sendCode = sendCode.Insert(12, "FF");
                        sendCode = sendCode.Remove(20, 2);
                        sendCode = sendCode.Insert(20, "03");
                    }
                    if (dataNow == data4)
                    {
                        sendCode = sendCode.Remove(14, 2);
                        sendCode = sendCode.Insert(14, "FF");
                        sendCode = sendCode.Remove(22, 2);
                        sendCode = sendCode.Insert(22, "04");
                    }
                    devRS3000.portDatas[id - 1].sendCode = sendCode;
                    txtSendCode.Text = SendShowStyle(sendCode);
                    dataGridView1.Rows[id - 1].Cells[2].Value = txtSendCode.Text;
                    return;
                }
            }


        }


        private void 删除全部ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            sendCode = sendCode.Remove(8, 16);
            sendCode = sendCode.Insert(8, "FFFFFFFF01020304");
            devRS3000.portDatas[id - 1].sendCode = sendCode;
            txtSendCode.Text = SendShowStyle(sendCode);
            dataGridView1.Rows[id - 1].Cells[2].Value = txtSendCode.Text;
        }

        #endregion

        #region 参数改变
        private void CbSendCheck_SelectedIndexChanged(object sender, EventArgs e)
        {
            //校验选中需要添加值
            if (cbSendCheck.SelectedIndex > 5)
            {
                txtSendStart.Enabled = true;
                txtSendEnd.Enabled = true;
                txtSendAdd.Enabled = true;
            }
            else if (cbSendCheck.SelectedIndex == 0)
            {
                txtSendStart.Enabled = false;
                txtSendEnd.Enabled = false;
                txtSendAdd.Enabled = false;
                txtSendStart.Text = "1";
                txtSendEnd.Text = "1";
                txtSendAdd.Text = "FF";
            }
            else
            {
                txtSendEnd.Enabled = true;
                txtSendStart.Enabled = true;
                txtSendAdd.Text = "FF";
                txtSendAdd.Enabled = false;
            }
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            int end = Convert.ToInt32(sendCode.Substring(2, 2), 16) +1;
            string check;
            bool isNode = false;
            switch (cbSendCheck.SelectedIndex)
            {
                case 0:
                    check = "80";
                    isNode = true;
                    lbSendCheckSection.Text = "无";
                    break;
                case 1:
                    check = "81";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 2:
                    check = "82";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 3:
                    check = "83";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 4:
                    check = "84";
                    lbSendCheckSection.Text = string.Format("{0},{1}", end, end + 1);
                    break;
                case 5:
                    check = "85";
                    lbSendCheckSection.Text = string.Format("{0},{1}", end, end + 1);
                    break;
                case 6:
                    check = "91";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 7:
                    check = "92";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 8:
                    check = "93";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 9:
                    check = "A1";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 10:
                    check = "A2";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                case 11:
                    check = "A3";
                    lbSendCheckSection.Text = end.ToString();
                    break;
                default:
                    check = "80";
                    isNode = true;
                    lbSendCheckSection.Text = "无";
                    break;
            }
            sendCode = sendCode.Remove(0, 2);
            sendCode = sendCode.Insert(0, check);
            if (isNode)
            {
                sendCode = sendCode.Remove(2, 4);
                sendCode = sendCode.Insert(2, "0000");
            }
            txtSendCode.Text = SendShowStyle(sendCode);
            dataGridView1.Rows[id-1].Cells[2].Value = txtSendCode.Text;
            devRS3000.portDatas[id - 1].sendCode = sendCode;
        }
        private void TxtSendStart_TextChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            if (string.IsNullOrEmpty(txtSendStart.Text))
            {
                return;
            }
            int start = Convert.ToInt32(txtSendStart.Text);
            if (!string.IsNullOrEmpty(txtSendEnd.Text))
            {
                int end = Convert.ToInt32(txtSendEnd.Text);
                if (start > end)
                {
                    MessageBox.Show("校验起始位或结束位错误，请检查");
                    return;

                }
            }
            List<string> list = txtSendCode.Text.Split(' ').ToList<string>();

            if (list.Count < start)
            {
                MessageBox.Show("校验起始位或结束位错误，请检查");
                return;
            }
            sendCode = sendCode.Remove(4, 2);
            sendCode = sendCode.Insert(4, start.ToString("X2"));
            devRS3000.portDatas[id - 1].sendCode = sendCode;
        }

        private void TxtSendEnd_TextChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            if (string.IsNullOrEmpty(txtSendEnd.Text))
            {
                return;
            }
            if (cbSendCheck.SelectedIndex == 0 || cbSendCheck.SelectedIndex == -1)
            {
                return;
            }
            int end = Convert.ToInt32(txtSendEnd.Text);
            if (!string.IsNullOrEmpty(txtSendStart.Text))
            {
                int start = Convert.ToInt32(txtSendStart.Text);
                if (start > end)
                {
                    MessageBox.Show("校验起始位或结束位错误，请检查");
                    return;

                }
            }
            List<string> list = txtSendCode.Text.Split(' ').ToList<string>();
            end = end + 1;

            if (cbSendCheck.SelectedIndex == 4 || cbSendCheck.SelectedIndex == 5)
            {
                if (list.Count <= end )
                {
                    MessageBox.Show("校验结束位错误，请检查");
                    return;
                }
                lbSendCheckSection.Text = string.Format("{0},{1}", end,end+1);
            }
            else
            {
                if (list.Count < end)
                {
                    MessageBox.Show("校验结束位错误，请检查");
                    return;
                }
                lbSendCheckSection.Text = end.ToString();
            }
            
        
            sendCode = sendCode.Remove(2, 2);
            sendCode = sendCode.Insert(2, end.ToString("X2"));
            
            txtSendCode.Text = SendShowStyle(sendCode);
            dataGridView1.Rows[id - 1].Cells[2].Value = txtSendCode.Text;
            devRS3000.portDatas[id - 1].sendCode = sendCode;
        }

        private void TxtSendStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtSendEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtSendAdd_TextChanged(object sender, EventArgs e)
        {
       
            int id = Convert.ToInt32(lbSendNum.Text);
            string sendCode = devRS3000.portDatas[id - 1].sendCode;
            string add = txtSendAdd.Text;
            if (add.Length == 1)
            {
                add = "0" + add;
            }
            else if(add.Length != 2)
            {
                return;
            }
            sendCode = sendCode.Remove(6, 2);
            sendCode = sendCode.Insert(6, add);
            devRS3000.portDatas[id - 1].sendCode = sendCode;
        }

        private void TxtSendAdd_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是16进制，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space && "0123456789ABCDEF".IndexOf(char.ToUpper(e.KeyChar)) < 0 )
            {
                e.Handled = true;
            }
        }


        //高级设置
        private void BtnSendSet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSendCode.Text))
            {
                TableShow(3);
                lbSendHnum.Text = lbSendNum.Text;
                TableCustomStyle();

            }
        }

        private void BtnSendTest_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbSendNum.Text);
            string hexID = DevModuel.id.ToString("X2");
            string hexPort = id.ToString("X2"); 
            string ord = string.Format("FE0010{0}{1}000400000000CA", hexID, hexPort);
            client5000.SendHexAsync(ord);
         
        }

        #endregion

        #endregion

        #region 接收界面
        private void CbTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是16进制，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtRcvCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            //禁止一切输入
            e.Handled = true;
        }

        private void TxtRcvCode_MouseUp(object sender, MouseEventArgs e)
        {
            GetStrSectionLen(txtRcvCode.Text, txtRcvCode.SelectedText, txtRcvCode.SelectionStart,false);
        }

        private void 提取数据1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractData(1);
        }

        private void 提取数据2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractData(2);
        }

        private void 提取数据3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractData(3);
        }

        private void 提取数据4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractData(4);
        }

        private void ExtractData(int dataNum)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(txtRcvCode.SelectedText))
            {
                
                return;
            }
            int selectStart = txtRcvCode.SelectionStart + 1;

            //当前选中位置
            if (txtRcvCode.SelectedText.Substring(0, 1) != " ")
            {
                selectStart = selectStart - 1;
            }
            string beforCode = txtRcvCode.Text.Substring(0, selectStart);
            selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#") * 2 - ToolsUtil.SubstringCount(beforCode, "/");
            int index = (selectStart / 2) + 1;
            string dataNow = index.ToString("X2");
            string data1 = rcvCode.Substring(22, 2);
            string data2 = rcvCode.Substring(24, 2);
            string data3 = rcvCode.Substring(26, 2);
            string data4 = rcvCode.Substring(28, 2);
            //清除旧的数据位
            if (dataNow == data1)
            {
                rcvCode = rcvCode.Remove(22, 2);
                rcvCode = rcvCode.Insert(22, "FF");
            }
            if (dataNow == data2)
            {
                rcvCode = rcvCode.Remove(24, 2);
                rcvCode = rcvCode.Insert(24, "FF");
            }
            if (dataNow == data3)
            {
                rcvCode = rcvCode.Remove(26, 2);
                rcvCode = rcvCode.Insert(26, "FF");
            }
            if (dataNow == data4)
            {
                rcvCode = rcvCode.Remove(28, 2);
                rcvCode = rcvCode.Insert(28, "FF");
            }
            //清除旧的屏蔽位
            //屏蔽位
            int bandNum;
            if (1 <= index && index <= 8)
            {
                bandNum = 6;
            }
            else if (9 <= index && index <= 16)
            {
                bandNum = 8;
            }
            else if (17 <= index && index <= 24)
            {
                bandNum = 10;
            }
            else if (25 <= index && index <= 32)
            {
                bandNum = 12;
            }
            else if (33 <= index && index <= 40)
            {
                bandNum = 14;
            }
            else if (41 <= index && index <= 48)
            {
                bandNum = 16;
            }
            else
            {
                bandNum = 6;
            }
            string band = DataChange.HexString2BinString(rcvCode.Substring(bandNum, 2)).Replace(" ", "");
            int bitNum = index % 8;
            if (bitNum == 0)
            {
                bitNum = 8;
            }
            band = band.Remove(bitNum - 1, 1);
            band = band.Insert(bitNum - 1, "0");
            rcvCode = rcvCode.Remove(bandNum, 2);
            rcvCode = rcvCode.Insert(bandNum, Convert.ToInt32(band, 2).ToString("X2"));

            //插入新的
            if (dataNum == 1)
            {
                rcvCode = rcvCode.Remove(22, 2);
                rcvCode = rcvCode.Insert(22, dataNow);
            }
            else if (dataNum == 2)
            {
                rcvCode = rcvCode.Remove(24, 2);
                rcvCode = rcvCode.Insert(24, dataNow);
            }
            else if (dataNum == 3)
            {
                rcvCode = rcvCode.Remove(26, 2);
                rcvCode = rcvCode.Insert(26, dataNow);
            }
            else if (dataNum == 4)
            {
                rcvCode = rcvCode.Remove(28, 2);
                rcvCode = rcvCode.Insert(28, dataNow);
            }
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            txtRcvCode.Text = RcvShowStyle(rcvCode);
            dataGridView1.Rows[id - 1].Cells[3].Value = txtRcvCode.Text;
            Console.WriteLine(ToolsUtil.AddBlank(rcvCode));
        }

        private void 屏蔽不比较数值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(txtRcvCode.SelectedText))
            {

                return;
            }
            int selectStart = txtRcvCode.SelectionStart + 1;

            //当前选中位置
            if (txtRcvCode.SelectedText.Substring(0, 1) != " ")
            {
                selectStart = selectStart - 1;
            }
            string beforCode = txtRcvCode.Text.Substring(0, selectStart);
            selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#") * 2 - ToolsUtil.SubstringCount(beforCode, "/");
            //当前选中数据的位置
            int index = (selectStart / 2) + 1;
            string dataNow = index.ToString("X2");
            string data1 = rcvCode.Substring(22, 2);
            string data2 = rcvCode.Substring(24, 2);
            string data3 = rcvCode.Substring(26, 2);
            string data4 = rcvCode.Substring(28, 2);
            //当前选中位置
            if (dataNow == data1)
            {
                rcvCode = rcvCode.Remove(22, 2);
                rcvCode = rcvCode.Insert(22, "FF");
            }
            if (dataNow == data2)
            {
                rcvCode = rcvCode.Remove(24, 2);
                rcvCode = rcvCode.Insert(24, "FF");
            }
            if (dataNow == data3)
            {
                rcvCode = rcvCode.Remove(26, 2);
                rcvCode = rcvCode.Insert(26, "FF");
            }
            if (dataNow == data4)
            {
                rcvCode = rcvCode.Remove(28, 2);
                rcvCode = rcvCode.Insert(28, "FF");
            }

            //比较屏蔽
            int bandNum ;
            if (1 <= index && index <= 8)
            {
                bandNum = 6;
            }
            else if (9 <= index && index <= 16)
            {
                bandNum = 8;
            }
            else if (17 <= index && index <= 24)
            {
                bandNum = 10;
            }
            else if (25 <= index && index <= 32)
            {
                bandNum = 12;
            }
            else if (33 <= index && index <= 40)
            {
                bandNum = 14;
            }
            else if (41 <= index && index <= 48)
            {
                bandNum = 16;
            }
            else
            {
                bandNum = 6;
            }
            string band = DataChange.HexString2BinString(rcvCode.Substring(bandNum, 2)).Replace(" ", "");
            int bitNum = index % 8;
            if (bitNum == 0)
            {
                bitNum = 8;
            }
            band = band.Remove(bitNum - 1,1);
            band = band.Insert(bitNum - 1, "0");
            
            rcvCode = rcvCode.Remove(bandNum, 2);
            rcvCode = rcvCode.Insert(bandNum, Convert.ToInt32(band, 2).ToString("X2"));

            //ui更新
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            txtRcvCode.Text = RcvShowStyle(rcvCode);
            dataGridView1.Rows[id - 1].Cells[3].Value = txtRcvCode.Text;
            Console.WriteLine(ToolsUtil.AddBlank(rcvCode));*/
            RcvBandBit();
        }


        /// <summary>
        /// 屏蔽位
        /// </summary>
        private void RcvBandBit() {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(txtRcvCode.SelectedText))
            {
                return;
            }
            int selectStart = txtRcvCode.SelectionStart + 1;
           
            //选中数据的个数
            int selectCount = txtRcvCode.SelectedText.Length - ToolsUtil.SubstringCount(txtRcvCode.SelectedText, " ") - ToolsUtil.SubstringCount(txtRcvCode.SelectedText, "#") * 2 - ToolsUtil.SubstringCount(txtRcvCode.SelectedText, "/");
            if (selectCount % 2 != 0)
            {
                selectCount++;
            }
            //当前选中位置
            if (txtRcvCode.SelectedText.Substring(0, 1) != " ")
            {
                selectStart = selectStart - 1;
            }
            
            string beforCode = txtRcvCode.Text.Substring(0, selectStart);
            selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#") * 2 - ToolsUtil.SubstringCount(beforCode, "/");
            //当前选中数据的位置
            int index = (selectStart / 2) + 1;
            //比较屏蔽
            int bandNum;
            for (int i = index; i < selectCount / 2 + index; i++)
            {
                string dataNow = i.ToString("X2");
                string data1 = rcvCode.Substring(22, 2);
                string data2 = rcvCode.Substring(24, 2);
                string data3 = rcvCode.Substring(26, 2);
                string data4 = rcvCode.Substring(28, 2);
                //当前选中位置
                if (dataNow == data1)
                {
                    rcvCode = rcvCode.Remove(22, 2);
                    rcvCode = rcvCode.Insert(22, "FF");
                }
                if (dataNow == data2)
                {
                    rcvCode = rcvCode.Remove(24, 2);
                    rcvCode = rcvCode.Insert(24, "FF");
                }
                if (dataNow == data3)
                {
                    rcvCode = rcvCode.Remove(26, 2);
                    rcvCode = rcvCode.Insert(26, "FF");
                }
                if (dataNow == data4)
                {
                    rcvCode = rcvCode.Remove(28, 2);
                    rcvCode = rcvCode.Insert(28, "FF");
                }

                
                if (1 <= i && i <= 8)
                {
                    bandNum = 6;
                }
                else if (9 <= i && i <= 16)
                {
                    bandNum = 8;
                }
                else if (17 <= i && i <= 24)
                {
                    bandNum = 10;
                }
                else if (25 <= i && i <= 32)
                {
                    bandNum = 12;
                }
                else if (33 <= i && i <= 40)
                {
                    bandNum = 14;
                }
                else if (41 <= i && i <= 48)
                {
                    bandNum = 16;
                }
                else
                {
                    bandNum = 6;
                }
                string band = DataChange.HexString2BinString(rcvCode.Substring(bandNum, 2)).Replace(" ", "");
                int bitNum = i % 8;
                if (bitNum == 0)
                {
                    bitNum = 8;
                }
                band = band.Remove(bitNum - 1, 1);
                band = band.Insert(bitNum - 1, "0");

                rcvCode = rcvCode.Remove(bandNum, 2);
                rcvCode = rcvCode.Insert(bandNum, Convert.ToInt32(band, 2).ToString("X2"));
            }
            

            //ui更新
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            txtRcvCode.Text = RcvShowStyle(rcvCode);
            dataGridView1.Rows[id - 1].Cells[3].Value = txtRcvCode.Text;
            Console.WriteLine(ToolsUtil.AddBlank(rcvCode));
        }

        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(txtRcvCode.SelectedText))
            {

                return;
            }
            int selectStart = txtRcvCode.SelectionStart + 1;
            //选中数据的个数
            int selectCount = txtRcvCode.SelectedText.Length - ToolsUtil.SubstringCount(txtRcvCode.SelectedText, " ") - ToolsUtil.SubstringCount(txtRcvCode.SelectedText, "#") * 2 - ToolsUtil.SubstringCount(txtRcvCode.SelectedText, "/");
            if (selectCount % 2 != 0)
            {
                selectCount++;
            }
            //当前选中位置
            if (txtRcvCode.SelectedText.Substring(0, 1) != " ")
            {
                selectStart = selectStart - 1;
            }
            string beforCode = txtRcvCode.Text.Substring(0, selectStart);
            selectStart = selectStart - ToolsUtil.SubstringCount(beforCode, " ") - ToolsUtil.SubstringCount(beforCode, "#") * 2 - ToolsUtil.SubstringCount(beforCode, "/");
            //当前选中数据的位置
            int index = (selectStart / 2) + 1;

            for (int i = index; i < selectCount / 2 + index; i++)
            {
                string dataNow = i.ToString("X2");
                string data1 = rcvCode.Substring(22, 2);
                string data2 = rcvCode.Substring(24, 2);
                string data3 = rcvCode.Substring(26, 2);
                string data4 = rcvCode.Substring(28, 2);
                //清除旧的数据位
                if (dataNow == data1)
                {
                    rcvCode = rcvCode.Remove(22, 2);
                    rcvCode = rcvCode.Insert(22, "FF");
                }
                if (dataNow == data2)
                {
                    rcvCode = rcvCode.Remove(24, 2);
                    rcvCode = rcvCode.Insert(24, "FF");
                }
                if (dataNow == data3)
                {
                    rcvCode = rcvCode.Remove(26, 2);
                    rcvCode = rcvCode.Insert(26, "FF");
                }
                if (dataNow == data4)
                {
                    rcvCode = rcvCode.Remove(28, 2);
                    rcvCode = rcvCode.Insert(28, "FF");
                }
                //清除旧的屏蔽位
                //屏蔽位
                int bandNum;
                if (1 <= i && i <= 8)
                {
                    bandNum = 6;
                }
                else if (9 <= i && i <= 16)
                {
                    bandNum = 8;
                }
                else if (17 <= i && i <= 24)
                {
                    bandNum = 10;
                }
                else if (25 <= i && i <= 32)
                {
                    bandNum = 12;
                }
                else if (33 <= i && i <= 40)
                {
                    bandNum = 14;
                }
                else if (41 <= i && i <= 48)
                {
                    bandNum = 16;
                }
                else
                {
                    bandNum = 6;
                }
                string band = DataChange.HexString2BinString(rcvCode.Substring(bandNum, 2)).Replace(" ", "");
                int bitNum = i % 8;
                if (bitNum == 0)
                {
                    bitNum = 8;
                }
                band = band.Remove(bitNum - 1, 1);
                band = band.Insert(bitNum - 1, "1");
                rcvCode = rcvCode.Remove(bandNum, 2);
                rcvCode = rcvCode.Insert(bandNum, Convert.ToInt32(band, 2).ToString("X2"));
            }
           

            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            txtRcvCode.Text = RcvShowStyle(rcvCode);
            dataGridView1.Rows[id - 1].Cells[3].Value = txtRcvCode.Text;
            Console.WriteLine(ToolsUtil.AddBlank(rcvCode));
        }

        private void 删除全部ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(rcvCode))
            {
                return;
            }
            string heart = rcvCode.Substring(0,6);
            string code = rcvCode.Substring(38, rcvCode.Length - 38);
            rcvCode = heart + GetBandSection(code.Length) + "FFFFFFFFFFFFFFFF" + code;
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            RcvCodeEdit(id);
            dataGridView1.Rows[id - 1].Cells[3].Value = txtRcvCode.Text;
        }

        private string GetBandSection(int codeLen)
        {
            int len = codeLen / 2;
            string bit = "";
            string bandCode = "";
            for (int i = 0; i < len; i++)
            {
                bit = bit + "1";
            }
            while (bit.Length < 64)
            {
                bit = bit + "0";
            }
            for (int i = 0; i < bit.Length; i += 8)
            {
                bandCode = bandCode + Convert.ToInt32(bit.Substring(i,8), 2).ToString("X2");
            }
            return bandCode;
        }

        //Data1-4屏蔽位
        private void BtnSaveChange_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(rcvCode))
            {

                return;
            }
            //Data1-4位屏蔽
            //string band = DataChange.HexString2BinString(rcvCode.Substring(30, 8)).Replace(" ", "");
            CheckBox[] checkBoxes = {cbRcvData17, cbRcvData16, cbRcvData15, cbRcvData14, cbRcvData13, cbRcvData12, cbRcvData11, cbRcvData10,
                cbRcvData27, cbRcvData26, cbRcvData25, cbRcvData24, cbRcvData23, cbRcvData22, cbRcvData21, cbRcvData20,
                cbRcvData37, cbRcvData36, cbRcvData35, cbRcvData34, cbRcvData33, cbRcvData32, cbRcvData31, cbRcvData30,
                cbRcvData47, cbRcvData46, cbRcvData45, cbRcvData44, cbRcvData43, cbRcvData42, cbRcvData41, cbRcvData40
            };
            //获取二进制位屏蔽
            string band = "" ;
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                if (checkBoxes[i].Checked)
                {
                    band = band + "1";
                }
                else
                {
                    band = band + "0";
                }
            }
            string tmp = "";
            for (int i = 0; i < 4; i++)
            {
                tmp = tmp + Convert.ToInt32(band.Substring(i*8, 8), 2).ToString("X2");
            }

            rcvCode = rcvCode.Remove(30, 8);
            rcvCode = rcvCode.Insert(30,tmp);
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
        }

        //处理接收数据
        private void CbRcvDealMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            txtPollingCode.Enabled = false;
      
            //btnRcvPortSet.Enabled = false;
            if (string.IsNullOrEmpty(rcvCode))
            {

                return;
            }
            string deal = "";
            if (cbRcvDealMode.SelectedIndex == 0)
            {
                deal = cbRcvDealMode.SelectedIndex.ToString("X2");
                txtPollingCode.Enabled = true;
     
                //btnRcvPortSet.Enabled = true;
            }
            else if (cbRcvDealMode.SelectedIndex == 1)
            {
                deal = "FF";
            }
            else {
                deal = (cbRcvDealMode.SelectedIndex -1).ToString("X2");

            }
            rcvCode = rcvCode.Remove(2, 2);
            rcvCode = rcvCode.Insert(2, deal);
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            //多选修改
            if (dataGridView1.SelectedCells.Count <= 1)
            {
                return;
            }

            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                rcvCode = devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].rcvCode;
                if (string.IsNullOrWhiteSpace(rcvCode))
                {
                    continue;
                }
                rcvCode = rcvCode.Remove(2, 2);
                rcvCode = rcvCode.Insert(2, deal);
                devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].rcvCode = rcvCode;
            }
        }

        //设置端口
        private void BtnRcvPortSet_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count <= 1)
            {
                return;
            }
            string rcvCode = "";
            string hexPort = "";
            List<int> ids = new List<int>();
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                ids.Add(dataGridView1.SelectedCells[i].RowIndex);
                
            }
            ids.Sort();
            for (int i = 0; i < ids.Count; i++)
            {
                rcvCode = devRS3000.portDatas[ids[i]].rcvCode;
                if (string.IsNullOrWhiteSpace(rcvCode))
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(devRS3000.portDatas[ids[i]].pollingCode))
                {
                    hexPort = (ids[i] + 1).ToString("X2");
                    rcvCode = rcvCode.Remove(2, 2);
                    rcvCode = rcvCode.Insert(2, "00");
                    devRS3000.portDatas[ids[i]].rcvCode = rcvCode;
                    continue;
                }
                if (string.IsNullOrWhiteSpace(hexPort))
                {
                    continue;
                }
                rcvCode = rcvCode.Remove(2, 2);
                rcvCode = rcvCode.Insert(2, hexPort);
                devRS3000.portDatas[ids[i]].rcvCode = rcvCode;
            }

        }


        //查询代码
        private void TxtPollingCode_MouseUp(object sender, MouseEventArgs e)
        {
            GetStrSectionLen(txtPollingCode.Text, txtPollingCode.SelectedText, txtPollingCode.SelectionStart,false);
        }

        private void TxtPollingCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是16进制，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space && "0123456789ABCDEF".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }
        private void TxtPollingCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.V))
            {
                if (Clipboard.ContainsText())
                {
                    if (Validator.IsHexadecimal(Clipboard.GetText().Replace(" ", "")))
                    {
                        ((TextBox)sender).Text = Clipboard.GetText(); //Ctrl+V 粘贴 

                    }
                }
            }
            else if (e.KeyData == (Keys.Control | Keys.C))
            {
                if (!string.IsNullOrEmpty(((TextBox)sender).SelectedText))
                {
                    Clipboard.SetText(((TextBox)sender).SelectedText);//Ctrl+C 复制

                }

            }
        }
        private void TxtPollingCode_Leave(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string code = txtPollingCode.Text.Replace(" ", "");
            if (!Validator.IsHexadecimal(code))
            {
                txtPollingCode.Text = "";
                dataGridView1.Rows[id-1].Cells[3].Style.BackColor = Color.White;
                return;
            }
            if ( string.IsNullOrEmpty(code))
            {
                devRS3000.portDatas[id - 1].pollingCode = "";
                dataGridView1.Rows[id - 1].Cells[3].Style.BackColor = Color.White;
            }
            else
            {
                dataGridView1.Rows[id - 1].Cells[3].Style.BackColor = Color.FromArgb(228, 228, 228);
                //替换code 
                if (code.Length % 2 == 1)
                {
                    //确保不为单数
                    code = code.Substring(0, code.Length - 1);
                }
                if (code.Length > 62)
                {
                    txtPollingCode.Text = ToolsUtil.AddBlank(devRS3000.portDatas[id - 1].pollingCode);
                    return;
                }
                devRS3000.portDatas[id - 1].pollingCode = code;
                txtPollingCode.Text = ToolsUtil.AddBlank(code);
               
            }
            
        }

        //键值反馈模式
        private void CbRcvKeyFeedback_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvNum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(rcvCode))
            {

                return;
            }
            string deal = cbRcvKeyFeedback.SelectedIndex.ToString("X2");
            btnRcvSet.Enabled = false;
            if (cbRcvKeyFeedback.SelectedIndex == 2)
            {
                btnRcvSet.Enabled = true;
            }
            rcvCode = rcvCode.Remove(0, 2);
            rcvCode = rcvCode.Insert(0, deal);
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;

            if (dataGridView1.SelectedCells.Count <= 1)
            {
                return;
            }
            
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                rcvCode = devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].rcvCode;
                if (string.IsNullOrWhiteSpace(rcvCode))
                {
                    continue;
                }
                rcvCode = rcvCode.Remove(0, 2);
                rcvCode = rcvCode.Insert(0, deal);
                devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].rcvCode = rcvCode;
            }

        }

        //接收高级设置
        private void BtnRcvSet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRcvCode.Text))
            {
                int id = Convert.ToInt32(lbRcvNum.Text);
                TableShow(4);
                string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
                int func = Convert.ToInt32(rcvCode.Substring(4, 2), 16);
                lbRcvHnum.Text = lbRcvNum.Text;
                //选中
                cbRcvFunc.SelectedIndex = func;
                TableFuncStyle();

            }
            
        }



        #endregion

        #region 发送高级界面
        private void BtnSendBack_Click(object sender, EventArgs e)
        {
            TableShow(1);
        }

        private void BtnSendSave_Click(object sender, EventArgs e)
        {
            try
            {


                int id = Convert.ToInt32(lbSendHnum.Text);
                int min, max, data1, data2, data3, data4, data5, mode;

                if (string.IsNullOrEmpty(txtMinVal.Text) || string.IsNullOrEmpty(txtMaxVal.Text) || string.IsNullOrEmpty(txtMove1.Text) ||
                    string.IsNullOrEmpty(txtMove2.Text) || string.IsNullOrEmpty(txtMove3.Text) || string.IsNullOrEmpty(txtMove4.Text) ||
                    string.IsNullOrEmpty(txtMove5.Text) || string.IsNullOrEmpty(txtDataMode.Text)  )
                {
                    MessageBox.Show("输入值不能为空!请检查");
                    return;
                }
                min = Convert.ToInt32(txtMinVal.Text);
                max = Convert.ToInt32(txtMaxVal.Text);
                data1 = Convert.ToInt32(txtMove1.Text);
                data2 = Convert.ToInt32(txtMove2.Text);
                data3 = Convert.ToInt32(txtMove3.Text);
                data4 = Convert.ToInt32(txtMove4.Text);
                data5 = Convert.ToInt32(txtMove5.Text);
                mode = Convert.ToInt32(txtDataMode.Text);
                if (min > 65535 || max > 65535 || data1 > 65535 || data2 > 65535 || data3 > 65535 || data4 > 65535 || data5 > 65535 || mode > 65535)
                {
                    MessageBox.Show("输入值超出最大范围!请检查");
                    return;
                }
                devRS3000.portDatas[id - 1].customCode = min.ToString("X4") + max.ToString("X4") + data1.ToString("X4") + data2.ToString("X4") + data3.ToString("X4") + data4.ToString("X4") + data5.ToString("X4") + mode.ToString("X4");
                MessageBox.Show("保存成功");
            }
            catch //(Exception ex)
            {
                MessageBox.Show("保存失败!请检查");
            }
        }

        private void TxtMinVal_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TableCustomStyle()
        {
            try
            {
                int id = Convert.ToInt32(lbSendHnum.Text);
                string customCode = devRS3000.portDatas[id - 1].customCode;
                txtMinVal.Text = "0";
                txtMaxVal.Text = "1";
                txtMove1.Text = "65535";
                txtMove2.Text = "65535";
                txtMove3.Text = "65535";
                txtMove4.Text = "65535";
                txtMove5.Text = "65535";
                txtDataMode.Text = "1";
                if (string.IsNullOrEmpty(customCode))
                {
                    return;
                }
                else
                {
                    txtMinVal.Text = Convert.ToInt32(customCode.Substring(0, 4), 16).ToString();
                    txtMaxVal.Text = Convert.ToInt32(customCode.Substring(4, 4), 16).ToString();
                    txtMove1.Text = Convert.ToInt32(customCode.Substring(8, 4), 16).ToString();
                    txtMove2.Text = Convert.ToInt32(customCode.Substring(12, 4), 16).ToString();
                    txtMove3.Text = Convert.ToInt32(customCode.Substring(16, 4), 16).ToString();
                    txtMove4.Text = Convert.ToInt32(customCode.Substring(20, 4), 16).ToString();
                    txtMove5.Text = Convert.ToInt32(customCode.Substring(24, 4), 16).ToString();
                    txtDataMode.Text = Convert.ToInt32(customCode.Substring(28, 4), 16).ToString();
                }

            }
            catch
            {
                txtMinVal.Text = "";
                txtMaxVal.Text = "";
                txtMove1.Text = "65535";
                txtMove2.Text = "65535";
                txtMove3.Text = "65535";
                txtMove4.Text = "65535";
                txtMove5.Text = "65535";
                txtDataMode.Text = "1";

            }
            

        }

        #endregion

        #region 接收高级界面
        //数据运算模式
        private void CbRcvFunc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TableFuncStyle();

        }

        private void BtnRcvBack_Click(object sender, EventArgs e)
        {
            TableShow(2);
        }


        private void BtnFuncSave_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(lbRcvHnum.Text);
            string rcvCode = devRS3000.portDatas[id - 1].rcvCode;
            if (string.IsNullOrEmpty(rcvCode))
            {
                return;
            }
            //保存index
            string deal = cbRcvFunc.SelectedIndex.ToString("X2");
            rcvCode = rcvCode.Remove(4, 2);
            rcvCode = rcvCode.Insert(4, deal);
            devRS3000.portDatas[id - 1].rcvCode = rcvCode;
            string logicCode = "";
            int data1, data2, data3, data4, data5, data6, data7, data8;
            switch (cbRcvFunc.SelectedIndex)
            {
                case 1:
                    #region 四则运算一
                    if (string.IsNullOrWhiteSpace(txtMath1a.Text) || string.IsNullOrWhiteSpace(txtMath1b.Text) || string.IsNullOrWhiteSpace(txtMath1c.Text))
                    {
                        MessageBox.Show("输入值不能为空!请检查");
                        return;
                    }
                    data1 = Convert.ToInt32(txtMath1a.Text);
                    data2 = Convert.ToInt32(txtMath1b.Text);
                    data3 = Convert.ToInt32(txtMath1c.Text);
                    if (data1 > 65535 || data2 > 65535 || data3 > 65535)
                    {
                        MessageBox.Show("输入值超出最大范围!请检查");
                        return;
                    }
                    logicCode = data1.ToString("X4") + data2.ToString("X4") + data3.ToString("X8");
                    #endregion
                    break;
                case 2:
                    #region 四则运算二
                    if (string.IsNullOrWhiteSpace(txtMath2a.Text) || string.IsNullOrWhiteSpace(txtMath2b.Text) || string.IsNullOrWhiteSpace(txtMath2c.Text))
                    {
                        MessageBox.Show("输入值不能为空!请检查");
                        return;
                    }
                    data1 = Convert.ToInt32(txtMath2a.Text);
                    data2 = Convert.ToInt32(txtMath2b.Text);
                    data3 = Convert.ToInt32(txtMath2c.Text);
                    if (data1 > 65535 || data2 > 65535 || data3 > 65535)
                    {
                        MessageBox.Show("输入值超出最大范围!请检查");
                        return;
                    }
                    logicCode = data1.ToString("X4") + data2.ToString("X4") + data3.ToString("X8");
                    #endregion
                    break;
                case 3:
                    #region 八数值匹配
                    if (string.IsNullOrWhiteSpace(txtVal81.Text) || string.IsNullOrWhiteSpace(txtVal82.Text) || string.IsNullOrWhiteSpace(txtVal83.Text) || string.IsNullOrWhiteSpace(txtVal84.Text) ||
                        string.IsNullOrWhiteSpace(txtVal85.Text) || string.IsNullOrWhiteSpace(txtVal86.Text) || string.IsNullOrWhiteSpace(txtVal87.Text) || string.IsNullOrWhiteSpace(txtVal88.Text) )
                    {
                        MessageBox.Show("输入值不能为空!请检查");
                        return;
                    }
                    data1 = Convert.ToInt32(txtVal81.Text);
                    data2 = Convert.ToInt32(txtVal82.Text);
                    data3 = Convert.ToInt32(txtVal83.Text);
                    data4 = Convert.ToInt32(txtVal84.Text);
                    data5 = Convert.ToInt32(txtVal85.Text);
                    data6 = Convert.ToInt32(txtVal86.Text);
                    data7 = Convert.ToInt32(txtVal87.Text);
                    data8 = Convert.ToInt32(txtVal88.Text);
                    if (data1 > 255 || data2 > 255 || data3 > 255 || data4 > 255 || data5 > 255 || data6 > 255 || data7 > 255 || data8 > 255 )
                    {
                        MessageBox.Show("输入值超出最大范围!请检查");
                        return;
                    }
                    logicCode = data1.ToString("X2") + data2.ToString("X2") + data3.ToString("X2") + data4.ToString("X2") + data5.ToString("X2") + data6.ToString("X2") + data7.ToString("X2") + data8.ToString("X2");
                    #endregion
                    break;
                case 4:
                    #region 九区范围匹配
                    if (string.IsNullOrWhiteSpace(txtSection91.Text) || string.IsNullOrWhiteSpace(txtSection92.Text) || string.IsNullOrWhiteSpace(txtSection93.Text) || string.IsNullOrWhiteSpace(txtSection94.Text) ||
                        string.IsNullOrWhiteSpace(txtSection95.Text) || string.IsNullOrWhiteSpace(txtSection96.Text) || string.IsNullOrWhiteSpace(txtSection97.Text) || string.IsNullOrWhiteSpace(txtSection98.Text))
                    {
                        MessageBox.Show("输入值不能为空!请检查");
                        return;
                    }
                    data1 = Convert.ToInt32(txtSection91.Text);
                    data2 = Convert.ToInt32(txtSection92.Text);
                    data3 = Convert.ToInt32(txtSection93.Text);
                    data4 = Convert.ToInt32(txtSection94.Text);
                    data5 = Convert.ToInt32(txtSection95.Text);
                    data6 = Convert.ToInt32(txtSection96.Text);
                    data7 = Convert.ToInt32(txtSection97.Text);
                    data8 = Convert.ToInt32(txtSection98.Text);
                    if (data1 > 255 || data2 > 255 || data3 > 255 || data4 > 255 || data5 > 255 || data6 > 255 || data7 > 255 || data8 > 255)
                    {
                        MessageBox.Show("输入值超出最大范围!请检查");
                        return;
                    }
                    if (data1 > data2 || data2 > data3 || data3 > data4 || data4 > data5 || data5 > data6 || data6 > data7 || data7 > data8)
                    {
                        MessageBox.Show("输入值需要从小到大排序!请检查");
                        return;
                    }
                    logicCode = data1.ToString("X2") + data2.ToString("X2") + data3.ToString("X2") + data4.ToString("X2") + data5.ToString("X2") + data6.ToString("X2") + data7.ToString("X2") + data8.ToString("X2");
                    #endregion
                    break;
                case 5:
                    #region 四数值匹配
                    if (string.IsNullOrWhiteSpace(txtVal41.Text) || string.IsNullOrWhiteSpace(txtVal42.Text) || string.IsNullOrWhiteSpace(txtVal43.Text) || string.IsNullOrWhiteSpace(txtVal44.Text) )
                    {
                        MessageBox.Show("输入值不能为空!请检查");
                        return;
                    }
                    data1 = Convert.ToInt32(txtVal41.Text);
                    data2 = Convert.ToInt32(txtVal42.Text);
                    data3 = Convert.ToInt32(txtVal43.Text);
                    data4 = Convert.ToInt32(txtVal44.Text);
                
                    if (data1 > 65535 || data2 > 65535 || data3 > 65535 || data4 > 65535)
                    {
                        MessageBox.Show("输入值超出最大范围!请检查");
                        return;
                    }
                    logicCode = data1.ToString("X4") + data2.ToString("X4") + data3.ToString("X4") + data4.ToString("X4");
                    #endregion
                    break;
                case 6:
                    #region 五区范围匹配
                    if (string.IsNullOrWhiteSpace(txtSection51.Text) || string.IsNullOrWhiteSpace(txtSection52.Text) || string.IsNullOrWhiteSpace(txtSection53.Text) || string.IsNullOrWhiteSpace(txtSection54.Text))
                    {
                        MessageBox.Show("输入值不能为空!请检查");
                        return;
                    }
                    data1 = Convert.ToInt32(txtSection51.Text);
                    data2 = Convert.ToInt32(txtSection52.Text);
                    data3 = Convert.ToInt32(txtSection53.Text);
                    data4 = Convert.ToInt32(txtSection54.Text);

                    if (data1 > 65535 || data2 > 65535 || data3 > 65535 || data4 > 65535)
                    {
                        MessageBox.Show("输入值超出最大范围!请检查");
                        return;
                    }
                    if (data1 > data2 || data2 > data3 || data3 > data4 )
                    {
                        MessageBox.Show("输入值需要从小到大排序!请检查");
                        return;
                    }
                    logicCode = data1.ToString("X4") + data2.ToString("X4") + data3.ToString("X4") + data4.ToString("X4");
                    #endregion
                    break;
                default:
                    break;
            }
            devRS3000.portDatas[id - 1].logicCode = logicCode;
            MessageBox.Show("保存成功");
        }

        //显示logicCode内容
        private void TableFuncStyle()
        {
            int id = Convert.ToInt32(lbRcvHnum.Text);
            string logicCode = devRS3000.portDatas[id - 1].logicCode;
            superTabControl2.Visible = true;
          
            switch (cbRcvFunc.SelectedIndex)
            {
                case 1:
                    superTabItem5.Text = "四则运算一";
                    superTabItem5.Visible = true;
                    superTabItem6.Visible = false;
                    superTabItem7.Visible = false;
                    superTabItem8.Visible = false;
                    superTabItem9.Visible = false;
                    superTabItem10.Visible = false;
                    if (string.IsNullOrEmpty(logicCode))
                    {
                        txtMath1a.Text = "255";
                        txtMath1b.Text = "255";
                        txtMath1c.Text = "255";
                    }
                    else
                    {
                        txtMath1a.Text = Convert.ToInt32(logicCode.Substring(0, 4), 16).ToString();
                        txtMath1b.Text = Convert.ToInt32(logicCode.Substring(4, 4), 16).ToString();
                        txtMath1c.Text = Convert.ToInt32(logicCode.Substring(8, 8), 16).ToString();
                    }
                    break;
                case 2:
                    superTabItem6.Text = "四则运算二";
                    superTabItem6.Visible = true;
                    superTabItem5.Visible = false;
                    superTabItem7.Visible = false;
                    superTabItem8.Visible = false;
                    superTabItem9.Visible = false;
                    superTabItem10.Visible = false;
                    if (string.IsNullOrEmpty(logicCode))
                    {
                        txtMath2a.Text = "255";
                        txtMath2b.Text = "255";
                        txtMath2c.Text = "255";
                    }
                    else {
                        txtMath2a.Text = Convert.ToInt32(logicCode.Substring(0, 4), 16).ToString();
                        txtMath2b.Text = Convert.ToInt32(logicCode.Substring(4, 4), 16).ToString();
                        txtMath2c.Text = Convert.ToInt32(logicCode.Substring(8, 8), 16).ToString();
                    }
                    break;
                case 3:
                    superTabItem7.Text = "八数值匹配";
                    superTabItem7.Visible = true;
                    superTabItem6.Visible = false;
                    superTabItem5.Visible = false;
                    superTabItem8.Visible = false;
                    superTabItem9.Visible = false;
                    superTabItem10.Visible = false;
                    if (string.IsNullOrEmpty(logicCode))
                    {
                        txtVal81.Text = "255";
                        txtVal82.Text = "255";
                        txtVal83.Text = "255";
                        txtVal84.Text = "255";
                        txtVal85.Text = "255";
                        txtVal86.Text = "255";
                        txtVal87.Text = "255";
                        txtVal88.Text = "255";
                    }
                    else
                    {
                        txtVal81.Text = Convert.ToInt32(logicCode.Substring(0, 2), 16).ToString();
                        txtVal82.Text = Convert.ToInt32(logicCode.Substring(2, 2), 16).ToString();
                        txtVal83.Text = Convert.ToInt32(logicCode.Substring(4, 2), 16).ToString();
                        txtVal84.Text = Convert.ToInt32(logicCode.Substring(6, 2), 16).ToString();
                        txtVal85.Text = Convert.ToInt32(logicCode.Substring(8, 2), 16).ToString();
                        txtVal86.Text = Convert.ToInt32(logicCode.Substring(10, 2), 16).ToString();
                        txtVal87.Text = Convert.ToInt32(logicCode.Substring(12, 2), 16).ToString();
                        txtVal88.Text = Convert.ToInt32(logicCode.Substring(14, 2), 16).ToString();
                    }
                    break;
                case 4:
                    superTabItem8.Text = "九区范围匹配";
                    superTabItem8.Visible = true;
                    superTabItem5.Visible = false;
                    superTabItem6.Visible = false;
                    superTabItem7.Visible = false;
                    superTabItem9.Visible = false;
                    superTabItem10.Visible = false;
                    if (string.IsNullOrEmpty(logicCode))
                    {
                        txtSection91.Text = "255";
                        lbSection91.Text = "255";
                        txtSection92.Text = "255";
                        lbSection92.Text = "255";
                        txtSection93.Text = "255";
                        lbSection93.Text = "255";
                        txtSection94.Text = "255";
                        lbSection94.Text = "255";
                        txtSection95.Text = "255";
                        lbSection95.Text = "255";
                        txtSection96.Text = "255";
                        lbSection96.Text = "255";
                        txtSection97.Text = "255";
                        lbSection97.Text = "255";
                        txtSection98.Text = "255";
                        lbSection98.Text = "255";
                    }
                    else
                    {
                        txtSection91.Text = Convert.ToInt32(logicCode.Substring(0, 2), 16).ToString();
                        lbSection91.Text = txtSection91.Text;
                        txtSection92.Text = Convert.ToInt32(logicCode.Substring(2, 2), 16).ToString();
                        lbSection92.Text = txtSection92.Text;
                        txtSection93.Text = Convert.ToInt32(logicCode.Substring(4, 2), 16).ToString();
                        lbSection93.Text = txtSection93.Text;
                        txtSection94.Text = Convert.ToInt32(logicCode.Substring(6, 2), 16).ToString();
                        lbSection94.Text = txtSection94.Text;
                        txtSection95.Text = Convert.ToInt32(logicCode.Substring(8, 2), 16).ToString();
                        lbSection95.Text = txtSection95.Text;
                        txtSection96.Text = Convert.ToInt32(logicCode.Substring(10, 2), 16).ToString();
                        lbSection96.Text = txtSection96.Text;
                        txtSection97.Text = Convert.ToInt32(logicCode.Substring(12, 2), 16).ToString();
                        lbSection97.Text = txtSection97.Text;
                        txtSection98.Text = Convert.ToInt32(logicCode.Substring(14, 2), 16).ToString();
                        lbSection98.Text = txtSection98.Text;
                    }
                    break;
                case 5:
                    
                    superTabItem9.Text = "四数值匹配";
                    superTabItem9.Visible = true;
                    superTabItem5.Visible = false;
                    superTabItem6.Visible = false;
                    superTabItem7.Visible = false;
                    superTabItem8.Visible = false;
                    superTabItem10.Visible = false;
                    if (string.IsNullOrEmpty(logicCode))
                    {
                        txtVal41.Text = "65535";
                        txtVal42.Text = "65535";
                        txtVal43.Text = "65535";
                        txtVal44.Text = "65535";

                    }
                    else
                    {
                        txtVal41.Text = Convert.ToInt32(logicCode.Substring(0, 4), 16).ToString();
                        txtVal42.Text = Convert.ToInt32(logicCode.Substring(4, 4), 16).ToString();
                        txtVal43.Text = Convert.ToInt32(logicCode.Substring(8, 4), 16).ToString();
                        txtVal44.Text = Convert.ToInt32(logicCode.Substring(12, 4), 16).ToString();
                    }
                    break;
                case 6:
                    superTabItem10.Text = "五区范围匹配";
                    superTabItem10.Visible = true;
                    superTabItem5.Visible = false;
                    superTabItem6.Visible = false;
                    superTabItem7.Visible = false;
                    superTabItem8.Visible = false;
                    superTabItem9.Visible = false;
                    if (string.IsNullOrEmpty(logicCode))
                    {
                        txtSection51.Text = "65535";
                        lbSection51.Text = "65535";
                        txtSection52.Text = "65535";
                        lbSection52.Text = "65535";
                        txtSection53.Text = "65535";
                        lbSection53.Text = "65535";
                        txtSection54.Text = "65535";
                        lbSection54.Text = "65535";
                    }
                    else
                    {
                        txtSection51.Text = Convert.ToInt32(logicCode.Substring(0, 4), 16).ToString();
                        lbSection51.Text = txtSection51.Text;
                        txtSection52.Text = Convert.ToInt32(logicCode.Substring(4, 4), 16).ToString();
                        lbSection52.Text = txtSection52.Text;
                        txtSection53.Text = Convert.ToInt32(logicCode.Substring(8, 4), 16).ToString();
                        lbSection53.Text = txtSection53.Text;
                        txtSection54.Text = Convert.ToInt32(logicCode.Substring(12, 4), 16).ToString();
                        lbSection54.Text = txtSection54.Text;
                    }
                    break;
             
                default:
                    superTabControl2.Visible = false;
                    break;

            }
        }

        #region 按下限制 输入数字
        private void TxtMath1a_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtMath2a_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtVal81_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtSection91_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtVal41_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }

        private void TxtSection51_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (e.KeyChar != (char)Keys.Back && "0123456789".IndexOf(char.ToUpper(e.KeyChar)) < 0)
            {
                e.Handled = true;
            }
        }



        #endregion

        #region 九区范围匹配 五区  lb显示字体
        private void ChanglbText(Label lb, TextBox tb)
        {
            if (string.IsNullOrEmpty(tb.Text))
            {
                lb.Text = "0";

            }
            else
            {
                if (tb.Text.Length > 5)
                {
                    lb.Text = "超出范围";
                }
                else
                {
                    lb.Text = tb.Text;
                }
            }
        }
        private void TxtSection91_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection91, txtSection91);
        }

        private void TxtSection92_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection92, txtSection92);
        }

        private void TxtSection93_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection93, txtSection93);
        }

        private void TxtSection94_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection94, txtSection94);
        }

        private void TxtSection95_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection95, txtSection95);
        }

        private void TxtSection96_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection96, txtSection96);
        }

        private void TxtSection97_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection97, txtSection97);
        }

        private void TxtSection98_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection98, txtSection98);
        }

        private void TxtSection51_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection51, txtSection51);
        }

        private void TxtSection52_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection52, txtSection52);
        }

        private void TxtSection53_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection53, txtSection53);
        }

        private void TxtSection54_TextChanged(object sender, EventArgs e)
        {
            ChanglbText(lbSection54, txtSection54);
        }











        #endregion

        #endregion

        #region 表格复制 粘贴快捷键
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedCells.Count <= 0)
            {
                return;
            }
            List<int> name = new List<int>();
            List<int> send = new List<int>();
            List<int> rcv = new List<int>();
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                switch (dataGridView1.SelectedCells[i].ColumnIndex)
                {
                    case 1:
                        name.Add(dataGridView1.SelectedCells[i].RowIndex);
                        break;
                    case 2:
                        send.Add(dataGridView1.SelectedCells[i].RowIndex);
                        break;
                    case 3:
                        rcv.Add(dataGridView1.SelectedCells[i].RowIndex);
                        break;

                }
            }
            name.Sort();
            send.Sort();
            rcv.Sort();

            nameCopy.Clear();
            sendCopy.Clear();
            rcvCopy.Clear();
            pollingCopy.Clear();

            foreach (int i in name)
            {
                nameCopy.Add(devRS3000.portDatas[i].name);
            }
            foreach (int i in send)
            {
                sendCopy.Add(devRS3000.portDatas[i].sendCode);
            }
            foreach (int i in rcv)
            {
                rcvCopy.Add(devRS3000.portDatas[i].rcvCode);
                pollingCopy.Add(devRS3000.portDatas[i].pollingCode);
            }


        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count <= 0)
            {
                return;
            }
            //行数最小
            List<int> StartRow = new List<int>();
            List<int> EndRow = new List<int>();
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                StartRow.Add(dataGridView1.SelectedCells[i].RowIndex);
            }
            StartRow.Sort();
            int j = 0;
            for (int i = StartRow[0]; i < StartRow[0]+ nameCopy.Count; i++)
            {
                if (i < dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows[i].Cells[1].Value = nameCopy[j];
                    devRS3000.portDatas[i].name = nameCopy[j];
                    EndRow.Add(i);

                }
                j++;
            }

            j = 0;
            for (int i = StartRow[0]; i < StartRow[0] + sendCopy.Count; i++)
            {
                if (i < dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows[i].Cells[2].Value = SendShowStyle(sendCopy[j]);
                    devRS3000.portDatas[i].sendCode = sendCopy[j];
                    EndRow.Add(i);

                }
                j++;
            }
            j = 0;
            for (int i = StartRow[0]; i < StartRow[0] + rcvCopy.Count; i++)
            {
                if (i < dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows[i].Cells[3].Value = RcvShowStyle(rcvCopy[j]);
                    devRS3000.portDatas[i].rcvCode = rcvCopy[j];
                    devRS3000.portDatas[i].pollingCode = pollingCopy[j];
                    if (string.IsNullOrEmpty(pollingCopy[j]))
                    {
                        dataGridView1.Rows[i].Cells[3].Style.BackColor = Color.White;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[3].Style.BackColor = Color.FromArgb(228, 228, 228);
                    }
                    EndRow.Add(i);
                }
                j++;

            }
            EndRow.Sort();
            if (EndRow.Count > 0)
            {
                //EndRow[EndRow.Count-1]
                dataGridView1.ClearSelection();
                if (EndRow[EndRow.Count - 1] +1 < dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows[EndRow[EndRow.Count - 1]+1].Selected = true;

                }
            }


        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count <= 0)
            {
                return;
            }
            List<int> name = new List<int>();
            List<int> send = new List<int>();
            List<int> rcv = new List<int>();
        
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                switch (dataGridView1.SelectedCells[i].ColumnIndex)
                {
                    case 1:
                        name.Add(dataGridView1.SelectedCells[i].RowIndex);
                        break;
                    case 2:
                        send.Add(dataGridView1.SelectedCells[i].RowIndex);
                        break;
                    case 3:
                        rcv.Add(dataGridView1.SelectedCells[i].RowIndex);
                        dataGridView1.SelectedCells[i].Style.BackColor = Color.White;
                        break;

                }
                dataGridView1.SelectedCells[i].Value = "";
            }
            name.Sort();
            send.Sort();
            rcv.Sort();

            nameCopy.Clear();
            sendCopy.Clear();
            rcvCopy.Clear();
            pollingCopy.Clear();
            foreach (int i in name)
            {
                nameCopy.Add(devRS3000.portDatas[i].name);
                devRS3000.portDatas[i].name = "";
            }
            foreach (int i in send)
            {
                sendCopy.Add(devRS3000.portDatas[i].sendCode);
                devRS3000.portDatas[i].sendCode = "";
            }
            foreach (int i in rcv)
            {
                rcvCopy.Add(devRS3000.portDatas[i].rcvCode);
                pollingCopy.Add(devRS3000.portDatas[i].pollingCode);
                devRS3000.portDatas[i].rcvCode = "";
                devRS3000.portDatas[i].pollingCode = "";
            }
        }

        private void 删除ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count <= 0)
            {
                return;
            }
            nameCopy.Clear();
            sendCopy.Clear();
            rcvCopy.Clear();
            pollingCopy.Clear();
            for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
            {
                switch (dataGridView1.SelectedCells[i].ColumnIndex)
                {
                    case 1:
                        devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].name = "";
                        break;
                    case 2:
                        devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].sendCode = "";
                        break;
                    case 3:
                        devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].rcvCode = "";
                        devRS3000.portDatas[dataGridView1.SelectedCells[i].RowIndex].pollingCode = "";
                        dataGridView1.SelectedCells[i].Style.BackColor = Color.White;
                        break;

                }
                dataGridView1.SelectedCells[i].Value = "";
            }
        }


        #endregion

        #region  Combox列表item间距变宽
        private void CbRcvFunc_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(cbRcvFunc.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
        }

        private void CbSendCheck_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(cbSendCheck.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
        }

        private void CbRcvDealMode_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(cbRcvDealMode.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
        }

        private void CbRcvKeyFeedback_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(cbRcvKeyFeedback.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
        }

        #endregion

        #region 同步名称到表
        private void BtnSync_Click(object sender, EventArgs e)
        {
            string ipLastHex = ToolsUtil.GetIPstyle(ip, 4);
            string idHex = DevModuel.id.ToString("X2");
            string portHex = "";
            string address = "";
            string name = "";
            string type = "14.0_data";
            bool isExitPoint = false;
            for (int i = 0; i < devRS3000.portDatas.Count; i++)
            {
                if (string.IsNullOrEmpty(devRS3000.portDatas[i].name))
                {
                    continue;
                }
                portHex = devRS3000.portDatas[i].id.ToString("X2");
                address = string.Format("FE00{0}{1}",idHex,portHex);
                name = string.Format("{0}.{1}_{2}", DevModuel.id, devRS3000.portDatas[i].id, devRS3000.portDatas[i].name);
                isExitPoint = false;
                foreach (DataJson.PointInfo point in FileMesege.PointList.equipment)
                {
                    //循环判断 NameList中是否存在该节点
                    if (address == point.address && point.ip == ip)
                    {
                        isExitPoint = true;
                        if (string.IsNullOrEmpty(point.area1))
                        {
                            //位置不存在
                            point.area1 = "未定义区域";
                            point.area2 = "";
                            point.area3 = "";
                            point.area4 = "";
                            
                        }
                        point.name = string.Format("{0}@{1}", name, ip.Split('.')[3]);
                        point.type = type;
                        break;

                    }

                }
                if (!isExitPoint)
                {
                    //不存在 插入新信息
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.pid = DataChange.randomNum();
                    eq.area1 = "未定义区域";
                    eq.area2 = "";
                    eq.area3 = "";
                    eq.area4 = "";
                    //eq.name = "";
                    eq.ip = ip;
                    eq.address = address;
                    eq.objType = "";
                    eq.value = "";
                    eq.type = type;
                    eq.name = string.Format("{0}@{1}", name, ip.Split('.')[3]);
                    FileMesege.PointList.equipment.Add(eq);
                }
               
            }
            IsSync = true;
            MessageBox.Show("点位同步成功");
        }
        #endregion

    }
}
