namespace eNet编辑器.ThreeView
{
    partial class ThreeBind
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreeBind));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imgLIst = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.Gainsboro;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imgLIst;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(223, 490);
            this.treeView1.TabIndex = 1;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // imgLIst
            // 
            this.imgLIst.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLIst.ImageStream")));
            this.imgLIst.TransparentColor = System.Drawing.Color.White;
            this.imgLIst.Images.SetKeyName(0, "0.ico");
            this.imgLIst.Images.SetKeyName(1, "1.ico");
            this.imgLIst.Images.SetKeyName(2, "2.ico");
            this.imgLIst.Images.SetKeyName(3, "offbook.ico");
            this.imgLIst.Images.SetKeyName(4, "3.16.ico");
            // 
            // ThreeBind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 490);
            this.Controls.Add(this.treeView1);
            this.Name = "ThreeBind";
            this.Text = "ThreeBind";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imgLIst;
    }
}