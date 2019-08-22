using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Net;

namespace eNet编辑器.Tools
{
    public partial class astronomicalClock : Form
    {
        public astronomicalClock()
        {
            InitializeComponent();
        }

        //UDP客户端
        UdpSocket udp;
        ClientAsync client6003;

        //本地IP
        string Localip = "";

        public event Action<string> AppTxtShow;
        private event Action<string> udpreceviceDelegate;
        private event Action<string> tcp6003receviceDelegate;

        private void astronomicalClock_Load(object sender, EventArgs e)
        {
            IniControl();
            udpreceviceDelegate += new Action<string>(udpReceviceDelegateMsg);
            tcp6003receviceDelegate += new Action<string>(tcp6003ReceviceDelegateMsg);
            findOnlineGW();
        }


        #region UDP获取所有在线网关IP  UNP6002端口


        /// <summary>
        /// 寻找加载在线的网关
        /// </summary>
        private void findOnlineGW()
        {
            try
            {

                //寻找加载在线的网关
                udp.udpClose();
            }
            catch
            {
            }
            udpIni();
            //获取本地IP
            Localip = SocketUtil.GetLocalIP();
            //udp 绑定
            udp.udpBing(Localip, SocketUtil.GetFreePort().ToString());
            //绑定成功
            if (udp.isbing)
            {

                udp.udpSend("255.255.255.255", "6002", "search all");

            }
        }

