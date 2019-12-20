namespace eNet编辑器.Controller
{
    partial class setSwitch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(setSwitch));
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnIni = new DevComponents.DotNetBar.ButtonX();
            this.btnAllWrite = new DevComponents.DotNetBar.ButtonX();
            this.btnWrite = new DevComponents.DotNetBar.ButtonX();
            this.btnRead = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbPower = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbLock = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnOn = new System.Windows.Forms.Button();
            this.btnOff = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lbTip = new System.Windows.Forms.Label();
            this.pgBar = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.plInfoTitle.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.flowLayoutPanel1);
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.label6);
            this.plInfoTitle.Location = new System.Drawing.Point(1, 1);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(527, 25);
            this.plInfoTitle.TabIndex = 79;
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.flowLayoutPanel1.Controls.Add(this.btnIni);
            this.flowLayoutPanel1.Controls.Add(this.btnAllWrite);
            this.flowLayoutPanel1.Controls.Add(this.btnWrite);
            this.flowLayoutPanel1.Controls.Add(this.btnRead);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(394, 1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(108, 25);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // btnIni
            // 
            this.btnIni.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnIni.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIni.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnIni.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnIni.FocusCuesEnabled = false;
            this.btnIni.Font = new System.Drawing.Font("黑体", 9F);
            this.btnIni.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnIni.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnIni.HoverImage")));
            this.btnIni.Image = ((System.Drawing.Image)(resources.GetObject("btnIni.Image")));
            this.btnIni.Location = new System.Drawing.Point(6, 6);
            this.btnIni.Margin = new System.Windows.Forms.Padding(6);
            this.btnIni.Name = "btnIni";
            this.btnIni.Size = new System.Drawing.Size(15, 15);
            this.btnIni.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnIni.TabIndex = 24;
            this.btnIni.Tooltip = "出厂默认";
            this.btnIni.Click += new System.EventHandler(this.btnIni_Click);
            // 
            // btnAllWrite
            // 
            this.btnAllWrite.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAllWrite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllWrite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAllWrite.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnAllWrite.FocusCuesEnabled = false;
            this.btnAllWrite.Font = new System.Drawing.Font("黑体", 9F);
            this.btnAllWrite.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnAllWrite.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnAllWrite.HoverImage")));
            this.btnAllWrite.Image = ((System.Drawing.Image)(resources.GetObject("btnAllWrite.Image")));
            this.btnAllWrite.Location = new System.Drawing.Point(33, 6);
            this.btnAllWrite.Margin = new System.Windows.Forms.Padding(6);
            this.btnAllWrite.Name = "btnAllWrite";
            this.btnAllWrite.Size = new System.Drawing.Size(15, 15);
            this.btnAllWrite.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAllWrite.TabIndex = 23;
            this.btnAllWrite.Tooltip = "同类型端口写入";
            this.btnAllWrite.Click += new System.EventHandler(this.btnAllWrite_Click);
            // 
            // btnWrite
            // 
            this.btnWrite.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnWrite.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnWrite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnWrite.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnWrite.FocusCuesEnabled = false;
            this.btnWrite.Font = new System.Drawing.Font("黑体", 9F);
            this.btnWrite.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnWrite.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnWrite.HoverImage")));
            this.btnWrite.Image = ((System.Drawing.Image)(resources.GetObject("btnWrite.Image")));
            this.btnWrite.Location = new System.Drawing.Point(60, 6);
            this.btnWrite.Margin = new System.Windows.Forms.Padding(6);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(15, 15);
            this.btnWrite.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnWrite.TabIndex = 16;
            this.btnWrite.Tooltip = "写入";
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // btnRead
            // 
            this.btnRead.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRead.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRead.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnRead.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnRead.FocusCuesEnabled = false;
            this.btnRead.Font = new System.Drawing.Font("黑体", 9F);
            this.btnRead.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnRead.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnRead.HoverImage")));
            this.btnRead.Image = ((System.Drawing.Image)(resources.GetObject("btnRead.Image")));
            this.btnRead.Location = new System.Drawing.Point(87, 6);
            this.btnRead.Margin = new System.Windows.Forms.Padding(6);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(15, 15);
            this.btnRead.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnRead.TabIndex = 18;
            this.btnRead.Tooltip = "读取";
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnClose.FocusCuesEnabled = false;
            this.btnClose.Font = new System.Drawing.Font("黑体", 9F);
            this.btnClose.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnClose.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnClose.HoverImage")));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(508, 8);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(13, 13);
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.TabIndex = 9;
            this.btnClose.Tooltip = "关闭";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // symbolBox1
            // 
            // 
            // 
            // 
            this.symbolBox1.BackgroundStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("symbolBox1.BackgroundStyle.BackgroundImage")));
            this.symbolBox1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.symbolBox1.Location = new System.Drawing.Point(4, 6);
            this.symbolBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.symbolBox1.Name = "symbolBox1";
            this.symbolBox1.Size = new System.Drawing.Size(13, 13);
            this.symbolBox1.Symbol = "";
            this.symbolBox1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(119)))));
            this.symbolBox1.TabIndex = 7;
            this.symbolBox1.Text = "symbolBox1";
            this.symbolBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.symbolBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(25, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "开关设置";
            this.label6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.label6.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(1, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(527, 242);
            this.panel1.TabIndex = 80;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel4);
            this.groupBox2.Controls.Add(this.cbPower);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.cbLock);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(11, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(503, 146);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "参数";
            // 
            // cbPower
            // 
            this.cbPower.FormattingEnabled = true;
            this.cbPower.Items.AddRange(new object[] {
            "开启",
            "关闭",
            "恢复断电状态"});
            this.cbPower.Location = new System.Drawing.Point(177, 35);
            this.cbPower.Name = "cbPower";
            this.cbPower.Size = new System.Drawing.Size(107, 28);
            this.cbPower.TabIndex = 23;
            this.cbPower.Text = "关闭";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(57, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 20);
            this.label11.TabIndex = 22;
            this.label11.Text = "开关互锁设置：";
            // 
            // cbLock
            // 
            this.cbLock.FormattingEnabled = true;
            this.cbLock.Items.AddRange(new object[] {
            "无"});
            this.cbLock.Location = new System.Drawing.Point(177, 91);
            this.cbLock.Name = "cbLock";
            this.cbLock.Size = new System.Drawing.Size(107, 28);
            this.cbLock.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(57, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(107, 20);
            this.label10.TabIndex = 19;
            this.label10.Text = "上电状态设置：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.btnOn);
            this.groupBox1.Controls.Add(this.btnOff);
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(11, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(503, 72);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(459, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // btnOn
            // 
            this.btnOn.BackColor = System.Drawing.Color.White;
            this.btnOn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOn.ForeColor = System.Drawing.Color.Black;
            this.btnOn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOn.Location = new System.Drawing.Point(61, 30);
            this.btnOn.Margin = new System.Windows.Forms.Padding(20);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(98, 25);
            this.btnOn.TabIndex = 15;
            this.btnOn.Text = "开启";
            this.btnOn.UseVisualStyleBackColor = false;
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // btnOff
            // 
            this.btnOff.BackColor = System.Drawing.Color.White;
            this.btnOff.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOff.ForeColor = System.Drawing.Color.Black;
            this.btnOff.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOff.Location = new System.Drawing.Point(199, 30);
            this.btnOff.Margin = new System.Windows.Forms.Padding(20);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(98, 25);
            this.btnOff.TabIndex = 14;
            this.btnOff.Text = "关闭";
            this.btnOff.UseVisualStyleBackColor = false;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // btnTest
            // 
            this.btnTest.BackColor = System.Drawing.Color.White;
            this.btnTest.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTest.ForeColor = System.Drawing.Color.Black;
            this.btnTest.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnTest.Location = new System.Drawing.Point(337, 30);
            this.btnTest.Margin = new System.Windows.Forms.Padding(20);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(98, 25);
            this.btnTest.TabIndex = 13;
            this.btnTest.Text = "检测";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 5000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.lbTip);
            this.panel4.Controls.Add(this.pgBar);
            this.panel4.Location = new System.Drawing.Point(130, 10);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(249, 80);
            this.panel4.TabIndex = 36;
            this.panel4.Visible = false;
            // 
            // lbTip
            // 
            this.lbTip.AutoSize = true;
            this.lbTip.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTip.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbTip.Location = new System.Drawing.Point(123, 60);
            this.lbTip.Name = "lbTip";
            this.lbTip.Size = new System.Drawing.Size(121, 17);
            this.lbTip.TabIndex = 67;
            this.lbTip.Text = "正在写入，请稍后. . .";
            // 
            // pgBar
            // 
            // 
            // 
            // 
            this.pgBar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.pgBar.Location = new System.Drawing.Point(17, 32);
            this.pgBar.Name = "pgBar";
            this.pgBar.Size = new System.Drawing.Size(216, 17);
            this.pgBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.pgBar.TabIndex = 66;
            // 
            // setSwitch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(530, 271);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plInfoTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "setSwitch";
            this.Text = "setSwitch";
            this.Load += new System.EventHandler(this.setSwitch_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.setSwitch_Paint);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOn;
        private System.Windows.Forms.Button btnOff;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevComponents.DotNetBar.ButtonX btnIni;
        private DevComponents.DotNetBar.ButtonX btnAllWrite;
        private DevComponents.DotNetBar.ButtonX btnWrite;
        private DevComponents.DotNetBar.ButtonX btnRead;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbLock;
        private System.Windows.Forms.ComboBox cbPower;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lbTip;
        private DevComponents.DotNetBar.Controls.ProgressBarX pgBar;
    }
}