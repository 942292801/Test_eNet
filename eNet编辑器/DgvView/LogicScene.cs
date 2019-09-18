using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using eNet编辑器.Properties;
using Newtonsoft.Json;
using System.Reflection;

namespace eNet编辑器.DgvView
{
    public partial class LogicScene : UserControl
    {
        public LogicScene()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true); // 禁止擦除背景.
            SetStyle(ControlStyles.DoubleBuffer, true); // 双缓冲
            //利用反射设置DataGridView的双缓冲
            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
            BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        
        public static event Action<string> AppTxtShow;

        //存放当前操作的的对象 
        DataJson.logicsInfo tmpLogicInfo;



        private void LogicScene_Load(object sender, EventArgs e)
        {
            //新增对象列 加载
            this.dataGridView1.Rows.Clear();
            DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
            DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "//types");
            foreach (FileInfo file in folder.GetFiles("*.ini"))
            {
                dgvc.Items.Add(IniConfig.GetValue(file.FullName, "define", "name"));
            }
            //设置列名
            dgvc.HeaderText = "类型";
            //设置下拉列表的默认值 
            dgvc.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            //或者这样设置 默认选择第一项
            dgvc.DefaultCellStyle.NullValue = dgvc.Items[0];
            dgvc.ReadOnly = true;
            dgvc.Name = "type";
            //插入
            this.dataGridView1.Columns.Insert(1, dgvc);
          
        }

        #region 窗体数据展现
        /// <summary>
        /// 总：窗体数据展现在窗体 
        /// </summary>
        public void formInfoIni(DataJson.logicsInfo logicInfo)
        {
            tmpLogicInfo = null;
            tmpLogicInfo = logicInfo;
            string ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            cbSceneGetItem(ip);
            //执行模式
            cbAttr.SelectedIndex = logicInfo.attr;
            if (tmpLogicInfo.content == null)
            {
                return;
            }
            DataJson.LogicSceneContent logicSceneContent = JsonConvert.DeserializeObject<DataJson.LogicSceneContent>(tmpLogicInfo.content);
            DataJson.scenes sc = DataListHelper.getSceneInfoListByPid(ip,logicSceneContent.pid);
            if (sc == null)
            {
                cbScene.Text = "";
                
            }
            else
            {
                DataJson.PointInfo point = DataListHelper.findPointByPid(logicSceneContent.pid);
                if (point != null)
                {
                    string section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                    string sectionName = string.Format("{0}{1} {2} {3}", Resources.Scene, sc.id, section, point.name);
                    cbScene.Text = sectionName;
                    //加载DGV信息
                    dgvAddItem(logicSceneContent.sceneInfo, ip);

                }


            }//if (sc == null)
            
        }

        /// <summary>
        /// cbScene获取当前IP存在的场景 和 区域进行筛选
        /// </summary>
        public void cbSceneGetItem(string ip)
        {
            cbScene.Items.Clear();

            string section = "";
            //筛选地域
            if (FileMesege.sceneList != null)
            {
                foreach (DataJson.Scene sc in FileMesege.sceneList)
                {
                    //  添加该网关IP的子节点
                    if (sc.IP == ip)
                    {
                        DataJson.PointInfo point = null;
                        if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy) || FileMesege.sectionNodeCopy.Contains("查看所有区域"))
                        {
                            foreach (DataJson.scenes scs in sc.scenes)
                            {
                                point = DataListHelper.findPointByPid(scs.pid, FileMesege.PointList.scene);
                                if (point != null)
                                {
                                    section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                    cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));

                                }
                            }
                        }
                        else
                        {
                            //区域
                            string[] sections = FileMesege.sectionNodeCopy.Split('\\');
                            foreach (DataJson.scenes scs in sc.scenes)
                            {
                                point = DataListHelper.findPointByPid(scs.pid, FileMesege.PointList.scene);
                                if (point != null)
                                {
                                    
                                    section = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim().Replace(" ", "\\");
                                    // 1 2 3 4 用于区分点击区域 区分加载搜索信息 例如东区 把东区所有信息加载进来 
                                    if (string.IsNullOrEmpty(sections[1]) && point.area1 == sections[0])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    // 1 2 3 0
                                    else if (string.IsNullOrEmpty(sections[2]) && point.area1 == sections[0] && point.area2 == sections[1])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    // 1 2 0 0
                                    else if (string.IsNullOrEmpty(sections[3]) && point.area1 == sections[0] && point.area2 == sections[1] && point.area3 == sections[2])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    //1 0 0 0
                                    else if (point.area1 == sections[0] && point.area2 == sections[1] && point.area3 == sections[2] && point.area4 == sections[3])
                                    {
                                        //添加信息
                                        cbScene.Items.Add(string.Format("{0}{1} {2} {3}", Resources.Scene, scs.id, section, point.name));
                                    }
                                    

                                }
                            }
                        }


                    }
                }
            }

            
        }

        /// <summary>
        /// dgv表格展现数据
        /// </summary>
        /// <param name="sceneInfo"></param>
        /// <param name="ip"></param>
        private void dgvAddItem(List<DataJson.sceneInfo> sceneInfo,string ip)
        {
            try
            {
                dataGridView1.Rows.Clear();
                List<DataJson.sceneInfo> delScene = new List<DataJson.sceneInfo>();
                if (sceneInfo == null)
                {
                    return;
                }
                //循环加载该场景号的所有信息
                foreach (DataJson.sceneInfo info in sceneInfo)
                {
                    int dex = dataGridView1.Rows.Add();

                    if (info.pid == 0)
                    {
                        //pid号为0则为空 按地址来找
                        if (info.address != "" && info.address != "FFFFFFFF")
                        {

                            DataJson.PointInfo point = DataListHelper.findPointByType_address(info.type, info.address, ip);
                            if (point != null)
                            {
                                info.pid = point.pid;
                                info.address = point.address;
                                info.type = point.type;
                                dataGridView1.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                                dataGridView1.Rows[dex].Cells[4].Value = point.name;
                            }
                        }

                    }
                    else
                    {
                        //pid号有效 需要更新address type
                        DataJson.PointInfo point = DataListHelper.findPointByPid(info.pid);
                        if (point == null)
                        {
                            //pid号有无效 删除该场景
                            delScene.Add(info);
                            dataGridView1.Rows.Remove(dataGridView1.Rows[dex]);
                            continue;
                        }
                        else
                        {
                            //pid号有效
                            try
                            {
                                if (info.address.Substring(2, 6) != point.address.Substring(2, 6))
                                {
                                    info.address = point.address;

                                }
                            }
                            catch
                            {
                                info.address = point.address;
                            }


                            //////////////////////////////////////////////////////争议地域
                            //类型不一致 在value寻找
                            if (info.type != point.type && !string.IsNullOrEmpty(point.value) && !string.IsNullOrEmpty(point.objType))
                            {
                                //根据value寻找type                        
                                point.type = IniHelper.findObjValueType_ByobjTypeValue(point.objType, point.value);
                            }
                            //////////////////////////////////////////////////////到这里
                            if (info.type != point.type || info.type == "")
                            {
                                //当类型为空时候清空操作
                                info.opt = "";
                                info.optName = "";
                            }
                            info.type = point.type;
                            dataGridView1.Rows[dex].Cells[3].Value = string.Format("{0} {1} {2} {3}", point.area1, point.area2, point.area3, point.area4).Trim();//改根据地址从信息里面获取
                            dataGridView1.Rows[dex].Cells[4].Value = point.name;
                        }

                    }

                    dataGridView1.Rows[dex].Cells[0].Value = info.id;
                    dataGridView1.Rows[dex].Cells[2].Value = DgvMesege.addressTransform(info.address);
                    dataGridView1.Rows[dex].Cells[1].Value = IniHelper.findTypesIniNamebyType(info.type);
                    dataGridView1.Rows[dex].Cells[5].Value = (info.optName + " " + info.opt).Trim();
                    //dataGridView1.Rows[dex].Cells[6].Value = Convert.ToDouble(info.delay) / 10;
                    dataGridView1.Rows[dex].Cells[7].Value = "删除";


                }
                for (int i = 0; i < delScene.Count; i++)
                {
                    sceneInfo.Remove(delScene[i]);
                }

            }
            catch (Exception ex)
            {
                this.dataGridView1.Rows.Clear();
                MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
            }
        }

        #endregion

        #region 设置执行模式和场景
        /// <summary>
        /// 设置按钮 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecid_Click(object sender, EventArgs e)
        {
            if (tmpLogicInfo == null)
            {
                return;
            }
            //主动模式
            if (cbAttr.SelectedIndex != -1)
            {
                tmpLogicInfo.attr = cbAttr.SelectedIndex;

            }
            else
            {
                cbAttr.SelectedIndex = 1;
                tmpLogicInfo.attr = 1;
            }
            string ip = FileMesege.logicSelectNode.Parent.Text.Split(' ')[0];
            int sceneNum = Validator.GetNumber(cbScene.Text.Split(' ')[0]);
            if (sceneNum == -1)
            {
                //场景号不存在
                return;
            }
            //获取该节点IP地址场景下的 场景信息对象
            DataJson.scenes sc = DataListHelper.getSceneInfoList(ip, sceneNum);
            if (sc == null)
            {
                return;
            }
            DataJson.LogicSceneContent logicSceneContent = new DataJson.LogicSceneContent();
            logicSceneContent.pid = sc.pid;
            logicSceneContent.sceneInfo = (List<DataJson.sceneInfo>) CommandManager.CloneObject(sc.sceneInfo);
            tmpLogicInfo.content = JsonConvert.SerializeObject(logicSceneContent);
            dgvAddItem(logicSceneContent.sceneInfo, ip);
            AppTxtShow("设置参数成功");
        }

        #endregion





    }///class
}
