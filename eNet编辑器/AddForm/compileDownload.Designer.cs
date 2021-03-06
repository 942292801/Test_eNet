﻿namespace eNet编辑器.AddForm
{
    partial class compileDownload
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(compileDownload));
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnSend = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cbTargetIp = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSourceIP = new System.Windows.Forms.ComboBox();
            this.lbName = new System.Windows.Forms.Label();
            this.plInfoTitle.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnSend);
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.lbTitle);
            this.plInfoTitle.Location = new System.Drawing.Point(1, 1);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(220, 25);
            this.plInfoTitle.TabIndex = 3;
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // btnSend
            // 
            this.btnSend.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSend.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnSend.FocusCuesEnabled = false;
            this.btnSend.Font = new System.Drawing.Font("黑体", 9F);
            this.btnSend.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnSend.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnSend.HoverImage")));
            this.btnSend.Image = ((System.Drawing.Image)(resources.GetObject("btnSend.Image")));
            this.btnSend.Location = new System.Drawing.Point(180, 5);
            this.btnSend.Margin = new System.Windows.Forms.Padding(6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(15, 15);
            this.btnSend.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSend.TabIndex = 20;
            this.btnSend.Tooltip = "编译下载";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
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
            this.btnClose.Location = new System.Drawing.Point(202, 6);
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
            this.lbTitle.Size = new System.Drawing.Size(56, 17);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "编译下载";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.cbTargetIp);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.cbSourceIP);
            this.panel4.Controls.Add(this.lbName);
            this.panel4.Location = new System.Drawing.Point(1, 27);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(220, 100);
            this.panel4.TabIndex = 5;
            // 
            // cbTargetIp
            // 
            this.cbTargetIp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTargetIp.FormattingEnabled = true;
            this.cbTargetIp.Location = new System.Drawing.Point(79, 53);
            this.cbTargetIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbTargetIp.Name = "cbTargetIp";
            this.cbTargetIp.Size = new System.Drawing.Size(122, 25);
            this.cbTargetIp.TabIndex = 67;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(14, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 66;
            this.label1.Text = "下载到：";
            // 
            // cbSourceIP
            // 
            this.cbSourceIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSourceIP.FormattingEnabled = true;
            this.cbSourceIP.Location = new System.Drawing.Point(79, 14);
            this.cbSourceIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbSourceIP.Name = "cbSourceIP";
            this.cbSourceIP.Size = new System.Drawing.Size(122, 25);
            this.cbSourceIP.TabIndex = 65;
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbName.Location = new System.Drawing.Point(14, 17);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(68, 17);
            this.lbName.TabIndex = 64;
            this.lbName.Text = "工程文件：";
            // 
            // compileDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(223, 129);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.plInfoTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "compileDownload";
            this.Text = "compileDownload";
            this.Load += new System.EventHandler(this.compileDownload_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.compileDownload_Paint);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox cbSourceIP;
        private System.Windows.Forms.Label lbName;
        private DevComponents.DotNetBar.ButtonX btnSend;
        private System.Windows.Forms.ComboBox cbTargetIp;
        private System.Windows.Forms.Label label1;
    }
}