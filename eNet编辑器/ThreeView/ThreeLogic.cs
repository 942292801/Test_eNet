using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using eNet编辑器.Properties;
using eNet编辑器.AddForm;
using System.Text.RegularExpressions;

namespace eNet编辑器.ThreeView
{
    public partial class ThreeLogic : Form
    {


        public event Action<string> clearTxtShow;
        public event Action updateLogicView;
        public event Action addTitleNode;
        public event Action dgvLogicAddItem;

        //判断true为选中父节点
        bool isSelectParetnNode = false;
        //树状图节点
        string fullpath = "";
        private logicAdd lgadd;

        public ThreeLogic()
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

        private void ThreeLogic_Load(object sender, EventArgs e)
        {
            lgadd = new logicAdd();
            lgadd.addLogicNode +=new Action(addLogicNode);
        }

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeLogicAddNode()
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
                    if (FileMesege.logicList != null)
                    {
                        foreach (DataJson.Logic lg in FileMesege.logicList)
                        {
                            //  添加该网关IP的子节点
                            if (lg.IP == d.ip)
                            {
                                foreach (DataJson.logics lgs in lg.logics)
                                {
                                    DataJson.PointInfo point = DataListHelper.findPointByPid(lgs.pid, FileMesege.PointList.logic);
                                    if (point != null)
                                    {
                                        section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Logic, lgs.id, section, point.name), index);

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
                MessageBox.Show("逻辑添加节点初始化失败,请检查logic.json文件");
            }
        }

        #region 节点样式
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
        #endregion

        #region 选中节点
        /// <summary>
        /// 选中DGVlogci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (FileMesege.logicSelectNode == null)
            {
                addTitleNode();
            }
            else
            {
                if (ToolsUtil.getIP(treeView1.SelectedNode) != ToolsUtil.getIP(FileMesege.logicSelectNode))
                {
                    addTitleNode();

                }
            }
            FileMesege.logicSelectNode = treeView1.SelectedNode;
            fullpath = treeView1.SelectedNode.FullPath;
            //DGVSceme添加场景
            dgvLogicAddItem();
          
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowLogicName + treeView1.SelectedNode.Text + "\r\n");
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1] + ".ini";
                clearTxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
            }
        }

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
                        //newitemflag = true;
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

        #region 新建 修改 复制 删除

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //右击树状图外面区域
            if (!isSelectParetnNode)
            {

            }
            else//右击树状图区域
            {
                //清除复制的感应
                copyLogic = null;
                //新建场景
                newLogic();

            }
        }

        /// <summary>
        /// 逻辑新建 修改弹框
        /// </summary>
        private void newLogic()
        {
            //展示居中
            lgadd.StartPosition = FormStartPosition.CenterParent;
            lgadd.Xflag = false;

            if (treeView1.SelectedNode.Parent == null)
            {
                //获取IP
                string[] ips = treeView1.SelectedNode.Text.Split(' ');
                lgadd.Ip = ips[0];
                //获取逻辑数
                lgadd.Num = (treeView1.SelectedNode.GetNodeCount(false)+1).ToString();
            }
            else
            {
                //复制的时候调用
                //获取IP
                string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
                lgadd.Ip = ips[0];
                //获取逻辑数
                lgadd.Num = (Convert.ToInt32(Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", "")) + 1).ToString();
            }

            lgadd.ShowDialog();
        }

         /// <summary>
        /// 新建逻辑回调
        /// </summary>
        private void addLogicNode()
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent == null)
            {
                treeView1.SelectedNode.Expand();

            }
            //获取定时号
            string num = Convert.ToInt32(lgadd.Num).ToString("X4");

            //获取IP最后一位 逻辑40
            string address = "FE40" + num;
            if (FileMesege.logicList == null)
            {
                FileMesege.logicList = new List<DataJson.Logic>();

            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加logicList 表中
            //该IP在logicList里面是否存在
            if (!FileMesege.logicList.Exists(x => x.IP == lgadd.Ip))
            {
                //不存在新建List
                DataJson.Logic lg = new DataJson.Logic();
                lg.IP = lgadd.Ip;
                lg.Dev = "GW100A";
                lg.logics = new List<DataJson.logics>();
                FileMesege.logicList.Add(lg);

            }

            //判断是否已经存在该点位信息
            int randomNum = DataChange.randomNum();

            foreach (DataJson.Logic lg in FileMesege.logicList)
            {
                //节点的IP相等 进入创建 不会存在相同ID号新建信息
                if (lg.IP == lgadd.Ip)
                {
                    DataJson.logics lgs = new DataJson.logics();
                    lgs.id = Convert.ToInt32(lgadd.Num);
                    lgs.pid = randomNum;
                    
                    lgs.logicsInfo = new List<DataJson.logicsInfo>();
                    DataJson.logicsInfo lgInfo = new DataJson.logicsInfo();
                    lgs.logicsInfo.Add(lgInfo);
                    if (copyLogic != null)
                    {
                        //复制副本
                        lgs.logicsInfo = (List<DataJson.logicsInfo>)CommandManager.CloneObject(copyLogic.logicsInfo);
                        //变量要唯一
                        //srs.ioNum = copySensor.ioNum;
                    }
                    lg.logics.Add(lgs);
                    //添加point点
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.area1 = lgadd.Area1;
                    eq.area2 = lgadd.Area2;
                    eq.area3 = lgadd.Area3;
                    eq.area4 = lgadd.Area4;
                    eq.address = address;
                    eq.pid = randomNum;
                    eq.ip = lgadd.Ip;
                    eq.name = lgadd.PanelName;
                    eq.type = IniHelper.findTypesIniTypebyName("逻辑");
                    eq.objType = "";
                    eq.value = "";
                    FileMesege.PointList.logic.Add(eq);
                    
                    /*string tmp = "";
                    //添加16个局部变量
                    for (int i = (lgs.id - 1) * 16 + 1; i <= lgs.id * 16; i++)
                    {
                        
                        //添加point点
                        DataJson.PointInfo localvar = new DataJson.PointInfo();
                        localvar.area1 = lgadd.Area1;
                        localvar.area2 = lgadd.Area2;
                        localvar.area3 = lgadd.Area3;
                        localvar.area4 = lgadd.Area4;
                        tmp = SocketUtil.strtohexstr(i.ToString());
                        while (tmp.Length < 4)
                        {
                           tmp = tmp.Insert(0,"0");
                        }
                        localvar.address = "FEF9" + tmp;
                        localvar.pid = DataChange.randomNum();
                        localvar.ip = lgadd.Ip;
                        localvar.name = string.Format("局部变量{0}@{1}", i.ToString(), lgadd.Ip.Split('.')[3]); ;
                        localvar.type = "15.0_LocalVariable";
                        localvar.objType = "";
                        localvar.value = "";
                        if (DataListHelper.findPointBySection_name(new List<string>() { eq.area1, eq.area2, eq.area3, eq.area4, localvar.name }) != null)
                        {
                            break;
                        }
                        FileMesege.PointList.localvar.Add(localvar);
                       
                    }*/
                    //排序
                    LogicSort(lg);
                    string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                    if (FileMesege.logicSelectNode.Parent == null)
                    {
                        fullpath = FileMesege.logicSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Logic, lgs.id, section, eq.name);

                    }
                    else
                    {
                        fullpath = FileMesege.logicSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Logic, lgs.id, section, eq.name);
                    }
                    updateLogicView();


                    break;
                }

            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        /// <summary>
        /// 在选中的该网关里面 逻辑按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void LogicSort(DataJson.Logic lg)
        {
            lg.logics.Sort(delegate(DataJson.logics x, DataJson.logics y)
            {
                return x.id.CompareTo(y.id);
            });
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            logicAdd logicadd = new logicAdd();
            //展示居中
            logicadd.StartPosition = FormStartPosition.CenterParent;
            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] timerNodeTxt = treeView1.SelectedNode.Text.Split(' ');
            logicadd.Ip = ips[0];
            //获取号数
            logicadd.Num = Regex.Replace(timerNodeTxt[0], @"[^\d]*", "");

            logicadd.Xflag = true;

            logicadd.ShowDialog();

            if (logicadd.DialogResult == DialogResult.OK)
            {
                //获取号
                string num = Convert.ToInt32(logicadd.Num).ToString("X4");

                //获取IP最后一位
                string address = ToolsUtil.GetIPstyle(logicadd.Ip, 4) + "40" + num;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址面板下的 面板信息对象
                DataJson.logics lgs = DataListHelper.getLogicsInfoList(ips[0], Convert.ToInt32(logicadd.Oldnum));
                if (lgs != null)
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
                    {
                        //修改当前的point点信息
                        if (lgs.pid == eq.pid)
                        {
                            /*
                            //修改16个变量
                            string addressVar = "";
                            int id = (Convert.ToInt32(logicadd.Num) - 1) * 16 + 1;
                            for (int i = (lgs.id - 1) * 16 + 1; i <= lgs.id * 16; i++)
                            {
                                addressVar = SocketUtil.strtohexstr(i.ToString());
                                while (addressVar.Length < 4)
                                {
                                    addressVar = addressVar.Insert(0, "0");
                                }
                                addressVar = "FEF9" + addressVar;
                                foreach (DataJson.PointInfo var in FileMesege.PointList.localvar)
                                {
                                    if (eq.ip == var.ip && var.address == addressVar)
                                    {
                                        var.area1 = logicadd.Area1;
                                        var.area2 = logicadd.Area2;
                                        var.area3 = logicadd.Area3;
                                        var.area4 = logicadd.Area4;
                                        string tmp = SocketUtil.strtohexstr(id.ToString());
                                        while (tmp.Length < 4)
                                        {
                                            tmp = tmp.Insert(0, "0");
                                        }
                                        var.address = "FEF9" + tmp;
                                        var.ip = logicadd.Ip;
                                        var.name = string.Format("局部变量{0}@{1}", id.ToString(), logicadd.Ip.Split('.')[3]); ;
                                        break;
                                    }
                                }
                                id++;
                            }*/
                            
                            lgs.id = Convert.ToInt32(logicadd.Num);
                            eq.area1 = logicadd.Area1;
                            eq.area2 = logicadd.Area2;
                            eq.area3 = logicadd.Area3;
                            eq.area4 = logicadd.Area4;
                            eq.address = address;
                            eq.name = logicadd.PanelName;
                            string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                            if (FileMesege.logicSelectNode.Parent == null)
                            {
                                fullpath = FileMesege.logicSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Logic, lgs.id, section, eq.name);

                            }
                            else
                            {
                                fullpath = FileMesege.logicSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Logic, lgs.id, section, eq.name);
                            }
                            break;
                        }

                    }
                    foreach (DataJson.Logic lg in FileMesege.logicList)
                    {
                        if (lg.IP == ips[0])
                        {
                            //排序
                            LogicSort(lg);
                            break;
                        }
                    }
                    updateLogicView();
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
            string[] logicNodetxt = treeView1.SelectedNode.Text.Split(' ');

            foreach (DataJson.Logic lg in FileMesege.logicList)
            {
                //进入IP同一个
                if (lg.IP == ips[0])
                {
                    foreach (DataJson.logics lgs in lg.logics)
                    {
                        //当场景号一样
                        if (lgs.id.ToString() == Regex.Replace(logicNodetxt[0], @"[^\d]*", ""))
                        {
                            //撤销
                            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                            int Nodeindex = treeView1.SelectedNode.Index;
                            int pNodeindex = treeView1.SelectedNode.Parent.Index;
                            //移除pointList 中地址
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
                            {
                                //获取address与IP地址相同的对象
                                if (eq.pid == lgs.pid)
                                {
                                    /*
                                    //删除16个变量
                                    string address = "";
                                    for (int i = (lgs.id - 1) * 16 + 1; i <= lgs.id * 16; i++)
                                    {
                                        address = SocketUtil.strtohexstr(i.ToString());
                                        while (address.Length < 4)
                                        {
                                            address = address.Insert(0, "0");
                                        }
                                        address = "FEF9" + address;
                                        foreach (DataJson.PointInfo var in FileMesege.PointList.localvar)
                                        {
                                            if (eq.ip == var.ip && var.address == address)
                                            {
                                                FileMesege.PointList.localvar.Remove(var);
                                                break;
                                            }
                                        }
                                    }*/
                                    //移除Namelist 的对象
                                    FileMesege.PointList.logic.Remove(eq);
                                    break;
                                }
                            }
                            
                            //移除sensorlist的对象
                            lg.logics.Remove(lgs);
                            //树状图移除选中节点
                            updateLogicView();
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

        //复制的逻辑
        DataJson.logics copyLogic = null;
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] ips = FileMesege.logicSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.logicSelectNode.Text.Split(' ');
            int logicNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址面板下的 面板信息对象
            DataJson.logics lgs = DataListHelper.getLogicsInfoList(ips[0], logicNum);
            //可能存在克隆现象需要解决
            copyLogic = lgs;
            //新建面板
            newLogic();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent != null)
            {
                return;
            }
            newLogic();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            删除ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        #endregion




    }
}
