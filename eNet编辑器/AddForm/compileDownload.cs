using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using Ionic.Zip;
using eNet编辑器.OtherView;
using Newtonsoft.Json;

namespace eNet编辑器.AddForm
{
    public partial class compileDownload : Form
    {
        public compileDownload()
        {
            InitializeComponent();
        }

        public event Action<string> AppTxtShow;

        private string sourceIP;
        private string targetIP;
        private BackgroundWorker backgroundWorker1;
        private PgView pgv;


        //UDP客户端
        UdpSocket udp;
        //本地IP
        string Localip = "";
        private event Action<string> udpreceviceDelegate;

        private void compileDownload_Load(object sender, EventArgs e)
        {
            udpreceviceDelegate += new Action<string>(udpReceviceDelegateMsg);
            findOnlineGW();
            findPrGW();

        }

        #region 获取工程中的IP地址
        private void findPrGW()
        {
            if (FileMesege.DeviceList == null)
            {
                return;
            }

            //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                cbSourceIP.Items.Add(d.ip);
            }
            if (cbSourceIP.Items.Count > 0)
            {
                cbSourceIP.SelectedIndex = 0;
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
                udp.udpSend("255.255.255.255", "6002", "Search all");
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
                    for (int i = 0; i < cbTargetIp.Items.Count; i++)
                    {
                        if (cbTargetIp.Items[i].ToString() == devIP[1])
                        {
                            //确定item里面没有 该ip项就添加
                            isExeit = true;
                        }
                    }
                    if (!isExeit)
                    {
                        cbTargetIp.Items.Add(devIP[1]);

                    }




                }

            }
            catch { }


        }



        #endregion

    


        #region 编译和下载
        //下载到主机
        private void btnSend_Click(object sender, EventArgs e)
        {
            
            try
            {

                if (string.IsNullOrEmpty(cbSourceIP.Text))
                {
                    AppTxtShow("请选取网关工程文件！");
                    return;
                }
                if ( string.IsNullOrEmpty(cbTargetIp.Text))
                {
                    AppTxtShow("请选取需下载网关！");
                    return;
                }
                sourceIP = cbSourceIP.Text;
                targetIP = cbTargetIp.Text;

                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.WorkerSupportsCancellation = true;
                backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
                backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;
                pgv = new PgView();
                pgv.setMaxValue(100);
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
                
                ToolsUtil.WriteLog(ex.Message);
            }

        }

        //运行工作
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                CompileDown(e);
                //DownZIP2Master(e);
            }
            catch (Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //设置值
            pgv.setValue(e.ProgressPercentage);
            if (e.UserState != null)
            {
                AppTxtShow(e.UserState.ToString());
            }

        }


        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    AppTxtShow(string.Format("({0})下载终止！", cbSourceIP.Text));
                }

                this.Enabled = true;
                if (pgv != null)
                {
                    pgv.Close();
                }

            }
            catch(Exception ex)
            {
                ToolsUtil.WriteLog(ex.Message);
            }

        }

        /// <summary>
        /// 编译和下载
        /// </summary>
        private void CompileDown(DoWorkEventArgs e)
        {
            //存在该Ip的信息 可以进行编译
            FileMesege fm = new FileMesege();
            if (fm.ObjDirClearByIP(targetIP))
            {
                backgroundWorker1.ReportProgress(5, string.Format("({0})工程文件夹创建成功！", targetIP));
            }
            else
            {
                backgroundWorker1.ReportProgress(100, string.Format("({0})工程文件夹创建失败！", targetIP));
                return ;
            }

            ToolsUtil.DelayMilli(2000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            backgroundWorker1.ReportProgress(6, null);
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            backgroundWorker1.ReportProgress(7, null);
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            backgroundWorker1.ReportProgress(8, null);
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            backgroundWorker1.ReportProgress(9, null);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //编译场景
            if (fm.getSceneJsonByIP(sourceIP, targetIP))
            {
           
                backgroundWorker1.ReportProgress(10, "场景文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "场景文件编译失败！");
                return ;

            }
            ToolsUtil.DelayMilli(800);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //编译定时
            if (fm.getTimerJsonByIP(sourceIP, targetIP))
            {
                backgroundWorker1.ReportProgress(15, "定时文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "定时文件编译失败！");
                return ;

            }
            ToolsUtil.DelayMilli(800);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            //编译面板
            if (fm.getPanelJsonByIP(sourceIP, targetIP))
            {
                backgroundWorker1.ReportProgress(20, "面板文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "面板文件编译失败！");
                return ;

            }
            ToolsUtil.DelayMilli(800);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            //编译感应
            if (fm.getSensorJsonByIP(sourceIP, targetIP))
            {
                backgroundWorker1.ReportProgress(25, "感应编组文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "感应编组文件编译失败！");
                return ;

            }
            ToolsUtil.DelayMilli(800);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            //编译逻辑
            if (fm.getLogicJsonByIp(sourceIP, targetIP))
            {
                backgroundWorker1.ReportProgress(30, "逻辑文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "逻辑文件编译失败！");
                return ;

            }
            ToolsUtil.DelayMilli(800);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }


            #region 获取point area device scene timer panel sensor logic文件 放入Backup
            //抽离point 信息
            string point = fm.BackupPointJsonByIP(sourceIP,targetIP);
            if (!string.IsNullOrEmpty(point))
            {
                backgroundWorker1.ReportProgress(33, "Backup点位文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup点位文件编译失败！");
                return ;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            //获取area 信息
            string area = fm.BackupAreaJsonByIP();
            if (!string.IsNullOrEmpty(area))
            {
                backgroundWorker1.ReportProgress(36, "Backup区域文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup区域文件编译失败！");
                return ;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            //获取device 信息
            string device = fm.BackupDeviceJsonByIP(sourceIP, targetIP);
            if (!string.IsNullOrEmpty(device))
            {
                backgroundWorker1.ReportProgress(39, "Backup设备列表文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup设备列表文件编译失败！");
                return ;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //获取scene 信息
            string scene = fm.BackupSceneJsonByIP(sourceIP, targetIP);
            if (!string.IsNullOrEmpty(scene))
            {
                backgroundWorker1.ReportProgress(42, "Backup场景文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup场景文件编译失败！");
                return;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //获取timer 信息
            string timer = fm.BackupTimerJsonByIP(sourceIP, targetIP);
            if (!string.IsNullOrEmpty(timer))
            {
                backgroundWorker1.ReportProgress(45, "Backup定时文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup定时文件编译失败！");
                return;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //获取panel 信息
            string panel = fm.BackupPanelJsonByIP(sourceIP, targetIP);
            if (!string.IsNullOrEmpty(panel))
            {
                backgroundWorker1.ReportProgress(46, "Backup面板文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup面板文件编译失败！");
                return;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //获取sensor信息
            string sensor = fm.BackupSensorJsonByIP(sourceIP, targetIP);
            if (!string.IsNullOrEmpty(sensor))
            {
                backgroundWorker1.ReportProgress(47, "Backup感应编组文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup感应编组文件编译失败！");
                return;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            //获取logic信息
            string logic = fm.BackupLogicJsonByIP(sourceIP, targetIP);
            if (!string.IsNullOrEmpty(logic))
            {
                backgroundWorker1.ReportProgress(48, "Backup逻辑文件编译通过！");
            }
            else
            {
                backgroundWorker1.ReportProgress(100, "Backup逻辑文件编译失败！");
                return;
            }
            ToolsUtil.DelayMilli(1000);
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            #endregion

            //建立压缩包
            string file = string.Format("{0}\\objs\\{1}", FileMesege.TmpFilePath, targetIP);
            try
            {

                //使用2.0版本Ionic.Zip压缩文件
                using (ZipFile zip = new ZipFile(string.Format("{0}.zip", file), Encoding.Default))
                {
                     
                    //将要压缩的文件夹添加到zip对象中去(要压缩的文件夹路径和名称)
                    zip.AddDirectory(file);
                    //将要压缩的文件添加到zip对象中去,如果文件不存在抛错FileNotFoundExcept
                    zip.Save();
                }
                backgroundWorker1.ReportProgress(50, null);
                ToolsUtil.DelayMilli(2000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, point, "point.json"))
                {
                    backgroundWorker1.ReportProgress(60, "Backup点位文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\point.json", file),point);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup点位文件载入失败！");
                    return ;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, area, "area.json"))
                {
                    backgroundWorker1.ReportProgress(63, "Backup区域文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\area.json", file), area);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup区域文件载入失败！");
                    return ;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, device, "device.json"))
                {
                    backgroundWorker1.ReportProgress(66, "Backup设备列表文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\device.json", file), device);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup设备列表文件载入失败！");
                    return ;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, scene, "scene.json"))
                {
                    backgroundWorker1.ReportProgress(69, "Backup场景文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\scene.json", file), scene);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup场景文件载入失败！");
                    return;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, timer, "timer.json"))
                {
                    backgroundWorker1.ReportProgress(72, "Backup定时文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\timer.json", file), timer);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup定时文件载入失败！");
                    return;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }


                if (SendFile2Backup(targetIP, panel, "panel.json"))
                {
                    backgroundWorker1.ReportProgress(73, "Backup面板文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\panel.json", file), panel);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup面板文件载入失败！");
                    return;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, sensor, "sensor.json"))
                {
                    backgroundWorker1.ReportProgress(76, "Backup感应编组文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\sensor.json", file), sensor);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup感应编组文件载入失败！");
                    return;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (SendFile2Backup(targetIP, logic, "logic.json"))
                {
                    backgroundWorker1.ReportProgress(80, "Backup逻辑文件载入完成！");
                    //临时添加
                    File.WriteAllText(string.Format("{0}\\logic.json", file), logic);
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Backup逻辑文件载入失败！");
                    return;
                }
                ToolsUtil.DelayMilli(1000);
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                DownZIP2Master(e);
            }
            catch(Exception ex)
            {
              
                backgroundWorker1.ReportProgress(100, ex.Message);
                ToolsUtil.WriteLog(ex.Message);
            }



        }



        /// <summary>
        /// 把ip.zip压缩包下载到主机里面
        /// </summary>
        /// <param name="ip"></param>
        private void DownZIP2Master(DoWorkEventArgs e)
        { 
            
            try
            {
                //连接网络 发送当前IP的压缩包到里面
                Socket sock = null;
                TimeOutHelper timeOutHelper = new TimeOutHelper();
                int count = 0;

                //写入数据格式
                string data = "down /enet.prj$";
                string filepath = string.Format("{0}\\objs\\{1}.zip", FileMesege.TmpFilePath, targetIP);

                TcpSocket ts = new TcpSocket();

                sock = ts.ConnectServer(targetIP, 6001, 2000);
                while (true)
                {
                    if (timeOutHelper.IsTimeout())
                    {
                        count++;
                        if (count == 2)
                        {
                            //连接2次超时 退出操作
                            backgroundWorker1.ReportProgress(100, string.Format("({0})工程写入失败！", targetIP));
                            return;
                        }
                        timeOutHelper = new TimeOutHelper();
                        sock = ts.ConnectServer(targetIP, 6001, 2000);
                    }
                    ToolsUtil.DelayMilli(100);
                    if (sock != null)
                    {
                        //连接成功
                        break;
                    }

                }

                int flag = 2;

                //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                flag = ts.SendData(sock, data, 1000);
                if (flag == 0)
                {
                    flag = ts.SendFile(sock, filepath);

                    if (flag == 0)
                    {
                        
                        backgroundWorker1.ReportProgress(82, string.Format("({0})工程写入成功，网关正在重启！", targetIP));
                        for (int i = 1; i < 19; i++)
                        {
                            ToolsUtil.DelayMilli(1000);
                            if (backgroundWorker1.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            backgroundWorker1.ReportProgress(82+i, null);
                        }
                        return;
                    }        

                }
                backgroundWorker1.ReportProgress(100, string.Format("({0})工程写入失败！", targetIP));
            
                if (sock != null)
                {

                    sock.Dispose();
                }

            }
            catch(Exception ex)
            {
                backgroundWorker1.ReportProgress(100, string.Format("({0})工程写入失败！", targetIP));
                ToolsUtil.WriteLog(ex.Message);
            }

        }//private


        /// <summary>
        /// 发送 文件到backup下面
        /// </summary>
        /// <param name="ip">ip网关</param>
        /// <param name="json">工程信息</param>
        /// <param name="name">写进backup的文件名</param>
        private bool SendFile2Backup(string ip, string json, string fileName)
        {
           
            try
            {
                
                //连接网络 发送当前IP的压缩包到里面
                Socket sock = null;
                TimeOutHelper timeOutHelper = new TimeOutHelper();
                int count = 0;
                //写入数据格式
                string data = string.Format("down /backup/{0}${1}",fileName,json);
                TcpSocket ts = new TcpSocket();
         
                sock = ts.ConnectServer(ip, 6001, 2000);
                while (true)
                {
                    ToolsUtil.DelayMilli(100);
                    if (timeOutHelper.IsTimeout())
                    {
                        count++;
                        if (count == 2)
                        {
                     
                            //连接2次超时 退出操作
                            Console.WriteLine("连接2次超时 退出操作");
                            return false;
                        }
                        timeOutHelper = new TimeOutHelper();
                        sock = ts.ConnectServer(ip, 6001, 2000);
                    }
                    if (sock != null)
                    {
                        //连接成功
                        //Console.WriteLine("连接成功");
                        break;
                    }
                    
                }

                int flag = 2;
                //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                //Console.WriteLine("发送");
                flag = ts.SendData(sock, data, 2000);
                timeOutHelper = new TimeOutHelper();
                while (true)
                {
                    //Console.WriteLine("等待发送结果");
                    ToolsUtil.DelayMilli(100);
                    if (timeOutHelper.IsTimeout() )
                    {
                        break;
                    }
                    if (flag != 2)
                    {
                        break;
                    }
                }

                if (sock != null)
                {
                    sock.Dispose();
                }
                if (flag == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                
            }
            catch//(Exception e )
            {
                return false;
            }
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
        private void compileDownload_Paint(object sender, PaintEventArgs e)
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


    }//class
}
