using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace eNet编辑器.Tools
{
    public partial class holidayUpdate : Form
    {
        public holidayUpdate()
        {
            InitializeComponent();
        }

        //UDP客户端
        UdpSocket udp;

        //本地IP
        string Localip = "";
        public static string configPath = Application.StartupPath + "\\conf.ini";

        public event Action<string> AppTxtShow;
        private event Action<string> udpreceviceDelegate;

        private void holidayUpdate_Load(object sender, EventArgs e)
        {
            udpreceviceDelegate += new Action<string>(udpReceviceDelegateMsg);
            findOnlineGW();
            findIniFilePath(txtPath, "holidayPath");
        }



        #region 初始化
        /// <summary>
        /// 初始化上次工程的路径
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="com"></param>
        private void findIniFilePath(TextBox tb, string com)
        {
            string info = IniConfig.GetValue(configPath, "filepath", com);
            if (System.IO.File.Exists(info))
            {
                tb.Text = info;
            }


        }

        #endregion

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
            Localip = ToolsUtil.GetLocalIP();
            //udp 绑定
            udp.udpBing(Localip, ToolsUtil.GetFreePort().ToString());
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
        private void holidayUpdate_Paint(object sender, PaintEventArgs e)
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
            this.Close();
        }

        #endregion

        #region 节假日文件选择
        private void btnPath_Click(object sender, EventArgs e)
        {
            openfile(txtPath, "holidayPath", "bin");
        }


        public bool openfile(TextBox tb, string key, string type)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                string historyPath = IniConfig.GetValue(configPath, "filepath", key);
                if (type == "json")
                {
                    op.Filter = "文件（*.json）|*.json|工程文件（*.zip）|*.zip|节假日文件（*.bin）|*.bin|All files(*.*)|*.*";
                }
                else if (type == "bin")
                {
                    op.Filter = "节假日文件（*.bin）|*.bin|工程文件（*.zip）|*.zip|文件（*.json）|*.json|All files(*.*)|*.*";
                }
                else
                {
                    op.Filter = "工程文件（*.zip）|*.zip|节假日文件（*.bin）|*.bin|文件（*.json）|*.json|All files(*.*)|*.*";
                }
                if (!string.IsNullOrEmpty(historyPath))
                {
                    //设置此次默认目录为上一次选中目录  
                    op.InitialDirectory = System.IO.Path.GetDirectoryName(historyPath);
                }
                op.Title = "选择文件";
                if (op.ShowDialog() == DialogResult.OK)
                {


                    //添加打开过的地址
                    IniConfig.SetValue(configPath, "filepath", key, op.FileName);
                    tb.Text = op.FileName.ToString();
                    return true;


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
        #endregion



        #region 更新节假日设置

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            download(cbOnlineIP.Text, txtPath.Text);
        }

        /// <summary>
        /// 把节假日文件下载到主机里面
        /// </summary>
        /// <param name="ip"></param>
        private bool download(string ip, string filepath)
        {
            if (string.IsNullOrEmpty(ip))
            {
                AppTxtShow("主机地址不能为空");
                return false;
            }
            if (!System.IO.File.Exists(filepath))
            {
                AppTxtShow("路径文件不存在");
                return false;
            }
            //连接网络 发送当前IP的压缩包到里面
            Socket sock = null;
            try
            {

                //写入数据格式
                string data = "down backup/holiday.bin$";

                TcpSocket ts = new TcpSocket();

                sock = ts.ConnectServer(ip, 6001, 1);
                ToolsUtil.DelayMilli(1000);

                if (sock == null)
                {
                    AppTxtShow("连接失败！请重试");
                    return false;
                }


                int flag = -1;

                //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                flag = ts.SendData(sock, data, 1);
                if (flag == 0)
                {
                    flag = ts.SendFile(sock, filepath);

                    if (flag == 0)
                    {
                        if (sock != null)
                        {

                            sock.Close();
                        }
                        AppTxtShow("节假日设置成功！");
                        return true;
                    }


                }

                if (sock != null)
                {

                    sock.Dispose();
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                AppTxtShow("更新失败");
                return false;

            }
        }

        #endregion

        





    }
}
