using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Text.RegularExpressions;

namespace eNet编辑器.AddForm
{
    public partial class correctionTime : Form
    {
        public correctionTime()
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

        private void correctionTime_Load(object sender, EventArgs e)
        {
            lbBJTime.Text = DateTime.Now.ToString();
            lbMasterTime.Text = "";
            timer1.Start();
            udpreceviceDelegate += new Action<string>(udpReceviceDelegateMsg);
            tcp6003receviceDelegate += new Action<string>(tcp6003ReceviceDelegateMsg);
            findOnlineGW(false);
            
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

        private void correctionTime_Paint(object sender, PaintEventArgs e)
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
            timer1.Stop();
            this.Close();
        }
        #endregion

        #region UDP获取所有在线网关IP  UNP6002端口
        /// <summary>
        /// 寻找加载在线的网关
        /// </summary>
        private void findOnlineGW(bool flag)
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
            Localip =  SocketUtil.GetLocalIP();
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



        /// <summary>
        /// 获取主机时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
      
            lbMasterTime.Text = "";
            getMsterTimeConnet();
            
        }

        /// <summary>
        /// 不停给时间加秒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            lbBJTime.Text = DateTime.Now.ToString();
            if(!string.IsNullOrEmpty(lbMasterTime.Text))
            {
                DateTime tmp = Convert.ToDateTime(lbMasterTime.Text);
                lbMasterTime.Text = tmp.AddSeconds(1.0).ToString();
            }
             
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (client6003 != null && client6003.Connected())
            {
                //2016-05-09T13:09:55
                string[] tmp = DateTime.Now.ToString("s").Split('T');
                //客户端发送数据
                client6003.SendAsync("SET;"+ tmp[0].Replace("-","")+";{254.251.0.2};\r\n");
                client6003.SendAsync("SET;00"+tmp[1].Replace(":","")+";{254.251.0.3};\r\n");
                AppTxtShow("更新主机时间成功");

                //再次获取时间
                client6003.SendAsync("GET;{254.251.0.2};\r\n");
                client6003.SendAsync("GET;{254.251.0.3};\r\n");
            }
        }



        #region tcp6003 获取时间
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
                if (client6003 != null && client6003.Connected())
                {
                    //客户端发送数据
                    client6003.SendAsync("GET;{254.251.0.2};\r\n");
                    client6003.SendAsync("GET;{254.251.0.3};\r\n");
                }
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
        string yyMMdd = "";
        string hhmmss = "";
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
                
                if (match.Groups[0].Value == "254.251.0.2")//年月日 2 
                {
                    yyMMdd = data;
                    dealTime();

                }
                else if (match.Groups[0].Value == "254.251.0.3")//时分秒 3
                {
                    hhmmss = data;
                    dealTime();
                }
                
                
               

            }//for
            bufferMsg = "";


        }

        /// <summary>
        /// 处理时间
        /// </summary>
        private void dealTime()
        {
            if (!string.IsNullOrEmpty(yyMMdd) && !string.IsNullOrEmpty(hhmmss))
            {
                string dateString = string.Format("20{0}{1}",yyMMdd.Substring(2,6),hhmmss.Substring(2,6));
                DateTime dt = DateTime.ParseExact(dateString, "yyyyMMddhhmmss", System.Globalization.CultureInfo.CurrentCulture);
                lbMasterTime.Text = dt.ToString();
                yyMMdd = "";
                hhmmss = "";
                
            }
        }

        #endregion


      
  



    }
}
