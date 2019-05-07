using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace eNet编辑器.AddForm
{
    public delegate void AddGW();
    public partial class tnGateway : Form
    {
        public tnGateway()
        {
            InitializeComponent();
        }
        //回调添加网关节点
        public event AddGW addgw;

        public static event Action<string> AppTxtShow;

        public  bool isNew = false;
        //修改时候旧的IP和版本号
        private string oldinfos = "";
        private void btnDecid_Click(object sender, EventArgs e)
        {
            bool isip = Regex.IsMatch(txtGateway.Text, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
            if (!isip)
            {

                AppTxtShow("IP地址输入格式错误");
                return;
            }
          
            //判断该IP是否已经存在
            if (FileMesege.DeviceList == null)
            {
                FileMesege.DeviceList = new List<DataJson.Device>();
            }
            foreach (DataJson.Device dev in FileMesege.DeviceList)
            {
                if (dev.ip == txtGateway.Text)
                {
                    AppTxtShow("修改失败！IP地址已存在");
                    return;
                }
            }
            if (isNew)
            {
                FileMesege.info = string.Format("{0} {1}", txtGateway.Text, cbVersion.Text);
                //回调函数
                addgw();
            }
            else
            {
                FileMesege.info = string.Format("{0} {1} {2}", txtGateway.Text, cbVersion.Text,oldinfos);
                this.DialogResult = DialogResult.OK;
                   
            }
              
               
         
            
        }

 

        private void tnGateway_Load(object sender, EventArgs e)
        {
            if (FileMesege.tnselectNode !=null &&  FileMesege.tnselectNode.Parent == null)
            { 
                string [] ips = FileMesege.tnselectNode.Text.Split(' ');
                txtGateway.Text = ips[0];
            }
            if (!isNew)
            { 
                //修改
                lbTitle.Text = "修改网关";
                oldinfos = string.Format("{0} {1}", txtGateway.Text, cbVersion.Text);
            }
        }

   



    }
}
