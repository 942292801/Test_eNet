using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using eNet编辑器.Properties;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.ComponentModel;
using eNet编辑器.OtherView;

namespace eNet编辑器.AddForm
{
   
    public partial class OnlineSearch : Form
    {
        //UDP客户端
        UdpSocket udp;
        //网关节点 选中的IP地址
        private string searchIP = string.Empty;
        private bool isSearchIP = false;
        // 记录延时操作
        private long lastTicks = 0;
        //本地IP
        private string Localip = string.Empty;

        TreeMesege tm ;
        public OnlineSearch()
        {
            InitializeComponent();
        }
 

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> TxtShow;

        //停止在线信息查询
        public event Action StopOnline;
        //更新节点
        public event Action UpdateNode;

        private BackgroundWorker backgroundWorker1;
        private PgView pgv;


        private delegate void SearchMasterRcvDelegate(string msg);
        private event SearchMasterRcvDelegate searchMasterRcvDelegate;

        private delegate void GetMasterDevDelegate(string msg);
        private event GetMasterDevDelegate getMasterDevDelegate;

        private delegate void UpdateNodeDelegate();
        private event UpdateNodeDelegate updateNodeDelegate;

        //搜索存放网关IP
        private HashSet<string> masterHs = new HashSet<string>();


        private void OnlineSearch_Load(object sender, EventArgs e)
        {
            StopOnline();
            tm = new TreeMesege();
            searchMasterRcvDelegate = new SearchMasterRcvDelegate(SearchMasterRcvDeal);
            getMasterDevDelegate = new GetMasterDevDelegate(GetMasterDevRcvDeal);
            updateNodeDelegate = new UpdateNodeDelegate(UpdateNode);

        }



        #region 搜索功能
       
        /// <summary>
        /// 搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
   
