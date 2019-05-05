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

namespace eNet编辑器.ThreeView
{
    public delegate void DgvSceneAddItem2();
    public delegate void DgvBindAddItem2();
    //public delegate void AddTitleDevCursor();
   // public delegate void AddTitlenNameCursor();
    public partial class ThreeTitle : Form
    {

        public event DgvSceneAddItem2 dgvsceneAddItem;
        public event DgvBindAddItem2 dgvbindAddItem;
        //public event AddTitleDevCursor addTitleDevCursor;
        //public event AddTitlenNameCursor addTitlenNameCursor;

        //添加点位
        public event Action addPoint;
        public ThreeTitle()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 解决窗体闪烁问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private void ThreeTitle_Load(object sender, EventArgs e)
        {
           
        }
      
        
        /// <summary>
        /// 存放ini define区域内 读取到的键值
        /// </summary>
        public List<string> keys = null;
        

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
                            nameAdd(num);
                            //MessageBox.Show("name");
                            break;
                        case "point":
                            treeView1.CheckBoxes = false;
                            treeView1.ContextMenuStrip = null;
                            nameAdd(num);
                            break;
                        case "scene":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = contextMenuStrip2;
                            sceneAdd(num);
                            // MessageBox.Show("scene");
                            break;
                        case "timer":
                            // MessageBox.Show("timer");
                            break;
                        case "bind":
                            treeView1.CheckBoxes = true;
                            treeView1.ContextMenuStrip = contextMenuStrip2;
                            sceneAdd(num);
                            // MessageBox.Show("bind");
                            break;
                        case "logic":
                            // MessageBox.Show("logic");
                            break;
                        case "operation":
                            // MessageBox.Show("operation");
                            break;
                        default: break;
                    }
                }
            }
            catch
            { 
            
            }
          
           
        
        }

        #region 命名加载节点
        /// <summary>
        /// 命名模式 命名按钮加载树状图节点
        /// </summary>
        /// <param name="num"></param>
        private void nameAdd(int num)
        {
            TreeMesege tm = new TreeMesege();
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
                        tm.AddNode1(treeView1, infolist[i]);
                    }
                }
                
            }
            else
            {
                //正常加载名称
                string[] strarr = strs.Split(',');
                if (strarr[0] == "")
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
        /// 命名模式 命名按钮加载树状图节点
        /// </summary>
        /// <param name="num"></param>
        private void pointAdd(int num)
        {
            TreeMesege tm = new TreeMesege();
            string strs = IniConfig.GetValue(inifilepath, "point", keys[num]);
            if (strs == null)
            {
                return;
            }
            string[] strarr = strs.Split(',');
            for (int i = 0; i < strarr.Length; i++)
            {
                tm.AddNode1(treeView1, strarr[i]);
            }
        }
        #endregion

        #region 场景加载节点
        /// <summary>
        /// 场景模式 按cbtype类型 来加载title节点
        /// </summary>
        /// <param name="num"></param>
        private void sceneAdd(int num)
        {
           

                try
                {
                    //按cbtype类型 来加载title节点
                    switch (IniConfig.GetValue(inifilepath, "scene", keys[num]))
                    {
                        case "equipment":
                            getNametree(FileMesege.PointList.equipment);
                            break;
                        case "scene":
                            getNametree(FileMesege.PointList.scene);
                            break;
                        case "timer":
                            getNametree(FileMesege.PointList.timer);
                            break;
                        case "link":
                            getNametree(FileMesege.PointList.link);
                            break;
                        case "operation":

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
        private void getNametree(List<DataJson.PointInfo> Jsonlist)
        {
            TreeMesege tm = new TreeMesege();
            //加载选中位置的Point节点
            List<string> infolist = DataListHelper.GetPointNodeBySectionName(Jsonlist);
            if (infolist != null)
            {
                for (int i = 0; i < infolist.Count; i++)
                {
                    tm.AddNode1(treeView1, infolist[i]);
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
                    addPoint();
                    //MessageBox.Show("name");
                    break;
                case "scene":
                    sceneFormtype(tn.Text);
                    //回调更新界面
                    dgvsceneAddItem();
                    break;
                case "timer":
                    break;
                case "panel":
                    bindFormtype(tn.Text);
                    //回调更新界面
                    dgvbindAddItem();
                    break;
                case "logic":
                    break;
                case "reaction":
                    break;
                default: break;
            }
            //选中添加的类型 搜索Title  NameList中的各种类型
        }

        #region 添加功能 场景DGV添加 新增SceneInfo对象
        /// <summary>
        /// 场景DGV添加 新增设备对象
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
                //3=绑定,link
                case "logic":
                    //预留
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
            string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
            string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
            int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = getSceneInfoList(ips[0], sceneNum);
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
                    info.type = eq.type;
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

        #region 添加功能 绑定DGV添加 新增BindInfo对象
        /// <summary>
        /// 绑定DGV添加 新增设备对象
        /// </summary>
        /// <param name="texts">Title节点 区域--名称--地址</param>
        private void bindFormtype(string nodeText)
        {
            /** 修改Mapper数据源 暂时屏蔽代码
            if (FileMesege.bindSelectNode == null || FileMesege.bindSelectNode.Parent == null)
            {
                return;
            }
            //提高查询效率 定位到那个NameList的分区
            switch (FileMesege.cbTypeIndex)
            {
                //0=设备,equipment 
                case 0:
                    dgvBindListAdd(FileMesege.MapperList.equipment, texts[2]);
                    //FileMesege.NameList.equipment;
                    break;
                //1=按键,key
                case 1:
                    //有问题！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                    //dgvBindListAdd(FileMesege.NameList.equipment, texts[2]);
                    //FileMesege.NameList.key;
                    break;
                //2=场景,scene
                case 2:
                    dgvBindListAdd(FileMesege.MapperList.scene, texts[2]);
                    //FileMesege.NameList.scene;
                    break;
                //3=定时,timer
                case 3:
                    dgvBindListAdd(FileMesege.MapperList.timer, texts[2]);
                    //FileMesege.NameList.timer;
                    break;
                //4=绑定,link
                case 4:
                    dgvBindListAdd(FileMesege.MapperList.link, texts[2]);
                    //FileMesege.NameList.link;
                    break;
                //5=电箱
                case 5:
                    break;
                default:
                    dgvBindListAdd(FileMesege.MapperList.equipment, texts[2]);
                    break;

            }
             **/
        }

        /// <summary>
        /// bind表中添加新的bingInfo信息
        /// </summary>
        /// <param name="eqList"></param>
        /// <param name="address"></param>
        private void dgvBindListAdd(List<DataJson.PointInfo> eqList, string address)
        {
            /** 修改Mapper数据源 暂时屏蔽代码
            if (FileMesege.bindList == null)
            {
                FileMesege.bindList = new List<DataJson.Bind>();
            }
            //ip + 网关名称+设备号+设备名称
            string[] commons = getNodeInfo(FileMesege.bindSelectNode);
            //获取节点信息失败
            if (commons == null)
            {
                return ;
            }
            DataJson.binds binds = getBindInfoList(commons[0], Convert.ToInt32(commons[2]));

            foreach (DataJson.Equipment eq in eqList)
            {
                //NameList中查找到匹配的信息
                if (eq.address == address)
                {
                    //新建表
                    DataJson.bindInfo binfo = new DataJson.bindInfo();
                    //有信息的表
                    if (binds.bindInfo.Count > 0)
                    {
                        binfo.keyId = binds.bindInfo[binds.bindInfo.Count - 1].keyId + 1;
                        binfo.groupId = binds.bindInfo[binds.bindInfo.Count - 1].groupId;
                        //binfo.mode = binds.bindInfo[binds.bindInfo.Count - 1].mode;
                        
                       
                        //binfo.showType = binds.bindInfo[count].showType;
                        //binfo.showMode = binds.bindInfo[binds.bindInfo.Count - 1].showMode;
                    }
                    //无信息的表 空白页
                    else 
                    {
                        
                        binfo.groupId = Convert.ToInt32(commons[2]);
                        binfo.keyId = 1;
                    }

                    binfo.objType = eq.objType;
                    binfo.showType = "1.0_switch";
                    binfo.showMode = "无";
                    binfo.address = "FE" + eq.address.Substring(2, 6);
                    //插入到 bindList
                    binds.bindInfo.Add(binfo);
                    break;
                }
            }
             **/
        }



        /// <summary>
        /// 获取节点信息  return 十进制  全ip + 网关名称+设备号+设备名称
        /// </summary>
        /// <param name="selectNode">选中的节点 且为子节点</param>
        /// <returns></returns>
        private string[] getNodeInfo(TreeNode SelectNode)
        {
            try
            {
                //选中子节点
                string[] ips = SelectNode.Parent.Text.Split(' ');
                string[] ids = SelectNode.Text.Split(' ');
                string sceneNum = Regex.Replace(ids[0], @"[^\d]*", "");
                //ip + 网关名称+设备号+设备名称
                string[] commons = { ips[0], ips[1], sceneNum, ids[1] };
                return commons;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取某个IP点 某个面板的对象列表 否则返回空
        /// </summary>
        /// <param name="IP">IP地址</param>
        /// <param name="num">设备号</param>
        /// <returns></returns>
        private DataJson.binds getBindInfoList(string ip, int num)
        {
            foreach (DataJson.Bind bdIP in FileMesege.bindList)
            {
                if (bdIP.IP == ip)
                {
                    foreach (DataJson.binds bd in bdIP.Binds)
                    {
                        if (bd.id == num)
                        {
                            return bd;
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
