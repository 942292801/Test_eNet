﻿namespace eNet编辑器.AddForm
{
    partial class sceneAdd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(sceneAdd));
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNum = new System.Windows.Forms.TextBox();
            this.lbNum = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.cbs1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbs2 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbs3 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbs4 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.ComboBox();
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.btnDecid = new DevComponents.DotNetBar.ButtonX();
            this.label6 = new System.Windows.Forms.Label();
            this.plInfoTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtGateway
            // 
            this.txtGateway.Enabled = false;
            this.txtGateway.Location = new System.Drawing.Point(75, 38);
            this.txtGateway.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(191, 23);
            this.txtGateway.TabIndex = 63;
            this.txtGateway.Text = "192.168.1.230";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.Location = new System.Drawing.Point(11, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 57;
            this.label1.Text = "网关：";
            // 
            // txtNum
            // 
            this.txtNum.Location = new System.Drawing.Point(75, 76);
            this.txtNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(191, 23);
            this.txtNum.TabIndex = 65;
            // 
            // lbNum
            // 
            this.lbNum.AutoSize = true;
            this.lbNum.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbNum.Location = new System.Drawing.Point(11, 76);
            this.lbNum.Name = "lbNum";
            this.lbNum.Size = new System.Drawing.Size(56, 17);
            this.lbNum.TabIndex = 64;
            this.lbNum.Text = "场景号：";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lbName.Location = new System.Drawing.Point(11, 285);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(44, 17);
            this.lbName.TabIndex = 66;
            this.lbName.Text = "名称：";
            // 
            // cbs1
            // 
            this.cbs1.Enabled = false;
            this.cbs1.FormattingEnabled = true;
            this.cbs1.Location = new System.Drawing.Point(75, 117);
            this.cbs1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs1.Name = "cbs1";
            this.cbs1.Size = new System.Drawing.Size(191, 25);
            this.cbs1.TabIndex = 69;
            this.cbs1.SelectedIndexChanged += new System.EventHandler(this.cbs1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label2.Location = new System.Drawing.Point(11, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 17);
            this.label2.TabIndex = 68;
            this.label2.Text = "区域1：";
            // 
            // cbs2
            // 
            this.cbs2.Enabled = false;
            this.cbs2.FormattingEnabled = true;
            this.cbs2.Location = new System.Drawing.Point(75, 159);
            this.cbs2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs2.Name = "cbs2";
            this.cbs2.Size = new System.Drawing.Size(191, 25);
            this.cbs2.TabIndex = 71;
            this.cbs2.SelectedIndexChanged += new System.EventHandler(this.cbs2_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label3.Location = new System.Drawing.Point(11, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 17);
            this.label3.TabIndex = 70;
            this.label3.Text = "区域2：";
            // 
            // cbs3
            // 
            this.cbs3.Enabled = false;
            this.cbs3.FormattingEnabled = true;
            this.cbs3.Location = new System.Drawing.Point(75, 198);
            this.cbs3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs3.Name = "cbs3";
            this.cbs3.Size = new System.Drawing.Size(191, 25);
            this.cbs3.TabIndex = 73;
            this.cbs3.SelectedIndexChanged += new System.EventHandler(this.cbs3_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label4.Location = new System.Drawing.Point(11, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 72;
            this.label4.Text = "区域3：";
            // 
            // cbs4
            // 
            this.cbs4.Enabled = false;
            this.cbs4.FormattingEnabled = true;
            this.cbs4.Location = new System.Drawing.Point(75, 240);
            this.cbs4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs4.Name = "cbs4";
            this.cbs4.Size = new System.Drawing.Size(191, 25);
            this.cbs4.TabIndex = 75;
            this.cbs4.SelectedIndexChanged += new System.EventHandler(this.cbs4_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label5.Location = new System.Drawing.Point(11, 240);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 17);
            this.label5.TabIndex = 74;
            this.label5.Text = "区域4：";
            // 
            // txtName
            // 
            this.txtName.FormattingEnabled = true;
            this.txtName.Location = new System.Drawing.Point(75, 284);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(191, 25);
            this.txtName.TabIndex = 76;
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.label6);
            this.plInfoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plInfoTitle.Location = new System.Drawing.Point(0, 0);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(274, 25);
            this.plInfoTitle.TabIndex = 77;
            // 
            // symbolBox1
            // 
            // 
            // 
            // 
            this.symbolBox1.BackgroundStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("symbolBox1.BackgroundStyle.BackgroundImage")));
            this.symbolBox1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.symbolBox1.Location = new System.Drawing.Point(6, 6);
            this.symbolBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.symbolBox1.Name = "symbolBox1";
            this.symbolBox1.Size = new System.Drawing.Size(13, 13);
            this.symbolBox1.Symbol = "";
            this.symbolBox1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(119)))));
            this.symbolBox1.TabIndex = 7;
            this.symbolBox1.Text = "symbolBox1";
            // 
            // btnDecid
            // 
            this.btnDecid.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDecid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDecid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDecid.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDecid.FocusCuesEnabled = false;
            this.btnDecid.Font = new System.Drawing.Font("黑体", 9F);
            this.btnDecid.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image;
            this.btnDecid.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDecid.HoverImage")));
            this.btnDecid.Image = ((System.Drawing.Image)(resources.GetObject("btnDecid.Image")));
            this.btnDecid.Location = new System.Drawing.Point(236, 320);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(6);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(25, 20);
            this.btnDecid.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDecid.TabIndex = 4;
            this.btnDecid.Tooltip = "添加";
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(24, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "添加场景";
            // 
            // sceneAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(274, 351);
            this.Controls.Add(this.plInfoTitle);
            this.Controls.Add(this.btnDecid);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.cbs4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbs3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbs2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbs1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.txtNum);
            this.Controls.Add(this.lbNum);
            this.Controls.Add(this.txtGateway);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ImeMode = System.Windows.Forms.ImeMode.HangulFull;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "sceneAdd";
            this.Text = "TsSceneAdd";
            this.Load += new System.EventHandler(this.TsSceneAdd_Load);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNum;
        private System.Windows.Forms.Label lbNum;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.ComboBox cbs1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbs2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbs3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbs4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox txtName;
        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnDecid;
        private System.Windows.Forms.Label label6;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
    }
}