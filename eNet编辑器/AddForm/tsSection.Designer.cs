namespace eNet编辑器.AddForm
{
    partial class tsSection
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
            this.btnCancle = new System.Windows.Forms.Button();
            this.btnDecid = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbname = new System.Windows.Forms.ComboBox();
            this.cbtype = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancle
            // 
            this.btnCancle.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancle.Location = new System.Drawing.Point(134, 100);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(81, 27);
            this.btnCancle.TabIndex = 62;
            this.btnCancle.Text = "返回";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // btnDecid
            // 
            this.btnDecid.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDecid.Location = new System.Drawing.Point(27, 100);
            this.btnDecid.Name = "btnDecid";
            this.btnDecid.Size = new System.Drawing.Size(81, 27);
            this.btnDecid.TabIndex = 59;
            this.btnDecid.Text = "添加";
            this.btnDecid.UseVisualStyleBackColor = true;
            this.btnDecid.Click += new System.EventHandler(this.btnDecid_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(13, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 14);
            this.label1.TabIndex = 57;
            this.label1.Text = "节点名称：";
            // 
            // cbname
            // 
            this.cbname.FormattingEnabled = true;
            this.cbname.Location = new System.Drawing.Point(96, 61);
            this.cbname.Name = "cbname";
            this.cbname.Size = new System.Drawing.Size(131, 20);
            this.cbname.TabIndex = 63;
            // 
            // cbtype
            // 
            this.cbtype.FormattingEnabled = true;
            this.cbtype.Location = new System.Drawing.Point(96, 26);
            this.cbtype.Name = "cbtype";
            this.cbtype.Size = new System.Drawing.Size(131, 20);
            this.cbtype.TabIndex = 65;
            this.cbtype.SelectedIndexChanged += new System.EventHandler(this.cbtype_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("幼圆", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(13, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 14);
            this.label2.TabIndex = 64;
            this.label2.Text = "节点类型：";
            // 
            // tsSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 139);
            this.Controls.Add(this.cbtype);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbname);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnDecid);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.ImeMode = System.Windows.Forms.ImeMode.HangulFull;
            this.Name = "tsSection";
            this.Text = "New Nodes";
            this.Load += new System.EventHandler(this.tsSection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Button btnDecid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbname;
        private System.Windows.Forms.ComboBox cbtype;
        private System.Windows.Forms.Label label2;

    }
}