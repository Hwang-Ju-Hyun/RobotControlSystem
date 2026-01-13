using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core_lib.Core.Domain;

namespace ControlCenter
{
    
    class MainViewModel
    {
        private ObservableCollection<Node> nodes;
        public ObservableCollection<Node> Nodes { get; }
        Node[,] grid;
        public MainViewModel()
        {
            Map map = new Map(30, 30);
            Nodes = new ObservableCollection<Node>();
            CreateGrids();
        }
        public void CreateGrids()
        {
            grid = new Node[30, 30];
            for (int r = 0; r < 30; r++)
            {
                for (int c = 0; c < 30; c++)
                {
                    Node n = new Node(r, c, c * 30, r * 30);
                    n.Type = Node.NodeType.EMPTY;
                    grid[r, c] = n;
                    Nodes.Add(n);
                }
            }
        }
    }
}
