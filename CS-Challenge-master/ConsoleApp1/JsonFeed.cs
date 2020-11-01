using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class JsonFeed
    {
        static string _url = "";
        static int _resultCount;
        public JsonFeed(string endpoint)
        {
            _url = endpoint;
        }

        public JsonFeed(string endpoint, int resultCount)
        {
            _url = endpoint;
            _resultCount = resultCount;
        }

        /// <summary>
        /// GetRandomJokes method returns list of jokes by replacing Chuck Norris depending upon input params
        /// </summary>
        /// <param name="firstname">First Name to be replaced in a joke</param>
        /// <param name="lastname">Last Name to be replaced in a joke</param>
        /// <param name="category">Category of the jokes</param>
        /// <param name="randomJokesEndpoint">API endpoint to fetch random jokes</param>
        /// <returns></returns>
        public static async Task<List<string>> GetRandomJokes(string firstname, string lastname, string category, string randomJokesEndpoint)
        {
            List<string> returnJokeList = new List<string>();
            if (category != null)
            {
                if (randomJokesEndpoint.Contains('?'))
                    randomJokesEndpoint += "&";
                else randomJokesEndpoint += "?";
                randomJokesEndpoint += "category=";
                randomJokesEndpoint += category;
            }
            List<string> getJokeResponses = await GetJokeResponses(randomJokesEndpoint);

            if (string.IsNullOrEmpty(firstname) && string.IsNullOrEmpty(lastname))
                return getJokeResponses;

            foreach (string joke in getJokeResponses)
            {
                returnJokeList.Add(joke.Replace("Chuck Norris", string.Join(" ", firstname, lastname), StringComparison.OrdinalIgnoreCase));
            }
            return returnJokeList;
        }

        /// <summary>
        /// GetJokeResponses method makes parallel calls to accumulate and return List of jokes
        /// </summary>
        /// <param name="randomJokesEndpoint">API endpoint to fetch random jokes</param>
        /// <returns>List of jokes</returns>
        private static async Task<List<string>> GetJokeResponses(string randomJokesEndpoint)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);
            var tasks = new List<Task<string>>();
            List<string> getJokeResponses = new List<string>();
            for (int i = 0; i < _resultCount; i++)
            {
                async Task<string> func()
                {
                    return await client.GetStringAsync(randomJokesEndpoint);
                }
                tasks.Add(func());
            }
            await Task.WhenAll(tasks);

            foreach (var t in tasks)
            {
                var getResponse = await t;
                dynamic jsonResponse = JsonConvert.DeserializeObject(getResponse);
                string joke = jsonResponse.value;
                getJokeResponses.Add(joke);
            }

            return getJokeResponses;
        }

        /// <summary>
        /// GetRandomName method returns an object containg random name by calling an API endpoint
        /// </summary>
        /// <returns>Returns an object containing name</returns>
        public static dynamic GetRandomName()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);
            var result = client.GetStringAsync("").Result;
            return JsonConvert.DeserializeObject<dynamic>(result);
        }

        /// <summary>
        /// GetCategories method returns a list of categories by calling an API endpoint
        /// </summary>
        /// <param name="categoriesEndpoint">API endpoint to fetch categories</param>
        /// <returns>Returns a List of categories from Chuck Norris API</returns>
        public static async Task<List<string>> GetCategories(string categoriesEndpoint)
        {
            List<string> categories = null;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);
            HttpResponseMessage response = await client.GetAsync(categoriesEndpoint);
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<List<string>>(content);
            }
            return categories;
        }
    }
}