            try
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                pgv = new PgView();
                pgv.setMaxValue(100);
                //this.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
                pgv.ShowDialog();
                if (pgv.DialogResult != DialogResult.OK)
                {
                    backgroundWorker1.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }

        }


        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (udp != null)
            {
                udp.udpClose();
            }
            //初始化UDP
            udp = new UdpSocket();
            udp.Received += new Action<string, string>((IP, msg) =>
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(msg))
                    {
                        //跨线程调用
                        this.Invoke(searchMasterRcvDelegate, msg);
                    }
                }
                catch (Exception ex)
                {
                    ToolsUtil.WriteLog(ex.Message);
                }
            });

            //获取本地IP
            Localip = ToolsUtil.GetLocalIP();
            //udp 绑定
            udp.udpBing(Localip, ToolsUtil.GetFreePort().ToString());

            if (udp.isbing)
            {
                masterHs.Clear();
                string[] tmps = Localip.Split('.');
                string broadcastIp = String.Format("{0}.{1}.{2}.255", tmps[0], tmps[1], tmps[2]);
                udp.udpSend(broadcastIp, "6002", "Search all");
                ToolsUtil.DelayMilli(200);
                backgroundWorker1.ReportProgress(2, null);
                udp.udpSend(broadcastIp, "6002", "search all");
                ToolsUtil.DelayMilli(200);
                backgroundWorker1.ReportProgress(4, null);
                udp.udpSend("255.255.255.255", "6002", "Search all");
                ToolsUtil.DelayMilli(200);
                backgroundWorker1.ReportProgress(6, null);
                udp.udpSend("255.255.255.255", "6002", "search all");

                //检查 如果ip数目还没变就退出 进行下一步操作
                int masterCount = masterHs.Count;
                ToolsUtil.DelayMilli(500);
                backgroundWorker1.ReportProgress(10, null);
                while (masterCount != masterHs.Count)
                {
                    masterCount = masterHs.Count;
                    ToolsUtil.DelayMilli(500);
                    backgroundWorker1.ReportProgress(14, null);
                }
                backgroundWorker1.ReportProgress(20, 1);//pg = 20
                int pgCount = 20;
                int time = 0;
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                //逐个IP搜索设备
                foreach (string masterIP in masterHs)
                {
                    searchIP = masterIP;
                    isSearchIP = true;
                    time = 0;
                    ReadSerial();
                    while (isSearchIP)
                    {
                        //超时3秒信息不回来 就断开这次
                        ToolsUtil.DelayMilli(500);
                        time = time + 500;
                        if (time >= 4000)
                        {
                            break;
                        }
                    }
                    backgroundWorker1.ReportProgress(pgCount++, null);
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                backgroundWorker1.ReportProgress(100, "搜索设备完成");




            }
            

        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //设置值
            pgv.setValue(e.ProgressPercentage);
            if (e.UserState != null)
            {
                switch (e.UserState.ToString())
                {
                    case "1":
                        //添加节点
                        treeView1.Nodes.Clear();
                        List<string> list = ToolsUtil.IPSort(masterHs);
                        foreach (string masterIP in list)
                        {
                            tm.AddNode1(treeView1, masterIP);
                        }
                        //开启添加设备
                        break;

                    default:
                        TxtShow(e.UserState.ToString());
                        break;
                }
              
            }

        }




        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this.Enabled = true;
                if (e.Cancelled)
                {
                    TxtShow("搜索终止！");
                }
                if (pgv != null)
                {
                    pgv.Close();
                }
                treeView1.ExpandAll();
            }
            catch
            {

            }

        }





        private void SearchMasterRcvDeal(string msg)
        {
            try
            {
                if (msg.Contains("devIP"))
                {
                    //搜索在线的主机处理
                    string[] devInfos = msg.Split(' ');
                    string[] devIP = devInfos[0].Split('=');
                    masterHs.Add(devIP[1]);
                }
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }

        }

   



        /// <summary>
        /// 获取serial设备信息 并处理
        /// </summary>
        private void ReadSerial()
        {

            ClientAsync client = new ClientAsync();
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                string key = string.Empty;
                try
                {
                    if (c.Client.Connected)
                    {
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch (Exception ex)
                {
                    ToolsUtil.WriteLog(ex.Message);
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
            client.Received += new Action<string, string>((key, msg) =>
            {
                if (!String.IsNullOrWhiteSpace(msg))
                {
                    //跨线程调用
                    this.Invoke(getMasterDevDelegate, msg);
                }
            });

            int count = 0;

            //异步连接
            client.ConnectAsync(searchIP, 6001);
            while (!client.Connected())
            {
                ToolsUtil.DelayMilli(200);
                count++;
                if (count == 10)
                {
                    isSearchIP = false;
                    return;
                }
            }

            client.SendAsync("read serial.json$");
        }



        //接收区信息缓存buffer
        string bufferMsg = "";

        /// <summary>
        /// udp信息 回调处理函数
        /// </summary>
        /// <param name="msg"></param>
        private void GetMasterDevRcvDeal(string msg)
        {

            try
            {
                //搜索子设备的信息返回
                bufferMsg = bufferMsg + msg;
                if (msg.Length == 2048)
                {
                    return;
                }
                if (bufferMsg.Contains("serial"))
                {
                    // 序列化接收信息
                    DataJson.Serial tmp = JsonConvert.DeserializeObject<DataJson.Serial>(bufferMsg);
                    if (tmp == null)
                    {
                        isSearchIP = false;
                        return;
                    }

                    FileMesege.serialList = tmp;
                    bufferMsg = string.Empty;
                    isSearchIP = false;
                    string filepath = string.Empty;
                    string device = string.Empty;
           

                    foreach (TreeNode node in treeView1.Nodes)
                    {
                        if (node.Text == searchIP)
                        {
                            int indexs = node.Index;
                            foreach (DataJson.serials sl in FileMesege.serialList.serial)
                            {
                                if (sl.id == 254)
                                {
                                    continue;
                                }
                                filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, sl.serial.Trim());
                                device = IniConfig.GetValue(filepath, "define", "display");
                                if (string.IsNullOrEmpty(device))
                                {
                                    device = sl.serial;
                                    tm.AddNode2(treeView1, Resources.UnrecognizedDev + sl.id + " " + device, indexs);
                                }
                                else
                                {
                                    tm.AddNode2(treeView1, Resources.Device + sl.id + " " + device, indexs);

                                }

                            }
                            break;
                        }
                    }
                    isSearchIP = false;

                }

            }
            catch
            {
                isSearchIP = false;
                //ToolsUtil.WriteLog(ex.Message);
            }


        }


        #endregion


        #region 导入功能

        private string parentIp = "";
        private List<string> addDevList = new List<string>();


        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            lock (FileMesege.resLock)
            {

                long elapsedTicks = DateTime.Now.Ticks - lastTicks;
                TimeSpan span = new TimeSpan(elapsedTicks);
                double diff = span.TotalSeconds;

                if (diff < 1)
                {
                 
                    lastTicks = DateTime.Now.Ticks;
                    return;
                }
                lastTicks = DateTime.Now.Ticks;

                if (treeView1.SelectedNode != null)
                {
                    try
                    {
                        //新建网关
                        Thread thread = new Thread(ThreadRunAddDev);
                        addDevList.Clear();
                        switch (treeView1.SelectedNode.Level)
                        {
                            case 0:
                                //添加子设备
                                parentIp = treeView1.SelectedNode.Text;
                                foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                                {
                                    addDevList.Add(tn.Text);

                                }
                                //TxtShow("添加设备个数：" + addDevList.Count.ToString());
                                thread.Start();
                                break;
                            case 1:
                                parentIp = treeView1.SelectedNode.Parent.Text;
                                addDevList.Add(treeView1.SelectedNode.Text);
                                thread.Start();
                                break;
                            default:
                                TxtShow("请选择需添加设备!");
                                break;
                        }
                     
                    }
                    catch (Exception ex)
                    {
                        ToolsUtil.WriteLog(ex.Message);
                    }
                }
                else
                {
                    TxtShow("添加设备失败!请选择添加设备!");
                }
            }
           
        }


        private void ThreadRunAddDev()
        {
            try
            {
                //检查网关新建
                bool isChange = false;
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                if (checkDevList(parentIp))
                {
                    isChange = true;
                }
                Console.WriteLine("网关：" + parentIp + "数量：" + FileMesege.DeviceList.Count);
                foreach (string info in addDevList)
                {
                    Console.WriteLine("网关：" + parentIp + "数量：" + FileMesege.DeviceList.Count);
                    if (UpdataDev(parentIp, info)) {
                        isChange = true;
                    }
                    
                }
                this.Invoke(updateNodeDelegate);
                if (isChange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }

                Console.WriteLine("更新节点" );
                TxtShow("设备添加成功!");
            }
            catch(Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }
            
        }

        /// <summary>
        /// DevList 添加设备或 更新设备  返回是否修改
        /// </summary>
        /// <param name="parentIp">父节点ID</param>
        /// <param name="ID">子节点ID号</param>
        /// <param name="device">设备名称</param>
        private bool UpdataDev(string parentIp, string info)
        {
            try
            {
                if (string.IsNullOrEmpty(parentIp) || string.IsNullOrEmpty(info))
                {
                    return false;
                }
                //设备节点的ID和名称信息
                string[] infos = info.Split(' ');
                //获取ID号 正则表达式只获取数字
                string ID = Regex.Replace(infos[0], @"[^\d]*", "");
                //获取设备名称 ET-XXXXX .INI
                string device = "";
                string filepath = IniHelper.findDevicesDisplay(infos[1]);
                if (string.IsNullOrEmpty(filepath))
                {
                    device = infos[1];
                }
                else
                {
                    device = Path.GetFileNameWithoutExtension(filepath);
                    if (string.IsNullOrEmpty(device))
                    {
                        device = infos[1];
                    }

                }

                bool isExit = false;
                foreach (DataJson.Device dev in FileMesege.DeviceList)
                {
                    if (dev.ip == parentIp)
                    {
                        //循环该IP地址的所有设备
                        foreach (DataJson.Module md in dev.module)
                        {
                            //存在则修改
                            if (md.id.ToString() == ID)
                            {
                                //修改设备
                                if (md.device != device)
                                {
                                    //删除Point信息
                                    DataListHelper.delPointID(parentIp, ID);

                                }
                                else
                                {
                                    //更改pointID信息
                                    DataListHelper.changePointID(parentIp, ID, ID);

                                }
                                DataListHelper.changeDevice(parentIp, ID, device, ID, md.device);
                                isExit = true;
                                break;

                            }

                        }
                        //不存在该ID
                        if (!isExit)
                        {
                            //新建设备
                            DataListHelper.newDevice(parentIp, ID, device);
                     
                            return true;
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
                return false;
            }
            

            
        }

     

        /// <summary>
        /// 新建网关 并判断该IP网关是否存在 
        /// </summary>
        /// <param name="ip"></param>
        private bool checkDevList(string parentIp)
        {
            try
            {
                bool isExit = false;
                if (FileMesege.DeviceList != null)
                {
                    foreach (DataJson.Device dev in FileMesege.DeviceList)
                    {
                        if (dev.ip == parentIp)
                        {
                            isExit = true;
                        }
                    }
                }
                if (!isExit)
                {
                    DataListHelper.newGateway(parentIp, "GW100A");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
                return false;
            }
           
        }


        #endregion


        #region 窗体关闭  树状图操作
        /// <summary>
        /// 双击控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
              
                if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
                {
                    btnImport_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }
        }




        /// <summary>
        /// 高亮显示选中项 重绘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {

            Color foreColor;
            Color backColor;
            if ((e.State & TreeNodeStates.Selected) > 0)
            {
                foreColor = Color.Black;//鼠标点击节点时文字颜色
                backColor = Color.FromArgb(204, 235, 248);//鼠标点击节点时背景颜色
            }
            else if ((e.State & TreeNodeStates.Hot) > 0)
            {
                foreColor = Color.Lime;//鼠标经过时文字颜色
                backColor = Color.Gray;//鼠标经过时背景颜色
            }
            else
            {
                foreColor = this.treeView1.ForeColor;
                backColor = this.treeView1.BackColor;
            }
            //e.Graphics.FillRectangle(new SolidBrush(backColor), new Rectangle(e.Bounds.Location, new Size(this.treeView1.Width - e.Bounds.X, e.Bounds.Height)));
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            e.Graphics.DrawString(e.Node.Text, this.treeView1.Font, new SolidBrush(foreColor), e.Bounds.X, e.Bounds.Y + 4);
        }


        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineSearch_FormClosing(object sender, FormClosingEventArgs e)
        {

            //关闭窗体自动停止UDP
            if (udp != null)
            {
                udp.udpClose();
            }
            treeView1.SelectedNode = null;
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

        private void OnlineSearch_Paint(object sender, PaintEventArgs e)
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










    }
}
