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
using eNet编辑器.Properties;
using eNet编辑器.OtherView;

namespace eNet编辑器.Controller
{
    public partial class SetDimmer : Form
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
        private BackgroundWorker backgroundWorker1;
        private PgView pgv;
        DataJson.PortDimmer portDimmer;
        //曲线数据点
        List<string> xData = new List<string>();
        List<int> yData = new List<int>();
        //默认曲线
        private static string DEFAULT_CURVE = "03E8040D04330459047E04A604CE04F6051E05460564058205A005BE05DC06180654069006CC070807300758078007A807D0080C0848088408C008FC0938097409B009EC0A280A640AA00ADC0B180B540B900BCC0C080C440C800CBC0CF80D340D700DAC0DE80E240E600E9C0ED80F140F500F8C0FC81004105410A410F41144119411E41234128412D41324137413C41414146414B41504155415A415F41644169E16F8175217AC1806187418E2195019BE1A2C1AB81B441BD01C5C1CE81DD81EC81FB820A82710";

        public SetDimmer()
        {
            InitializeComponent();
        }

        private void setDimmer_Load(object sender, EventArgs e)
        {
            tcp6003receviceDelegate += new Action<string>(tcp6003ReceviceDelegateMsg);
            label1.Text = label1.Text + "-" + Resources.Port + DevPort.portID.ToString();
            //链接tcp
            Connect6003Tcp(ip);
            lastIP = ip.Split('.')[3];
            timer2.Start();
            //图表初始化
            chart1Init();
            //窗口状态初始化
            getFormState(devPort.portContent);
            groupBox2.Click += new EventHandler(groupBox2Click);
            cbMode.SelectedIndex = 0;

        }

        

        #region  图形初始化函数

