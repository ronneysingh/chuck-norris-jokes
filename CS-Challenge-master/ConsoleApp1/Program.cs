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
        static List<string> results = new List<string>();
        static char key;
        static Tuple<string, string> names;
        static ConsolePrinter printer = new ConsolePrinter();
        static string chuckNorrisApiBaseAddress;
        static string nameGeneratorApiEndpoint;
        static string categoriesEndpoint;
        static string randomJokesEndpoint;
        static List<string> categories;

        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("configsettings.json", optional: true, reloadOnChange: true);

                chuckNorrisApiBaseAddress = builder.Build().GetSection("ApiEndpoints").GetSection("ChuckNorrisApi").GetSection("BaseAddress").Value;
                nameGeneratorApiEndpoint = string.Concat(builder.Build().GetSection("ApiEndpoints").GetSection("NameGeneratorApi").GetSection("BaseAddress").Value, "/",
                                                            builder.Build().GetSection("ApiEndpoints").GetSection("NameGeneratorApi").GetSection("ApiEndpoint").Value);
                categoriesEndpoint = builder.Build().GetSection("ApiEndpoints").GetSection("ChuckNorrisApi").GetSection("CategoriesEndpoint").Value;
                randomJokesEndpoint = builder.Build().GetSection("ApiEndpoints").GetSection("ChuckNorrisApi").GetSection("RandomJokeEndpoint").Value;
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                ProcessJokes();
            }
            catch (Exception)
            {
                printer.Value("Something went wrong.");
            }
        }

        private static void ProcessJokes()
        {
            printer.Value("Press ? to get instructions.").ToString();
            bool invalidInput = true;
            while (invalidInput)
            {
                if (Console.ReadLine() == "?")
                    invalidInput = false;
                else
                    printer.Value("You have pressed an Invalid key. Press ? to get instructions.").ToString();
            }
            while (true)
            {
                if (key != 'e')
                {
                    printer.Value("Press c to get categories").ToString();
                    printer.Value("Press r to get random jokes").ToString();
                    printer.Value("Press Esc to exit...").ToString();
                }
                GetEnteredKey(Console.ReadKey());

                if (key == 'e')
                    continue;

                if (key == 'c')
                {
                    printer.Value("Please wait while we get the categories for you...").ToString();
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
                    int n = 0;
                    if (key == 'y')
                    {
                        printer.Value("How many jokes do you want? (1-9)").ToString();
                        Int32.TryParse(Console.ReadLine(), out n);
                        if (n > 0 && n < 10)
                        {
                            printer.Value("Enter a category;").ToString();
                            string enteredCategory = Console.ReadLine();
                            if (categories == null || categories.Count == 0)
                                GetCategories(chuckNorrisApiBaseAddress, categoriesEndpoint);
                            if (!categories.Any(x=> x.Equals(enteredCategory, StringComparison.OrdinalIgnoreCase)))
                                printer.Value("You have entered invalid value for a category. Let's start again...").ToString();
                            else
                            {
                                GetRandomJokes(enteredCategory.ToLower(), n, chuckNorrisApiBaseAddress, randomJokesEndpoint);
                                PrintResults();
                            }
                        }
                        else
                            printer.Value("You have entered invalid value for number of jokes. Let's start again...").ToString();
                    }
                    else if (key == 'n')
                    {
                        printer.Value("How many jokes do you want? (1-9)").ToString();
                        Int32.TryParse(Console.ReadLine(), out n);
                        if (n > 0 && n < 10)
                        {
                            GetRandomJokes(null, n, chuckNorrisApiBaseAddress, randomJokesEndpoint);
                            PrintResults();
                        }
                        else
                            printer.Value("You have entered invalid value for number of jokes. Let's start again...").ToString();
                    }
                    else
                        printer.Value("You have pressed an Invalid Key. Please try again.").ToString();
                }
                else
                    printer.Value("You have pressed an Invalid Key. Please try again.").ToString();
                names = null;
                key = '\0';
            }
        }

        private static void PrintResults()
        {
            printer.Value(Environment.NewLine + string.Join("\n", results)).ToString();
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
                case ConsoleKey.Enter:
                    key = 'e';
                    break;
                case ConsoleKey.Escape:
                    Environment.Exit(0);
                    break;
                default:
                    key = 'i';
                    break;
            }
        }

        private static void GetRandomJokes(string category, int number, string baseAddress, string randomJokesEndpoint)
        {
            new JsonFeed(baseAddress, number);
            results = JsonFeed.GetRandomJokes(names?.Item1, names?.Item2, category, randomJokesEndpoint).Result;
        }

        private static void GetCategories(string baseAddress, string categoriesEndpoint)
        {
            new JsonFeed(baseAddress);
            results = JsonFeed.GetCategories(categoriesEndpoint).Result;
            categories = results;
        }

        private static void GetNames(string nameGeneratorApiEndpoint)
        {
            new JsonFeed(nameGeneratorApiEndpoint);
            dynamic result = JsonFeed.GetRandomName();
            names = Tuple.Create(result.name.ToString(), result.surname.ToString());
        }
    }
}
