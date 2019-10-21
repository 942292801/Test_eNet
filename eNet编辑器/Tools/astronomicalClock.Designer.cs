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
            resources.ApplyResources(this.plInfoTitle, "plInfoTitle");
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnUpdate);
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.lbTitle);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnUpdate.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnUpdate.FocusCuesEnabled = false;
            this.btnUpdate.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnUpdate.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnUpdate.HoverImage")));
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.btnClose.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnClose.FocusCuesEnabled = false;
            this.btnClose.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnClose.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnClose.HoverImage")));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Name = "btnClose";
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // symbolBox1
            // 
            resources.ApplyResources(this.symbolBox1, "symbolBox1");
            // 
            // 
            // 
            this.symbolBox1.BackgroundStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("symbolBox1.BackgroundStyle.BackgroundImage")));
            this.symbolBox1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.symbolBox1.Name = "symbolBox1";
            this.symbolBox1.Symbol = "";
            this.symbolBox1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(119)))));
            this.symbolBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.symbolBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // lbTitle
            // 
            resources.ApplyResources(this.lbTitle, "lbTitle");
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // panel4
            // 
            resources.ApplyResources(this.panel4, "panel4");
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
            this.panel4.Name = "panel4";
            // 
            // cbltSeconds
            // 
            resources.ApplyResources(this.cbltSeconds, "cbltSeconds");
            this.cbltSeconds.FormattingEnabled = true;
            this.cbltSeconds.Name = "cbltSeconds";
            // 
            // cbltMinute
            // 
            resources.ApplyResources(this.cbltMinute, "cbltMinute");
            this.cbltMinute.FormattingEnabled = true;
            this.cbltMinute.Name = "cbltMinute";
            // 
            // cbltDegree
            // 
            resources.ApplyResources(this.cbltDegree, "cbltDegree");
            this.cbltDegree.FormattingEnabled = true;
            this.cbltDegree.Name = "cbltDegree";
            // 
            // cbLatitude
            // 
            resources.ApplyResources(this.cbLatitude, "cbLatitude");
            this.cbLatitude.FormattingEnabled = true;
            this.cbLatitude.Items.AddRange(new object[] {
            resources.GetString("cbLatitude.Items"),
            resources.GetString("cbLatitude.Items1")});
            this.cbLatitude.Name = "cbLatitude";
            // 
            // cblgSeconds
            // 
            resources.ApplyResources(this.cblgSeconds, "cblgSeconds");
            this.cblgSeconds.FormattingEnabled = true;
            this.cblgSeconds.Name = "cblgSeconds";
            // 
            // cblgMinute
            // 
            resources.ApplyResources(this.cblgMinute, "cblgMinute");
            this.cblgMinute.FormattingEnabled = true;
            this.cblgMinute.Name = "cblgMinute";
            // 
            // cblgDegree
            // 
            resources.ApplyResources(this.cblgDegree, "cblgDegree");
            this.cblgDegree.FormattingEnabled = true;
            this.cblgDegree.Name = "cblgDegree";
            // 
            // cbLongitude
            // 
            resources.ApplyResources(this.cbLongitude, "cbLongitude");
            this.cbLongitude.FormattingEnabled = true;
            this.cbLongitude.Items.AddRange(new object[] {
            resources.GetString("cbLongitude.Items"),
            resources.GetString("cbLongitude.Items1")});
            this.cbLongitude.Name = "cbLongitude";
            // 
            // cbTimeZone
            // 
            resources.ApplyResources(this.cbTimeZone, "cbTimeZone");
            this.cbTimeZone.FormattingEnabled = true;
            this.cbTimeZone.Name = "cbTimeZone";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Name = "label4";
            // 
            // lbSunset
            // 
            resources.ApplyResources(this.lbSunset, "lbSunset");
            this.lbSunset.Name = "lbSunset";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lbSunrise
            // 
            resources.ApplyResources(this.lbSunrise, "lbSunrise");
            this.lbSunrise.Name = "lbSunrise";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnRefresh.FocusCuesEnabled = false;
            this.btnRefresh.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnRefresh.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnRefresh.HoverImage")));
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cbOnlineIP
            // 
            resources.ApplyResources(this.cbOnlineIP, "cbOnlineIP");
            this.cbOnlineIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOnlineIP.FormattingEnabled = true;
            this.cbOnlineIP.Name = "cbOnlineIP";
            // 
            // lbName
            // 
            resources.ApplyResources(this.lbName, "lbName");
            this.lbName.Name = "lbName";
            // 
            // astronomicalClock
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.plInfoTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "astronomicalClock";
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