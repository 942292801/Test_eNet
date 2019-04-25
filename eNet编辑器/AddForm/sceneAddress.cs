using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace eNet编辑器.AddForm
{
    public partial class sceneAddress : Form
    {
        public sceneAddress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 场景选中 hex信息地址
        /// </summary>
        private string obj;

        public string Obj
        {
            get { return obj; }
            set { obj = value; }
        }

        private string objType;

        /// <summary>
        /// 场景的对象类型
        /// </summary>
        public string ObjType
        {
            get { return objType; }
            set { objType = value; }
        }

        /// <summary>
        /// 存放当前dev表中读取到的 ID+设备
        /// </summary>
        List<string> devName = null;

        private void sceneAddress_Load(object sender, EventArgs e)
        {
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type= "";
            bool Flag = false;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                //找到类型一致
                if (type == objType)
                {
                    Label[] lbs = { lb1,lb2,lb3,lb4};
                    ComboBox[] cbs = { cb1,cb2,cb3,cb4};
                    //读取【address】1-4信息
                    for (int i = 0; i < 4; i++)
                    {
                        string [] infos =  IniConfig.GetValue(file.FullName, "address", (i+1).ToString()).Split(',');
                        if (infos.Length == 2)
                        {
                            lbs[i].Text = infos[0];
                            cbTextChange(cbs[i],infos[1]);
                        }
                        else
                        {
                            lbs[i].Text = "";
                            //cbs[i].Text = "";
                            cbs[i].Enabled = false;
                            Flag = true;
                        }
                    }
                    try
                    {
                        if (IniConfig.GetValue(file.FullName, "address", "2").Split(',')[1] == "0")
                        {
                            devName = new List<string>();
                            string ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
                            cb3.Items.Clear();
                            foreach (DataJson.Device devip in FileMesege.DeviceList)
                            {
                                if (devip.ip == ip)
                                {
                                    foreach (DataJson.Module md in devip.module)
                                    {
                                        devName.Add(md.id +" "+ md.device);
                                        cb3.Items.Add(md.id);
                                    }
                                    break;
                                }
                            }
                        }
                    }catch
                    {
                    
                    }
                   
                    break; 
                }
                
            }
            cb1.SelectedIndex = 0;
            cb2.SelectedIndex = 0;
            if (obj != "" && obj != null)
            {
                string[] infos = obj.Split('.');
                if (Flag)
                {
                    cb1.Text = infos[0];
                    //cb2.Text = infos[1];
                    //场景号cb3 为空
                    cb4.Text = (Convert.ToInt32(infos[2]) * 256 + Convert.ToInt32(infos[3])).ToString();
                }
                else
                {
                    cb1.Text = infos[0];
                    //cb2.Text = infos[1];
                    cb3.Text = infos[2];
                    cb4.Text = infos[3];
                    findPort(infos[2]);
                }
                   
            }
            
            
        }

        public void findPort(string id)
        {
            for (int i = 0; i < devName.Count; i++)
            {
                if (devName[i].Split(' ')[0] == id)
                {
                    string filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, devName[i].Split(' ')[1]); 
                    //获取全部Section下的Key
                    List<string> list = IniConfig.ReadKeys("ports", filepath);
                    cb4.Items.Clear();
                    //循环添加行信息
                    for (int j = 0; j < list.Count; j++)
                    {

                        cb4.Items.Add(list[j]);
                    }
                    break;
                }
            }
            
        }

        /// <summary>
        /// cb信息内容的判断1-9 或 1,2,3  或数字
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void cbTextChange(ComboBox cb, string info)
        {
            cb.Items.Clear();
            if (info.Contains("-"))
            {
                string[] infos = info.Split('-');
                int j = Convert.ToInt32(infos[1]);
                for (int i = Convert.ToInt32(infos[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString());
                }
                cb.Enabled = true;
            }
            else
            {
                cb.Items.Add(info);
                cb.Enabled = false;
            }
           
        }

        private void btnAffirm_Click(object sender, EventArgs e)
        {
            string newobj = "";
            if (cb1.Text != "" && cb2.Text!="")
            {
                newobj = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr(cb2.Text);
                if (cb3.Text != "" && cb4.Text != "")
                {
                    newobj = newobj + SocketUtil.strtohexstr(cb3.Text) + SocketUtil.strtohexstr(cb4.Text);
                }
                else if (cb3.Text == "" && cb4.Text != "")
                {
                    string tmp = SocketUtil.strtohexstr(cb4.Text);
                    while (tmp.Length < 4)
                    {
                        tmp =tmp.Insert(0, "0");
                    }
                    newobj = newobj + tmp;
                }
            }
            if (newobj.Length == 8)
            {
                this.obj = newobj;
                this.DialogResult = DialogResult.OK;
            }
            this.DialogResult = DialogResult.OK;
            
            
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

   

        private void cb3_TextChanged(object sender, EventArgs e)
        {
            findPort(cb3.Text);
        }


        


    }
}
