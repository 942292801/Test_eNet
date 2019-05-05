using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using eNet编辑器.Properties;

namespace eNet编辑器.AddForm
{
   
    public partial class OnlineSearch : Form
    {
        //UDP客户端
        UdpSocket udp;
        //网关节点 选中的IP地址
        string selectIP = "";
        TreeMesege tm ;
        public OnlineSearch()
        {
            InitializeComponent();
        }
 

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> TxtShow;

        private delegate void ReceviceDelegate(string msg);
        private event ReceviceDelegate receviceDelegate;
       
        private void OnlineSearch_Load(object sender, EventArgs e)
        {
            udpIni();
            tm = new TreeMesege();
            receviceDelegate = new ReceviceDelegate(receviceDelegateMsg);
           

        }

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
                        this.Invoke(receviceDelegate, msg);
                    }


                }
                catch
                {
                    //报错不处理
                }
            });
        }

        /// <summary>
        /// udp信息 回调处理函数
        /// </summary>
        /// <param name="msg"></param>
        private void receviceDelegateMsg(string msg) 
        {

            try
            {
                if (msg.Contains("serial"))
                {
                    // 序列化接收信息
                    FileMesege.serialList = JsonConvert.DeserializeObject<DataJson.Serial>(msg);


                    //当前选中节点和IP相等时  
                    if (treeView1.SelectedNode.Text == selectIP)
                    {
                        //选中索引
                        int indexs = treeView1.SelectedNode.Index;
                        foreach (DataJson.serials sl in FileMesege.serialList.serial)
                        {
                            tm.AddNode2(treeView1, Resources.Device + sl.id + " " + sl.serial, indexs);

                        }
                        //展开所有节点
                        treeView1.ExpandAll();

                    }


                }
                else if (msg.Contains("devIP"))
                {

                    //网关加载
                    string[] devInfos = msg.Split(' ');
                    //devIP = 0.0.0.0
                    string[] devIP = devInfos[0].Split('=');
                    bool isExeit = false;
                    for (int i = 0; i < treeView1.Nodes.Count; i++)
                    {
                        if (treeView1.Nodes[i].Text == devIP[1])
                        {
                            isExeit = true;
                        }
                    }
                    if (!isExeit)
                    {
                        //添加网关
                        tm.AddNode1(treeView1, devIP[1]);
                    }

                }
            }
            catch { }
           

        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                try
                {
                    string parentIp = "";
                    switch (treeView1.SelectedNode.Level)
                    {
                        case 0:
                            //父节点的IP地址
                            parentIp = treeView1.SelectedNode.Text;
                            //新建网关
                            checkDevList(parentIp);
                            foreach (TreeNode tn in treeView1.SelectedNode.Nodes)
                            {
                                UpdataDev(parentIp, tn);

                            }     
                            TxtShow("添加设备成功!");
                            break;
                        case 1:
                            //父节点的IP地址
                            parentIp = treeView1.SelectedNode.Parent.Text;
                            checkDevList(parentIp);
                            //DevList 添加设备或 更新设备
                            UpdataDev(parentIp, treeView1.SelectedNode);
                  
                            TxtShow("添加设备成功!");
                            break;
                        default:
                            TxtShow("添加设备失败!请选择添加设备!");
                            break;
                    }
                }
                catch { }
            }
            else
            {
                TxtShow("添加设备失败!请选择添加设备!");
            }
        }

        /// <summary>
        /// DevList 添加设备或 更新设备
        /// </summary>
        /// <param name="parentIp">父节点ID</param>
        /// <param name="ID">子节点ID号</param>
        /// <param name="device">设备名称</param>
        private void UpdataDev(string parentIp, TreeNode tn)
        {
            if (parentIp == "" || tn == null)
            {
                return;
            }
            //设备节点的ID和名称信息
            string[] infos = tn.Text.Split(' ');
            //获取ID号 正则表达式只获取数字
            string ID = Regex.Replace(infos[0], @"[^\d]*", "");
            //设备型号
            string device = infos[1];
            bool isExit = false;
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == parentIp)
                {
                    //循环该IP地址的所有设备
                    foreach (DataJson.Module md in dev.module)
                    {
                        //存在则修改
                        if (md.id == ID)
                        {
                            //修改网关
                            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
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
                            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                            FileMesege.cmds.DoNewCommand(NewList, OldList);
                            isExit = true;
                            break;
                            
                        }

                    }
                    //不存在该ID
                    if (!isExit)
                    {
                        //新建设备
                        DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                        DataListHelper.newDevice(parentIp,ID,device);
                        DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                        FileMesege.cmds.DoNewCommand(NewList, OldList);
                        break;
                    }

                }
            }

            
        }

     

        /// <summary>
        /// 新建网关 并判断该IP网关是否存在 
        /// </summary>
        /// <param name="ip"></param>
        private void checkDevList(string parentIp)
        {
            bool b = Regex.IsMatch(parentIp, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
            if (!b)
            {
                return;
            }
            bool isExit = false;
            if (FileMesege.DeviceList == null)
            {
                FileMesege.DeviceList = new List<DataJson.Device>();
            }
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == parentIp)
                {
                    isExit = true;
                }
            }

            if (!isExit)
            {
                DataListHelper.newGateway(parentIp, "GW100A");
               
            }
        }
        //本地IP
        string Localip = "";
        /// <summary>
        /// 搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //每次搜索清空树状图
            treeView1.Nodes.Clear();
            
            
           
            udp.udpClose();
            udpIni();
            //获取本地IP
            Localip = SocketUtil.GetLocalIP();
            //udp 绑定
            udp.udpBing(Localip, "6004");              
            
            //绑定成功
            if (udp.isbing)
            {
                udp.udpSend("255.255.255.255","6002","search all");
                string[]tmps = Localip.Split('.');
                string broadcastIp = String.Format("{0}.{1}.{2}.255", tmps[0], tmps[1], tmps[2]);
                udp.udpSend(broadcastIp, "6002", "Search all");
                
            }
            TxtShow("搜索主机结束！");

        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlineSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭窗体自动停止UDP
            udp.udpClose();
            treeView1.SelectedNode = null;
        }



        /// <summary>
        /// 刷新按钮  获取serial设备信息 并处理
        /// </summary>
        private void selectHandle()
        {
            ClientAsync client = new ClientAsync();
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
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

                        break;
                    default:
                        break;
                }
            });
            //信息接收处理
            client.Received += new Action<string, string>((key, msg) =>
            {
                if (msg.Contains("serial"))
                {
                    if (!String.IsNullOrWhiteSpace(msg))
                    {
                        //跨线程调用
                        this.Invoke(receviceDelegate, msg);
                    }

                }//if msg 

            });

            //异步连接
            client.ConnectAsync(selectIP, 6001);
            client.SendAsync("read serial.json$");
        }

        /// <summary>
        /// 双击控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectIP = "";
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 0)
            {
                //清空当前选中节点
                treeView1.SelectedNode.Nodes.Clear();
                selectIP = treeView1.SelectedNode.Text;
                if (selectIP != "")
                {
                    selectHandle();
                }

            }
            else if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
            {
                btnImport_Click(this, EventArgs.Empty);
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

 

      
            
        
       

    }
}
