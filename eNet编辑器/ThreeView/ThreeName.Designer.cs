namespace eNet编辑器.ThreeView
{
    partial class ThreeName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreeName));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.新建ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.修改ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.展开所有节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.收起所有节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imgLIst = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.修改ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.plInfoTitle = new System.Windows.Forms.Panel();
            this.btnAddGw = new DevComponents.DotNetBar.ButtonX();
            this.btnAddDev = new DevComponents.DotNetBar.ButtonX();
            this.btnDel = new DevComponents.DotNetBar.ButtonX();
            this.symbolBox1 = new DevComponents.DotNetBar.Controls.SymbolBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.progressBarX1 = new DevComponents.DotNetBar.Controls.ProgressBarX();
            this.node30 = new DevComponents.AdvTree.Node();
            this.node28 = new DevComponents.AdvTree.Node();
            this.node26 = new DevComponents.AdvTree.Node();
            this.node25 = new DevComponents.AdvTree.Node();
            this.node24 = new DevComponents.AdvTree.Node();
            this.cell18 = new DevComponents.AdvTree.Cell();
            this.node22 = new DevComponents.AdvTree.Node();
            this.node21 = new DevComponents.AdvTree.Node();
            this.node20 = new DevComponents.AdvTree.Node();
            this.node18 = new DevComponents.AdvTree.Node();
            this.cell17 = new DevComponents.AdvTree.Cell();
            this.node17 = new DevComponents.AdvTree.Node();
            this.node16 = new DevComponents.AdvTree.Node();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.plInfoTitle.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imgLIst;
            this.treeView1.ItemHeight = 24;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.treeView1.SelectedImageIndex = 2;
            this.treeView1.Size = new System.Drawing.Size(281, 381);
            this.treeView1.TabIndex = 0;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建ToolStripMenuItem,
            this.修改ToolStripMenuItem,
            this.删除ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.展开所有节点ToolStripMenuItem,
            this.收起所有节点ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 120);
            // 
            // 新建ToolStripMenuItem
            // 
            this.新建ToolStripMenuItem.Name = "新建ToolStripMenuItem";
            this.新建ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.新建ToolStripMenuItem.Text = "添加";
            this.新建ToolStripMenuItem.Click += new System.EventHandler(this.新建ToolStripMenuItem_Click);
            // 
            // 修改ToolStripMenuItem
            // 
            this.修改ToolStripMenuItem.Name = "修改ToolStripMenuItem";
            this.修改ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.修改ToolStripMenuItem.Text = "修改";
            this.修改ToolStripMenuItem.Click += new System.EventHandler(this.修改ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
            // 
            // 展开所有节点ToolStripMenuItem
            // 
            this.展开所有节点ToolStripMenuItem.Name = "展开所有节点ToolStripMenuItem";
            this.展开所有节点ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.展开所有节点ToolStripMenuItem.Text = "展开所有节点";
            this.展开所有节点ToolStripMenuItem.Click += new System.EventHandler(this.展开所有节点ToolStripMenuItem_Click);
            // 
            // 收起所有节点ToolStripMenuItem
            // 
            this.收起所有节点ToolStripMenuItem.Name = "收起所有节点ToolStripMenuItem";
            this.收起所有节点ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.收起所有节点ToolStripMenuItem.Text = "收起所有节点";
            this.收起所有节点ToolStripMenuItem.Click += new System.EventHandler(this.收起所有节点ToolStripMenuItem_Click);
            // 
            // imgLIst
            // 
            this.imgLIst.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLIst.ImageStream")));
            this.imgLIst.TransparentColor = System.Drawing.Color.White;
            this.imgLIst.Images.SetKeyName(0, "u74.png");
            this.imgLIst.Images.SetKeyName(1, "u70.png");
            this.imgLIst.Images.SetKeyName(2, "u73.png");
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.修改ToolStripMenuItem1,
            this.删除ToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(101, 48);
            // 
            // 修改ToolStripMenuItem1
            // 
            this.修改ToolStripMenuItem1.Name = "修改ToolStripMenuItem1";
            this.修改ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.修改ToolStripMenuItem1.Text = "修改";
            this.修改ToolStripMenuItem1.Click += new System.EventHandler(this.修改ToolStripMenuItem1_Click);
            // 
            // 删除ToolStripMenuItem1
            // 
            this.删除ToolStripMenuItem1.Name = "删除ToolStripMenuItem1";
            this.删除ToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.删除ToolStripMenuItem1.Text = "删除";
            this.删除ToolStripMenuItem1.Click += new System.EventHandler(this.删除ToolStripMenuItem1_Click);
            // 
            // plInfoTitle
            // 
            this.plInfoTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.plInfoTitle.Controls.Add(this.btnAddGw);
            this.plInfoTitle.Controls.Add(this.btnAddDev);
            this.plInfoTitle.Controls.Add(this.btnDel);
            this.plInfoTitle.Controls.Add(this.symbolBox1);
            this.plInfoTitle.Controls.Add(this.label1);
            this.plInfoTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.plInfoTitle.Location = new System.Drawing.Point(0, 0);
            this.plInfoTitle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.plInfoTitle.Name = "plInfoTitle";
            this.plInfoTitle.Size = new System.Drawing.Size(281, 25);
            this.plInfoTitle.TabIndex = 2;
            // 
            // btnAddGw
            // 
            this.btnAddGw.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAddGw.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAddGw.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAddGw.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnAddGw.FocusCuesEnabled = false;
            this.btnAddGw.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnAddGw.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image;
            this.btnAddGw.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnAddGw.HoverImage")));
            this.btnAddGw.Image = ((System.Drawing.Image)(resources.GetObject("btnAddGw.Image")));
            this.btnAddGw.Location = new System.Drawing.Point(205, 5);
            this.btnAddGw.Margin = new System.Windows.Forms.Padding(6);
            this.btnAddGw.Name = "btnAddGw";
            this.btnAddGw.Size = new System.Drawing.Size(15, 15);
            this.btnAddGw.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAddGw.TabIndex = 5;
            this.btnAddGw.Tooltip = "新增网关";
            this.btnAddGw.Click += new System.EventHandler(this.btnAddGw_Click);
            // 
            // btnAddDev
            // 
            this.btnAddDev.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAddDev.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAddDev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAddDev.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnAddDev.FocusCuesEnabled = false;
            this.btnAddDev.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnAddDev.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image;
            this.btnAddDev.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnAddDev.HoverImage")));
            this.btnAddDev.Image = ((System.Drawing.Image)(resources.GetObject("btnAddDev.Image")));
            this.btnAddDev.Location = new System.Drawing.Point(232, 5);
            this.btnAddDev.Margin = new System.Windows.Forms.Padding(6);
            this.btnAddDev.Name = "btnAddDev";
            this.btnAddDev.Size = new System.Drawing.Size(15, 15);
            this.btnAddDev.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAddDev.TabIndex = 4;
            this.btnAddDev.Tooltip = "新建设备";
            this.btnAddDev.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDel
            // 
            this.btnDel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDel.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnDel.FocusCuesEnabled = false;
            this.btnDel.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnDel.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.Image;
            this.btnDel.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnDel.HoverImage")));
            this.btnDel.Image = ((System.Drawing.Image)(resources.GetObject("btnDel.Image")));
            this.btnDel.Location = new System.Drawing.Point(259, 5);
            this.btnDel.Margin = new System.Windows.Forms.Padding(6);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(15, 15);
            this.btnDel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDel.TabIndex = 3;
            this.btnDel.Tooltip = "删除";
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // symbolBox1
            // 
            this.symbolBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            // 
            // 
            // 
            this.symbolBox1.BackgroundStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("symbolBox1.BackgroundStyle.BackgroundImage")));
            this.symbolBox1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.symbolBox1.Location = new System.Drawing.Point(5, 5);
            this.symbolBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.symbolBox1.Name = "symbolBox1";
            this.symbolBox1.Size = new System.Drawing.Size(15, 15);
            this.symbolBox1.Symbol = "";
            this.symbolBox1.SymbolColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(119)))), ((int)(((byte)(119)))));
            this.symbolBox1.TabIndex = 2;
            this.symbolBox1.Text = "symbolBox1";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(27, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "导航";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(281, 413);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(281, 413);
            this.panel2.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.treeView1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 32);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(281, 381);
            this.panel4.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.plInfoTitle);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(281, 32);
            this.panel3.TabIndex = 3;
            // 
            // progressBarX1
            // 
            // 
            // 
            // 
            this.progressBarX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.progressBarX1.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;
            this.progressBarX1.Location = new System.Drawing.Point(58, 276);
            this.progressBarX1.Name = "progressBarX1";
            this.progressBarX1.Size = new System.Drawing.Size(140, 20);
            this.progressBarX1.TabIndex = 4;
            this.progressBarX1.Text = "progressBarX1";
            this.progressBarX1.Value = 63;
            // 
            // node30
            // 
            this.node30.Name = "node30";
            this.node30.Text = "Progress bar";
            // 
            // node28
            // 
            this.node28.Name = "node28";
            this.node28.Text = "DotNetBar <a href=\"textmarkup\">text-markup</a> is fully supported";
            // 
            // node26
            // 
            this.node26.CellPartLayout = DevComponents.AdvTree.eCellPartLayout.Vertical;
            this.node26.ImageAlignment = DevComponents.AdvTree.eCellPartAlignment.CenterTop;
            this.node26.Name = "node26";
            this.node26.Text = "Orientation";
            // 
            // node25
            // 
            this.node25.ImageAlignment = DevComponents.AdvTree.eCellPartAlignment.FarCenter;
            this.node25.Name = "node25";
            this.node25.Text = "Image/text alignment";
            // 
            // node24
            // 
            this.node24.Name = "node24";
            this.node24.Text = "Multiple images per node";
            // 
            // cell18
            // 
            this.cell18.Name = "cell18";
            this.cell18.StyleMouseOver = null;
            // 
            // node22
            // 
            this.node22.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.node22.CheckBoxVisible = true;
            this.node22.Name = "node22";
            this.node22.Text = "Option 3";
            // 
            // node21
            // 
            this.node21.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.node21.CheckBoxVisible = true;
            this.node21.Name = "node21";
            this.node21.Text = "Option 2";
            // 
            // node20
            // 
            this.node20.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.node20.CheckBoxVisible = true;
            this.node20.Name = "node20";
            this.node20.Text = "Option 1";
            // 
            // node18
            // 
            this.node18.CheckBoxVisible = true;
            this.node18.Name = "node18";
            this.node18.Text = "Option 3";
            // 
            // cell17
            // 
            this.cell17.CheckBoxVisible = true;
            this.cell17.Name = "cell17";
            this.cell17.StyleMouseOver = null;
            this.cell17.Text = "Option 3";
            // 
            // node17
            // 
            this.node17.CheckBoxVisible = true;
            this.node17.Name = "node17";
            this.node17.Text = "Option 2";
            // 
            // node16
            // 
            this.node16.CheckBoxThreeState = true;
            this.node16.CheckBoxVisible = true;
            this.node16.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.node16.Name = "node16";
            this.node16.Text = "Option 1 with 3-state";
            // 
            // ThreeName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 413);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ThreeName";
            this.Text = "ThreeName";
            this.Load += new System.EventHandler(this.ThreeName_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.plInfoTitle.ResumeLayout(false);
            this.plInfoTitle.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imgLIst;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 新建ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 修改ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 展开所有节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 收起所有节点ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 修改ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem1;
        private System.Windows.Forms.Panel plInfoTitle;
        private DevComponents.DotNetBar.Controls.SymbolBox symbolBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private DevComponents.DotNetBar.Controls.ProgressBarX progressBarX1;
        private DevComponents.AdvTree.Node node30;
        private DevComponents.AdvTree.Node node28;
        private DevComponents.AdvTree.Node node26;
        private DevComponents.AdvTree.Node node25;
        private DevComponents.AdvTree.Node node24;
        private DevComponents.AdvTree.Cell cell18;
        private DevComponents.AdvTree.Node node22;
        private DevComponents.AdvTree.Node node21;
        private DevComponents.AdvTree.Node node20;
        private DevComponents.AdvTree.Node node18;
        private DevComponents.AdvTree.Cell cell17;
        private DevComponents.AdvTree.Node node17;
        private DevComponents.AdvTree.Node node16;
        private DevComponents.DotNetBar.ButtonX btnAddDev;
        private DevComponents.DotNetBar.ButtonX btnDel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private DevComponents.DotNetBar.ButtonX btnAddGw;
    }
}