namespace eNet编辑器.AddForm
{
    partial class timerHHMM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(timerHHMM));
            this.tmSelect = new DevComponents.Editors.DateTimeAdv.TimeSelector();
            this.btnSunUP = new DevComponents.DotNetBar.ButtonX();
            this.btnSunOut = new DevComponents.DotNetBar.ButtonX();
            this.cbMin = new System.Windows.Forms.CheckBox();
            this.cbHour = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tmSelect
            // 
            this.tmSelect.AutoSize = true;
            // 
            // 
            // 
            this.tmSelect.BackgroundStyle.BackColor = System.Drawing.Color.White;
            this.tmSelect.BackgroundStyle.Class = "DataGridViewBorder";
            this.tmSelect.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.tmSelect.ContainerControlProcessDialogKey = true;
            this.tmSelect.Location = new System.Drawing.Point(1, 4);
            this.tmSelect.Name = "tmSelect";
            this.tmSelect.OkButtonVisible = false;
            this.tmSelect.SelectorType = DevComponents.Editors.DateTimeAdv.eTimeSelectorType.MonthCalendarStyle;
            this.tmSelect.Size = new System.Drawing.Size(187, 163);
            this.tmSelect.TimeFormat = DevComponents.Editors.DateTimeAdv.eTimeSelectorFormat.Time24H;
            this.tmSelect.SelectedTimeChanged += new System.EventHandler(this.tmSelect_SelectedTimeChanged);
            this.tmSelect.OkClick += new System.EventHandler(this.timeSelector1_OkClick);
            // 
            // btnSunUP
            // 
            this.btnSunUP.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSunUP.BackColor = System.Drawing.Color.White;
            this.btnSunUP.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnSunUP.FocusCuesEnabled = false;
            this.btnSunUP.Font = new System.Drawing.Font("黑体", 9F);
            this.btnSunUP.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnSunUP.Image = ((System.Drawing.Image)(resources.GetObject("btnSunUP.Image")));
            this.btnSunUP.Location = new System.Drawing.Point(90, 174);
            this.btnSunUP.Margin = new System.Windows.Forms.Padding(6);
            this.btnSunUP.Name = "btnSunUP";
            this.btnSunUP.Size = new System.Drawing.Size(32, 32);
            this.btnSunUP.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSunUP.TabIndex = 19;
            this.btnSunUP.Tooltip = "日出时间";
            this.btnSunUP.Click += new System.EventHandler(this.btnSunUP_Click);
            // 
            // btnSunOut
            // 
            this.btnSunOut.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSunOut.BackColor = System.Drawing.Color.White;
            this.btnSunOut.ColorTable = DevComponents.DotNetBar.eButtonColor.Blue;
            this.btnSunOut.FocusCuesEnabled = false;
            this.btnSunOut.Font = new System.Drawing.Font("黑体", 9F);
            this.btnSunOut.HotTrackingStyle = DevComponents.DotNetBar.eHotTrackingStyle.None;
            this.btnSunOut.Image = ((System.Drawing.Image)(resources.GetObject("btnSunOut.Image")));
            this.btnSunOut.Location = new System.Drawing.Point(134, 174);
            this.btnSunOut.Margin = new System.Windows.Forms.Padding(6);
            this.btnSunOut.Name = "btnSunOut";
            this.btnSunOut.Size = new System.Drawing.Size(32, 32);
            this.btnSunOut.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSunOut.TabIndex = 20;
            this.btnSunOut.Tooltip = "日落时间";
            this.btnSunOut.Click += new System.EventHandler(this.btnSunOut_Click);
            // 
            // cbMin
            // 
            this.cbMin.AutoSize = true;
            this.cbMin.Location = new System.Drawing.Point(29, 194);
            this.cbMin.Name = "cbMin";
            this.cbMin.Size = new System.Drawing.Size(48, 16);
            this.cbMin.TabIndex = 22;
            this.cbMin.Text = "每分";
            this.cbMin.UseVisualStyleBackColor = true;
            this.cbMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cbMin_MouseUp);
            // 
            // cbHour
            // 
            this.cbHour.AutoSize = true;
            this.cbHour.Location = new System.Drawing.Point(29, 172);
            this.cbHour.Name = "cbHour";
            this.cbHour.Size = new System.Drawing.Size(48, 16);
            this.cbHour.TabIndex = 21;
            this.cbHour.Text = "每时";
            this.cbHour.UseVisualStyleBackColor = true;
            this.cbHour.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cbHour_MouseUp);
            // 
            // timerHHMM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(190, 215);
            this.Controls.Add(this.cbMin);
            this.Controls.Add(this.cbHour);
            this.Controls.Add(this.btnSunOut);
            this.Controls.Add(this.btnSunUP);
            this.Controls.Add(this.tmSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "timerHHMM";
            this.Deactivate += new System.EventHandler(this.timerHHMM_Deactivate);
            this.Load += new System.EventHandler(this.timerHHMM_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.timerHHMM_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.Editors.DateTimeAdv.TimeSelector tmSelect;
        private DevComponents.DotNetBar.ButtonX btnSunUP;
        private DevComponents.DotNetBar.ButtonX btnSunOut;
        private System.Windows.Forms.CheckBox cbMin;
        private System.Windows.Forms.CheckBox cbHour;

    }
}