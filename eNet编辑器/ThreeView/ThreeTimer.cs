using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eNet编辑器.AddForm;
using System.Text.RegularExpressions;
using eNet编辑器.Properties;
using System.Reflection;
using System.Net;

namespace eNet编辑器.ThreeView
{
    public partial class ThreeTimer : Form
    {
        public ThreeTimer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
            //利用反射设置DataGridView的双缓冲
            //Type dgvType = this.treeView1.GetType();
            //PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            //BindingFlags.Instance | BindingFlags.NonPublic);
            //pi.SetValue(this.treeView1, true, null);
            
        }

        #region 解决背景闪烁
        //测试 解决背景闪烁
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014)
                // 禁掉清除背景消息         
                return;
            base.WndProc(ref m);
        }
        //测试 解决背景闪烁
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        #endregion

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        public event Action updateTimerView;
        public event Action dgvTimerAddItem;
        public event Action addTitleNode;
        private event Action<string> TCP6003Delegate;
        private timerAdd timeradd = null;

        //判断true为选中父节点
        bool isSelectParetnNode = false;

        //树状图节点
        string fullpath = "";
        //客户端
        ClientAsync client;
        string ip = "";
        SearchSection searchSection = new SearchSection();

        private void ThreeTimer_Load(object sender, EventArgs e)
        {
            TCP6003Delegate += ThreePanel_TCP6003Delegate;
            timeradd = new timerAdd();
            timeradd.addTimerNode += new Action(addTimerNode);
        }

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeTimerAddNode()
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
                    
                    if (FileMesege.timerList != null)
                    {
                        foreach (DataJson.Timer timer in FileMesege.timerList)
                        {
                            //  添加该网关IP的子节点
                            if (timer.IP == d.ip)
                            {
                                foreach (DataJson.timers tms in timer.timers)
                                {
                                    DataJson.PointInfo point = DataListHelper.findPointByPid(tms.pid, FileMesege.PointList.timer);
                                    if (point != null)
                                    {
                                        section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Timer, tms.id, section, point.name), index);

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
        /// 根据point点 选中树节点
        /// </summary>
        /// <param name="point"></param>
        public void FindNodeSelect(DataJson.PointInfo point)
        {
            TreeMesege.SelectNodeByPoint(treeView1,point);
        }

        #region  新建 修改 删除 复制
        private void 添加定时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //右击树状图外面区域
            if (!isSelectParetnNode)
            {

            }
            else//右击树状图区域
            {
                //清除复制的定时
                copyTimer = null;
                //新建场景
                newTimer(false);

            }
        }

        /// <summary>
        /// 新建定时弹框  true为新建假期时钟  false为新建普通时钟
        /// </summary>
        /// <param name="isHoliday">true为新建假期时钟  false为新建普通时钟</param>
        private void newTimer(bool isHoliday)
        {
            //展示居中
            timeradd.StartPosition = FormStartPosition.CenterParent;
            timeradd.Xflag = false;
            
            if (treeView1.SelectedNode.Parent == null)
            {
                //获取IP
                string[] ips = treeView1.SelectedNode.Text.Split(' ');
                timeradd.Ip = ips[0];
                //获取定时号数
                timeradd.Num = (treeView1.SelectedNode.GetNodeCount(false) + 1).ToString();
            }
            else
            {
                //复制的时候调用
                //获取IP
                string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
                timeradd.Ip = ips[0];
                //获取定时号数
                timeradd.Num = (Convert.ToInt32(Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "")) + 1).ToString();
            }
            if (isHoliday)
            {
                
                timeradd.Num = "10000";
                foreach (TreeNode node in treeView1.SelectedNode.Nodes)
                {
                    if (node.Text == "节假日定时")
                    {
                        return;
                    }
                }
            }
            timeradd.IsHoliday = isHoliday;
            
            timeradd.ShowDialog();
        }

        /// <summary>
        /// 新建定时回调操作
        /// </summary>
        private void addTimerNode()
        {
            
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent == null)
            {
                treeView1.SelectedNode.Expand();

            }
            //获取定时号
            string num = Convert.ToInt32(timeradd.Num).ToString("X4");

            //获取IP最后一位 定时为20
            string address ="FE20" + num;
            if (FileMesege.timerList == null)
            {
                FileMesege.timerList = new List<DataJson.Timer>();

            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加timerList 表中
            //该IP在timerList里面是否存在  
            if (!FileMesege.timerList.Exists(x => x.IP == timeradd.Ip))
            {
                //不存在新建List
                DataJson.Timer timer = new DataJson.Timer();
                timer.IP = timeradd.Ip;
                timer.Dev = "GW100A";
                timer.timers = new List<DataJson.timers>();
                FileMesege.timerList.Add(timer);

            }

            //判断是否已经存在该点位信息
            int randomNum = DataChange.randomNum();

            foreach (DataJson.Timer timer in FileMesege.timerList)
            {
                //节点的IP相等 进入创建 不会存在相同ID号新建信息
                if (timer.IP == timeradd.Ip)
                {
                    DataJson.timers tms = new DataJson.timers();
                    tms.id = Convert.ToInt32(timeradd.Num);
                    tms.pid = randomNum;
                    tms.dates = "";
                    //默认不跳过节假日
                    tms.priorHoloday = "01000001";
                    tms.timersInfo = new List<DataJson.timersInfo>();
                    if (copyTimer != null)
                    {
                        //复制副本
                        tms.timersInfo = (List<DataJson.timersInfo>)ToolsUtil.CloneObject(copyTimer.timersInfo);
                        //tms.timersInfo = TransExpV2<List<DataJson.timersInfo>, List<DataJson.timersInfo>>.Trans(copyTimer.timersInfo);
                        tms.dates = copyTimer.dates;
                        tms.priorHoloday = copyTimer.priorHoloday;
                    }
                    
                    timer.timers.Add(tms);
                    
                    //添加point点
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.area1 = timeradd.Area1;
                    eq.area2 = timeradd.Area2;
                    eq.area3 = timeradd.Area3;
                    eq.area4 = timeradd.Area4;
                    eq.address = address;
                    eq.pid = randomNum;
                    eq.ip = timeradd.Ip;
                    eq.name = timeradd.TimerName;
                    eq.type = IniHelper.findTypesIniTypebyName("定时");
                    eq.objType = "";
                    eq.value = "";
                    FileMesege.PointList.timer.Add(eq);

                    //排序
                    TimerSort(timer);
                    string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                    if (FileMesege.timerSelectNode.Parent == null)
                    {
                        fullpath = FileMesege.timerSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Timer, tms.id, section, eq.name);

                    }
                    else
                    {
                        fullpath = FileMesege.timerSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Timer, tms.id, section, eq.name);
                    }
                    updateTimerView();
                   

                    break;
                }

            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        /// <summary>
        /// 在选仲网关 场景按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void TimerSort(DataJson.Timer timer)
        {
            timer.timers.Sort(delegate(DataJson.timers x, DataJson.timers y)
            {
                return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
            });
        }

        private void 添加节假日定时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //新建场景
            newTimer(true);
        }
        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
  
            timerAdd tmadd = new timerAdd();
            //展示居中
            tmadd.StartPosition = FormStartPosition.CenterParent;
            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] timerNodeTxt = treeView1.SelectedNode.Text.Split(' ');
            tmadd.Ip = ips[0];
            //获取定时号数
            tmadd.Num = Regex.Replace(timerNodeTxt[0], @"[^\d]*", "");
            if (tmadd.Num == "10000")
            {
                //MessageBox.Show("不能修改节假日定时","警告");
                return;
            }
            tmadd.Xflag = true;
            timeradd.IsHoliday = false;
            tmadd.ShowDialog();
            
            if (tmadd.DialogResult == DialogResult.OK)
            {
                //获取场景号
                string num = Convert.ToInt32(tmadd.Num).ToString("X4");

                //获取IP最后一位
                string address =  "FE20" + num;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.timers tms = DataListHelper.getTimersInfoList(ips[0], Convert.ToInt32(tmadd.Oldnum));
                if (tms != null)
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
                    {
                        //修改当前的point点信息
                        if (tms.pid == eq.pid)
                        {
                            tms.id = Convert.ToInt32(tmadd.Num);
                            eq.area1 = tmadd.Area1;
                            eq.area2 = tmadd.Area2;
                            eq.area3 = tmadd.Area3;
                            eq.area4 = tmadd.Area4;
                            eq.address = address;
                            eq.name = tmadd.TimerName;
                            string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                            if (FileMesege.timerSelectNode.Parent == null)
                            {
                                fullpath = FileMesege.timerSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Timer, tms.id, section, eq.name);

                            }
                            else
                            {
                                fullpath = FileMesege.timerSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Timer, tms.id, section, eq.name);
                            }
                            break;
                        }

                    }
                    foreach (DataJson.Timer tmIP in FileMesege.timerList)
                    {
                        if (tmIP.IP == ips[0])
                        {
                            //排序
                            TimerSort(tmIP);
                            break;
                        }
                    }
                    updateTimerView();
                }

                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent == null)
            {
                return;
            }

            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] timerNodetxt = treeView1.SelectedNode.Text.Split(' ');

            foreach (DataJson.Timer timer in FileMesege.timerList)
            {
                //进入IP同一个
                if (timer.IP == ips[0])
                {
                    foreach (DataJson.timers tms in timer.timers)
                    {
                        //当场景号一样
                        if (tms.id.ToString() == Regex.Replace(timerNodetxt[0], @"[^\d]*", ""))
                        {
                            //撤销
                            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                            int Nodeindex = treeView1.SelectedNode.Index;
                            int pNodeindex = treeView1.SelectedNode.Parent.Index;
                            //移除pointList 中地址
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
                            {
                                //获取address与IP地址相同的对象
                                if (eq.pid == tms.pid)
                                {
                                    //移除Namelist 的对象
                                    FileMesege.PointList.timer.Remove(eq);
                                    break;
                                }
                            }
                            //移除timerlist的对象
                            timer.timers.Remove(tms);
                            //树状图移除选中节点
                            updateTimerView();
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
        DataJson.timers copyTimer = null;
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] ips = FileMesege.timerSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.timerSelectNode.Text.Split(' ');
            int timerNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址定时下的 定时信息对象
            DataJson.timers tms = DataListHelper.getTimersInfoList(ips[0], timerNum); 
            //可能存在克隆现象需要解决
            copyTimer = tms;
            //新建定时
            newTimer(false);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
            //新建场景
            newTimer(false);
        }
        private void btnAddHoliday_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
            //新建定时
            newTimer(true);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            删除ToolStripMenuItem_Click(this, EventArgs.Empty);
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
                    ThreeTimerAddNode();
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
                    if (FileMesege.timerList != null)
                    {
                        foreach (DataJson.Timer timer in FileMesege.timerList)
                        {
                            //  添加该网关IP的子节点
                            if (timer.IP == d.ip)
                            {
                                foreach (DataJson.timers tms in timer.timers)
                                {
                                    point = DataListHelper.findPointByPid(tms.pid, FileMesege.PointList.timer);
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
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Timer, tms.id, section, point.name), index);

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
            catch
            {

            }
           
        }

        #endregion

        #region  节点高亮 节点单击 节点单击后
        //选中节点高亮不消失
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
        /// 选中节点加载DGVtimer信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (FileMesege.timerSelectNode == null)
            {
                addTitleNode();
            }
            else
            {
                if (ToolsUtil.getIP(treeView1.SelectedNode) != ToolsUtil.getIP(FileMesege.timerSelectNode))
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
            FileMesege.timerSelectNode = treeView1.SelectedNode;
            fullpath = treeView1.SelectedNode.FullPath;
            //DGVtimer添加定时
            dgvTimerAddItem();
 
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowTimerName + treeView1.SelectedNode.Text );
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1]+".ini";
                clearTxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note"));
            }
        }

        /// <summary>
        /// 右击选中节点
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











        #endregion

        #region 获取定时开启状态
        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (client != null && client.Connected())
                {
                    string msg = "GET;{254.32.255.255};\r\n";
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
                    string msg = "GET;{254.32.255.255};\r\n";
                    //客户端发送数据
                    client.SendAsync(msg);
                }


            }
            catch (Exception ex)
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
                Invoke(TCP6003Delegate, msg);

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
                    //分割内容 FB;00000001;{230.32.0.102};
                    string[] strs = strArray[i].Split(';');
                    if (strs.Length > 2)
                    {
                        if (match.Groups[2].Value == "32")
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
        private void UpdateTreeNode(string val, string panelID)
        {
            foreach (TreeNode ipNode in treeView1.Nodes)
            {
                if (ipNode.Text.Contains(ip) && !string.IsNullOrWhiteSpace(ip))
                {
                    string name = Resources.Timer + panelID;
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
            if (FileMesege.timerSelectNode == null)
            {
                return;
            }
            if (treeView1.SelectedNode.Parent == null)
            {
                //选中网关IP
                if (FileMesege.timerSelectNode.Parent == null)
                {
                    if (FileMesege.timerSelectNode == treeView1.SelectedNode)
                    {
                        return;
                    }
                }
                else
                {
                    if (FileMesege.timerSelectNode.Parent == treeView1.SelectedNode)
                    {
                        return;
                    }
                }
            }
            else
            {
                //选中子节点
                if (FileMesege.timerSelectNode.Parent == null)
                {
                    if (FileMesege.timerSelectNode == treeView1.SelectedNode.Parent)
                    {
                        return;
                    }
                }
                else
                {
                    if (FileMesege.timerSelectNode.Parent == treeView1.SelectedNode.Parent)
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

       
    }
}
