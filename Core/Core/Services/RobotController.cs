using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;
using Core_lib.Core.Domain;
using Core_lib.Core.Protocol;

namespace Core.Core.Services
{
    public class PathNode
    {
        public int Row{ get; set; }
        public int Col { get; set; }
    }
    public class RobotController
    {
        public bool HasPath => CurrentPath.Count > 0;

        public Queue<PathNode> CurrentPath = new Queue<PathNode>();
        public PathNode Tick()
        {
            if (CurrentPath.Count == 0)
                return new PathNode { Row = -1, Col = -1 };

            return CurrentPath.Dequeue();
        }                  
    } 
}