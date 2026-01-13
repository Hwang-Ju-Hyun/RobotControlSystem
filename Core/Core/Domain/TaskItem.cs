using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_lib.Core.Domain
{
    public enum TaskState { WAITING,ASSIGNED,RUNNING,DONE}
    public class TaskItem
    {
        public int TaskId { get; }        
        public Node TargetNode { get; set; }
        public TaskState State { get; set; }
        public int AssignRobotId {  get; set; }
        public TaskItem(Node targetNode)
        {
            this.TargetNode = targetNode;
        }
    }
}
