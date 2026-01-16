using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Core.Services;
using Core_lib.Core.Domain;

namespace Core_lib.Core.Protocol
{
    public class RobotMessage
    {          
        public int RobotId {  get; set; }
        public MessageType Type { get; set; }        
        public RobotState State {  get; set; }          
        public List<PathNode> Path {  get; set; }

        public RobotState GetTypeFromData(string data)
        {
            RobotState type = RobotState.IDLE;
            switch(data)
            {
                case "Idle":
                    type = RobotState.IDLE;
                    break;
                case "Moving":
                    type = RobotState.MOVING;
                    break;
                case "Waiting":
                    type = RobotState.WAITING;
                    break;
                case "End":
                    type = RobotState.END;
                    break;
            }
            return type;
        }
        public int Row {  get; set; }
        public int Col {  get; set; }
    }
}
