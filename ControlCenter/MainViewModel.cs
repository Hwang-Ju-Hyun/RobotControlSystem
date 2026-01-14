using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Core_lib.Core.Domain;
using ControlServer;
using ControlCenter.Service;
using Core.Core.Services;
using Core.Domain;

namespace ControlCenter
{
    
    class MainViewModel 
    {        
        public ObservableCollection<Node> Nodes { get; }
        Node[,] grid;
        Map map;
        PathResult result=null;
        public ControlServer.Server server { get; }
        public MainViewModel()
        {
            map = new Map(30, 30);
            Nodes = new ObservableCollection<Node>();
            CreateGrids();
            server = new ControlServer.Server();            
        }
        public void CreateGrids()
        {
            grid = new Node[30, 30];
            for (int r = 0; r < 30; r++)
            {
                for (int c = 0; c < 30; c++)
                {
                    Node n = map.Nodes[r,c];
                    n.Type = Node.NodeType.EMPTY;
                    grid[r, c] = n;
                    Nodes.Add(n);
                }
            }  
            map.Nodes = grid;
        }

        public void StartPathFinding()
        {
            PathFind pf = new PathFind();
            Dictionary<int,Robot> d_rb=RobotManager.GetInstance.GetAllRobots();
            Node start=null,end=null;
            for(int r=0;r<map.Row;r++)
            {
                for(int c=0;c<map.Col;c++)
                {
                    if (grid[r, c].Type==Node.NodeType.START)
                    {
                        start = grid[r, c];
                    }
                    else if(grid[r, c].Type == Node.NodeType.GOAL)
                    {
                        end = grid[r, c];
                    }
                }
            }
            if (start == null)
            {
                MessageBox.Show("Start Node is not exist");
            }
            else if (end == null)
            {
                MessageBox.Show("End Node is not exist");
            }
            else
            {
                result = pf.FindPath(d_rb[1], start, end, map);
                if (result.Found)
                {
                    foreach (PathNode p in result.Path)
                    {
                        if (p.Row == start.Row &&p.Col==start.Col)
                        {
                            grid[p.Row, p.Col].Type = Node.NodeType.START;
                        }                 
                        else if(p.Row == end.Row && p.Col == end.Col)
                        {
                            grid[p.Row, p.Col].Type = Node.NodeType.GOAL;
                        }
                        else
                        {
                            grid[p.Row, p.Col].Type = Node.NodeType.PATH;
                        }
                    }
                    _=SimulateRobot();
                }                
                else
                {
                    MessageBox.Show("Can't find path");
                }
            }            
        }
        public async Task SimulateRobot()
        {
            foreach(PathNode p in result.Path)
            {
                grid[p.Row,p.Col].Type = Node.NodeType.ROBOT;

                await Task.Delay(300);
            }
        }
    }
}
