using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using System.IO;  

namespace eNet编辑器
{

    /// <summary>   
    /// 适用与ZIP压缩   
    /// </summary>   
    public class ZipHelper
    { 

    //缓存大小
    private int BufferSize;

    //压缩率 0（无压缩）-9（压缩率最高）
    private int CompressionLevel;

    //压缩是用于存放压缩路径
    private string CompressPath;


    #region 构造函数
    /// <summary>
    /// 无参构造
    /// </summary>
    public ZipHelper()
    {
        this.BufferSize = 4096;
        this.CompressionLevel = 9;
    }

    /// <summary>
    /// 带参数构造
    /// </summary>
    /// <param name="compressionLevel">0（无压缩）-9（压缩率最高）</param>
    public ZipHelper(int compressionLevel)
    {
        this.BufferSize = 4096;
        this.CompressionLevel = compressionLevel;
    }
    #endregion

    #region 压缩方法
    /// <summary>
    /// 功能： ZIP方式压缩文件（压缩目录）
    /// </summary>
    /// <param name="dirPath">被压缩的文件夹夹路径</param>
    /// <param name="zipFilePath">生成压缩文件的路径，缺省值：文件夹名+.zip</param>
    /// <param name="msg">返回消息</param>
    /// <returns>是否压缩成功</returns>
    public bool ZipFolder(string dirPath, string zipFilePath, out string msg)
    {
        this.CompressPath = dirPath;
        //判断目录是否为空
        if (dirPath == string.Empty)
        {
            msg = "The Folder is empty";
            return false;
        }

        //判断目录是否存在
        if (!Directory.Exists(dirPath))
        {
            msg = "The Folder is not exists";
            return false;
        }

        //压缩文件名为空时使用 文件夹名.zip
        if (zipFilePath == string.Empty || zipFilePath == null)
        {
            if (dirPath.EndsWith("//"))
            {
                dirPath = dirPath.Substring(0, dirPath.Length - 1);
            }
            zipFilePath = dirPath + ".zip";
        }

        try
        {
            using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
            {
                s.SetLevel(this.CompressionLevel);
                ZipDirectory(dirPath, s);//压缩目录，包含子目录
                s.Finish();
                s.Close();
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return false;
        }

        msg = "Success";
        return true;
    }//end function

    /// <summary>
    /// 压缩文件指定文件
    /// </summary>
    /// <param name="filenames">要压缩文件的绝对路径</param>
    /// <param name="zipFilePath">生成压缩文件的路径, 缺省值为当前时间.zip</param>
    /// <param name="msg">返回消息</param>
    /// <returns>是否压缩成功</returns>
    public bool ZipFiles(string[] filenames, string zipFilePath, out string msg)
    {
        msg = "";
        if (filenames.Length == 0)
        {
            msg = "No File Selected";
            return false;
        }

        //压缩文件名为空时使用 文件夹名.zip
        if (zipFilePath == string.Empty || zipFilePath == null)
        {
            zipFilePath = System.Environment.CurrentDirectory + "\\" + DateTime.Now.ToFileTimeUtc().ToString() + ".zip";
        }

        try
        {
            using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
            {
                s.SetLevel(this.CompressionLevel);
                ZipFilesAndDirectory(filenames,s);
                s.Finish();
                s.Close();
            }//end using
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return false;
        }

        msg = "Success";
        return true;
    }

    #endregion

    #region 解压方法
    /// <summary>
    /// 解压ZIP格式的文件。
    /// </summary>
    /// <param name="zipFilePath">压缩文件路径</param>
    /// <param name="unZipDir">解压文件存放路径,缺省值压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
    /// <param name="msg">返回消息</param>
    /// <returns>解压是否成功</returns>
    public bool UnZipFile(string zipFilePath, string unZipDir, out string msg)
    {
        msg = "";

        //判读路径是否为空
        if (zipFilePath == string.Empty || zipFilePath == null)
        {
            msg = "ZipFile No Selected";
            return false;
        }

        //判读文件是否存在
        if (!File.Exists(zipFilePath))
        {
            msg = "ZipFile No Exists";
            return false;
        }
        //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
        if (unZipDir == string.Empty || unZipDir == null)
        {
            unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
        }

        if (!unZipDir.EndsWith("//")) 
        {
            unZipDir += "//";
        }
            
        if (!Directory.Exists(unZipDir))
        {
            Directory.CreateDirectory(unZipDir);
        }

        try
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (!directoryName.EndsWith("//"))
                    { 
                        directoryName += "//";
                    }
                    if (fileName != String.Empty && fileName != null)
                    {
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {
                            CopyStream(s, streamWriter);
                        }
                    }
                }//end while
            }//end using
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return false;
        }

