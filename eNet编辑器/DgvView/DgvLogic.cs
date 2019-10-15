using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using DevComponents.DotNetBar;

namespace eNet编辑器.DgvView
{
    public partial class DgvLogic : Form
    {

        public SuperTabControl superTabControl1;

        public LogicScene logicScene;

        public LogicCondition logicCondition;

        public LogicVoice logicVoice;

        public  LogicType logicType = 0;

        public enum LogicType
        {
            /// <summary>
            /// 场景类型
            /// </summary>
            Scene = 1,
            /// <summary>
            /// 多条件处理
            /// </summary>
            Condition = 2,
            /// <summary>
            /// 表达式处理
            /// </summary>
            Voice = 3,
           

        }


       

        public DgvLogic()
        {
            
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            this.UpdateStyles();
        }

        private void DgvLogic_Load(object sender, EventArgs e)
        {
            IniControl();
        }


        #region 控件初始化
        /// <summary>
        /// 控件初始化
        /// </summary>
        private void IniControl()
        {
            SuperTabControllCreat();
            formCreat();
            //Tab表格关闭按钮
            superTabControl1.CloseButtonOnTabsVisible = true;
            //清空表格
            clearSuperTabControl1();

        }

        /// <summary>
        /// 新建superTabControl1控件 解决黑闪问题
        /// </summary>
        private void SuperTabControllCreat()
        {
            panel2.Controls.Clear();
            superTabControl1 = null;
            superTabControl1 = new SuperTabControl();

            superTabControl1.Dock = DockStyle.Fill;
            superTabControl1.CloseButtonOnTabsVisible = true;
            superTabControl1.MouseClick +=new MouseEventHandler(superTabControl1_MouseClick);
            superTabControl1.TabStripMouseUp +=new EventHandler<MouseEventArgs>(superTabControl1_TabStripMouseUp);
            superTabControl1.SelectedTabChanged += new EventHandler<SuperTabStripSelectedTabChangedEventArgs>(superTabControl1_SelectedTabChanged);
            superTabControl1.ControlBox.MenuBox.PopupOpen += new DotNetBarManager.PopupOpenEventHandler(superTabControl1_ControlBox_MenuBox_PopupOpen);
            superTabControl1.TabItemClose += new EventHandler<SuperTabStripTabItemCloseEventArgs>(superTabControl1_TabItemClose);
            panel2.Controls.Add(superTabControl1);

           
        }

       

        /// <summary>
        /// 创建窗口对象  在删除的时候会把对象释放掉
        /// </summary>
        private void formCreat()
        {
            if (logicScene == null || logicScene.IsDisposed)
            {
                logicScene = new LogicScene();
                logicScene.Dock = DockStyle.Fill;
            }
            if (logicCondition == null || logicCondition.IsDisposed)
            {
                logicCondition = new LogicCondition();
                logicCondition.Dock = DockStyle.Fill;
            }
            if (logicVoice == null || logicVoice.IsDisposed)
            {
                logicVoice = new LogicVoice();
                logicVoice.Dock = DockStyle.Fill;
            }

        }

        /// <summary>
        /// 回调场景更新cbScene的内容
        /// </summary>
        public void delegeteLogicCbSceneGetItem()
        {
            try
            {
                if (superTabControl1.SelectedTab.Text.Contains("场景处理"))
                {
                    string ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
                    logicScene.cbSceneGetItem(ip);
                }
            }
            catch
            { 
            
            }
        }

        #endregion
  

