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
            //利用反射设置DataGridView的双缓冲
            //Type dgvType = this.treeView1.GetType();
            //PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            //BindingFlags.Instance | BindingFlags.NonPublic);
            //pi.SetValue(this.treeView1, true, null);
            
        }

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        public event Action updateTimerView;
        public event Action dgvTimerAddItem;
        public event Action addTitleNode;

        private timerAdd timeradd = null;

        //判断true为选中父节点
        bool isSelectParetnNode = false;

        private void ThreeTimer_Load(object sender, EventArgs e)
        {
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
                        tms.timersInfo = (List<DataJson.timersInfo>)CommandManager.CloneObject(copyTimer.timersInfo);
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
                    string parentNodePath = "";
                    if (treeView1.SelectedNode != null)
                    {
                        parentNodePath = treeView1.SelectedNode.FullPath;
                    }
                    updateTimerView();
                    if (FileMesege.timerSelectNode != null)
                    {
                        try
                        {
                            TreeMesege.findNodeByName(treeView1, parentNodePath).Expand();
                        }
                        catch
                        {

                        }

                    }

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
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent != null)
            {
                return;
            }
            //新建场景
            newTimer(false);
        }
        private void btnAddHoliday_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent != null)
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
            
            FileMesege.timerSelectNode = treeView1.SelectedNode;
            //DGVtimer添加定时
            dgvTimerAddItem();
            addTitleNode();
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowTimerName + treeView1.SelectedNode.Text + "\r\n");
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1]+".ini";
                clearTxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
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

       

       






    }
}
