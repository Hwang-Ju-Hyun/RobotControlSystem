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
        public ObservableCollection<LogMessage> LogMessages { get; }=new ObservableCollection<LogMessage>();
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
            server.OnClientStateReceived += OnClientStateReceive;
            StartSimulationTimer();
        }

        private void OnClientStateReceive(ClientStatePacket packet)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogMessages.Add(new LogMessage
                {
                    Time=DateTime.Now,
                    Row = packet.Row,
                    ClientID = packet.ClientID,
                    Col = packet.Col,
                    State = packet.State
                });
            });
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
            
            // 1. 선택된 로봇의 도메인 모델 가져오기
            Robot robot = RobotManager.GetInstance.GetRobot(SelectedRobot.RobotID);

            // 2. 전역 그리드를 뒤지는 대신, 로봇이 이미 가진 노드 정보를 사용
            if (robot.StartNode == null || robot.GoalNode == null)
            {
                MessageBox.Show($"로봇 {robot.Id}의 시작점 또는 목적지가 설정되지 않았습니다.");
                return;
            }
                                    
            PathFind pf = new PathFind();
            result = pf.FindPath(robot, robot.StartNode, robot.GoalNode, map);
            if (result.Found)
            {
                // 3. 경로 시각화 (현재 그리드에 표시)
                foreach (PathNode p in result.Path)
                {
                    // 시작/끝 제외하고 경로 표시
                    if (grid[p.Row, p.Col].Type != Node.NodeType.START &&
                        grid[p.Row, p.Col].Type != Node.NodeType.GOAL)
                    {
                        grid[p.Row, p.Col].Type = Node.NodeType.PATH;
                    }
                }

                // 4. 해당 로봇에게 명령 전송
                RobotMessage move = new RobotMessage
                {
                    RobotId = robot.Id,
                    Type = MessageType.MOVE,
                    Path = result.Path
                };

                // 로봇 매니저의 큐에 경로 추가 (관제 화면 동기화용)
                foreach (PathNode p in result.Path)
                {
                    RobotManager.GetInstance.EnqueuePath(move.RobotId, p);
                }

                // 특정 클라이언트에게만 메시지 전송
                server.Send(ProtocolParser.ObjectToPacket(move), SelectedRobot);
            }
            map.Clear(map.Row, map.Col);
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
