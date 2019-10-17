using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace eNet编辑器
{
    [Serializable]
    public class DataJson
    {

        #region 项目执行文件key k-.json 写进主机的项目信息
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

        #region 项目执行文件scene s-.json 写进主机的项目信息
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
            public string obj { get; set; }
            public string val { get; set; }
            public string optname { get; set; }
            public int delay { get; set; }

        }
        #endregion

        #region 项目执行文件 timer t-.json 写进主机的项目信息
        [Serializable]
        public class Tn
        {
            public string type { get; set; }
            public string describe { get; set; }
            public List<Timernumber> timer = new List<Timernumber>();
            public Tn()
            {
                this.type = "timer";
                //描述
                this.describe = "create by system";
            }

        }
        [Serializable]
        public class Timernumber
        {
            public int num { get; set; }
            public int year { get; set; }
            public int mon { get; set; } 
            public int day { get; set; }
            public int date { get; set; }
            public int hour { get; set; }
            public int min { get; set; }
            public string obj { get; set; }
            public string data { get; set; }
            public string optname { get; set; }
        }
        #endregion

        #region 项目执行文件logic l-.json 写进主机的项目信息
        [Serializable]
        public class Lc
        {
            public string type { get; set; }
            public string describe { get; set; }
            public List<TriggerNumber> trigger = new List<TriggerNumber>();
            public Lc()
            {
                this.type = "logic";
                this.describe = "create by system";
            }

        }
        [Serializable]
        public class TriggerNumber
        {
            public int num { get; set; }
            public int attr { get; set; }
            public string modelType { get; set; }
            public string @switch { get; set; }//用@来做关键字为变量名
            public List<Condition> condition = new List<Condition>();
            

        }

        [Serializable]
        public class Condition
        {
            public string @case { get; set; }
            public string obj { get; set; }
            public string data { get; set; }
            public string optname { get; set; }
            public int delay { get; set; }
            public Condition()
            {
                delay = 0;
            }
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
            public int  id { get; set; }
            public string  device { get; set; }
            public string  sn{ get; set; }
            public string  ver{ get; set; }
            public string area1 { get; set; }
            public string area2 { get; set; }
            public string area3 { get; set; }
            public string area4 { get; set; }
            public string name { get; set; }
            public List<DevPort> devPortList = new List<DevPort>();
        }

        [Serializable]
        public class DevPort
        {
            public int portID { get; set; }
            public string portType { get; set; }
            public string portInterface { get; set; }
            public string  portContent { get; set; }

        }

        #region 端口内容 开关 调光 串口1000 串口2000 温感 光感
        
        [Serializable]
        public class PortSwitch 
        {
            // 01 00 ff 断电恢复
            public string powerState { get; set; }
            // 00 11 12  21 22 23  31 32 33 34  
            public string interLock { get; set; }

            public string powerStateReg { get; set; }
            public string interLockReg { get; set; }
            
            public PortSwitch()
            {
                powerStateReg = "40";
                interLockReg = "60";
                powerState = "00";
                interLock = "00";
            }
        }

        [Serializable]
        public class PortDimmer 
        {
            public string powerState { get; set; }
            public string onState { get; set; }
            public string changeState { get; set; }
            public string max { get; set; }
            public string min { get; set; }
            public string spline { get; set; }

            public string powerStateReg { get; set; }
            public string onStateReg { get; set; }
            public string changeStateReg { get; set; }
            public string maxReg { get; set; }
            public string minReg { get; set; }
            public string splineReg { get; set; }

            
            /// <summary>
            /// 寄存器通道
            /// </summary>
            public PortDimmer()
            {
                powerStateReg = "40";
                onStateReg = "60";
                changeStateReg = "61";
                maxReg = "62";
                minReg = "63";
                splineReg = "65";

            }
        }

        #endregion


        #endregion

        #region 项目辅助文件Point.json
        [Serializable]
        public class Point
        {
            public List<PointInfo> equipment = new List<PointInfo>();
            public List<PointInfo> scene = new List<PointInfo>() ;
            public List<PointInfo> timer  = new List<PointInfo>();
            public List<PointInfo> link = new List<PointInfo>();
            public List<PointInfo> virtualport = new List<PointInfo>();
            public List<PointInfo> logic = new List<PointInfo>();
            public List<PointInfo> localvar = new List<PointInfo>();
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
            //public string address { get; set; }
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
            public int delay { get; set; }
            public sceneInfo()
            {
                delay = 0;
                pid = 0;
                id = 0;
            }

        }
        #endregion

        #region 项目辅助文件timer.json
        [Serializable]
        public class Timer
        {
            public string IP { get; set; }
            public string Dev { get; set; }
            public List<timers> timers = new List<timers>();
        }
        [Serializable]
        public class timers
        {
            public int id { get; set; }
            public int pid { get; set; }
            public string dates { get; set; }//格式设置 0，1，2，3，4，5，6或者 年\月\日，年\月\日，年\月\日，年\月\日，（255为每天）
            public string priorHoloday { get; set; }//假期优先 01000001  非假期优先 00000001
            public List<timersInfo> timersInfo = new List<timersInfo>();
        }
        [Serializable]
        public class timersInfo
        {

            public int id { get; set; }
            public int pid { get; set; }
            public string type { get; set; }
            public string address { get; set; }
            public string opt { get; set; }
            public string optName { get; set; }
            public string shortTime { get; set; }
            
            
        }
        #endregion

        #region 项目辅助文件panel.json
        [Serializable]
        public class Panel
        {
            public string IP { get; set; }
            public string Dev { get; set; }
            public List<panels> panels = new List<panels>();
        }
        [Serializable]
        public class panels
        {
            //ID号限制为1-999
            public int id { get; set; }
            public int pid { get; set; }
            public int keyNum { get; set; }
            public List<panelsInfo> panelsInfo = new List<panelsInfo>();
        }
        [Serializable]
        public class panelsInfo
        {
            public int id { get; set; }
            public int pid { get; set; }
            //ip + id + ye + num
            public string keyAddress { get; set; }
            public string objAddress { get; set; }
            public string objType { get; set; }
            public int opt { get; set; }

            public string showAddress { get; set; }
            public string showMode { get; set; }
        }
        #endregion

        #region 项目辅助文件Sensor.json
        [Serializable]
        public class Sensor
        {
            public string IP { get; set; }
            public string Dev { get; set; }
            public List<sensors> sensors = new List<sensors>();
        }
        [Serializable]
        public class sensors
        {
            //ID号限制为1001-1999
            public int id { get; set; }
            public int pid { get; set; }
            public int ioNum { get; set; }
            public List<sensorsInfo> sensorsInfo = new List<sensorsInfo>();
        }
        [Serializable]
        public class sensorsInfo
        {
            public int id { get; set; }
            public int pid { get; set; }
            //ip + id + ye + num
            public string keyAddress { get; set; }
            public string objAddress { get; set; }
            public string objType { get; set; }
            public string opt { get; set; }
            public string optName { get; set; }
            //输入状态
            public int fbmode { get; set; }
        }
        #endregion

        #region 项目辅助文件Logic.json
        [Serializable]
        public class Logic
        {
            public string IP { get; set; }
            public string Dev { get; set; }
            public List<logics> logics = new List<logics>();
        }
        [Serializable]
        public class logics
        {
            
            public int id { get; set; }
            public int pid { get; set; }
            public List<logicsInfo> logicsInfo = new List<logicsInfo>();
        }

        [Serializable]
        public class logicsInfo
        {
            public int id { get; set; }
            public int attr { get; set; }//是否主动反馈 1为主动触发  0为被动触发
            public string modelType { get; set; }
            public string content { get; set; }
            //public LogicSceneContent logicSceneContent = new LogicSceneContent();
            public logicsInfo()
            {
                id = 1;
                attr = 1;
                modelType = "SceneDeal";//"SceneDeal" "ConditionDeal" "VoicelDeal"
            }

        }

        #region 内容信息

        #region 场景处理

        [Serializable]
        public class LogicSceneContent 
        {
            public int pid { get; set; }
            //public string address { get; set; }
            public List<sceneInfo> sceneInfo = new List<sceneInfo>();
        }
        #endregion

        #region 多条件处理
        [Serializable]
        public class ConditionContent
        {
            public List<ConditionInfo> conditionInfo = new List<ConditionInfo>();
            public List<sceneInfo> trueDo = new List<sceneInfo>();
            public List<sceneInfo> falseDo = new List<sceneInfo>();

        }

        [Serializable]
        public class ConditionInfo
        {
            public int id { get; set; }
            public int a { get; set; }
            public int objPid { get; set; }
            public string objAddress { get; set; }
            public string objType { get; set; }
            public int b { get; set; }
            public string operation { get; set; }
            public int c { get; set; }
            public int comparePid { get; set; }
            public string compareobjAddress { get; set; }
            public string compareobjType { get; set; }
            public int d { get; set; }
            public ConditionInfo()
            {
                a = 1;
                b = 0;
                c = 1;
                d = 0;

            }
        }
        #endregion

        #region 表达式处理
        [Serializable]
        public class VoiceContent
        {
            public string voice { get; set; }
            public string voiceAssignment { get; set; }
            public List<VoiceItem> voiceItem = new List<VoiceItem>();
            public List<VoiceIfItem> voiceIfItem = new List<VoiceIfItem>();
        }

        [Serializable]
        public class VoiceItem
        {
            public string letter { get; set; }
            public int pid { get; set; }
            public string type { get; set; }
            public string address { get; set; }
        }

        [Serializable]
        public class VoiceIfItem : sceneInfo
        {
            public string result { get; set; }
    
        }

        #endregion

        #endregion

        #endregion


        #region 项目总文件  还需要添加
        [Serializable]
        public class totalList
        {

            public string DeviceList { get; set; }//工程设备的保存记录
            public string AreaList { get; set; }//
            public string PointList { get; set; }//Title表的设备信息
            public string sceneList { get; set; }//场景
            public string timerList { get; set; }//定时
            public string panelList { get; set; }//绑定
            public string sensorList { get; set; }//感应
            public string logicList { get; set; }//逻辑
        }

        #endregion


    }


}
