using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows.Forms.DataVisualization.Charting;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace eNet编辑器.Controller
{
    public partial class setDimmer : Form
    {
        private string ip ;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        private string lastIP;

        private DataJson.Module devModuel;

        public DataJson.Module DevModuel
        {
            get { return devModuel; }
            set { devModuel = value; }
        }

        private DataJson.DevPort devPort;

        public DataJson.DevPort DevPort
        {
            get { return devPort; }
            set { devPort = value; }
        }


        ClientAsync client6003;
        private event Action<string> tcp6003receviceDelegate;

        //曲线数据点
        List<string> xData = new List<string>();
        List<int> yData = new List<int>();

        public setDimmer()
        {
            InitializeComponent();
        }

        private void setDimmer_Load(object sender, EventArgs e)
        {
            tcp6003receviceDelegate += new Action<string>(tcp6003ReceviceDelegateMsg);
            //链接tcp
            Connect6003Tcp(ip);
            lastIP = ip.Split('.')[3];
            timer2.Start();
            //图表初始化
            chart1Init();
            //获取表格数字
            updateTable();
            //窗口状态初始化
            getFormState(devPort.portContent);
        }


        #region  图形初始化函数

        /// <summary>
        /// 图表初始化
        /// </summary>
        private void chart1Init()
        {
            xData.Clear();
            yData.Clear();
            for (int i = 1; i <= 100; i++)
            {
                xData.Add(i.ToString() + "%");
                yData.Add(i * 100);

            }
            //线条颜色
            chart1.Series[0].Color = Color.Green;
            //线条粗细
            chart1.Series[0].BorderWidth = 1;
            //标记点边框颜色      
            chart1.Series[0].MarkerBorderColor = Color.Blue;
            //标记点边框大小
            chart1.Series[0].MarkerBorderWidth = 3; //chart1.;// Xaxis 
            //标记点中心颜色
            chart1.Series[0].MarkerColor = Color.White;//AxisColor
            //标记点大小
            chart1.Series[0].MarkerSize = 2;
            //标记点类型     
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle;
            //需要提示的信息    
            chart1.Series[0].ToolTip = "X轴：#VALX\nY轴：#VAL\n";
            //将文字移到外侧
            chart1.Series[0]["PieLabelStyle"] = "Outside";
            //绘制黑色的连线
            chart1.Series[0]["PieSplineColor"] = "Black";

            chart1.Series[0].Points.DataBindXY(xData, yData);

            chart1.Series[0].ShadowOffset = 1;
            chart1.Series[0].MarkerStep = 3;
              
            chart1.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            //chart1.ChartAreas[0].AxisX.Title = "百分点(0-100%)";
            //chart1.ChartAreas[0].AxisY.Title = "TEMPERATURE";
           
 

            //改变X轴刻度间隔
            chart1.ChartAreas[0].AxisX.Interval = 25;
            chart1.ChartAreas[0].AxisX.Maximum = 100;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            //改变Y轴刻度间隔
            chart1.ChartAreas[0].AxisY.Interval = 1000;
            chart1.ChartAreas[0].AxisY.Maximum = 10010;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            //设置游标
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            
            
        }



        #endregion

       
        #region tcp6003 链接 以及处理反馈信息 发送信息

        /// <summary>
        /// 定时5秒链接tcp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (client6003 != null && !client6003.Connected())
            {
                
                //链接tcp
                Connect6003Tcp(ip);
            }

           
        }

        private void Connect6003Tcp(string ip)
        {
            try
            {

                if (client6003 != null )
                {
                    client6003.Dispoes();
                }
                pictureBox1.Image = Properties.Resources.OutLine;
                client6003 = new ClientAsync();
                client6003Ini();
                if (string.IsNullOrEmpty(ip))
                {
                    return;
                }
                //异步连接
                client6003.ConnectAsync(ip, 6003);
                if (client6003 != null && client6003.Connected())
                {
                    pictureBox1.Image = Properties.Resources.Online;
                    //开启心跳 
                    client6003.SendAsync("SET;00000001;{254.251.0.1};\r\n");
                    return;
                }
                else
                {
                    //异步连接失败 再次连接
                    client6003.ConnectAsync(ip, 6003);
                    if (client6003 != null && client6003.Connected())
                    {
                        pictureBox1.Image = Properties.Resources.Online;
                        //开启心跳 
                        client6003.SendAsync("SET;00000001;{254.251.0.1};\r\n");
                        return;
                    }
                }
                
            }
            catch
            {
         
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
               
                    switch (enAction)
                    {
                        case ClientAsync.EnSocketAction.Connect:
                            // MessageBox.Show("已经与" + key + "建立连接");
                            break;
                        case ClientAsync.EnSocketAction.SendMsg:

                            //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                            break;
                        case ClientAsync.EnSocketAction.Close:
                            //跨线程调用
                            this.Invoke(tcp6003receviceDelegate, "outline");
                            //MessageBox.Show("服务端连接关闭");
                            break;
                        case ClientAsync.EnSocketAction.Error:
                            //MessageBox.Show("连接发生错误,请检查网络连接");
                            //跨线程调用
                            this.Invoke(tcp6003receviceDelegate, "outline");
                            break;
                        default:
                            break;
                    }
                }
                catch { }
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
            //离线处理
            if (msg == "outline")
            {
                if (client6003 != null)
                {
                    client6003.Dispoes();
                }
                pictureBox1.Image = Properties.Resources.OutLine;
                return;
            }
            bufferMsg = bufferMsg + msg;
            if (msg.Length == 1024)
            {
                return;
            }
            
            string[] strArray = bufferMsg.Split(new string[] { "FB", "ACK" }, StringSplitOptions.RemoveEmptyEntries);
            string dataID = DevModuel.id.ToString();
            Regex reg = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+):(\d+)");
            for (int i = 0; i < strArray.Length; i++)
            {

                if (strArray[i].Contains("ack"))
                {
                    continue;
                }
                //数据信息  FB;{254.XX.0.XX:XX}; 64;
                string data = strArray[i].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[2];
                Match match = reg.Match(strArray[i]);
                if (match.Groups[3].Value == dataID && match.Groups[5].Value == "64")//40上电状态
                {

                    GetPowerState(data);
                   
                }
                else if (match.Groups[3].Value == dataID && match.Groups[5].Value == "96")//60开启状态
                {
                    GetOnState(data);
                }
                else if (match.Groups[3].Value == dataID && match.Groups[5].Value == "97")//61渐变参数
                {
                    GetChangeState(data);
                }
                else if (match.Groups[3].Value == dataID && match.Groups[5].Value == "98")//62最大值
                {
                    GetMax(data);
                }
                else if (match.Groups[3].Value == dataID && match.Groups[5].Value == "99")//63最小值
                {
                    GetMin(data);
                }
                else if (match.Groups[3].Value == dataID && match.Groups[5].Value == "101")//67 LED曲线
                {
                    GetLinesp(data);
                }

            }//for
            bufferMsg = "";

        
        }


        /// <summary>
        /// 普通发送函数
        /// </summary>
        /// <param name="dataOrder"></param>
        private bool sendOrder(string dataOrder)
        {
            if (client6003 != null && client6003.Connected())
            {
                string tmp = string.Format("SET;{0};{{{1}.0.{2}.{3}}};\r\n", dataOrder, lastIP, DevModuel.id, DevPort.portID);
                //发送
                client6003.SendAsync(tmp);
                return true;

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 普通发送函数  加端口
        /// </summary>
        /// <param name="dataOrder"></param>
        /// <param name="portID"></param>
        private bool sendRegOrder(string dataOrder, string portID, string reg)
        {
            if (string.IsNullOrEmpty(dataOrder) || string.IsNullOrEmpty(portID) || string.IsNullOrEmpty(reg))
            {
                return false;
            }
            if (client6003 != null && client6003.Connected())
            {
                reg = Convert.ToInt32(reg, 16).ToString();
                string tmp = string.Format("set;{{{1}.0.{2}.{3}:{4}}};{0};\r\n", dataOrder, lastIP, DevModuel.id, portID, reg);
                //发送
                client6003.SendAsync(tmp);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送信息函数 发送到寄存器寄存器
        /// </summary>
        /// <param name="dataOrder"></param>
        private bool  sendRegOrder(string dataOrder, string reg)
        {
            if (string.IsNullOrEmpty(dataOrder) || string.IsNullOrEmpty(reg))
            {
                return false;
            }
            if (client6003 != null && client6003.Connected())
            {
                reg = Convert.ToInt32(reg, 16).ToString();
                string tmp = string.Format("set;{{{1}.0.{2}.{3}:{4}}};{0};\r\n", dataOrder, lastIP, DevModuel.id, DevPort.portID, reg);
                //发送
                client6003.SendAsync(tmp);
                 //SocketUtil.WriteLog(tmp);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发送读取指令 发送到寄存器寄存器
        /// </summary>
        /// <param name="dataOrder"></param>
        private bool sendGetOrder( string reg)
        {
            if (string.IsNullOrEmpty(reg))
            {
                return false;
            }
            if (client6003 != null && client6003.Connected())
            {
                reg = Convert.ToInt32(reg, 16).ToString();
                string tmp = string.Format("get;{{{0}.0.{1}.{2}:{3}}};\r\n", lastIP, DevModuel.id, DevPort.portID, reg);
                //发送
                client6003.SendAsync(tmp);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重启设备
        /// </summary>
        private bool sendResetdriver()
        {
            if (client6003 != null && client6003.Connected())
            {
                string str = "set;{" + lastIP + ".0." + DevModuel.id + ".255:254};acac;\r\n";
                client6003.SendAsync(str);
                return true;
            }
            return false;
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

        private void setDimmer_Paint(object sender, PaintEventArgs e)
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
            //关闭的时候 保存
            FileMesege.portDimmer = SaveFormState();
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            timer1.Stop();
            sendOrder("10000000");
            timer2.Stop();
            if (client6003 != null)
            {
                client6003.Dispoes();
            }
            this.Close();
        
        }

        #endregion


        #region 操作框

        /// <summary>
        /// 拉度条 操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            lblmTiao.Text = tblmTiao.Value.ToString() + "%";
            string order = string.Format("100000{0}", tblmTiao.Value.ToString("X2"));
            sendOrder(order);
        }

        private void btnlmTest_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = 1000;
                if (btnlmTest.BackColor == Color.White)
                {
                    btnlmTest.BackColor = Color.FromArgb(204, 235, 248);
                    timer1.Start();
                }
                else
                {
                    btnlmTest.BackColor = Color.White;
                    timer1.Stop();
                    sendOrder("10000000");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                sendOrder("01000000");
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        #endregion


        #region 曲线图 
        private void chart1_Click(object sender, EventArgs e)
        {
            try
            {
                //用来注册键盘焦点
                panel1.Focus();
                Point p = chart1.PointToClient(MousePosition);
                double XVal = chart1.ChartAreas[0].CursorX.Position;//获取X轴

                if (0 < XVal && XVal <= 100)
                {
                    sendliangdu(Convert.ToInt32(XVal));
                }
            }
            catch { 
            
            }
        }

        /// <summary>
        /// 双击曲线图修改数据点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Point p = chart1.PointToClient(MousePosition);
                
                double XVal = chart1.ChartAreas[0].CursorX.Position;//获取X轴
                double YVal = chart1.ChartAreas[0].CursorY.Position;//获取Y轴
                int x = Convert.ToInt32(XVal);
                //确保xy的值为正常值
                if (0 < XVal && XVal <= 100)
                {
                    if (YVal < 100 ) 
                    {
                        YVal = 0.0;
                    }
                    if (YVal > 9900)
                    {
                        YVal = 10000.0;
                    }
                    switch(x)
                    {
                        case 1:
                            getLinespA(XVal, YVal, 100, yData[99]);
                            break;
                        case 25:
                            getLinespA(1, yData[0], XVal, YVal);
                            getLinespA(XVal, YVal, 50, yData[49]);
                            break;
                        case 50:
                            getLinespA(25, yData[24], XVal, YVal);
                            getLinespA(XVal, YVal, 75, yData[74]);
                            break;
                        case 75:
                            getLinespA(50, yData[49], XVal, YVal);
                            getLinespA(XVal, YVal, 100, yData[99]);
                            break;
                        case 100:
                            getLinespA(1, yData[0], XVal, YVal);
                            break;

                        default:
                            if (75 < x && x < 100)
                            {
                                getLinespA(75, yData[74], XVal, YVal);
                                getLinespA(XVal, YVal, 100, yData[99]);
                            }
                            else if (50 < x && x < 75)
                            {
                                getLinespA(50, yData[49], XVal, YVal);
                                getLinespA(XVal, YVal, 75, yData[74]);
                            }
                            else if (25 < x && x < 50)
                            {
                                getLinespA(25, yData[24], XVal, YVal);
                                getLinespA(XVal, YVal, 50, yData[49]);
                            }
                            else
                            {
                                getLinespA(1, yData[0], XVal, YVal);
                                getLinespA(XVal, YVal, 25, yData[24]);
                            }
                            break;

                    }
                    chart1.Series[0].Points.DataBindXY(xData, yData);
                    //当连接时候发送当前亮度代码
                    sendliangdu(x);
                    updateTable();
                }
                
                
            }
            catch 
            {
                //接收信息发生异常
                //MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取二次函数的 a  以 (x1,y1)点为定点产生
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private void getLinespA(double x1, double y1, double x2, double y2)
        {
            double a;
            //左边是否存在高于的y1值
            bool isMax = false;
            bool isMin = false;
            int maxIndex = 0;
            //找左边y1小过y2 没有就以y2为标准
            for (int i = Convert.ToInt32(x2); i > 0 ; i--)
            {
                if (yData[i - 1] > y2)
                {
                    isMax = true;
                    maxIndex = i;
                    break;
                }
            }
            if (isMax)
            {
                //寻找左边小于y2的值 是属于哪个区间 
                for (int i = maxIndex; i > 0; i--)
                {
                    if (yData[i - 1] <= y2)
                    {
                        if (i >= 75)
                        {
                            x1 = 75;
                            y1 = yData[74];
                        }
                        else if (i >= 50)
                        {
                            x1 = 50;
                            y1 = yData[49];
                        }
                        else if (i >= 25)
                        {
                            x1 = 25;
                            y1 = yData[24];
                        }
                        else
                        {
                            x1 = 1;
                            y1 = yData[0];
                        }
                        isMin = true;
                        break;
                    }

                }//for

                //左边的y值都比y2大
                if (!isMin)
                {
                    x1 = 1;
                    yData[0] = Convert.ToInt32(y2);
                }

            }
            if (y1 < y2)
            {
                a = (y2 - y1) / Math.Pow(x2 - x1, 2);
            }
            else
            {
                a = 0;
                y1 = y2;
            }
            
   
            for (int i = Convert.ToInt32(x1); i <= Convert.ToInt32(x2); i++)
            {

                yData[i - 1] = Convert.ToInt32(a * Math.Pow(i - x1, 2) + y1);
            }
            
            
        }

        //发送当前亮度
        private void sendliangdu(int x)
        {
            //当连接时候发送当前亮度代码
            string str = yData[x - 1].ToString("X8");
            sendRegOrder(str, "03");
  
        }




        /// <summary>
        /// 曲线图按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setDimmer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!panel1.Focused)
                {
                    return;
                }
                int x = (int)chart1.ChartAreas[0].CursorX.Position;//获取X轴
                switch (e.KeyCode)
                {
                    case Keys.Left:

                        chart1.ChartAreas[0].CursorX.Position = x - 1;
                        sendliangdu(x-1);
                        break;
                    case Keys.Right:
                        chart1.ChartAreas[0].CursorX.Position = x + 1;
                        sendliangdu(x + 1);
                        break;
                    default:
                        break;
                }


            }
            catch
            {

            }
        }
        #endregion


        #region 图表
        private void btnLeft_Click(object sender, EventArgs e)
        {
            TextBox[] txtunits = new TextBox[] { txtunit1, txtunit2, txtunit3, txtunit4, txtunit5, txtunit6, txtunit7, txtunit8, txtunit9, txtunit10 };
            if (txtunits[0].Text == "1%")
            {
                return;
            }
            //获取数字
            int start = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", "")) - 10;
            for (int i = 0; i < txtunits.Length; i++)
            {
                txtunits[i].Text = string.Format("{0}%", start + i);
            }
            updateTable();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            TextBox[] txtunits = new TextBox[] { txtunit1, txtunit2, txtunit3, txtunit4, txtunit5, txtunit6, txtunit7, txtunit8, txtunit9, txtunit10 };
            if (txtunits[0].Text == "91%")
            {
                return;
            }
            //获取数字
            int start = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", "")) + 10;
            for (int i = 0; i < txtunits.Length; i++)
            {
                txtunits[i].Text = string.Format("{0}%", start + i);
            }
            updateTable();
            
        }

        /// <summary>
        /// 更新表格上面的数值
        /// </summary>
        private void updateTable()
        {
            TextBox[] txtabs = new TextBox[] { txtabs1, txtabs2, txtabs3, txtabs4, txtabs5, txtabs6, txtabs7, txtabs8, txtabs9, txtabs10 };

            //获取数字
            int start = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", ""));
            int end = start + 9;
            int j = 0;
            for (int i = start; i <= end; i++)
            {
                txtabs[j].Text = yData[i - 1].ToString();
                j++;
            }

        }

        /// <summary>
        /// txt值改变并失去焦点后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtabs1_Leave(object sender, EventArgs e)
        {
            ChangeNum();
        }

        private void txtabs1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是wu、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 更新表格上面的数值
        /// </summary>
        private void ChangeNum()
        {
            try
            {
                TextBox[] txtabs = new TextBox[] { txtabs1, txtabs2, txtabs3, txtabs4, txtabs5, txtabs6, txtabs7, txtabs8, txtabs9, txtabs10 };

                //获取数字
                int start = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", ""));
                int end = start + 9;
                int j = 0;
                int data = 0;
                for (int i = start; i <= end; i++)
                {
                    data = Convert.ToInt32(Regex.Replace(txtabs[j].Text, @"[^\d]*", ""));
                    if (data > 10000)
                    {
                        data = 10000;
                    }
                    else if (data < 0)
                    {
                        data = 1;
                    }
                    yData[i - 1] = data;
                    j++;
                }
                //更新曲线
                chart1.Series[0].Points.DataBindXY(xData, yData);
            }
            catch {
                updateTable();
            }

        }

        #endregion


        #region 菜单栏 按钮
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            getFormState(JsonConvert.SerializeObject(FileMesege.portDimmer));
            
        }

        private void btnIni_Click(object sender, EventArgs e)
        {
            getFormState("64", "64", "01", "64", "00", "");
        }

        private void btnAllWrite_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.PortDimmer portDimmer = SaveFormState();
                if (portDimmer == null)
                {
                    return;
                }
                string id = "";
                string msg = "";
                bool isSend = true;
               
                foreach (DataJson.DevPort dp  in devModuel.devPortList)
                {
                    if (dp.portType == DevPort.portType)
                    {
                        dp.portContent = JsonConvert.SerializeObject(portDimmer);
                        id = dp.portID.ToString();
                        isSend = sendRegOrder("01", id, "64");//设置为线性模式 00自定义 01曲线 02废除了 
                        SocketUtil.DelayMilli(1000);
                        isSend = sendRegOrder(portDimmer.powerState, id, "40");
                        SocketUtil.DelayMilli(1000);
                        isSend = sendRegOrder(portDimmer.onState, id, "60");
                        SocketUtil.DelayMilli(1000);
                        isSend = sendRegOrder(portDimmer.changeState, id, "61");
                        SocketUtil.DelayMilli(1000);
                        isSend = sendRegOrder(portDimmer.max, id, "62");
                        SocketUtil.DelayMilli(1000);
                        isSend = sendRegOrder(portDimmer.min, id, "63");
                        SocketUtil.DelayMilli(1000);
                        isSend = sendRegOrder(portDimmer.spline, id, "65");
                        SocketUtil.DelayMilli(1000);
                   

                        if (isSend)
                        {
                            msg += string.Format("写入至模块端口{0}成功！\r\n", dp.portID);
                        }
                        else
                        {
                            msg += string.Format("写入至模块端口{0}失败！\r\n", dp.portID);
                            break;
                        }
                       
                    }
                }

                
                if (sendResetdriver())//重启设备
                {
                    MessageBox.Show(msg+"设备重启中请稍候", "提示");
                }
                else
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                DataJson.PortDimmer portDimmer = SaveFormState();
                bool isSend = true;
                if (portDimmer == null)
                {
                    return;
                }
                devPort.portContent = JsonConvert.SerializeObject(portDimmer);
                isSend = sendRegOrder("01", "64");//设置为线性模式 00自定义 01曲线 02废除了 
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(400);
                sendRegOrder(portDimmer.powerState,"40");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(400);
                sendRegOrder(portDimmer.onState,"60");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(400);
                sendRegOrder(portDimmer.changeState, "61");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(400);
                sendRegOrder(portDimmer.max, "62");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(400);
                sendRegOrder(portDimmer.min, "63");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(400);
                sendRegOrder(portDimmer.spline, "65");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    return;
                }
                SocketUtil.DelayMilli(2000);
                if (sendResetdriver())//重启设备
                {
                    MessageBox.Show("成功写入至模块,设备重启中请稍候！", "提示");
                }
                else
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示");
                    
                }
                
                 

               
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            sendGetOrder("40");//上电状态
            SocketUtil.DelayMilli(200);
            sendGetOrder("60");//开启状态
            SocketUtil.DelayMilli(200);
            sendGetOrder("61");
            SocketUtil.DelayMilli(200);
            sendGetOrder("62");
            SocketUtil.DelayMilli(200);
            sendGetOrder("63");
            SocketUtil.DelayMilli(200);
            sendGetOrder("65");

        }

        private void btnImport_Click(object sender, EventArgs e)
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
                op.Title = "请打开工程文件";
                op.Filter = "项目文件（*.line）|*.line|压缩文件（*.json）|*.json|All files(*.*)|*.*";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    GetLinesp(File.ReadAllText(op.FileName));
                    //获取表格数字
                    updateTable();
                    //添加打开过的地址
                    IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath", op.FileName);

                }
       

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
          
            } 
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            try
            {
       
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "请选择保存路径";
                //设置文件类型 
                sfd.Filter = "曲线文件（*.line）|*.line|JSON文件（*.json）|*.json|All files(*.*)|*.*";
                //设置默认文件类型显示顺序 
                sfd.FilterIndex = 1;
                //保存对话框是否记忆上次打开的目录 
                sfd.RestoreDirectory = true;
                //设置默认的文件名

                sfd.FileName = "Linesp";// in wpf is  sfd.FileName = "YourFileName";

                string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath");
                if (System.IO.File.Exists(newPath))
                {
                    //设置此次默认目录为上一次选中目录  
                    sfd.InitialDirectory = newPath;

                }
                //点了保存按钮进入 
                if (sfd.ShowDialog() == DialogResult.OK)
                {

                    string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                    IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "splinePath", localFilePath);
                    File.WriteAllText(localFilePath,SaveLinesp());
                        
                }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            
            }  
        }

        #endregion


        #region 把十六进制参数 展现在窗体中

        /// <summary>
        /// 解释对象 在窗体展示对象参数
        /// </summary>
        /// <param name="portDimmer"></param>
        private void getFormState( string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                return;
            }
            DataJson.PortDimmer portDimmer = JsonConvert.DeserializeObject<DataJson.PortDimmer>(info);
            if (portDimmer == null)
            {
                return;
            }
            //上电状态
            GetPowerState(portDimmer.powerState);
            //开启状态
            GetOnState(portDimmer.onState);
            //渐变时间
            GetChangeState(portDimmer.changeState);
            //亮度最大值
            GetMax(portDimmer.max);
            //亮度最小值
            GetMin(portDimmer.min);
            //获取曲线
            GetLinesp(portDimmer.spline);
            //获取表格数字
            updateTable();
        }

        /// <summary>
        /// 输入参数  在窗体展示参数
        /// </summary>
        /// <param name="powerState"></param>
        /// <param name="onState"></param>
        /// <param name="changeState"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="spline"></param>
        private void getFormState(string powerState,string onState,string changeState,string max,string min,string spline)
        {
            //上电状态
            GetPowerState(powerState);
            //开启状态
            GetOnState(onState);
            //渐变时间
            GetChangeState(changeState);
            //亮度最大值
            GetMax(max);
            //亮度最小值
            GetMin(min);
            //获取曲线
            GetLinesp(spline);
            //获取表格数字
            updateTable();
        }

        /// <summary>
        /// 获取上电状态
        /// </summary>
        /// <param name="info"></param>
        private void GetPowerState(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                cbPowerState.SelectedItem = null;
                return;
            }
            if (info == "00")
            {
                cbPowerState.SelectedItem = cbPowerState.Items[0];
            }
            else if (info == "ff" || info == "FF")
            {
                cbPowerState.SelectedItem = cbPowerState.Items[11];
            }
            else
            {
                cbPowerState.Text = string.Format("{0}%", Convert.ToInt32(info, 16));
            }

        }

        /// <summary>
        /// 获取开启状态
        /// </summary>
        /// <param name="info"></param>
        private void GetOnState(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                cbOnState.SelectedItem = null;
                return;
            }
            if (info == "00")
            {
                cbOnState.SelectedItem = cbOnState.Items[10];
            }
            else
            {

                cbOnState.Text = string.Format("{0}%", Convert.ToInt32(info, 16));
            }
  
        }


        /// <summary>
        /// 获取渐变时间
        /// </summary>
        /// <param name="info"></param>
        private void GetChangeState(string info)
        {
            try
            {
                if (string.IsNullOrEmpty(info))
                {
                    cbChangeState.SelectedItem = null;
                    return;
                }
                switch (info)
                {
                    case "f6":
                        cbChangeState.SelectedItem = cbChangeState.Items[1];
                        break;
                    case "f7":
                        cbChangeState.SelectedItem = cbChangeState.Items[2];
                        break;
                    case "f8":
                        cbChangeState.SelectedItem = cbChangeState.Items[3];
                        break;
                    case "f9":
                        cbChangeState.SelectedItem = cbChangeState.Items[4];
                        break;
                    case "fa":
                        cbChangeState.SelectedItem = cbChangeState.Items[5];
                        break;
                    case "fb":
                        cbChangeState.SelectedItem = cbChangeState.Items[6];
                        break;
                    case "fc":
                        cbChangeState.SelectedItem = cbChangeState.Items[7];
                        break;
                    case "fd":
                        cbChangeState.SelectedItem = cbChangeState.Items[8];
                        break;
                    case "fe":
                        cbChangeState.SelectedItem = cbChangeState.Items[9];
                        break;
                    case "00":
                        cbChangeState.SelectedItem = cbChangeState.Items[0];
                        break;
                    case "F6":
                        cbChangeState.SelectedItem = cbChangeState.Items[1];
                        break;
                    case "F7":
                        cbChangeState.SelectedItem = cbChangeState.Items[2];
                        break;
                    case "F8":
                        cbChangeState.SelectedItem = cbChangeState.Items[3];
                        break;
                    case "F9":
                        cbChangeState.SelectedItem = cbChangeState.Items[4];
                        break;
                    case "FA":
                        cbChangeState.SelectedItem = cbChangeState.Items[5];
                        break;
                    case "FB":
                        cbChangeState.SelectedItem = cbChangeState.Items[6];
                        break;
                    case "FC":
                        cbChangeState.SelectedItem = cbChangeState.Items[7];
                        break;
                    case "FD":
                        cbChangeState.SelectedItem = cbChangeState.Items[8];
                        break;
                    case "FE":
                        cbChangeState.SelectedItem = cbChangeState.Items[9];
                        break;
                    default:

                        Regex reg2 = new Regex(@"([\d]+)");
                        for (int i = 10; i < 52; i++)
                        {

                            Match match = reg2.Match(cbChangeState.Items[i].ToString());
                            if (Convert.ToInt32(info, 16).ToString() == match.Groups[1].Value)
                            {
                                cbChangeState.SelectedItem = cbChangeState.Items[i];
                                break;
                            }
                        }

                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }


        /// <summary>
        /// 获取亮度最大值
        /// </summary>
        /// <param name="info"></param>
        private void GetMax(string info)
        {
            try
            {
                if (string.IsNullOrEmpty(info))
                {
                    txtMax.Text = "";
                }
                else
                {
                    txtMax.Text = Convert.ToInt32(info, 16).ToString();
                }
            }
            catch { }
            
        }


        /// <summary>
        /// 获取亮度最小值
        /// </summary>
        /// <param name="info"></param>
        private void GetMin(string info)
        {
            try
            {
                if (string.IsNullOrEmpty(info))
                {
                    txtMin.Text = "";
                }
                else
                {
                    txtMin.Text = Convert.ToInt32(info, 16).ToString();
                }
             }
            catch { }
        }


        /// <summary>
        /// 获取曲线 展现在界面
        /// </summary>
        /// <param name="str"></param>
        private void GetLinesp(string info)
        {
            try
            {
            if (string.IsNullOrEmpty(info))
            {
                chart1Init();
                return;
            }
            info = info.Replace(" ", "");
            for (int i = 0; i < 100; i++)
            {
                string tmp = info.Substring((i * 4), 4);
                while (tmp.Substring(0, 1) == "0")
                {
                    if (tmp.Length == 1)
                    {
                        break;
                    }
                    tmp = tmp.Substring(1, tmp.Length - 1);
                }
                yData[i] = Convert.ToInt32(tmp, 16);

            }
            chart1.Series[0].Points.DataBindXY(xData, yData);
            }
            catch { }
        }


        #endregion


        #region 把窗体参数 存储起来
        private DataJson.PortDimmer SaveFormState()
        {
            DataJson.PortDimmer tmp = new DataJson.PortDimmer();
            tmp.powerState = SavePowerState(cbPowerState.Text);
            tmp.onState = SaveOnState(cbOnState.Text);
            tmp.changeState = SaveChangeState(cbChangeState.Text);
            tmp.max = SaveMax(txtMax.Text);
            tmp.min = SaveMin(txtMin.Text);
            tmp.spline = SaveLinesp();
            return tmp;
        }

        /// <summary>
        /// 保存上电状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string SavePowerState(string info)
        {
            if (info == cbPowerState.Items[0].ToString())
            {
                info = "00";
            }
            else if (info == cbPowerState.Items[11].ToString())
            {
                info = "FF";
            }
            else
            { 
                info  = Regex.Replace(info, @"[^\d]*", "");
                if (string.IsNullOrEmpty(info))
                {
                    info = "";
                }
                else
                {
                    
                    info = Convert.ToInt32(info).ToString("X2");
                }
                
            }
            
            return info;


        }

        /// <summary>
        /// 保存开启状态
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string SaveOnState(string info)
        {
            if (info == cbOnState.Items[10].ToString())
            {
                info = "00";
            }
            else
            {
                info = Regex.Replace(info, @"[^\d]*", "");
                if (string.IsNullOrEmpty(info))
                {
                    info = "";
                }
                else
                {

                    info = Convert.ToInt32(info).ToString("X2");
                }

            }

            return info;

        }

        /// <summary>
        /// 保存渐变状态
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string SaveChangeState(string str)
        {
            if (str == cbChangeState.Items[0].ToString())
            {
                return "00";
            }
            else if (str == cbChangeState.Items[1].ToString())
            {
                return "F6";
            }
            else if (str == cbChangeState.Items[2].ToString())
            {
                return "F7";
            }
            else if (str == cbChangeState.Items[3].ToString())
            {
                return "F8";
            }
            else if (str == cbChangeState.Items[4].ToString())
            {
                return "F9";
            }
            else if (str == cbChangeState.Items[5].ToString())
            {
                return "FA";
            }
            else if (str == cbChangeState.Items[6].ToString())
            {
                return "FB";
            }
            else if (str == cbChangeState.Items[7].ToString())
            {
                return "FC";
            }
            else if (str == cbChangeState.Items[8].ToString())
            {
                return "FD";
            }
            else if (str == cbChangeState.Items[9].ToString())
            {
                return "FE";
            }
            for (int i = 10; i < 52; i++)
            {
                if (str == cbChangeState.Items[i].ToString())
                {
                    Regex reg = new Regex(@"([\d]+)");
                    Match match = reg.Match(str);
                    return Convert.ToInt32(match.Groups[1].Value).ToString("X");

                }
            }
            return "";
        }

        /// <summary>
        /// 保存亮度最大值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string SaveMax(string info)
        {
            try
            {
                int tmp = Convert.ToInt32(txtMax.Text);
                if(tmp>100)
                {
                    tmp = 100;
                }
                if(tmp <0)
                {
                    tmp = 0;
                }
                return tmp.ToString("X");
            }
            catch
            {
                return "64";
            }
            
        }

        /// <summary>
        /// 保存亮度最小值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private string SaveMin(string info)
        {
            try
            {
                int min = Convert.ToInt32(txtMin.Text);
                int max = Convert.ToInt32(txtMax.Text);
                if(max>100)
                {
                    max = 100;
                }
                if(max <0)
                {
                    max = 0;
                }
                if (min > max )
                {
                    min = 0;
                }

                return min.ToString("X");
            }
            catch
            {
                return "0";
            }
        }

        /// <summary>
        /// 保存曲线
        /// </summary>
        /// <returns></returns>
        private string SaveLinesp()
        {
            string tmp = "";
            for (int i = 0; i < 100; i++)
            {

                tmp += yData[i].ToString("X4");

            }
            return tmp;
        }


        #endregion

      




    }//endClass
}