        #region 加载信息
        public void dgvLogicAddItem()
        {
            //清空表单
            //superTabControl1.Hide();解决黑闪失败
            //superTabControl1.Tabs.Clear();
            DataJson.logics lgs = DataListHelper.getLogicInfoListByNode();
            if (lgs == null)
            {
                clearSuperTabControl1();
                //superTabControl1.Show();解决黑闪失败
                return;
            }
            SuperTabControllCreat();
            foreach (DataJson.logicsInfo lginfo in lgs.logicsInfo)
            {
                //加载各个框
                switch (lginfo.modelType)
                {
                    case "SceneDeal":
                        AddNewSceneTab();
                        break;
                    case "ConditionDeal":
                        AddNewConditionTab(lginfo.id);
                        break;
                    case "VoiceDeal":
                        AddNewVoiceTab(lginfo.id);
                        break;
                    default:
                        break;
                }

            }
            try
            {
                //恢复上次选中
                superTabControl1.SelectedTabIndex = FileMesege.LogicSelectedTabIndex;
            }
            catch
            {
                //选中第一个
                superTabControl1.SelectedTabIndex = 0;
            }

        }

        //用于恢复上一次选中框
        private void superTabControl1_TabStripMouseUp(object sender, MouseEventArgs e)
        {
            FileMesege.LogicSelectedTabIndex = superTabControl1.SelectedTabIndex;
        }
        

