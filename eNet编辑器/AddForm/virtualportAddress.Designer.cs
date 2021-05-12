namespace eNet编辑器.AddForm
{
    partial class virtualportAddress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(virtualportAddress));
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.lbName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb1 = new System.Windows.Forms.Label();
            this.cb1 = new System.Windows.Forms.ComboBox();
            this.btnDecid = new DevComponents.DotNetBar.ButtonX();
            this.cb2 = new System.Windows.Forms.ComboBox();
            this.cb4 = new System.Windows.Forms.ComboBox();
            this.cb3 = new System.Windows.Forms.ComboBox();
            this.lb4 = new System.Windows.Forms.Label();
            this.lb2 = new System.Windows.Forms.Label();
            this.lb3 = new System.Windows.Forms.Label();
            this.plInfoTitle.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.lbName);
            this.plInfoTitle.Location = new System.Drawing.Point(1, 1);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(211, 25);
            this.plInfoTitle.TabIndex = 83;
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
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
            this.btnClose.Location = new System.Drawing.Point(192, 6);
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
            this.symbolBox1.Location = new System.Drawing.Point(5, 6);
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
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbName.Location = new System.Drawing.Point(25, 4);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(56, 17);
            this.lbName.TabIndex = 1;
            this.lbName.Text = "对象地址";
            this.lbName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbName.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lb1);
            this.panel1.Controls.Add(this.cb1);
            this.panel1.Controls.Add(this.btnDecid);
            this.panel1.Controls.Add(this.cb2);
            this.panel1.Controls.Add(this.cb4);
            this.panel1.Controls.Add(this.cb3);
            this.panel1.Controls.Add(this.lb4);
            this.panel1.Controls.Add(this.lb2);
            this.panel1.Controls.Add(this.lb3);
            this.panel1.Location = new System.Drawing.Point(1, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(211, 287);
            this.panel1.TabIndex = 84;
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb1.Location = new System.Drawing.Point(15, 9);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(32, 17);
            this.lb1.TabIndex = 72;
            this.lb1.Text = "网关";
            // 
            // cb1
            // 
            this.cb1.Enabled = false;
            this.cb1.FormattingEnabled = true;
            this.cb1.Location = new System.Drawing.Point(19, 36);
            this.cb1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cb1.Name = "cb1";
            this.cb1.Size = new System.Drawing.Size(175, 25);
            this.cb1.TabIndex = 73;
            this.cb1.Text = "254";
            // 
            // btnDecid
            // 
            this.btnDecid.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDecid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDecid.BackColor = System.Drawing.Color.White;
            this.btnDecid.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDecid.FocusCuesEnabled = false;
            this.btnDecid.Font = new System.Drawing.Font("黑体", 9F);
            this.btnDecid.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnDecid.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDecid.HoverImage")));
            this.btnDecid.Image = ((System.Drawing.Image)(resources.GetObject("btnDecid.Image")));
            this.btnDecid.Location = new System.Drawing.Point(169, 257);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(6);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(24, 24);
            this.btnDecid.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDecid.TabIndex = 4;
            this.btnDecid.Tooltip = "确认";
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // cb2
            // 
            this.cb2.Enabled = false;
            this.cb2.FormattingEnabled = true;
            this.cb2.Location = new System.Drawing.Point(18, 98);
            this.cb2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cb2.Name = "cb2";
            this.cb2.Size = new System.Drawing.Size(175, 25);
            this.cb2.TabIndex = 74;
            this.cb2.Text = "251";
            // 
            // cb4
            // 
            this.cb4.FormattingEnabled = true;
            this.cb4.Location = new System.Drawing.Point(18, 223);
            this.cb4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cb4.Name = "cb4";
            this.cb4.Size = new System.Drawing.Size(175, 25);
            this.cb4.TabIndex = 79;
            // 
            // cb3
            // 
            this.cb3.Enabled = false;
            this.cb3.FormattingEnabled = true;
            this.cb3.Location = new System.Drawing.Point(18, 161);
            this.cb3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cb3.Name = "cb3";
            this.cb3.Size = new System.Drawing.Size(175, 25);
            this.cb3.TabIndex = 75;
            this.cb3.Text = "3";
            // 
            // lb4
            // 
            this.lb4.AutoSize = true;
            this.lb4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb4.Location = new System.Drawing.Point(15, 196);
            this.lb4.Name = "lb4";
            this.lb4.Size = new System.Drawing.Size(32, 17);
            this.lb4.TabIndex = 78;
            this.lb4.Text = "端口";
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb2.Location = new System.Drawing.Point(15, 72);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(32, 17);
            this.lb2.TabIndex = 76;
            this.lb2.Text = "类型";
            // 
            // lb3
            // 
            this.lb3.AutoSize = true;
            this.lb3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb3.Location = new System.Drawing.Point(15, 134);
            this.lb3.Name = "lb3";
            this.lb3.Size = new System.Drawing.Size(32, 17);
            this.lb3.TabIndex = 77;
            this.lb3.Text = "设备";
            // 
            // virtualportAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(214, 316);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plInfoTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "virtualportAddress";
            this.Text = "virtualportAddress";
            this.Load += new System.EventHandler(this.virtualportAddress_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.virtualportAddress_Paint);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.ComboBox cb1;
        private DevComponents.DotNetBar.ButtonX btnDecid;
        private System.Windows.Forms.ComboBox cb2;
        private System.Windows.Forms.ComboBox cb4;
        private System.Windows.Forms.ComboBox cb3;
        private System.Windows.Forms.Label lb4;
        private System.Windows.Forms.Label lb2;
        private System.Windows.Forms.Label lb3;
    }
}