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
            AddNewUserTab();
        }

        public void dgvLogicAddItem()
        { 
        
        }

        private void btnAddDay_Click(object sender, EventArgs e)
        {
            FileMesege.PointList = new DataJson.Point();
            while(true)
            {
                int randomNum = DataChange.randomNum();
                if (FileMesege.PointList.equipment.Exists(x => x.pid == randomNum))
                {

                    MessageBox.Show(string.Format("第{0}个存在相同pid", FileMesege.PointList.equipment.Count));
                    break;
                }
                else
                {
                    DataJson.PointInfo point = new DataJson.PointInfo();
                    point.pid = randomNum;
                    FileMesege.PointList.equipment.Add(point);

                }
            }
            
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
                    bi.Text = "Add User Tab";
                    bi.BeginGroup = true;
                    bi.Click += AddUserTab_Click;

                    pi.SubItems.Insert(0, bi);

                    bi = new ButtonItem();
                    bi.Text = "Delete User Tab";
                    bi.Tag = superTabControl1.SelectedTab;
                    bi.Click += DelUserTab_Click;

                    pi.SubItems.Insert(1, bi);

                    // Enable the added Delete entry if the currently selected
                    // tab is one they have previously added

                    if (superTabControl1.SelectedTab == null ||
                        superTabControl1.SelectedTab.Text.StartsWith("New Tab") == false)
                    {
                        bi.Enabled = false;
                    }

                    if (pi.SubItems.Count > 2)
                        pi.SubItems[2].BeginGroup = true;
                }
            
        }

        #region DelUserTab_Click

        void DelUserTab_Click(object sender, EventArgs e)
        {
            ButtonItem bi = sender as ButtonItem;

            if (bi != null)
            {
                SuperTabItem tab = bi.Tag as SuperTabItem;

                if (tab != null && tab.Text.StartsWith("New Tab") == true)
                    tab.Close();
            }
        }

        #endregion

        #region AddUserTab_Click

        void AddUserTab_Click(object sender, EventArgs e)
        {
            AddNewUserTab();
        }

        #region AddNewUserTab

        private void AddNewUserTab()
        {
            SuperTabItem tab = superTabControl1.CreateTab("New Tab " + _UserTabCount, 0);
            tab.Tag = _UserTabCount++;

   

            LabelX lbx = new LabelX();
            lbx.Text = "This space intentionally left blank.";
            lbx.TextAlignment = StringAlignment.Center;
            lbx.TextLineAlignment = StringAlignment.Center;
            lbx.BackColor = Color.Transparent;

            tab.AttachedControl.Controls.Add(lbx);
            lbx.Dock = DockStyle.Fill;

            //UpdateDelUserButton();
        }

        #endregion

        #endregion

        #endregion



    }
}
