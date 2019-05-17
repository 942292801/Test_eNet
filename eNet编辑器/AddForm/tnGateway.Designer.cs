namespace eNet编辑器.AddForm
{
    partial class tnGateway
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tnGateway));
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDecid = new DevComponents.DotNetBar.ButtonX();
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.cbVersion = new System.Windows.Forms.ComboBox();
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
            this.plInfoTitle.TabIndex = 80;
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
            this.lbTitle.Text = "添加网关";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnDecid);
            this.panel1.Controls.Add(this.txtGateway);
            this.panel1.Controls.Add(this.cbVersion);
            this.panel1.Location = new System.Drawing.Point(1, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(211, 125);
            this.panel1.TabIndex = 81;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "网关：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label2.Location = new System.Drawing.Point(19, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "型号：";
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
            this.btnDecid.Location = new System.Drawing.Point(175, 96);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(25, 20);
            this.btnDecid.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDecid.TabIndex = 4;
            this.btnDecid.Tooltip = "确认";
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // txtGateway
            // 
            this.txtGateway.Location = new System.Drawing.Point(77, 15);
            this.txtGateway.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(111, 23);
            this.txtGateway.TabIndex = 55;
            this.txtGateway.Text = "192.168.1.230";
            // 
            // cbVersion
            // 
            this.cbVersion.Enabled = false;
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Location = new System.Drawing.Point(77, 58);
            this.cbVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(111, 25);
            this.cbVersion.TabIndex = 56;
            this.cbVersion.Text = "GW100A";
            // 
            // tnGateway
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(214, 154);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.plInfoTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.HangulFull;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "tnGateway";
            this.Load += new System.EventHandler(this.tnGateway_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.tnGateway_Paint);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plInfoTitle;
        private System.Windows.Forms.Label lbTitle;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevComponents.DotNetBar.ButtonX btnDecid;
        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.ComboBox cbVersion;
        private DevComponents.DotNetBar.ButtonX btnClose;
    }
}