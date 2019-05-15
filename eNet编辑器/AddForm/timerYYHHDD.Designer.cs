namespace eNet编辑器.AddForm
{
    partial class timerYYHHDD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(timerYYHHDD));
            this.cbYear = new System.Windows.Forms.CheckBox();
            this.cbMonth = new System.Windows.Forms.CheckBox();
            this.Calendar = new DevComponents.Editors.DateTimeAdv.MonthCalendarAdv();
            this.btnAdd = new DevComponents.DotNetBar.ButtonX();
            this.SuspendLayout();
            // 
            // cbYear
            // 
            this.cbYear.AutoSize = true;
            this.cbYear.Location = new System.Drawing.Point(12, 213);
            this.cbYear.Name = "cbYear";
            this.cbYear.Size = new System.Drawing.Size(63, 21);
            this.cbYear.TabIndex = 1;
            this.cbYear.Text = "忽略年";
            this.cbYear.UseVisualStyleBackColor = true;
            // 
            // cbMonth
            // 
            this.cbMonth.AutoSize = true;
            this.cbMonth.Location = new System.Drawing.Point(81, 213);
            this.cbMonth.Name = "cbMonth";
            this.cbMonth.Size = new System.Drawing.Size(63, 21);
            this.cbMonth.TabIndex = 2;
            this.cbMonth.Text = "忽略月";
            this.cbMonth.UseVisualStyleBackColor = true;
            // 
            // Calendar
            // 
            this.Calendar.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.Calendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.Calendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.Calendar.ContainerControlProcessDialogKey = true;
            this.Calendar.DaySize = new System.Drawing.Size(24, 24);
            this.Calendar.DisplayMonth = new System.DateTime(2007, 10, 1, 0, 0, 0, 0);
            this.Calendar.Location = new System.Drawing.Point(8, 7);
            this.Calendar.Name = "Calendar";
            // 
            // 
            // 
            this.Calendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.Calendar.Size = new System.Drawing.Size(172, 192);
            this.Calendar.TabIndex = 16;
            this.Calendar.Text = "monthCalendarAdv1";
            this.Calendar.ItemDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Calendar_ItemDoubleClick);
            // 
            // btnAdd
            // 
            this.btnAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnAdd.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnAdd.FocusCuesEnabled = false;
            this.btnAdd.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnAdd.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnAdd.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnAdd.HoverImage")));
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Location = new System.Drawing.Point(148, 213);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 20);
            this.btnAdd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAdd.TabIndex = 17;
            this.btnAdd.Tooltip = "添加日期";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // timerYYHHDD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(188, 246);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.Calendar);
            this.Controls.Add(this.cbMonth);
            this.Controls.Add(this.cbYear);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "timerYYHHDD";
            this.Text = "timerYYHHDD";
            this.Deactivate += new System.EventHandler(this.timerYYHHDD_Deactivate);
            this.Load += new System.EventHandler(this.timerYYHHDD_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.timerYYHHDD_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbYear;
        private System.Windows.Forms.CheckBox cbMonth;
        private DevComponents.Editors.DateTimeAdv.MonthCalendarAdv Calendar;
        private DevComponents.DotNetBar.ButtonX btnAdd;
    }
}