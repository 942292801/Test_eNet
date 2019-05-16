using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using eNet编辑器.ThreeView;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.IO;
using eNet编辑器.DgvView;
using eNet编辑器.AddForm;
using eNet编辑器.Properties;

namespace eNet编辑器
{
    public partial class Form1 : Form
    {
        public ThreeName threename;
        public ThreePoint threepoint;
        public ThreeScene threescene;
        public ThreeTimer threetimer;
        public ThreeBind threebind;
        public ThreeLogic threelogic;
        public ThreeOperation threeoperation;
        public ThreeSection threesection;
        public ThreeTitle threetitle;


        public DgvName dgvname;
        public DgvPoint dgvpoint;
        public DgvDevice dgvdevice;
        public DgvScene dgvscene;
        public DgvTimer dgvtimer;
        public DgvBind dgvbind;
        public DgvLogic dgvlogic;
        public DgvOperation dgvoperation;
        //在线搜索窗体
        public OnlineSearch os;
        //检索栏号码 右侧两个树状图号码
        public int searchLocition = 2;
        public Form1()
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
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadIni();
        }



        #region 界面 初始化  回调
        /// <summary>
        /// 添加树状图窗口 初始化皮肤 开启内存处理 
        /// </summary>
        private void LoadIni()
        {
            /////////////////////////////////////////////////////////////
            //树状图窗口实例化
            threename = new ThreeName();
            //添加跨窗口调用委托事件
            threepoint = new ThreePoint();
            threescene = new ThreeScene();
            threetimer = new ThreeTimer();
            threebind = new ThreeBind();
            threelogic = new ThreeLogic();
            threeoperation = new ThreeOperation();
            threesection = new ThreeSection();
            threetitle = new ThreeTitle();

            dgvname = new DgvName();
            dgvpoint = new DgvPoint();
            dgvdevice = new DgvDevice();
            
            dgvscene = new DgvScene();
            dgvtimer = new DgvTimer();
            dgvbind = new DgvBind();
            dgvlogic = new DgvLogic();
            dgvoperation = new DgvOperation();

            //第一次加载基本窗口
            Control_Add(threename, plLeft);//默认添加窗口一
            Control_Add(threesection, plSection);
            Control_Add(dgvname, plDgv);
            Control_Add(threetitle, plTitleTree);

            //对象选择框调用
            threesection.addTitleNode += new AddTitleNode(threesection_addTitleNode);
            //显示加载窗口true 为网关 false 为设备
            threename.showDevice += new ShowDeviceDgv(threename_showDgvDevice);
            /////////////////////////////////////////////////////////////
            
            //txt窗口 信息显示 清空所有
            threename.sendFormContrl += new SendFormContrl(clearTxtShow);
            //txt窗口 信息显示 清空所有
            threescene.clearTxtShow += new Action<string>(clearTxtShow);
            threebind.clearTxtShow += new Action<string>(clearTxtShow);
            dgvpoint.txtAppShow += new Action<string>(AppTxtShow);
            dgvname.txtAppShow += new Action<string>(AppTxtShow);
            dgvscene.AppTxtShow += new Action<string>((msg) =>//TXT窗口显示信息 
            {
                AppTxtShow(msg);//后面直接加 非清空
            });

            dgvbind.AppTxtShow += new Action<string>((msg) =>//TXT窗口显示信息 
            {
                AppTxtShow(msg);//后面直接加 非清空
            });

            threetimer.clearTxtShow += new Action<string>(clearTxtShow);
            dgvtimer.AppTxtShow += new Action<string>(AppTxtShow);

            tnGateway.AppTxtShow += new Action<string>(AppTxtShow);
            tnDevice.AppTxtShow += new Action<string>(AppTxtShow);
            /////////////////////////////////////////////////////////////
            //  网关 根据DeviceList更新同步加载所有的TreeView
            //  os.UpdateTreeView += new Action<bool>((flag) =>
            //  {
            //      updataTreeNode();
            //
            //  });
            ////////////////////////////////////////////////////////////
            //刷新左栏树状图所有节点
            DataListHelper.UpdateTreeView += new Action(updataTreeNode);

           
            //DataListHelper.ClearTxtShow += new Action<string>(clearTxtShow);
            /////////////////////////////////////////////////////////////


            //刷新加载窗口
            //添加treeName给dgvname的委托 对象为空是 另外新建的一个窗口   
            threename.dgvNameAddItem += new DgvNameAddItem(dgvname.dgvNameAddItem);
            //TreeScene委托给Dgvscene窗口做
            threescene.dgvsceneAddItem += new DgvSceneAddItem(dgvscene.dgvsceneAddItem);
            //更新所有View的信息
            threescene.updateAllView += new Action(updataAllView);
            threetimer.updateAllView += new Action(updataAllView);
            //调用添加场景
            threetitle.dgvsceneAddItem += new DgvSceneAddItem2(dgvscene.dgvsceneAddItem);
            threetitle.dgvtimerAddItem += new Action(dgvtimer.TimerAddItem);
            threetitle.dgvbindAddItem  +=new DgvBindAddItem2(dgvbind.dgvbindAddItem);
            threetitle.addPoint += new Action<string>(dgvpoint.addPoint);
            threename.dgvDeviceAddItem += new DgvDeviceAddItem(dgvdevice.dgvDeviceAddItem);
            threebind.dgvbindAddItem += new DgvBindAddItem(dgvbind.dgvbindAddItem);
            threetimer.dgvTimerAddItem +=new Action(dgvtimer.TimerAddItem);
            threesection.sectionDgvNameAddItem += new SectionDgvNameAddItem(dgvname.dgvNameAddItem);
            threesection.sectionDgvDevAddItem += new SectionDgvDevAddItem(dgvdevice.dgvDeviceAddItem);
            threesection.updatePointDgv += new Action(dgvpoint.dgvPointAddItemBySection);
            threepoint.updateDgvPoint += new Action(dgvpoint.dgvPointAddItemByObjType);
            /////////////////////////////////////////////////////////////
            //光标事件调用 
            //添加加号光标
            //threesection.addSectionDevCursor += new AddSectionDevCursor(dgvdevice.cursor_copy);
            //threesection.addSectionNameCursor +=new AddSectionNameCursor(dgvname.cursor_copy);
            //threetitle.addTitleDevCursor += new AddTitleDevCursor(dgvdevice.cursor_copy);
            //threetitle.addTitlenNameCursor += new AddTitlenNameCursor(dgvname.cursor_copy);

            dgvdevice.updateSectionTitleNode += new Action(() =>//刷新右边两树状图 取消选中状态 
            {
                threesection.ThreeSEctionAddNode();
                threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            });
            dgvname.updateSectionTitleNode += new Action(() =>//刷新右边两树状图 取消选中状态 
            {
                threesection.ThreeSEctionAddNode();
                threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            });

            dgvpoint.updateTitleNode += new Action(() =>//刷新右边title状图 取消选中状态 
            {
                threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            });

            dgvpoint.updateAllView += new Action(updataAllView);
            //默认光标
            //dgvname.dgvDeviceCursorDefault +=new DgvDeviceCursorDefault(dgvdevice.cursor_default);
            //dgvdevice.dgvNameCursorDefault += new DgvNameCursorDefault(dgvname.cursor_default);
            /////////////////////////////////////////////////////////////
            //dgv窗体跳转
            dgvscene.jumpSetInfo += new Action<DataJson.PointInfo>(dgv_jumpSetInfo);

            /////////////////////////////////////////////////////
            //初始化类型表
            cbtypeName("equipment");//初始化title类型表               
            timer1.Enabled = false; //开启清除内存时钟
            this.Text = Resources.SoftName + "Edit New Project";
        }