        msg = "UnZip Success ! ";
        return true;
    }//end function

    /// <summary>
    /// 解压指定ZIP文件到当前路径
    /// </summary>
    /// <param name="zipFilePath">ZIP文件绝对路径</param>
    /// <param name="msg">返回消息</param>
    /// <returns>解压是否成功</returns>
    public bool UnZipFileToCurrentPath(string zipFilePath, out string msg)
    {
        msg = "";

        //判读路径是否为空
        if (zipFilePath == string.Empty || zipFilePath == null)
        {
            msg = "ZipFile No Selected";
            return false;
        }

        //判读文件是否存在
        if (!File.Exists(zipFilePath))
        {
            msg = "ZipFile No Exists";
            return false;
        }
        //解压到当前目录
        string unZipDir = zipFilePath.Substring(0, zipFilePath.LastIndexOf(@"\"));

        if (!unZipDir.EndsWith("//"))
        {
            unZipDir += "//";
        }

        if (!Directory.Exists(unZipDir))
        {
            Directory.CreateDirectory(unZipDir);
        }

        try
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (!directoryName.EndsWith("//"))
                    {
                        directoryName += "//";
                    }
                    if (fileName != String.Empty && fileName != null)
                    {
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {
                            CopyStream(s, streamWriter);
                        }
                    }
                }//end while
            }//end using
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return false;
        }

        msg = "UnZip Success ! ";
        return true;
    }//end function

    #endregion

    #region 内部方法

    /// <summary>
    /// 内部类：递归压缩目录
    /// </summary>
    /// <param name="dirPath"></param>
    /// <param name="s"></param>
    private void ZipDirectory(string dirPath, ZipOutputStream s)
    {
        string[] filenames = Directory.GetFiles(dirPath);
        byte[] buffer = new byte[this.BufferSize];
        foreach (string file in filenames)
        {
            //处理相对路径问题
            ZipEntry entry = new ZipEntry(file.Replace(this.CompressPath + "\\", String.Empty));
            //设置压缩时间
            entry.DateTime = DateTime.Now;

            s.PutNextEntry(entry);
            using (FileStream fs = File.OpenRead(file))
            {
                CopyStream(fs, s);
            }
        }

        //递归处理子目录
        DirectoryInfo di = new DirectoryInfo(dirPath);
        foreach (DirectoryInfo subDi in di.GetDirectories())
        {
            ZipDirectory(subDi.FullName, s);
        }

    }//end function

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filenames"></param>
    /// <param name="s"></param>
    private void ZipFilesAndDirectory(string[] filenames, ZipOutputStream s)
    {
        byte[] buffer = new byte[this.BufferSize];
        foreach (string file in filenames)
        {
            //当时文件的时候
            if (System.IO.File.Exists(file))
            {
                ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(file))
                {
                    CopyStream(fs, s);
                }
            }

            //当时目录的时候
            if(System.IO.Directory.Exists(file))
            {
                this.CompressPath = file.Replace("\\" + Path.GetFileName(file),String.Empty);
                ZipDirectory(file, s);
            }

        }

       
    }



    /// <summary>
    /// 按块复制
    /// </summary>
    /// <param name="source">源Stream</param>
    /// <param name="target">目标Stream</param>
    private void CopyStream(Stream source, Stream target)
    {
        byte[] buf = new byte[this.BufferSize];
        int byteRead = 0;
        while ((byteRead = source.Read(buf, 0, this.BufferSize)) > 0)
        {
            target.Write(buf, 0, byteRead);
        }

    }
    #endregion



    }//class
}
