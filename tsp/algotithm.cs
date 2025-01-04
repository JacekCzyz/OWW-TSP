using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static algorithm_result evolution(MapGraph Graph, int generations, int population_size)
        {
            algorithm_result result = new algorithm_result();
            List<int[]> selected_specimen = new List<int[]>();
            int specimen_amount;
            double mutation_factor = 1.0;
            int intAmountVertexes = Graph.intAmountVertexes;

            List<int[]> population = generate_init_population(population_size, intAmountVertexes);

            for (int i=0; i < generations; i++)
            {
                selected_specimen = selection(population, population_size, (int)Math.Sqrt(population_size), Graph);
                specimen_amount = selected_specimen.Count();
                population = generate_population(selected_specimen, (int)Math.Pow(specimen_amount, 2), intAmountVertexes, mutation_factor, 4);
                mutation_factor *= 0.99;
            }

            result.dPathLen = calculate_len(selected_specimen[0], Graph);
            result.permutation = selected_specimen[0];

            return result;
        }

        public static List<int[]> generate_init_population(int population_size, int vertex_amount)
        {
            List<int[]> new_population = new List<int[]>();

            for (int i = 0; i < population_size; i++)
            {
                new_population.Add(create_permutation(vertex_amount));
            }

            return new_population;
        }

        public static List<int[]> generate_population(List<int[]> population, int population_size, int vertex_amount, double mutation_factor, int maxDegreeOfParallelism)
        {
            List<int[]> new_population = new List<int[]>();
            //int[] temp_specimen = new int[vertex_amount];
            Random random = new Random();
            object lockObj = new object();
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            Parallel.ForEach(population, options, specimen1 =>
            {
                foreach (var specimen2 in population)
                {
                    int[] temp_specimen;
                    if (specimen1 != specimen2)
                    {
                        temp_specimen = cross_over(specimen1, specimen2);
                        if (random.NextDouble() < mutation_factor)
                        {
                            mutate(temp_specimen);
                        }
                    }
                    else
                    {
                        temp_specimen = specimen1;
                    }

                    lock (lockObj)
                    {
                        new_population.Add(temp_specimen);
                    }
                }
            });
            return new_population;
        }

        public static int[] cross_over(int[] specimen1, int[] specimen2)
        {
            int[] result = new int[specimen1.Count()];

            Array.Copy(specimen1, 0, result, 0, specimen1.Length / 2);
            Array.Copy(specimen2, specimen1.Length / 2, result, specimen1.Length / 2, specimen1.Length - specimen1.Length / 2);
            //for(int i=0; i < (specimen1.Count()/2); i++)
            //{
            //    result[i]= specimen1[i];
            //}
            //for (int j = (specimen1.Count() / 2); j < specimen1.Count(); j++)
            //{
            //    result[j] = specimen2[j];
            //}
            return result;
        }
        public static void mutate(int[] specimen)
        {
            int temp;
            Random rand = new Random();
            int a = rand.Next(0, specimen.Length);
            int b = rand.Next(0, specimen.Length);
            while(a!=b)
            {
                b = rand.Next(0, specimen.Length);
            }
            temp = specimen[a];
            specimen[a] = specimen[b];
            specimen[b] = temp;
        }
        public static List<int[]> selection(List<int[]> population, int population_size, int amount_to_select, MapGraph Graph)
        {
            List<(double length, int index)> collIndexedLengths = new List<(double, int)>();
            List<int[]> selected_items = new List<int[]>();

            for (int i = 0; i < population.Count; i++)
            {
                collIndexedLengths.Add((calculate_len(population[i], Graph), i));
            }

            collIndexedLengths = collIndexedLengths.OrderBy(x => x.length).ToList();

            int[] indexes_to_return = new int[amount_to_select];
            for (int i = 0; i < amount_to_select; i++)
            {
                indexes_to_return[i] = collIndexedLengths[i].index;
            }

            foreach (int index in indexes_to_return)
            {
                selected_items.Add(population[index]);
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
