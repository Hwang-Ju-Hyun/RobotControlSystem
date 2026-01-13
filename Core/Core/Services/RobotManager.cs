using System;
using System.Collections.Generic;
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
        public Dictionary<int, Robot> GetAllRobots()
        {
            lock(_lock)
                return robots;
        } 
    }
}
