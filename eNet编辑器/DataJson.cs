using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNet编辑器
{
    class DataJson 
    {

        #region 项目执行文件key k-.json
        [Serializable]
        public class Kn
        {
            public string type { get; set; }
            public List<Keynumber> key = new List<Keynumber>();
            public Kn()
            {
                this.type = "key";

            }
        }
        [Serializable]
        public class Keynumber
        {
            public int num { get; set; }
            public string key { get; set; }
            public string obj { get; set; }
            public int mode { get; set; }
            public string fback { get; set; }
            public int fbmode { get; set; }
        }
        #endregion

        #region 项目执行文件scene s-.json
        [Serializable]
        public class Sn
        {
            public string type { get; set; }
            public string describe { get; set; }
            public List<Scenenumber> action = new List<Scenenumber>();
            public Sn()
            {
                this.type = "scene";
                this.describe = "create by system";
            }

        }
        [Serializable]
        public class Scenenumber
        {
            public int num { get; set; }
            public string obj { get; set; }//改FE开头
            public string val { get; set; }
            public int delay { get; set; }

        }
        #endregion

        #region 项目执行文件 timer t-.json
        [Serializable]
        public class Tn
        {
            public string type { get; set; }
            public string describe { get; set; }
            public List<Timernumber> timer = new List<Timernumber>();
            public Tn()
            {
                this.type = "timer";
                this.describe = "create by system";
            }

        }
        [Serializable]
        public class Timernumber
        {
            public int num { get; set; }
            public int year { get; set; }
            public int mon { get; set; }
            public int date { get; set; }
            public int day { get; set; }
            public int hour { get; set; }
            public int min { get; set; }
            public string obj { get; set; }
            public string data { get; set; }

        }
        #endregion

        #region 项目主文件Point_GW.json 写进主机的项目信息
        [Serializable]
        public class Point_GW
        {
            public List<PointInfo_GW> equipment = new List<PointInfo_GW>();
            public List<PointInfo_GW> scene = new List<PointInfo_GW>();
            public List<PointInfo_GW> timer = new List<PointInfo_GW>();
            public List<PointInfo_GW> link = new List<PointInfo_GW>();

        }
        [Serializable]
        public class PointInfo_GW
        {
            public int pid { get; set; }
            public string area1 { get; set; }
            public string area2 { get; set; }
            public string area3 { get; set; }
            public string area4 { get; set; }
            public string name { get; set; } 
            public string address { get; set; }//改FE开头
            public string objType { get; set; }
            public string value { get; set; }
            public string type { get; set; }
            //public string range { get; set; }
        }



        #endregion

        #region 项目主文件device_GW.json 写进主机的项目信息
        [Serializable]
        public class Device_GW
        {
            public string master { get; set; }
            public string mask { get; set; }
            public string gateway { get; set; }
            public string mac { get; set; }
            public string sn { get; set; }
            public string ver { get; set; }
            public string location { get; set; }
            public string name { get; set; }
            public List<Module> module = new List<Module>();
        }
      

        #endregion

        #region 在线设备型号及版本序列号数据文件 serial.json
        [Serializable]
        public class Serial
        {
            public string type { get; set; }
            public List<serials> serial = new List<serials>();

        }
        [Serializable]
        public class serials
        {
            public int nid { get; set; }
            public int lid { get; set; }
            public int id { get; set; }
            public int Portnum { get; set; }
            public string serial { get; set; }
            public string version { get; set; }
            public string mac8 { get; set; }

        }

        #endregion

        #region 软件辅助文件namelib.json
       /** public class NameLib {
            public string title { get; set; }
            public List<Names> names =new List<Names>();
        }

        public class Names {
            public string name { get; set; }
        }**/
        #endregion

        #region 项目辅助文件area.json
        [Serializable]
        public class Area1 {
            public string area { get; set; }
            public string id1 { get; set; }
            public string id2 { get; set; }
            public string id3 { get; set; }
            public string id4 { get; set; }
            public List<Area2> area2 = new List<Area2>();
        }
        [Serializable]
        public class Area2 {
            public string area { get; set; }
            public string id1 { get; set; }
            public string id2 { get; set; }
            public string id3 { get; set; }
            public string id4 { get; set; }
            public List<Area3> area3 = new List<Area3>();
        }
        [Serializable]
        public class Area3 {
            public string area { get; set; }
            public string id1 { get; set; }
            public string id2 { get; set; }
            public string id3 { get; set; }
            public string id4 { get; set; }
            public List<Area4> area4 = new List<Area4>();
        }
        [Serializable]
        public class Area4
        {
            public string area { get; set; }
            public string id1 { get; set; }
            public string id2 { get; set; }
            public string id3 { get; set; }
            public string id4 { get; set; }
        }
        #endregion

        #region 项目辅助和主文件device.json   
        [Serializable]
        public class Device {
            public string master { get; set; }
            public string  ip { get; set; }
            public string  mask { get; set; }
            public string  gateway { get; set; }
            public string  mac { get; set; }
            public string  sn { get; set; }
            public string  ver { get; set; }
            public string area1 { get; set; }
            public string area2 { get; set; }
            public string area3 { get; set; }
            public string area4 { get; set; }
            public string name { get; set; }
            public List<Module> module = new List<Module>();
        }
        [Serializable]
        public class Module {
            public string  id { get; set; }
            public string  device { get; set; }
            public string  sn{ get; set; }
            public string  ver{ get; set; }
            public string area1 { get; set; }
            public string area2 { get; set; }
            public string area3 { get; set; }
            public string area4 { get; set; }
            public string name { get; set; }
        }

        #endregion

        #region 项目辅助文件Point.json 
        [Serializable]
        public class Point
        {
            public List<PointInfo> equipment = new List<PointInfo>();
            public List<PointInfo> scene = new List<PointInfo>() ;
            public List<PointInfo> timer  = new List<PointInfo>();
            public List<PointInfo> link = new List<PointInfo>();
            
        }
        [Serializable]
        public class PointInfo {
            public int pid { get; set; }
            public string area1 { get; set; }
            public string area2 { get; set; }
            public string area3 { get; set; }
            public string area4 { get; set; }
            public string name { get; set; }
            public string ip { get; set; }
            public string address { get; set; }
            public string objType { get; set; }
            public string value { get; set; }
            public string type { get; set; }
            //public string range { get; set; }
        }


        
        #endregion   

        #region 项目辅助文件scene.json
        [Serializable]
        public class Scene {
            public string IP { get; set; }
            public string Dev { get; set; }
            public List<scenes> scenes = new List<scenes>();
        }
        [Serializable]
        public class scenes {
            public int id { get; set; }
            public int pid { get; set; }
            public string address { get; set; }
            public List<sceneInfo> sceneInfo = new List<sceneInfo>();
        }
        [Serializable]
        public class sceneInfo {
            public int id { get; set; }
            public int pid { get; set; }
            public string type { get; set; }
            public string address { get; set; }
            public string opt { get; set; }
            public string optName { get; set; }
            public int Delay { get; set; }
        }
        #endregion

        #region 项目辅助文件bind.json
        [Serializable]
        public class Bind
        {
            public string IP { get; set; }
            public string Dev { get; set; }
            public List<binds> Binds = new List<binds>();
        }
        [Serializable]
        public class binds
        {
            public int id { get; set; }
            public string bindName { get; set; }
            public List<bindInfo> bindInfo = new List<bindInfo>();
        }
        [Serializable]
        public class bindInfo
        {
            //public int num { get; set; }
            public int keyId { get; set; }
            public int groupId { get; set; }
            public string objType { get; set; }
            public string address { get; set; }
            public string mode { get; set; }
            public string showType { get; set; }
            public string showAddress { get; set; }
            public string showMode { get; set; }
        }
        #endregion

        #region 项目总文件  还需要添加
        [Serializable]
        public class totalList
        {
           
            public  List<DataJson.Device> DeviceList = new List<Device>();//工程设备的保存记录
            public  List<DataJson.Area1> AreaList = new List<Area1>();//
            public DataJson.Point PointList = new Point();//Title表的设备信息
            //public  DataJson.Serial serialList;//命名在线设备文件
            public List<DataJson.Scene> sceneList = new List<Scene>();//场景
            public List<DataJson.Bind> bindList = new List<Bind>();//绑定
        }

        #endregion


    }
}
