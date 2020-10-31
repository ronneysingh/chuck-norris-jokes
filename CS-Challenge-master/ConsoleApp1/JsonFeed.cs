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

        public static async Task<List<string>> GetRandomJokes(string firstname, string lastname, string category, string randomJokesEndpoint)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);
            var tasks = new List<Task<string>>();
            List<string> getJokeResponses = new List<string>();
            List<string> returnJokeList = new List<string>();
            if (category != null)
            {
                if (randomJokesEndpoint.Contains('?'))
                    randomJokesEndpoint += "&";
                else randomJokesEndpoint += "?";
                randomJokesEndpoint += "category=";
                randomJokesEndpoint += category;
            }

            //parallel calls to the api to reduce overall time and improve performance
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

            if (string.IsNullOrEmpty(firstname) && string.IsNullOrEmpty(lastname))
                return getJokeResponses;

            foreach (string joke in getJokeResponses)
            {
                returnJokeList.Add(joke.Replace("Chuck Norris", string.Join(" ", firstname, lastname)));
            }
            return returnJokeList;
        }

        /// <summary>
        /// returns an object that contains name and surname
        /// </summary>
        /// <param name="client2"></param>
        /// <returns></returns>
		public static dynamic GetNames()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_url);
            var result = client.GetStringAsync("").Result;
            return JsonConvert.DeserializeObject<dynamic>(result);
        }

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
