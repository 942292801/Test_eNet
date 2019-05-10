using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;


namespace eNet编辑器
{
    class FileMesege
    {
        // 静态跨窗口数据传输  txtGateway.Text + " " +cbVersion.Text + " " +txtinfo.Text 跨窗口传输数据
        public static string info = "";
        /// <summary>
        /// 打开工程文件夹当前路径
        /// </summary>
        public static string filePath = "";
        /// <summary>
        /// 打开工程文件夹当前路径的文件名称
        /// </summary>
        public static string fileName = "";
        //工程临时缓存数据 默认路径
        public static string TmpFilePath = Application.StartupPath + "\\protmp";
        //treesection临时存放数据处 区域
        public static TreeNode sectionNode = null;
        //四个区域点 补齐 分割\\
        public static string sectionNodeCopy = "";//备份 不清除
        //点位区域 英文ini文件名称 tv_ir
        public static string objType = "";
        //treetitle名字临时存放 对象表
        public static string titleinfo = "";
        public static string titlePointSection = "";//存放 点位的名称 section//section---name

        //左栏 树状图treename节点选中临时存放
        public static TreeNode tnselectNode = null;

        //左栏 树状图treename节点选中临时存放
        public static TreeNode sceneSelectNode = null;
        //左栏 树状图treename节点选中临时存放
        public static TreeNode timerSelectNode = null;

        //左栏 树状图treebind节点选中临时存放
        public static TreeNode bindSelectNode = null;
        /// <summary>
        /// form下按钮的选择命名、场景。。。。  默认为命名 用来设置treetitle的显示功能 name,point,scene,timer,panel,reaction,logic
        /// </summary>
        public static string formType = "name";
        /// <summary>
        /// form下面cbType的索引号
        /// </summary>
        public static int cbTypeIndex = 0;

        //复制 粘贴副本
        public static DataJson.PointInfo copyPoint = null;
        public static DataJson.sceneInfo copyScene = null;
        public static DataJson.timersInfo copyTimer = null;

        public static List<DataJson.Device> DeviceList;//工程设备的保存记录
        public static List<DataJson.Area1> AreaList;//
        public static DataJson.Point PointList;//Title表的设备信息
        public static DataJson.Serial serialList;//命名在线设备文件
        public static List<DataJson.Scene> sceneList;//场景
        public static List<DataJson.Timer> timerList;
        public static List<DataJson.Bind> bindList;//绑定

      
     
        //undo redo
        public static CommandManager cmds = new CommandManager();

        /// <summary>
        /// 窗体关闭发生事件
        /// </summary>
        /// <param name="e"></param>
        public  void formclosing(FormClosingEventArgs e)
        {
            if (MessageBox.Show("若未保存工程，窗口关闭后，数据即将丢失！是否现在关闭窗口", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

        //初始化 清空所有值 
        public static void iniPath()
       {
           // 静态跨窗口数据传输  txtGateway.Text + " " +cbVersion.Text + " " + 旧的（修改前）   txtGateway.Text + " " +cbVersion.Text  跨窗口传输数据
            info = "";
            //左栏 树状图treename节点选中临时存放
            tnselectNode = null;
            //左栏 树状图treename节点选中临时存放
            sceneSelectNode = null;
            //左栏 树状图treebind节点选中临时存放
            bindSelectNode = null;
            //工程设备的保存记录
            DeviceList = null;
            //工程位置树状图
            AreaList = null;
            //Title表的设备信息
            PointList = null;           
            //命名在线设备文件
            serialList = null;
            //场景
            sceneList = null;
            //绑定
            bindList = null;
            //undo redo
            cmds = new CommandManager();
            filePath = "";
            fileName = "";
             sectionNode = null;
            //四个区域点 补齐 分割\\
            sectionNodeCopy = "";//备份 不清除
            //点位区域 英文ini文件名称 tv_ir
            objType = "";
            titleinfo = "";
            titlePointSection = "";//存放 点位的名称 section//section---name
       }

        public bool newfile()
        {
            //IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", filePath);
            iniPath();
            return true;            
            
        }

        public bool openfile()
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                string historyPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                if (historyPath != "")
                {
                    //设置此次默认目录为上一次选中目录  
                    op.InitialDirectory = historyPath.Split(',')[0];
                    
                }  
                op.Title = "请打开工程文件";
                op.Filter = "项目文件（*.yc）|*.yc|压缩文件（*.zip）|*.zip|All files(*.*)|*.*";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    if (readProject(op.FileName.ToString()))
                    {
                        if (!historyPath.Contains(filePath))
                        {
                            //添加打开过的地址
                            IniConfig.SetValue(Application.StartupPath + "\\conf.ini", "filepath", "path", filePath + "," + historyPath);
                        }
                        return true;
                    }

                }
                return false;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }  

        }

