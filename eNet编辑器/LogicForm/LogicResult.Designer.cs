namespace eNet编辑器.LogicForm
{
    partial class LogicResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogicResult));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnDecid = new DevComponents.DotNetBar.ButtonX();
            this.cbOperation = new System.Windows.Forms.ComboBox();
            this.txtLeft = new System.Windows.Forms.TextBox();
            this.txtRight = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.plInfoTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.plInfoTitle);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Margin = new System.Windows.Forms.Padding(6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(287, 146);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.txtRight);
            this.panel2.Controls.Add(this.txtLeft);
            this.panel2.Controls.Add(this.cbOperation);
            this.panel2.Controls.Add(this.btnDecid);
            this.panel2.Location = new System.Drawing.Point(0, 26);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(2);
            this.panel2.Size = new System.Drawing.Size(287, 120);
            this.panel2.TabIndex = 94;
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.lbTitle);
            this.plInfoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plInfoTitle.Location = new System.Drawing.Point(0, 0);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(2);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(287, 25);
            this.plInfoTitle.TabIndex = 93;
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
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
            this.btnClose.Location = new System.Drawing.Point(268, 6);
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
            this.lbTitle.Size = new System.Drawing.Size(32, 17);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "结果";
            this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
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
            this.btnDecid.Location = new System.Drawing.Point(250, 90);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(6);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(24, 24);
            this.btnDecid.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDecid.TabIndex = 5;
            this.btnDecid.Tooltip = "添加";
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // cbOperation
            // 
            this.cbOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOperation.FormattingEnabled = true;
            this.cbOperation.Location = new System.Drawing.Point(114, 38);
            this.cbOperation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbOperation.Name = "cbOperation";
            this.cbOperation.Size = new System.Drawing.Size(70, 25);
            this.cbOperation.TabIndex = 70;
            this.cbOperation.SelectedIndexChanged += new System.EventHandler(this.cbOperation_SelectedIndexChanged);
            // 
            // txtLeft
            // 
            this.txtLeft.Location = new System.Drawing.Point(42, 38);
            this.txtLeft.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLeft.Name = "txtLeft";
            this.txtLeft.Size = new System.Drawing.Size(52, 23);
            this.txtLeft.TabIndex = 71;
            this.txtLeft.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLeft_KeyPress);
            // 
            // txtRight
            // 
            this.txtRight.Location = new System.Drawing.Point(202, 38);
            this.txtRight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRight.Name = "txtRight";
            this.txtRight.Size = new System.Drawing.Size(52, 23);
            this.txtRight.TabIndex = 72;
            this.txtRight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRight_KeyPress);
            // 
            // LogicResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(290, 149);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LogicResult";
            this.Text = "LogicResult";
            this.Load += new System.EventHandler(this.LogicResult_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LogicResult_Paint);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnClose;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Panel panel2;
        private DevComponents.DotNetBar.ButtonX btnDecid;
        private System.Windows.Forms.ComboBox cbOperation;
        private System.Windows.Forms.TextBox txtRight;
        private System.Windows.Forms.TextBox txtLeft;
    }
}