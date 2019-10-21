using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace eNet编辑器
{
    /// <summary>
    /// undo redo的工具类  infos为传递的参数 
    /// </summary>
    class CommandManager
    {
        #region Command定义
        public class Command
        {
            //新的信息
            internal DataJson.totalList totalList;
            //旧的信息
            internal DataJson.totalList unDototalList;


            internal Command(DataJson.totalList totalList, DataJson.totalList unDototalList)
            {
                this.totalList = totalList;
                this.unDototalList = unDototalList;
                
                
            }

            internal void Do() {
                updateInfos(this.totalList);
            }
            internal void UnDo() {
                updateInfos(this.unDototalList);
            }

            internal void updateInfos(DataJson.totalList list)
            {
                ////////////////////////////////后期还需要添加补充完整/////////////////////////////////

                if (!string.IsNullOrEmpty(list.DeviceList))
                {
                    FileMesege.DeviceList = JsonConvert.DeserializeObject<List<DataJson.Device>>(list.DeviceList);
                }
                else
                {
                    FileMesege.DeviceList = new List<DataJson.Device>();
                }
                if (!string.IsNullOrEmpty(list.AreaList))
                {
                    FileMesege.AreaList = JsonConvert.DeserializeObject<List<DataJson.Area1>>(list.AreaList);

                }
                else
                {
                    FileMesege.AreaList = new List<DataJson.Area1>();
                }
                if (!string.IsNullOrEmpty(list.PointList))
                {
                    FileMesege.PointList = JsonConvert.DeserializeObject<DataJson.Point>(list.PointList);

                }
                else
                {
                    FileMesege.PointList = new DataJson.Point();
                }
                if (!string.IsNullOrEmpty(list.sceneList))
                {
                    FileMesege.sceneList = JsonConvert.DeserializeObject<List<DataJson.Scene>>(list.sceneList);

                }
                else
                {
                    FileMesege.sceneList = new List<DataJson.Scene>();
                }
                if (!string.IsNullOrEmpty(list.timerList))
                {
                    FileMesege.timerList = JsonConvert.DeserializeObject<List<DataJson.Timer>>(list.timerList);

                }
                else
                {
                    FileMesege.timerList = new List<DataJson.Timer>();
                }
                if (!string.IsNullOrEmpty(list.panelList))
                {
                    FileMesege.panelList = JsonConvert.DeserializeObject<List<DataJson.Panel>>(list.panelList);

                }
                else
                {
                    FileMesege.panelList = new List<DataJson.Panel>();
                }
                if (!string.IsNullOrEmpty(list.sensorList))
                {
                    FileMesege.sensorList = JsonConvert.DeserializeObject<List<DataJson.Sensor>>(list.sensorList);

                }
                else
                {
                    FileMesege.sensorList = new List<DataJson.Sensor>();
                }

                if (!string.IsNullOrEmpty(list.logicList))
                {
                    FileMesege.logicList = JsonConvert.DeserializeObject<List<DataJson.Logic>>(list.logicList);

                }
                else
                {
                    FileMesege.logicList = new List<DataJson.Logic>();
                }
       
            }
           
        }
        #endregion

        public Stack<Command> ReDoActionStack { get; private set; }
        public Stack<Command> UnDoActionStack { get; private set; }

        public CommandManager()
        {
            ReDoActionStack = new Stack<Command>();
            UnDoActionStack = new Stack<Command>();
        }

        public void DoNewCommand(DataJson.totalList totalList, DataJson.totalList unDototalList)
        {
            var cmd = new Command(totalList,unDototalList);
            if (UnDoActionStack.Count > 2)
            {
                var cmd3 = UnDoActionStack.Pop();
                var cmd2 = UnDoActionStack.Pop();
                UnDoActionStack.Pop();
                UnDoActionStack.Push(cmd2);
                UnDoActionStack.Push(cmd3);
            }
            UnDoActionStack.Push(cmd);
            ReDoActionStack.Clear();
            cmd.Do();
        }

        /// <summary>
        /// 把上一个添加的记录 出栈并删除
        /// </summary> 
        public void RemoveLast()
        {
            if (UnDoActionStack.Count > 0)
            {
                UnDoActionStack.Pop();
            }
        }

        public void UnDo()
        {
            if (!CanUnDo)
                return;

            var cmd = UnDoActionStack.Pop();
            ReDoActionStack.Push(cmd);
            cmd.UnDo();
        }

        public void ReDo()
        {
            if (!CanReDo)
                return;

            var cmd = ReDoActionStack.Pop();
            UnDoActionStack.Push(cmd);
            cmd.Do();
        }

        
        public  DataJson.totalList getListInfos()
        {
            ////////////////////////////////后期还需要添加补充完整/////////////////////////////////
            DataJson.totalList totalList = new DataJson.totalList();
            if (FileMesege.DeviceList != null)
            {
                totalList.DeviceList = JsonConvert.SerializeObject(FileMesege.DeviceList);
               
            }
            if (FileMesege.AreaList != null)
            {
                totalList.AreaList = JsonConvert.SerializeObject(FileMesege.AreaList);
            }
            if (FileMesege.PointList != null)
            {
                totalList.PointList = JsonConvert.SerializeObject(FileMesege.PointList);
            
            }
            if (FileMesege.sceneList != null)
            {
                totalList.sceneList = JsonConvert.SerializeObject(FileMesege.sceneList);
            }

            if (FileMesege.timerList != null)
            {
                totalList.timerList = JsonConvert.SerializeObject(FileMesege.timerList);
            }

            if (FileMesege.panelList != null)
            {
                totalList.panelList = JsonConvert.SerializeObject(FileMesege.panelList);

            }
            if (FileMesege.sensorList != null)
            {
                totalList.sensorList = JsonConvert.SerializeObject(FileMesege.sensorList);

            }
            if (FileMesege.logicList != null)
            {
                totalList.logicList = JsonConvert.SerializeObject(FileMesege.logicList);

            }

            return totalList;
        }

        
       /// <summary>
       /// 普通的对象可以克隆 有object的不行
       /// </summary>
       /// <param name="obj"></param>
       /// <returns></returns>
        public static object CloneObject(object obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter(null,
                  new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(memStream, obj);
                memStream.Seek(0, SeekOrigin.Begin);
                return binaryFormatter.Deserialize(memStream);
            }
        }

        /// <summary>
        /// 父类对象赋值给子类对象
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TChild AutoCopy<TParent, TChild>(TParent parent) where TChild : TParent, new()
        {
            TChild child = new TChild();
            var ParentType = typeof(TParent);
            var Properties = ParentType.GetProperties();
            foreach (var Propertie in Properties)
            {
                //循环遍历属性
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    //进行属性拷贝
                    Propertie.SetValue(child, Propertie.GetValue(parent, null), null);
                }
            }
            return child;
        }
       

        public bool CanUnDo { get { return UnDoActionStack.Count != 0; } }
        public bool CanReDo { get { return ReDoActionStack.Count != 0; } }
        //public IEnumerable<Command> Actions { get { return ReDoActionStack.Reverse().Concat(UnDoActionStack); } }
    


    }//endclass
}
