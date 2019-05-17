namespace eNet编辑器.AddForm
{
    partial class timerAdd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(timerAdd));
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnDecid = new DevComponents.DotNetBar.ButtonX();
            this.txtName = new System.Windows.Forms.ComboBox();
            this.cbs4 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbs3 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbs2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbs1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.txtNum = new System.Windows.Forms.TextBox();
            this.lbNum = new System.Windows.Forms.Label();
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.plInfoTitle.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.lbTitle);
            this.plInfoTitle.Location = new System.Drawing.Point(1, 1);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(211, 25);
            this.plInfoTitle.TabIndex = 92;
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
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbTitle.Location = new System.Drawing.Point(25, 4);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(56, 17);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "添加定时";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // btnDecid
            // 
            this.btnDecid.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDecid.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDecid.BackColor = System.Drawing.Color.White;
            this.btnDecid.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDecid.FocusCuesEnabled = false;
            this.btnDecid.Font = new System.Drawing.Font("黑体", 9F);
            this.btnDecid.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnDecid.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDecid.HoverImage")));
            this.btnDecid.Image = ((System.Drawing.Image)(resources.GetObject("btnDecid.Image")));
            this.btnDecid.Location = new System.Drawing.Point(172, 297);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(25, 20);
            this.btnDecid.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDecid.TabIndex = 5;
            this.btnDecid.Tooltip = "确认";
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // txtName
            // 
            this.txtName.FormattingEnabled = true;
            this.txtName.Location = new System.Drawing.Point(79, 261);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(115, 25);
            this.txtName.TabIndex = 91;
            // 
            // cbs4
            // 
            this.cbs4.Enabled = false;
            this.cbs4.FormattingEnabled = true;
            this.cbs4.Location = new System.Drawing.Point(79, 217);
            this.cbs4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs4.Name = "cbs4";
            this.cbs4.Size = new System.Drawing.Size(115, 25);
            this.cbs4.TabIndex = 90;
            this.cbs4.SelectedIndexChanged += new System.EventHandler(this.cbs4_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label5.Location = new System.Drawing.Point(15, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 89;
            this.label5.Text = "区域4：";
            // 
            // cbs3
            // 
            this.cbs3.Enabled = false;
            this.cbs3.FormattingEnabled = true;
            this.cbs3.Location = new System.Drawing.Point(79, 175);
            this.cbs3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs3.Name = "cbs3";
            this.cbs3.Size = new System.Drawing.Size(115, 25);
            this.cbs3.TabIndex = 88;
            this.cbs3.SelectedIndexChanged += new System.EventHandler(this.cbs3_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label4.Location = new System.Drawing.Point(15, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 87;
            this.label4.Text = "区域3：";
            // 
            // cbs2
            // 
            this.cbs2.Enabled = false;
            this.cbs2.FormattingEnabled = true;
            this.cbs2.Location = new System.Drawing.Point(79, 136);
            this.cbs2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs2.Name = "cbs2";
            this.cbs2.Size = new System.Drawing.Size(115, 25);
            this.cbs2.TabIndex = 86;
            this.cbs2.SelectedIndexChanged += new System.EventHandler(this.cbs2_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label3.Location = new System.Drawing.Point(15, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 85;
            this.label3.Text = "区域2：";
            // 
            // cbs1
            // 
            this.cbs1.Enabled = false;
            this.cbs1.FormattingEnabled = true;
            this.cbs1.Location = new System.Drawing.Point(79, 94);
            this.cbs1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs1.Name = "cbs1";
            this.cbs1.Size = new System.Drawing.Size(115, 25);
            this.cbs1.TabIndex = 84;
            this.cbs1.SelectedIndexChanged += new System.EventHandler(this.cbs1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label2.Location = new System.Drawing.Point(15, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 83;
            this.label2.Text = "区域1：";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbName.Location = new System.Drawing.Point(15, 262);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(44, 17);
            this.lbName.TabIndex = 82;
            this.lbName.Text = "名称：";
            // 
            // txtNum
            // 
            this.txtNum.Location = new System.Drawing.Point(79, 53);
            this.txtNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(115, 23);
            this.txtNum.TabIndex = 81;
            // 
            // lbNum
            // 
            this.lbNum.AutoSize = true;
            this.lbNum.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbNum.Location = new System.Drawing.Point(15, 53);
            this.lbNum.Name = "lbNum";
            this.lbNum.Size = new System.Drawing.Size(56, 17);
            this.lbNum.TabIndex = 80;
            this.lbNum.Text = "定时号：";
            // 
            // txtGateway
            // 
            this.txtGateway.Enabled = false;
            this.txtGateway.Location = new System.Drawing.Point(79, 15);
            this.txtGateway.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(115, 23);
            this.txtGateway.TabIndex = 79;
            this.txtGateway.Text = "192.168.1.230";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 78;
            this.label1.Text = "网关：";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnDecid);
            this.panel1.Controls.Add(this.txtGateway);
            this.panel1.Controls.Add(this.lbNum);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.txtNum);
            this.panel1.Controls.Add(this.cbs4);
            this.panel1.Controls.Add(this.lbName);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cbs3);
            this.panel1.Controls.Add(this.cbs1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cbs2);
            this.panel1.Location = new System.Drawing.Point(1, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(211, 326);
            this.panel1.TabIndex = 93;
            // 
            // timerAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(214, 355);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plInfoTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "timerAdd";
            this.Text = "timerAdd";
            this.Load += new System.EventHandler(this.timerAdd_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.timerAdd_Paint);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plInfoTitle;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.ComboBox txtName;
        private System.Windows.Forms.ComboBox cbs4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbs3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbs2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbs1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.TextBox txtNum;
        private System.Windows.Forms.Label lbNum;
        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.Label label1;
        private DevComponents.DotNetBar.ButtonX btnDecid;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Panel panel1;
        private DevComponents.DotNetBar.ButtonX btnClose;

    }
}