using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace eNet编辑器.DgvView
{
    public partial class DgvVar : Form
    {
        public DgvVar()
        {
            InitializeComponent();
        }

        public event Action<string> AppTxtShow;

        HashSet<DataJson.PointInfo> multipleList = new HashSet<DataJson.PointInfo>();

        private void DgvVar_Load(object sender, EventArgs e)
        {

        }

      

         /// <summary>
        /// 加载DgV所有信息
        /// </summary>
        public void dgvVarAddItem()
        {
            try
            {
                //清空并联点
                //multipleList.Clear();
                this.dataGridView1.Rows.Clear();
                if(FileMesege.varSelectNode == null )
                {
                    return;
                }
                string ip = FileMesege.varSelectNode.Text.Split(' ')[0];
                foreach (DataJson.PointInfo eq in FileMesege.PointList.equipment)
                {
                    //加载该IP的变量
                    if (eq.ip == ip)
                    {
                        CountAddInfo(eq);
                    }

                }
            }
            catch
            {
                this.dataGridView1.Rows.Clear();
                //MessageBox.Show(e.Message + "\r\n后期可以屏蔽该错误");
                //报错不作处理
            }
        }

        /// <summary>
        /// 添加每一行的信息
        /// </summary>
        /// <param name="eq"></param>
        private void CountAddInfo(DataJson.PointInfo eq)
        {
            //添加新的一行 rowNum为行号
            int rowNum = this.dataGridView1.Rows.Add();
            //序号
            this.dataGridView1.Rows[rowNum].Cells[0].Value = (rowNum + 1);
            //地址
            this.dataGridView1.Rows[rowNum].Cells[1].Value = DgvMesege.addressTransform(eq.address);
            //区域
            this.dataGridView1.Rows[rowNum].Cells[2].Value = string.Format("{0} {1} {2} {3}", eq.area1, eq.area2, eq.area3, eq.area4).Trim();
            //名字
            this.dataGridView1.Rows[rowNum].Cells[3].Value = eq.name;
            //删除
            this.dataGridView1.Rows[rowNum].Cells[4].Value = "删除";
        }

        #region 增加 清空 删除
        /// <summary>
        /// 增加按钮 新增一条变量信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            addPoint("变量");
            dgvVarAddItem();
        }

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="typeName"></param>
        public void addPoint(string typeName)
        {
            
            if (string.IsNullOrEmpty(FileMesege.titleinfo))
            {
                AppTxtShow("请选择名称");
                return;
            }
            //搜索选中区域  加载所有同区域的节点
            //区域
            string[] sect = null;
            if (string.IsNullOrEmpty(FileMesege.sectionNodeCopy))
            {
                sect = new string[] { "", "", "", "" };
            }
            else
            {
                sect = FileMesege.sectionNodeCopy.Split('\\');
            }
            if (sect[0] == "全部")
            {
                sect = new string[] { "", "", "", "" };
            }
            //计算name的排序
            DataJson.PointInfo point = new DataJson.PointInfo();
            point.pid = DataChange.randomNum();
            //point.name = sortName(sect);
            //point.address = "FEFB03"+;
            point.area1 = sect[0];
            point.area2 = sect[1];
            point.area3 = sect[2];
            point.area4 = sect[3];
            point.ip = "";

            point.objType = IniHelper.findObjsFileNae_ByName(typeName);


            //point.range = "";
            point.type = "";
            point.value = "";

            //撤销
            DataJson.totalList OldList = FileMesege.cmds.getListInfos();
            FileMesege.PointList.variable.Add(point);
            DataJson.totalList NewList = FileMesege.cmds.getListInfos();
            FileMesege.cmds.DoNewCommand(NewList, OldList);
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

        }

        private void btnDel_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