        #region 刷新窗口
        /// <summary>
        /// 刷新左栏树状图的节点
        /// </summary>
        private void updataTreeNode()
        {
            threename.ThreeNameAddNode();
            threescene.ThreeSceneAddNode();
            threetimer.ThreeTimerAddNode();
            threebind.ThreeBindAddNode();
            threeoperation.ThreeOperationAddNode();
            threelogic.ThreeLogicAddNode();
            threesection.ThreeSEctionAddNode();
            //threepoint.ThreePointAddNode();
        }

        /// <summary>
        /// 更新窗口所有信息
        /// </summary>
        private void updataAllView()
        {
            threename.ThreeNameAddNode();
            threescene.ThreeSceneAddNode();
            threetimer.ThreeTimerAddNode();
            threebind.ThreeBindAddNode();
            threeoperation.ThreeOperationAddNode();
            threelogic.ThreeLogicAddNode();
            threesection.ThreeSEctionAddNode();
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex); 

            //dgvdevice.dgvDeviceAddItem();
            dgvname.dgvNameAddItem();
            dgvpoint.dgvPointAddItemBySection();
            dgvscene.dgvsceneAddItem();
            dgvbind.dgvbindAddItem();
            dgvtimer.TimerAddItem();
            txtShow.Clear();
        }

        private void updateDgv()
        {
            switch (FileMesege.formType)
            {
                case "name":
                    if (FileMesege.tnselectNode != null && FileMesege.tnselectNode.Parent == null)
                    {
                        dgvdevice.dgvDeviceAddItem();
                    }
                    else
                    {
                        dgvname.dgvNameAddItem();
                    }
                    break;

                case "point":
                    dgvpoint.dgvPointAddItemBySection();
                    break;
                case "scene":
                    dgvscene.dgvsceneAddItem();
                    break;
                case "timer":
                   
                    break;
                case "bind":
                    dgvbind.dgvbindAddItem();
                    break;
                case "logic":
                   
                    break;
                case "operation":
                    
                    break;
                default: break;
            }
        }
        #endregion


        /// <summary>
        /// 跳转窗口  
        /// </summary>
        /// <param name="point"></param>
        private void dgv_jumpSetInfo(DataJson.PointInfo point)
        {
            ///////////////////////后续还要补充
            switch (point.type.Split('_')[1])
            {
                case "scene":
                    tabScene_Click(this, EventArgs.Empty);
                    tabStrip.SelectedTab = tabScene;
                    threescene.FindNodeSelect(point);
                    break;

                case "time":
                    tabTimer_Click(this, EventArgs.Empty);
                    tabStrip.SelectedTab = tabTimer;
                    threetimer.FindNodeSelect(point);
                    break;

                case "group":
                    tabPanel_Click(this, EventArgs.Empty);
                    tabStrip.SelectedTab = tabPanel;
                    threebind.FindNodeSelect(point);
                    break;

                default:

                    break;
            }
        }

        

        /// <summary>
        /// DgvName和DgvDevice两个框改变
        /// </summary>
        /// <param name="flag">显示加载窗口true 为网关 false 为设备</param>
        public void threename_showDgvDevice(bool flag)
        {
            if (flag)
            {
                Control_Add(dgvdevice, plDgv);
            }
            else
            {
                Control_Add(dgvname, plDgv);
            }
            
        }


        /// <summary>
        /// 加载 窗口 
        /// </summary>
        /// <param name="form">窗口</param>
        /// <param name="panel">panel容器</param>
        private void Control_Add(Form form, Panel panel)
        {
            panel.Controls.Clear();    //移除所有控件  
            form.TopLevel = false;      //设置为非顶级窗体  
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; //设置窗体为非边框样式  
            form.Dock = System.Windows.Forms.DockStyle.Fill;                  //设置样式是否填充整个panel  
            panel.Controls.Add(form);        //添加窗体  
            form.Show();                      //窗体运行  
        }

        /// <summary>
        /// 窗体显示信息 清空信息后显示
        /// </summary>
        /// <param name="msg"></param>
        public void clearTxtShow(string msg)
        {
            txtShow.Clear();
            txtShow.AppendText(msg);
        }

        /// <summary>
        /// 窗体显示信息 直接换行追加
        /// </summary>
        /// <param name="msg"></param>
        public void AppTxtShow(string msg)
        {
            txtShow.AppendText(string.Format("{0}\r\n", msg));
        }

        //清除txt信息
        private void btnInfoClear_Click(object sender, EventArgs e)
        {
            txtShow.Clear();
        }


        /// <summary>
        /// treetitle对象选择框改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {

            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
        }

        
        /// <summary>
        /// TitleTree 回调 根据按钮选择Cbtype改变类型
        /// </summary>
        public void threesection_addTitleNode()
        {
            cbType_SelectedIndexChanged(null,EventArgs.Empty);
        }

        #endregion


        #region 左栏按钮 命名 场景 时钟 绑定 逻辑 运算 cbtype 读取

        private void tabName_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threename, plLeft);
            if (FileMesege.tnselectNode != null && FileMesege.tnselectNode.Parent == null)
            {
                Control_Add(dgvdevice, plDgv);
            }
            else
            {
                Control_Add(dgvname, plDgv);
            }
            tabName.ImageIndex = 6;
            //cbtype添加选择项 
            cbtypeName("equipment");
            //界面显示类型 
            FileMesege.formType = "name";
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleDevice;
            dgvname.dgvNameAddItem();
        }

        private void tabPoint_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threepoint, plLeft);
            Control_Add(dgvpoint, plDgv);
            //cbtype添加选择项 
            cbtypeName("point");
            //界面显示类型 
            FileMesege.formType = "point";
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            threepoint.ThreePointAddNode();
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleDevice;
            //更新加载表格
            dgvpoint.dgvPointAddItemBySection();
        }

        private void tabScene_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threescene, plLeft);
            Control_Add(dgvscene, plDgv);
            //界面显示类型 
            FileMesege.formType = "scene";
            //cbtype添加选择项
            cbtypeName("scene");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
            dgvscene.dgvsceneAddItem();
        }

        private void tabTimer_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threetimer, plLeft);
            Control_Add(dgvtimer, plDgv);
            //界面显示类型 
            FileMesege.formType = "timer";
            //cbtype添加选择项 
            cbtypeName("timer");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
        }

        //面板绑定
        private void tabPanel_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threebind, plLeft);
            Control_Add(dgvbind, plDgv);
            //界面显示类型 
            FileMesege.formType = "panel";
            //cbtype添加选择项 
            cbtypeName("panel");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
        }

        //感应设置
        private void tabReaction_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threeoperation, plLeft);
            Control_Add(dgvoperation, plDgv);
            //界面显示类型 
            FileMesege.formType = "reaction";
            //cbtype添加选择项 
            cbtypeName("reaction");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
        }

        private void tabLogic_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threelogic, plLeft);
            Control_Add(dgvlogic, plDgv);
            //界面显示类型 
            FileMesege.formType = "logic";
            //cbtype添加选择项 
            cbtypeName("logic");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
        }


        /// <summary>
        /// cb框添加命名的信息 typeName参数为ini文件名
        /// </summary>
        /// <param name="typeName"></param>
        private void cbtypeName(string typeName)
        {
            threetitle.keys.Clear();
            if (typeName == "point")
            {
                DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "\\objs");
                string name = "";
                foreach (FileInfo file in folder.GetFiles("*.ini"))
                {

                    name = IniConfig.GetValue(file.FullName, "define", "name");
                    if (name != "")
                    {
                        threetitle.keys.Add(name);
                    }

                }
            }
            else
            {
                threetitle.inifilepath = Application.StartupPath + "\\names\\commonName.ini";
                threetitle.keys = IniConfig.ReadKeys(typeName, threetitle.inifilepath);
            }
            cbType.Text = "";
            cbType.Items.Clear();
            for (int i = 0; i < threetitle.keys.Count; i++)
            {
                //添加所有的keys项
                cbType.Items.Add(threetitle.keys[i]);
            }
            if (cbType.Items.Count > 0)
            {
                //如果有加载 默认选中第一项
                cbType.SelectedIndex = 0;
            }
            
           
        }

        private void btnStyleIni()
        {
           
            
        }
        #endregion


        #region 内存回收

        private void timer1_Tick(object sender, EventArgs e)
        {
            ClearMemory();
        }
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion


        #region Muilt 新建文件 打开文件 另存为 保存 退出 窗体关闭
        private void 新建项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
            if (fm.newfile())
            {
                
                    this.Text = Resources.SoftName + "Edit New Project";
                    txtShow.AppendText("新建工程\r\n");
                    updataAllView();
                    threesection.ThreeSEctionAddNode();
                    //添加对象树状图
                    threetitle.ThreeTitleAddNode(cbType.SelectedIndex);  
               
            }
            else
            {
                
                txtShow.AppendText("新建工程失败\r\n");
            }           
        }

        private void 打开项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
            if (fm.openfile())
            {
                
                    txtShow.AppendText("打开工程成功\r\n");
                    this.Text = Resources.SoftName + FileMesege.filePath;
                    //刷新所有窗口
                    updataAllView();                
                 
               
            }
            else
            {
                txtShow.AppendText("打开工程失败\r\n");
            }            
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
           
            if ( fm.savefile())
            {

                txtShow.AppendText("保存工程成功\r\n");
            }
            else
            {
                txtShow.AppendText("保存工程失败\r\n");
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
            if (fm.othersavefile())
            {
                txtShow.AppendText("另存为工程成功\r\n");
            }
            else
            {
                txtShow.AppendText("另存为工程失败\r\n");
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tsNewfile_Click(object sender, EventArgs e)
        {
            新建项目ToolStripMenuItem_Click(this,EventArgs.Empty);
        }

        private void tsOpenfile_Click(object sender, EventArgs e)
        {
            打开项目ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void tsSave_Click(object sender, EventArgs e)
        {
            保存ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void tsOtherSave_Click(object sender, EventArgs e)
        {
            另存为ToolStripMenuItem_Click(this, EventArgs.Empty);
        }


    

        private void 最近打开ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            string[] filepath =  IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path").Split(',');
            最近打开ToolStripMenuItem.DropDownItems.Clear();
            if (string.IsNullOrEmpty( filepath[0]) && filepath.Length ==1)
            {
                ToolStripMenuItem tsmi2 = new ToolStripMenuItem("(none)");
                //添加子菜单
                最近打开ToolStripMenuItem.DropDownItems.Add(tsmi2);
            }
            else
            {
                for (int i = 0; i < filepath.Length; i++)
                {
                    if (string.IsNullOrEmpty(filepath[i]))
                    {
                        continue;
                    }
                    ToolStripMenuItem tsmi2 = new ToolStripMenuItem(filepath[i]);
                    //绑定子菜单点击事件
                    tsmi2.Click += DemoClick;
                    //添加子菜单
                    最近打开ToolStripMenuItem.DropDownItems.Add(tsmi2);
                }
            }
            ToolStripSeparator link = new ToolStripSeparator();
            //添加分割线
            最近打开ToolStripMenuItem.DropDownItems.Add(link);
            ToolStripMenuItem clearBtn = new ToolStripMenuItem("清除历史");
            //绑定子菜单点击事件
            clearBtn.Click += 清除历史ToolStripMenuItem_Click;
            //添加子菜单
            最近打开ToolStripMenuItem.DropDownItems.Add(clearBtn);
            
            
        }

        //自己定义个点击事件需要执行的动作
        private void DemoClick(object sender, EventArgs e)
        {
            ToolStripMenuItem but = sender as ToolStripMenuItem;
            FileMesege fm = new FileMesege();
            if (fm.readProject(but.Text))
            {

                txtShow.AppendText("打开工程成功\r\n");
                this.Text = Resources.SoftName + FileMesege.filePath;
                //刷新所有窗口
                updataAllView();


            }
            else
            {
                txtShow.AppendText("打开工程失败\r\n");
            }   
            
        }

        private void 清除历史ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", "");

        }

        private void 导出ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //窗体关闭
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileMesege fm  = new FileMesege();
            fm.formclosing(e);
            
        }
        #endregion


        #region 编辑 快捷键

        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanUnDo)
                return;

            FileMesege.cmds.UnDo();
            updataAllView();
        }

        private void tsBackOut_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanUnDo)
                return;

            FileMesege.cmds.UnDo();
            updataAllView();
        }

        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanReDo)
                return;

            FileMesege.cmds.ReDo();
            updataAllView();
        }

        private void tsReform_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanReDo)
                return;

            FileMesege.cmds.ReDo();
            updataAllView();
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 复制CCtrlCToolStripMenuItem_Click(object sender, EventArgs e)
        {
       
            switch (FileMesege.formType)
            {
                case "name":

                    break;

                case "point":
                    dgvpoint.copyData();
                    break;
                case "scene":
                    dgvscene.copyData();
                    break;
                case "timer":
                    dgvtimer.copyData();
                    break;
                case "bind":
                    break;
                case "logic":

                    break;
                case "operation":

                    break;
                default: break;
            }
        }

        private void 粘贴PCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
    
            switch (FileMesege.formType)
            {
                case "name":

                    break;

                case "point":
                    dgvpoint.pasteData();
                    break;
                case "scene":
                    dgvscene.pasteData();
                    break;
                case "timer":
                    dgvtimer.pasteData();
                    break;
                case "bind":
                    break;
                case "logic":

                    break;
                case "operation":

                    break;
                default: break;
            }
        }

        private void 查找FCtrlFToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 替换ECtrlHToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 全选AToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 删除DToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion


        #region 功能
        private void 设备在线搜索ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //在线栏
            os = new OnlineSearch();
            os.TxtShow += new Action<string>((msg) =>//TXT窗口显示信息 
            {
                AppTxtShow(msg);//后面直接加 非清空
            });
            //展示居中
            os.StartPosition = FormStartPosition.CenterParent;
            os.ShowDialog();
        }

        private void 时钟校时ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 按键检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 固件更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 编译ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 编译下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion


        #region 工具
        private void 应急状态设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 天文时钟设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 管理密码设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 节假日设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion


        #region 帮助
        private void 欢迎ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 关于软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion


        #region 快捷键功能 因为在meustrip设置了快捷键 所以这个快捷键可以删除
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Control )
            {
                switch (e.KeyCode)
                {

                    case Keys.N:
                       // 新建项目ToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
                    case Keys.S:
                    //    保存ToolStripMenuItem_Click(this, EventArgs.Empty);
                        break;
          
                    
                }
            }
        }

        #endregion

 

   
  



     

     
   
   



































    }
}
 