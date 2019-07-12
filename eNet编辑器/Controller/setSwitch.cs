using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;

namespace eNet编辑器.Controller
{
    public partial class setSwitch : Form
    {

        private string ip;

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


        public setSwitch()
        {
            InitializeComponent();
        }

        private void setSwitch_Load(object sender, EventArgs e)
        {

            tcp6003receviceDelegate += new Action<string>(tcp6003ReceviceDelegateMsg);
            //链接tcp
            Connect6003Tcp(ip);
            lastIP = ip.Split('.')[3];
            timer2.Start();
            LockIni();
            getFormState((DataJson.PortSwitch)devPort.portContent);
        }

        /// <summary>
        /// 互锁设置初始化
        /// </summary>
        private void LockIni()
        {
            ///有Bug 如果两路开关 两路调光
            int num = devPort.portID % 4;
            switch (num)
            {
                case 1:
                    cbLock.Items.Add(string.Format("{0}-{1}", devPort.portID, devPort.portID + 1));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}", devPort.portID, devPort.portID + 1, devPort.portID + 2));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}-{3}", devPort.portID, devPort.portID + 1, devPort.portID + 2, devPort.portID + 3));
                    break;
                case 2:
                    cbLock.Items.Add(string.Format("{0}-{1}", devPort.portID-1, devPort.portID));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}", devPort.portID - 1, devPort.portID, devPort.portID + 1));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}-{3}", devPort.portID - 1, devPort.portID, devPort.portID + 1, devPort.portID+2));
                    break;
                case 3:
                    cbLock.Items.Add(string.Format("{0}-{1}", devPort.portID, devPort.portID + 1));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}", devPort.portID - 2, devPort.portID - 1, devPort.portID));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}-{3}", devPort.portID - 2, devPort.portID - 1, devPort.portID, devPort.portID+1));
                    break;
                case 0:
                    cbLock.Items.Add(string.Format("{0}-{1}", devPort.portID - 1, devPort.portID));
                    cbLock.Items.Add(string.Format("{0}-{1}-{2}-{3}", devPort.portID-3, devPort.portID - 2, devPort.portID - 1, devPort.portID));
                    break;
                default: break;

            }
            cbLock.SelectedItem = cbLock.Items[0];
        }


        #region 重绘
        private void setSwitch_Paint(object sender, PaintEventArgs e)
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
           
            timer1.Stop();
            timer2.Stop();
            if (client6003 != null)
            {
                client6003.Dispoes();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        #endregion


        #region tcp6003 链接 以及处理反馈信息 发送信息

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

                if (client6003 != null )//&& client6003.Connected())
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
                if (client6003 != null )
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
                string data = strArray[i].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[2];
                Match match = reg.Match(strArray[i]);
                if (match.Groups[3].Value == dataID && match.Groups[5].Value == "64")//40上电状态
                {
                    GetPowerState(data);
                }
                else if (match.Groups[3].Value == dataID && match.Groups[5].Value == "96")//60开启状态
                {
                    GetLock(data);
                }
           

            }//for
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
                //发送
                client6003.SendAsync(tmp);

            }
        }

        /// <summary>
        /// 普通发送函数  加端口
        /// </summary>
        /// <param name="dataOrder"></param>
        /// <param name="portID"></param>
        private void sendRegOrder(string dataOrder, string portID, string reg)
        {
            if (string.IsNullOrEmpty(dataOrder) || string.IsNullOrEmpty(portID) || string.IsNullOrEmpty(reg))
            {
                return;
            }
            if (client6003 != null && client6003.Connected())
            {
                reg = Convert.ToInt32(reg, 16).ToString();
                string tmp = string.Format("set;{{{1}.0.{2}.{3}:{4}}};{0};\r\n", dataOrder, lastIP, DevModuel.id, portID, reg);
                //发送
                client6003.SendAsync(tmp);

            }
        }

        /// <summary>
        /// 发送信息函数 发送到寄存器寄存器
        /// </summary>
        /// <param name="dataOrder"></param>
        private void sendRegOrder(string dataOrder, string reg)
        {
            if (string.IsNullOrEmpty(dataOrder) || string.IsNullOrEmpty(reg))
            {
                return;
            }
            if (client6003 != null && client6003.Connected())
            {
                reg = Convert.ToInt32(reg, 16).ToString();
                string tmp = string.Format("set;{{{1}.0.{2}.{3}:{4}}};{0};\r\n", dataOrder, lastIP, DevModuel.id, DevPort.portID, reg);
                //发送
                client6003.SendAsync(tmp);
                //SocketUtil.WriteLog(tmp);
            }
        }

        /// <summary>
        /// 发送读取指令 发送到寄存器寄存器
        /// </summary>
        /// <param name="dataOrder"></param>
        private void sendGetOrder(string reg)
        {
            if (string.IsNullOrEmpty(reg))
            {
                return;
            }
            if (client6003 != null && client6003.Connected())
            {
                reg = Convert.ToInt32(reg, 16).ToString();
                string tmp = string.Format("get;{{{0}.0.{1}.{2}:{3}}};\r\n", lastIP, DevModuel.id, DevPort.portID, reg);
                //发送
                client6003.SendAsync(tmp);

            }
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


        #region 操作
        private void btnOn_Click(object sender, EventArgs e)
        {
            sendOrder("01000001");
        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            sendOrder("01000002");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Interval = 1000;
                if (btnTest.BackColor == Color.White)
                {
                    btnTest.BackColor = Color.FromArgb(204, 235, 248);
                    timer1.Start();
                }
                else
                {
                    btnTest.BackColor = Color.White;
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


        #region 菜单栏 按钮
        private void btnIni_Click(object sender, EventArgs e)
        {
            getFormState("01","00");
        }

        private void btnAllWrite_Click(object sender, EventArgs e)
        {
            try
            {
                SavePowerState(cbPower.Text);
                string id = "";
                string msg = "";
                //当前参数对象 
                DataJson.PortSwitch pw = (DataJson.PortSwitch)devPort.portContent;
                foreach (DataJson.DevPort dp in devModuel.devPortList)
                {
                    if (dp.portType == DevPort.portType)
                    {
                        id = dp.portID.ToString();
                        
                        DataJson.PortSwitch tmpPW = null;
                        if (dp.portContent == null)
                        {
                            tmpPW = new DataJson.PortSwitch();
                        }
                        else
                        {
                            tmpPW = (DataJson.PortSwitch)CommandManager.CloneObject(dp.portContent);
                            //tmpPW = TransExpV2<DataJson.PortSwitch, DataJson.PortSwitch>.Trans((DataJson.PortSwitch)dp.portContent);
                        }
                        tmpPW.powerState = pw.powerState;
                        //复制对象
                        dp.portContent = tmpPW;

                        sendRegOrder(pw.powerState, id, "40");
                        SocketUtil.DelayMilli(500);
               
                        
                        msg += string.Format("上电状态：成功写入至模块端口{0}！\r\n", dp.portID);

                    }
                }

                if (client6003 != null && client6003.Connected())//
                {
                    MessageBox.Show(msg , "提示");
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
                SaveFormState();
                DataJson.PortSwitch pw = (DataJson.PortSwitch)devPort.portContent;
                sendRegOrder(pw.powerState, "40");
                SocketUtil.DelayMilli(400);
                sendRegOrder(pw.interLock, "60");
                SocketUtil.DelayMilli(400);
                if (client6003 != null && client6003.Connected())
                {
                    MessageBox.Show("成功写入至模块！", "提示");
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
          
        }

        #endregion


        #region 把十六进制参数 展现在窗体中
        /// <summary>
        /// 解释对象 在窗体展示对象参数
        /// </summary>
        /// <param name="portSwitch"></param>
        private void getFormState(DataJson.PortSwitch portSwitch)
        {

            //上电状态
            GetPowerState(portSwitch.powerState);
            //互锁状态
            GetLock(portSwitch.interLock);
        }


        private void getFormState(string powerState, string interLock)
        {
            //上电状态
            GetPowerState(powerState);
            //互锁状态
            GetLock(interLock);

        }

        /// <summary>
        /// 获取上电状态
        /// </summary>
        /// <param name="info"></param>
        private void GetPowerState(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                cbPower.SelectedItem = null;
                return;
            }
            if (info == "00")
            {
                cbPower.SelectedItem = cbPower.Items[1];
            }
            else if (info == "ff")
            {
                cbPower.SelectedItem = cbPower.Items[2];
            }
            else//"01"
            {
                cbPower.SelectedItem = cbPower.Items[0];
            }

        }

        /// <summary>
        /// 获取互锁状态
        /// </summary>
        /// <param name="info"></param>
        private void GetLock(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                cbLock.SelectedItem = null;
                return;
            }
  
            //互锁是0-3
            int LockNum = Convert.ToInt32(info.Substring(0, 1)) + 1;
            for (int i = 0; i < cbLock.Items.Count; i++)
            {
                string[] tmpPort = cbLock.Items[i].ToString().Split('-');
                if (tmpPort.Length == LockNum)
                {
                    cbLock.SelectedItem = cbLock.Items[i];
                }
            }
            

        }

        #endregion


        #region 把窗体参数 存储起来
        /// <summary>
        /// 把信息直接存储到该端口中
        /// </summary>
        private void SaveFormState()
        {
            SavePowerState(cbPower.Text);
            SaveLock(cbLock.Text);
            
        }

        /// <summary>
        /// 保存上电状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private void  SavePowerState(string info)
        {
            if (info == cbPower.Items[1].ToString())
            {
                info = "00";
            }
            else if (info == cbPower.Items[2].ToString())
            {
                info = "ff";
            }
            else 
            {
                info = "01";

            }

            DataJson.PortSwitch tmpPW = null;
            if (devPort.portContent == null)
            {
                tmpPW = new DataJson.PortSwitch();
            }
            else
            {
                tmpPW = (DataJson.PortSwitch)CommandManager.CloneObject(devPort.portContent);
                //tmpPW = TransExpV2<DataJson.PortSwitch, DataJson.PortSwitch>.Trans((DataJson.PortSwitch)devPort.portContent);
            }
            tmpPW.powerState = info;

            DevPort.portContent = tmpPW;
         
           
        }

        /// <summary>
        /// 保存互锁状态
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private void SaveLock(string info)
        {
            //读取这个端口是什么互锁 然后这次设置什么互锁  
            DataJson.PortSwitch pw = (DataJson.PortSwitch)devPort.portContent;
            if (pw.interLock != "00")
            {
                //几路互锁0-3
                int oldLockNum = Convert.ToInt32(pw.interLock.Substring(0, 1));
                //互锁状态下第几个端口1-4
                int oldLockPort = Convert.ToInt32(pw.interLock.Substring(1, 1));

                //端口开头
                int heartLockPort = devPort.portID - oldLockPort + 1;
                //端口结尾
                int tailLockPort = heartLockPort + oldLockNum;
                //List端口清空原有的信息


                for (int i = heartLockPort - 1; i < tailLockPort; i++)
                {
                    if (devModuel.devPortList[i].portType == DevPort.portType)
                    {
                        DataJson.PortSwitch tmpPW = null;
                        if (devModuel.devPortList[i].portContent == null)
                        {
                            tmpPW = new DataJson.PortSwitch();
                        }
                        else
                        {
                            tmpPW = (DataJson.PortSwitch)CommandManager.CloneObject(devModuel.devPortList[i].portContent);
                            //tmpPW = TransExpV2<DataJson.PortSwitch, DataJson.PortSwitch>.Trans((DataJson.PortSwitch)devModuel.devPortList[i].portContent);
                        }
                        tmpPW.interLock = "00";
                        devModuel.devPortList[i].portContent = tmpPW;
                    }
                }
            }
           

            string[] LockPorts = info.Split('-');
            if(LockPorts.Length > 1)
            {
                //新几路互锁0-3
                int newLockNum = LockPorts.Length - 1;
                int newLockPort = 1;
                int newHeartLockPort = Convert.ToInt32(LockPorts[0]);
                int newTailLockPort = Convert.ToInt32(LockPorts[LockPorts.Length -1]);               
                for (int i = newHeartLockPort - 1; i < newTailLockPort; i++)
                {
                    if (devModuel.devPortList[i].portType == DevPort.portType)
                    {
                        DataJson.PortSwitch tmpPW = null;
                        if (devModuel.devPortList[i].portContent == null)
                        {
                            tmpPW = new DataJson.PortSwitch();
                        }
                        else
                        {
                            tmpPW = (DataJson.PortSwitch)CommandManager.CloneObject(devModuel.devPortList[i].portContent);
                            //tmpPW = TransExpV2<DataJson.PortSwitch, DataJson.PortSwitch>.Trans((DataJson.PortSwitch)devModuel.devPortList[i].portContent);
                        }
                        
                        tmpPW.interLock = string.Format("{0}{1}", newLockNum, newLockPort);
                        devModuel.devPortList[i].portContent = tmpPW;
                        newLockPort++;
                    }
                }
            }
            else
            {
                //无互锁
                //不用操作 上面已经清空过互锁
            }


        }
        #endregion


    }//class
}
