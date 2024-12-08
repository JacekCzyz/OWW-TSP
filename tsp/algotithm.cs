using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Graph;
namespace evolution
{
    internal class algorithm_result
    {
        double dPathLen=0;
        List<Vertex> permutation;


        public void  example(MapGraph Graph)
        {
            permutation = new List<Vertex>();
            permutation.Add(Graph.collVertexes[0]);
            for (int i = 1; i < Graph.intAmountVertexes; i++)
            {
                dPathLen += Graph.calculate_path(Graph.collVertexes[i - 1], Graph.collVertexes[i]);
                permutation.Add(Graph.collVertexes[i]);
            }
            dPathLen += Graph.calculate_path(Graph.collVertexes[0], Graph.collVertexes[Graph.intAmountVertexes-1]);
            permutation.Add(Graph.collVertexes[0]);

        }
    }
}
