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

namespace eNet编辑器.ThreeView
{
    public delegate void DgvPanelAddItem();
    public partial class ThreePanel : Form
    {
        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        public event Action updatePanelView;
        public event DgvPanelAddItem dgvpanelAddItem;
        public event Action addTitleNode;
        public ThreePanel()
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

        private panelAdd pladd;

        //判断true为选中父节点
        bool isSelectParetnNode = false;

        private void ThreePanel_Load(object sender, EventArgs e)
        {
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

            FileMesege.panelSelectNode = treeView1.SelectedNode;
            //DGVtimer添加定时
            dgvpanelAddItem();
            addTitleNode();
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowPanelName + treeView1.SelectedNode.Text + "\r\n");
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1] + ".ini";
                clearTxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
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

            if (treeView1.SelectedNode.Parent == null)
            {
                //获取IP
                string[] ips = treeView1.SelectedNode.Text.Split(' ');
                pladd.Ip = ips[0];
                //获取定面板数
                pladd.Num = (treeView1.SelectedNode.GetNodeCount(false) + 1).ToString();
            }
            else
            {
                //复制的时候调用
                //获取IP
                string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
                pladd.Ip = ips[0];
                //获取面板数
                pladd.Num = (Convert.ToInt32(Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "")) + 1).ToString();
            }

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
            //获取定时号
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
                    pls.keyNum = 6;
                    pls.panelsInfo = new List<DataJson.panelsInfo>();
                    if (copyPanel != null)
                    {
                        //复制副本
                        pls.panelsInfo = (List<DataJson.panelsInfo>)CommandManager.CloneObject(copyPanel.panelsInfo);
                        //pls.panelsInfo = TransExpV2<List<DataJson.panelsInfo>, List<DataJson.panelsInfo>>.Trans(copyPanel.panelsInfo);
                        pls.keyNum = copyPanel.keyNum;
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
                    string parentNodePath = "";
                    if (treeView1.SelectedNode != null)
                    {
                        parentNodePath = treeView1.SelectedNode.FullPath;
                    }
                    updatePanelView();
                    if (FileMesege.panelSelectNode != null)
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
                    foreach (DataJson.panels pls in pl.panels)
                    {
                        //当场景号一样
                        if (pls.id.ToString() == Regex.Replace(panelNodetxt[0], @"[^\d]*", ""))
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
            //新建面板
            newPanel();
            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent != null)
            {
                return;
            }
            newPanel();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            删除ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        #endregion

       











    }//class
}
