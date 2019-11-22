using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eNet编辑器.Properties;
using eNet编辑器.AddForm;
using System.Text.RegularExpressions;
using System.Reflection;

namespace eNet编辑器.ThreeView
{
    public partial class ThreeSensor : Form
    {
        public ThreeSensor()
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

        private sensorAdd sradd;

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        public event Action updateSensorView;
        public event Action dgvSensorAddItem;
        public event Action addTitleNode;
        //判断true为选中父节点
        bool isSelectParetnNode = false;

        //树状图节点
        string fullpath = "";

        private void ThreeSensor_Load(object sender, EventArgs e)
        {
            sradd = new sensorAdd();
            sradd.addSensorNode += new Action(addSensorNode);
        }

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeSensorAddNode()
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

                    if (FileMesege.sensorList != null)
                    {
                        foreach (DataJson.Sensor sr in FileMesege.sensorList)
                        {
                            //  添加该网关IP的子节点
                            if (sr.IP == d.ip)
                            {
                                foreach (DataJson.sensors srs in sr.sensors)
                                {
                                    DataJson.PointInfo point = DataListHelper.findPointByPid(srs.pid, FileMesege.PointList.link);
                                    if (point != null)
                                    {
                                        section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Sensor, srs.id, section, point.name), index);

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
        //选中字体高亮不消失
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (FileMesege.sensorSelectNode == null)
            {
                addTitleNode();
            }
            else
            {
                if (ToolsUtil.getIP(treeView1.SelectedNode) != ToolsUtil.getIP(FileMesege.sensorSelectNode))
                {
                    addTitleNode();

                }
            }
            FileMesege.sensorSelectNode = treeView1.SelectedNode;
            fullpath = treeView1.SelectedNode.FullPath;
            //DGVtimer添加定时
            dgvSensorAddItem();
  
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowSensorName + treeView1.SelectedNode.Text + "\r\n");
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

        #endregion

        #region  新建修改删除 复制
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //右击树状图外面区域
            if (!isSelectParetnNode)
            {

            }
            else//右击树状图区域
            {
                //清除复制的感应
                copySensor = null;
                //新建场景
                newSensor();

            }
        }

        /// <summary>
        /// 感应新建 修改弹框
        /// </summary>
        private void newSensor()
        {
            //展示居中
            sradd.StartPosition = FormStartPosition.CenterParent;
            sradd.Xflag = false;

            if (treeView1.SelectedNode.Parent == null)
            {
                //获取IP
                string[] ips = treeView1.SelectedNode.Text.Split(' ');
                sradd.Ip = ips[0];
                //获取编组数
                sradd.Num = (treeView1.SelectedNode.GetNodeCount(false) + 1001).ToString();
            }
            else
            {
                //复制的时候调用
                //获取IP
                string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
                sradd.Ip = ips[0];
                //获取面板数
                sradd.Num = (Convert.ToInt32(Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "")) + 1).ToString();
            }

            sradd.ShowDialog();
        }

        /// <summary>
        /// 新建感应回调
        /// </summary>
        private void addSensorNode()
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent == null)
            {
                treeView1.SelectedNode.Expand();

            }
            //获取定时号
            string num = Convert.ToInt32(sradd.Num).ToString("X4");

