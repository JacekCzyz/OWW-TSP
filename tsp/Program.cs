using evolution;
using Graph;

class Program
{
    static void Main(string[] args)
    {
        // Parametry ewolucji
        int generations = 1000;
        int population_size = 50;
        int island_amount = 4;
        int migration_rate = 10;
        int num_threads = 4; // Liczba wątków (1 dla sekwencyjnego, >1 dla równoległego)

        // Tworzenie grafu
        MapGraph mapGraph = new MapGraph();
        mapGraph.generate_map(500, 4); // 500 wierzchołków, 4 wątki

        Console.WriteLine($"Wygenerowano graf z {mapGraph.intAmountVertexes} wierzchołkami.");

        // Uruchomienie przykładowej metody
        algorithm_result result = algorithm.example(mapGraph);
        Console.WriteLine($"Przykładowa długość ścieżki: {result.dPathLen}");

        // Wybór metody ewolucji (sekwencyjna lub równoległa)
        algorithm_result result2;
        if (num_threads > 1)
        {
            // Tryb równoległy
            result2 = algorithm.evolution_parallel(mapGraph, generations, population_size, island_amount, migration_rate, num_threads);
            Console.WriteLine($"Długość najlepszej ścieżki po równoległej ewolucji: {result2.dPathLen}");
        }
        else
        {
            // Tryb sekwencyjny
            result2 = algorithm.evolution_sequential(mapGraph, generations, population_size, island_amount, migration_rate);
            Console.WriteLine($"Długość najlepszej ścieżki po sekwencyjnej ewolucji: {result2.dPathLen}");
        }

        // Uruchomienie eksperymentów
        algorithm.RunExperiments(mapGraph, generations, population_size, island_amount, migration_rate);

        Console.WriteLine("Koniec programu.");
    }
}
