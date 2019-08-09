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
        public ThreePanel threepanel;
        public ThreeLogic threelogic;
        public ThreeSensor threesensor;
        public ThreeSection threesection;
        public ThreeTitle threetitle;
        public ThreeVar threevar;


        public DgvName dgvname;
        public DgvPoint dgvpoint;
        public DgvDevice dgvdevice;
        public DgvScene dgvscene;
        public DgvTimer dgvtimer;
        public DgvPanel dgvpanel;
        public DgvLogic dgvlogic;
        public DgvSensor dgvsensor;
        public DgvVar dgvvar;
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
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
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
            threepanel = new ThreePanel();
            threelogic = new ThreeLogic();
            threesensor = new ThreeSensor();
            threesection = new ThreeSection();
            threetitle = new ThreeTitle();
            threevar = new ThreeVar();

            dgvname = new DgvName();
            dgvpoint = new DgvPoint();
            dgvdevice = new DgvDevice();
            
            dgvscene = new DgvScene();
            dgvtimer = new DgvTimer();
            dgvpanel = new DgvPanel();
            dgvlogic = new DgvLogic();
            dgvsensor = new DgvSensor();
            dgvvar = new DgvVar();

            //第一次加载基本窗口
            Control_Add(threename, plLeft);//默认添加窗口一
            Control_Add(threesection, plSection);
            Control_Add(dgvname, plDgv);
            Control_Add(threetitle, plTitleTree);

            //刷新titleNode节点
            threesection.addTitleNode += new AddTitleNode(threesection_addTitleNode);
            threename.addTitleNode += new Action(threesection_addTitleNode);
            threepanel.addTitleNode += new Action(threesection_addTitleNode);
            threescene.addTitleNode += new Action(threesection_addTitleNode);
            threetimer.addTitleNode += new Action(threesection_addTitleNode);
            threesensor.addTitleNode += new Action(threesection_addTitleNode);
            //显示加载窗口true 为网关 false 为设备
            threename.showDevice += new ShowDeviceDgv(threename_showDgvDevice);
            /////////////////////////////////////////////////////////////
            
            //txt窗口 信息显示 清空所有
            threename.sendFormContrl += new SendFormContrl(clearTxtShow);
            //txt窗口 信息显示 清空所有
            threescene.clearTxtShow += new Action<string>(clearTxtShow);
            threepanel.clearTxtShow += new Action<string>(clearTxtShow);
            threesensor.clearTxtShow += new Action<string>(clearTxtShow);
            threevar.clearTxtShow += new Action<string>(clearTxtShow);
            dgvpoint.txtAppShow += new Action<string>(AppTxtShow);
            dgvname.txtAppShow += new Action<string>(AppTxtShow);
            dgvdevice.AppTxtShow += new Action<string>(AppTxtShow);
            dgvscene.AppTxtShow += new Action<string>((msg) =>//TXT窗口显示信息 
            {
                AppTxtShow(msg);//后面直接加 非清空
            });

            dgvpanel.AppTxtShow += new Action<string>((msg) =>//TXT窗口显示信息 
            {
                AppTxtShow(msg);//后面直接加 非清空
            });
            dgvsensor.AppTxtShow += new Action<string>(AppTxtShow);
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
            threescene.updateSceneView += new Action(updateDgvTreeByFormType);
            threetimer.updateTimerView += new Action(updateDgvTreeByFormType);
            threepanel.updatePanelView += new Action(updateDgvTreeByFormType);
            threesensor.updateSensorView += new Action(updateDgvTreeByFormType);
            //调用添加场景
            threetitle.dgvsceneAddItem += new DgvSceneAddItem2(dgvscene.dgvsceneAddItem);
            threetitle.dgvtimerAddItem += new Action(dgvtimer.TimerAddItem);
            threetitle.dgvVariableAddItem += new Action(dgvvar.dgvVarAddItem);
            threetitle.addPoint += new Action<string>(dgvpoint.addPoint);
            threetitle.addVariable += new Action<string>(dgvvar.addVariable);
            threename.dgvDeviceAddItem += new DgvDeviceAddItem(dgvdevice.dgvDeviceAddItem);
            threepanel.dgvpanelAddItem += new DgvPanelAddItem(dgvpanel.dgvPanelAddItem);
            threetimer.dgvTimerAddItem +=new Action(dgvtimer.TimerAddItem);
            threesensor.dgvSensorAddItem +=new Action(dgvsensor.dgvSensorAddItem);
            threevar.dgvVarAddItem +=new Action(dgvvar.dgvVarAddItem);

            //区域按当前窗口来更新
            threesection.sectionUpdateDgvTreeByFormType += new Action(updateDgvTreeByFormType);
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

            dgvpoint.updatePointView += new Action(updateDgvTreeByFormType);

            dgvpanel.updateSectionTitleNode += new Action(() =>//刷新右边两树状图 取消选中状态 
            {
                //threesection.ThreeSEctionAddNode();
                threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            });
            dgvsensor.updateSectionTitleNode += new Action(() =>//刷新右边两树状图 取消选中状态 
            {
                //threesection.ThreeSEctionAddNode();
                threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            });
            //默认光标
            //dgvname.dgvDeviceCursorDefault +=new DgvDeviceCursorDefault(dgvdevice.cursor_default);
            //dgvdevice.dgvNameCursorDefault += new DgvNameCursorDefault(dgvname.cursor_default);
            /////////////////////////////////////////////////////////////
            //dgv窗体跳转
            dgvscene.jumpSetInfo += new Action<DataJson.PointInfo>(dgv_jumpSetInfo);
            dgvtimer.jumpSetInfo += new Action<DataJson.PointInfo>(dgv_jumpSetInfo);
            dgvpanel.jumpSetInfo += new Action<DataJson.PointInfo>(dgv_jumpSetInfo);
            dgvsensor.jumpSetInfo += new Action<DataJson.PointInfo>(dgv_jumpSetInfo);
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
            threepanel.ThreePanelAddNode();
            threesensor.ThreeSensorAddNode();
            threelogic.ThreeLogicAddNode();
            threesection.ThreeSEctionAddNode();
            threevar.ThreeVarAddNode();
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
            threepanel.ThreePanelAddNode();
            threesensor.ThreeSensorAddNode();
            threevar.ThreeVarAddNode();

            threelogic.ThreeLogicAddNode();
            threesection.ThreeSEctionAddNode();
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);

            dgvdevice.dgvDeviceAddItem(true);
            dgvname.dgvNameAddItem();
            dgvpoint.dgvPointAddItemBySection();
            dgvscene.dgvsceneAddItem();
            dgvpanel.dgvPanelAddItem();
            dgvtimer.TimerAddItem();
            dgvsensor.dgvSensorAddItem();
            dgvvar.dgvVarAddItem();
            txtShow.Clear();
        }

        /// <summary>
        /// 根据选择窗口类型更新窗口
        /// </summary>
        private void updateDgvTreeByFormType()
        {
            switch (FileMesege.formType)
            {
                case "name":
                    if (FileMesege.tnselectNode != null && FileMesege.tnselectNode.Parent == null)
                    {
                        dgvdevice.dgvDeviceAddItem(true);
                    }
                    else
                    {
                        dgvname.dgvNameAddItem();
                    }
                    threename.ThreeNameAddNode();
                    break;

                case "point":
                    dgvpoint.dgvPointAddItemBySection();
                    break;
                case "scene":
                    threescene.ThreeSceneAddNode();
                    dgvscene.dgvsceneAddItem();
                    break;
                case "timer":
                    threetimer.ThreeTimerAddNode();
                    dgvtimer.TimerAddItem();
                    break;
                case "panel":
                    threepanel.ThreePanelAddNode();
                    dgvpanel.dgvPanelAddItem();
                    break;
                case "sensor":
                    threesensor.ThreeSensorAddNode();
                    dgvsensor.dgvSensorAddItem();
                    break;
                case "logic":

                    break;
                case "variable":
                    threevar.ThreeVarAddNode();
                    dgvvar.dgvVarAddItem();
                    break;

                default: break;
            }

            threesection.ThreeSEctionAddNode();
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            txtShow.Clear();
        }

        #endregion


        /// <summary>
        /// 跳转窗口  
        /// </summary>
        /// <param name="point"></param>
        private void dgv_jumpSetInfo(DataJson.PointInfo point)
        {
            //////////////////////////////////////////后续还要补充
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

                case "panel":
                    tabPanel_Click(this, EventArgs.Empty);
                    tabStrip.SelectedTab = tabPanel;
                    threepanel.FindNodeSelect(point);
                    break;
                case "sensor":
                    tabSensor_Click(this, EventArgs.Empty);
                    tabStrip.SelectedTab = tabSensor;
                    threesensor.FindNodeSelect(point);
                    break;

                default:

                    break;
            }
        }


        bool isGwDgv = false;
        /// <summary>
        /// DgvName和DgvDevice两个框改变
        /// </summary>
        /// <param name="flag">显示加载窗口true 为网关 false 为设备</param>
        public void threename_showDgvDevice(bool flag)
        {
            if (flag)
            {
                Control_Add(dgvdevice, plDgv);
                isGwDgv = true;
            }
            else
            {
                Control_Add(dgvname, plDgv);
                isGwDgv = false;
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

        #region 文本框操作信息

        int clearNum = 0;
        /// <summary>
        /// 窗体显示信息 清空信息后显示
        /// </summary>
        /// <param name="msg"></param>
        public void clearTxtShow(string msg)
        {
            clearNum = 0;
            txtShow.Clear();
            txtShow.AppendText(msg);
        }

        /// <summary>
        /// 窗体显示信息 直接换行追加
        /// </summary>
        /// <param name="msg"></param>
        public void AppTxtShow(string msg)
        {

            string tmp = string.Format("({1}) {0}\r\n", msg, DateTime.Now.ToLongTimeString());
            
            if (clearNum == 2)
            {
                txtShow.AppendText(tmp);
                this.txtShow.ScrollToCaret();
            }
            else
            {
                this.txtShow.ScrollToCaret();
                txtShow.AppendText(tmp);
            }
            //来一个延时
            SocketUtil.DelayMilli(300);
            txtShow.SelectAll();
            this.txtShow.SelectionColor = Color.Black;
     
            txtShow.Select(txtShow.Text.Length - tmp.Length + 1,tmp.Length);
            this.txtShow.SelectionColor = Color.Red;
            clearNum++;
            
        }

        //清除txt信息
        private void btnInfoClear_Click(object sender, EventArgs e)
        {
            txtShow.Clear();
        }
        #endregion

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


        #region 按钮 设备 场景 时钟 绑定 逻辑 运算 cbtype 读取

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
            dgvtimer.TimerAddItem();
        }

        //面板绑定
        private void tabPanel_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threepanel, plLeft);
            Control_Add(dgvpanel, plDgv);
            //界面显示类型 
            FileMesege.formType = "panel";
            //cbtype添加选择项 
            cbtypeName("link");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
            dgvpanel.dgvPanelAddItem();
        }

        //感应设置
        private void tabSensor_Click(object sender, EventArgs e)
        {
            
   
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threesensor, plLeft);
            Control_Add(dgvsensor, plDgv);
            //界面显示类型 
            FileMesege.formType = "sensor";
            //cbtype添加选择项 
            cbtypeName("sensor");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
            dgvsensor.dgvSensorAddItem();
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


        private void tabVar_Click(object sender, EventArgs e)
        {
            //自定义函数加载窗体 CleanRecycle  
            Control_Add(threevar, plLeft);
            Control_Add(dgvvar, plDgv);
            //界面显示类型 
            FileMesege.formType = "variable";
            //cbtype添加选择项 
            cbtypeName("variable");
            //添加对象树状图
            threetitle.ThreeTitleAddNode(cbType.SelectedIndex);
            //更改Title 小标题
            LbTitleName.Text = Resources.lbTitleObj;
            dgvvar.dgvVarAddItem();
        }

        /// <summary>
        /// cb框添加命名的信息 typeName参数为ini文件名
        /// </summary>
        /// <param name="typeName"></param>
        private void cbtypeName(string typeName)
        {

            ThreeTitle.keys.Clear();
            if (typeName == "point")
            {
                DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "\\objs");
                string name = "";
                foreach (FileInfo file in folder.GetFiles("*.ini"))
                {

                    name = IniConfig.GetValue(file.FullName, "define", "name");
                    if (name != "")
                    {
                        ThreeTitle.keys.Add(name);
                    }

                }
            }
            else
            {
                threetitle.inifilepath = Application.StartupPath + "\\names\\commonName.ini";
                ThreeTitle.keys = IniConfig.ReadKeys(typeName, threetitle.inifilepath);
            }
            cbType.Text = "";
            cbType.Items.Clear();
            for (int i = 0; i < ThreeTitle.keys.Count; i++)
            {
                //添加所有的keys项
                cbType.Items.Add(ThreeTitle.keys[i]);
            }
            if (cbType.Items.Count > 0)
            {
                //如果有加载 默认选中第一项
                cbType.SelectedIndex = 0;
            }
            
           
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
                    updataAllView();
                    txtShow.Clear();
                    AppTxtShow("新建工程");
            }
            else
            {

                AppTxtShow("新建工程失败");
            }           
        }

        private void 打开项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
            if (fm.openfile())
            {

                    AppTxtShow("打开工程成功");
                    this.Text = Resources.SoftName + FileMesege.filePath;
                    //刷新所有窗口
                    updataAllView();                
                 
               
            }
            else
            {
                AppTxtShow("打开工程失败");
            }            
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
           
            if ( fm.savefile())
            {

                AppTxtShow("保存工程成功");
            }
            else
            {
                AppTxtShow("保存工程失败");
            }
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileMesege fm = new FileMesege();
            if (fm.othersavefile())
            {
                AppTxtShow("另存为工程成功");
            }
            else
            {
                AppTxtShow("另存为工程失败");
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("若未保存工程，窗口关闭后，数据即将丢失！是否现在关闭窗口", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
               
                return;
            }
            else
            {
                System.Environment.Exit(0);
            }
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
        private void btnOpen_Click(object sender, EventArgs e)
        {
            打开项目ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            保存ToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        #endregion


        #region 编辑 快捷键

        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanUnDo)
                return;

            FileMesege.cmds.UnDo();
            updateDgvTreeByFormType();
        }



        private void 重做ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanReDo)
                return;

            FileMesege.cmds.ReDo();
            updateDgvTreeByFormType();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanUnDo)
                return;

            FileMesege.cmds.UnDo();
            updateDgvTreeByFormType();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (FileMesege.cmds == null || !FileMesege.cmds.CanReDo)
                return;

            FileMesege.cmds.ReDo();
            updateDgvTreeByFormType();
        }



        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            复制CCtrlCToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void btnPast_Click(object sender, EventArgs e)
        {
            粘贴PCtrlVToolStripMenuItem_Click(this, EventArgs.Empty);
        }

        private void 复制CCtrlCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                switch (FileMesege.formType)
                {
                    case "name":
                        if (isGwDgv)
                        {
                            dgvdevice.copyData();
                        }
                        
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
                    case "panel":
                        dgvpanel.copyData();
                        break;
                    case "logic":

                        break;
                    case "sensor":
                        dgvsensor.copyData();
                        break;
                    case "variable":
                        break;
                    default: break;
                }
            }
            catch
            {
                MessageBox.Show("复制出现问题错误号858");
            }
        }

        private void 粘贴PCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                switch (FileMesege.formType)
                {
                    case "name":
                        if (isGwDgv)
                        {
                            dgvdevice.pasteData();
                        }
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
                    case "panel":
                        dgvpanel.pasteData();
                        break;
                    case "logic":

                        break;
                    case "sensor":
                        dgvsensor.pasteData();
                        break;
                    case "variable":
                        break;
                    default: break;
                }
            }
            catch
            {
                MessageBox.Show("粘贴出现问题错误号895");
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
            correctionTime ctTimer = new correctionTime();
            ctTimer.StartPosition = FormStartPosition.CenterParent;
            ctTimer.AppTxtShow += new Action<string>(AppTxtShow);
            ctTimer.ShowDialog();
        }

        private void 按键检测ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 固件更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            systemPrjUpdate prjUpdate = new systemPrjUpdate();
            prjUpdate.AppTxtShow += new Action<string>(AppTxtShow);
            //展示居中
            prjUpdate.StartPosition = FormStartPosition.CenterParent;
            prjUpdate.ShowDialog();
        }

 

        private void 编译下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compileDownload cpdown = new compileDownload();
            cpdown.AppTxtShow += new Action<string>(AppTxtShow);
            //展示居中
            cpdown.StartPosition = FormStartPosition.CenterParent;
            cpdown.ShowDialog();
        }

        private void btnCompileDownload_Click_1(object sender, EventArgs e)
        {
            compileDownload cpdown = new compileDownload();
            cpdown.AppTxtShow += new Action<string>(AppTxtShow);
            //展示居中
            cpdown.StartPosition = FormStartPosition.CenterParent;
            cpdown.ShowDialog();
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

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 关于软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            versionInfo ver = new versionInfo();
            //把窗口向屏幕中间刷新
            ver.StartPosition = FormStartPosition.CenterScreen;
            ver.Show();
            
        }

        #endregion

     

     





     

 

       

       

      
        

 

      
    

      

 

   
  



     

     
   
   



































    }
}
 