using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.XPath;

namespace Core_lib.Core.Domain
{    
    public class Node: INotifyPropertyChanged
    {
        public enum NodeType
        {
            START=0,
            GOAL=1,
            EMPTY=2,
            OBSTACLE=3,
            OPEN=4,
            CLOSE=5,
            PATH=6,
            ROBOT=7
        };
        public int X {  get; set; }
        public int Y { get; set; }
        public int Row {  get; set; }
        public int Col { get; set; }
        
        public Node(int row,int col,int x=-1,int y=-1)
        {
            this.Row = row;
            this.Col = col;
            this.X = x;
            this.Y = y;
        }
        private NodeType _type; // 필드 추가
        public NodeType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type)); // 값이 바뀔 때 UI에 알림
                }
            }
        }

        public float Fn { get; set; }
        public float Hn { get; set; }
        public float Gn { get; set; }
        public float Weight {  get; set; }
        public Node Parent { get; set; }
        public bool Close {  get; set; }        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name = null)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is Node n)
            {
                return this.Row == n.Row && this.Col == n.Col;
            }                      
            else
                return false;
        }
    }
}
