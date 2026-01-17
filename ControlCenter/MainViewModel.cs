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
using System.Windows.Threading;
using Core_lib.Core.Protocol;
using ControlCenter.Server;
using System.Collections;
using System.Xml.Linq;

namespace ControlCenter
{
    
    class MainViewModel 
    {        
        public ObservableCollection<Node> Nodes { get; }
        Node[,] grid;
        Map map;
        PathResult result=null;        
        DispatcherTimer timer;
        private ConnectedRobot selectedRobot;
        public ConnectedRobot SelectedRobot
        {
            get => selectedRobot;
            set
            {
                selectedRobot = value;                
            }
        }

        public ControlServer.Server server { get; }
        public MainViewModel()
        {
            map = new Map(30, 30);
            Nodes = new ObservableCollection<Node>();
            CreateGrids();
            server = new ControlServer.Server();            

            StartSimulationTimer();
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

        public void SetStartNode(ConnectedRobot cr,Node node)
        {
            Robot robot = RobotManager.GetInstance.GetRobot(cr.RobotID);
            robot.StartNode = node;
            
        }

        public void SetGoalNode(ConnectedRobot cr, Node node)
        {
            Robot robot = RobotManager.GetInstance.GetRobot(cr.RobotID);
            robot.GoalNode = node;
        }

        public void StartPathFinding()
        {
            if (SelectedRobot == null)
                return;

            PathFind pf = new PathFind();
            
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
                if (SelectedRobot == null)
                {
                    MessageBox.Show("로봇을 선택하세요");
                    return;
                }
                Robot robot = RobotManager.GetInstance.GetRobot(SelectedRobot.RobotID);                
                result = pf.FindPath(robot, robot.StartNode, robot.GoalNode, map);
                
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
                    RobotMessage move = new RobotMessage
                    {
                        RobotId = robot.Id,
                        Type = MessageType.MOVE,
                        Path = result.Path
                    };
                    foreach(PathNode p in result.Path)
                    {
                        RobotManager.GetInstance.EnqueuePath(move.RobotId, p);
                    }
                    
                    server.Send(ProtocolParser.ObjectToPacket(move),server.ConnectedClient[0]);
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
        private void StartSimulationTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += OnSimulationTick;
            timer.Start();
        }

        private void OnSimulationTick(object sender,EventArgs e)
        {
            List<(int, PathNode)> movedRobots = RobotManager.GetInstance.TickRobots();                                    
            foreach (var (_, node) in movedRobots)
            {                                
                grid[node.Row, node.Col].Type = Node.NodeType.ROBOT;
            }            
        }
    }
}
