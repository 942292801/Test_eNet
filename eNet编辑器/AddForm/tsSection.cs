using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.AddForm
{
    public delegate void AddNode();
    public partial class tsSection : Form
    {
        public tsSection()
        {
            InitializeComponent();
        }

        //记录上次关闭的选中点
        private int selectindex = 0;

        public int Selectindex
        {
            get { return selectindex; }
            set { selectindex = value; }
        }
        /// <summary>
        /// 新建第一个节点标志
        /// </summary>
        private bool newflag = false;

        public bool Newflag
        {
            get { return newflag; }
            set { newflag = value; }
        }

        //回调添加节点
        public event AddNode addNode;
        private void tsSection_Load(object sender, EventArgs e)
        {
            addtype(cbtype);
            cbtype.SelectedIndex = selectindex;
           
        }

        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (cbname.Text == "" || cbname.Text == null)
            {
                MessageBox.Show("新建节点名称不能为空");
                return;
            }
            FileMesege.info = cbname.Text;
            try
            {
                cbname.SelectedIndex = cbname.SelectedIndex + 1;
            }
            catch { 
            
            }
            //利用回调机制 
            addNode();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cbtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            addname(cbname, cbtype.Text);
        }

        /// <summary>
        /// 获取Ini文件所有key值
        /// </summary>
        /// <param name="cbtype"></param>
        private void addtype(ComboBox cbtype) 
        {
            List<string> keys = IniConfig.ReadKeys("sectionNode", String.Format("{0}{1}", Application.StartupPath, "\\names\\commonName.ini"));
            cbtype.Items.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                cbtype.Items.Add(keys[i]);
            }
            cbtype.SelectedIndex = 0;
        }

        /// <summary>
        /// 获取key值类型下的所有名称
        /// </summary>
        /// <param name="cbname"></param>
        /// <param name="type"></param>
        private void addname(ComboBox cbname,string type)
        {
            string[] node = IniConfig.GetValue(String.Format("{0}{1}", Application.StartupPath, "\\names\\commonName.ini"), "sectionNode", type).Split(',');
            cbname.Items.Clear();
            cbname.Text = "";
            for (int i = 0; i < node.Length; i++)
            {
                cbname.Items.Add(node[i]);
            }
            cbname.SelectedIndex = 0;
        }

 

      
     
    }
}
