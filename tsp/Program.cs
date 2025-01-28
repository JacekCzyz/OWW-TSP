using evolution;
using Graph;


MapGraph mapGraph = new MapGraph();

mapGraph.generate_map(500, 4);

algorithm_result result = new algorithm_result();
result = algorithm.example(mapGraph);

algorithm_result result2 = new algorithm_result();
result2 = algorithm.evolution(mapGraph, 1000, 9, 8, 100);


Console.WriteLine("as");