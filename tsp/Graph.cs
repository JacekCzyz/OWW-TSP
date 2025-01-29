using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph
{
    public class MapGraph
    {
        public int intAmountVertexes {  get; set; }
        public List<Vertex> collVertexes { get; set; }

        public void generate_map(int intAmount, int maxDegreeOfParallelism)
        { 
            intAmountVertexes = intAmount;
            collVertexes = new List<Vertex>();
            Random random = new Random();

            ParallelOptions options = new ParallelOptions{
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };
        
            Parallel.For(0, intAmount, options, i =>
            {
                lock (collVertexes)
                {
                    collVertexes.Add(new Vertex
                    {
                        x = random.Next(0, 100),
                        y = random.Next(0, 100)
                    });
                }
            });
        }

        public void generate_map_seq(int intAmount)
        {
            intAmountVertexes = intAmount;
            collVertexes = new List<Vertex>();
            Random random = new Random();
            for (int i = 0; i < intAmount; i++)
            {
                collVertexes.Add(new Vertex
                {
                    x = random.Next(0, 100),
                    y = random.Next(0, 100)
                });
            }
        }

            public double calculate_path(Vertex vertexA, Vertex vertexB)
        {
            double path = Math.Round(Math.Pow(vertexA.x - vertexB.x, 2) + Math.Pow(vertexA.y - vertexB.y, 2));
            return path;
        }
    }
    public class Vertex
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vertex predecessor;

    }
}