        /// <summary>
        /// 清空表框 并加载空白的项
        /// </summary>
        private void clearSuperTabControl1()
        {
            //superTabControl1.Tabs.Clear();
            SuperTabControllCreat();
            SuperTabItem tab = superTabControl1.CreateTab("LogicSet", superTabControl1.Tabs.Count);
            superTabControl1.SelectedTab = tab;

            LabelX lbx = new LabelX();
            lbx.Text = "Easy Control";
            lbx.TextAlignment = StringAlignment.Center;
            lbx.TextLineAlignment = StringAlignment.Center;
            lbx.BackColor = Color.Transparent;

            tab.AttachedControl.Controls.Add(lbx);
            lbx.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 选中加载信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superTabControl1_SelectedTabChanged(object sender, SuperTabStripSelectedTabChangedEventArgs e)
        {
            SuperTabItem tab = superTabControl1.SelectedTab;
            DataJson.logicsInfo logicInfo = DataListHelper.findLogicInfoByTabName(tab.Text);
            //记录当前tab表的信息
            FileMesege.LogicTabName = tab.Text;
           
            if (logicInfo == null)
            {
                return;
            }

            tabSelectAddPanel(logicInfo);
            
        }

        private void superTabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        /// <summary>
        /// 根据LogicInfo.modelType类型加载框和内容
        /// </summary>
        /// <param name="modelType"></param>
        private void tabSelectAddPanel(DataJson.logicsInfo logicInfo)
        {
            formCreat();
            //加载各个框的Panel信息
            switch (logicInfo.modelType)
            {
                case "SceneDeal":
                    logicType = LogicType.Scene;
                    //显示场景处理框
                    superTabControl1.SelectedTab.AttachedControl.Controls.Add(logicScene);
                    //加载信息内容 
                    logicScene.formInfoIni();

                    break;
                case "ConditionDeal":
                    logicType = LogicType.Condition;
                    //显示多条件处理
                    superTabControl1.SelectedTab.AttachedControl.Controls.Add(logicCondition);
                    //加载信息内容 
                    logicCondition.formInfoIni();
                    break;
                case "VoiceDeal":
                    logicType = LogicType.Voice;
                    //显示表达式处理
                    superTabControl1.SelectedTab.AttachedControl.Controls.Add(logicVoice);
                    //加载信息内容 
                    logicVoice.formInfoIni();
                    break;
                default:
                    break;
            }
        }
        #endregion


        #region 表格控件整体操作 MenuBox_TabMenuOpen

        #region 表格添加菜单栏
        /// <summary>
        /// 菜单栏样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superTabControl1_ControlBox_MenuBox_PopupOpen(object sender, DevComponents.DotNetBar.PopupOpenEventArgs e)
        {
            // If the user has said they want us to modify the TabMenu,
            // just add a couple of items to the list to let them
            // add or delete new user tabs

                PopupItem pi = sender as PopupItem;
                //菜单主题  
                pi.Style = eDotNetBarStyle.Metro;

                if (pi != null)
                {
                    ButtonItem bi = new ButtonItem();
                    bi.Text = "场景处理模式";
                    bi.Click += AddSceneTab_Click;
                    pi.SubItems.Insert(0, bi);

                    
                    bi = new ButtonItem();
                    bi.Text = "多条件处理模式";
                    bi.Click += AddConditionTab_Click;
                    pi.SubItems.Insert(1, bi);

                    bi = new ButtonItem();
                    bi.Text = "表达式处理模式";
                    bi.Click += AddVoiceTab_Click;
                    pi.SubItems.Insert(2, bi);
                    
                    bi = new ButtonItem();
                    bi.Text = "删除";
                    bi.Tag = superTabControl1.SelectedTab;
                    bi.Click += DelUserTab_Click;
                    pi.SubItems.Insert(3, bi);

                    // Enable the added Delete entry if the currently selected
                    // tab is one they have previously added

                    if (superTabControl1.SelectedTab == null ||
                        superTabControl1.Tabs.Count <2 )//.SelectedTab.Text.StartsWith("New Tab") == false)
                    {
                        bi.Enabled = false;
                    }

                    if (pi.SubItems.Count > 4)
                        pi.SubItems[4].BeginGroup = true;
                }
            
        }
        #endregion

        #region 删除框操作
        /// <summary>
        /// 框 删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void superTabControl1_TabItemClose(object sender, SuperTabStripTabItemCloseEventArgs e)
        {
            SuperTabItem tab = e.Tab as SuperTabItem;
            // In our sample app, only let the user close tabs they have added
            if (tab == null || superTabControl1.Tabs.Count < 2)
            {
                e.Cancel = true;
                return;
            }
            //删除该信息
            removeLogicInfo(tab.Text);

        }

        /// <summary>
        ///  菜单栏 删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DelUserTab_Click(object sender, EventArgs e)
        {
            ButtonItem bi = sender as ButtonItem;

            if (bi != null)
            {
                SuperTabItem tab = bi.Tag as SuperTabItem;

                if (tab != null && superTabControl1.Tabs.Count > 1)
                {
                    removeLogicInfo(tab.Text);
                    
                    tab.Close();
                }
                    
            }
        }

        /// <summary>
        /// 删除框的信息内容
        /// </summary>
        /// <param name="tabName"></param>
        private void removeLogicInfo(string tabName)
        {
            DataJson.logics lgs = DataListHelper.getLogicInfoListByNode();
            if (lgs == null)
            {
                return;
            }
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //删除逻辑框信息内容
            DataJson.logicsInfo logicInfo = DataListHelper.findLogicInfoByTabName(tabName);
            if (logicInfo == null)
            {
                return;
            }
            lgs.logicsInfo.Remove(logicInfo);
   
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
        }

      

        #endregion


        #region 添加场景处理框  AddSceneTab_Click

        void AddSceneTab_Click(object sender, EventArgs e)
        {
            DataJson.logics lgs = DataListHelper.getLogicInfoListByNode();
            if (lgs == null)
            {
                return;
            }
            if (lgs.logicsInfo[0].modelType != "SceneDeal")
            {
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //清除所有信息 界面框也清除
                lgs.logicsInfo.Clear();
                //superTabControl1.Tabs.Clear();
                SuperTabControllCreat();
                //添加新的场景处理
                DataJson.logicsInfo lgInfo = new DataJson.logicsInfo();
                lgs.logicsInfo.Add(lgInfo);
                AddNewSceneTab();
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            
           
        }

        /// <summary>
        /// 添加场景框
        /// </summary>
        private void AddNewSceneTab()
        {
            SuperTabItem tab = superTabControl1.CreateTab("场景处理 1", superTabControl1.Tabs.Count);
            superTabControl1.SelectedTab = tab;
            tab.AttachedControl.Controls.Add(logicScene);

        }

        #endregion

        #region  添加多条件处理框 AddConditionTab_Click

        void AddConditionTab_Click(object sender, EventArgs e)
        {
            DataJson.logics lgs = DataListHelper.getLogicInfoListByNode();
            if (lgs == null)
            {
                return;
            }
            if (lgs.logicsInfo[0].modelType == "SceneDeal")
            {
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //清除所有信息 界面框也清除
                lgs.logicsInfo.Clear();
                //superTabControl1.Tabs.Clear();
                SuperTabControllCreat();
                //添加新的场景处理
                DataJson.logicsInfo lgInfo = new DataJson.logicsInfo();
                lgInfo.id = DataListHelper.getLogicInfoID(lgs);
                lgInfo.modelType = "ConditionDeal";
                lgs.logicsInfo.Add(lgInfo);
                AddNewConditionTab(lgInfo.id);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            else
            {
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //添加新的场景处理
                DataJson.logicsInfo lgInfo = new DataJson.logicsInfo();
                lgInfo.id = DataListHelper.getLogicInfoID(lgs);
                lgInfo.modelType = "ConditionDeal";
                lgs.logicsInfo.Insert(lgInfo.id-1, lgInfo);
                AddNewConditionTab(lgInfo.id);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            
        }


     
        /// <summary>
        /// 添加多条件框
        /// </summary>
        private void AddNewConditionTab(int id)
        {
            SuperTabItem tab = superTabControl1.CreateTab("多条件处理 "+id, superTabControl1.Tabs.Count);
            tab.Tag = id;
        }

        #endregion

        #region  添加表达式处理框 AddVoiceTab_Click

        void AddVoiceTab_Click(object sender, EventArgs e)
        {
       
            DataJson.logics lgs = DataListHelper.getLogicInfoListByNode();
            if (lgs == null)
            {
                return;
            }
            if (lgs.logicsInfo[0].modelType == "SceneDeal")
            {
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //清除所有信息 界面框也清除
                lgs.logicsInfo.Clear();
                //superTabControl1.Tabs.Clear();
                SuperTabControllCreat();
                //添加新的场景处理
                DataJson.logicsInfo lgInfo = new DataJson.logicsInfo();
                lgInfo.id = DataListHelper.getLogicInfoID(lgs);
                lgInfo.modelType = "VoiceDeal";
                lgs.logicsInfo.Add(lgInfo);
                AddNewVoiceTab(lgInfo.id);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
            else
            {
                //撤销
                DataJson.totalList OldList = FileMesege.cmds.getListInfos();
                //添加新的场景处理
                DataJson.logicsInfo lgInfo = new DataJson.logicsInfo();
                lgInfo.id = DataListHelper.getLogicInfoID(lgs);
                lgInfo.modelType = "VoiceDeal";
                lgs.logicsInfo.Insert(lgInfo.id-1,lgInfo);
                AddNewVoiceTab(lgInfo.id);
                DataJson.totalList NewList = FileMesege.cmds.getListInfos();
                FileMesege.cmds.DoNewCommand(NewList, OldList);
            }
        }

  
        /// <summary>
        /// 添加表达式处理框
        /// </summary>
        private void AddNewVoiceTab(int id)
        {
            SuperTabItem tab = superTabControl1.CreateTab("表达式处理 " + id, superTabControl1.Tabs.Count);
            tab.Tag = id;

        }


 

        #endregion

  
        #endregion


        #region 添加 下载 加载 清空
        private void btnAdd_Click(object sender, EventArgs e)
        {
            SuperTabItem tab = superTabControl1.SelectedTab;
            DataJson.logicsInfo logicInfo = DataListHelper.findLogicInfoByTabName(tab.Text);
            if (logicInfo == null)
            {
                return;
            }
            if (logicInfo.modelType == "SceneDeal")
            { 
                //添加一行新内容
                logicScene.dgvAddRow();
            }
            
            
        }

        private void btnDown_Click(object sender, EventArgs e)
        {

        }

        private void btnRead_Click(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SuperTabItem tab = superTabControl1.SelectedTab;
            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            //删除逻辑框信息内容
            DataJson.logicsInfo logicInfo = DataListHelper.findLogicInfoByTabName(tab.Text);
            if (logicInfo == null)
            {
                return;
            }
            logicInfo.content = "";
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            tabSelectAddPanel(logicInfo);
        }

        #endregion


    }
}