        /// <summary>
        /// udp 事件初始化
        /// </summary>
        private void udpIni()
        {
            //初始化UDP
            udp = new UdpSocket();
            udp.Received += new Action<string, string>((IP, msg) =>
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(msg))
                    {
                        //跨线程调用
                        this.Invoke(udpreceviceDelegate, msg);
                    }


                }
                catch
                {
                    //报错不处理
                }
            });
        }


        /// <summary>
        /// 网络信息 处理函数
        /// </summary>
        /// <param name="msg"></param>
        private void udpReceviceDelegateMsg(string msg)
        {

            try
            {
                if (msg.Contains("success"))
                {

                    //MessageBox.Show("数据更新完成");
                }
                if (msg.Contains("devIP"))
                {

                    //网关加载到cb里面
                    string[] devInfos = msg.Split(' ');
                    //devIP = 0.0.0.0
                    string[] devIP = devInfos[0].Split('=');
                    bool isExeit = false;
                    for (int i = 0; i < cbOnlineIP.Items.Count; i++)
                    {
                        if (cbOnlineIP.Items[i].ToString() == devIP[1])
                        {
                            //确定item里面没有 该ip项就添加
                            isExeit = true;
                        }
                    }
                    if (!isExeit)
                    {
                        cbOnlineIP.Items.Add(devIP[1]);

                    }




                }

            }
            catch { }


        }



        #endregion

        #region tcp6003 链接并获取日出日落/时区/经纬度
        private void getMsterTimeConnet()
        {
            try
            {
                if (client6003 != null && client6003.Connected())
                {
                    client6003.Dispoes();
                }

                client6003 = new ClientAsync();
                client6003Ini();
                if (string.IsNullOrEmpty(cbOnlineIP.Text))
                {
                    return;
                }
                //异步连接
                client6003.ConnectAsync(cbOnlineIP.Text, 6003);
                
            }
            catch
            {
                client6003 = null;
                return;
            }
        }


        private void client6003Ini()
        {
            client6003.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                string key = "";

                try
                {
                    if (c.Client.Connected)
                    {
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch { }
                switch (enAction)
                {
                    case ClientAsync.EnSocketAction.Connect:
                        // MessageBox.Show("已经与" + key + "建立连接");
                        break;
                    case ClientAsync.EnSocketAction.SendMsg:

                        //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                        break;
                    case ClientAsync.EnSocketAction.Close:

                        //MessageBox.Show("服务端连接关闭");
                        break;
                    case ClientAsync.EnSocketAction.Error:

                        //MessageBox.Show("连接发生错误,请检查网络连接");

                        break;
                    default:
                        break;
                }
            });
            //信息接收处理
            client6003.Received += new Action<string, string>((key, msg) =>
            {
                try
                {

                    if (!string.IsNullOrEmpty(msg))
                    {
                        //跨线程调用
                        this.Invoke(tcp6003receviceDelegate, msg);

                    }


                }
                catch
                {
                    //报错不处理
                }

            });

        }

        //接收区信息缓存buffer
        string bufferMsg = "";
        /// <summary>
        /// 6003端口回调函数
        /// </summary>
        /// <param name="msg"></param>
        private void tcp6003ReceviceDelegateMsg(string msg)
        {
            bufferMsg = bufferMsg + msg;
            if (msg.Length == 1024)
            {
                return;
            }

            string[] strArray = bufferMsg.Split(new string[] { "FB", "ACK" }, StringSplitOptions.RemoveEmptyEntries);
            Regex reg = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
            for (int i = 0; i < strArray.Length; i++)
            {

                if (strArray[i].Contains("ack"))
                {
                    continue;
                }
                //数据信息  FB;05190809;{254.251.0.2}; 
                string data = strArray[i].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                Match match = reg.Match(strArray[i]);

                switch (match.Groups[0].Value)
                {
                    case "254.251.0.15":
                        //经度
                        longitudeDeal(data);
                        break;
                    case "254.251.0.16":
                        //纬度
                        latitudeDeal(data);
                        break;
                    case "254.251.0.17":
                        //时区
                        timeDeal(data);
                        break;
                    case "254.251.0.18":
                        //日出日落
                        sunDeal(data);
                        break;
                    default: 
                        break;
                }
 




            }//for
            bufferMsg = "";
        }

        /// <summary>
        /// 处理经度信息
        /// </summary>
        /// <param name="data"></param>
        private void longitudeDeal(string data)
        {
            if (data.Substring(0, 2) == "00")
            {
                //东经
                cbLongitude.SelectedIndex = 0;
            }
            else
            {
                //西经
                cbLongitude.SelectedIndex = 1;
            }
            //度 分 秒
            dataDeal(cblgDegree, data, 2);
            dataDeal(cblgMinute, data, 4);
            dataDeal(cblgSeconds, data, 6);
        }

        /// <summary>
        /// 处理纬度信息
        /// </summary>
        /// <param name="data"></param>
        private void latitudeDeal(string data)
        {
            if (data.Substring(0, 2) == "00")
            {
                //北纬
                cbLatitude.SelectedIndex = 0;
            }
            else
            {
                //南纬
                cbLatitude.SelectedIndex = 1;
            }
            //度 分 秒
            dataDeal(cbltDegree, data, 2);
            dataDeal(cbltMinute, data, 4);
            dataDeal(cbltSeconds, data, 6);
        }

        private void timeDeal(string data)
        {
            if (data.Substring(0, 2) == "00")
            {
                //东区 和 零区
                int time =  Convert.ToInt32( data.Substring(6, 2),16);
                if (time == 0)
                {
                    //0区
                    cbTimeZone.SelectedIndex = 0;
                }
                else
                {
                    //东1 - 11区
                    cbTimeZone.SelectedIndex = time;
                }
            }
            else
            { 
                //西区和 东西12区
                int time = Convert.ToInt32(data.Substring(6, 2), 16);
                if (time == 12)
                {
                    //东西12区
                    cbTimeZone.SelectedIndex = 23;
                }
                else
                {
                    //西1 - 11区
                    cbTimeZone.SelectedIndex = time+11;
                }
            }

        }

        private void sunDeal(string data)
        {
            lbSunrise.Text = dataTimeDeal(data,0);
            lbSunset.Text = dataTimeDeal(data, 4);
        }

        /// <summary>
        /// 经纬度 把16进制字符转换为十进制字符
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="data"></param>
        /// <param name="start"></param>
        private void dataDeal(ComboBox cb,string data,int start)
        {
            cb.Text = Convert.ToInt32(data.Substring(start,2),16).ToString();
        }

        /// <summary>
        /// 格式化日出日落时间
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private string dataTimeDeal(string data, int start)
        {
            string hour = Convert.ToInt32(data.Substring(start, 2), 16).ToString("D2");
            string min = Convert.ToInt32(data.Substring(start+2, 2), 16).ToString("D2");
            return string.Format("{0}:{1}", hour, min);
        }

        #endregion

        #region 控件初始化
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void IniControl()
        {
            //日出日落
            lbSunrise.Text = "";
            lbSunset.Text = "";
            //时区
            cbAddItemTime(cbTimeZone, 1, 11);
            //经度
            cbLongitude.SelectedIndex = 0;
            cbAddItem(cblgDegree, 0, 179);
            cbAddItem(cblgMinute, 0, 59);
            cbAddItem(cblgSeconds, 0, 59);
            //纬度
            cbLatitude.SelectedIndex = 0;
            cbAddItem(cbltDegree, 0, 89);
            cbAddItem(cbltMinute, 0, 59);
            cbAddItem(cbltSeconds, 0, 59);
        }

        private void cbAddItemTime(ComboBox cbbox, int start, int end)
        {
            cbbox.Items.Clear();
            cbbox.Items.Add("0区");
            for (int i = start; i <= end; i++)
            {
                cbbox.Items.Add(string.Format("东{0}区",i));
            }
            for (int i = start; i <= end; i++)
            {
                cbbox.Items.Add(string.Format("西{0}区", i));
            }
            cbbox.Items.Add("东西12区");
            cbbox.SelectedIndex = 0;
        }

        private void cbAddItem(ComboBox cbbox ,int start,int end)
        {
            cbbox.Items.Clear();
            for (int i = start; i <= end; i++)
            {
                cbbox.Items.Add(i);
            }
            cbbox.SelectedIndex = 0;
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

        private void astronomicalClock_Paint(object sender, PaintEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            //timer1.Stop();
            this.Close();
        }
        #endregion

        #region 设置 和获取刷新
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lbSunrise.Text = "";
            lbSunset.Text = "";
            getMsterTimeConnet();
            if (client6003 != null && client6003.Connected())
            {
                //客户端发送数据
                //日出日落
                client6003.SendAsync("GET;{254.251.0.18};\r\n");
                //时区
                client6003.SendAsync("GET;{254.251.0.17};\r\n");
                //经度
                client6003.SendAsync("GET;{254.251.0.15};\r\n");
                //纬度
                client6003.SendAsync("GET;{254.251.0.16};\r\n");

            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            getMsterTimeConnet();
            if (client6003 != null && client6003.Connected())
            {
                //2016-05-09T13:09:55
                string[] tmp = DateTime.Now.ToString("s").Split('T');
                //客户端发送数据
                client6003.SendAsync(getTime());
                client6003.SendAsync(getLongitude());
                client6003.SendAsync(getLatitude());
                AppTxtShow("天文时钟设置成功");
                SocketUtil.DelayMilli(2000);
                lbSunrise.Text = "";
                lbSunset.Text = "";
                //再次获取日出日落时间
                client6003.SendAsync("GET;{254.251.0.18};\r\n");
            }
        }

        /// <summary>
        /// 获取时区发送指令
        /// </summary>
        /// <returns></returns>
        private string getTime()
        {
            string tmp = "";
            if (cbTimeZone.SelectedIndex < 12)
            {
                //0时区 和东1-11区
                tmp = string.Format("000000{0}", cbTimeZone.SelectedIndex.ToString("X2"));
            }
            else
            {
                //西1-11区 和东西12区
                tmp = string.Format("010000{0}", (cbTimeZone.SelectedIndex - 11).ToString("X2"));
            }
            return "SET;" + tmp + ";{254.251.0.17};\r\n";

        }

        /// <summary>
        /// 获取经度发送指令
        /// </summary>
        /// <returns></returns>
        private string getLongitude()
        {
            
            string tmp = cbLongitude.SelectedIndex.ToString("X2")+ cblgDegree.SelectedIndex.ToString("X2")+
                cblgMinute.SelectedIndex.ToString("X2")+cblgSeconds.SelectedIndex.ToString("X2");
            
            return "SET;" + tmp + ";{254.251.0.15};\r\n";
        }

        /// <summary>
        /// 获取纬度发送指令
        /// </summary>
        /// <returns></returns>
        private string getLatitude()
        {
            string tmp = cbLatitude.SelectedIndex.ToString("X2") + cbltDegree.SelectedIndex.ToString("X2") +
               cbltMinute.SelectedIndex.ToString("X2") + cbltSeconds.SelectedIndex.ToString("X2");
            return "SET;" + tmp + ";{254.251.0.16};\r\n";
        }


        #endregion

    }
}
