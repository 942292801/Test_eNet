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
using System.Runtime.InteropServices;

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

        /// <summary>
        /// 操作名称
        /// </summary>
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
        
        //getObj用的point  只限equipment用
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

        /// <summary>
        /// address下 2 的内容（ 类型，xx,0 ）
        /// </summary>
        List<string> TypeList = null;

        LinkType linkType = LinkType.Dev;

        /// <summary>
        /// 链路的类型
        /// </summary>
        public enum LinkType
        {
            /// <summary>
            /// 设备类型
            /// </summary>
            Dev = 1,
            /// <summary>
            /// 虚拟端口类型
            /// </summary>
            Var = 2,
            /// <summary>
            /// 场景 定时 面板 类型
            /// </summary>
            Com = 3

        }


        private void sceneConcrol_Load(object sender, EventArgs e)
        {
            try
            {
                //循环读取INI里面的信息
                DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
                string type = "";
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
                        cbVersion.Items.Add("赋值");
                        //默认加载cbversion第一个选项信息
                        cbVersion.SelectedIndex = 0;
                        cbTypeitemIni();
                        cbAndlb(0);
                        break;
                    }//类型一致


                }

                //窗口恢复
                if (string.IsNullOrEmpty(ver) && string.IsNullOrEmpty(opt))
                {
                    return;
                }
                object isExit = null;
                string itmeTxt = "";
                for (int i = 0; i < cbVersion.Items.Count; i++)
                {
                    itmeTxt = cbVersion.Items[i].ToString();
                    if (itmeTxt == ver)
                    {
                        //optName存在该名称
                        cbVersion.SelectedItem = cbVersion.Items[i];
                        string tmp1 = DataChange.HexStringToString(opt.Substring(0, 2));
                        for (int j = 0; j < cb1.Items.Count; j++)
                        {
                            if (cb1.Items[j].ToString().Contains(tmp1))
                            {
                                cb1.Text = cb1.Items[j].ToString();
                                break;
                            }
                        }
                        string tmp2 = DataChange.HexStringToString(opt.Substring(2, 2));
                        if (tmp1 == "254" )
                        {
                            //赋值状态
                            if( tmp2 == "251")
                            {
                                //虚拟端口
                                cb2.SelectedIndex = cb2.Items.Count - 1;
                                //cb3.Text = DataChange.HexStringToString(opt.Substring(4, 2));
                                cb4.Text = DataChange.HexStringToString(opt.Substring(6, 2));
                                findVarNum();
                               
                            }
                            else if (linkType == LinkType.Var )
                            {
                                //虚拟端口
                                cb2.Text = IniHelper.findIniLinkTypeByAddress(opt);
                                if (linkType == LinkType.Dev)
                                {
                                    string num3 = DataChange.HexStringToString(opt.Substring(4, 2));
                                    cb3.Text = num3;
                                    cb4.Text = DataChange.HexStringToString(opt.Substring(6, 2));
                                    findPort(num3);
                                }
                                else if (linkType == LinkType.Var)
                                {
                                    cb4.Text = DataChange.HexStringToString(opt.Substring(6, 2));
                                    findVarNum();
                                }
                                else if (linkType == LinkType.Com)
                                {
                                    string num3 = DataChange.HexStringToString(opt.Substring(4, 2));
                                    string num4 = DataChange.HexStringToString(opt.Substring(6, 2));
                                    //将超256 的号数操作
                                    cb4.Text = (Convert.ToInt32(num3) * 256 + Convert.ToInt32(num4)).ToString();
                                    findNum();
                                }
         
                            }
                            else if (linkType == LinkType.Dev)
                            {
                                cb2.SelectedIndex = 0;
                                string num3 = DataChange.HexStringToString(opt.Substring(4, 2));
                                cb3.Text = num3;
                                cb4.Text = DataChange.HexStringToString(opt.Substring(6, 2));
                                findPort(num3);
                            }
                            else if (linkType == LinkType.Com)
                            {
                                //不为场景 定时 面板 编组
                                cb2.SelectedIndex = 0;
                                string num3 = DataChange.HexStringToString(opt.Substring(4, 2));
                                string num4 = DataChange.HexStringToString(opt.Substring(6, 2));
                                //将超256 的号数操作
                                cb4.Text = (Convert.ToInt32(num3) * 256 + Convert.ToInt32(num4)).ToString();
                                findNum();
                            }
                           
                            return;
                        }
                        else
                        {
                            for (int j = 0; j < cb2.Items.Count; j++)
                            {
                                if (cb2.Items[j].ToString().Contains(tmp2))
                                {
                                    cb2.Text = cb2.Items[j].ToString();
                                    break;
                                }
                            }
                        }

                        string tmp3 = DataChange.HexStringToString(opt.Substring(4, 2));
                        for (int j = 0; j < cb3.Items.Count; j++)
                        {
                            if (cb3.Items[j].ToString().Contains(tmp3))
                            {
                                cb3.Text = cb3.Items[j].ToString();
                                break;
                            }
                        }
                        string tmp4 = DataChange.HexStringToString(opt.Substring(6, 2));
                        for (int j = 0; j < cb4.Items.Count; j++)
                        {
                            if (cb4.Items[j].ToString().Contains(tmp4))
                            {
                                cb4.Text = cb4.Items[j].ToString();
                                break;
                            }
                        }
                        return;
                    }
                    if (itmeTxt == "状态操作")
                    {
                        isExit = cbVersion.Items[i];
                    }
                }

                if (isExit != null)
                {
                    //存在状态操作 恢复状态操作
                    cbVersion.SelectedItem = isExit;
                    cb1.Text = DataChange.HexStringToString(opt.Substring(0, 2));
                    cb2.Text = DataChange.HexStringToString(opt.Substring(2, 2));
                    cb3.Text = DataChange.HexStringToString(opt.Substring(4, 2));
                    string tmp = DataChange.HexStringToString(opt.Substring(6, 2));
                    for (int i = 0; i < cb4.Items.Count; i++)
                    {
                        if (cb4.Items[i].ToString().Contains(tmp))
                        {
                            cb4.SelectedItem = cb4.Items[i];
                            return;
                        }
                    }

                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
        }

        /// <summary>
        /// Version选项改变时 lb信息 和cb信息更新
        /// </summary>
        /// <param name="indexs">version索引</param>
        private void cbAndlb(int indexs)
        {
            if (comdsList.Count > indexs)
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
            else
            {
                ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                //清除信息
                for (int i = 0; i < 4; i++)
                {
                    cbs[i].Items.Clear();
                    cbs[i].Enabled = true;
                    cbs[i].Text = "";
                }
                lb2.Text = "类型";
                if (objType == "虚拟端口")
                {
                  
                    for (int i = 0; i < TypeList.Count; i++)
                    {
                        if (TypeList[i].Split(',')[1] == "按键" || TypeList[i].Split(',')[1] == "感应输入")
                        {
                            continue;
                        }
                        cb2.Items.Add(TypeList[i].Split(',')[1]);
                    }
                    cb2.Text = objType;
                    addIp();
                    searchNum();
                }
                else
                {
               
                    iniInfo(objType);
                    addIp();
                    searchNum();
                    cb2.Items.Add("虚拟端口");
                    
                }
                
            }
        }


        /// <summary>
        /// 初始化加载ini里面的类型
        /// </summary>
        private void cbTypeitemIni()
        {
            //记录全部cb2的类型

            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");

            HashSet<string> nameList = new HashSet<string>();
            string tmp = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                tmp = IniConfig.GetValue(file.FullName, "address", "2");
                if (!string.IsNullOrEmpty(tmp))
                {
                    nameList.Add(tmp);
                }

            }
            TypeList = nameList.ToList<string>();
           
        
        }


        /// <summary>
        /// 根据类型type（中文名） cb2会加载该类型的
        /// </summary>
        private void iniInfo(string findType)
        {
            linkType = LinkType.Dev;
            string fileName = IniHelper.findTypesIniTypebyName(findType);
            string path = string.Format("{0}//types//{1}.ini", Application.StartupPath, fileName);
            Label[] lbs = { lb1, lb2, lb3, lb4 };
            ComboBox[] cbs = { cb1, cb2, cb3, cb4 };

            //读取【address】1-4信息
            for (int i = 0; i < 4; i++)
            {
                string[] infos = IniConfig.GetValue(path, "address", (i + 1).ToString()).Split(',');
                if (infos[0] == "固定" && infos.Length == 2)
                {
                    lbs[i].Text = infos[0];
                    cbs[i].Text = infos[1];
                    cb1.Text = "254";
                    cb3.Enabled = false;
                    //设置为虚拟端口类型
                    linkType = LinkType.Var;
                }
                else if (infos.Length == 2)
                {
                    lbs[i].Text = infos[0];
                    dealInfoNum(cbs[i], infos[1]);
                }
                else if (infos.Length == 3)
                {
                    //类型信息  添加
                    cb2.Text = infos[1];
                    cb2.Items.Add(infos[1]);
                   
                }
                else
                {
                    //场景 定时 面板 传感 即cb3关闭
                    lb3.Text = "";
                    cb3.Text = "";
                    cb3.Enabled = false;
                    //设置 以下类型
                    linkType = LinkType.Com;
                }
            }///foreach 


        }

        /// <summary>
        /// 根据类型type（中文名）cb2不会加载该类型
        /// </summary>
        /// <param name="findType"></param>
        private void getInfo(string findType)
        {
            linkType = LinkType.Dev;
            string fileName = IniHelper.findTypesIniTypebyName(findType);
            string path = string.Format("{0}//types//{1}.ini", Application.StartupPath, fileName);
            Label[] lbs = { lb1, lb2, lb3, lb4 };
            ComboBox[] cbs = { cb1, cb2, cb3, cb4 };

            //读取【address】1-4信息
            for (int i = 0; i < 4; i++)
            {
                string[] infos = IniConfig.GetValue(path, "address", (i + 1).ToString()).Split(',');
                if (infos[0] == "固定" && infos.Length == 2)
                {
                    lbs[i].Text = infos[0];
                    cbs[i].Text = infos[1];
                    cb1.Text = "254";
                    cb3.Enabled = false;
                    //设置为虚拟端口类型
                    linkType = LinkType.Var;
                }
                else if (infos.Length == 2)
                {
                    lbs[i].Text = infos[0];
                    dealInfoNum(cbs[i], infos[1]);
                }
                else if (infos.Length == 3)
                {
                    //类型信息  添加
                    //cb2.Text = infos[1];
                    //cb2.Items.Add(infos[1]);
                }
                else
                {
                    //场景 定时 面板 传感 即cb3关闭
                    lb3.Text = "";
                    cb3.Text = "";
                    cb3.Enabled = false;
                    //设置 以下类型
                    linkType = LinkType.Com;
                }
            }///foreach 


        }

        /// <summary>
        /// 当cb2类型改变cb3 cb4的格式  TypeList的格式
        /// </summary>
        /// <param name="cbType"></param>
        private void typeChange(string cbType)
        {
            //循环读取INI里面的信息
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            string type = "";
            cb3.Text = "";
            cb4.Text = "";
            linkType = LinkType.Dev;
            string tmp = "";
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                tmp = IniConfig.GetValue(file.FullName, "address", "2");
                if (string.IsNullOrEmpty(tmp))
                {
                    continue;
                }
                type = tmp.Split(',')[1];
                //找到类型一致
                if (type == cbType)
                {
                    //记录当前类型 场景 定时 面板 人感 编组
                   
                    Label[] lbs = { lb1, lb2, lb3, lb4 };
                    ComboBox[] cbs = { cb1, cb2, cb3, cb4 };
                    //读取【address】1-4信息
                    for (int i = 0; i < 4; i++)
                    {
                        string[] infos = IniConfig.GetValue(file.FullName, "address", (i + 1).ToString()).Split(',');
                        if (infos[0] == "固定" && infos.Length == 2)
                        {
                            lbs[i].Text = infos[0];
                            cbs[i].Text = infos[1];
                            cb1.Text = "254";
                            cb3.Enabled = false;
                            linkType = LinkType.Var;

                        }
                        else if (infos.Length == 2)
                        {
                            lbs[i].Text = infos[0];
                            dealInfoNum(cbs[i], infos[1]);
                        }
                        else if (infos.Length == 3)
                        {
                            //类型信息
                            //cb2.Text = infos[1];

                        }
                        else
                        {
                            //场景 定时 面板 传感 即cb3关闭
                            lbs[i].Text = "";
                            cb3.Items.Clear();
                            cb3.Text = "";
                            cb3.Enabled = false;
                            linkType = LinkType.Com;
                        }

                    }
                    break;
                }
               
            }///foreach 

        }

        #region 处理1-9 或 1,2,3  或数字
        /// <summary>
        /// cb信息内容的判断1-9 或 1,2,3  或数字
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void cbTextChange(ComboBox cb, string info)
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
                if (point != null && !string.IsNullOrEmpty(point.objType) && !string.IsNullOrEmpty(point.value) )
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
            try
            {
                //初始化索引
                cb.SelectedIndex = 0;
            }
            catch {
            }
            
         
        }

        /// <summary>
        /// 斜杆
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealXiegan(ComboBox cb, string info)
        {
            string[] parents = info.Split('\\');
            for (int l = 0; l < parents.Length; l++)
            {
                if (parents[l].Contains(':'))
                {
                    dealMaohao(cb, parents[l]);
                }
                else if (parents[l].Contains('-'))
                {
                    dealHenggan(cb, parents[l]);
                }
                else
                {
                    cb.Items.Add(parents[l]);
                }
            }
        }

        /// <summary>
        /// 冒号
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealMaohao(ComboBox cb, string info)
        {
            string[] parents = info.Split(':');
            if (parents[0].Contains("-"))
            {
                string[] child = parents[0].Split('-');
                int j = Convert.ToInt32(child[1]);
                for (int i = Convert.ToInt32(child[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString()  + parents[1]);
                }
            }
            else
            {
                cb.Items.Add(info);
            }

        }

        /// <summary>
        /// 横杆
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealHenggan(ComboBox cb, string info)
        {
            string[] child = info.Split('-');
            int j = Convert.ToInt32(child[1]);
            if (j > 100)
            {
                j = 100;
            }
            for (int i = Convert.ToInt32(child[0]); i <= j; i++)
            {
                cb.Items.Add(i.ToString());
            }
        }

        private void dealGetObj(ComboBox cb, string info)
        {
            string filepath = string.Format("{0}//objs//{1}.ini", Application.StartupPath, point.objType);
            List<string> keys = IniConfig.ReadKeys(point.value, filepath);

            for (int i = 2; i < keys.Count; i++)
            {
                cbTextChange(cb, IniConfig.GetValue(filepath, point.value, keys[i]));
            }
        }

        #endregion

        private void btnDecid_Click(object sender, EventArgs e)
        {
            try
            {
                if (comdsList.Count > cbVersion.SelectedIndex)
                {
                    string cb1Num = dealNum(cb1.Text);
                    string cb2Num = dealNum(cb2.Text);
                    string cb3Num = dealNum(cb3.Text);
                    string cb4Num = dealNum(cb4.Text);
                    if (string.IsNullOrEmpty(cb1Num) || string.IsNullOrEmpty(cb2Num) || string.IsNullOrEmpty(cb3Num) || string.IsNullOrEmpty(cb4Num))
                    {
                        this.DialogResult = DialogResult.No;
                        return;
                    }
                    this.opt = ToolsUtil.strtohexstr(cb1Num) + ToolsUtil.strtohexstr(cb2Num) + ToolsUtil.strtohexstr(cb3Num) + ToolsUtil.strtohexstr(cb4Num);
                    if (cb4.Text.Split(':').Length > 1)
                    {
                        if (cbVersion.Text == "状态操作")
                        {
                            //当为getObj的操作的时候
                            this.ver = cb4.Text.Split(':')[1];
                        }
                        else
                        {
                            this.ver = cbVersion.Text;
                        }

                    }
                    else
                    {
                        
                        this.ver = cbVersion.Text;
                    }
                }
                else
                {
                    //赋值
                    string newobj = "";
                    if (cb1.Text != "" && cb2.Text != "")
                    {

                        for (int i = 0; i < TypeList.Count; i++)
                        {
                            if (TypeList[i].Split(',')[1] == cb2.Text)
                            {
                                newobj = ToolsUtil.strtohexstr(cb1.Text) + ToolsUtil.strtohexstr(TypeList[i].Split(',')[2]);
                                break;
                            }
                        }

                        if (cb3.Text != "" && cb4.Text != "")
                        {
                            //设备
                            newobj = newobj + ToolsUtil.strtohexstr(cb3.Text) + ToolsUtil.strtohexstr(cb4.Text);
                        }
                        else if (cb3.Text == "" && cb4.Text != "")
                        {
                            //非设备类
                            string tmp = ToolsUtil.strtohexstr(cb4.Text);
                            while (tmp.Length < 4)
                            {
                                tmp = tmp.Insert(0, "0");
                            }
                            newobj = newobj + tmp;
                        }
                    }
                    if (newobj.Length != 8)
                    {
         
                        this.DialogResult = DialogResult.No;
                        return;
                    }
                    this.opt = newobj;


                    this.ver = cbVersion.Text;


                }
                if (string.IsNullOrEmpty(this.ver) )
                {
                    this.DialogResult = DialogResult.No;
                    return;
                }
                this.DialogResult = DialogResult.OK;

            }
            catch
            {
                this.opt = "";
                this.ver = "";
                this.DialogResult = DialogResult.No;
            }
    
        }

        /// <summary>
        /// 提取 21秒 或 255 ：0.1秒 前面的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string dealNum(string str)
        {
            string tmp = "";
            if (str.Contains(":"))
            {
                tmp = Regex.Replace(str.Split(':')[0], @"[^\d]*", "");
            }
            else
            {
                tmp = Regex.Replace(str, @"[^\d]*", "");
            }
            return tmp;
        }

  

        private void cbVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbAndlb(cbVersion.SelectedIndex);
        }

        private void cb2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVersion.SelectedIndex < comdsList.Count)
            {
                //选中不为赋值
                return;
            }
            if (objType == "虚拟端口")
            {
                typeChange(cb2.Text);
                searchNum();
            }
            else
            {
                //多一次虚拟端口加载
                if (cb2.SelectedIndex == 0)
                {

                    getInfo(objType);
                    addIp();
                    searchNum();
                }
                else
                {
                    //选中变量
                    lb3.Text = "固定";
                    lb4.Text = "虚拟端口号";
                    cb1.Text = "254";
                    cb3.Enabled = false;
                    cb3.Text = "3";
                    dealInfoNum(cb4, "1-100");
                    findVarNum();
                }
            }
            
    
        }



        private void cb3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cb2.Text == "设备" && !string.IsNullOrEmpty(cb3.Text))
            {

                findPort(cb3.Text);
            }
            
        }

        /// <summary>
        /// 存放当前dev表中读取到的 ID+设备
        /// </summary>
        List<string> devName = null;
        private string ip = "";


        /// <summary>
        /// 加载网关Ip到cb1的框里面 
        /// </summary>
        /// <param name="selectSumNode">选中子节点</param>
        private void addIp()
        {

            try
            {
                devName = new List<string>();
                switch (FileMesege.formType)
                {
                    ////////////////////////////////////还需要后续添加选中节点参数
                    case "name":

                        break;

                    case "point":
                        break;
                    case "scene":
                        ip = FileMesege.sceneSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "timer":
                        ip = FileMesege.timerSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "panel":
                        ip = FileMesege.panelSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "sensor":
                        ip = FileMesege.sensorSelectNode.Parent.Text.Split(' ')[0];
                        break;
                    case "logic":
                        ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
                        break;

                    default: break;
                }

                cb1.Enabled = true;
                cb1.Text = "254"; ;
                

            }
            catch
            {

            }

        }

        /// <summary>
        /// 加载网关的设备到cb3里面
        /// </summary>
        private void addNumForDevList()
        {
            cb3.Enabled = true;
            foreach (DataJson.Device devip in FileMesege.DeviceList)
            {
                if (devip.ip == ip)
                {
                    cb3.Items.Clear();
                    foreach (DataJson.Module md in devip.module)
                    {
                        devName.Add(md.id + " " + md.device);
                        cb3.Items.Add(md.id);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 寻找该设备的端口数目
        /// </summary>
        /// <param name="id"></param>
        private void findPort(string id)
        {
            if (devName == null)
            {
                return;
            }
            cb4.Enabled = true;
            for (int i = 0; i < devName.Count; i++)
            {
                if (devName[i].Split(' ')[0] == id)
                {
                    string filepath = IniHelper.findDevicesDisplay(devName[i].Split(' ')[1]);
                    //string filepath = string.Format("{0}\\devices\\{1}.ini", Application.StartupPath, devName[i].Split(' ')[1]);
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
        /// 查找当前ip的存在场景 定时 面板  逻辑 感应编组 号
        /// </summary>
        private void findNum()
        {
            try
            {

                //////////////////////////////////后期还需要添加
                switch (cb2.Text)
                {
                    case "场景":
                        cb4.Items.Clear();
                         string tmpip = ip;
                        if (cb1.Text != "254")
                        {
                            string[] ips = ip.Split('.');
                            tmpip = string.Format("{0}.{1}.{2}.{3}", ips[0], ips[1], ips[2], cb1.Text);
                        }
                        DataJson.Scene scene = DataListHelper.getSceneList(tmpip);

                        foreach (DataJson.scenes scenes in scene.scenes)
                        {
                            cb4.Items.Add(scenes.id);
                        }
                        break;
                    case "定时":
                        cb4.Items.Clear();
                        DataJson.Timer timer = DataListHelper.getTimerList(ip);

                        foreach (DataJson.timers tms in timer.timers)
                        {
                            cb4.Items.Add(tms.id);
                        }
                        break;
                    case "面板":
                        cb4.Items.Clear();
                        DataJson.Panel panel = DataListHelper.getPanelList(ip);

                        foreach (DataJson.panels pls in panel.panels)
                        {
                            cb4.Items.Add(pls.id);
                        }
                        break;
                    case "感应编组":
                        cb4.Items.Clear();
                        DataJson.Sensor sensor = DataListHelper.getSensorList(ip);

                        foreach (DataJson.sensors srs in sensor.sensors)
                        {
                            cb4.Items.Add(srs.id);
                        }
                        break;
                    case "逻辑":
                        cb4.Items.Clear();
                        DataJson.Logic logic = DataListHelper.getLogicList(ip);

                        foreach (DataJson.logics lgs in logic.logics)
                        {
                            cb4.Items.Add(lgs.id);
                        }
                        break;
                    case "局部变量":
                        cb4.Items.Clear();
                         logic = DataListHelper.getLogicList(ip);

                        foreach (DataJson.logics lgs in logic.logics)
                        {
                            for (int i = (lgs.id-1) * 16 + 1; i <= lgs.id * 16; i++)
                            {
                                cb4.Items.Add(i);
                            }
                                
                        }
                        break;
                    default:
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// 查找当前ip的存在虚拟端口
        /// </summary>
        private void findVarNum()
        {
            try
            {
                cb4.Items.Clear();
                foreach (DataJson.PointInfo point in FileMesege.PointList.virtualport)
                {
                    if (point.ip == ip)
                    {

                        cb4.Items.Add(Convert.ToInt32(point.address.Substring(6, 2), 16));
                    }
                }

            }
            catch { }
        }



        /// <summary>
        /// 搜索当前类型的 实际存在号加载到cb框
        /// </summary>
        private void searchNum()
        {
            if (linkType == LinkType.Dev)
            {
                addNumForDevList();

            }
            else if (linkType == LinkType.Com)
            {
                findNum();
                cb3.Enabled = false;
            }
            else if (linkType == LinkType.Var)
            {
                findVarNum();
                cb3.Enabled = false;
            }
        }

        /// <summary>
        /// cb信息内容的判断1-9 或 1,2,3  或数字（链路类型）
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="info"></param>
        private void dealInfoNum(ComboBox cb, string info)
        {
 
            if (info.Contains("-"))
            {
                string[] infos = info.Split('-');
                int j = Convert.ToInt32(infos[1]);
                if (j > 100)
                {
                    j = 100;
                }
                for (int i = Convert.ToInt32(infos[0]); i <= j; i++)
                {
                    cb.Items.Add(i.ToString());
                }
            }
            else if (info.Contains(","))
            {
                string[] infos = info.Split(',');

                for (int i = 0; i < infos.Length; i++)
                {
                    cb.Items.Add(infos[i]);
                }
            }
            else
            {
                cb.Items.Add(info);

            }

        }

        #region 窗体样色


        #region 窗体样色2
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
         );

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

        private bool m_aeroEnabled;                     // variables for box shadow
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        public struct MARGINS                           // struct for box shadow
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        private const int WM_NCHITTEST = 0x84;          // variables for dragging the form
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }

        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:                        // box shadow
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 1,
                            rightWidth = 1,
                            topHeight = 1
                        };
                        DwmExtendFrameIntoClientArea(this.Handle, ref margins);

                    }
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)     // drag the form
                m.Result = (IntPtr)HTCAPTION;

        }
        #endregion
        private void sceneConcrol_Paint(object sender, PaintEventArgs e)
        {
            
            Rectangle myRectangle = new Rectangle(0, 0, this.Width, this.Height);
            //ControlPaint.DrawBorder(e.Graphics, myRectangle, Color.Blue, ButtonBorderStyle.Solid);//画个边框 
            ControlPaint.DrawBorder(e.Graphics, myRectangle,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 1, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid,
                Color.DarkGray, 2, ButtonBorderStyle.Solid
            );
        }

        private Point mPoint;

    

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void plInfoTitle_MouseDown(object sender, MouseEventArgs e)
        {
            mPoint = new Point(e.X, e.Y);
        }

        private void plInfoTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Location = new Point(this.Location.X + e.X - mPoint.X, this.Location.Y + e.Y - mPoint.Y);
            }
        }

        #endregion



     

        

      
    }
}
