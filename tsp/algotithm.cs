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
            result.permutation = new int[Graph.intAmountVertexes+1];
            result.permutation[0] = 0;
            for (int i = 1; i < Graph.intAmountVertexes; i++)
            {
                result.dPathLen += Graph.calculate_path(Graph.collVertexes[i - 1], Graph.collVertexes[i]);
                result.permutation[i] = i;
            }
            result.dPathLen += Graph.calculate_path(Graph.collVertexes[0], Graph.collVertexes[Graph.intAmountVertexes-1]);
            result.permutation[Graph.intAmountVertexes] = 0;

            return result;
        }


        public static List<int[]> generate_population(int population_size, int vertex_amount)
        {
            List<int[]> population = new List<int[]>();

            for (int i = 0; i < population_size; i++)
            {
                population.Add(create_permutation(vertex_amount));
            }

            return population;
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
            for (int i = 1; i < Graph.intAmountVertexes; i++)
            {
                path += Graph.calculate_path(Graph.collVertexes[permutation[i]], Graph.collVertexes[permutation[Graph.intAmountVertexes - 1]]);
            }

            path += Graph.calculate_path(Graph.collVertexes[permutation[0]], Graph.collVertexes[permutation[Graph.intAmountVertexes - 1]]);
        
            return path;
        }

        public static algorithm_result evolution(MapGraph Graph, int generations, int population_size)
        {
            algorithm_result result = new algorithm_result();

            List<int[]> population = generate_population(population_size, Graph.intAmountVertexes);

            List<int[]> selected_specimen = selection(population, population_size, 1, Graph);

            //only one for now
            result.dPathLen = calculate_len(selected_specimen[0],Graph);
            result.permutation = selected_specimen[0];

            return result;
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

            int[] result = new int[amount + 1];
            for (int i = 0; i < amount; i++)
            {
                result[i] = numbers[i];
            }

            result[result.Length - 1] = result[0];

            return result;
        }

    }
}
