namespace eNet编辑器.AddForm
{
    partial class TsSceneAdd
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
            this.txtGateway = new System.Windows.Forms.TextBox();
            this.btnCancle = new System.Windows.Forms.Button();
            this.btnDecid = new System.Windows.Forms.Button();
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
            this.SuspendLayout();
            // 
            // txtGateway
            // 
            this.txtGateway.Enabled = false;
            this.txtGateway.Location = new System.Drawing.Point(76, 18);
            this.txtGateway.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtGateway.Name = "txtGateway";
            this.txtGateway.Size = new System.Drawing.Size(191, 23);
            this.txtGateway.TabIndex = 63;
            this.txtGateway.Text = "192.168.1.230";
            // 
            // btnCancle
            // 
            this.btnCancle.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancle.Location = new System.Drawing.Point(169, 317);
            this.btnCancle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(94, 38);
            this.btnCancle.TabIndex = 62;
            this.btnCancle.Text = "返回";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // btnDecid
            // 
            this.btnDecid.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDecid.Location = new System.Drawing.Point(15, 317);
            this.btnDecid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(94, 38);
            this.btnDecid.TabIndex = 59;
            this.btnDecid.Text = "添加";
            this.btnDecid.UseVisualStyleBackColor = true;
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 57;
            this.label1.Text = "网关：";
            // 
            // txtNum
            // 
            this.txtNum.Location = new System.Drawing.Point(76, 64);
            this.txtNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNum.Name = "txtNum";
            this.txtNum.Size = new System.Drawing.Size(191, 23);
            this.txtNum.TabIndex = 65;
            // 
            // lbNum
            // 
            this.lbNum.AutoSize = true;
            this.lbNum.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbNum.Location = new System.Drawing.Point(12, 64);
            this.lbNum.Name = "lbNum";
            this.lbNum.Size = new System.Drawing.Size(63, 14);
            this.lbNum.TabIndex = 64;
            this.lbNum.Text = "场景号：";
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbName.Location = new System.Drawing.Point(12, 273);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(49, 14);
            this.lbName.TabIndex = 66;
            this.lbName.Text = "名称：";
            // 
            // cbs1
            // 
            this.cbs1.Enabled = false;
            this.cbs1.FormattingEnabled = true;
            this.cbs1.Location = new System.Drawing.Point(76, 105);
            this.cbs1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs1.Name = "cbs1";
            this.cbs1.Size = new System.Drawing.Size(191, 25);
            this.cbs1.TabIndex = 69;
            this.cbs1.SelectedIndexChanged += new System.EventHandler(this.cbs1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 14);
            this.label2.TabIndex = 68;
            this.label2.Text = "区域1：";
            // 
            // cbs2
            // 
            this.cbs2.Enabled = false;
            this.cbs2.FormattingEnabled = true;
            this.cbs2.Location = new System.Drawing.Point(76, 147);
            this.cbs2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs2.Name = "cbs2";
            this.cbs2.Size = new System.Drawing.Size(191, 25);
            this.cbs2.TabIndex = 71;
            this.cbs2.SelectedIndexChanged += new System.EventHandler(this.cbs2_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(12, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 14);
            this.label3.TabIndex = 70;
            this.label3.Text = "区域2：";
            // 
            // cbs3
            // 
            this.cbs3.Enabled = false;
            this.cbs3.FormattingEnabled = true;
            this.cbs3.Location = new System.Drawing.Point(76, 186);
            this.cbs3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs3.Name = "cbs3";
            this.cbs3.Size = new System.Drawing.Size(191, 25);
            this.cbs3.TabIndex = 73;
            this.cbs3.SelectedIndexChanged += new System.EventHandler(this.cbs3_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(12, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 14);
            this.label4.TabIndex = 72;
            this.label4.Text = "区域3：";
            // 
            // cbs4
            // 
            this.cbs4.Enabled = false;
            this.cbs4.FormattingEnabled = true;
            this.cbs4.Location = new System.Drawing.Point(76, 228);
            this.cbs4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbs4.Name = "cbs4";
            this.cbs4.Size = new System.Drawing.Size(191, 25);
            this.cbs4.TabIndex = 75;
            this.cbs4.SelectedIndexChanged += new System.EventHandler(this.cbs4_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(12, 228);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 14);
            this.label5.TabIndex = 74;
            this.label5.Text = "区域4：";
            // 
            // txtName
            // 
            this.txtName.FormattingEnabled = true;
            this.txtName.Location = new System.Drawing.Point(76, 272);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(191, 25);
            this.txtName.TabIndex = 76;
            // 
            // TsSceneAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 373);
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
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnDecid);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ImeMode = System.Windows.Forms.ImeMode.HangulFull;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TsSceneAdd";
            this.Text = "TsSceneAdd";
            this.Load += new System.EventHandler(this.TsSceneAdd_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGateway;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Button btnDecid;
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
    }
}