        /// <summary>
        /// 图表初始化
        /// </summary>
        private void chart1Init()
        {
            //开关曲线
            xData.Clear();
            yData.Clear();
            for (int i = 1; i <= 100; i++)
            {
                xData.Add(i.ToString() + "%");
                yData.Add(10000);

            }
            //线条颜色
            chart1.Series[0].Color = Color.Green;
            //线条粗细
            chart1.Series[0].BorderWidth = 1;
            //标记点边框颜色      
            chart1.Series[0].MarkerBorderColor = Color.Blue;
            //标记点边框大小
            chart1.Series[0].MarkerBorderWidth = 2; //chart1.;// Xaxis 
            //标记点中心颜色
            chart1.Series[0].MarkerColor = Color.LightBlue;//AxisColor
            //标记点大小
            chart1.Series[0].MarkerSize = 2;
            //标记点类型     
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle;
            //标记点偏移量
            chart1.Series[0].MarkerStep = 1;
            

            //需要提示的信息    
            chart1.Series[0].ToolTip = "X轴：#VALX\nY轴：#VAL\n";
            //将文字移到外侧
            chart1.Series[0]["PieLabelStyle"] = "Outside";
            //绘制黑色的连线
            chart1.Series[0]["PieSplineColor"] = "Black";

            chart1.Series[0].ShadowOffset = 0;
              
            chart1.ChartAreas[0].AxisX.IsLabelAutoFit = false;
            //chart1.ChartAreas[0].AxisX.Title = "百分点(0-100%)";
            //chart1.ChartAreas[0].AxisY.Title = "TEMPERATURE";
            chart1.Series[0].Points.DataBindXY(xData, yData);

            //改变X轴刻度间隔
            chart1.ChartAreas[0].AxisX.Interval = 5;
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
                int x = Convert.ToInt32(XVal);
                if (0 < x && x <= 100)
                {
                    sendliangdu(x);
                }
                TxtBackStyleChange(x,false);

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
                int y = Convert.ToInt32(YVal);
                LineUpdateByPoint(x,y);
                chart1.Series[0].Points.DataBindXY(xData, yData);
                //当连接时候发送当前亮度代码
                sendliangdu(x);
                updateTable();


            }
            catch 
            {
                //接收信息发生异常
                //MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取二次函数的 a  以 (x1,y1)点为定点产生 曲线函数
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
            for (int i = Convert.ToInt32(x2); i > 0; i--)
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

        /// <summary>
        /// 根据某一个点更新图表
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void LineUpdateByPoint(int x,int y)
        {
            //确保xy的值为正常值
            if (x < 1 || x > 100)
            {
                return;
            }
            if (y < 1 || y > 11000)
            {
                return;
            }
            if (y > 10000)
            {
                y = 10000;
            }
            if (y < 100)
            {
                y = 100;
            }
            //确保xy的值为正常值
            if (cbMode.SelectedIndex == 0)
            {
                switch (x)
                {
                    case 1:
                        LinePointUpdate(1, y, 10, yData[9]);
                        break;
                    case 10:
                        if (yData[0] <= y && y <= yData[24])
                        {
                            LinePointUpdate(1, yData[0], 10, y);
                            LinePointUpdate(10, y, 25, yData[24]);

                        }
                        else
                        {
                            if (y < yData[0])
                            {
                                LinePointUpdate(1, yData[0], 10, yData[0]);
                                LinePointUpdate(10, yData[0], 25, yData[24]);
                            }
                            else if (y > yData[24])
                            {
                                LinePointUpdate(1, yData[0], 10, yData[24]);
                                LinePointUpdate(10, yData[24], 25, yData[24]);
                            }
                        }
                        

                        break;
                    case 25:
                    
                        //正常范围值    
                        if (yData[9] <= y && y <= yData[49])
                        {
                            LinePointUpdate(10, yData[9], 25, y);
                            LinePointUpdate(25, y, 50, yData[49]);

                        }
                        else
                        {
                            
                            if (y < yData[9])
                            {
                                //范围值小于最低
                                LinePointUpdate(10, yData[9], 25, yData[9]);
                                LinePointUpdate(25, yData[9], 50, yData[49]);
                            }
                            else if (y > yData[49])
                            {
                                //范围值大于最大
                                LinePointUpdate(10, yData[9], 25, yData[49]);
                                LinePointUpdate(25, yData[49], 50, yData[49]);
                            }
                        }
                        break;
                    case 50:
                        //正常范围值    
                        if (yData[24] <= y && y <= yData[74])
                        {
                            LinePointUpdate(25, yData[24], 50, y);
                            LinePointUpdate(50, y, 75, yData[74]);

                        }
                        else
                        {

                            if (y < yData[24])
                            {
                                //范围值小于最低
                                LinePointUpdate(25, yData[24], 50, yData[24]);
                                LinePointUpdate(50, yData[24], 75, yData[74]);
                            }
                            else if (y > yData[74])
                            {
                                //范围值大于最大
                                LinePointUpdate(25, yData[24], 50, yData[74]);
                                LinePointUpdate(50, yData[74], 75, yData[74]);
                            }
                        }
                        break;
                    case 75:
               
                        //正常范围值    
                        if (yData[49] <= y && y <= yData[99])
                        {
                            LinePointUpdate(50, yData[49], 75, y);
                            LinePointUpdate(75, y, 100, yData[99]);

                        }
                        else
                        {

                            if (y < yData[49])
                            {
                                //范围值小于最低
                                LinePointUpdate(50, yData[49], 75, yData[49]);
                                LinePointUpdate(75, yData[49], 100, yData[99]);
                            }
                            else if (y > yData[99])
                            {
                                //范围值大于最大
                                LinePointUpdate(50, yData[49], 75, yData[99]);
                                LinePointUpdate(75, yData[99], 100, yData[99]);
                            }
                        }
                        break;
                    case 100:
                        if (y <= yData[74])
                        {
                            LinePointUpdate(75, yData[74], 100, yData[74]);
                        }
                        else
                        {
                            LinePointUpdate(75, yData[74], 100, y);

                        }

                        break;
                    default:
                        break;

                }
            }
            else if (cbMode.SelectedIndex == 1)
            {
                switch (x)
                {
                    case 1:
                        LinePointUpdate(1, y, 10, yData[9]);
                        break;
                    case 10:
                        if (yData[0] <= y && y <= yData[19])
                        {
                            LinePointUpdate(1, yData[0], 10, y);
                            LinePointUpdate(10, y, 20, yData[19]);

                        }
                        else
                        {
                            if (y < yData[0])
                            {
                                LinePointUpdate(1, yData[0], 10, yData[0]);
                                LinePointUpdate(10, yData[0], 20, yData[19]);
                            }
                            else if (y > yData[19])
                            {
                                LinePointUpdate(1, yData[0], 10, yData[19]);
                                LinePointUpdate(10, yData[19], 20, yData[19]);
                            }
                        }


                        break;
                    case 20:

                        //正常范围值    
                        if (yData[9] <= y && y <= yData[29])
                        {
                            LinePointUpdate(10, yData[9], 20, y);
                            LinePointUpdate(20, y, 30, yData[29]);

                        }
                        else
                        {

                            if (y < yData[9])
                            {
                                //范围值小于最低
                                LinePointUpdate(10, yData[9], 20, yData[9]);
                                LinePointUpdate(20, yData[9], 30, yData[29]);
                            }
                            else if (y > yData[29])
                            {
                                //范围值大于最大
                                LinePointUpdate(10, yData[9], 20, yData[29]);
                                LinePointUpdate(20, yData[29], 30, yData[29]);
                            }
                        }
                        break;
                    case 30:
                        if (yData[19] <= y && y <= yData[39])
                        {
                            LinePointUpdate(20, yData[19], 30, y);
                            LinePointUpdate(30, y, 40, yData[39]);

                        }
                        else
                        {
                            if (y < yData[19])
                            {
                                LinePointUpdate(20, yData[19], 30, yData[19]);
                                LinePointUpdate(30, yData[19], 40, yData[39]);
                            }
                            else if (y > yData[39])
                            {
                                LinePointUpdate(20, yData[19], 30, yData[39]);
                                LinePointUpdate(30, yData[39], 40, yData[39]);
                            }
                        }


                        break;
                    case 40:
                        if (yData[29] <= y && y <= yData[49])
                        {
                            LinePointUpdate(30, yData[29], 40, y);
                            LinePointUpdate(40, y, 50, yData[49]);

                        }
                        else
                        {
                            if (y < yData[29])
                            {
                                LinePointUpdate(30, yData[29], 40, yData[29]);
                                LinePointUpdate(40, yData[29], 50, yData[49]);
                            }
                            else if (y > yData[49])
                            {
                                LinePointUpdate(30, yData[29], 40, yData[49]);
                                LinePointUpdate(40, yData[49], 50, yData[49]);
                            }
                        }


                        break;
                    case 50:
                        //正常范围值    
                        if (yData[39] <= y && y <= yData[59])
                        {
                            LinePointUpdate(40, yData[39], 50, y);
                            LinePointUpdate(50, y, 60, yData[59]);

                        }
                        else
                        {

                            if (y < yData[39])
                            {
                                //范围值小于最低
                                LinePointUpdate(40, yData[39], 50, yData[39]);
                                LinePointUpdate(50, yData[39], 60, yData[59]);
                            }
                            else if (y > yData[59])
                            {
                                //范围值大于最大
                                LinePointUpdate(40, yData[39], 50, yData[59]);
                                LinePointUpdate(50, yData[59], 60, yData[59]);
                            }
                        }
                        break;
                    case 60:
                        if (yData[49] <= y && y <= yData[69])
                        {
                            LinePointUpdate(50, yData[49], 60, y);
                            LinePointUpdate(60, y, 70, yData[69]);

                        }
                        else
                        {
                            if (y < yData[49])
                            {
                                LinePointUpdate(50, yData[49], 60, yData[49]);
                                LinePointUpdate(60, yData[49], 70, yData[69]);
                            }
                            else if (y > yData[69])
                            {
                                LinePointUpdate(50, yData[49], 60, yData[69]);
                                LinePointUpdate(60, yData[69], 70, yData[69]);
                            }
                        }
                        break;
                    case 70:

                        //正常范围值    
                        if (yData[59] <= y && y <= yData[79])
                        {
                            LinePointUpdate(60, yData[59], 70, y);
                            LinePointUpdate(70, y, 80, yData[79]);

                        }
                        else
                        {

                            if (y < yData[59])
                            {
                                //范围值小于最低
                                LinePointUpdate(60, yData[59], 70, yData[59]);
                                LinePointUpdate(70, yData[59], 80, yData[79]);
                            }
                            else if (y > yData[79])
                            {
                                //范围值大于最大
                                LinePointUpdate(60, yData[59], 70, yData[79]);
                                LinePointUpdate(70, yData[79], 80, yData[79]);
                            }
                        }
                        break;
                    case 80:
                        if (yData[69] <= y && y <= yData[89])
                        {
                            LinePointUpdate(70, yData[69], 80, y);
                            LinePointUpdate(80, y, 90, yData[89]);

                        }
                        else
                        {
                            if (y < yData[69])
                            {
                                LinePointUpdate(70, yData[69], 80, yData[69]);
                                LinePointUpdate(80, yData[69], 90, yData[89]);
                            }
                            else if (y > yData[89])
                            {
                                LinePointUpdate(70, yData[69], 80, yData[89]);
                                LinePointUpdate(80, yData[89], 90, yData[89]);
                            }
                        }


                        break;
                    case 90:
                        if (yData[79] <= y && y <= yData[99])
                        {
                            LinePointUpdate(80, yData[79], 90, y);
                            LinePointUpdate(90, y, 100, yData[99]);

                        }
                        else
                        {
                            if (y < yData[79])
                            {
                                LinePointUpdate(80, yData[79], 90, yData[79]);
                                LinePointUpdate(90, yData[79], 100, yData[99]);
                            }
                            else if (y > yData[99])
                            {
                                LinePointUpdate(80, yData[79], 90, yData[99]);
                                LinePointUpdate(90, yData[99], 100, yData[99]);
                            }
                        }


                        break;
                    case 100:
                        if (y <= yData[89])
                        {
                            LinePointUpdate(90, yData[89], 100, yData[89]);
                        }
                        else
                        {
                            LinePointUpdate(90, yData[89], 100, y);

                        }

                        break;
                    default:
                        break;

                }
            }

           
        }

        /// <summary>
        /// 根据两点产生直线函数
        /// </summary>
        /// <param name="Xstart"></param>
        /// <param name="XEnd"></param>
        /// <param name="YEndVal"></param>
        private void LinePointUpdate(int XStart, int YStartVal,int XEnd,int YEndVal)
        {
            //如果前面比后面高则拉平
            if (YStartVal >= YEndVal)
            {
                for (int i = XStart; i <= XEnd; i++)
                {
                    yData[i - 1] = YEndVal;
                }
            }
            else
            {
                double k = Convert.ToDouble(YEndVal - YStartVal) / Convert.ToDouble(XEnd - XStart);
                double b = YEndVal - (XEnd * k);
                for (int i = XStart; i <= XEnd; i++)
                {
                    yData[i - 1] = Convert.ToInt32(k * i + b);
                }
            }
           

        }


        //发送当前亮度
        private void sendliangdu(int x)
        {
            if (x - 1 < 0)
            {
                return;
            }
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
                if (!panel1.Focused )
                {
                    return;
                }
                keyDown(e);
            }
            catch
            {

            }
        }

