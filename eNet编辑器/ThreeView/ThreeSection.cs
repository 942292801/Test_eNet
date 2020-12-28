using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eNet编辑器.AddForm;
using System.Reflection;

namespace eNet编辑器.ThreeView
{
    public delegate void AddTitleNode();
    //public delegate void AddSectionDevCursor();
    //public delegate void AddSectionNameCursor();
  
    public partial class ThreeSection : Form
    {
        
        /// <summary>
        /// 存放新增的区域
        /// </summary>
        string sectionname = "";
        bool newflag = false;
        tsSection tss;
        public ThreeSection()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
     
           
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

        public event AddTitleNode addTitleNode;
        //public event AddSectionDevCursor addSectionDevCursor;
        //public event AddSectionNameCursor addSectionNameCursor;
   
        //按窗口类型来更新窗口
        public event Action sectionUpdateTreeByFormType;
        public event Action updatePointDgv;

        public event Action logicCbSceneGetItem;
        //取消选中点位
        //public event Action unSelectPointNode;

        //树状图节点
        string fullpath = "";

        private void ThreeSection_Load(object sender, EventArgs e)
        {
            //新建
            tss = new tsSection();
            tss.addNode += new AddNode(addNodeDelegate);
        }

        public void ThreeSEctionAddNode()
        {
            TreeMesege tm = new TreeMesege();
            
            if (FileMesege.AreaList == null)
            {
                treeView1.Nodes.Clear();
                tm.AddNode1(treeView1, "查看所有区域");
                return;
            }
            //记录当前节点展开状况
            List<string> isExpands = tm.treeIsExpandsState(treeView1);
            //循环区域一 查看有没 未定义区域 没有就新建
            bool isExitNode = false;
            foreach (DataJson.Area1 a1 in FileMesege.AreaList)
            {
                if (a1.area == "未定义区域")
                {
                    isExitNode = true;
                }
            }
            if (!isExitNode)
            {
                sectionname = "未定义区域";
                DataJson.Area1 a1 = new DataJson.Area1();
                a1.area = sectionname;
                a1.id1 = FileMesege.AreaList.Count.ToString();
                a1.id2 = "";
                a1.id3 = "";
                a1.id4 = "";
                a1.area2 = new List<DataJson.Area2>();
                FileMesege.AreaList.Add(a1);
            }
            //区域一
            foreach (DataJson.Area1 a1 in FileMesege.AreaList)
            {
                int index = tm.AddNode1(treeView1, a1.area);

                //区域二
                foreach (DataJson.Area2 a2 in a1.area2)
                {
                    int index2 = tm.AddNode2(treeView1, a2.area, index);
                    //区域三
                    foreach (DataJson.Area3 a3 in a2.area3)
                    {
                        int index3 = tm.AddNode3(treeView1, a3.area, index, index2);
                        //区域四
                        foreach (DataJson.Area4 a4 in a3.area4)
                        {
                            int index4 = tm.AddNode4(treeView1, a4.area, index, index2, index3);
                        }
                    }

                }

            }
            //展开记录的节点
            tm.treeIspandsStateRcv(treeView1, isExpands);
            tm.AddNode1(treeView1, "查看所有区域");
            TreeMesege.SetPrevVisitNode(treeView1,fullpath);
            
        }

        /// <summary>
        /// 取消节点选中
        /// </summary>
        public void unSelectNode()
        {
            treeView1.SelectedNode = null;
        }

   

        #region 新建 修改 删除 展开（收起）节点

  

        private void 新建节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAddChild = false;
            //把窗口向屏幕中间刷新
            tss.StartPosition = FormStartPosition.CenterParent;
            tss.Newflag = newflag;
            //设定开始层级
            int i = 0;
            if (treeView1.SelectedNode != null)
            {
                i = treeView1.SelectedNode.Level;

            }
            if (i > 3)
            {
                return;
            }
            addSelectNode = FileMesege.sectionNode;
            tss.LbText = "添加节点";
            tss.Selectindex = i;
            tss.ShowDialog();
            
        }

