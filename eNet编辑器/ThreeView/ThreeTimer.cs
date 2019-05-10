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

namespace eNet编辑器.ThreeView
{
    public partial class ThreeTimer : Form
    {
        public ThreeTimer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        public event Action updateAllView;
        public event Action dgvTimerAddItem;
        timerAdd timeradd = null;

        //判断true为选中父节点
        bool newitemflag = false;

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
                TreeMesege tm = new TreeMesege();

                //记录当前节点展开状况 
                List<string> isExpands = tm.treeIsExpandsState(treeView1);

                if (FileMesege.DeviceList == null)
                {
                    return;
                }
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

        #region  新建 修改 删除 复制
        private void 添加定时ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //右击树状图外面区域
            if (newitemflag != true)
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
        /// 新建定时弹框
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
                //获取场景号数
                timeradd.Num = (treeView1.SelectedNode.GetNodeCount(false) + 1).ToString();
            }
            else
            {
                //获取IP
                string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
                timeradd.Ip = ips[0];
                //获取场景号数
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
            string num = Convert.ToInt32(timeradd.Num).ToString("X");
            while (num.Length < 4)
            {
                num = num.Insert(0, "0");
            }
            //获取IP最后一位
            string address = SocketUtil.GetIPstyle(timeradd.Ip, 4) + "10" + num;
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
                    tms.timersInfo = new List<DataJson.timersInfo>();
                    if (copyTimer != null)
                    {

                        tms.timersInfo = (List<DataJson.timersInfo>)CommandManager.CloneObject(copyTimer);
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
                    eq.type = "5.0_time";
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
                    updateAllView();
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

        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //复制的场景
        List<DataJson.timersInfo> copyTimer = null;
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            //新建场景
            newTimer(true);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent == null)
            {
                return;
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
            
            FileMesege.timerSelectNode = treeView1.SelectedNode;
            //DGVSceme添加场景
            dgvTimerAddItem();
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowTimerName + treeView1.SelectedNode.Text + "\r\n");
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1]+".ini";
                clearTxtShow(Resources.TxtShowTimerName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
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
                newitemflag = false;
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
                        newitemflag = true;
                    }
                }
            }
        }


        #endregion

       

       






    }
}
