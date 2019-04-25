namespace eNet编辑器.DgvView
{
    partial class DgvPoint
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMultiple = new DevComponents.DotNetBar.ButtonX();
            this.btnAdd = new DevComponents.DotNetBar.ButtonX();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pointNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pointAdd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pointSection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pointName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pointDel = new System.Windows.Forms.DataGridViewButtonColumn();
            this.pointMultiple = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.doubleClickTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.Controls.Add(this.btnMultiple);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(864, 31);
            this.panel1.TabIndex = 1;
            // 
            // btnMultiple
            // 
            this.btnMultiple.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnMultiple.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.btnMultiple.Location = new System.Drawing.Point(57, 1);
            this.btnMultiple.Margin = new System.Windows.Forms.Padding(0);
            this.btnMultiple.Name = "btnMultiple";
            this.btnMultiple.Size = new System.Drawing.Size(57, 30);
            this.btnMultiple.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.btnMultiple.TabIndex = 13;
            this.btnMultiple.Text = "合并";
            this.btnMultiple.Click += new System.EventHandler(this.btnMultiple_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.MagentaWithBackground;
            this.btnAdd.Location = new System.Drawing.Point(0, 1);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(57, 30);
            this.btnAdd.Style = DevComponents.DotNetBar.eDotNetBarStyle.VS2005;
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "增加";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(391, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "点位编辑";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.Color.Lavender;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.pointNum,
            this.pointAdd,
            this.pointSection,
            this.pointName,
            this.pointDel,
            this.pointMultiple});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(-2, 36);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(864, 606);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
            // 
            // pointNum
            // 
            this.pointNum.HeaderText = "序号";
            this.pointNum.Name = "pointNum";
            this.pointNum.ReadOnly = true;
            this.pointNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.pointNum.Width = 60;
            // 
            // pointAdd
            // 
            this.pointAdd.HeaderText = "地址";
            this.pointAdd.Name = "pointAdd";
            this.pointAdd.ReadOnly = true;
            this.pointAdd.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // pointSection
            // 
            this.pointSection.HeaderText = "区域";
            this.pointSection.Name = "pointSection";
            this.pointSection.ReadOnly = true;
            this.pointSection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.pointSection.Width = 160;
            // 
            // pointName
            // 
            this.pointName.HeaderText = "名称";
            this.pointName.Name = "pointName";
            this.pointName.ReadOnly = true;
            this.pointName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.pointName.Width = 80;
            // 
            // pointDel
            // 
            this.pointDel.HeaderText = "删除";
            this.pointDel.Name = "pointDel";
            this.pointDel.Width = 55;
            // 
            // pointMultiple
            // 
            this.pointMultiple.HeaderText = "";
            this.pointMultiple.Name = "pointMultiple";
            this.pointMultiple.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.pointMultiple.Width = 50;
            // 
            // doubleClickTimer
            // 
            this.doubleClickTimer.Interval = 40;
            this.doubleClickTimer.Tick += new System.EventHandler(this.doubleClickTimer_Tick);
            // 
            // DgvPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(862, 642);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "DgvPoint";
            this.Text = "DgvPoint";
            this.Load += new System.EventHandler(this.DgvPoint_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private DevComponents.DotNetBar.ButtonX btnAdd;
        private System.Windows.Forms.Timer doubleClickTimer;
        private DevComponents.DotNetBar.ButtonX btnMultiple;
        private System.Windows.Forms.DataGridViewTextBoxColumn pointNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn pointAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn pointSection;
        private System.Windows.Forms.DataGridViewTextBoxColumn pointName;
        private System.Windows.Forms.DataGridViewButtonColumn pointDel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn pointMultiple;
    }
}