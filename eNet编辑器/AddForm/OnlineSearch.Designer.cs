namespace eNet编辑器.AddForm
{
    partial class OnlineSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlineSearch));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnImport = new DevComponents.DotNetBar.ButtonX();
            this.btnSearch = new DevComponents.DotNetBar.ButtonX();
            this.btnClose = new DevComponents.DotNetBar.ButtonX();
            this.panel3.SuspendLayout();
            this.plInfoTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.treeView1.HideSelection = false;
            this.treeView1.ItemHeight = 24;
            this.treeView1.Location = new System.Drawing.Point(0, 11);
            this.treeView1.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.treeView1.Size = new System.Drawing.Size(224, 427);
            this.treeView1.TabIndex = 0;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDoubleClick);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.treeView1);
            this.panel3.Location = new System.Drawing.Point(1, 27);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(224, 438);
            this.panel3.TabIndex = 3;
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnClose);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.label3);
            this.plInfoTitle.Controls.Add(this.btnImport);
            this.plInfoTitle.Controls.Add(this.btnSearch);
            this.plInfoTitle.Location = new System.Drawing.Point(1, 1);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(224, 25);
            this.plInfoTitle.TabIndex = 2;
            this.plInfoTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.plInfoTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(25, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "在线搜索";
            this.label3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseDown);
            this.label3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plInfoTitle_MouseMove);
            // 
            // btnImport
            // 
            this.btnImport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnImport.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnImport.FocusCuesEnabled = false;
            this.btnImport.Font = new System.Drawing.Font("黑体", 9F);
            this.btnImport.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnImport.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnImport.HoverImage")));
            this.btnImport.Image = ((System.Drawing.Image)(resources.GetObject("btnImport.Image")));
            this.btnImport.Location = new System.Drawing.Point(156, 5);
            this.btnImport.Margin = new System.Windows.Forms.Padding(6);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(15, 15);
            this.btnImport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnImport.TabIndex = 4;
            this.btnImport.Tooltip = "导入";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSearch.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnSearch.FocusCuesEnabled = false;
            this.btnSearch.Font = new System.Drawing.Font("黑体", 9F);
            this.btnSearch.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnSearch.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnSearch.HoverImage")));
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(183, 5);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(15, 15);
            this.btnSearch.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Tooltip = "搜索";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
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
            this.btnClose.Location = new System.Drawing.Point(205, 6);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(13, 13);
            this.btnClose.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnClose.TabIndex = 10;
            this.btnClose.Tooltip = "关闭";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // OnlineSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(227, 467);
            this.Controls.Add(this.plInfoTitle);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "OnlineSearch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnlineSearch_FormClosing);
            this.Load += new System.EventHandler(this.OnlineSearch_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnlineSearch_Paint);
            this.panel3.ResumeLayout(false);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.ButtonX btnImport;
        private DevComponents.DotNetBar.ButtonX btnSearch;
        private System.Windows.Forms.Label label3;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private DevComponents.DotNetBar.ButtonX btnClose;



    }
}