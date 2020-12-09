namespace eNet编辑器.DgvView
{
    partial class LogicVoice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogicVoice));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAssignment = new System.Windows.Forms.TextBox();
            this.txtVoice = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAttr = new DevComponents.DotNetBar.ButtonX();
            this.cbAttr = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.num1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.section1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.del1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnElseIfAdd = new DevComponents.DotNetBar.ButtonX();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.num2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.address2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.section2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operation2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.delay2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.del2 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnElseAdd = new DevComponents.DotNetBar.ButtonX();
            this.label5 = new System.Windows.Forms.Label();
            this.doubleClickTimer1 = new System.Windows.Forms.Timer(this.components);
            this.doubleClickTimer2 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.相同ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.升序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.降序ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.panel4.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtAssignment);
            this.panel1.Controls.Add(this.txtVoice);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnAttr);
            this.panel1.Controls.Add(this.cbAttr);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 87);
            this.panel1.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(414, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 44;
            this.label4.Text = "赋值-->";
            // 
            // txtAssignment
            // 
            this.txtAssignment.Location = new System.Drawing.Point(482, 49);
            this.txtAssignment.Name = "txtAssignment";
            this.txtAssignment.Size = new System.Drawing.Size(253, 23);
            this.txtAssignment.TabIndex = 43;
            this.txtAssignment.DoubleClick += new System.EventHandler(this.txtAssignment_DoubleClick);
            this.txtAssignment.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAssignment_KeyPress);
            // 
            // txtVoice
            // 
            this.txtVoice.Location = new System.Drawing.Point(105, 49);
            this.txtVoice.Name = "txtVoice";
            this.txtVoice.Size = new System.Drawing.Size(289, 23);
            this.txtVoice.TabIndex = 42;
            this.txtVoice.DoubleClick += new System.EventHandler(this.txtVoice_DoubleClick);
            this.txtVoice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVoice_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 41;
            this.label1.Text = "表达式：";
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
            this.btnAttr.Location = new System.Drawing.Point(207, 11);
            this.btnAttr.Margin = new System.Windows.Forms.Padding(6);
            this.btnAttr.Name = "btnAttr";
            this.btnAttr.Size = new System.Drawing.Size(24, 24);
            this.btnAttr.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAttr.TabIndex = 38;
            this.btnAttr.Tooltip = "设置";
            this.btnAttr.Click += new System.EventHandler(this.btnAttr_Click);
            // 
            // cbAttr
            // 
            this.cbAttr.FormattingEnabled = true;
            this.cbAttr.Items.AddRange(new object[] {
            "被动触发",
            "主动触发"});
            this.cbAttr.Location = new System.Drawing.Point(106, 10);
            this.cbAttr.Name = "cbAttr";
            this.cbAttr.Size = new System.Drawing.Size(83, 25);
            this.cbAttr.TabIndex = 34;
            this.cbAttr.Text = "主动触发";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 33;
            this.label2.Text = "执行模式：";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 87);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel1.Controls.Add(this.panel3);
            this.splitContainer1.Panel1MinSize = 22;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView2);
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Panel2MinSize = 22;
            this.splitContainer1.Size = new System.Drawing.Size(811, 417);
            this.splitContainer1.SplitterDistance = 189;
            this.splitContainer1.TabIndex = 8;
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
            this.num1,
            this.address1,
            this.section1,
            this.name1,
            this.del1});
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
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(811, 167);
            this.dataGridView1.TabIndex = 42;
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseMove);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // num1
            // 
            this.num1.HeaderText = "序号";
            this.num1.Name = "num1";
            this.num1.ReadOnly = true;
            this.num1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.num1.Width = 60;
            // 
            // address1
            // 
            this.address1.HeaderText = "地址";
            this.address1.Name = "address1";
            this.address1.ReadOnly = true;
            // 
            // section1
            // 
            this.section1.HeaderText = "区域";
            this.section1.Name = "section1";
            this.section1.ReadOnly = true;
            this.section1.Width = 160;
            // 
            // name1
            // 
            this.name1.HeaderText = "名称";
            this.name1.Name = "name1";
            this.name1.ReadOnly = true;
            this.name1.Width = 80;
            // 
            // del1
            // 
            this.del1.HeaderText = "删除";
            this.del1.Name = "del1";
            this.del1.ReadOnly = true;
            this.del1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.del1.Width = 55;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnElseIfAdd);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(811, 22);
            this.panel3.TabIndex = 41;
            // 
            // btnElseIfAdd
            // 
            this.btnElseIfAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnElseIfAdd.BackColor = System.Drawing.Color.White;
            this.btnElseIfAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnElseIfAdd.FocusCuesEnabled = false;
            this.btnElseIfAdd.Font = new System.Drawing.Font("黑体", 9F);
            this.btnElseIfAdd.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnElseIfAdd.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnElseIfAdd.HoverImage")));
            this.btnElseIfAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnElseIfAdd.Image")));
            this.btnElseIfAdd.Location = new System.Drawing.Point(76, 3);
            this.btnElseIfAdd.Margin = new System.Windows.Forms.Padding(10, 6, 6, 6);
            this.btnElseIfAdd.Name = "btnElseIfAdd";
            this.btnElseIfAdd.Size = new System.Drawing.Size(15, 15);
            this.btnElseIfAdd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnElseIfAdd.TabIndex = 40;
            this.btnElseIfAdd.Tooltip = "增加";
            this.btnElseIfAdd.Click += new System.EventHandler(this.btnElseIfAdd_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 40;
            this.label3.Text = "表达项：";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView2.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.num2,
            this.result,
            this.address2,
            this.section2,
            this.name2,
            this.operation2,
            this.delay2,
            this.del2});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(235)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView2.Location = new System.Drawing.Point(0, 22);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.RowHeadersWidth = 30;
            this.dataGridView2.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(811, 202);
            this.dataGridView2.TabIndex = 43;
            this.dataGridView2.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView2_CellBeginEdit);
            this.dataGridView2.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellEndEdit);
            this.dataGridView2.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView2_CellMouseDown);
            this.dataGridView2.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView2_CellMouseMove);
            this.dataGridView2.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView2_DataError);
            this.dataGridView2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView2_Scroll);
            this.dataGridView2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView2_KeyUp);
            this.dataGridView2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView2_MouseDown);
            // 
            // num2
            // 
            this.num2.HeaderText = "序号";
            this.num2.Name = "num2";
            this.num2.ReadOnly = true;
            this.num2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.num2.Width = 60;
            // 
            // result
            // 
            this.result.HeaderText = "结果";
            this.result.Name = "result";
            this.result.ReadOnly = true;
            // 
            // address2
            // 
            this.address2.HeaderText = "地址";
            this.address2.Name = "address2";
            this.address2.ReadOnly = true;
            // 
            // section2
            // 
            this.section2.HeaderText = "区域";
            this.section2.Name = "section2";
            this.section2.ReadOnly = true;
            this.section2.Width = 160;
            // 
            // name2
            // 
            this.name2.HeaderText = "名称";
            this.name2.Name = "name2";
            this.name2.ReadOnly = true;
            this.name2.Width = 80;
            // 
            // operation2
            // 
            this.operation2.HeaderText = "操作";
            this.operation2.Name = "operation2";
            this.operation2.ReadOnly = true;
            this.operation2.Width = 140;
            // 
            // delay2
            // 
            this.delay2.HeaderText = "延时（秒）";
            this.delay2.Name = "delay2";
            this.delay2.ReadOnly = true;
            this.delay2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // del2
            // 
            this.del2.HeaderText = "删除";
            this.del2.Name = "del2";
            this.del2.ReadOnly = true;
            this.del2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.del2.Width = 55;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnElseAdd);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(811, 22);
            this.panel4.TabIndex = 42;
            // 
            // btnElseAdd
            // 
            this.btnElseAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnElseAdd.BackColor = System.Drawing.Color.White;
            this.btnElseAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnElseAdd.FocusCuesEnabled = false;
            this.btnElseAdd.Font = new System.Drawing.Font("黑体", 9F);
            this.btnElseAdd.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnElseAdd.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnElseAdd.HoverImage")));
            this.btnElseAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnElseAdd.Image")));
            this.btnElseAdd.Location = new System.Drawing.Point(115, 3);
            this.btnElseAdd.Margin = new System.Windows.Forms.Padding(10, 6, 6, 6);
            this.btnElseAdd.Name = "btnElseAdd";
            this.btnElseAdd.Size = new System.Drawing.Size(15, 15);
            this.btnElseAdd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnElseAdd.TabIndex = 40;
            this.btnElseAdd.Tooltip = "增加";
            this.btnElseAdd.Click += new System.EventHandler(this.btnElseAdd_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 17);
            this.label5.TabIndex = 40;
            this.label5.Text = "表达式判断项：";
            // 
            // doubleClickTimer1
            // 
            this.doubleClickTimer1.Interval = 40;
            this.doubleClickTimer1.Tick += new System.EventHandler(this.doubleClickTimer1_Tick);
            // 
            // doubleClickTimer2
            // 
            this.doubleClickTimer2.Interval = 40;
            this.doubleClickTimer2.Tick += new System.EventHandler(this.doubleClickTimer2_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.相同ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.升序ToolStripMenuItem,
            this.降序ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip2";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 98);
            // 
            // 相同ToolStripMenuItem
            // 
            this.相同ToolStripMenuItem.Name = "相同ToolStripMenuItem";
            this.相同ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.相同ToolStripMenuItem.Text = "相同(Ctrl+Q)";
            this.相同ToolStripMenuItem.Click += new System.EventHandler(this.相同ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // 升序ToolStripMenuItem
            // 
            this.升序ToolStripMenuItem.Name = "升序ToolStripMenuItem";
            this.升序ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.升序ToolStripMenuItem.Text = "升序(Ctrl+W)";
            this.升序ToolStripMenuItem.Click += new System.EventHandler(this.升序ToolStripMenuItem_Click);
            // 
            // 降序ToolStripMenuItem
            // 
            this.降序ToolStripMenuItem.Name = "降序ToolStripMenuItem";
            this.降序ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.降序ToolStripMenuItem.Text = "降序(Ctrl+E)";
            this.降序ToolStripMenuItem.Click += new System.EventHandler(this.降序ToolStripMenuItem_Click);
            // 
            // LogicVoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LogicVoice";
            this.Size = new System.Drawing.Size(811, 504);
            this.Load += new System.EventHandler(this.LogicVoice_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DevComponents.DotNetBar.ButtonX btnAttr;
        private System.Windows.Forms.ComboBox cbAttr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel3;
        private DevComponents.DotNetBar.ButtonX btnElseIfAdd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Panel panel4;
        private DevComponents.DotNetBar.ButtonX btnElseAdd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAssignment;
        private System.Windows.Forms.TextBox txtVoice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer doubleClickTimer1;
        private System.Windows.Forms.Timer doubleClickTimer2;
        private System.Windows.Forms.DataGridViewTextBoxColumn num1;
        private System.Windows.Forms.DataGridViewTextBoxColumn address1;
        private System.Windows.Forms.DataGridViewTextBoxColumn section1;
        private System.Windows.Forms.DataGridViewTextBoxColumn name1;
        private System.Windows.Forms.DataGridViewButtonColumn del1;
        private System.Windows.Forms.DataGridViewTextBoxColumn num2;
        private System.Windows.Forms.DataGridViewTextBoxColumn result;
        private System.Windows.Forms.DataGridViewTextBoxColumn address2;
        private System.Windows.Forms.DataGridViewTextBoxColumn section2;
        private System.Windows.Forms.DataGridViewTextBoxColumn name2;
        private System.Windows.Forms.DataGridViewTextBoxColumn operation2;
        private System.Windows.Forms.DataGridViewTextBoxColumn delay2;
        private System.Windows.Forms.DataGridViewButtonColumn del2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 相同ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 升序ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 降序ToolStripMenuItem;
    }
}
