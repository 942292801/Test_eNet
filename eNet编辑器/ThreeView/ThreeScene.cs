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
    public delegate void DgvSceneAddItem();

    public partial class ThreeScene : Form
    {

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> clearTxtShow;
        sceneAdd tss ;
        public event DgvSceneAddItem dgvsceneAddItem;
        public event Action updateSceneView;

        public event Action addTitleNode;
        //判断true为选中父节点
        bool newitemflag = false;

        //树状图节点
        string fullpath = "";

        public ThreeScene()
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


        private void ThreeScene_Load(object sender, EventArgs e)
        {

            tss = new sceneAdd();
            tss.addSceneNode += new AddSceneNode(addSceneNodeDelegate);
        }

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeSceneAddNode()
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
                    if (FileMesege.sceneList != null)
                    {
                        foreach (DataJson.Scene sc in FileMesege.sceneList)
                        {
                            //  添加该网关IP的子节点
                            if (sc.IP==d.ip)
                            {
                                foreach (DataJson.scenes scs in sc.scenes)
                                {
                                    DataJson.PointInfo point = DataListHelper.findPointByPid(scs.pid,FileMesege.PointList.scene);
                                    if(point != null)
                                    {
                                        section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                        int index2 = tm.AddNode2(treeView1, string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name), index);
                                            
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
            catch { 
                //错误处理
                MessageBox.Show("场景添加节点初始化失败,请检查scene.json文件");
            }
        }

        /// <summary>
        /// 根据point点 选中树节点
        /// </summary>
        /// <param name="point"></param>
        public void FindNodeSelect(DataJson.PointInfo point)
        {
            TreeMesege.SelectNodeByPoint(treeView1, point);
        }

        #region 树状图新建 删除 修改 排序
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //右击树状图外面区域
            if (newitemflag != true)
            {

            }
            else//右击树状图区域
            {
                //清除复制的场景
                copyScene = null;
                //新建场景
                newTsScene();
                    
            }

            
        }

        /// <summary>
        /// 在选仲网关 场景按照ID重新排列顺序
        /// </summary>
        /// <param name="sc">当前对象排序</param>
        private void ScenesSort(DataJson.Scene sc)
        {
            sc.scenes.Sort(delegate(DataJson.scenes x, DataJson.scenes y)
            {
                return Convert.ToInt32(x.id).CompareTo(Convert.ToInt32(y.id));
            });
        }


        /// <summary>
        /// 弹出新建的窗口
        /// </summary>
        /// <returns></returns>
        private void newTsScene()
        {
            
            //展示居中
            tss.StartPosition = FormStartPosition.CenterParent;
            tss.Xflag = false;
            if (treeView1.SelectedNode.Parent == null)
            {
                //获取IP
                string[] ips = treeView1.SelectedNode.Text.Split(' ');
                tss.Ip = ips[0];
                //获取场景号数
                tss.Num = (treeView1.SelectedNode.GetNodeCount(false) + 1).ToString();
            }
            else
            {
                //复制功能用的
                //获取IP
                string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
                tss.Ip = ips[0];
                //获取场景号数
                tss.Num = (Convert.ToInt32(Regex.Replace(treeView1.SelectedNode.Text.Split(' ')[0], @"[^\d]*", ""))+1).ToString();
            }
            tss.ShowDialog();
            
        }

        /// <summary>
        /// 添加场景回调
        /// </summary>
        private void addSceneNodeDelegate()
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent == null)
            {
                treeView1.SelectedNode.Expand();
                
            }
            //获取场景号
            string num = Convert.ToInt32(tss.Num).ToString("X4");
            //获取IP最后一位
            //string address = SocketUtil.GetIPstyle(tss.Ip, 4) + "10" + num;
            string address = "FE10" + num;
            if (FileMesege.sceneList == null)
            {
                FileMesege.sceneList = new List<DataJson.Scene>();

            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //添加SceneList 表中
            //该IP在SceneList里面是否存在  
            if (!FileMesege.sceneList.Exists(x => x.IP == tss.Ip))
            {
                //不存在新建List
                DataJson.Scene sc = new DataJson.Scene();
                sc.IP = tss.Ip;
                sc.Dev = "GW100A";
                sc.scenes = new List<DataJson.scenes>();
                FileMesege.sceneList.Add(sc);

            }
         
            //判断是否已经存在该点位信息
            int randomNum = DataChange.randomNum();
           
            foreach (DataJson.Scene sc in FileMesege.sceneList)
            {
                //节点的IP相等 进入创建 不会存在相同ID号新建信息
                if (sc.IP == tss.Ip)
                {
                    DataJson.scenes scs = new DataJson.scenes();
                    scs.id = Convert.ToInt32(tss.Num);                   
                    scs.pid = randomNum; 
                    //scs.address = address;
                    scs.sceneInfo = new List<DataJson.sceneInfo>();
                    if (copyScene != null)
                    {

                        scs.sceneInfo = (List<DataJson.sceneInfo>)ToolsUtil.CloneObject(copyScene);
                        //scs.sceneInfo = TransExpV2<List<DataJson.sceneInfo>, List<DataJson.sceneInfo>>.Trans(copyScene);
                    }
                    sc.scenes.Add(scs);
                    //添加point点
                    DataJson.PointInfo eq = new DataJson.PointInfo();
                    eq.area1 = tss.Area1;
                    eq.area2 = tss.Area2;
                    eq.area3 = tss.Area3;
                    eq.area4 = tss.Area4;
                    eq.address = address;
                    eq.pid = randomNum;
                    eq.ip = tss.Ip;
                    eq.name = tss.SceneName;
                    eq.type = IniHelper.findTypesIniTypebyName("场景");
                    eq.objType = "";
                    //eq.range = "";
                    eq.value = "";
                    FileMesege.PointList.scene.Add(eq);
                    //排序
                    ScenesSort(sc);
                    string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                    if (FileMesege.sceneSelectNode.Parent == null)
                    {
                        fullpath = FileMesege.sceneSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, eq.name);

                    }
                    else
                    {
                        fullpath = FileMesege.sceneSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, eq.name);
                    }
                    updateSceneView();
                   
                    
                    break;
                }

            }
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
           
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sceneAdd tss = new sceneAdd();
            //展示居中
            tss.StartPosition = FormStartPosition.CenterParent;
            //获取IP
            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] secenNodeTxt = treeView1.SelectedNode.Text.Split(' ');
            tss.Ip = ips[0];
            //获取场景号数
            tss.Num = Regex.Replace(secenNodeTxt[0], @"[^\d]*", "");
            tss.Xflag = true;
            tss.ShowDialog();
            if (tss.DialogResult == DialogResult.OK)
            {
                //获取场景号
                string num = Convert.ToInt32(tss.Num).ToString("X4");

                //获取IP最后一位
                string address = "FE10" + num;
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //获取该节点IP地址场景下的 场景信息对象
                DataJson.scenes scs =DataListHelper.getSceneInfoList(ips[0],Convert.ToInt32(tss.Oldnum));
                if (scs != null)
                {
                    foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
                    {
                        //修改当前的point点信息
                        if (scs.pid == eq.pid)
                        {
                            scs.id = Convert.ToInt32(tss.Num);
                            eq.area1 = tss.Area1;
                            eq.area2 = tss.Area2;
                            eq.area3 = tss.Area3;
                            eq.area4 = tss.Area4;
                            eq.address = address;
                            //scs.address = address;
                            eq.name = tss.SceneName;
                            string section = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim().Replace(" ", "\\");
                            if (FileMesege.sceneSelectNode.Parent == null)
                            {
                                fullpath = FileMesege.sceneSelectNode.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, eq.name);

                            }
                            else
                            {
                                fullpath = FileMesege.sceneSelectNode.Parent.FullPath + "\\" + string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, eq.name);
                            }
                            break;
                        }

                    }
                    foreach (DataJson.Scene scIP in FileMesege.sceneList)
                    {
                        if (scIP.IP == ips[0])
                        {
                            //排序
                            ScenesSort(scIP);
                            break;
                        }
                    }

                    updateSceneView();
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
            string[] secen = treeView1.SelectedNode.Text.Split(' ');

            foreach (DataJson.Scene sc in FileMesege.sceneList)
            {
                //进入IP同一个
                if (sc.IP == ips[0])
                {
                    foreach (DataJson.scenes scs in sc.scenes)
                    {
                        //当场景号一样
                        if (scs.id.ToString() == Regex.Replace(secen[0], @"[^\d]*", ""))
                        {
                            //撤销
                            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                            int Nodeindex = treeView1.SelectedNode.Index;
                            int pNodeindex = treeView1.SelectedNode.Parent.Index;
                            //移除pointList 中地址
                            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
                            {
                                //获取address与IP地址相同的对象
                                if ( eq.pid == scs.pid)
                                {
                                    //移除Namelist 的对象
                                    FileMesege.PointList.scene.Remove(eq);
                                    break;
                                }
                            }
                            //移除scenelist的对象
                            sc.scenes.Remove(scs);
                            //树状图移除选中节点
                            updateSceneView();
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

        //复制的场景
        List<DataJson.sceneInfo> copyScene = null;
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
            //可能存在克隆现象需要解决
            copyScene = sc.sceneInfo;
            //新建场景
            newTsScene();
            //FileMesege.cmds.RemoveLast();


        }

        //添加场景
        private void btnAddGw_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
            //清除复制的场景
            copyScene = null;
            //新建场景
            newTsScene();
        }

        //删除场景
        private void btnDel_Click(object sender, EventArgs e)
        {
            删除ToolStripMenuItem_Click(this, EventArgs.Empty);
        }


        #endregion


        #region 节点点击和点击后事件  树状图重绘
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
                        //newitemflag = true;
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


        /// <summary>
        /// 选中节点加载DGVscene信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (FileMesege.sceneSelectNode == null)
            {
                addTitleNode();
            }
            else
            {
                if (ToolsUtil.getIP(treeView1.SelectedNode) != ToolsUtil.getIP(FileMesege.sceneSelectNode))
                {
                    addTitleNode();

                }
            }
            FileMesege.sceneSelectNode = treeView1.SelectedNode;
            fullpath = treeView1.SelectedNode.FullPath;
            //DGVSceme添加场景
            dgvsceneAddItem();
           
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                clearTxtShow(Resources.TxtShowScnName + treeView1.SelectedNode.Text + "\r\n");
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1]+".ini";
                clearTxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
            }
           
            
        }

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
                return;
                //foreColor = Color.Lime;//鼠标经过时文字颜色
                //backColor = Color.Gray;//鼠标经过时背景颜色
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

       

        







    }//class
}
