namespace eNet编辑器.DgvView
{
    partial class LogicScene
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogicScene));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.num = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.section = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.state = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.del = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAttr = new DevComponents.DotNetBar.ButtonX();
            this.btnDecid = new DevComponents.DotNetBar.ButtonX();
            this.cbScene = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAttr = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.doubleClickTimer = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnIfAdd = new DevComponents.DotNetBar.ButtonX();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.num,
            this.address,
            this.section,
            this.name,
            this.operation,
            this.state,
            this.del});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(235)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(0, 22);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(765, 351);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseMove);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // num
            // 
            this.num.HeaderText = "序号";
            this.num.Name = "num";
            this.num.ReadOnly = true;
            this.num.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.num.Width = 60;
            // 
            // address
            // 
            this.address.HeaderText = "地址";
            this.address.Name = "address";
            this.address.ReadOnly = true;
            // 
            // section
            // 
            this.section.HeaderText = "区域";
            this.section.Name = "section";
            this.section.ReadOnly = true;
            this.section.Width = 160;
            // 
            // name
            // 
            this.name.HeaderText = "名称";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Width = 80;
            // 
            // operation
            // 
            this.operation.HeaderText = "操作";
            this.operation.Name = "operation";
            this.operation.ReadOnly = true;
            this.operation.Width = 140;
            // 
            // state
            // 
            this.state.HeaderText = "状态";
            this.state.Name = "state";
            this.state.ReadOnly = true;
            this.state.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // del
            // 
            this.del.HeaderText = "删除";
            this.del.Name = "del";
            this.del.ReadOnly = true;
            this.del.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.del.Width = 55;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.btnAttr);
            this.panel1.Controls.Add(this.btnDecid);
            this.panel1.Controls.Add(this.cbScene);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbAttr);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 90);
            this.panel1.TabIndex = 5;
            // 
            // btnAttr
            // 
            this.btnAttr.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAttr.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnAttr.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnAttr.FocusCuesEnabled = false;
            this.btnAttr.Font = new System.Drawing.Font("黑体", 9F);
            this.btnAttr.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnAttr.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnAttr.HoverImage")));
            this.btnAttr.Image = ((System.Drawing.Image)(resources.GetObject("btnAttr.Image")));
            this.btnAttr.Location = new System.Drawing.Point(207, 12);
            this.btnAttr.Margin = new System.Windows.Forms.Padding(6);
            this.btnAttr.Name = "btnAttr";
            this.btnAttr.Size = new System.Drawing.Size(24, 24);
            this.btnAttr.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAttr.TabIndex = 38;
            this.btnAttr.Tooltip = "设置";
            this.btnAttr.Click += new System.EventHandler(this.btnAttr_Click);
            // 
            // btnDecid
            // 
            this.btnDecid.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDecid.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnDecid.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDecid.FocusCuesEnabled = false;
            this.btnDecid.Font = new System.Drawing.Font("黑体", 9F);
            this.btnDecid.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnDecid.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDecid.HoverImage")));
            this.btnDecid.Image = ((System.Drawing.Image)(resources.GetObject("btnDecid.Image")));
            this.btnDecid.Location = new System.Drawing.Point(423, 51);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(6);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(24, 24);
            this.btnDecid.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDecid.TabIndex = 37;
            this.btnDecid.Tooltip = "设置";
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // cbScene
            // 
            this.cbScene.FormattingEnabled = true;
            this.cbScene.Location = new System.Drawing.Point(106, 51);
            this.cbScene.Name = "cbScene";
            this.cbScene.Size = new System.Drawing.Size(299, 25);
            this.cbScene.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 35;
            this.label1.Text = "场景：";
            // 
            // cbAttr
            // 
            this.cbAttr.FormattingEnabled = true;
            this.cbAttr.Items.AddRange(new object[] {
            "被动触发",
            "主动触发"});
            this.cbAttr.Location = new System.Drawing.Point(106, 11);
            this.cbAttr.Name = "cbAttr";
            this.cbAttr.Size = new System.Drawing.Size(83, 25);
            this.cbAttr.TabIndex = 34;
            this.cbAttr.Text = "主动触发";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 33;
            this.label2.Text = "执行模式：";
            // 
            // doubleClickTimer
            // 
            this.doubleClickTimer.Interval = 40;
            this.doubleClickTimer.Tick += new System.EventHandler(this.doubleClickTimer_Tick);
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.dataGridView1);
            this.panel5.Controls.Add(this.panel2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 90);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(765, 373);
            this.panel5.TabIndex = 45;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnIfAdd);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(765, 22);
            this.panel2.TabIndex = 40;
            // 
            // btnIfAdd
            // 
            this.btnIfAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnIfAdd.BackColor = System.Drawing.Color.White;
            this.btnIfAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnIfAdd.FocusCuesEnabled = false;
            this.btnIfAdd.Font = new System.Drawing.Font("黑体", 9F);
            this.btnIfAdd.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnIfAdd.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnIfAdd.HoverImage")));
            this.btnIfAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnIfAdd.Image")));
            this.btnIfAdd.Location = new System.Drawing.Point(91, 3);
            this.btnIfAdd.Margin = new System.Windows.Forms.Padding(6);
            this.btnIfAdd.Name = "btnIfAdd";
            this.btnIfAdd.Size = new System.Drawing.Size(15, 15);
            this.btnIfAdd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnIfAdd.TabIndex = 40;
            this.btnIfAdd.Tooltip = "增加";
            this.btnIfAdd.Visible = false;
            this.btnIfAdd.Click += new System.EventHandler(this.btnIfAdd_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 39;
            this.label3.Text = "场景判断：";
            // 
            // LogicScene
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LogicScene";
            this.Size = new System.Drawing.Size(765, 463);
            this.Load += new System.EventHandler(this.LogicScene_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAttr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbScene;
        private DevComponents.DotNetBar.ButtonX btnDecid;
        private System.Windows.Forms.Timer doubleClickTimer;
        private System.Windows.Forms.Timer timer1;
        private DevComponents.DotNetBar.ButtonX btnAttr;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel2;
        private DevComponents.DotNetBar.ButtonX btnIfAdd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewTextBoxColumn num;
        private System.Windows.Forms.DataGridViewTextBoxColumn address;
        private System.Windows.Forms.DataGridViewTextBoxColumn section;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn operation;
        private System.Windows.Forms.DataGridViewTextBoxColumn state;
        private System.Windows.Forms.DataGridViewButtonColumn del;
    }
}