        /// <summary>
        /// 读取需要打开的文件项目
        /// </summary>
        /// <param name="openFilePath"></param>
        /// <returns></returns>
        public bool readProject(string openFilePath)
        {
            string localFilePath = openFilePath.ToString(); //获得文件路径 
            string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
            //读取的后缀不为.cy
            if (fileNameExt.Split('.')[1] != "yc")
            {
                return false;
            }
            tmpPathClear();
            ZipHelper zipHelp = new ZipHelper();
            try
            {
                
                zipHelp.UnZip(localFilePath, TmpFilePath);
            }
            catch
            {
                return false;
            }
            
            if (!System.IO.File.Exists(TmpFilePath + "\\pro\\area.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\device.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\point.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\scene.json")
                || !System.IO.File.Exists(TmpFilePath + "\\pro\\bind.json")
                )
            {
                MessageBox.Show("操作失败！工程文件缺失！", "提示");
                return false;
            }
            iniPath();
            DeviceList = JsonConvert.DeserializeObject<List<DataJson.Device>>(File.ReadAllText(TmpFilePath + "\\pro\\device.json"));
            AreaList = JsonConvert.DeserializeObject<List<DataJson.Area1>>(File.ReadAllText(TmpFilePath + "\\pro\\area.json"));
            PointList = JsonConvert.DeserializeObject<DataJson.Point>(File.ReadAllText(TmpFilePath + "\\pro\\point.json"));
            sceneList = JsonConvert.DeserializeObject<List<DataJson.Scene>>(File.ReadAllText(TmpFilePath + "\\pro\\scene.json"));
            bindList = JsonConvert.DeserializeObject<List<DataJson.Bind>>(File.ReadAllText(TmpFilePath + "\\pro\\bind.json"));

            filePath = localFilePath;
            fileName = fileNameExt;
            return true;
        }

        public bool savefile()
        {
            try 
            {
                string localFilePath = filePath;
                string fileNameExt = fileName;
                //数据在缓存文件夹中
                if (string.IsNullOrEmpty(filePath))
                {
                    
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "请选择保存路径";
                    //设置文件类型 
                    sfd.Filter = "项目文件（*.yc）|*.yc|压缩文件（*.zip）|*.zip|All files(*.*)|*.*";
                    //设置默认文件类型显示顺序 
                    sfd.FilterIndex = 1;
                    //保存对话框是否记忆上次打开的目录 
                    sfd.RestoreDirectory = true;

                    //设置默认的文件名

                    sfd.FileName = "ProjectFile";// in wpf is  sfd.FileName = "YourFileName";

                    string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                    if (newPath != "")
                    {
                        //设置此次默认目录为上一次选中目录  
                        sfd.InitialDirectory = newPath.Split(',')[0];

                    } 
                    //点了保存按钮进入 
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        localFilePath = sfd.FileName.ToString(); //获得文件路径 
                        fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                    }
                    else
                    {
                        //不做操作 退出
                        return false;
                    }
                    
                }
                filePath = localFilePath;
                fileName = fileNameExt;
                writeAlltoPro(TmpFilePath);
                ZipHelper zipHelp = new ZipHelper();
                string[] fileList = {TmpFilePath+"//pro",TmpFilePath+"//objs"};
                try
                {
                    //压缩到选中路径
                    zipHelp.ZipManyFilesOrDictorys(fileList, localFilePath);
                }
                catch
                {
                    return false;
                }
                return true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }  

             
        }

        public bool othersavefile()
        {
            try
            {
                string localFilePath = "";
                string fileNameExt = "";
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "请选择保存路径";
                //设置文件类型 
                sfd.Filter = "项目文件（*.yc）|*.yc|压缩文件（*.zip）|*.zip|All files(*.*)|*.*";
                //设置默认文件类型显示顺序 
                sfd.FilterIndex = 1;
                //保存对话框是否记忆上次打开的目录 
                sfd.RestoreDirectory = true;

                //设置默认的文件名

                sfd.FileName = "ProjectFile";// in wpf is  sfd.FileName = "YourFileName";
                string newPath = IniConfig.GetValue(Application.StartupPath + "\\conf.ini", "filepath", "path");
                if (newPath != "")
                {
                    //设置此次默认目录为上一次选中目录  
                    sfd.InitialDirectory = newPath.Split(',')[0];

                }
                //点了保存按钮进入 
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    localFilePath = sfd.FileName.ToString(); //获得文件路径 
                    fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                    writeAlltoPro(TmpFilePath);
                    ZipHelper zipHelp = new ZipHelper();
                    string[] fileList = { TmpFilePath + "//pro", TmpFilePath + "//objs" };
                    try
                    {
                        //压缩到选中路径
                        zipHelp.ZipManyFilesOrDictorys(fileList, localFilePath);
                    }
                    catch
                    {
                        return false;
                    }

                    return true;

                }
                else
                {
                    //不做操作 退出
                    return false;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return false;
            }  
                       
        }

        /// <summary>
        /// 格式化JSON文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }


        /// <summary>
        /// 把缓存数据写进文档 补充说明：工程保存写入文件
        /// </summary>
        /// <param name="path">Pro文件路径</param>
        private void writeAlltoPro(string path)
        {
            tmpPathClear();
            File.WriteAllText(path + "\\pro\\point.json", ConvertJsonString(JsonConvert.SerializeObject(PointList)));
            File.WriteAllText(path + "\\pro\\area.json", ConvertJsonString(JsonConvert.SerializeObject(AreaList)));
            File.WriteAllText(path + "\\pro\\device.json", ConvertJsonString(JsonConvert.SerializeObject(DeviceList)));
            File.WriteAllText(path + "\\pro\\scene.json", ConvertJsonString(JsonConvert.SerializeObject(sceneList)));
            File.WriteAllText(path + "\\pro\\bind.json", ConvertJsonString(JsonConvert.SerializeObject(bindList)));
        }

        /// <summary>
        /// 清除protmp缓存的文件
        /// </summary>
        /// <param name="path"></param>
        public void tmpPathClear()
        {
            if (System.IO.Directory.Exists(TmpFilePath + "\\objs"))
            {
                Directory.Delete(TmpFilePath + "\\objs", true);
            }
            if (System.IO.Directory.Exists(TmpFilePath + "\\pro"))
            {
                Directory.Delete(TmpFilePath + "\\pro", true);
            }
            Directory.CreateDirectory(TmpFilePath + "\\objs");
            Directory.CreateDirectory(TmpFilePath + "\\pro");
        }





        //end
    }
}
