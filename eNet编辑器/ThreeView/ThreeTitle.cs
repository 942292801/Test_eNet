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
using eNet编辑器.DgvView;
using System.IO;
using System.Reflection;

namespace eNet编辑器.ThreeView
{
    public delegate void DgvSceneAddItem2();
    //public delegate void AddTitleDevCursor();
   // public delegate void AddTitlenNameCursor();
    public partial class ThreeTitle : Form
    {

        public event DgvSceneAddItem2 dgvsceneAddItem;
        public event Action dgvtimerAddItem;
        public event Action dgvVariableAddItem;
        //public event AddTitleDevCursor addTitleDevCursor;
        //public event AddTitlenNameCursor addTitlenNameCursor;

        //添加点位
        public event Action<string> addPoint;
        //添加变量
        public event Action<string> addVariable;


        public ThreeTitle()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.treeView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.treeView1, true, null);
        }


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

      
        

        /// <summary>
        /// 树状图加载总设置函数 从names.ini文件加载的名字 或从nameJson加载 
        /// </summary>
        /// <param name="num">对象类型cbType索引号</param>
        public void ThreeTitleAddNode(int num)
        {
            try
            {
                FileMesege.cbTypeIndex = num;
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
                            nameAdd(num, SocketUtil.getIP(FileMesege.tnselectNode));
                            //MessageBox.Show("name");
                            break;
                        case "point":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            pointAdd(num);
                            break;
                        case "scene":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = contextMenuStrip2;
                            sceneAdd(num, SocketUtil.getIP(FileMesege.sceneSelectNode));
                            break;
                        case "timer":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = contextMenuStrip2;
                            timerAdd(num, SocketUtil.getIP(FileMesege.timerSelectNode));
                            break;
                        case "panel":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = null;
                            panelAdd(num, SocketUtil.getIP(FileMesege.panelSelectNode));
                            // MessageBox.Show("bind");
                            break;
                        case "sensor":
                             treeView1.CheckBoxes = true;
                             treeView1.ContextMenuStrip = null;
                            sensorAdd(num,SocketUtil.getIP(FileMesege.sensorSelectNode));
                            break;
                        case "logic":
                            // MessageBox.Show("logic");
                            break;
                        case "variable":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            variableAdd(num);
                            break;
                      
                        default: break;
                    }
                }
            }
            catch
            { 
            
            }
          
           
        
        }

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
                        case "variable":
                            getNametree(FileMesege.PointList.variable, "variable", ipLast);
                            break;
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

        #region 变量加载节点
        /// <summary>
        /// 变量模式 变量按钮加载树状图节点
        /// </summary>
        /// <param name="num"></param>
        private void variableAdd(int num)
        {
            TreeMesege tm = new TreeMesege();
            string strs = IniConfig.GetValue(inifilepath, "variable", keys[num]);
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
            if (FileMesege.formType == "name" && FileMesege.cbTypeIndex == 0)
            {
                //当选中为 命名和点位
                FileMesege.titlePointSection = treeView1.SelectedNode.Text;
                FileMesege.titleinfo = "";
            }
            if (FileMesege.formType == "scene" )
            {
                //选中为场景时候
                FileMesege.titlePointSection = treeView1.SelectedNode.Text;
                FileMesege.titleinfo = "";
            }
            if (FileMesege.formType == "panel" || FileMesege.formType == "sensor")
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
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
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
            Point ClickPoint = this.PointToClient(Control.MousePosition);
            TreeNode CurrentNode = treeView1.GetNodeAt(ClickPoint);

            if (CurrentNode != null)
            {
                treeView1.SelectedNode = CurrentNode;//选中这个节点

            }
            else
            {
                return;
            }
            if (startrow >= 0)
            {
                int endrow = this.treeView1.SelectedNode.Index;

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
                updateDgv();
            }
            catch { 
            
            }

        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            //循环判断treeView的节点 如果是选中
            foreach (TreeNode tn in treeView1.Nodes)
            {
                if (tn.Checked)
                {
                    dgvNodeAdd(tn);    
                }
            }//foreach所有节点信息处理完
            //添加完成刷新窗口
            updateDgv();
        }



        /// <summary>
        /// title节点 添加到dgv表中
        /// </summary>
        /// <param name="tn"></param>
        private void dgvNodeAdd(TreeNode tn)
        {
            //区域
            switch (FileMesege.formType)
            {
                case "name":

                    //MessageBox.Show("name");
                    break;
                case "point":
                    //添加点位
                    addPoint(keys[FileMesege.cbTypeIndex]);
                    //MessageBox.Show("name");
                    break;
                case "scene":
                    sceneFormtype(tn.Text);
                    //回调更新界面
                   
                    break;
                case "timer":
                    timerFormtype(tn.Text);
                    break;
                case "panel":
                    //回调更新界面
                    break;
                case "logic":
                    break;
                case "sensor":
                    break;
                case "variable":
                    addVar(tn.Text);
                    
                    break;
                default: break;
            }
            //选中添加的类型 搜索Title  NameList中的各种类型
        }

        /// <summary>
        /// 添加变量 根据变量后面 +？
        /// </summary>
        /// <param name="nodeTxt"></param>
        private void addVar(string nodeTxt)
        {
            try
            {
                
                int number = Convert.ToInt32( Regex.Replace(nodeTxt, @"[^\d]*", ""));
                FileMesege.titleinfo = nodeTxt.Split('+')[0];
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                for (int i = 0; i < number; i++)
                {
                    //添加变量
                    addVariable(keys[FileMesege.cbTypeIndex]);
                }
                dgvVariableAddItem();
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
                    
            }
            catch { 
            
            }
        }

        /// <summary>
        /// 更新dgv窗口 后续还有添加
        /// </summary>
        private void updateDgv()
        {
            //区域
            switch (FileMesege.formType)
            {
                case "name":

                    break;
                case "point":
                    
                    break;
                case "scene":
                    dgvsceneAddItem();
      
                    break;
                case "timer":
                    dgvtimerAddItem();
                    break;
                case "panel":
                    //dgvPanelAddItem();
                    break;
                case "logic":
                    break;
                case "sensor":
                    break;
                case "variable":
                    break;
                default: break;
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
                case "variable":
                    dgvSceneListAdd(FileMesege.PointList.variable, nodeText);
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
          
                    
                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
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
                    info.Delay = 0;
                    info.address = eq.address;
                    info.pid = eq.pid;
                    info.opt = "";
                    info.optName = "";
                    info.type = eq.type;
                    //插入到 sceneList
                    sc.sceneInfo.Add(info);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
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
                case "variable":
                    dgvTimerListAdd(FileMesege.PointList.variable, nodeText);
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

                    //撤销
                    DataJson.totalList OldList = FileMesege.cmds.getListInfos();
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
                    tmInfo.address = eq.address;
                    tmInfo.pid = eq.pid;
                    tmInfo.opt = "";
                    tmInfo.optName = "";
                    tmInfo.type = eq.type;
                    tms.timersInfo.Add(tmInfo);
                    DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                    FileMesege.cmds.DoNewCommand(NewList, OldList);
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

       














    }
}