            //获取IP最后一位 定时为20
            string address ="FE30" + num;
            if (FileMesege.sensorList == null)
            {
                FileMesege.sensorList = new List<DataJson.Sensor>();

            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加timerList 表中
            //该IP在timerList里面是否存在  
            if (!FileMesege.sensorList.Exists(x => x.IP == sradd.Ip))
            {
                //不存在新建List
                DataJson.Sensor sr = new DataJson.Sensor();
                sr.IP = sradd.Ip;
                sr.Dev = "GW100A";
                sr.sensors = new List<DataJson.sensors>();
                FileMesege.sensorList.Add(sr);

            }

            //判断是否已经存在该点位信息
            int randomNum = DataChange.randomNum();

            foreach (DataJson.Sensor sr in FileMesege.sensorList)
            {
                //节点的IP相等 进入创建 不会存在相同ID号新建信息
                if (sr.IP == sradd.Ip)
                {
                    DataJson.sensors srs = new DataJson.sensors();
                    srs.id = Convert.ToInt32(sradd.Num);
                    srs.pid = randomNum;
                    srs.ioNum = 2;
                    srs.sensorsInfo = new List<DataJson.sensorsInfo>();
                    if (copySensor != null)
                    {
                        //复制副本
                        srs.sensorsInfo = (List<DataJson.sensorsInfo>)CommandManager.CloneObject(copySensor.sensorsInfo);
                        
                        //srs.sensorsInfo = TransExpV2<List<DataJson.sensorsInfo>, List<DataJson.sensorsInfo>>.Trans(copySensor.sensorsInfo);
                        srs.ioNum = copySensor.ioNum;
                    }
                    sr.sensors.Add(srs);
                    
                    //添加point点
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.area1 = sradd.Area1;
                    eq.area2 = sradd.Area2;
                    eq.area3 = sradd.Area3;
                    eq.area4 = sradd.Area4;
                    eq.address = address;
                    eq.pid = randomNum;
                    eq.ip = sradd.Ip;
                    eq.name = sradd.PanelName;
                    eq.type = IniHelper.findTypesIniTypebyName("感应编组");
                    eq.objType = "";
                    eq.value = "";
                    FileMesege.PointList.link.Add(eq);

                    //排序
                    PanelSort(sr);
                    string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                    if (FileMesege.sensorSelectNode.Parent == null)
                    {
                        fullpath = FileMesege.sensorSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Sensor, srs.id, section, eq.name);

                    }
                    else
                    {
                        fullpath = FileMesege.sensorSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Sensor, srs.id, section, eq.name);
                    }
                    updateSensorView();
               
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
        private void PanelSort(DataJson.Sensor sr)
        {
            sr.sensors.Sort(delegate(DataJson.sensors x, DataJson.sensors y)
            {
                return x.id.CompareTo(y.id);
            });
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sensorAdd sensoradd = new sensorAdd();
            //展示居中
            sensoradd.StartPosition = FormStartPosition.CenterParent;
            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] timerNodeTxt = treeView1.SelectedNode.Text.Split(' ');
            sensoradd.Ip = ips[0];
            //获取面板号数
            sensoradd.Num = Regex.Replace(timerNodeTxt[0], @"[^\d]*", "");

            sensoradd.Xflag = true;

            sensoradd.ShowDialog();

            if (sensoradd.DialogResult == DialogResult.OK)
            {
                //获取面板号
                string num = Convert.ToInt32(sensoradd.Num).ToString("X4");

                //获取IP最后一位
                string address = "FE30" + num;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址面板下的 面板信息对象
                DataJson.sensors srs = DataListHelper.getSensorsInfoList(ips[0], Convert.ToInt32(sensoradd.Oldnum));
                if (srs != null)
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
                    {
                        //修改当前的point点信息
                        if (srs.pid == eq.pid)
                        {
                            srs.id = Convert.ToInt32(sensoradd.Num);
                            eq.area1 = sensoradd.Area1;
                            eq.area2 = sensoradd.Area2;
                            eq.area3 = sensoradd.Area3;
                            eq.area4 = sensoradd.Area4;
                            eq.address = address;
                            eq.name = sensoradd.PanelName;
                            string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                            if (FileMesege.sensorSelectNode.Parent == null)
                            {
                                fullpath = FileMesege.sensorSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Sensor, srs.id, section, eq.name);

                            }
                            else
                            {
                                fullpath = FileMesege.sensorSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Sensor, srs.id, section, eq.name);
                            }
                            break;
                        }

                    }
                    foreach (DataJson.Sensor sr in FileMesege.sensorList)
                    {
                        if (sr.IP == ips[0])
                        {
                            //排序
                            PanelSort(sr);
                            break;
                        }
                    }
                    updateSensorView();
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

            foreach (DataJson.Sensor sr in FileMesege.sensorList)
            {
                //进入IP同一个
                if (sr.IP == ips[0])
                {
                    foreach (DataJson.sensors srs in sr.sensors)
                    {
                        //当场景号一样
                        if (srs.id.ToString() == Regex.Replace(panelNodetxt[0], @"[^\d]*", ""))
                        {
                            //撤销
                            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                            int Nodeindex = treeView1.SelectedNode.Index;
                            int pNodeindex = treeView1.SelectedNode.Parent.Index;
                            //移除pointList 中地址
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
                            {
                                //获取address与IP地址相同的对象
                                if (eq.pid == srs.pid)
                                {
                                    //移除Namelist 的对象
                                    FileMesege.PointList.link.Remove(eq);
                                    break;
                                }
                            }
                            //移除sensorlist的对象
                            sr.sensors.Remove(srs);
                            //树状图移除选中节点
                            updateSensorView();
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

        //复制的感应
        DataJson.sensors copySensor = null;
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] ips = FileMesege.sensorSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sensorSelectNode.Text.Split(' ');
            int panelNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址面板下的 面板信息对象
            DataJson.sensors srs = DataListHelper.getSensorsInfoList(ips[0], panelNum);
            //可能存在克隆现象需要解决
            copySensor = srs;
            //新建面板
            newSensor();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent != null)
            {
                return;
            }
            newSensor();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            删除ToolStripMenuItem_Click(this, EventArgs.Empty);

        }
        #endregion

    }
}
