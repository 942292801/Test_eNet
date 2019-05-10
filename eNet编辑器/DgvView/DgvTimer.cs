using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;

namespace eNet编辑器.DgvView
{
    public partial class DgvTimer : Form
    {
        public DgvTimer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 主Form信息显示
        /// </summary>
        public event Action<string> AppTxtShow;

        //加载DgV所有信息
        public void dgvsceneAddItem()
        {
            
            /*
            this.dataGridView1.Rows.Clear();
            multipleList.Clear();
            if (FileMesege.sceneSelectNode == null)
            {
                return;
            }
            if (FileMesege.sceneSelectNode.Parent != null)
            {
                try
                {
                    //选中子节点
                    //循环获取
                    string[] ips = FileMesege.sceneSelectNode.Parent.Text.Split(' ');
                    string[] ids = FileMesege.sceneSelectNode.Text.Split(' ');
                    int sceneNum = Convert.ToInt32(Regex.Replace(ids[0], @"[^\d]*", ""));
                    string ip4 = SocketUtil.strtohexstr(SocketUtil.getIP(FileMesege.sceneSelectNode));//16进制
                    //获取该节点IP地址场景下的 场景信息对象
                    DataJson.scenes sc = DataListHelper.getSceneInfoList(ips[0], sceneNum);
                    if (sc == null)
                    {
                        return;
                    }
                    List<DataJson.sceneInfo> delScene = new List<DataJson.sceneInfo>();
                    //循环加载该场景号的所有信息
                    foreach (DataJson.sceneInfo info in sc.sceneInfo)
                    {
                        int dex = dataGridView1.Rows.Add();

                        if (info.pid == 0)
                        {
                            //pid号为0则为空 按地址来找
                            if (info.address != "" && info.address != "FFFFFFFF")
                            {
                                DataJson.PointInfo point = DataListHelper.findPointByType_address(info.type, ip4 + info.address.Substring(2, 6));
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
                                info.address = point.address;
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
                        dataGridView1.Rows[dex].Cells[6].Value = Convert.ToDouble(info.Delay) / 10;
                        dataGridView1.Rows[dex].Cells[7].Value = "删除";


                    }
                    for (int i = 0; i < delScene.Count; i++)
                    {
                        sc.sceneInfo.Remove(delScene[i]);
                    }

                }
                catch (Exception ex)
                {
                    this.dataGridView1.Rows.Clear();
                    MessageBox.Show(ex + "\r\n临时调试错误信息 后期删除屏蔽");
                }

            }
            else
            {
                //选中父节点
            }
            */
        }

        private void DgvTimer_Load(object sender, EventArgs e)
        {
            listbox.DataItemVisualCreated += new DataItemVisualEventHandler(ListBox3DataItemVisualCreated);
            listbox.ValueMember = "Id"; // Id property will be used as ValueMemeber so SelectedValue will return the Id
            listbox.DisplayMember = "Text"; // Text property will be used as the item text
     
        }

        void ListBox3DataItemVisualCreated(object sender, DataItemVisualEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)e.Visual;
            
            item.HotTracking = true;
            
        }

        private void listbox_ItemClick(object sender, EventArgs e)
        {
      
        }
    }
}
