using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core_lib.Core.Domain;

namespace Core.Domain
{
    public class Robot
    {
        public int Id { get; set; }
        public Node CurrentNode { get; set; }
        public RobotState CurrentState { get; set; }

        public Robot(int id, Node startNode=null)
        {
            Id = id;
            CurrentNode = startNode;
            CurrentState = RobotState.IDLE;
        }   
    }
}
