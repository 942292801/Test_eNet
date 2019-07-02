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
            //图表初始化
            chart1Init();
            updateTable();
            //链接tcp
            Connet6003Tcp(ip);
            lastIP = ip.Split('.')[3];
      
        }


        #region 初始化函数

        /// <summary>
        /// 图表初始化
        /// </summary>
        private void chart1Init()
        {
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
        private void Connet6003Tcp(string ip)
        {
            try
            {
                if (client6003 != null && client6003.Connected())
                {
                    client6003.Dispoes();
                }

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
                    //开启心跳 
                    client6003.SendAsync("SET;00000003;{254.251.0.1};\r\n");
                }
            }
            catch
            {
                //client6003 = null;
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
                        MessageBox.Show("连接发生错误,请检查网络连接");
                        //this.Close();
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
            // 序列化接收信息
            //DataJson.Serial serialList = JsonConvert.DeserializeObject<DataJson.Serial>(bufferMsg);
            bufferMsg = "";

        
        }


        /// <summary>
        /// 普通发送函数
        /// </summary>
        /// <param name="dataOrder"></param>
        private void sendOrder(string dataOrder)
        {
            if (client6003 != null && client6003.Connected())
            {
                string tmp = string.Format("SET;{0};{{{1}.0.{2}.{3}}};\r\n", dataOrder, lastIP, DevModuel.id, DevPort.portID);
                //发送心跳
                client6003.SendAsync(tmp);
            }
        }

        /// <summary>
        /// 普通发送函数  加端口
        /// </summary>
        /// <param name="dataOrder"></param>
        /// <param name="portID"></param>
        private void sendOrder(string dataOrder,string portID)
        {
            if (client6003 != null && client6003.Connected())
            {
                string tmp = string.Format("SET;{0};{{{1}.0.{2}.{3}}};\r\n", dataOrder, lastIP, DevModuel.id, portID);
                //发送心跳
                client6003.SendAsync(tmp);
            }
        }

        /// <summary>
        /// 发送信息函数 发送到寄存器寄存器
        /// </summary>
        /// <param name="dataOrder"></param>
        private void sendRegOrder(string dataOrder, string reg)
        {
            if (client6003 != null && client6003.Connected())
            {
                string tmp = string.Format("set;{{{1}.0.{2}.{3}:{4}}};{0};\r\n", dataOrder, lastIP, DevModuel.id, DevPort.portID, reg);
                //发送心跳
                client6003.SendAsync(tmp);
            }
        }

        #endregion


        #region 重绘
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
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Dispose();
        
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
            catch (Exception ex)
            {
                //接收信息发生异常
                MessageBox.Show(ex.Message);
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
            //如果输入的不是数字键，也不是、Backspace键，则取消该输入
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

      



     

      



   

        

  



    }//endClass
}
