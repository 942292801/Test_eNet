﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace eNet编辑器.ThreeView
{
    public delegate DataJson.sceneInfo DgvsceneAddOneItem(DataJson.sceneInfo info, string ip);
 
    public partial class ThreeTitle : Form
    {

        public event DgvsceneAddOneItem dgvsceneAddOneItem;
        public event Func<DataJson.timersInfo, string, DataJson.timersInfo> dgvtimerAddOneItem;
  

        //添加点位
        public event Action<string> addPoint;
        //添加虚拟端口
        public event Action<string> addVirtualport;
        public event Action selectLastCountScene;
        public event Action selectLastCountTimer;
        public event Action selectLastCountLocalVar;

        /// <summary>
        /// 传输point点跳转窗口
        /// </summary>
        public event Action<DataJson.PointInfo> jumpSetInfo;

        //树状图节点
        public string fullpath = "";

        public ThreeTitle()
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

        private void ThreeTitle_Load(object sender, EventArgs e)
        {
           
        }
      
        
        /// <summary>
        /// 存放ini define区域内 读取到的键值
        /// </summary>
        public static List<string> keys = new List<string>();
        

        /// <summary>
        /// 当前cbtype类型  ini文件路径 renamed scene
        /// </summary>
        public string inifilepath = "";


        
        #region 测试异步加载
        public void ThreeTitleAddNode(int num)
        {
            FileMesege.cbTypeIndex = num;
            Thread t = new Thread(ShowDatatable);
            t.IsBackground = true;
            t.Start();
        }

        public delegate void FormIniDelegate();
        private void ShowDatatable()
        {
            this.Invoke(new FormIniDelegate(TabIni));

        }



        /// <summary>
        /// 树状图加载总设置函数 从names.ini文件加载的名字 或从nameJson加载 
        /// </summary>
        /// <param name="num">对象类型cbType索引号</param>
        public void TabIni()
        {
            try
            {
                
                //路径非空
                if (!String.IsNullOrWhiteSpace(inifilepath))
                {
                  
                    treeView1.Nodes.Clear();
                    FileMesege.titleinfo = "";
                    FileMesege.titlePointSection = "";
                    //根据form.按钮选择来加载cbtype内容 和
                    switch (FileMesege.formType)
                    {
                        case "name":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            nameAdd(FileMesege.cbTypeIndex, ToolsUtil.getIP(FileMesege.tnselectNode));
                            //MessageBox.Show("name");
                            break;
                        case "point":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            pointAdd(FileMesege.cbTypeIndex);
                            break;
                        case "scene":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = contextMenuStrip2;
                            sceneAdd(FileMesege.cbTypeIndex, ToolsUtil.getIP(FileMesege.sceneSelectNode));
                            break;
                        case "timer":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = contextMenuStrip2;
                            timerAdd(FileMesege.cbTypeIndex, ToolsUtil.getIP(FileMesege.timerSelectNode));
                            break;
                        case "panel":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = contextMenuStrip1;
                            panelAdd(FileMesege.cbTypeIndex, ToolsUtil.getIP(FileMesege.panelSelectNode));
                            // MessageBox.Show("bind");
                            break;
                        case "sensor":
                             treeView1.CheckBoxes = false;
                             treeView1.ContextMenuStrip = contextMenuStrip1;
                             sensorAdd(FileMesege.cbTypeIndex, ToolsUtil.getIP(FileMesege.sensorSelectNode));
                            break;
                        case "logic":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            sensorAdd(FileMesege.cbTypeIndex, ToolsUtil.getIP(FileMesege.logicSelectNode));
                            break;
                        case "virtualport":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            virtualportAdd(FileMesege.cbTypeIndex);
                            break;
                      
                        default: break;
                    }
                    TreeMesege.SetPrevVisitNode(treeView1, fullpath);
                }
            }
            catch
            { 
            
            }
          
           
        
        }

        #endregion

        #region 取消选中
        public void unSelectTitleNode()
        {
            treeView1.SelectedNode = null;
            FileMesege.titlePointSection = null;
            FileMesege.titleinfo = "";
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                treeView1.Nodes[i].Checked = false;

            }
        }

        public void UpdataNodeText(string oldNodeText,string newNodeText)
        {
            TreeNode node = TreeMesege.findNodeByName(treeView1,oldNodeText);
            if (node != null)
            {
                node.Text = newNodeText;
            }
        }
        #endregion

        #region 设备加载节点
        /// <summary>
        /// 设备模式 设备按钮加载树状图节点
        /// </summary>
        /// <param name="num"></param>
        private void nameAdd(int num,string ipLast)
        {
            TreeMesege tm = new TreeMesege();
            ipLast = "@" + ipLast;
            string strs = IniConfig.GetValue(inifilepath, "equipment", keys[num]);
            //读取到的ini值为空 则加载当前选中节点的地址
            if (string.IsNullOrEmpty(strs) && FileMesege.PointList != null)
            {
                //加载选中位置的Point节点
                List<string> infolist = DataListHelper.GetPointNodeBySectionName(FileMesege.PointList.equipment);
                if (infolist != null)
                {
                    for (int i = 0; i < infolist.Count; i++)
                    {
                        if (!infolist[i].Contains(ipLast) && !infolist[i].Contains("@255"))
                        {
                            continue;
                        }
                        tm.AddNode1(treeView1, infolist[i]);
                    }
                }
                
            }
            else
            {
                //正常加载名称
                string[] strarr = strs.Split(',');
                if (string.IsNullOrEmpty(strarr[0]))
                {
                    return;
                }
                for (int i = 0; i < strarr.Length; i++)
                {
                    tm.AddNode1(treeView1, strarr[i]);
                }
            }
           
        }
        #endregion

        #region 点位加载节点
        /// <summary>
        /// 点位模式 点位按钮加载树状图节点
        /// </summary>
        /// <param name="num"></param>
        private void pointAdd(int num)
        {
            TreeMesege tm = new TreeMesege();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//objs");
            string name = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                
                name = IniConfig.GetValue(file.FullName, "define", "name");
                if (name == keys[num])
                {
                    string[] strarr = IniConfig.GetValue(file.FullName, "define", "species").Split(',');
                    if (string.IsNullOrEmpty( strarr[0]))
                    {
                        return;
                    }
                    for (int i = 0; i < strarr.Length; i++)
                    {
                        tm.AddNode1(treeView1, strarr[i]);
                    }
                    break;
                }

            }
           
        }
        #endregion

        #region 场景加载节点
        /// <summary>
        /// 场景模式 按cbtype类型 来加载title节点
        /// </summary>
        /// <param name="num"></param>
        private void sceneAdd(int num,string ipLast)
        {
                try
                {
                    //按cbtype类型 来加载title节点
                    switch (IniConfig.GetValue(inifilepath, "scene", keys[num]))
                    {
                        case "equipment":
                            getNametree(FileMesege.PointList.equipment, "equipment", ipLast);
                            break;
                        case "scene":
                            getNametree(FileMesege.PointList.scene, "scene", ipLast);
                            break;
                        case "timer":
                            getNametree(FileMesege.PointList.timer, "timer", ipLast);
                            break;
                        case "link":
                            getNametree(FileMesege.PointList.link, "link", ipLast);
                            break;
                        case "sensor":
                            getNametree(FileMesege.PointList.link, "sensor", ipLast);
                            break;
                        case "virtualport":
                            getNametree(FileMesege.PointList.virtualport, "virtualport", ipLast);
                            break;
                        case "logic":
                            getNametree(FileMesege.PointList.logic, "logic", ipLast);
                            break;
                            /*
                        case "localvar":
                            getNametree(FileMesege.PointList.localvar, "localvar", ipLast);
                            break;*/
                        default: break;
                    }
                }
                catch {
                    //MessageBox.Show("节点加载失败!请检查names.ini文件","提示");
                }
          
        }

        /// <summary>
        /// 由section  获取PointList节点 添加到title列表中
        /// </summary>
        /// <param name="list">Namelist相对应的子项表</param>
        private void getNametree(List<DataJson.PointInfo> Jsonlist, string type, string ipLast)
        {
            TreeMesege tm = new TreeMesege();
            bool isScene = false;
            if (type == "scene")
            {
                isScene = true;
            }
            else
            {
                ipLast = "@" + ipLast;
                isScene = false;
            }
            //加载选中位置的Point节点
            List<string> infolist = DataListHelper.GetPointNodeBySectionName(Jsonlist);
            if (infolist != null)
            {
                for (int i = 0; i < infolist.Count; i++)
                {
                    if (!isScene && !infolist[i].Contains(ipLast) && !infolist[i].Contains("@255"))
                    {
                        //过滤ip地址
                        continue;
                    }

                    if (type == "link")
                    {
                        //加载面板
                        if (infolist[i].Contains("面板"))
                        {
                            tm.AddNode1(treeView1, infolist[i]);
                        }
                    }
                    else if (type == "sensor")
                    {
                        //加载感应
                        //加载面板
                        if (infolist[i].Contains("感应编组"))
                        {
                            tm.AddNode1(treeView1, infolist[i]);
                        }
                    }
                    else
                    {
                        tm.AddNode1(treeView1, infolist[i]);
                    }
                    
                }
            }
            
        }

        #endregion

        #region 定时加载节点
        private void timerAdd(int num,string ipLast)
        {
            sceneAdd(num, ipLast);

        }
        #endregion


        #region 面板加载节点
        private void panelAdd(int num, string ipLast)
        {
            sceneAdd(num, ipLast);

        }
        #endregion

        #region 感应加载节点
        private void sensorAdd(int num, string ipLast)
        {
            sceneAdd(num, ipLast);

        }
        #endregion

        #region 逻辑加载节点
        private void logicAdd(int num, string ipLast)
        {
            sceneAdd(num, ipLast);

        }
        #endregion

        #region 虚拟端口加载节点
        /// <summary>
        /// 虚拟端口模式 虚拟端口按钮加载树状图节点
        /// </summary>
        /// <param name="num"></param>
        private void virtualportAdd(int num)
        {
            TreeMesege tm = new TreeMesege();
            string strs = IniConfig.GetValue(inifilepath, "virtualport", keys[num]);
            //读取到的ini值为空 则加载当前选中节点的地址
            if (string.IsNullOrEmpty(strs) && FileMesege.PointList != null)
            {
                /*
                //加载选中位置的Point节点
                List<string> infolist = DataListHelper.GetPointNodeBySectionName(FileMesege.PointList.equipment);
                if (infolist != null)
                {
                    for (int i = 0; i < infolist.Count; i++)
                    {
                        tm.AddNode1(treeView1, infolist[i]);
                    }
                }
                */
            }
            else
            {
                //正常加载名称
                string[] strarr = strs.Split(',');
                if (string.IsNullOrEmpty(strarr[0]))
                {
                    return;
                }
                for (int i = 0; i < strarr.Length; i++)
                {
                    tm.AddNode1(treeView1, strarr[i]);
                }
            }

        }
        #endregion

        #region 选中节点高亮  鼠标点击事件

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FileMesege.titlePointSection = "";
            //把选中节点保存至文档中 
            FileMesege.titleinfo = treeView1.SelectedNode.Text;
            fullpath = treeView1.SelectedNode.FullPath;
            if (FileMesege.formType == "name" && FileMesege.cbTypeIndex == 0)
            {
                //当选中为 命名和点位
                FileMesege.titlePointSection = treeView1.SelectedNode.Text;
                FileMesege.titleinfo = "";
            }
            /*
            if (FileMesege.formType == "scene")
            {
                //选中为场景时候
                FileMesege.titlePointSection = treeView1.SelectedNode.Text;
                FileMesege.titleinfo = "";
            }*/
            if (FileMesege.formType == "scene"|| FileMesege.formType == "panel" || FileMesege.formType == "sensor" || FileMesege.formType == "logic")
            {
                FileMesege.titlePointSection = treeView1.SelectedNode.Text;
                FileMesege.titleinfo = "";
            }

            //鼠标光标改变
            //addTitleDevCursor();
            //addTitlenNameCursor();
            
        }

        /// <summary>
        /// 高亮显示选中项 重绘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// 开始第一个选择的节点Index
        /// </summary>
        private int startrow = -1;

        /// <summary>
        /// 点击事件 获取当前选中节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            //右击选中节点
            if (e.Button == MouseButtons.Right|| e.Button == MouseButtons.Left)
            {
                Point ClickPoint = new Point(e.X, e.Y);
                TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);
                //newflag = false;
                if (CurrentNode != null)
                {
                    treeView1.SelectedNode = CurrentNode;//选中这个节点
                    startrow = this.treeView1.SelectedNode.Index;
                    //newflag = true;
                }
            }

           
        }



        /// <summary>
        /// 鼠标起来 选择鼠标正选还是反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            //选中checkbox
            Point ClickPoint = this.PointToClient(Control.MousePosition);
            TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);

            if (CurrentNode == null)
            {
                return;
            }

            if (startrow >= 0)
            {
                int endrow = CurrentNode.Index;// this.treeView1.SelectedNode.Index;

                if (startrow < endrow)
                {

                    //正序选时
                    for (int x = startrow; x <= endrow; x++)
                    {
                        this.treeView1.Nodes[x].Checked = this.treeView1.Nodes[startrow].Checked;

                    }
                }
                else if(startrow > endrow)
                {

                    //倒序选时
                    for (int x = endrow; x <= startrow; x++)
                    {

                        this.treeView1.Nodes[x].Checked = this.treeView1.Nodes[startrow].Checked;
                       
                        
                    }
                }
                startrow = endrow;

            }
        }

        #endregion

        #region 菜单栏 添加 全选 反选 双击节点

        /// <summary>
        /// 双击添加 节点到DGV中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                
                dgvNodeAdd(treeView1.SelectedNode);
                //右击选中节点
           
            }
            catch { 
            
            }

        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addTitleItem();
        }

        public void addTitleItem()
        {
            try
            {
                bool isChange = false;
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //循环判断treeView的节点 如果是选中
                foreach (TreeNode tn in treeView1.Nodes)
                {
                    if (tn.Checked)
                    {
                        if (dgvNodeAdd(tn))
                        {
                            isChange = true;
                        }

                    }
                }//foreach所有节点信息处理完
                if (isChange)
                {
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
                }
            }
            catch { }
        }

        /// <summary>
        /// title节点 添加到dgv表中
        /// </summary>
        /// <param name="tn"></param>
        private bool dgvNodeAdd(TreeNode tn)
        {
            bool isChange = false;
            //区域
            switch (FileMesege.formType)
            {
                case "name":

                    //MessageBox.Show("name");
                    break;
                case "point":
                    //添加点位
                    addPoint(keys[FileMesege.cbTypeIndex]);
                   
                    break;
                case "scene":
                    sceneFormtype(tn.Text);
                    isChange = true;
                    break;
                case "timer":
                    timerFormtype(tn.Text);
                    isChange = true;
                    break;
                case "panel":
                    //不允许双击添加
                    break;
                case "logic":
                    break;
                case "sensor":
                    //不允许双击添加
                    break;
                case "virtualport":
                    addVar(tn.Text);
                    isChange = true;
                    break;
                default: break;
            }
            return isChange;
        }

        /// <summary>
        /// 添加虚拟端口 根据虚拟端口后面 +？
        /// </summary>
        /// <param name="nodeTxt"></param>
        private void addVar(string nodeTxt)
        {
            try
            {
                
                int number = Convert.ToInt32( Regex.Replace(nodeTxt, @"[^\d]*", ""));
                FileMesege.titleinfo = nodeTxt.Split('+')[0];
                for (int i = 0; i < number; i++)
                {
                    //添加虚拟端口
                    addVirtualport(keys[FileMesege.cbTypeIndex]);
                }
             
               
                selectLastCountLocalVar();
            }
            catch { 
            
            }
        }


        #region 添加功能 场景DGV添加 新增SceneInfo对象
        /// <summary>
        /// 场景DGV添加 新增SceneInfo对象
        /// </summary>
        /// <param name="texts">Title节点 区域--名称--地址</param>
        private void sceneFormtype(string nodeText) 
        {
            if (FileMesege.sceneSelectNode == null || FileMesege.sceneSelectNode.Parent == null)
            {
                return;
            }

            //根据commandNames的【scene】中的 key->value 决定
            switch (IniConfig.GetValue(inifilepath, "scene", keys[FileMesege.cbTypeIndex]))
            {
                //0=设备,equipment
                case "equipment":
                    dgvSceneListAdd(FileMesege.PointList.equipment, nodeText);
                    break;
                //1=场景,scene
                case "scene":
                    dgvSceneListAdd(FileMesege.PointList.scene, nodeText);
                    break;
                //2=定时,timer
                case "timer":
                    dgvSceneListAdd(FileMesege.PointList.timer, nodeText);
                    break;
                //3=绑定,link
                case "link":
                    dgvSceneListAdd(FileMesege.PointList.link, nodeText);
                    break;
                case "sensor":
                    dgvSceneListAdd(FileMesege.PointList.link, nodeText);
                    break;
                case "virtualport":
                    dgvSceneListAdd(FileMesege.PointList.virtualport, nodeText);
                    break;
                default:
                    dgvSceneListAdd(FileMesege.PointList.equipment, nodeText);
                    break;

            }
        }

        /// <summary>
        /// scene表中添加新的eq信息
        /// </summary>
        /// <param name="eqList"></param>
        /// <param name="nodeText"> title节点的信息 1\\2---name</param>
        private void dgvSceneListAdd(List<DataJson.PointInfo> eqList, string nodeText) 
        {
            if (FileMesege.sceneList == null)
            {
                FileMesege.sceneList = new List<DataJson.Scene>();
            }
            //选中子节点
            //循环获取
            string ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = getSceneInfoList(ip, sceneNum);
            List<string> section_name = DataListHelper.dealPointInfo(nodeText);
            if(section_name == null)
            {
                return;
            }
            foreach (DataJson.PointInfo eq in eqList)
            {
                //NameList中查找到匹配的信息
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] &&eq.area3 == section_name[2] &&eq.area4 == section_name[3] && eq.name == section_name[4])
                {
          
                    
        
                    //新建表
                    DataJson.sceneInfo info = new DataJson.sceneInfo();
                    int id = 0;
                    //获取ID
                    foreach (DataJson.sceneInfo find in sc.sceneInfo)
                    {
                        if (find.id > id)
                        {
                            id = find.id;
                        }
                    }
                    info.id = id+1;
                    info.delay = 0;
                    if (ip != eq.ip && !string.IsNullOrEmpty(eq.ip))
                    {
                        info.address = ToolsUtil.GetIPstyle(eq.ip,4) + eq.address.Substring(2,6);
                    }
                    else
                    {
                        info.address = eq.address;
                    }
                    info.pid = eq.pid;
               
                    info.type = eq.type;

                    List<string> optList = IniHelper.findTypesIniCommandbyType(eq.type);
                    if (optList != null)
                    {
                        info.optName = optList[0];
                        info.opt = optList[1];

                    }
                    else
                    {
                        info.optName = "";
                        info.opt = "";

                    }

                    //插入到 sceneList
                    sc.sceneInfo.Add(info);

                    
                    dgvsceneAddOneItem(info, ip);
                    selectLastCountScene();
                    break;
                }
            }
        }
       

        /// <summary>
        /// 获取某个IP点 某个场景的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">场景号</param>
        /// <returns></returns>
        private DataJson.scenes getSceneInfoList(string ip, int num)
        {
            foreach (DataJson.Scene scIP in FileMesege.sceneList)
            {
                if (scIP.IP == ip)
                {
                    foreach (DataJson.scenes sc in scIP.scenes)
                    {
                        if (sc.id == num)
                        {
                            return sc;
                        }
                    }

                }
            }
            return null;
        }
         #endregion


        #region 添加功能 定时DGV添加 新增TimerInfo对象
        /// <summary>
        /// 定时DGV添加 新增timerInfo对象
        /// </summary>
        /// <param name="nodeText"></param>
        private void timerFormtype(string nodeText)
        {
            if (FileMesege.timerSelectNode == null || FileMesege.timerSelectNode.Parent == null)
            {
                return;
            }

            //根据commandNames的【timer】中的 key->value 决定
            switch (IniConfig.GetValue(inifilepath, "timer", keys[FileMesege.cbTypeIndex]))
            {
                //0=设备,equipment
                case "equipment":
                    dgvTimerListAdd(FileMesege.PointList.equipment, nodeText);
                    break;
                //1=场景,scene
                case "scene":
                    dgvTimerListAdd(FileMesege.PointList.scene, nodeText);
                    break;
                //2=定时,timer
                case "timer":
                    dgvTimerListAdd(FileMesege.PointList.timer, nodeText);
                    break;
                //3=绑定,link
                case "link":
                    dgvTimerListAdd(FileMesege.PointList.link, nodeText);
                    break;
                case "sensor":
                    dgvTimerListAdd(FileMesege.PointList.link, nodeText);
                    break;
                case "virtualport":
                    dgvTimerListAdd(FileMesege.PointList.virtualport, nodeText);
                    break;
                default:
                    dgvTimerListAdd(FileMesege.PointList.equipment, nodeText);
                    break;

            }
        }

        /// <summary>
        /// timerList 下的timers添加新的timerInfo
        /// </summary>
        /// <param name="eqList"></param>
        /// <param name="nodeText"></param>
        private void dgvTimerListAdd(List<DataJson.PointInfo> eqList, string nodeText)
        {
            if (FileMesege.timerList == null)
            {
                FileMesege.timerList = new List<DataJson.Timer>();
            }
            //选中子节点
            //循环获取
            string ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
            string[] ids = FileMesege.timerSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址定时下的 定时信息对象
            DataJson.timers tms = getTimerInfoList(ip, sceneNum);
            List<string> section_name = DataListHelper.dealPointInfo(nodeText);
            if (section_name == null)
            {
                return;
            }
            foreach (DataJson.PointInfo eq in eqList)
            {
                //NameList中查找到匹配的信息
                if (eq.area1 == section_name[0] && eq.area2 == section_name[1] && eq.area3 == section_name[2] && eq.area4 == section_name[3] && eq.name == section_name[4])
                {
                    //新建表
                    DataJson.timersInfo tmInfo = new DataJson.timersInfo();
                    int id = 0;
                    //获取ID
                    foreach (DataJson.timersInfo find in tms.timersInfo)
                    {
                        if (find.id > id)
                        {
                            id = find.id;
                        }
                    }
                    tmInfo.id = id + 1;
                    tmInfo.shortTime = "";
                    if (ip != eq.ip && !string.IsNullOrEmpty(eq.ip))
                    {
                        tmInfo.address = ToolsUtil.GetIPstyle(eq.ip, 4) + eq.address.Substring(2, 6);
                    }
                    else
                    {
                        tmInfo.address = eq.address;
                    }
                    tmInfo.pid = eq.pid;
                    
                    tmInfo.type = eq.type;
                    List<string> optList = IniHelper.findTypesIniCommandbyType(eq.type);
                    if (optList != null)
                    {
                        tmInfo.optName = optList[0];
                        tmInfo.opt = optList[1];

                    }
                    else
                    {
                        tmInfo.optName = "";
                        tmInfo.opt = "";

                    }
                    tms.timersInfo.Add(tmInfo);
             
                    dgvtimerAddOneItem(tmInfo,ip);
                    selectLastCountTimer();
                    break;
                }
            }
        }

        /// <summary>
        /// 获取该IP和定时号的 timers 对象
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private DataJson.timers getTimerInfoList(string ip, int num)
        {
            foreach (DataJson.Timer tmIP in FileMesege.timerList)
            {
                if (tmIP.IP == ip)
                {
                    foreach (DataJson.timers tms in tmIP.timers)
                    {
                        if (tms.id == num)
                        {
                            return tms;
                        }
                    }

                }
            }
            return null;
        }
        #endregion



        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                treeView1.Nodes[i].Checked = true;
            }
           
        }

        private void 反选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < treeView1.Nodes.Count; i++)
            {
                treeView1.Nodes[i].Checked = !treeView1.Nodes[i].Checked;
            }
        }



        #endregion


        #region 编辑 跳转函数
        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //设置对象跳转
                DataJson.PointInfo point = dgvJumpSet();
                if (point != null)
                {
                    //传输到form窗口控制
                    //AppTxtShow("传输到主窗口"+DateTime.Now);
                    jumpSetInfo(point);
                }
            }
            catch
            {

            }
        }



        /// <summary>
        /// 对象跳转获取 场景 定时 编组 point点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataJson.PointInfo dgvJumpSet()
        {
            if (treeView1.SelectedNode == null)
            {
                return null;
            }
            List<string> section_name = DataListHelper.dealPointInfo(treeView1.SelectedNode.Text);
            DataJson.PointInfo eq = DataListHelper.findPointBySection_name(section_name);
            if (eq.type == "3.0_logic" || eq.type == "4.0_scene" || eq.type == "5.0_timer" || eq.type == "6.1_panel" || eq.type == "6.2_sensor")
            {
                return eq;
            }
            return null;
        }
        #endregion

    }
}
