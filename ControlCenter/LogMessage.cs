using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core_lib.Core.Domain;

namespace ControlCenter
{
    public class LogMessage
    {
        public DateTime Time { get; set; }
        public string ClientID {  get; set; }
        public int Row {  get; set; }
        public int Col { get; set; }
        public string State { get; set; }
        public override string ToString()
        {
            return $"[{Time:HH:mm:ss}] {ClientID} | ({Row},{Col}) | {State}";
        }
    }
}
