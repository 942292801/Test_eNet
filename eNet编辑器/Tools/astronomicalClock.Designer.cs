namespace eNet编辑器.Tools
{
    partial class astronomicalClock
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(astronomicalClock));
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnUpdate = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cbltSeconds = new System.Windows.Forms.ComboBox();
            this.cbltMinute = new System.Windows.Forms.ComboBox();
            this.cbltDegree = new System.Windows.Forms.ComboBox();
            this.cbLatitude = new System.Windows.Forms.ComboBox();
            this.cblgSeconds = new System.Windows.Forms.ComboBox();
            this.cblgMinute = new System.Windows.Forms.ComboBox();
            this.cblgDegree = new System.Windows.Forms.ComboBox();
            this.cbLongitude = new System.Windows.Forms.ComboBox();
            this.cbTimeZone = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbSunset = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbSunrise = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefresh = new DevComponents.DotNetBar.ButtonX();
            this.cbOnlineIP = new System.Windows.Forms.ComboBox();
            this.lbName = new System.Windows.Forms.Label();
            this.plInfoTitle.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnUpdate);
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.lbTitle);
            this.plInfoTitle.Location = new System.Drawing.Point(1, 1);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(306, 25);
            this.plInfoTitle.TabIndex = 8;
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // btnUpdate
            // 
            this.btnUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnUpdate.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnUpdate.FocusCuesEnabled = false;
            this.btnUpdate.Font = new System.Drawing.Font("黑体", 9F);
            this.btnUpdate.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnUpdate.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnUpdate.HoverImage")));
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.Location = new System.Drawing.Point(263, 6);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(6);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(15, 15);
            this.btnUpdate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnUpdate.TabIndex = 20;
            this.btnUpdate.Tooltip = "设置";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnClose.FocusCuesEnabled = false;
            this.btnClose.Font = new System.Drawing.Font("黑体", 9F);
            this.btnClose.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnClose.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnClose.HoverImage")));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(287, 6);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(13, 13);
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.TabIndex = 8;
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
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbTitle.Location = new System.Drawing.Point(25, 4);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(80, 17);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "天文时钟设置";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.cbltSeconds);
            this.panel4.Controls.Add(this.cbltMinute);
            this.panel4.Controls.Add(this.cbltDegree);
            this.panel4.Controls.Add(this.cbLatitude);
            this.panel4.Controls.Add(this.cblgSeconds);
            this.panel4.Controls.Add(this.cblgMinute);
            this.panel4.Controls.Add(this.cblgDegree);
            this.panel4.Controls.Add(this.cbLongitude);
            this.panel4.Controls.Add(this.cbTimeZone);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.lbSunset);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.lbSunrise);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.btnRefresh);
            this.panel4.Controls.Add(this.cbOnlineIP);
            this.panel4.Controls.Add(this.lbName);
            this.panel4.Location = new System.Drawing.Point(1, 27);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(306, 223);
            this.panel4.TabIndex = 9;
            // 
            // cbltSeconds
            // 
            this.cbltSeconds.FormattingEnabled = true;
            this.cbltSeconds.Location = new System.Drawing.Point(239, 165);
            this.cbltSeconds.Name = "cbltSeconds";
            this.cbltSeconds.Size = new System.Drawing.Size(53, 25);
            this.cbltSeconds.TabIndex = 107;
            // 
            // cbltMinute
            // 
            this.cbltMinute.FormattingEnabled = true;
            this.cbltMinute.Location = new System.Drawing.Point(180, 165);
            this.cbltMinute.Name = "cbltMinute";
            this.cbltMinute.Size = new System.Drawing.Size(53, 25);
            this.cbltMinute.TabIndex = 106;
            // 
            // cbltDegree
            // 
            this.cbltDegree.FormattingEnabled = true;
            this.cbltDegree.Location = new System.Drawing.Point(121, 165);
            this.cbltDegree.Name = "cbltDegree";
            this.cbltDegree.Size = new System.Drawing.Size(53, 25);
            this.cbltDegree.TabIndex = 105;
            // 
            // cbLatitude
            // 
            this.cbLatitude.FormattingEnabled = true;
            this.cbLatitude.Items.AddRange(new object[] {
            "北纬",
            "南纬"});
            this.cbLatitude.Location = new System.Drawing.Point(63, 165);
            this.cbLatitude.Name = "cbLatitude";
            this.cbLatitude.Size = new System.Drawing.Size(53, 25);
            this.cbLatitude.TabIndex = 104;
            // 
            // cblgSeconds
            // 
            this.cblgSeconds.FormattingEnabled = true;
            this.cblgSeconds.Location = new System.Drawing.Point(239, 128);
            this.cblgSeconds.Name = "cblgSeconds";
            this.cblgSeconds.Size = new System.Drawing.Size(53, 25);
            this.cblgSeconds.TabIndex = 103;
            // 
            // cblgMinute
            // 
            this.cblgMinute.FormattingEnabled = true;
            this.cblgMinute.Location = new System.Drawing.Point(180, 128);
            this.cblgMinute.Name = "cblgMinute";
            this.cblgMinute.Size = new System.Drawing.Size(53, 25);
            this.cblgMinute.TabIndex = 102;
            // 
            // cblgDegree
            // 
            this.cblgDegree.FormattingEnabled = true;
            this.cblgDegree.Location = new System.Drawing.Point(121, 128);
            this.cblgDegree.Name = "cblgDegree";
            this.cblgDegree.Size = new System.Drawing.Size(53, 25);
            this.cblgDegree.TabIndex = 101;
            // 
            // cbLongitude
            // 
            this.cbLongitude.FormattingEnabled = true;
            this.cbLongitude.Items.AddRange(new object[] {
            "东经",
            "西经"});
            this.cbLongitude.Location = new System.Drawing.Point(63, 128);
            this.cbLongitude.Name = "cbLongitude";
            this.cbLongitude.Size = new System.Drawing.Size(53, 25);
            this.cbLongitude.TabIndex = 100;
            // 
            // cbTimeZone
            // 
            this.cbTimeZone.FormattingEnabled = true;
            this.cbTimeZone.Location = new System.Drawing.Point(63, 92);
            this.cbTimeZone.Name = "cbTimeZone";
            this.cbTimeZone.Size = new System.Drawing.Size(111, 25);
            this.cbTimeZone.TabIndex = 98;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 168);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 17);
            this.label9.TabIndex = 97;
            this.label9.Text = "纬度：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label8.Location = new System.Drawing.Point(15, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 17);
            this.label8.TabIndex = 94;
            this.label8.Text = "时区：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 17);
            this.label7.TabIndex = 93;
            this.label7.Text = "经度：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(202, 199);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 17);
            this.label4.TabIndex = 90;
            this.label4.Text = "格式：度/分/秒";
            // 
            // lbSunset
            // 
            this.lbSunset.AutoSize = true;
            this.lbSunset.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbSunset.Location = new System.Drawing.Point(204, 58);
            this.lbSunset.Name = "lbSunset";
            this.lbSunset.Size = new System.Drawing.Size(39, 17);
            this.lbSunset.TabIndex = 78;
            this.lbSunset.Text = "16:48";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label3.Location = new System.Drawing.Point(140, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 77;
            this.label3.Text = "日落时间：";
            // 
            // lbSunrise
            // 
            this.lbSunrise.AutoSize = true;
            this.lbSunrise.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbSunrise.Location = new System.Drawing.Point(77, 58);
            this.lbSunrise.Name = "lbSunrise";
            this.lbSunrise.Size = new System.Drawing.Size(32, 17);
            this.lbSunrise.TabIndex = 76;
            this.lbSunrise.Text = "6:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(15, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 75;
            this.label1.Text = "日出时间：";
            // 
            // btnRefresh
            // 
            this.btnRefresh.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnRefresh.FocusCuesEnabled = false;
            this.btnRefresh.Font = new System.Drawing.Font("黑体", 9F);
            this.btnRefresh.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnRefresh.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.HoverImage")));
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(194, 19);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(6);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(15, 15);
            this.btnRefresh.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnRefresh.TabIndex = 70;
            this.btnRefresh.Tooltip = "获取信息";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cbOnlineIP
            // 
            this.cbOnlineIP.FormattingEnabled = true;
            this.cbOnlineIP.Location = new System.Drawing.Point(63, 14);
            this.cbOnlineIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbOnlineIP.Name = "cbOnlineIP";
            this.cbOnlineIP.Size = new System.Drawing.Size(122, 25);
            this.cbOnlineIP.TabIndex = 65;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbName.Location = new System.Drawing.Point(13, 17);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(44, 17);
            this.lbName.TabIndex = 64;
            this.lbName.Text = "主机：";
            // 
            // astronomicalClock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(309, 252);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.plInfoTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "astronomicalClock";
            this.Text = "astronomicalClock";
            this.Load += new System.EventHandler(this.astronomicalClock_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.astronomicalClock_Paint);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnUpdate;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Panel panel4;
        private DevComponents.DotNetBar.ButtonX btnRefresh;
        private System.Windows.Forms.ComboBox cbOnlineIP;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbSunset;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbSunrise;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbltSeconds;
        private System.Windows.Forms.ComboBox cbltMinute;
        private System.Windows.Forms.ComboBox cbltDegree;
        private System.Windows.Forms.ComboBox cbLatitude;
        private System.Windows.Forms.ComboBox cblgSeconds;
        private System.Windows.Forms.ComboBox cblgMinute;
        private System.Windows.Forms.ComboBox cblgDegree;
        private System.Windows.Forms.ComboBox cbLongitude;
        private System.Windows.Forms.ComboBox cbTimeZone;
    }
}