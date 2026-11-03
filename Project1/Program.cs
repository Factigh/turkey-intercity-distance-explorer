using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Runtime.CompilerServices;

namespace Project1
{
    internal class Program
    {

        static Random rand = new Random();

        static string[,] getMatrix() {
            string path = "ilmesafe.csv";

            try {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"File not found: {path}");
                    return null;
                }

            string[] lines = File.ReadAllLines(path);

                if (lines.Length == 0) {
                    Console.WriteLine("File is empty");
                    return null;
                }


            int rows = lines.Length;
            int cols = lines[0].Split(';').Length;

            string[,] matrix = new string[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] values = lines[i].Split(';');
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = values[j];

                }
            }
            return matrix;

        }
        catch (Exception err){
                Console.WriteLine("An error occurred"+err.Message);
                return null;

            }
        }

        static void travelRandom(String[,] matrix,int cityCount,Random random)
        {
            List<int> choosens = new List<int>();
            int totalDist = 0;

            
            while (choosens.Count < cityCount){

                int cityIndex = random.Next(matrix.GetLength(0));
                if (!choosens.Contains(cityIndex))
                    choosens.Add(cityIndex);
            }

            Console.WriteLine("Random travel path:");
            foreach (int i in choosens)
                Console.Write($"-> {matrix[i, 0]} "); 
            Console.WriteLine();

            
            for (int i = 0; i < choosens.Count - 1; i++)
            {
                int prevCity = choosens[i];
                int nextCity = choosens[i + 1];

                
                int distance = int.Parse(matrix[prevCity,nextCity+1]); 

                Console.WriteLine($"{matrix[prevCity, 0]} -> {matrix[nextCity, 0]} : {distance} km");
                totalDist += distance;
            }

            Console.WriteLine($"\nTotal Distance: {totalDist} km");

        }

        static Dictionary<string,List<string>> showNeighbors()
        {
            string path = "komsutext.txt";
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"File not found: {path}");
                    return null;
                }               
                string[] lines = File.ReadAllLines(path);
                if (lines.Length == 0)
                {
                    Console.WriteLine("File is empty");
                    return null;
                }
                Dictionary<string, List<string>> neighborTable = new Dictionary<string, List<string>>();

                foreach (string line in lines)
                {
                    string[] words = line.Split(',');
                    string city = words[0];

                    List<string> neighbors = new List<string>();

                    for (int i = 1; i < words.Length; i++)
                    {
                        neighbors.Add(words[i]);
                    }
                    neighborTable[city] = neighbors;

                }
                return neighborTable;

            }
            catch (Exception err)
            {
                Console.WriteLine("An error occurred" + err.Message);
                    return null;
            }
            
        }

        static void farthestNeighbors(Dictionary<string, List<string>> neighborTable, string[,] matrix, string[] cityNames)
        {
            string city1 = "";
            string city2 = "";
            int maxDist = -1;


            for (int i = 1; i < cityNames.Length; i++)
            {
                string currentCity = cityNames[i];

                string key = null;
                foreach (string k in neighborTable.Keys)
                {
                    if (k.Equals(currentCity,StringComparison.OrdinalIgnoreCase))
                    {
                        key = k;
                        break;
                    }
                }

                if (key == null) continue; 



                foreach (string neighbor in neighborTable[key])
                {
                    
                    int j = -1;
                    for (int k = 1; k < cityNames.Length; k++)
                    {
                        if (cityNames[k].Equals(neighbor,StringComparison.OrdinalIgnoreCase))
                        {
                            j = k;
                            break;
                        }
                    }
                    if (j == -1) continue;
                    if (i == j) continue;

                    int distance = int.Parse(matrix[i - 1, j]);
                   
                    if (distance > maxDist)
                    {
                        maxDist = distance;
                        city1 = currentCity;
                        city2 = neighbor;
                    }
                }

                
            }
            Console.WriteLine($"Farthest city pair distance: {city1}-{city2}: {maxDist} km");
        }

        static int[] distancesToIzmir()
        {
            string path = "81il Izmır.txt";
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"File not found: {path}");
                    return null;
                }
                string[] lines = File.ReadAllLines(path);
                if (lines.Length == 0)
                {
                    Console.WriteLine("File is empty");
                    return null;
                }

                int[] array = new int[lines.Length];

                for (int i = 0; i < lines.Length; i++)
                {

                    array[i] = int.Parse(lines[i]);

                }
                return array;
            }

            catch (Exception err)
            {
                Console.WriteLine("An error occurred" + err.Message);
                return null;
            }

        }


        static void travelToIzmir(Random random,string[,] matrix, int[] IzmirArray, Dictionary<string, List<string>> neighborTable)
        {
            int totalDist = 0;
            int cityIndex = random.Next(matrix.GetLength(0));
            while (cityIndex == 34)
            {
                cityIndex=random.Next(matrix.GetLength(0));
            }
            int indexofClosest = -1;

            while (indexofClosest != 34)
            {
                int minDist = IzmirArray[cityIndex];
                
                string cityName = matrix[cityIndex, 0];
                string[] neighborArray = { };
                foreach (KeyValuePair<string, List<string>> pair in neighborTable)
                {

                    if (pair.Key == cityName)
                    {
                        neighborArray = new string[pair.Value.Count];
                        for (int i = 0; i < neighborArray.Length; i++)
                        {
                            neighborArray[i] = pair.Value[i];
                        }
                        break;

                    }

                }

                for (int i = 0; i < neighborArray.Length; i++)
                {

                    int indexofNeighbor = -1;
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        if (neighborArray[i] == matrix[j, 0])
                        {
                            indexofNeighbor = j;
                            break;
                        }
                    }
                    if (IzmirArray[indexofNeighbor] < minDist)
                    {
                        minDist = IzmirArray[indexofNeighbor];
                        indexofClosest = indexofNeighbor;
                    }
                }

                int distance= int.Parse(matrix[cityIndex, indexofClosest + 1]);
                Console.Write($"{cityName}->");
                totalDist += distance;
                cityIndex = indexofClosest;
            }
            
            Console.WriteLine($"{matrix[indexofClosest, 0]}\nDistance to İzmir: {totalDist} km");

        }

        static void Main(string[] args)
        {
            String[,] matrix = getMatrix();
            String[] cityNames = new String[matrix.GetLength(0)+1];
            cityNames[0] = "";
            for (int i = 1; i < cityNames.Length; i++)
            {
                cityNames[i] = matrix[i-1, 0];
            }
            travelRandom(matrix, 10, rand);
            Dictionary<string, List<string>> neighborTable = showNeighbors();
            Console.WriteLine("\nNeighbor Cities:");
           foreach (KeyValuePair<string, List<string>> pair in neighborTable)
            {
                Console.Write(pair.Key + ": ");
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    Console.Write(pair.Value[i] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            farthestNeighbors(neighborTable,matrix,cityNames);
            int[] IzmirArray = distancesToIzmir();
            Console.WriteLine();
            travelToIzmir(rand,matrix,IzmirArray,neighborTable);
         
        }
    }
}