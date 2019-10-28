namespace eNet编辑器.DgvView
{
    partial class DgvSensor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DgvSensor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.cbDevNum = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbIONum = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.symbolBox2 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.label3 = new System.Windows.Forms.Label();
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDown = new DevComponents.DotNetBar.ButtonX();
            this.btnImport = new DevComponents.DotNetBar.ButtonX();
            this.btnOn = new DevComponents.DotNetBar.ButtonX();
            this.btnOff = new DevComponents.DotNetBar.ButtonX();
            this.btnClear = new DevComponents.DotNetBar.ButtonX();
            this.btnDel = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.label1 = new System.Windows.Forms.Label();
            this.doubleClickTimer = new System.Windows.Forms.Timer(this.components);
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.keyAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.objAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.section = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.del = new System.Windows.Forms.DataGridViewButtonColumn();
            this.add = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel10.SuspendLayout();
            this.plInfoTitle.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.plInfoTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(994, 564);
            this.panel1.TabIndex = 19;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.ultraSplitter1);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(994, 539);
            this.panel2.TabIndex = 18;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(214, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(780, 539);
            this.panel4.TabIndex = 7;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
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
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.keyAddress,
            this.objAddress,
            this.section,
            this.name,
            this.operation,
            this.del,
            this.add});
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
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(780, 539);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseMove);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.Location = new System.Drawing.Point(204, 0);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 207;
            this.ultraSplitter1.Size = new System.Drawing.Size(10, 539);
            this.ultraSplitter1.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(204, 539);
            this.panel3.TabIndex = 4;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(204, 539);
            this.panel5.TabIndex = 3;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel8);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(204, 539);
            this.panel6.TabIndex = 5;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.White;
            this.panel8.Controls.Add(this.panel7);
            this.panel8.Controls.Add(this.panel10);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(204, 539);
            this.panel8.TabIndex = 7;
            // 
            // panel7
            // 
            this.panel7.AutoScroll = true;
            this.panel7.Controls.Add(this.cbDevNum);
            this.panel7.Controls.Add(this.label4);
            this.panel7.Controls.Add(this.cbIONum);
            this.panel7.Controls.Add(this.label2);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(0, 25);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(204, 514);
            this.panel7.TabIndex = 31;
            // 
            // cbDevNum
            // 
            this.cbDevNum.FormattingEnabled = true;
            this.cbDevNum.Location = new System.Drawing.Point(108, 64);
            this.cbDevNum.Name = "cbDevNum";
            this.cbDevNum.Size = new System.Drawing.Size(46, 25);
            this.cbDevNum.TabIndex = 34;
            this.cbDevNum.SelectedIndexChanged += new System.EventHandler(this.cbDevNum_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 33;
            this.label4.Text = "感应设备号：";
            // 
            // cbIONum
            // 
            this.cbIONum.FormattingEnabled = true;
            this.cbIONum.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.cbIONum.Location = new System.Drawing.Point(108, 19);
            this.cbIONum.Name = "cbIONum";
            this.cbIONum.Size = new System.Drawing.Size(46, 25);
            this.cbIONum.TabIndex = 32;
            this.cbIONum.TextChanged += new System.EventHandler(this.cbIoNum_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 31;
            this.label2.Text = "感应数量：";
            // 
            // panel10
            // 
            this.panel10.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel10.Controls.Add(this.symbolBox2);
            this.panel10.Controls.Add(this.label3);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(204, 25);
            this.panel10.TabIndex = 5;
            // 
            // symbolBox2
            // 
            // 
            // 
            // 
            this.symbolBox2.BackgroundStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("symbolBox2.BackgroundStyle.BackgroundImage")));
            this.symbolBox2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.symbolBox2.Location = new System.Drawing.Point(5, 6);
            this.symbolBox2.Margin = new System.Windows.Forms.Padding(2, 4, 3, 4);
            this.symbolBox2.Name = "symbolBox2";
            this.symbolBox2.Size = new System.Drawing.Size(13, 13);
            this.symbolBox2.Symbol = "";
            this.symbolBox2.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(119)))));
            this.symbolBox2.TabIndex = 13;
            this.symbolBox2.Text = "symbolBox2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(24, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "感应编组属性";
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.plInfoTitle.Controls.Add(this.flowLayoutPanel1);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.label1);
            this.plInfoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plInfoTitle.Location = new System.Drawing.Point(0, 0);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(994, 25);
            this.plInfoTitle.TabIndex = 17;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.flowLayoutPanel1.Controls.Add(this.btnDown);
            this.flowLayoutPanel1.Controls.Add(this.btnImport);
            this.flowLayoutPanel1.Controls.Add(this.btnOn);
            this.flowLayoutPanel1.Controls.Add(this.btnOff);
            this.flowLayoutPanel1.Controls.Add(this.btnClear);
            this.flowLayoutPanel1.Controls.Add(this.btnDel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(831, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(163, 25);
            this.flowLayoutPanel1.TabIndex = 20;
            // 
            // btnDown
            // 
            this.btnDown.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDown.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDown.FocusCuesEnabled = false;
            this.btnDown.Font = new System.Drawing.Font("黑体", 9F);
            this.btnDown.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnDown.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDown.HoverImage")));
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.Location = new System.Drawing.Point(6, 6);
            this.btnDown.Margin = new System.Windows.Forms.Padding(6);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(15, 15);
            this.btnDown.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDown.TabIndex = 16;
            this.btnDown.Tooltip = "加载";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnImport
            // 
            this.btnImport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnImport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnImport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnImport.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnImport.Enabled = false;
            this.btnImport.FocusCuesEnabled = false;
            this.btnImport.Font = new System.Drawing.Font("黑体", 9F);
            this.btnImport.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnImport.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnImport.HoverImage")));
            this.btnImport.Image = ((System.Drawing.Image)(resources.GetObject("btnImport.Image")));
            this.btnImport.Location = new System.Drawing.Point(33, 6);
            this.btnImport.Margin = new System.Windows.Forms.Padding(6);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(15, 15);
            this.btnImport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnImport.TabIndex = 18;
            this.btnImport.Tooltip = "载入";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnOn
            // 
            this.btnOn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOn.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnOn.FocusCuesEnabled = false;
            this.btnOn.Font = new System.Drawing.Font("黑体", 9F);
            this.btnOn.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnOn.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnOn.HoverImage")));
            this.btnOn.Image = ((System.Drawing.Image)(resources.GetObject("btnOn.Image")));
            this.btnOn.Location = new System.Drawing.Point(60, 6);
            this.btnOn.Margin = new System.Windows.Forms.Padding(6);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(15, 15);
            this.btnOn.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnOn.TabIndex = 15;
            this.btnOn.Tooltip = "开启";
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // btnOff
            // 
            this.btnOff.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOff.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOff.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnOff.FocusCuesEnabled = false;
            this.btnOff.Font = new System.Drawing.Font("黑体", 9F);
            this.btnOff.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnOff.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnOff.HoverImage")));
            this.btnOff.Image = ((System.Drawing.Image)(resources.GetObject("btnOff.Image")));
            this.btnOff.Location = new System.Drawing.Point(87, 6);
            this.btnOff.Margin = new System.Windows.Forms.Padding(6);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(15, 15);
            this.btnOff.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnOff.TabIndex = 17;
            this.btnOff.Tooltip = "关闭";
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // btnClear
            // 
            this.btnClear.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnClear.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnClear.FocusCuesEnabled = false;
            this.btnClear.Font = new System.Drawing.Font("黑体", 9F);
            this.btnClear.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnClear.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnClear.HoverImage")));
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.Location = new System.Drawing.Point(114, 6);
            this.btnClear.Margin = new System.Windows.Forms.Padding(6);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(15, 15);
            this.btnClear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClear.TabIndex = 5;
            this.btnClear.Tooltip = "清空";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDel
            // 
            this.btnDel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDel.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDel.FocusCuesEnabled = false;
            this.btnDel.Font = new System.Drawing.Font("黑体", 9F);
            this.btnDel.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnDel.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDel.HoverImage")));
            this.btnDel.Image = ((System.Drawing.Image)(resources.GetObject("btnDel.Image")));
            this.btnDel.Location = new System.Drawing.Point(141, 6);
            this.btnDel.Margin = new System.Windows.Forms.Padding(6);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(15, 15);
            this.btnDel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDel.TabIndex = 19;
            this.btnDel.Tooltip = "清空加载";
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // symbolBox1
            // 
            // 
            // 
            // 
            this.symbolBox1.BackgroundStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("symbolBox1.BackgroundStyle.BackgroundImage")));
            this.symbolBox1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.symbolBox1.Location = new System.Drawing.Point(5, 6);
            this.symbolBox1.Margin = new System.Windows.Forms.Padding(2, 4, 3, 4);
            this.symbolBox1.Name = "symbolBox1";
            this.symbolBox1.Size = new System.Drawing.Size(13, 13);
            this.symbolBox1.Symbol = "";
            this.symbolBox1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(119)))));
            this.symbolBox1.TabIndex = 12;
            this.symbolBox1.Text = "symbolBox1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(24, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "内容";
            // 
            // doubleClickTimer
            // 
            this.doubleClickTimer.Interval = 40;
            this.doubleClickTimer.Tick += new System.EventHandler(this.doubleClickTimer_Tick);
            // 
            // id
            // 
            this.id.HeaderText = "感应";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.id.Width = 60;
            // 
            // keyAddress
            // 
            this.keyAddress.HeaderText = "输入地址";
            this.keyAddress.Name = "keyAddress";
            this.keyAddress.ReadOnly = true;
            this.keyAddress.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.keyAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.keyAddress.Width = 80;
            // 
            // objAddress
            // 
            this.objAddress.HeaderText = "执行地址";
            this.objAddress.Name = "objAddress";
            this.objAddress.ReadOnly = true;
            this.objAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // section
            // 
            this.section.HeaderText = "区域";
            this.section.Name = "section";
            this.section.ReadOnly = true;
            this.section.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.section.Width = 160;
            // 
            // name
            // 
            this.name.HeaderText = "名称";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.name.Width = 80;
            // 
            // operation
            // 
            this.operation.HeaderText = "操作";
            this.operation.Name = "operation";
            this.operation.ReadOnly = true;
            this.operation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.operation.Width = 140;
            // 
            // del
            // 
            this.del.HeaderText = "删除";
            this.del.Name = "del";
            this.del.ReadOnly = true;
            this.del.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.del.Width = 55;
            // 
            // add
            // 
            this.add.HeaderText = "添加";
            this.add.Name = "add";
            this.add.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.add.Width = 60;
            // 
            // DgvSensor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 564);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DgvSensor";
            this.Text = "DgvSensor";
            this.Load += new System.EventHandler(this.DgvSensor_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.ComboBox cbDevNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbIONum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel10;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnDel;
        private DevComponents.DotNetBar.ButtonX btnImport;
        private DevComponents.DotNetBar.ButtonX btnClear;
        private DevComponents.DotNetBar.ButtonX btnOff;
        private DevComponents.DotNetBar.ButtonX btnDown;
        private DevComponents.DotNetBar.ButtonX btnOn;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer doubleClickTimer;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn objAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn section;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn operation;
        private System.Windows.Forms.DataGridViewButtonColumn del;
        private System.Windows.Forms.DataGridViewButtonColumn add;
    }
}