using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static string[] results = new string[50];
        static char key;
        static Tuple<string, string> names;
        static ConsolePrinter printer = new ConsolePrinter();
        static string chuckNorrisApiBaseAddress;
        static string nameGeneratorApiEndpoint;
        static string categoriesEndpoint;
        static string randomJokesEndpoint;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("configsettings.json", optional: true, reloadOnChange: true);

            chuckNorrisApiBaseAddress = builder.Build().GetSection("ApiEndpoints").GetSection("ChuckNorrisApi").GetSection("BaseAddress").Value;
            nameGeneratorApiEndpoint = string.Concat(builder.Build().GetSection("ApiEndpoints").GetSection("NameGeneratorApi").GetSection("BaseAddress").Value, "/",
                                                        builder.Build().GetSection("ApiEndpoints").GetSection("NameGeneratorApi").GetSection("ApiEndpoint").Value);
            categoriesEndpoint = builder.Build().GetSection("ApiEndpoints").GetSection("ChuckNorrisApi").GetSection("CategoriesEndpoint").Value;
            randomJokesEndpoint = builder.Build().GetSection("ApiEndpoints").GetSection("ChuckNorrisApi").GetSection("RandomJokeEndpoint").Value;

            printer.Value("Press ? to get instructions.").ToString();
            if (Console.ReadLine() == "?")
            {
                while (true)
                {
                    printer.Value("Press c to get categories").ToString();
                    printer.Value("Press r to get random jokes").ToString();
                    GetEnteredKey(Console.ReadKey());
                    
                    if (key == 'c')
                    {
                        GetCategories(chuckNorrisApiBaseAddress, categoriesEndpoint);
                        PrintResults();
                    }
                    else if (key == 'r')
                    {
                        printer.Value("Want to use a random name? y/n").ToString();
                        GetEnteredKey(Console.ReadKey());
                        if (key == 'y')
                            GetNames(nameGeneratorApiEndpoint);
                        printer.Value("Want to specify a category? y/n").ToString();
                        GetEnteredKey(Console.ReadKey());
                        if (key == 'y')
                        {
                            printer.Value("How many jokes do you want? (1-9)").ToString();
                            int n = Int32.Parse(Console.ReadLine());
                            printer.Value("Enter a category;").ToString();
                            GetRandomJokes(Console.ReadLine(), n, chuckNorrisApiBaseAddress, randomJokesEndpoint);
                            PrintResults();
                        }
                        else
                        {
                            printer.Value("How many jokes do you want? (1-9)").ToString();
                            int n = Int32.Parse(Console.ReadLine());
                            GetRandomJokes(null, n, chuckNorrisApiBaseAddress, randomJokesEndpoint);
                            PrintResults();
                        }
                    }
                    else
                    {
                        printer.Value("You have pressed an Invalid Key. Please try again.").ToString();
                    }
                    names = null;
                    key = '\0';
                }
            }

        }

        private static void PrintResults()
        {
            printer.Value(Environment.NewLine + "[" + string.Join(",", results) + "]").ToString();
        }

        private static void GetEnteredKey(ConsoleKeyInfo consoleKeyInfo)
        {
            switch (consoleKeyInfo.Key)
            {
                case ConsoleKey.C:
                    key = 'c';
                    break;
                case ConsoleKey.D0:
                    key = '0';
                    break;
                case ConsoleKey.D1:
                    key = '1';
                    break;
                case ConsoleKey.D2:
                    key = '2';
                    break;
                case ConsoleKey.D3:
                    key = '3';
                    break;
                case ConsoleKey.D4:
                    key = '4';
                    break;
                case ConsoleKey.D5:
                    key = '5';
                    break;
                case ConsoleKey.D6:
                    key = '6';
                    break;
                case ConsoleKey.D7:
                    key = '7';
                    break;
                case ConsoleKey.D8:
                    key = '8';
                    break;
                case ConsoleKey.D9:
                    key = '9';
                    break;
                case ConsoleKey.R:
                    key = 'r';
                    break;
                case ConsoleKey.Y:
                    key = 'y';
                    break;
                case ConsoleKey.N:
                    key = 'n';
                    break;
            }
        }

        private static void GetRandomJokes(string category, int number, string baseAddress, string randomJokesEndpoint)
        {
            new JsonFeed(baseAddress, number);
            results = JsonFeed.GetRandomJokes(names?.Item1, names?.Item2, category, randomJokesEndpoint);
        }

        private static void GetCategories(string baseAddress, string categoriesEndpoint)
        {
            new JsonFeed(baseAddress, 0);
            results = JsonFeed.GetCategories(categoriesEndpoint).Result;
        }

        private static void GetNames(string nameGeneratorApiEndpoint)
        {
            new JsonFeed(nameGeneratorApiEndpoint, 0);
            dynamic result = JsonFeed.GetNames();
            names = Tuple.Create(result.name.ToString(), result.surname.ToString());
        }
    }
}
