using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;
using Core_lib.Core.Domain;
using Core_lib.Core.Protocol;

namespace Core.Core.Services
{
    public class RobotManager
    {
        private static readonly Dictionary<int, Robot> robots=new Dictionary<int, Robot>();        
        private static readonly object _lock =new object();
        private RobotManager() { }
        private static RobotManager getInstance=null;
        
        public static RobotManager GetInstance
        {
            get
            {
                lock(_lock)
                {
                    if (getInstance == null)
                    {
                        getInstance = new RobotManager();
                    }
                    return getInstance;
                }                
            }
        }
        public void UpdateStatus(int robotId, RobotState status)
        {
            //비동기이지만 사실 싱글스레드가 아님
            //스레드풀에서 돌아기때문에 비동기 메서드에서 이런 Lock없이 로직을 부를경우 충돌유발
            lock(_lock)
            {                
                if (robots.ContainsKey(robotId)==false)
                {
                    robots.Add(robotId, new Robot(robotId));                    
                }
                robots[robotId].CurrentState = status;
            }                
        }                
        public PathNode EnqueuePath(int robotId, PathNode node)
        {            
            if (!robots.TryGetValue(robotId, out var robot))
            {
                robot = new Robot(robotId);             
                robots.Add(robotId, robot);                
            }
            robots[robotId].CurrentPath.Enqueue(node);
            return node;
        }
        public Robot GetRobot(int robotID)
        {
            return robots[robotID];
        }
        public List<(int robotId, PathNode node)> TickRobots()
        {
            List<(int, PathNode)> moved = new List<(int, PathNode)>();

            lock (_lock)
            {
                foreach (var robot in robots.Values)
                {
                    if(robot.CurrentPath.Count != 0)
                    {
                        PathNode next = robot.CurrentPath.Dequeue();
                        moved.Add((robot.Id, next));
                    }                    
                }
            }

            return moved;
        }

    }
}
