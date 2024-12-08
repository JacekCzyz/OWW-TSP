using evolution;
using Graph;


MapGraph mapGraph = new MapGraph();

mapGraph.generate_map(100);

algorithm_result result = new algorithm_result();
result.example(mapGraph);

Console.WriteLine("as");