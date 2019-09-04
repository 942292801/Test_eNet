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
        public DgvLogic()
        {
            
            //设置窗体双缓存
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            InitializeComponent();
           
            /*
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);*/
        }

        private void DgvLogic_Load(object sender, EventArgs e)
        {

            IniControl();
            
        }

        public void dgvLogicAddItem()
        {
        }

        /// <summary>
        /// 控件初始化
        /// </summary>
        private void IniControl()
        {
            //Tab表格关闭按钮
            superTabControl1.CloseButtonOnTabsVisible = true;
            superTabControl1.Tabs.Clear();
            AddNewSceneTab();
        }

        #region MenuBox_TabMenuOpen
        private int _UserTabCount = 1;
        private void superTabControl1_ControlBox_MenuBox_PopupOpen(object sender, DevComponents.DotNetBar.PopupOpenEventArgs e)
        {
            // If the user has said they want us to modify the TabMenu,
            // just add a couple of items to the list to let them
            // add or delete new user tabs

                PopupItem pi = sender as PopupItem;

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

        #region DelUserTab
        private void superTabControl1_TabItemClose(object sender, SuperTabStripTabItemCloseEventArgs e)
        {
            SuperTabItem tab = e.Tab as SuperTabItem;

            // In our sample app, only let the user close tabs they have added

            if (tab == null || superTabControl1.Tabs.Count < 2)
            {
                MessageBox.Show("模式不能为空，至少保留一项");

                e.Cancel = true;
            }
        }

        #endregion

        #region DelUserTab_Click

        void DelUserTab_Click(object sender, EventArgs e)
        {
            ButtonItem bi = sender as ButtonItem;

            if (bi != null)
            {
                SuperTabItem tab = bi.Tag as SuperTabItem;

                if (tab != null && superTabControl1.Tabs.Count > 1)
                    tab.Close();
            }
        }

        #endregion

        #region AddSceneTab_Click

        void AddSceneTab_Click(object sender, EventArgs e)
        {
            AddNewSceneTab();
        }

        #region AddNewSceneTab

        /// <summary>
        /// 
        /// </summary>
        private void AddNewSceneTab()
        {
            SuperTabItem tab = superTabControl1.CreateTab("场景处理" + _UserTabCount, superTabControl1.Tabs.Count);
            tab.Tag = _UserTabCount++;

   
            /*
            LabelX lbx = new LabelX();
            lbx.Text = "This space intentionally left blank. Scene" + _UserTabCount;
            lbx.TextAlignment = StringAlignment.Center;
            lbx.TextLineAlignment = StringAlignment.Center;
            lbx.BackColor = Color.Transparent;

            tab.AttachedControl.Controls.Add(lbx);
            lbx.Dock = DockStyle.Fill;
            superTabControl1.SelectedTab = tab;*/
            //UpdateDelUserButton();
        }

        #endregion
        #endregion

        #region AddConditionTab_Click

        void AddConditionTab_Click(object sender, EventArgs e)
        {
            AddNewConditionTab();
        }

        #region AddNewConditionTab

        private void AddNewConditionTab()
        {
            SuperTabItem tab = superTabControl1.CreateTab("多条件处理 " + _UserTabCount, superTabControl1.Tabs.Count);
            tab.Tag = _UserTabCount++;


            /*
            LabelX lbx = new LabelX();
            lbx.Text = "This space intentionally left blank. Condition " + _UserTabCount;
            lbx.TextAlignment = StringAlignment.Center;
            lbx.TextLineAlignment = StringAlignment.Center;
            lbx.BackColor = Color.Transparent;

            tab.AttachedControl.Controls.Add(lbx);
            lbx.Dock = DockStyle.Fill;
            */
            //UpdateDelUserButton();
        }

        #endregion
        #endregion

        #region AddVoiceTab_Click

        void AddVoiceTab_Click(object sender, EventArgs e)
        {
            AddNewVoiceTab();
        }

        #region AddNewVoiceTab

        private void AddNewVoiceTab()
        {
            SuperTabItem tab = superTabControl1.CreateTab("表达式处理 " + _UserTabCount, superTabControl1.Tabs.Count);
            tab.Tag = _UserTabCount++;


            /*
            LabelX lbx = new LabelX();
            lbx.Text = "This space intentionally left blank. Voice" + _UserTabCount;
            lbx.TextAlignment = StringAlignment.Center;
            lbx.TextLineAlignment = StringAlignment.Center;
            lbx.BackColor = Color.Transparent;

            tab.AttachedControl.Controls.Add(lbx);
            lbx.Dock = DockStyle.Fill;
            */
            //UpdateDelUserButton();
        }


        #endregion

        private void superTabControl1_SelectedTabChanged(object sender, SuperTabStripSelectedTabChangedEventArgs e)
        {
            SuperTabItem tab = superTabControl1.SelectedTab;
            LogicScene logicScene = new LogicScene(superTabControl1.SelectedTabIndex);
            logicScene.Dock = DockStyle.Fill;
            
            tab.AttachedControl.Controls.Add(logicScene);
        }



        #endregion

        #endregion



    }
}
