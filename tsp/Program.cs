using evolution;
using Graph;


MapGraph mapGraph = new MapGraph();

mapGraph.generate_map(100);

algorithm_result result = new algorithm_result();
result = algorithm.example(mapGraph);

algorithm_result result2 = new algorithm_result();
result2 = algorithm.evolution(mapGraph, 100, 4);


Console.WriteLine("as");