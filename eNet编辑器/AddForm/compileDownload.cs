using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace eNet编辑器.AddForm
{
    public partial class compileDownload : Form
    {
        public compileDownload()
        {
            InitializeComponent();
        }

        public event Action<string> AppTxtShow;

        private void compileDownload_Load(object sender, EventArgs e)
        {
            
            if (FileMesege.DeviceList == null)
            {
                return;
            }
 
                //从设备加载网关信息
            foreach (DataJson.Device d in FileMesege.DeviceList)
            {
                cbIP.Items.Add(d.ip);
            }
            if (cbIP.Items.Count > 0)
            {
                cbIP.SelectedIndex = 0;
            }

            showIp();
        }

        private void showPgBar()
        {
            pgBar.Value = 0;
            pgBar.Visible = true;
            lbTip.Visible = true;
            lbName.Visible = false;
            cbIP.Visible = false;
            btnClose.Enabled = false;
            btnSend.Enabled = false;
            timer1.Stop();
        }

        private void showIp()
        {
            pgBar.Value = 0;
            pgBar.Visible = false;
            lbTip.Visible = false;
            lbName.Visible = true;
            cbIP.Visible = true;
            btnClose.Enabled = true;
            btnSend.Enabled = true;
            timer1.Stop();
        }

        #region 重绘
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


        /// <summary>
        /// 已经隐藏该按钮功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompile_Click(object sender, EventArgs e)
        {
            //编译
            compile(cbIP.Text);
        }

        //下载到主机
        private void btnSend_Click(object sender, EventArgs e)
        {
            showPgBar();
            //编译s.json k.json t.json 并压缩
            if (!compile(cbIP.Text))
            {
                showIp();
                return;
            }
            //
            if (!sendBackUp(cbIP.Text))
            {
                showIp();
                return;
            }
            download(cbIP.Text);
            
        }

        /// <summary>
        /// //编译s.json k.json t.json 并压缩
        /// </summary>
        /// <param name="ip"></param>
        private bool compile(string ip)
        {
         
            for (int i = 0; i < cbIP.Items.Count; i++)
            {
                if (cbIP.Items[i].ToString() == ip)
                {
                    //存在该Ip的信息 可以进行编译
                    FileMesege fm = new FileMesege();
                    if (fm.ObjDirClearByIP(cbIP.Text))
                    {
                        AppTxtShow(string.Format("({0})工程文件初始化成功！", ip));
                        pgBar.Value += 4;
                    }
                    else
                    {
                        AppTxtShow(string.Format("({0})工程文件初始化失败！", ip));
                        return false;
                    }
                    
                  
                    
                    //编译场景
                    if (fm.getSceneJsonByIP(ip))
                    {
                        AppTxtShow("scene.json编译通过！");
                        pgBar.Value += 2;
                    }
                    else
                    {
                        AppTxtShow("scene.json编译失败！");

                    }
                    //编译定时
                    if (fm.getTimerJsonByIP(ip))
                    {
                        AppTxtShow("timer.json编译通过！");
                        pgBar.Value += 2;
                    }
                    else
                    {
                        AppTxtShow("timer.json编译失败！");

                    }
                    //编译面板
                    if (fm.getPanelJsonByIP(ip))
                    {
                        AppTxtShow("panel.json编译通过！");
                        pgBar.Value += 2;
                    }
                    else
                    {
                        AppTxtShow("panel.json编译失败！");

                    }
                    //编译感应
                    if (fm.getSensorJsonByIP(ip))
                    {
                        AppTxtShow("sensor.json编译通过！");
                        pgBar.Value += 2;
                    }
                    else
                    {
                        AppTxtShow("sensor.json编译失败！");

                    }
                    ZipHelper zipHelp = new ZipHelper();
                    string file = string.Format("{0}\\objs\\{1}", FileMesege.TmpFilePath, ip);
                    try
                    {
                        string msg;
                        //压缩到选中路径
                        zipHelp.ZipFolder(file, string.Format("{0}.zip", file),out msg);
                        //AppTxtShow(string.Format("({0})工程文件编译成功！", ip));
                        pgBar.Value += 2;
                        return true;
                    }
                    catch
                    {
                        //AppTxtShow(string.Format("({0})工程文件编译失败！", ip));

                        return false;
                    }
                    
                }// if(ip)
            }//for
            return false;
            
        }


        /// <summary>
        /// 编译并下载 point.json area.json device.json 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool sendBackUp(string ip)
        {
            try
            {
              
                FileMesege fm = new FileMesege();
                
                //抽离point 信息
                string point  = fm.getPointJsonByIP(ip);
                if (!string.IsNullOrEmpty(point))
                {
                    AppTxtShow("point.json编译通过！");
                    pgBar.Value += 2;
                }
                else
                {
                    AppTxtShow("point.json编译失败！");
                    return false;
                }

                //获取area 信息
                string area = fm.getAreaJsonByIP(ip);
                if (!string.IsNullOrEmpty(area))
                {
                    AppTxtShow("area.json编译通过！");
                    pgBar.Value += 2;
                }
                else
                {
                    AppTxtShow("area.json编译失败！");
                    return false;
                }

                //获取device 信息
                string device = fm.getDeviceJsonByIP(ip);
                if (!string.IsNullOrEmpty(device))
                {
                    AppTxtShow("device.json编译通过！");
                    pgBar.Value += 2;
                }
                else
                {
                    AppTxtShow("device.json编译失败！");
                    return false;
                }

                if (sendData(ip, point, "point.json"))
                {
                    AppTxtShow("point.json载入完成！");
                    pgBar.Value += 4;
                 
                }
                else
                {
                    AppTxtShow("point.json载入失败！");
                    return false;
                }

                if (sendData(ip, area, "area.json"))
                {
                    AppTxtShow("area.json载入完成！");
                    pgBar.Value += 4;

                }
                else
                {
                    AppTxtShow("area.json载入失败！");
                    return false;
                }

                if (sendData(ip, device, "device.json"))
                {
                    AppTxtShow("device.json载入完成！");
                    pgBar.Value += 4;

                }
                else
                {
                    AppTxtShow("device.json载入失败！");
                    return false;
                }

                return true;
            }
            catch 
            {
                return false;
            }

          
        }

        /// <summary>
        /// 把ip.zip压缩包下载到主机里面
        /// </summary>
        /// <param name="ip"></param>
        private void download(string ip)
        { 
            //连接网络 发送当前IP的压缩包到里面
            Socket sock = null;
            try
            {

                //写入数据格式
                string data = "down /enet.prj$";
                string filepath = string.Format("{0}\\objs\\{1}.zip",FileMesege.TmpFilePath,ip);
                TcpSocket ts = new TcpSocket();

                sock = ts.ConnectServer(ip, 6001, 1);
                SocketUtil.Delay(1);
                if (sock == null)
                {
                    
                    AppTxtShow("文件载入主机失败");
                    showIp();
                    return;
                }


                int flag = -1;

                //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                flag = ts.SendData(sock, data, 1);
                if (flag == 0)
                {
                    flag = ts.SendFile(sock, filepath);
                    
                    if (flag == 0)
                    {
                        AppTxtShow("文件载入主机成功！请等待主机重启程序！");
                        timer1.Start();
                    }
                    else
                    {
                         AppTxtShow("文件载入主机失败");
                         showIp();
                    }

                }
                else
                {
                    AppTxtShow("文件载入主机失败");
                    showIp();
                }

                if (sock != null)
                {

                    sock.Dispose();
                }
                
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                showIp();
            }
        }//private


        /// <summary>
        /// 发送 文件到backup下面
        /// </summary>
        /// <param name="ip">ip网关</param>
        /// <param name="json">工程信息</param>
        /// <param name="name">写进backup的文件名</param>
        private bool sendData(string ip, string json, string fileName)
        {
            //连接网络 发送当前IP的压缩包到里面
            Socket sock = null;
            try
            {
                //写入数据格式
                string data = string.Format("down /backup/{0}${1}",fileName,json);
                TcpSocket ts = new TcpSocket();
                sock = ts.ConnectServer(ip, 6001, 1);
                SocketUtil.Delay(1);
                if (sock == null)
                {
                    AppTxtShow("连接主机失败，请检查网络！");
                    return false;
                }
                int flag = 0;
                //0:发送数据成功；-1:超时；-2:发送数据出现错误；-3:发送数据时出现异常
                flag = ts.SendData(sock, data, 1);
                if (sock != null)
                {
                  
                    sock.Dispose();
                }
                if (flag != 0)
                {

                    return false;
                }
                return true;

                
            }
            catch(Exception e )
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pgBar.Value += 1;
            if (pgBar.Value > 98)
            {
                showIp();
            }
        }



    }//class
}
