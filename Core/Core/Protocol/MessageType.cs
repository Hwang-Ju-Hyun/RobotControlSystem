using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_lib.Core.Protocol
{
    public enum MessageType : byte
    {
        STATUS=1,
        POSITION=2,
        TASK_ASSIGN=3,
        TASK_COMPELETE=4,
        HEARTBEAT=5
    };    
}
