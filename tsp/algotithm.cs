using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Graph;

namespace evolution
{
    public class algorithm_result
    {
        public double dPathLen = 0;
        public int[] permutation; //array of ints - indexes of vertexes to visit in order

    }
    public class algorithm 
    {
        public static algorithm_result example(MapGraph Graph)
        {
            algorithm_result result = new algorithm_result();
            result.permutation = new int[Graph.intAmountVertexes];
            for (int i = 0; i < Graph.intAmountVertexes; i++)
            {
                result.permutation[i] = i;
            }
            result.dPathLen = calculate_len(result.permutation, Graph);

            return result;
        }

        public static algorithm_result evolution(MapGraph Graph, int generations, int population_size, int island_amount, int migration_rate)
        {
            algorithm_result result = new algorithm_result();
            List<int[]> selected_specimen = new List<int[]>();
            int specimen_amount;
            double mutation_factor = 1.0;
            int intAmountVertexes = Graph.intAmountVertexes;

            List<List<int[]>> islands =  new List<List<int[]>>();

            for(int i=0; i < island_amount; i++)
            {
                islands.Add(generate_init_population(population_size, intAmountVertexes));
            }

            double starting_islands_mutation = 1.0;
            double ending_islands_mutation = 1.0;

            for (int j=0; j < generations/migration_rate; j++)
            {
                for(int k=0; k<island_amount;k++)
                {
                    mutation_factor = starting_islands_mutation;
                    for (int i = 0; i < migration_rate; i++)
                    {
                        selected_specimen = selection(islands[k], population_size, (int)Math.Sqrt(population_size), Graph);
                        specimen_amount = selected_specimen.Count();
                        islands[k] = generate_population(selected_specimen, (int)Math.Pow(specimen_amount, 2), intAmountVertexes, mutation_factor);
                        mutation_factor *= 0.9999;
                    }
                    ending_islands_mutation = mutation_factor;
                }
                starting_islands_mutation = ending_islands_mutation;

                if (j < generations / migration_rate - 1)
                {
                    List<int[]> migrants = new List<int[]>();
                    int migration_size = Math.Max(1, population_size / 10); //10% migruje

                    for (int i = 0; i < island_amount; i++)
                    {
                        List<int[]> selected_specimens = selection(islands[i], population_size, migration_size, Graph);
                        migrants.AddRange(selected_specimens);
                    }
                    Random random = new Random();

                    migrants = migrants.OrderBy(x => random.Next()).ToList();
                    for (int i = 0; i < island_amount; i++)
                    {
                        for (int m = 0; m < migration_size; m++)
                        {
                            islands[i].RemoveAt(islands[i].Count - 1);
                            islands[i].Add(migrants[(i * migration_size + m) % migrants.Count]);
                        }
                    }
                }
            }

            result.dPathLen = double.MaxValue;

            foreach (List<int[]> island in islands)
            {
                List<int[]> selected_specimens = selection(island, population_size, 1, Graph);
                double currentLen = calculate_len(selected_specimen[0], Graph);
                if (currentLen < result.dPathLen)
                {
                    result.dPathLen = currentLen;
                    result.permutation = selected_specimen[0];
                }
            }

            return result;
        }

        public static List<int[]> generate_init_population(int population_size, int vertex_amount)
        {
            List<int[]> new_population = new List<int[]>();
            Random random = new Random();
            for (int i = 0; i < population_size; i++)
            {
                new_population.Add(create_permutation(vertex_amount));
            }

            return new_population.OrderBy(x => random.Next()).ToList(); // Extra shuffle
        }

        public static List<int[]> generate_population(List<int[]> population, int population_size, int vertex_amount, double mutation_factor)
        {
            List<int[]> new_population = new List<int[]>();
            int[] temp_specimen = new int[vertex_amount];
            Random random = new Random();
            for (int i  = 0; i < population.Count(); i++)
            {
                for (int j = 0; j < population.Count(); j++)
                {
                    if(i!=j)
                    {
                        temp_specimen = cross_over(population[i], population[j]);
                        if (random.NextDouble() < mutation_factor)
                        {
                            mutate(temp_specimen);
                        }
                    }


                    else
                        temp_specimen = population[i];
                    new_population.Add(temp_specimen);
                }
            }
            return new_population;
        }

        public static int[] cross_over(int[] parent1, int[] parent2)
        {
            int length = parent1.Length;
            int[] child = new int[length];
            Array.Fill(child, -1); // Mark empty spots
            Random random = new Random();
            int start = random.Next(length / 3);
            int end = start + random.Next(length / 3, length - start);

            // Copy a segment from parent1
            HashSet<int> usedGenes = new HashSet<int>();
            for (int i = start; i < end; i++)
            {
                child[i] = parent1[i];
                usedGenes.Add(parent1[i]);
            }

            // Fill remaining positions from parent2 in order
            int index = 0;
            for (int i = 0; i < length; i++)
            {
                if (child[i] == -1) // Empty spot
                {
                    while (usedGenes.Contains(parent2[index])) { index++; }
                    child[i] = parent2[index++];
                }
            }

            return child;
        }
        public static void mutate(int[] specimen)
        {
            Random random = new Random();
            int start = random.Next(specimen.Length / 2);
            int end = start + random.Next(2, specimen.Length / 2);

            List<int> sublist = specimen.Skip(start).Take(end - start).ToList();
            sublist = sublist.OrderBy(x => random.Next()).ToList(); // Shuffle

            for (int i = start; i < end; i++)
            {
                specimen[i] = sublist[i - start];
            }
        }
        public static List<int[]> selection(List<int[]> population, int population_size, int amount_to_select, MapGraph Graph)
        {
            List<int[]> selected_items = new List<int[]>();

            List<(double fitness, int[] specimen)> fitnessList = new List<(double, int[])>();
            double totalFitness = 0;

            for (int i = 0; i < population.Count; i++)
            {
                double pathLength = calculate_len(population[i], Graph);
                double fitness = 1 / (pathLength + 1);
                fitnessList.Add((fitness, population[i]));
                totalFitness += fitness;
            }

            Random random = new Random();
            for (int i = 0; i < amount_to_select; i++)
            {
                double randomValue = random.NextDouble() * totalFitness;
                double cumulativeFitness = 0;

                foreach (var item in fitnessList)
                {
                    cumulativeFitness += item.fitness;
                    if (cumulativeFitness >= randomValue)
                    {
                        selected_items.Add(item.specimen);
                        break;
                    }
                }
            }

            return selected_items;
        }

        public static double calculate_len(int[] permutation, MapGraph Graph)
        {
            double path = 0;

            for (int i = 0; i < permutation.Length - 1; i++)
            {
                path += Graph.calculate_path(
                    Graph.collVertexes[permutation[i]],
                    Graph.collVertexes[permutation[i + 1]]
                );
            }

            path += Graph.calculate_path(
                Graph.collVertexes[permutation[permutation.Length - 1]],
                Graph.collVertexes[permutation[0]]
            );

            return path;
        }


        public static int[] create_permutation(int amount)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < amount; i++)
            {
                numbers.Add(i);
            }

            Random rng = new Random();
            for (int i = numbers.Count - 1; i > 0; i--)
            {
                int randomIndex = rng.Next(i + 1);
                int temp = numbers[i];
                numbers[i] = numbers[randomIndex];
                numbers[randomIndex] = temp;
            }
            return numbers.ToArray();
        }
    }
}
