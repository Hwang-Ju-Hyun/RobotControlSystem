using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Core.Domain;
using Core_lib.Core.Domain;

namespace ControlCenter.Service
{
    public class PathResult
    {
        public bool Found { get; set; }
        public IReadOnlyList<PathNode> Path { get; set; }
    }

    public struct PathNode
    {
        public int Row;
        public int Col;
    }

    public class PathFind
    {
        public PathFind() { }
        public delegate float Heuristic((float x, float y) a, (float x, float y) b);
                
        Heuristic H_Method;
        private int DigaonalCost=14;
        private int StraightCost = 10;                
        
        float Manhattan((float x, float y) a, (float x, float y) b)
        {
            return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
        }

        float GetDist((float x, float y) a, (float x, float y) b,Heuristic method)
        {
            return method(a, b);
        }

        public PathResult FindPath(Robot robot,Node start, Node end, Map map)
        {
            PriorityQueue<Node,float> open=new PriorityQueue<Node, float> ();
            
            H_Method = Manhattan;                          
            
            start.Gn = 0;
            start.Hn = 10*GetDist((start.Row, start.Col),(end.Row,end.Col), H_Method);
            start.Weight = 1.0f;
            start.Fn = (start.Hn * start.Weight) + start.Gn;
            start.Parent = null;
            open.Enqueue(start,start.Fn);

            //ClockWise from 12
            int[] dir_x = { 0, 1, 1, 1, 0, -1, -1, -1 };
            int[] dir_y = { -1, -1, 0, 1, 1, 1, 0, -1 };

            while (open.Count!=0)
            {
                Node cur=open.Dequeue();

                if (cur.Equals(end))                                
                {
                    return BuildPath(end);                    
                }
                cur.Close = true;
                cur.Weight = 1.0f;

                if (cur.Type != Node.NodeType.START && cur.Type != Node.NodeType.GOAL)
                {
                    cur.Type = Node.NodeType.CLOSE;
                }

                for (int i=0;i<8;i++)
                {
                    int next_row = cur.Row+dir_y[i];
                    int next_col = cur.Col + dir_x[i];

                    int cost = (dir_x[i] * dir_y[i] != 0) ? DigaonalCost : StraightCost;

                    if (next_row < 0 || next_col < 0)
                        continue;
                    if (next_row >= map.Row || next_col >= map.Col)
                        continue;
                    if (map.Nodes[next_row,next_col].Type == Node.NodeType.OBSTACLE)
                        continue;

                    //diagonal
                    if (dir_x[i] * dir_y[i]!=0)
                    {
                        int adj_row1 = cur.Row + dir_y[i];
                        int adj_col1 = cur.Col;

                        int adj_row2 = cur.Row;
                        int adj_col2 = cur.Col + dir_x[i];

                        if (map.Nodes[adj_row1, adj_col1].Type == Node.NodeType.OBSTACLE ||
                            map.Nodes[adj_row2, adj_col2].Type == Node.NodeType.OBSTACLE)
                        {
                            continue;
                        }                            
                    }

                    if (map.Nodes[next_row,next_col].Close == true)
                        continue;

                    Node nextNode=map.Nodes[next_row,next_col];                    

                    float tentativGn;
                    tentativGn = cur.Gn + cost;

                    if (tentativGn < nextNode.Gn)
                    {
                        nextNode.Gn = tentativGn;
                        nextNode.Hn = 10*GetDist((nextNode.Row, nextNode.Col), (end.Row, end.Col), H_Method);
                        nextNode.Weight = 1.0f;
                        nextNode.Fn = (nextNode.Hn * nextNode.Weight) + nextNode.Gn;
                        nextNode.Parent = cur;

                        open.Enqueue(nextNode, nextNode.Fn);
                        if (nextNode.Type != Node.NodeType.GOAL)
                        {
                            nextNode.Type = Node.NodeType.OPEN;
                        }
                    }
                }                
            }
            return new PathResult { Found = false };
        }
        private PathResult BuildPath(Node goal)
        {
            List<PathNode> list = new List<PathNode>();
            Node cur = goal;

            while (cur != null)
            {
                list.Add(new PathNode{Row = cur.Row,Col = cur.Col});
                cur = cur.Parent;
            }

            list.Reverse();

            return new PathResult{Found = true,Path = list};
        }

    }
}
