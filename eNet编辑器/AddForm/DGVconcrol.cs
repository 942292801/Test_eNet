using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace eNet编辑器.AddForm
{
    public partial class DGVconcrol : Form
    {
        public DGVconcrol()
        {
            InitializeComponent();
        }
        ClientAsync client = new ClientAsync();
        //private string type;
        /// <summary>
        /// 保存信息存回dgv 开关（0,0,0,1）
        /// </summary>
        private string dgvshow;
        public string Dgvshow
        {
            get { return dgvshow; }
            set { dgvshow = value; }
        }
        /// <summary>
        /// 存放type.ini里面的commod信息
        /// </summary>
        private List<data> comdsList =new List<data>();
        class data {
            public string[] list { get;set;}
        }
        /// <summary>
        /// 软件运行路径
        /// </summary>
        private string path = Application.StartupPath;
        /// <summary>
        /// 当前行数 0开始
        /// </summary>
        private int rowindex;
        public int Rowindex
        {
            get { return rowindex; }
            set { rowindex = value; }
        }

        private DataJson.PointInfo point = null;

        internal DataJson.PointInfo Point
        {
            get { return point; }
            set { point = value; }
        }

        private void DGVconcrol_Load(object sender, EventArgs e)
        {
            //1.获取当前设备的名字信息 读一次device .ini 该行的类型 
            string[] names = FileMesege.tnselectNode.Text.Split(' ');
            string[] type = IniConfig.GetValue(path + "//devices//" + names[1] + ".ini", "ports", (rowindex + 1).ToString()).Split(',');

            //2.打开对应类型的type.ini文件加载
            string tmp = "";
            //循环读取command 1= 2= 3= 信息
            for (int i = 1; i < 30; i++)
            {
                tmp = IniConfig.GetValue(path + "//types//" + type[0] + ".ini", "command", i.ToString());
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
            Init();
           
             
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
        private void cbTextChange(ComboBox cb,string info)
        {
            cb.Items.Clear();
            cb.Enabled = true;
            if (info.Contains("\\"))
            {
                dealXiegan(cb, info);
            }
            else if (info.Contains(":"))
            {
                dealMaohao(cb, info);
            }
            else if (info.Contains("-"))
            {
                dealHenggan(cb, info);
            }
            else if (info == "getObj")
            {
                if (point != null && point.objType != "" && point.value != "")
                {
                    dealGetObj(cb, info);
                    
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

        private void dealXiegan(ComboBox cb,string info)
        {
            string[] parents = info.Split('\\');
            for (int l = 0; l < parents.Length; l++)
            {
                if (parents[l].Contains(':'))
                {
                    dealMaohao(cb, parents[l]);
                }
                else if(parents[l].Contains('-'))
                {
                    dealHenggan(cb, parents[l]);
                }
                else
                {
                    cb.Items.Add(parents[l]);
                }
            }
        }

        private void dealMaohao(ComboBox cb, string info)
        {
            string[] parents = info.Split(':');
            if (parents[0].Contains("-"))
            {
                string[] child = parents[0].Split('-');
                int j = Convert.ToInt32(child[1]);
                for (int i = Convert.ToInt32(child[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString()+parents[1]);
                }
            }
            else
            {
                cb.Items.Add(info);
            }
            
        }

        private void dealHenggan(ComboBox cb, string info)
        {
            string[] child = info.Split('-');
            int j = Convert.ToInt32(child[1]);
            for (int i = Convert.ToInt32(child[0]); i <= j; i++)
            {
                cb.Items.Add(i.ToString());
            }
        }

        private void dealGetObj(ComboBox cb, string info)
        {
            string filepath = string.Format("{0}//objs//{1}.ini", path, point.objType);
            List<string> keys = IniConfig.ReadKeys(point.value, filepath);
            
            for (int i = 2; i < keys.Count; i++)
            {
                cbTextChange(cb, IniConfig.GetValue(filepath, point.value, keys[i]));
            }
        }

        //改变cbversion窗口值改变
        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbAndlb(cbVersion.SelectedIndex);
        }

        //确认 发送代码操作 或把值传输回DGV框内
        private void btnAffirm_Click(object sender, EventArgs e)
        {
            if ( client.Connected())
            {
                string cb4Num = Regex.Replace(cb4.Text, @"[^\d]*", "");
                if (string.IsNullOrEmpty(cb4Num))
                {
                    return;
                }
                //1.筛选当前发送内容
                //获取obj发送的指令    SET; 指令;对象信息  \r\n
                string content = SocketUtil.strtohexstr(cb1.Text) + SocketUtil.strtohexstr(cb2.Text) + SocketUtil.strtohexstr(cb3.Text) + SocketUtil.strtohexstr(cb4Num);
                string msg = SocketUtil.getObjSet(content, rowindex, FileMesege.tnselectNode);
                //客户端发送数据
                client.SendAsync(msg);
            }
        

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        private void DGVconcrol_FormClosed(object sender, FormClosedEventArgs e)
        {
            //断开tcp连接
            if ( client.Connected())
            {
                client.Dispoes();
            }

        }

        /// <summary>
        /// 异步连接TCP信息回调初始化
        /// </summary>
        private void Init()
        {
            
            client.Completed += new Action<System.Net.Sockets.TcpClient, ClientAsync.EnSocketAction>((c, enAction) =>
            {
                /*string key = "";

                try
                {
                    if ( c.Client.Connected)
                    {
                        IPEndPoint iep = c.Client.RemoteEndPoint as IPEndPoint;
                        key = string.Format("{0}:{1}", iep.Address.ToString(), iep.Port);
                    }
                }
                catch {}*/

                switch (enAction)
                {
                    case ClientAsync.EnSocketAction.Connect:
                        //MessageBox.Show("已经与" + key + "建立连接");
                        break;
                    case ClientAsync.EnSocketAction.SendMsg:

                        //MessageBox.Show(DateTime.Now + "：向" + key + "发送了一条消息");
                        break;
                    case ClientAsync.EnSocketAction.Close:

                        //MessageBox.Show("服务端连接关闭");
                        break;
                    case ClientAsync.EnSocketAction.Error:

                        MessageBox.Show("连接发生错误,请检查网络连接");

                        break;
                    default:
                        break;
                }
            });
            //信息接收处理
            client.Received += new Action<string, string>((key, msg) =>
            {
                MessageBox.Show(key + msg);
            });
            string[] strip = FileMesege.tnselectNode.Parent.Text.Split(' ');
            //异步连接
            client.ConnectAsync(strip[0], 6003);
            
        }

 
    }
}
