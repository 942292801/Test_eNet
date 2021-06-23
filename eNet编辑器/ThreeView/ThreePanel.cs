using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.Properties;
using eNet编辑器.AddForm;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using eNet编辑器.Tools;

namespace eNet编辑器.ThreeView
{
    public delegate void DgvPanelAddItem(object page);
    public partial class ThreePanel : Form
    {

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        public event Action updatePanelView;
        public event DgvPanelAddItem dgvpanelAddItem;
        public event Action addTitleNode;

        private event Action<string> TCP6003Delegate;
        public ThreePanel()
        {
            InitializeComponent();
      
     
        }



        private panelAdd pladd;
        string ip = "";

        //判断true为选中父节点
        bool isSelectParetnNode = false;

        //树状图节点
        string fullpath = "";
        //搜索区域
        SearchSection searchSection = new SearchSection();

        //客户端
        ClientAsync client;

        private void ThreePanel_Load(object sender, EventArgs e)
        {
            TCP6003Delegate += ThreePanel_TCP6003Delegate;
            pladd = new panelAdd();
            pladd.addPanelNode += new Action(addPanelNode);
        }

       

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreePanelAddNode()
        {
            try
            {
              
                if (FileMesege.DeviceList == null)
                {
                    treeView1.Nodes.Clear();
                    return;
                }
                TreeMesege tm = new TreeMesege();
                //记录当前节点展开状况 
                List<string> isExpands = tm.treeIsExpandsState(treeView1);

                string section = "";
                //从设备加载网关信息
                foreach (DataJson.Device d in FileMesege.DeviceList)
                {
                    int index = tm.AddNode1(treeView1, d.ip + " " + d.master);

                    if (FileMesege.panelList != null)
                    {
                        foreach (DataJson.Panel pl in FileMesege.panelList)
                        {
                            //  添加该网关IP的子节点
                            if (pl.IP == d.ip)
                            {
                                foreach (DataJson.panels pls in pl.panels)
                                {
                                    DataJson.PointInfo point = DataListHelper.findPointByPid(pls.pid, FileMesege.PointList.link);
                                    if (point != null)
                                    {
                                        section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Panel, pls.id, section, point.name), index);

                                    }

                                }

                            }
                        }
                    }

                }
                //展开记录的节点
                tm.treeIspandsStateRcv(treeView1, isExpands);
                TreeMesege.SetPrevVisitNode(treeView1, fullpath);
             
            }
            catch
            {
                //错误处理
                MessageBox.Show("场景添加节点初始化失败,请检查timer.json文件");
            }
        }

        /// <summary>
        /// 根据point点信息 定位到树节点 用于窗口跳转
        /// </summary>
        /// <param name="point"></param>
        public void FindNodeSelect(DataJson.PointInfo point)
        {
            TreeMesege.SelectNodeByPoint(treeView1, point);
        }

       

        #region 节点点击 点击后事件 树状图重绘
        /// <summary>
        /// 选中节点  选择显示的DGV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
            if (FileMesege.panelSelectNode == null)
            {
                addTitleNode();
            }
            else
            {
                if (ToolsUtil.getIP(treeView1.SelectedNode) != ToolsUtil.getIP(FileMesege.panelSelectNode))
                {
                    addTitleNode();

                }
            }
            
            TreeNodeIni();
            if (FileMesege.isDgvNameDeviceConnet)
            {
                if (client == null || !client.Connected())
                {
                    clientConnect();
                    timer1.Start();
                }
            }
            FileMesege.panelSelectNode = treeView1.SelectedNode;
            fullpath = treeView1.SelectedNode.FullPath;
            //刷新dgv表格
            dgvpanelAddItem(0);
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {
                
                clearTxtShow(Resources.TxtShowPanelName + treeView1.SelectedNode.Text );
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1] + ".ini";
                clearTxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note"));
            }
        }

        

        /// <summary>
        /// 鼠标右击选中节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                isSelectParetnNode = false;
                if (CurrentNode != null)
                {
                    treeView1.SelectedNode = CurrentNode;//选中这个节点
                    if (CurrentNode.Parent != null)
                    {
                        //右击选择为子节点 显示菜单2
                        CurrentNode.ContextMenuStrip = contextMenuStrip2;
                    }
                    else
                    {

                        //右击选择为父节点 显示示菜单1
                        CurrentNode.ContextMenuStrip = contextMenuStrip1;
                        isSelectParetnNode = true;
                    }
                }
            }
        }

        //选中字体高亮
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


        #endregion


        #region 新建 修改 删除 复制
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //右击树状图外面区域
            if (!isSelectParetnNode)
            {

            }
            else//右击树状图区域
            {
                //清除复制的定时
                copyPanel = null;
                //新建场景
                newPanel();

            }
        }

        /// <summary>
        /// 面板新建 修改弹框
        /// </summary>
        private void newPanel()
        {
            //展示居中
            pladd.StartPosition = FormStartPosition.CenterParent;
            pladd.Xflag = false;
            string[] ips;
            if (treeView1.SelectedNode.Parent == null)
            {
                ips = treeView1.SelectedNode.Text.Split(' ');
            }
            else
            {
                ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            }
            pladd.Ip = ips[0];
            //获取定面板数
            pladd.Num = (treeView1.SelectedNode.GetNodeCount(false) + 101).ToString();
            pladd.ShowDialog();
        }

   

        /// <summary>
        /// 新建面板回调操作
        /// </summary>
        private void addPanelNode()
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent == null)
            {
                treeView1.SelectedNode.Expand();

            }
            //获取面板号
            string num = Convert.ToInt32(pladd.Num).ToString("X4");
            //获取IP最后一位 定时为20
            string address ="FE30" + num;
            if (FileMesege.panelList == null)
            {
                FileMesege.panelList = new List<DataJson.Panel>();

            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加timerList 表中
            //该IP在timerList里面是否存在  
            if (!FileMesege.panelList.Exists(x => x.IP == pladd.Ip))
            {
                //不存在新建List
                DataJson.Panel pl = new DataJson.Panel();
                pl.IP = pladd.Ip;
                pl.Dev = "GW100A";
                pl.panels = new List<DataJson.panels>();
                FileMesege.panelList.Add(pl);

            }

            //判断是否已经存在该点位信息
            int randomNum = DataChange.randomNum();

            foreach (DataJson.Panel pl in FileMesege.panelList)
            {
                //节点的IP相等 进入创建 不会存在相同ID号新建信息
                if (pl.IP == pladd.Ip)
                {
                    DataJson.panels pls = new DataJson.panels();
                    pls.id = Convert.ToInt32(pladd.Num);
                    pls.pid = randomNum;
                    pls.panelsInfo = new List<DataJson.panelsInfo>();
                    if (copyPanel != null)
                    {
                        //复制副本
                        pls.panelsInfo = (List<DataJson.panelsInfo>)ToolsUtil.CloneObject(copyPanel.panelsInfo);
                    }

                    pl.panels.Add(pls);
                    //添加point点
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.area1 = pladd.Area1;
                    eq.area2 = pladd.Area2;
                    eq.area3 = pladd.Area3;
                    eq.area4 = pladd.Area4;
                    eq.address = address;
                    eq.pid = randomNum;
                    eq.ip = pladd.Ip;
                    eq.name = pladd.PanelName ;
                    eq.type = IniHelper.findTypesIniTypebyName("面板");
                    eq.objType = "";
                    eq.value = "";
                    FileMesege.PointList.link.Add(eq);

                    //排序
                    PanelSort(pl);
                    string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                    if (FileMesege.panelSelectNode.Parent == null)
                    {
                        fullpath = FileMesege.panelSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Panel, pls.id, section, eq.name);

                    }
                    else
                    {
                        fullpath = FileMesege.panelSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Panel, pls.id, section, eq.name);
                    }
                    updatePanelView();
                  

                    break;
                }

            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        /// <summary>
        /// 在选中的该网关里面 面板按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void PanelSort(DataJson.Panel pl)
        {
            pl.panels.Sort(delegate(DataJson.panels x, DataJson.panels y)
            {
                return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
            });
        }


        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelAdd paneladd = new panelAdd();
            //展示居中
            paneladd.StartPosition = FormStartPosition.CenterParent;
            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] timerNodeTxt = treeView1.SelectedNode.Text.Split(' ');
            paneladd.Ip = ips[0];
            //获取面板号数
            paneladd.Num = Regex.Replace(timerNodeTxt[0], @"[^\d]*", "");

            paneladd.Xflag = true;

            paneladd.ShowDialog();

            if (paneladd.DialogResult == DialogResult.OK)
            {
                //获取面板号
                string num = Convert.ToInt32(paneladd.Num).ToString("X4");
    
                //获取IP最后一位
                string address = "FE30" + num;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址面板下的 面板信息对象
                DataJson.panels pls = DataListHelper.getPanelsInfoList(ips[0], Convert.ToInt32(paneladd.Oldnum));
                if (pls != null)
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
                    {
                        //修改当前的point点信息
                        if (pls.pid == eq.pid)
                        {
                            pls.id = Convert.ToInt32(paneladd.Num);
                            eq.area1 = paneladd.Area1;
                            eq.area2 = paneladd.Area2;
                            eq.area3 = paneladd.Area3;
                            eq.area4 = paneladd.Area4;
                            eq.address = address;
                            eq.name = paneladd.PanelName;
                            string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                            if (FileMesege.panelSelectNode.Parent == null)
                            {
                                fullpath = FileMesege.panelSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Panel, pls.id, section, eq.name);

                            }
                            else
                            {
                                fullpath = FileMesege.panelSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Panel, pls.id, section, eq.name);
                            }
                            break;
                        }

                    }
                    foreach (DataJson.Panel plIP in FileMesege.panelList)
                    {
                        if (plIP.IP == ips[0])
                        {
                            //排序
                            PanelSort(plIP);
                            break;
                        }
                    }
                    updatePanelView();
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }

               
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent == null)
            {
                return;
            }

            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] panelNodetxt = treeView1.SelectedNode.Text.Split(' ');

            foreach (DataJson.Panel pl in FileMesege.panelList)
            {
                //进入IP同一个
                if (pl.IP == ips[0])
                {
                    int id =Convert.ToInt32( Regex.Replace(panelNodetxt[0], @"[^\d]*", ""));
                    if (id < 100)
                    {

                        return;
                    }
                    foreach (DataJson.panels pls in pl.panels)
                    {
                        //当面板号一样
                        if (pls.id == id)
                        {
                            //撤销
                            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                            int Nodeindex = treeView1.SelectedNode.Index;
                            int pNodeindex = treeView1.SelectedNode.Parent.Index;
                            //移除pointList 中地址
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
                            {
                                //获取address与IP地址相同的对象
                                if (eq.pid == pls.pid)
                                {
                                    //移除Namelist 的对象
                                    FileMesege.PointList.link.Remove(eq);
                                    break;
                                }
                            }
                            //移除panellist的对象
                            pl.panels.Remove(pls);
                            //树状图移除选中节点
                            updatePanelView();
                            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                            FileMesege.cmds.DoNewCommand(NewList, OldList);
                            //选中删除节点的下一个节点 没有节点就直接选中父节点
                            if (treeView1.Nodes[pNodeindex].Nodes.Count > 0)
                            {
                                if (Nodeindex < treeView1.Nodes[pNodeindex].Nodes.Count)
                                {
                                    treeView1.SelectedNode = treeView1.Nodes[pNodeindex].Nodes[Nodeindex];
                                }
                                else
                                {
                                    treeView1.SelectedNode = treeView1.Nodes[pNodeindex].Nodes[0];
                                }

                            }
                            else
                            {
                                treeView1.SelectedNode = treeView1.Nodes[pNodeindex];
                            }
                            return;
                        }
                    }

                }

            }//IP FOREACH
        }

        //复制的定时
        DataJson.panels copyPanel = null;
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] ips = FileMesege.panelSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.panelSelectNode.Text.Split(' ');
            int panelNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址面板下的 面板信息对象
            DataJson.panels pls = DataListHelper.getPanelsInfoList(ips[0], panelNum);
            //可能存在克隆现象需要解决
            copyPanel = pls;
            //复制面板
            if (copyPanel == null)
            {
                return;
            }
            //展示居中
            pladd.StartPosition = FormStartPosition.CenterParent;
            pladd.Xflag = false;
            //复制的时候调用
            //获取IP
            //string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            pladd.Ip = ips[0];
            //获取面板数
            pladd.Num = (Convert.ToInt32(Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "")) + 1).ToString();
            pladd.ShowDialog();


        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
            //清除复制的定时
            copyPanel = null;
            newPanel();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            删除ToolStripMenuItem_Click(this, EventArgs.Empty);
        }


      
        #endregion

        #region 获取面板开启状态

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (client != null && client.Connected())
                {
                    string msg = "GET;{254.48.255.255};\r\n";
                    //客户端发送数据
                    client.SendAsync(msg);
                }
                if (!FileMesege.isDgvNameDeviceConnet)
                {
                    timer1.Stop();
                    client = null;
                }

            }
            catch
            {
                timer1.Stop();
                client = null;
                return;
            }
        }

        private void clientConnect()
        {
            try
            {

                if (client != null)
                {
                    client.Dispoes();
                    //client = null;
                }
                client = new ClientAsync();
                IniClient();
                if (treeView1.SelectedNode == null)
                {
                    return;
                }
                if (treeView1.SelectedNode.Parent == null)
                {
                    ip = treeView1.SelectedNode.Text.Split(' ')[0];
                }
                else
                {
                    ip = treeView1.SelectedNode.Parent.Text.Split(' ')[0];
                }
                //异步连接
                if (client != null)
                {
                    client.ConnectAsync(ip, 6003);

                }
                if (client != null && client.Connected())
                {
                    string msg = "GET;{254.48.255.255};\r\n";
                    //客户端发送数据
                    client.SendAsync(msg);
                }


            }
            catch(Exception ex)
            {
                Console.WriteLine("连接错误：" + ex.StackTrace + ex.Message);
                client = null;
                timer1.Stop();
                return;
            }
        }



        /// <summary>
        /// 初始化客户端的处理
        /// </summary>
        private void IniClient()
        {

            //实例化事件 传值到封装函数  c为函数类处理返回的client
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                string key = "";

                try
                {
                    if (c.Client.Connected)
                    {
                        //强转类型
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        //返回的IP 和 端口号
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch { }

                try
                {
                    switch (enAction)
                    {
                        case ClientAsync.EnSocketAction.Connect:
                            //MessageBox.Show("已经与" + key + "建立连接");

                            //timer1.Start();

                            break;
                        case ClientAsync.EnSocketAction.SendMsg:

                            //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                            break;
                        case ClientAsync.EnSocketAction.Close:
                            //client.Close();
                            //btnNew.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
                            //MessageBox.Show("服务端连接关闭");
                            break;
                        case ClientAsync.EnSocketAction.Error:
                            //btnNew.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
                            //MessageBox.Show("连接发生错误,请检查网络连接");

                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            });
            //信息接收处理
            client.Received += new Action<string, string>((key, msg) =>
            {
                Invoke(TCP6003Delegate,msg);
                
            });
        }


        /// <summary>
        /// 回调处理信息函数
        /// </summary>
        /// <param name="rcvInfo"></param>
        private void ThreePanel_TCP6003Delegate(string rcvInfo)
        {
            try
            {

                //获取FB开头的信息
                string[] strArray = rcvInfo.Split(new string[] { "FB", "ACK" }, StringSplitOptions.RemoveEmptyEntries);
                //MessageBox.Show(msg);
                Regex reg = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
                for (int i = 0; i < strArray.Length; i++)
                {
                    //数组信息按IP提取 
                    Match match = reg.Match(strArray[i]);
                    //分割内容 FB;00000001;{230.48.0.102};
                    string[] strs = strArray[i].Split(';');
                    if (strs.Length > 2)
                    {
                        if (match.Groups[2].Value == "48")
                        {
                            UpdateTreeNode(strs[1], match.Groups[4].Value);

                        }
                    }
                    //面板号
                    /*int panelID = Convert.ToInt32(match.Groups[4].Value);
                    Console.WriteLine("panelsID:"+ match.Groups[4].Value);*/

                }
            }
            catch
            {

            }
        }

        //更新网关里面key文件的开关
        private void UpdateTreeNode(string val,string panelID)
        {
            foreach (TreeNode ipNode in treeView1.Nodes)
            {
                if (ipNode.Text.Contains(ip) && !string.IsNullOrWhiteSpace(ip))
                {
                    string name = Resources.Panel + panelID;
                    //循环子节点
                    foreach (TreeNode tn in ipNode.Nodes)
                    {
                        if (tn.Text.Contains(name))
                        {
                            if (val.Contains("1"))
                            {
                                //绿色图标开启
                                tn.ImageIndex = 5;
                                tn.SelectedImageIndex = 5;

                            }
                            else
                            {
                                //红色图标关闭
                                tn.ImageIndex = 4;
                                tn.SelectedImageIndex = 4;
                            }
                            break;

                        }
                    }
                    break;
                }
            }
        }

        //更新树状图 图标
        private void TreeNodeIni()
        {
            if (FileMesege.panelSelectNode == null )
            {
                return;
            }
            if (treeView1.SelectedNode.Parent == null)
            {
                //选中网关IP
                if (FileMesege.panelSelectNode.Parent == null)
                {
                    if (FileMesege.panelSelectNode == treeView1.SelectedNode)
                    {
                        return;
                    }
                }
                else
                {
                    if (FileMesege.panelSelectNode.Parent == treeView1.SelectedNode)
                    {
                        return;
                    }
                }
            }
            else
            {
                //选中子节点
                if (FileMesege.panelSelectNode.Parent == null)
                {
                    if (FileMesege.panelSelectNode== treeView1.SelectedNode.Parent)
                    {
                        return;
                    }
                }
                else
                {
                    if (FileMesege.panelSelectNode.Parent == treeView1.SelectedNode.Parent)
                    {
                        return;
                    }
                }
               
            }
            //重新连接
            //获取当前状态
            if (FileMesege.isDgvNameDeviceConnet)
            {
                clientConnect();
                timer1.Start();

            }
            else
            {
                timer1.Stop();
                client = null;
            }

            foreach (TreeNode ipNode in treeView1.Nodes)
            {
                //循环子节点
                foreach (TreeNode tn in ipNode.Nodes)
                {
                    //白色图标开启
                    tn.ImageIndex = 1;
                    tn.SelectedImageIndex = 3;
                        
                }
                    
                
            }
        }



        #endregion


        #region 九键面板设置 和 搜索
        private void ButtonX1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || !treeView1.SelectedNode.Text.Contains("智能触摸屏")) {
                clearTxtShow("请先选中智能触摸屏的树状图节点");
                return;
            }
            string ip = treeView1.SelectedNode.Parent.Text.Split(' ')[0];
            string id = Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "");
            NineKeySet nineKeySet = new NineKeySet();
            nineKeySet.Ip = ip;
            nineKeySet.Id = Convert.ToInt32(id);

            DataJson.Module devModuel = DataListHelper.findDeviceByIP_ID(ip, Convert.ToInt32(id));
            if (devModuel == null)
            {
                return;
            }
            //假如从手机读回来的端口没有信息  在这里添加类型
            if (devModuel.devPortList == null || devModuel.devPortList.Count == 0)
            {
                DataListHelper.addPortType(devModuel.devPortList, devModuel.device);
            }
            nineKeySet.DevModuel = devModuel;
            nineKeySet.ShowDialog();
        }

        //搜索
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            searchSection.StartPosition = FormStartPosition.CenterParent;
            searchSection.SearchTreeNode += new Action(SearchTreeNode);
            searchSection.ShowDialog();
            if (searchSection.DialogResult == DialogResult.OK)
            {


            }
        }

        private void SearchTreeNode()
        {
            try
            {
                if (string.IsNullOrEmpty(searchSection.Area1))
                {
                    ThreePanelAddNode();
                    return;
                }
                //添加特定区域
                treeView1.Nodes.Clear();
                if (FileMesege.DeviceList == null)
                {
                    return;
                }
                //记录当前节点展开状况 
                TreeMesege tm = new TreeMesege();
                DataJson.PointInfo point = null;
                string section = "";
                //从设备加载网关信息
                foreach (DataJson.Device d in FileMesege.DeviceList)
                {
                    int index = tm.AddNode1(treeView1, d.ip + " " + d.master);
                    if (FileMesege.panelList != null)
                    {
                        foreach (DataJson.Panel sc in FileMesege.panelList)
                        {
                            //  添加该网关IP的子节点
                            if (sc.IP == d.ip)
                            {
                                foreach (DataJson.panels scs in sc.panels)
                                {
                                    point = DataListHelper.findPointByPid(scs.pid, FileMesege.PointList.link);
                                    if (point != null)
                                    {
                                        //判断是否符合搜索的点位
                                        if (string.IsNullOrEmpty(searchSection.Area4))
                                        {
                                            if (string.IsNullOrEmpty(searchSection.Area3))
                                            {
                                                if (string.IsNullOrEmpty(searchSection.Area2))
                                                {
                                                    if (string.IsNullOrEmpty(searchSection.Area1))
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (point.area1 != searchSection.Area1)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (point.area1 != searchSection.Area1
                                                        || point.area2 != searchSection.Area2)
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (point.area1 != searchSection.Area1
                                                    || point.area2 != searchSection.Area2
                                                    || point.area3 != searchSection.Area3)
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (point.area1 != searchSection.Area1
                                                || point.area2 != searchSection.Area2
                                                || point.area3 != searchSection.Area3
                                                || point.area4 != searchSection.Area4)
                                            {
                                                continue;
                                            }
                                        }
                                        section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Panel, scs.id, section, point.name), index);

                                    }

                                }

                            }
                        }
                    }

                }
                foreach (TreeNode node in treeView1.Nodes)
                {
                    node.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }
        #endregion


    }//class
}