        private void 添加子节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Text == "查看所有区域" || treeView1.SelectedNode.Text == "未定义区域")
            {
                return;
            }
            isAddChild = true;
            newflag = true;
            newTsSection();
        }

        /// <summary>
        /// 新建节点弹框获取信息
        /// </summary>
        /// <returns></returns>
        private void newTsSection()
        {
            tss.LbText = "添加子节点";
            //把窗口向屏幕中间刷新
            tss.StartPosition = FormStartPosition.CenterParent;
            tss.Newflag = true;
            //设定开始层级
            int i = 0;
            if (treeView1.SelectedNode != null)
            {
                i = treeView1.SelectedNode.Level + 1;
                if (treeView1.SelectedNode.Text == "查看所有区域" || treeView1.SelectedNode.Text == "未定义区域")
                {
                    isAddChild = false;
                    i = 0;
                }
            }
            if (i > 3)
            {
                return;
            }
            addSelectNode = FileMesege.sectionNode;

            tss.Selectindex = i;
            tss.ShowDialog();

        }

        //选中的节点
        TreeNode addSelectNode = null;
        //新建节点
        public void addNodeDelegate()
        {
            try
            {
                sectionname = FileMesege.info;
                if (sectionname == "查看所有区域" || sectionname == "未定义区域")
                {
                    return;
                }
                int id = -1;
                string[] nums = null;
                if (newflag == false)
                {
                    //新建节点 
                    id = area1();
                    fullpath = treeView1.Nodes[id].FullPath;
                }
                else
                {
                    TreeMesege tm = new TreeMesege();
                    if (isAddChild)
                    {
                        if (addSelectNode != null )
                        {
                            if (addSelectNode.Text == "查看所有区域" || addSelectNode.Text == "未定义区域")
                            {
                                return;

                            }
                        }

                        //添加子节点
                        nums = tm.GetNodeNum(addSelectNode).Split(' ');
                        switch (nums.Length)
                        {
                            case 4:
                                //area1();
                                break;
                            case 1:
                                id = area2(nums[0]);
                                fullpath = treeView1.Nodes[Convert.ToInt32(nums[0])].Nodes[id].FullPath;
                                break;
                            case 2:
                                id = area3(nums[0], nums[1]);
                                fullpath = treeView1.Nodes[Convert.ToInt32(nums[0])].Nodes[Convert.ToInt32(nums[1])].Nodes[id].FullPath;

                                break;
                            case 3:
                                id = area4(nums[0], nums[1], nums[2]);
                                fullpath = treeView1.Nodes[Convert.ToInt32(nums[0])].Nodes[Convert.ToInt32(nums[1])].Nodes[Convert.ToInt32(nums[2])].Nodes[id].FullPath;

                                break;
                            default:
                                id = area1();
                                fullpath = treeView1.Nodes[id].FullPath;
                                break;
                        }

                    }
                    else
                    {
                        //添加节点
                        nums = tm.GetNodeNum(addSelectNode).Split(' ');
                        switch (nums.Length)
                        {
                            case 4:
                                id = area4(nums[0], nums[1], nums[2]);
                                fullpath = treeView1.Nodes[Convert.ToInt32(nums[0])].Nodes[Convert.ToInt32(nums[1])].Nodes[Convert.ToInt32(nums[2])].Nodes[id].FullPath;

                                break;
                            case 1:
                                id = area1();
                                fullpath = treeView1.Nodes[id].FullPath;
                                break;
                            case 2:
                                id = area2(nums[0]);
                                fullpath = treeView1.Nodes[Convert.ToInt32(nums[0])].Nodes[id].FullPath;
                                break;
                            case 3:
                                id = area3(nums[0], nums[1]);
                                fullpath = treeView1.Nodes[Convert.ToInt32(nums[0])].Nodes[Convert.ToInt32(nums[1])].Nodes[id].FullPath;


                                break;
                            default:
                                break;
                        }
                    }

                }


                ThreeSEctionAddNode();
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
            }


        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Text == "查看所有区域" || treeView1.SelectedNode.Text == "未定义区域")
            {
                return;
            }
            if (newflag == false)
            {
                return;
            }
            else
            {
                tsChange tsc = new tsChange();
                //把窗口向屏幕中间刷新
                tsc.StartPosition = FormStartPosition.CenterParent;
                FileMesege.info = treeView1.SelectedNode.Text;
                tsc.ShowDialog();
                if (tsc.DialogResult == DialogResult.OK)
                {
                    //获取修改后的名字                       
                    TreeMesege tm = new TreeMesege();
                    string[] nums = tm.GetNodeNum(treeView1.SelectedNode).Split(' ');
                    string[] section = null;
                    switch (nums.Length)
                    {
                        case 4:
                            section = tm.GetSection(nums[0], nums[1], nums[2], nums[3]).Split('\\');
                            AreaListChange1(section,nums[0], nums[1], nums[2], nums[3]);
                                
                            break;
                        case 1:
                            section = tm.GetSection(nums[0], "", "", "").Split('\\');
                            AreaListChange1(section, nums[0]);
                            break;
                        case 2:
                            section = tm.GetSection(nums[0], nums[1], "", "").Split('\\');
                            AreaListChange1(section, nums[0], nums[1]);
                            break;
                        case 3:
                            section = tm.GetSection(nums[0], nums[1], nums[2], "").Split('\\');
                            AreaListChange1(section, nums[0], nums[1], nums[2]);
                            break;
                        default: break;
                    }
                    sectionUpdateTreeByFormType();
                    updatePointDgv();

                }
                    
            }


           
        }

        #region  修改AreaList表中数据
        /// <summary>
        /// 保存到AreaList中
        /// </summary>
        private void AreaListChange1(string[] section, string index1)
        {
            int i1 = Convert.ToInt32(index1);
            string newSection = FileMesege.info;

            foreach (DataJson.Area1 a1 in FileMesege.AreaList)
            {
                if (a1.area == newSection)
                {
                    MessageBox.Show("修改失败！该名称已存在\r\n");
                    return;
                }
            }
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
         
            //修改PointList信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                pointChange1(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                pointChange1(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                pointChange1(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                pointChange1(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                pointChange1(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                pointChange1(eq, section, newSection);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                pointChange1(eq, section, newSection);
            }*/
            FileMesege.AreaList[i1].area = newSection;
            treeView1.SelectedNode.Text = newSection;
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        private void pointChange1(DataJson.PointInfo eq,string[] section,string newSection)
        {
            if (eq.area1 == section[0])
            {
                eq.area1 = newSection;
            }
        }

        private void AreaListChange1(string[] section, string index1, string index2)
        {
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            string newSection = FileMesege.info;
            foreach (DataJson.Area2 a2 in FileMesege.AreaList[i1].area2)
            {
                if (a2.area == newSection)
                {
                    MessageBox.Show("修改失败！该名称已存在\r\n");
                    return;
                }
            }
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
      
            //修改PointList信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                pointChange2(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                pointChange2(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                pointChange2(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                pointChange2(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                pointChange2(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                pointChange2(eq, section, newSection);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                pointChange2(eq, section, newSection);
            }*/
            FileMesege.AreaList[i1].area2[i2].area = newSection;
            treeView1.SelectedNode.Text = newSection;
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        private void pointChange2(DataJson.PointInfo eq, string[] section, string newSection)
        {
            if (eq.area1 == section[0] && eq.area2 == section[1])
            {
                eq.area2 = newSection;
            }
        }

        private void AreaListChange1(string[] section, string index1, string index2, string index3)
        {

            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            string newSection = FileMesege.info;
            foreach (DataJson.Area3 a3 in FileMesege.AreaList[i1].area2[i2].area3)
            {
                if (a3.area == newSection)
                {
                    MessageBox.Show("修改失败！该名称已存在\r\n");
                    return;
                }
            }

            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
    
            //修改PointList信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                pointChange3(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                pointChange3(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                pointChange3(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                pointChange3(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                pointChange3(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                pointChange3(eq, section, newSection);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                pointChange3(eq, section, newSection);
            }*/
            FileMesege.AreaList[i1].area2[i2].area3[i3].area = newSection;
            treeView1.SelectedNode.Text = newSection;
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

        private void pointChange3(DataJson.PointInfo eq, string[] section, string newSection)
        {
            if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2])
            {
                eq.area3 = newSection;
            }
        }

        private void AreaListChange1(string[] section, string index1, string index2, string index3, string index4)
        {
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            int i4 = Convert.ToInt32(index4);
            string newSection = FileMesege.info;
            foreach (DataJson.Area4 a4 in FileMesege.AreaList[i1].area2[i2].area3[i3].area4)
            {
                if (a4.area == newSection)
                {
                    MessageBox.Show("修改失败！该名称已存在\r\n");
                    return;
                }
            }

            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
          
            //修改PointList信息  //、、、、、、、、、、、、、、、、、、、、、、、、后续还要添加
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                pointChange4(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                pointChange4(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                pointChange4(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                pointChange4(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                pointChange4(eq, section, newSection);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                pointChange4(eq, section, newSection);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                pointChange4(eq, section, newSection);
            }*/
            FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].area = newSection;
            treeView1.SelectedNode.Text = newSection;
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }


        private void pointChange4(DataJson.PointInfo eq, string[] section, string newSection)
        {
            if (eq.area1 == section[0] && eq.area2 == section[1] && eq.area3 == section[2] && eq.area4 == section[3])
            {
                eq.area4 = newSection;
            }
        }
        #endregion

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && FileMesege.sectionNode.Text != "查看所有区域" && FileMesege.sectionNode.Text != "未定义区域")
            {
              
                //右击树状图外面区域
                if (newflag == false)
                {
                    return;
                }
                else//右击树状图区域
                {

                    if (deleteNode(treeView1.SelectedNode))
                    {
                        //选中上一个可视节点
                        fullpath = "";
                        if (FileMesege.sectionNode != null && FileMesege.sectionNode.PrevVisibleNode != null)
                        {

                            fullpath = FileMesege.sectionNode.PrevVisibleNode.FullPath;

                        }
                        ThreeSEctionAddNode();
                    }
                   
                    
                }
            }
        }

        #region 删除AreaList表中数据
        /// <summary>
        /// json删除节点 后面数据自动排序
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool deleteNode(TreeNode node)
        { 
            TreeMesege tm = new TreeMesege();
            string[] nums = tm.GetNodeNum(node).Split(' ');
            string section = null;
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
         
            switch (nums.Length)
            {
                case 4:
                    section = tm.GetSection(nums[0], nums[1], nums[2], nums[3]);
                    AreaListDelect(section,nums[0], nums[1], nums[2], nums[3],true);

                    break;
                case 1:

                    section = tm.GetSection(nums[0],"","","");
                    AreaListDelect(section,nums[0], true);
                    break;
                case 2:
                    section = tm.GetSection(nums[0], nums[1], "","");
                    AreaListDelect(section,nums[0], nums[1], true);
                    break;
                case 3:
                    section = tm.GetSection(nums[0], nums[1], nums[2], "");
                    AreaListDelect(section,nums[0], nums[1], nums[2], true);
                    break;
                default: return false;
            }
            sectionUpdateTreeByFormType();
            updatePointDgv();
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return true;
        }

        /// <summary>
        /// 删除节点 修改AreaList节点信息和NameList受影响的设备端口信息
        /// </summary>
        /// <param name="index1">节点号</param>
        /// <param name="index2"></param>
        /// <param name="index3"></param>
        /// <param name="index4"></param>
        /// <param name="objDel">是否修改NameList的信息 true为正常  false为拖拽不修改</param>
        /// <returns></returns>
        private DataJson.Area4 AreaListDelect(string section,string index1, string index2, string index3, string index4,bool objDel)
        {
            //四个位置信息  例==>一 二 “” “”
            string[] sections = section.Split('\\');
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            int i4 = Convert.ToInt32(index4);
            //删除节点的ID4
            int id4 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id4);
            int id3 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id3);
            int id2 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id2);
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id1);
            int oldid4= 0;
            foreach (DataJson.Area4 a4 in FileMesege.AreaList[i1].area2[i2].area3[i3].area4)
            {
                ////当前设备端口的id4
                oldid4 = Convert.ToInt32(a4.id4);
                if ( oldid4 > id4)
                {
                    
                    //后头的id4减1 往前挪
                    a4.id4 = (oldid4 - 1).ToString();
                }
            }
            //删除NameList的设备地域信息 或更新信息  、、、、、、、、、、、、、、、后续还需添加
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                delupdatePoint4(eq,sections,objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                delupdatePoint4(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                delupdatePoint4(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                delupdatePoint4(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                delupdatePoint4(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                delupdatePoint4(eq, sections, objDel);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                delupdatePoint4(eq, sections, objDel);
            }*/
            //删除的节点所有信息
            DataJson.Area4 obj = FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4];
            FileMesege.AreaList[i1].area2[i2].area3[i3].area4.Remove( FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4]);
            return obj;
             
           
        }

        private void delupdatePoint4(DataJson.PointInfo eq, string[] sections,bool objDel)
        {
            //是否为节点同一个父节点  前面3个ID相等
            if (eq.area1 == sections[0] && eq.area2 == sections[1] && eq.area3 == sections[2])
            {
                //是否为当前删除节点
                if (eq.area4 == sections[3])
                {

                    if (objDel)
                    {
                        //当前删除节点的信息清除
                        eq.area1 = "";
                        eq.area2 = "";
                        eq.area3 = "";
                        eq.area4 = "";
                    }
                    else
                    {
                        //拖拽标记
                        eq.area1 = "del";
                        eq.area2 = "del";
                        eq.area3 = "del";
                        eq.area4 = "del";
                    }
                }

            }
        }

        private DataJson.Area3 AreaListDelect(string section, string index1, string index2, string index3, bool objDel)
        {
            //四个位置信息  例==>一 二 “” “”
            string[] sections = section.Split('\\');
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            int id3 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].id3);
            int id2 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].id2);
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].id1);
            int oldid3 = 0;
            string IDnum= "";
            foreach (DataJson.Area3 a3 in FileMesege.AreaList[i1].area2[i2].area3)
            {
                oldid3 = Convert.ToInt32(a3.id3);
                if (oldid3 > id3)
                {
                    //把区域3的ID3全部改变
                    IDnum = (oldid3 - 1).ToString();
                    a3.id3 = IDnum;
                    //循环该区域3的区域子节点 修改id3
                    foreach (DataJson.Area4 a4 in a3.area4)
                    {
                        a4.id3 = IDnum;
                    }
                }
            }
            //删除NameList的设备地域信息 或更新信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                delupdatePoint3(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                delupdatePoint3(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer )
            {
                delupdatePoint3(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                delupdatePoint3(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                delupdatePoint3(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                delupdatePoint3(eq, sections, objDel);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                delupdatePoint3(eq, sections, objDel);
            }*/
            //删除的节点所有信息
            DataJson.Area3 obj = FileMesege.AreaList[i1].area2[i2].area3[i3];
            FileMesege.AreaList[i1].area2[i2].area3.Remove(FileMesege.AreaList[i1].area2[i2].area3[i3]);
            return obj;
            
        }

        private void delupdatePoint3(DataJson.PointInfo eq, string[] sections, bool objDel)
        {
            //是否为节点同一个父节点  前面2个ID相等
            if (eq.area1 == sections[0] && eq.area2 == sections[1])
            {
                //是否为当前节点
                if (eq.area3 == sections[2])
                {

                    if (objDel)
                    {
                        //当前删除节点的信息清除
                        eq.area1 = "";
                        eq.area2 = "";
                        eq.area3 = "";
                        eq.area4 = "";
                    }
                    else
                    {
                        //拖拽标记
                        eq.area1 = "del";
                        eq.area2 = "del";
                        eq.area3 = "del";
                        //eq.area4 = "del";
                    }
                }

            }
        }

        private DataJson.Area2 AreaListDelect(string section, string index1, string index2, bool objDel)
        {
            //四个位置信息  例==>一 二 “” “”
            string[] sections = section.Split('\\');
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int id2 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].id2);
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].id1);
            int oldid2 = 0;
            string IDnum = "";
            foreach (DataJson.Area2 a2 in FileMesege.AreaList[i1].area2)
            {
                oldid2 = Convert.ToInt32(a2.id2);
                if (oldid2 > id2)
                {
                    //把区域2的ID2全部改变
                    IDnum = (oldid2 - 1).ToString();
                    a2.id2 = IDnum;
                    //循环该区域2的区域子节点 修改id2
                    foreach (DataJson.Area3 a3 in a2.area3)
                    {
                        a3.id2 = IDnum;
                        //循环该区域3的区域子节点 修改id2
                        foreach (DataJson.Area4 a4 in a3.area4)
                        {
                            a4.id2 = IDnum;
                        }
                    }
                }
            }
            //删除NameList的设备地域信息 或更新信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                delupdatePoint2(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                delupdatePoint2(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                delupdatePoint2(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                delupdatePoint2(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                delupdatePoint2(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                delupdatePoint2(eq, sections, objDel);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                delupdatePoint2(eq, sections, objDel);
            }*/
            //删除的节点所有信息
            DataJson.Area2 obj = FileMesege.AreaList[i1].area2[i2];
            FileMesege.AreaList[i1].area2.Remove(FileMesege.AreaList[i1].area2[i2]);
            return obj;
             
        }

        private void delupdatePoint2(DataJson.PointInfo eq, string[] sections, bool objDel)
        {
            //是否为节点同一个父节点  前面1个ID相等
            if (eq.area1 == sections[0])
            {
                //是否为当前节点
                if (eq.area2 == sections[1])
                {

                    if (objDel)
                    {
                        //当前删除节点的信息清除
                        eq.area1 = "";
                        eq.area2 = "";
                        eq.area3 = "";
                        eq.area4 = "";
                    }
                    else
                    {
                        //拖拽标记
                        eq.area1 = "del";
                        eq.area2 = "del";
                        //eq.area3 = "del";
                        //eq.area4 = "del";
                    }
                }

            }
        }

        private DataJson.Area1 AreaListDelect(string section, string index1, bool objDel)
        {
            
            //四个位置信息  例==>一 二 “” “”
            string[] sections = section.Split('\\');
            int i1 = Convert.ToInt32(index1);
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].id1);
            int oldid1 = 0;
            string IDnum = "";
            foreach (DataJson.Area1 a1 in FileMesege.AreaList)
            {
                oldid1 = Convert.ToInt32(a1.id1);
                if (oldid1 > id1)
                {
                    //把区域1的ID1全部改变
                    IDnum = (oldid1 - 1).ToString();
                    a1.id1 = IDnum;
                    //循环该区域1的区域子节点 修改id1
                    foreach (DataJson.Area2 a2 in a1.area2)
                    {
                        a2.id1 = IDnum;
                        
                        foreach (DataJson.Area3 a3 in a2.area3)
                        {
                            a3.id1 = IDnum;
                            //循环该区域3的区域子节点 修改id2
                            foreach (DataJson.Area4 a4 in a3.area4)
                            {
                                a4.id1 = IDnum;
                            }
                        }

                    }
                }//if
            }
            //删除NameList的设备地域信息 或更新信息
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                delupdatePoint1(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                delupdatePoint1(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                delupdatePoint1(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                delupdatePoint1(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                delupdatePoint1(eq, sections, objDel);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                delupdatePoint1(eq, sections, objDel);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                delupdatePoint1(eq, sections, objDel);
            }*/
            //删除的节点所有信息
            DataJson.Area1 obj = FileMesege.AreaList[i1];
            FileMesege.AreaList.Remove(FileMesege.AreaList[i1]);
            return obj;
            
        }

        private void delupdatePoint1(DataJson.PointInfo eq, string[] sections, bool objDel)
        {
            //是否为节点同一个父节点  前面1个ID相等
            if (eq.area1 == sections[0])
            {

                if (objDel)
                {
                    //当前删除节点的信息清除
                    eq.area1 = "";
                    eq.area2 = "";
                    eq.area3 = "";
                    eq.area4 = "";
                }
                else
                {
                    //拖拽标记
                    eq.area1 = "del";
                    //eq.area2 = "del";
                    //eq.area3 = "del";
                    //eq.area4 = "del";
                }
            }
        }


        #endregion

        private void 展开所有节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void 收起所有节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
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
                newflag = false;
                if (CurrentNode != null)
                {
                    treeView1.SelectedNode = CurrentNode;//选中这个节点 
                    newflag = true;
                    
                }
            }
        }

    

        /// <summary>
        /// 添加新的第一级节点
        /// </summary>
        /// <returns></returns>
        private int area1()
        {
            //新建节点 FileMesege.AreaList
            if (FileMesege.AreaList == null)
            {
                FileMesege.AreaList = new List<DataJson.Area1>();
            }
            foreach (DataJson.Area1 area1 in FileMesege.AreaList)
            {
                if (area1.area == sectionname)
                {
                    return -1;
                }
            }
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            if (treeView1.Nodes.Count != 0)
            {
                //把最后面的节点（所有区域） 删除掉
                treeView1.Nodes[treeView1.Nodes.Count - 1].Remove();
            }
            
            TreeMesege tm = new TreeMesege();
            DataJson.Area1 a1 = new DataJson.Area1();
            int id = tm.AddNode1(treeView1, sectionname);
            a1.area = sectionname;
            if (id == 0)
            {
                a1.id1 = id.ToString();
            }
            else
            {
                //当前ID为第四级 最后新增点的ID号
                id = Convert.ToInt32(FileMesege.AreaList[id - 1].id1) + 1;
                a1.id1 = id.ToString();
            }
            a1.id2 = "";
            a1.id3 = "";
            a1.id4 = "";
            a1.area2 = new List<DataJson.Area2>();
            FileMesege.AreaList.Add(a1);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return id;
        }

        /// <summary>
        /// 添加新的第二级节点
        /// </summary>
        /// <param name="index1">第一级节点的索引号</param>
        /// <returns></returns>
        private int area2(string index1)
        {
            int i1 = Convert.ToInt32(index1);
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
           
            //新建节点 FileMesege.AreaList
            foreach (DataJson.Area2 area2 in FileMesege.AreaList[i1].area2)
            {
                if (area2.area == sectionname)
                {
                    return -1;
                }
            }
            TreeMesege tm = new TreeMesege();
            int id = tm.AddNode2(treeView1, sectionname,i1);
            
            DataJson.Area2 a2 = new DataJson.Area2();
            a2.area = sectionname;
            if (id == 0)
            {
                a2.id2 = id.ToString();
            }
            else
            {
                //当前ID为第四级 最后新增点的ID号
                id = Convert.ToInt32(FileMesege.AreaList[i1].area2[id - 1].id2) + 1;
                a2.id2 = id.ToString();
            }
            a2.id1 = index1;
            
            a2.id3 = "";
            a2.id4 = "";
            a2.area3 = new List<DataJson.Area3>();
            FileMesege.AreaList[i1].area2.Add(a2);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return id;
        }

        /// <summary>
        /// 添加新的第三级节点
        /// </summary>
        /// <param name="index1">第一级节点的索引号</param>
        /// <param name="index2">第二级节点的索引号</param>
        /// <returns></returns>
        private int area3(string index1, string index2)
        {
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
          
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            //新建节点 FileMesege.AreaList
            foreach (DataJson.Area3 area3 in FileMesege.AreaList[i1].area2[i2].area3)
            {
                if (area3.area == sectionname)
                {
                    return -1;
                }
            }
            TreeMesege tm = new TreeMesege();
            int id = tm.AddNode3(treeView1, sectionname,i1,i2);
            DataJson.Area3 a3 = new DataJson.Area3();
            a3.area = sectionname;
            if (id == 0)
            {
                a3.id3 = id.ToString();
            }
            else
            {
                //当前ID为第四级 最后新增点的ID号
                id = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[id - 1].id3) + 1;
                a3.id3 = id.ToString();
            }
            a3.id1 = index1;
            a3.id2 = index2;
           
            a3.id4 = "";
            a3.area4 = new List<DataJson.Area4>();
            FileMesege.AreaList[i1].area2[i2].area3.Add(a3);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return id;
        }

        /// <summary>
        /// 添加新的第四级节点
        /// </summary>
        /// <param name="index1">第一级节点的索引号</param>
        /// <param name="index2">第二级节点的索引号</param>
        /// <param name="index3">第三级节点的索引号</param>
        /// <returns></returns>
        private int area4(string index1, string index2, string index3)
        {
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
     
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            //新建节点 FileMesege.AreaList
            foreach (DataJson.Area4 area4 in FileMesege.AreaList[i1].area2[i2].area3[i3].area4)
            {
                if (area4.area == sectionname)
                {
                    return -1;
                }
            }
            TreeMesege tm = new TreeMesege();
            //新增后的最后一位ID号
            int id = tm.AddNode4(treeView1, sectionname, i1, i2,i3);
            DataJson.Area4 a4 = new DataJson.Area4();
            if (id == 0)
            {          
                a4.id4 = id.ToString();                
            }
            else
            { 
                //当前ID为第四级 最后新增点的ID号
                id = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[id - 1].id4) + 1;               
                a4.id4 = id.ToString();
            }
            a4.area = sectionname;
            a4.id1 = index1;
            a4.id2 = index2;
            a4.id3 = index3;
            FileMesege.AreaList[i1].area2[i2].area3[i3].area4.Add(a4);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            return id;
        }

        #endregion


        #region 选中节点  树状图重绘
        //选中节点后发生的事件
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //把选中节点保存至文档中 
            FileMesege.sectionNode = treeView1.SelectedNode;
            TreeMesege tm = new TreeMesege();
            FileMesege.sectionNodeCopy = tm.GetSectionByNode(treeView1.SelectedNode);
            fullpath = treeView1.SelectedNode.FullPath;
            if (FileMesege.formType == "point")
            {
                //unSelectPointNode();
                //刷PointDGV
                updatePointDgv();
            }
            if (FileMesege.formType == "name" && FileMesege.cbTypeIndex == 0)
            {
                //刷title新树状图
                addTitleNode();
            }

            if (FileMesege.formType == "scene" || FileMesege.formType == "timer" || FileMesege.formType == "panel" || FileMesege.formType == "sensor" || FileMesege.formType == "logic")
            {
                //刷title新树状图
                addTitleNode();
            }
            
            if (FileMesege.formType == "logic")
            {
                //逻辑的场景cb框内容 按区域搜索
                logicCbSceneGetItem();
            }

            //修改光标为加号
            //addSectionDevCursor();
            //addSectionNameCursor();
            
        }

        //节点蓝色高亮
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

        #region 树状图拖拽节点

        Point Position = new Point(0, 0);
        //当用户开始拖动节点时发生
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
         
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        //在将对象拖入控件的边界时发生
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            
            //判断拖动的是否为树节点
            if (e.Data.GetDataPresent(typeof(TreeNode)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            
            TreeNode myNode = null;
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                myNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            }
            else
            {
                MessageBox.Show("error");
            }
            if (myNode != null )
            {
                if (myNode.Text == "查看所有区域" || myNode.Text == "未定义区域")
                {
                    return;
                }
            }
            Position.X = e.X;
            Position.Y = e.Y;
            Position = treeView1.PointToClient(Position);
            //目标节点
            TreeNode DropNode = this.treeView1.GetNodeAt(Position);
            if (DropNode != null)
            {
                if (DropNode.Text == "查看所有区域" || DropNode.Text == "未定义区域")
                {
                    return;
                }
            }
            // 1.目标节点不是空。2.目标节点不是被拖拽接点的字节点。3.目标节点不是被拖拽节点本身
            if (DropNode != null && DropNode.Parent != myNode && DropNode != myNode)
            {
                //同级才能操作
                if ( DropNode.Level == myNode.Level )
                {
                    TreeMesege tm = new TreeMesege();
                    //拖拽节点
                    TreeNode DragNode = myNode;

                    string[] targetnum = tm.GetNodeNum(DropNode).Split(' ');
                    
                    string[] nums = tm.GetNodeNum(myNode).Split(' ');
                    string section = null;
                    string[] target = null;
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                    switch (nums.Length)
                    {
                        case 4:
                            if (DragNode.Parent != DropNode.Parent)
                            {
                                //判断节点名称是否已经存在
                                foreach (DataJson.Area4 area4 in FileMesege.AreaList[Convert.ToInt32(targetnum[0])].area2[Convert.ToInt32(targetnum[1])].area3[Convert.ToInt32(targetnum[2])].area4)
                                {
                                    if (area4.area == DragNode.Text)
                                    {
                                        return;
                                    }
                                }
                            }
                            //删除 并重新排序List信息
                            section = tm.GetSection(nums[0], nums[1], nums[2], nums[3]);
                            //返回的obj为被删除的那个节点
                            DataJson.Area4 obj4 = AreaListDelect(section,nums[0], nums[1], nums[2], nums[3], false);
                            // 将被拖拽节点从原来位置删除。
                            myNode.Remove();
                            target = tm.GetNodeNum(DropNode).Split(' ');
                            //插入 再重新排序list信息
                            AreaListUpdata(target[0], target[1], target[2], target[3], obj4);

                            break;
                        case 1:
                            if (DragNode.Parent != DropNode.Parent)
                            {
                                //判断节点名称是否已经存在
                                foreach (DataJson.Area1 area1 in FileMesege.AreaList)
                                {
                                    if (area1.area == DragNode.Text)
                                    {

                                        return;
                                    }
                                }
                            }
                            section = tm.GetSection(nums[0], "", "", "");
                            DataJson.Area1 obj1 = AreaListDelect(section, nums[0], false);
                            // 将被拖拽节点从原来位置删除。
                            myNode.Remove();
                            target = tm.GetNodeNum(DropNode).Split(' ');
                            //插入 再重新排序list信息
                            AreaListUpdata(target[0], obj1);
                            break;
                        case 2:
                            if (DragNode.Parent != DropNode.Parent)
                            {
                                //判断节点名称是否已经存在
                                foreach (DataJson.Area2 area2 in FileMesege.AreaList[Convert.ToInt32(targetnum[0])].area2)
                                {
                                    if (area2.area == DragNode.Text)
                                    {

                                        return;
                                    }
                                }
                            }
                            section = tm.GetSection(nums[0], nums[1], "","");
                            DataJson.Area2 obj2 = AreaListDelect(section, nums[0], nums[1], false);
                            // 将被拖拽节点从原来位置删除。
                            myNode.Remove();
                            target = tm.GetNodeNum(DropNode).Split(' ');
                            //插入 再重新排序list信息
                            AreaListUpdata(target[0], target[1], obj2);
                            break;
                        case 3:
                            if (DragNode.Parent != DropNode.Parent)
                            {
                                //判断节点名称是否已经存在
                                foreach (DataJson.Area3 area3 in FileMesege.AreaList[Convert.ToInt32(targetnum[0])].area2[Convert.ToInt32(targetnum[1])].area3)
                                {
                                    if (area3.area == DragNode.Text)
                                    {

                                        return;
                                    }
                                }
                            }
                            section = tm.GetSection(nums[0], nums[1], nums[2], "");
                            DataJson.Area3 obj3 = AreaListDelect(section, nums[0], nums[1], nums[2], false);
                            // 将被拖拽节点从原来位置删除。
                            myNode.Remove();
                            target = tm.GetNodeNum(DropNode).Split(' ');
                            //插入 再重新排序list信息
                            AreaListUpdata(target[0], target[1], target[2], obj3);
                            break;
                        default: return ;
                    }
                   
                   
                    if (DropNode.Level == 0)
                    {
                        //第一层节点
                        treeView1.Nodes.Insert(DropNode.Index, DragNode);
                    }
                    else
                    {
                        DropNode.Parent.Nodes.Insert(DropNode.Index, DragNode);
                    }
                   
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                    //刷新窗口
                    sectionUpdateTreeByFormType();
                 
                    
                   
                    
                }
                
               
            }
            /* 如果目标节点不存在，即拖拽的位置不存在节点，那么就将被拖拽节点放在根节点之下
            if (DropNode == null)
            {
                TreeNode DragNode = myNode;
                myNode.Remove();
                treeView1.Nodes.Add(DragNode);
            }*/
        }

        //拖拽节点 修改AreaList
        private void AreaListUpdata(string index1, string index2, string index3, string index4, DataJson.Area4 obj)
        {   
            //索引号
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            int i4 = Convert.ToInt32(index4);
            //目标id 1-4
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id1);
            int id2 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id2);
            int id3 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id3);
            int id4 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].id4);

            //插入到target目标 List下面 AreaList
            FileMesege.AreaList[i1].area2[i2].area3[i3].area4.Insert(i4, obj);
            //修改插入后面数据的id序号
            for (int i = i4; i < FileMesege.AreaList[i1].area2[i2].area3[i3].area4.Count; i++)
            {
                //更新区域表中的ID1-4
                FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i].id1 = id1.ToString();
                FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i].id2 = id2.ToString();
                FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i].id3 = id3.ToString();
                FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i].id4 = i.ToString();
            }

            //把插入obj的NameList信息全部修改为目标ID1-4
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                PointUpdata4(eq, i1, i2, i3, i4);
            }*/
        }

        private void PointUpdata4(DataJson.PointInfo eq,int i1,int i2,int i3,int i4)
        {
            //修改插入节点的新ID eqID1-4删除的时候作标记
            if ("del" == eq.area1 && "del" == eq.area2 && "del" == eq.area3 && "del" == eq.area4)
            {
                eq.area1 = FileMesege.AreaList[i1].area;
                eq.area2 = FileMesege.AreaList[i1].area2[i2].area;
                eq.area3 = FileMesege.AreaList[i1].area2[i2].area3[i3].area;
                eq.area4 = FileMesege.AreaList[i1].area2[i2].area3[i3].area4[i4].area;
            }
        }

        //拖拽节点 修改AreaList
        private void AreaListUpdata(string index1, string index2, string index3, DataJson.Area3 obj)
        {
            
            //索引号
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            int i3 = Convert.ToInt32(index3);
            //目标id 1-4
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].id1);
            int id2 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].id2);
            int id3 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].area3[i3].id3);

            //插入到target目标 List下面
            FileMesege.AreaList[i1].area2[i2].area3.Insert(i3, obj);
            //修改插入后面数据的id序号
            for (int i = i3; i < FileMesege.AreaList[i1].area2[i2].area3.Count; i++)
            {
                FileMesege.AreaList[i1].area2[i2].area3[i].id1 = id1.ToString();
                FileMesege.AreaList[i1].area2[i2].area3[i].id2 = id2.ToString();
                FileMesege.AreaList[i1].area2[i2].area3[i].id3 = i.ToString();
                //area3[i]里面的area4所有id1-3都要修改
                foreach (DataJson.Area4 a4 in FileMesege.AreaList[i1].area2[i2].area3[i].area4)
                {
                    a4.id1 = id1.ToString();
                    a4.id2 = id2.ToString();
                    a4.id3 = i.ToString();
                }
            }

           
            //把插入obj的NameList信息全部修改为目标ID1-4
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                PointUpdata3(eq,i1,i2,i3);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                PointUpdata3(eq, i1, i2, i3);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                PointUpdata3(eq, i1, i2, i3);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                PointUpdata3(eq, i1, i2, i3);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                PointUpdata3(eq, i1, i2, i3);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                PointUpdata3(eq, i1, i2, i3);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                PointUpdata3(eq, i1, i2, i3);
            }*/

            
        }

        private void PointUpdata3(DataJson.PointInfo eq, int i1, int i2, int i3)
        {
            //修改插入节点的新ID eqID1-4删除的时候作标记
            if ("del" == eq.area1 && "del" == eq.area2 && "del" == eq.area3)
            {
                eq.area1 = FileMesege.AreaList[i1].area;
                eq.area2 = FileMesege.AreaList[i1].area2[i2].area;
                eq.area3 = FileMesege.AreaList[i1].area2[i2].area3[i3].area;
            }
        }

        //拖拽节点 修改AreaList
        private void AreaListUpdata(string index1, string index2, DataJson.Area2 obj)
        {
            
            //索引号
            int i1 = Convert.ToInt32(index1);
            int i2 = Convert.ToInt32(index2);
            //目标id 1-4
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].id1);
            int id2 = Convert.ToInt32(FileMesege.AreaList[i1].area2[i2].id2);

            //插入到target目标 List下面
            FileMesege.AreaList[i1].area2.Insert(i2, obj);
            //修改插入后面数据的id序号
            for (int i = i2; i < FileMesege.AreaList[i1].area2.Count; i++)
            {
                //修改area1[i1]里面所有的area2插入后面数据 修改id1-2
                FileMesege.AreaList[i1].area2[i].id1 = id1.ToString();
                FileMesege.AreaList[i1].area2[i].id2 = i.ToString();
                //area2[i]里面的area3所有id1-2都要修改
                foreach (DataJson.Area3 a3 in FileMesege.AreaList[i1].area2[i].area3)
                {
                    a3.id1 = id1.ToString();
                    a3.id2 = i.ToString();
                    //area3[i]里面的area4所有id1-2都要修改
                    foreach (DataJson.Area4 a4 in a3.area4)
                    {
                        a4.id1 = id1.ToString();
                        a4.id2 = i.ToString();
                    }
                }
            }
        
            //把插入obj的NameList信息全部修改为目标ID1-4
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                PointUpdata2(eq,i1,i2);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                PointUpdata2(eq, i1, i2);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                PointUpdata2(eq, i1, i2);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                PointUpdata2(eq, i1, i2);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                PointUpdata2(eq, i1, i2 );
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                PointUpdata2(eq, i1, i2 );
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                PointUpdata2(eq, i1, i2 );
            }*/
            

        }

        private void PointUpdata2(DataJson.PointInfo eq, int i1, int i2)
        {
            //修改插入节点的新ID eqID1-4删除的时候作标记
            if ("del" == eq.area1 && "del" == eq.area2)
            {
                eq.area1 = FileMesege.AreaList[i1].area;
                eq.area2 = FileMesege.AreaList[i1].area2[i2].area;
            }
        }


        //拖拽节点 修改AreaList
        private void AreaListUpdata(string index1, DataJson.Area1 obj)
        {
           
            //索引号
            int i1 = Convert.ToInt32(index1);
            //目标id 1-4
            int id1 = Convert.ToInt32(FileMesege.AreaList[i1].id1);

            //插入到target目标 List下面
            FileMesege.AreaList.Insert(i1, obj);
            //修改插入后面数据的id序号
            for (int i = i1; i < FileMesege.AreaList.Count; i++)
            {
                //修改List里面所有的area1插入后面数据 修改id1
                FileMesege.AreaList[i].id1 = i.ToString();
                //area1[i]里面的area2所有id1都要修改
                foreach (DataJson.Area2 a2 in FileMesege.AreaList[i].area2)
                {
                    a2.id1 = i.ToString();

                    //area2[i]里面的area3所有id1都要修改
                    foreach (DataJson.Area3 a3 in a2.area3)
                    {
                        a3.id1 = i.ToString();
                        //area3[i]里面的area4所有id1都要修改
                        foreach (DataJson.Area4 a4 in a3.area4)
                        {
                            a4.id1 = i.ToString();
                        }
                    }
                }
            }

            //把插入obj的NameList信息全部修改为目标ID1-4
            foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
            {
                PointUpdata1(eq,i1);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.scene)
            {
                PointUpdata1(eq, i1);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.link)
            {
                PointUpdata1(eq, i1);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.timer)
            {
                PointUpdata1(eq, i1);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.virtualport)
            {
                PointUpdata1(eq, i1);
            }
            foreach (DataJson.PointInfo eq in FileMesege.PointList.logic)
            {
                PointUpdata1(eq, i1);
            }
            /*
            foreach (DataJson.PointInfo eq in FileMesege.PointList.localvar)
            {
                PointUpdata1(eq, i1);
            }*/

        }

        private void PointUpdata1(DataJson.PointInfo eq, int i1)
        {
            //修改插入节点的新ID eqID1-4删除的时候作标记
            if ("del" == eq.area1)
            {
                eq.area1 = FileMesege.AreaList[i1].area;
            }
        }
        #endregion

        #region 新增节点  子节点 删除 （按键）
        //用于标记是否新建子节点
        private bool isAddChild = false;
        private void btnAddNode_Click(object sender, EventArgs e)
        {
            isAddChild = false;
            //把窗口向屏幕中间刷新
            tss.StartPosition = FormStartPosition.CenterParent;
            tss.Newflag = newflag;
            //设定开始层级
            int i = 0;
            if (treeView1.SelectedNode != null)
            {
                i = treeView1.SelectedNode.Level;
                
            }
            if (i > 3)
            {
                return;
            }
            tss.LbText = "添加节点";
            tss.Selectindex = i;
            tss.ShowDialog();
        }

        private void btnAddChild_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Text == "查看所有区域" || treeView1.SelectedNode.Text == "未定义区域")
            {
                return;
            }
            isAddChild = true;
            newflag = true;
            newTsSection();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && FileMesege.sectionNode.Text != "查看所有区域" && FileMesege.sectionNode.Text != "未定义区域")
            {

                if (deleteNode(treeView1.SelectedNode))
                {
                    //选中上一个可视节点
                    fullpath = "";
                    if (FileMesege.sectionNode != null && FileMesege.sectionNode.PrevVisibleNode != null)
                    {

                        fullpath = FileMesege.sectionNode.PrevVisibleNode.FullPath;

                    }
                    ThreeSEctionAddNode();
                
                }
               
            }
        }
        #endregion

       


    }
}
