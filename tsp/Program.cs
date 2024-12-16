using evolution;
using Graph;


MapGraph mapGraph = new MapGraph();

mapGraph.generate_map(50);

algorithm_result result = new algorithm_result();
result = algorithm.example(mapGraph);

algorithm_result result2 = new algorithm_result();
result2 = algorithm.evolution(mapGraph, 10000, 9);


Console.WriteLine("as");