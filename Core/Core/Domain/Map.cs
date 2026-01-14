using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_lib.Core.Domain
{
    public class Map
    {
        public int Row {  get; set; }
        public int Col { get; set; }

        public Node[,] Nodes { get; set; }

        public Map(int row,int col)
        {
            this.Row= row;
            this.Col= col;

            Nodes = new Node[row,col];

            for(int r=0;r<row;r++)
            {
                for(int c=0;c<col;c++)
                {
                    Nodes[r, c] = new Node(r,c,c*col,r*row);
                    Nodes[r, c].Gn = float.MaxValue;
                    Nodes[r, c].Hn = 0;
                    Nodes[r, c].Fn = 0;
                    Nodes[r, c].Close = false;
                }
            }
        }

        public IEnumerable<Node> GetNeighbors(Node node)
        {
            //12 -> 11 Clockwise
            int[] dir_r = { 0, 1, 1,  1,  0,  -1,  -1,  -1};
            int[] dir_c = {-1,-1, 0,  1,  1,   1,   0,  -1};           

            for(int i=0;i<8;i++)
            {
                int next_row = dir_r[i] + node.Row;
                int next_col = dir_r[i] + node.Col;

                if ((next_row >= 0 &&next_col >= 0)&&
                    (next_row<this.Row&&next_col<this.Col))
                {
                    yield return Nodes[next_row, next_col];
                }                
            }

        }
    }
}
