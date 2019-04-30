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
    public delegate void DgvSceneAddItem();

    public partial class ThreeScene : Form
    {

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> TxtShow;
        TsSceneAdd tss ;
        public event DgvSceneAddItem dgvsceneAddItem;
        public event Action updateAllView;
        //判断true为选中父节点
        bool newitemflag = false;
        public ThreeScene()
        {
            InitializeComponent();
        }

        private void ThreeScene_Load(object sender, EventArgs e)
        {
            //ThreeSceneAddNode();
            //TxtShow("场景加载");
            tss = new TsSceneAdd();
            tss.addSceneNode += new AddSceneNode(addSceneNodeDelegate);
        }

        /// <summary>
        /// 初始化添加device名字树状图 根据JsonList文件重新加载子节点
        /// </summary>
        public void ThreeSceneAddNode()
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

            }
            catch { 
                //错误处理
                MessageBox.Show("场景添加节点初始化失败,请检查scene.json文件");
            }
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
            //获取IP
            string [] ips = treeView1.SelectedNode.Text.Split(' ');
            tss.Ip = ips[0];
            //获取场景号数
            tss.Num = (treeView1.SelectedNode.GetNodeCount(false)+1).ToString();
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
            string num = Convert.ToInt32(tss.Num).ToString("X");
            while (num.Length < 4)
            {
                num = num.Insert(0, "0");
            }
            //获取IP最后一位
            string address = SocketUtil.GetIPstyle(tss.Ip, 4) + "10" + num;
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
                    scs.address = address;
                    scs.sceneInfo = new List<DataJson.sceneInfo>();
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
                    eq.type = "4.0_scene";
                    eq.objType = "";
                    //eq.range = "";
                    eq.value = "";
                    FileMesege.PointList.scene.Add(eq);

                    //排序
                    ScenesSort(sc);
                    string parentNodePath = "";
                    if (treeView1.SelectedNode != null)
                    {
                        parentNodePath = treeView1.SelectedNode.FullPath;
                    }
                    updateAllView();
                    if (FileMesege.sceneSelectNode != null)
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

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TsSceneAdd tss = new TsSceneAdd();
            //展示居中
            tss.StartPosition = FormStartPosition.CenterParent;
            //获取IP
            string[] ips = treeView1.SelectedNode.Parent.Text.Split(' ');
            string[] secen = treeView1.SelectedNode.Text.Split(' ');
            tss.Ip = ips[0];
            //获取场景号数
            tss.Num = Regex.Replace(secen[0], @"[^\d]*", "");
            tss.Xflag = true;
            tss.ShowDialog();
            if (tss.DialogResult == DialogResult.OK)
            {
                //获取场景号
                string num = Convert.ToInt32(tss.Num).ToString("X");
                while (num.Length < 4)
                {
                    num = num.Insert(0, "0");
                }
                //获取IP最后一位
                string address = SocketUtil.GetIPstyle(tss.Ip, 4) + "10" + num;
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
                            scs.address = address;
                            eq.name = tss.SceneName;
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
                    updateAllView();
                }            
               
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
           
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
                                updateAllView();
                                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                                FileMesege.cmds.DoNewCommand(NewList, OldList);
                                return;
                            }
                        }



                    }

                }//IP FOREACH


          
            
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

             FileMesege.sceneSelectNode = treeView1.SelectedNode;
            //DGVSceme添加场景
            dgvsceneAddItem();
            string[] names = treeView1.SelectedNode.Text.Split(' ');
            if (treeView1.SelectedNode.Parent != null)
            {

                TxtShow(Resources.TxtShowScnName + treeView1.SelectedNode.Text + "\r\n");
            }
            else
            {
                string filepath = Application.StartupPath + "\\devices\\" + names[1]+".ini";
                TxtShow(Resources.TxtShowDevName + IniConfig.GetValue(filepath, "define", "note") + "\r\n");
            }
           
            
        }

        //选中节点高亮不消失
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(204, 235, 248)), e.Bounds);
                e.Graphics.DrawString(e.Node.Text, treeView1.Font, new SolidBrush(Color.Black), e.Bounds.Location);
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        #endregion







    }//class
}
