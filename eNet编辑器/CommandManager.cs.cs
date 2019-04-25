using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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
                FileMesege.DeviceList = (List<DataJson.Device>)CloneObject(list.DeviceList);
                FileMesege.AreaList = (List<DataJson.Area1>)CloneObject(list.AreaList);
                FileMesege.PointList = (DataJson.Point)CloneObject(list.PointList);
                FileMesege.sceneList = (List<DataJson.Scene>)CloneObject(list.sceneList);
                FileMesege.bindList = (List<DataJson.Bind>)CloneObject(list.bindList);
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
            DataJson.totalList totalList = new DataJson.totalList();
            if (FileMesege.DeviceList != null)
            {
                totalList.DeviceList = (List<DataJson.Device>)CloneObject(FileMesege.DeviceList);
            }
            if (FileMesege.AreaList != null)
            {
                totalList.AreaList = (List<DataJson.Area1>)CloneObject(FileMesege.AreaList);
            }
            if (FileMesege.PointList != null)
            {
                totalList.PointList = (DataJson.Point )CloneObject(FileMesege.PointList);
            
            }
            if (FileMesege.sceneList != null)
            {
                totalList.sceneList = (List<DataJson.Scene>)CloneObject(FileMesege.sceneList);
            }

            if (FileMesege.bindList != null)
            {
                totalList.bindList = (List<DataJson.Bind>)CloneObject(FileMesege.bindList);

            }
            return totalList;
        }

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


        public bool CanUnDo { get { return UnDoActionStack.Count != 0; } }
        public bool CanReDo { get { return ReDoActionStack.Count != 0; } }
        //public IEnumerable<Command> Actions { get { return ReDoActionStack.Reverse().Concat(UnDoActionStack); } }
    


    }//endclass
}
