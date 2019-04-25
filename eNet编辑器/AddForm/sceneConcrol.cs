using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace eNet编辑器.AddForm
{
    public partial class sceneConcrol : Form
    {
        public sceneConcrol()
        {
            InitializeComponent();
        }

        private string opt;

        public string Opt
        {
            get { return opt; }
            set { opt = value; }
        }

        private string ver;

        public string Ver
        {
            get { return ver; }
            set { ver = value; }
        }
    
        private string objType;

        public string ObjType
        {
            get { return objType; }
            set { objType = value; }
        }

        private DataJson.PointInfo point = null;

        internal DataJson.PointInfo Point
        {
            get { return point; }
            set { point = value; }
        }

        /// <summary>
        /// 存放type.ini里面的commod信息
        /// </summary>
        private List<data> comdsList = new List<data>();
        class data
        {
            public string[] list { get; set; }
        }

        private void sceneConcrol_Load(object sender, EventArgs e)
        {
             //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type= "";
            //bool Flag = false;
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                type = IniConfig.GetValue(file.FullName, "define", "name");
                //找到类型一致
                if (type == objType)
                {
                    string tmp = "";
                    //循环读取command 1= 2= 3= 信息
                    for (int i = 1; i < 30; i++)
                    {
                        tmp = IniConfig.GetValue(file.FullName, "command", i.ToString());
                        if (tmp == "")
                        {
                            break;
                        }
                        data a = new data();

                        a.list = tmp.Split(',');

                        //把命令存放到信息
                        comdsList.Add(a);
                    }
                    //2.1 cb1 加载 type.ini信息的头一个信息 
                    foreach (data comd in comdsList)
                    {
                        cbVersion.Items.Add(comd.list[0]);
                    }
                    //默认加载cbversion第一个选项信息
                    cbVersion.SelectedIndex = 0;
                    cbAndlb(0);
                    break;
                }//类型一致
            }
        }

        /// <summary>
        /// 当Version选项改变时 lb信息 和cb信息更新
        /// </summary>
        /// <param name="indexs">version索引</param>
        private void cbAndlb(int indexs)
        {
            //2.2 cb2-3 lb1-4按读取分割的顺序添加 信息 
            lb1.Text = comdsList[indexs].list[1];
            lb2.Text = comdsList[indexs].list[3];
            lb3.Text = comdsList[indexs].list[5];
            lb4.Text = comdsList[indexs].list[7];
            //2.3 cb2-3 的内容需要判断 1-100 表示有1-100个内容选择 enable打开
            cbTextChange(cb1, comdsList[indexs].list[2]);
            cbTextChange(cb2, comdsList[indexs].list[4]);
            cbTextChange(cb3, comdsList[indexs].list[6]);
            cbTextChange(cb4, comdsList[indexs].list[8]);
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
            else if (info == "getObj")
            {
                if (point != null && point.objType != "" && point.value != "")
                {
                    string filepath = string.Format("{0}//objs//{1}.ini", Application.StartupPath, point.objType);
                    List<string> keys = IniConfig.ReadKeys(point.value, filepath);
                    for (int i = 2; i < keys.Count; i++)
                    {
                        cb.Items.Add(string.Format("{0} {1}", keys[i], IniConfig.GetValue(filepath, point.value, keys[i])));
                    }
                    cb.Enabled = true;
                }
                else
                {
                    cb.Items.Add("点位：对象和参数未赋值 ");

                    cb.Enabled = false;
                }

            }
            else
            {
                cb.Items.Add(info);
                cb.Enabled = false;
            }
            //初始化索引
            cb.SelectedIndex = 0;
         
        }

        private void btnAffirm_Click(object sender, EventArgs e)
        {
            //Socket sk = new Socket();
            try
            {
                string cb4Num = Regex.Replace(cb4.Text, @"[^\d]*", "");
                if (string.IsNullOrEmpty(cb4Num))
                {
                    return;
                }
                this.opt = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr(cb2.Text) + SocketUtil.strtohexstr(cb3.Text) + SocketUtil.strtohexstr(cb4Num);
                if (cb4.Text.Split(' ').Length >1)
                {
                    this.ver = cb4.Text.Split(' ')[1];
                }
                else
                {
                    this.ver = cbVersion.Text;
                }
                
                
            }
            catch
            {
                this.opt = "";
                this.ver = "";
            }
            finally
            {

                this.DialogResult = DialogResult.OK;
            }
            
            
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbAndlb(cbVersion.SelectedIndex);
        }

      
    }
}