        private void SetDimmer_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (!panel1.Focused)
                {
                    return;
                }
                keyUp(e);
            }
            catch
            {

            }
        }

        private void Txtunit1_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown(e);
        }

        private void Txtunit1_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(e);
        }

        //按下修改值
        private void Txtabs1_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown(e);
        
        }

        //释放按键 发送代码
        private void Txtabs1_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(e);
        }

        private void keyDown(KeyEventArgs e)
        {
            try
            {
                int x = (int)chart1.ChartAreas[0].CursorX.Position;//获取X轴
                //int y = (int)chart1.ChartAreas[0].CursorY.Position;//获取X轴
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        if (x - 1 < 0)
                        {
                            break;
                        }
                        chart1.ChartAreas[0].CursorX.Position = x - 1;
                        TxtBackStyleChange(x-1, true);
                        break;
                    case Keys.Right:
                        if (x +1 > 100)
                        {
                            break;
                        }
                        chart1.ChartAreas[0].CursorX.Position = x + 1;
                        TxtBackStyleChange(x + 1, true);
                        break;
                    case Keys.Up:
                        if (x - 1 < 0)
                        {
                            break;
                        }
                        LineUpdateByPoint(x, yData[x - 1] + 5);
                        chart1.Series[0].Points.DataBindXY(xData, yData);

                        updateTable();
                        break;
                    case Keys.Down:
                        if (x - 1 < 0)
                        {
                            break;
                        }
                        LineUpdateByPoint(x, yData[x - 1] - 5);
                        chart1.Series[0].Points.DataBindXY(xData, yData);
                        updateTable();
                        break;
                    default:
                        break;
                }


            }
            catch
            {

            }
        }

        private void keyUp(KeyEventArgs e)
        {
            try
            {
                int x = (int)chart1.ChartAreas[0].CursorX.Position;//获取X轴
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        sendliangdu(x - 1);
                        break;
                    case Keys.Right:
                        sendliangdu(x + 1);
                        break;
                    case Keys.Up:
                        sendliangdu(x);
                        break;
                    case Keys.Down:
                        sendliangdu(x);
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

        #region 百分比 表格

        #region 改变数值 和更新数值
        /// <summary>
        /// 更新表格上面的数值
        /// </summary>
        private void updateTable()
        {

            TextBox[] txtPercent= new TextBox[] { txtunit1, txtunit2, txtunit3, txtunit4, txtunit5, txtunit6, txtunit7, txtunit8, txtunit9, txtunit10, txtunit11};
            TextBox[] txtvals = new TextBox[] { txtabs1, txtabs2, txtabs3, txtabs4, txtabs5, txtabs6, txtabs7, txtabs8, txtabs9, txtabs10, txtabs11};
            TextBox[] txtCankaoVals = new TextBox[] { txtVal1, txtVal2, txtVal3, txtVal4, txtVal5, txtVal6, txtVal7, txtVal8, txtVal9, txtVal10, txtVal11 };
            int percent = 0;
            int val = 0;
            for (int i = 0; i < txtPercent.Length; i++)
            {
                if (txtPercent[i].Visible)
                {
                    percent = Convert.ToInt32(Regex.Replace(txtPercent[i].Text, @"[^\d]*", ""));
                    txtvals[i].Text = yData[percent - 1].ToString();
                    if (string.IsNullOrEmpty(txtVal100.Text))
                    {
                        val = 0;
                    }
                    else
                    {
                        val = Convert.ToInt32(txtVal100.Text);
                    }
                    txtCankaoVals[i].Text = (val*percent/100).ToString();
                }
                    
            }

        }

        /// <summary>
        /// txt值改变并失去焦点后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtabs1_Leave(object sender, EventArgs e)
        {
            //TableValUpdataLine();
        }

        private void txtabs1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            //如果输入的不是数字键，也不是wu、Backspace键，则取消该输入
            /*if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            //回车
            if (e.KeyChar == (char)Keys.Enter)
            {
                groupBox2.Focus();
            }*/
        }

        /// <summary>
        /// 通过表格更新曲线图
        /// </summary>
        private void TableValUpdataLine()
        {
            try
            {
                TextBox[] txtPercent = new TextBox[] { txtunit1, txtunit2, txtunit3, txtunit4, txtunit5, txtunit6, txtunit7, txtunit8, txtunit9, txtunit10, txtunit11 };
                TextBox[] txtvals = new TextBox[] { txtabs1, txtabs2, txtabs3, txtabs4, txtabs5, txtabs6, txtabs7, txtabs8, txtabs9, txtabs10, txtabs11 };
                int percent = 0;
                int y = 0;
                for (int i = 0; i < txtPercent.Length; i++)
                {
                    if (txtPercent[i].Visible)
                    {
                        percent = Convert.ToInt32(Regex.Replace(txtPercent[i].Text, @"[^\d]*", ""));
                        y = Convert.ToInt32(txtvals[i].Text);
                        LineUpdateByPoint(percent, y);
                    }

                }
                //更新曲线
                chart1.Series[0].Points.DataBindXY(xData, yData);

            }
            catch
            {
                updateTable();
            }



        }

        //添加点击事情 获取焦点 用于边框点恐怖地方
        private void groupBox2Click(object sender, EventArgs e)
        {
            groupBox2.Focus();
        }
        #endregion

        #region 点击编辑表格变橙色 图表定位
        /// <summary>
        /// 选中框变橙色 并定位到xy点
        /// </summary>
        /// <param name="XSelect">是否定位到xy轴点</param>
        private void TxtBackStyleChange(int XSelect,bool isChangeLine)
        {
            try
            {
                TextBox[] txtPercent = new TextBox[] { txtunit1, txtunit2, txtunit3, txtunit4, txtunit5, txtunit6, txtunit7, txtunit8, txtunit9, txtunit10, txtunit11 };
                TextBox[] txtvals = new TextBox[] { txtabs1, txtabs2, txtabs3, txtabs4, txtabs5, txtabs6, txtabs7, txtabs8, txtabs9, txtabs10, txtabs11 };
                TextBox[] txtCankaoVals = new TextBox[] { txtVal1, txtVal2, txtVal3, txtVal4, txtVal5, txtVal6, txtVal7, txtVal8, txtVal9, txtVal10, txtVal11 };
                int percent = 0;
                int y = 0;
                for (int i = 0; i < txtPercent.Length; i++)
                {
                    if (txtPercent[i].Visible)
                    {
                        percent = Convert.ToInt32(Regex.Replace(txtPercent[i].Text, @"[^\d]*", ""));
                        if (XSelect == percent)
                        {
                            txtPercent[i].BackColor = Color.LightSalmon;
                            txtvals[i].BackColor = Color.LightSalmon;
                            txtCankaoVals[i].BackColor = Color.LightSalmon;
                            if (isChangeLine)
                            {
                                y = Convert.ToInt32(txtvals[i].Text);
                                chart1.ChartAreas[0].CursorX.Position = Convert.ToDouble(percent);
                                chart1.ChartAreas[0].CursorY.Position = Convert.ToDouble(y);
                                sendliangdu(percent);
                            }
                            
                        }
                        else
                        {
                            txtPercent[i].BackColor = Color.WhiteSmoke;
                            txtvals[i].BackColor = Color.WhiteSmoke;
                            txtCankaoVals[i].BackColor = Color.WhiteSmoke;
                        }
                    }

                }
                //更新曲线
                chart1.Series[0].Points.DataBindXY(xData, yData);

            }
            catch
            {
                updateTable();
            }
        }

        private void Txtabs1_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent,true);
            panel1.Focus();
            
        }

        private void Txtabs2_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit2.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs3_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit3.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs4_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit4.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs5_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit5.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs6_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit6.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs7_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit7.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs8_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit8.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs9_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit9.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs10_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit10.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtabs11_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit11.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }
        private void Txtunit1_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit2_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit2.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit3_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit3.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit4_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit4.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit5_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit5.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit6_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit6.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit7_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit7.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit8_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit8.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit9_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit9.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit10_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit10.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void Txtunit11_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit11.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal1_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit1.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();

        }

        private void TxtVal2_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit2.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal3_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit3.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal4_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit4.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal5_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit5.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal6_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit6.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal7_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit7.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal8_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit8.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal9_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit9.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal10_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit10.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        private void TxtVal11_Enter(object sender, EventArgs e)
        {
            int percent = Convert.ToInt32(Regex.Replace(txtunit11.Text, @"[^\d]*", ""));
            TxtBackStyleChange(percent, true);
            panel1.Focus();
        }

        #endregion

        #endregion

        #region 菜单栏 按钮
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            getFormState(JsonConvert.SerializeObject(FileMesege.portDimmer));
        }

        private void btnIni_Click(object sender, EventArgs e)
        {
            //getFormState("0", "10", "10", "64", "00", "");
            cbPowerState.SelectedIndex = 0;
            cbOnState.SelectedIndex = 10;
            cbChangeState.SelectedIndex = 10;
            txtMax.Text = "100";
            txtMin.Text = "0";
            GetLinesp(DEFAULT_CURVE);
            //获取表格数字
            updateTable();
          
        }

        private void btnAllWrite_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork_AllWrite;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                portDimmer = SaveFormState();
                pgv = new PgView();
                pgv.setMaxValue(devModuel.devPortList.Count *7 +1);
                this.Enabled = false;
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

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork_Write;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                portDimmer = SaveFormState();
                pgv = new PgView();
                pgv.setMaxValue(8);
                this.Enabled = false;
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

        #region 全部写入 和写入 
        private void BackgroundWorker1_DoWork_Write(object sender, DoWorkEventArgs e)
        {
            try
            {
                bool isSend = true;
                int count = 0;
                if (portDimmer == null)
                {
                    return;
                }
                devPort.portContent = JsonConvert.SerializeObject(portDimmer).Replace("\\", "");

                isSend = sendRegOrder("01", "64");//设置为线性模式 00自定义 01曲线 02废除了 
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                sendRegOrder(portDimmer.powerState, "40");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                sendRegOrder(portDimmer.onState, "60");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                sendRegOrder(portDimmer.changeState, "61");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                sendRegOrder(portDimmer.max, "62");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                sendRegOrder(portDimmer.min, "63");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                sendRegOrder(portDimmer.spline, "65");
                if (!isSend)
                {
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示",MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                ToolsUtil.DelayMilli(800);
                count++;
                backgroundWorker1.ReportProgress(count);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                isSend = sendResetdriver();
                if (isSend)
                {
                    ToolsUtil.DelayMilli(3000);
                    count++;
                    backgroundWorker1.ReportProgress(count);
                    MessageBox.Show("成功写入至模块,设备重启中请稍候！", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
                else
                {
                    count++;
                    backgroundWorker1.ReportProgress(count);
                    MessageBox.Show("写入失败！\r\n请检查网络连接或参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                }
                




            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void BackgroundWorker1_DoWork_AllWrite(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (portDimmer == null)
                {
                    return;
                }
                string id = "";
                string msg = "";
                bool isSend = true;
                int count = 0;
                foreach (DataJson.DevPort dp in devModuel.devPortList)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }
                    if (dp.portType == DevPort.portType)
                    {
                        dp.portContent = JsonConvert.SerializeObject(portDimmer);
                        id = dp.portID.ToString();
                        isSend = sendRegOrder("01", id, "64");//设置为线性模式 00自定义 01曲线 02废除了 
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);

                        isSend = sendRegOrder(portDimmer.powerState, id, "40");
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);

                        isSend = sendRegOrder(portDimmer.onState, id, "60");
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);
                        isSend = sendRegOrder(portDimmer.changeState, id, "61");
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);

                        isSend = sendRegOrder(portDimmer.max, id, "62");
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);

                        isSend = sendRegOrder(portDimmer.min, id, "63");
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);

                        isSend = sendRegOrder(portDimmer.spline, id, "65");
                        ToolsUtil.DelayMilli(1000);
                        count++;
                        backgroundWorker1.ReportProgress(count);

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

                isSend = sendResetdriver();
                if (isSend)
                {
                    ToolsUtil.DelayMilli(5000);
                }
                else
                {
                    msg += "发送重启指令失败！\r\n";
                }
                count++;
                backgroundWorker1.ReportProgress(count);
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //设置值
            pgv.setValue(e.ProgressPercentage);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.Enabled = true;
                if (pgv != null)
                {
                    pgv.Close();
                }
            }
            catch
            {

            }
        }
        #endregion

        private void btnRead_Click(object sender, EventArgs e)
        {
            this.btnRead.Enabled = false;
            sendGetOrder("40");//上电状态
            ToolsUtil.DelayMilli(200);
            sendGetOrder("60");//开启状态
            ToolsUtil.DelayMilli(200);
            sendGetOrder("61");
            ToolsUtil.DelayMilli(200);
            sendGetOrder("62");
            ToolsUtil.DelayMilli(200);
            sendGetOrder("63");
            ToolsUtil.DelayMilli(200);
            sendGetOrder("65");
            this.btnRead.Enabled = true;
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
            try
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
            catch
            {
                return null;
            }
            
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
      
        #region 默认直线 曲线按钮
        private void BtnSwitchLine_Click(object sender, EventArgs e)
        {
            //开关曲线
            xData.Clear();
            yData.Clear();
            for (int i = 1; i <= 100; i++)
            {
                xData.Add(i.ToString() + "%");
                yData.Add(10000);

            }
            chart1.Series[0].Points.DataBindXY(xData, yData);
            updateTable();
        }

        private void BtnStraightLine_Click(object sender, EventArgs e)
        {
            //直线
            xData.Clear();
            yData.Clear();
            for (int i = 1; i <= 100; i++)
            {
                xData.Add(i.ToString() + "%");
                yData.Add(i * 100);

            }
            chart1.Series[0].Points.DataBindXY(xData, yData);
            updateTable();
        }

        
        private void BtnCurveLine_Click(object sender, EventArgs e)
        {
            GetLinesp(DEFAULT_CURVE);
            //获取表格数字
            updateTable();
            //曲线
            /*getLinespA(1, 100, 100, 10000);
            chart1.Series[0].Points.DataBindXY(xData, yData);
            updateTable();*/
        }


        #endregion

        #region 快速模式和精细模式切换
        private void CbMode_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbMode.SelectedIndex == 0)
            {
                //快速模式
                ModeVisable(true);
            }
            else if (cbMode.SelectedIndex == 1)
            {
                //精细模式
                ModeVisable(false);
            }

        }

        private void ModeVisable(bool isQuick)
        {
            txtunit7.Visible = !isQuick;
            txtunit8.Visible = !isQuick;
            txtunit9.Visible = !isQuick;
            txtunit10.Visible = !isQuick;
            txtunit11.Visible = !isQuick;

            txtabs7.Visible = !isQuick;
            txtabs8.Visible = !isQuick;
            txtabs9.Visible = !isQuick;
            txtabs10.Visible = !isQuick;
            txtabs11.Visible = !isQuick;

            txtVal7.Visible = !isQuick;
            txtVal8.Visible = !isQuick;
            txtVal9.Visible = !isQuick;
            txtVal10.Visible = !isQuick;
            txtVal11.Visible = !isQuick;
            if (isQuick)
            {
                flowLayoutPanel2.Size = new Size(370,78);
                txtunit1.Text = "1%";
                txtunit2.Text = "10%";
                txtunit3.Text = "25%";
                txtunit4.Text = "50%";
                txtunit5.Text = "75%";
                txtunit6.Text = "100%";
            }
            else
            {
                flowLayoutPanel2.Size = new Size(653, 78);
                txtunit1.Text = "1%";
                txtunit2.Text = "10%";
                txtunit3.Text = "20%";
                txtunit4.Text = "30%";
                txtunit5.Text = "40%";
                txtunit6.Text = "50%";
                txtunit7.Text = "60%";
                txtunit8.Text = "70%";
                txtunit9.Text = "80%";
                txtunit10.Text = "90%";
                txtunit11.Text = "100%";
            }
            int x = 0;
            try
            {
                x = Convert.ToInt32(chart1.ChartAreas[0].CursorX.Position);
            }catch{ }
            TxtBackStyleChange(x, false);
            updateTable();

        }


        #endregion

        #region 左栏参数设置默认值  和限制输入 参考输入值
        private void TxtMax_KeyPress(object sender, KeyPressEventArgs e)
        {
             if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
        private void BtnSetIni_Click(object sender, EventArgs e)
        {
            /*cbPowerState.SelectedIndex = 0;
            cbOnState.SelectedIndex = 10;
            cbChangeState.SelectedIndex = 10;
            txtMax.Text = "100";
            txtMin.Text = "0";*/
        }

        /// <summary>
        /// 100参考照度值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtVal100_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是数字键，也不是wu、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
            //回车
            if (e.KeyChar == (char)Keys.Enter)
            {
                groupBox2.Focus();
            }
        }
        private void TxtVal100_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtVal100.Text))
                {
                    txtVal100.Text = "0";
                }
                int val = Convert.ToInt32(txtVal100.Text);
                if (val > 99999)
                {
                    txtVal100.Text = "10000";
                }
                //更新表格内容
                updateTable();
            }
            catch {
            }
           
        }

   

        #endregion


    }//endClass
}
