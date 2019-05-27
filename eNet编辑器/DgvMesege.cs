using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace eNet编辑器
{
    class DgvMesege
    {
        /// <summary>
        /// 改变鼠标的图标 只支持GIF  PNG图片
        /// </summary>
        /// <param name="cursor">图像参数  Bitmap a = (Bitmap)Bitmap.FromFile(Application.StartupPath + "\\cursor64.png");</param>
        /// <param name="hotPoint">参数输入 new Point(0, 0)</param>
        /// <param name="ctl">参数为控件名</param>
        public static void SetCursor(Bitmap cursor, Point hotPoint, Control ctl)
        {

            int hotX = hotPoint.X;

            int hotY = hotPoint.Y;

            Bitmap myNewCursor = new Bitmap(cursor.Width * 2 - hotX, cursor.Height * 2 - hotY);

            Graphics g = Graphics.FromImage(myNewCursor);

            g.Clear(Color.FromArgb(0, 0, 0, 0));

            g.DrawImage(cursor, cursor.Width - hotX, cursor.Height - hotY, cursor.Width,

            cursor.Height);

            ctl.Cursor = new Cursor(myNewCursor.GetHicon());

            g.Dispose();

            myNewCursor.Dispose();

        }

        /// <summary>
        /// 地址转换
        /// </summary>
        /// <returns></returns>
        public static string addressTransform(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return "";
            }
            string ip = Convert.ToInt32(address.Substring(0, 2),16).ToString();
            string link = Convert.ToInt32(address.Substring(2, 2), 16).ToString();
            string ID = Convert.ToInt32(address.Substring(4, 2), 16).ToString();
            string Port = Convert.ToInt32(address.Substring(6, 2), 16).ToString();
        
            return string.Format("{0}.{1}.{2}.{3}",ip,link,ID,Port);
        }


        /// <summary>
        /// 实现复制功能，将DataGridView中选定单元格的值复制到剪贴板中
        /// </summary>
        /// <param name="dgv_Test"></param>
        public static void CopyData(DataGridView dgv_Test)
        {
            Clipboard.SetDataObject(dgv_Test.GetClipboardContent());
        }

        /// <summary>
        /// 实现粘贴功能，将剪贴板中的内容粘贴到DataGridView中
        /// </summary>
        /// <param name="dgv_Test"></param>  
        public static void PasteData(DataGridView dgv_Test)
        {
            try
            {
                string clipboardText = Clipboard.GetText(); //获取剪贴板中的内容
                if (string.IsNullOrEmpty(clipboardText))
                {
                    return;
                }
                int colnum = 0;
                int rownum = 0;
                for (int i = 0; i < clipboardText.Length; i++)
                {
                    if (clipboardText.Substring(i, 1) == "\t")
                    {
                        colnum++;
                    }
                    if (clipboardText.Substring(i, 1) == "\n")
                    {
                        rownum++;
                    }
                }
                //粘贴板上的数据来源于EXCEL时，每行末尾都有\n，来源于DataGridView是，最后一行末尾没有\n
                if (clipboardText.Substring(clipboardText.Length - 1, 1) == "\n")
                {
                    rownum--;
                }
                colnum = colnum / (rownum + 1);
                object[,] data; //定义object类型的二维数组
                data = new object[rownum + 1, colnum + 1];  //根据剪贴板的行列数实例化数组
                string rowStr = "";
                //对数组各元素赋值
                for (int i = 0; i <= rownum; i++)
                {
                    for (int j = 0; j <= colnum; j++)
                    {
                        //一行中的其它列
                        if (j != colnum)
                        {
                            rowStr = clipboardText.Substring(0, clipboardText.IndexOf("\t"));
                            clipboardText = clipboardText.Substring(clipboardText.IndexOf("\t") + 1);
                        }
                        //一行中的最后一列
                        if (j == colnum && clipboardText.IndexOf("\r") != -1)
                        {
                            rowStr = clipboardText.Substring(0, clipboardText.IndexOf("\r"));
                        }
                        //最后一行的最后一列
                        if (j == colnum && clipboardText.IndexOf("\r") == -1)
                        {
                            rowStr = clipboardText.Substring(0);
                        }
                        data[i, j] = rowStr;
                    }
                    //截取下一行及以后的数据
                    clipboardText = clipboardText.Substring(clipboardText.IndexOf("\n") + 1);
                }
                //获取当前选中单元格的列序号
                int colIndex = dgv_Test.CurrentRow.Cells.IndexOf(dgv_Test.CurrentCell);
                //获取当前选中单元格的行序号
                int rowIndex = dgv_Test.CurrentRow.Index;
                for (int i = 0; i <= rownum; i++)
                {
                    for (int j = 0; j <= colnum; j++)
                    {
                        dgv_Test.Rows[i + rowIndex].Cells[j + colIndex].Value = data[i, j];
                    }
                }
            }
            catch
            {
                MessageBox.Show("粘贴区域大小不一致");
                return;
            }
        }



      

        #region  判断DGV是否点击空白

        /// <summary>
        /// 点击空白处取消dataGridView1的选中  y为e.Y坐标
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="y"></param>
        public static void endDataViewCurrent(DataGridView dataGridView1,int y)
        {
            if (GetRowIndexAt(dataGridView1,y) == -1)
            {
                dataGridView1.CurrentCell = null;
            }
        }
        public static int GetRowIndexAt(DataGridView dataGridView1, int mouseLocation_Y)
        {
            if (dataGridView1.FirstDisplayedScrollingRowIndex < 0)
            {
                return -1;
            }
            if (dataGridView1.ColumnHeadersVisible == true && mouseLocation_Y <= dataGridView1.ColumnHeadersHeight)
            {
                return -1;
            }
            int index = dataGridView1.FirstDisplayedScrollingRowIndex;
            int displayedCount = dataGridView1.DisplayedRowCount(true);
            for (int k = 1; k <= displayedCount; )
            {
                if (dataGridView1.Rows[index].Visible == true)
                {
                    Rectangle rect = dataGridView1.GetRowDisplayRectangle(index, true);  // 取该区域的显示部分区域   
                    if (rect.Top <= mouseLocation_Y && mouseLocation_Y < rect.Bottom)
                    {
                        return index;
                    }
                    k++;
                }
                index++;
            }
            return -1;
        }
        #endregion
    }//class
}
