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
        public JsonFeed(string endpoint, int results)
        {
            _url = endpoint;
        }
        
		public static string[] GetRandomJokes(string firstname, string lastname, string category, string randomJokesEndpoint)
		{
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_url);
			//string url = "jokes/random";
			if (category != null)
			{
				if (randomJokesEndpoint.Contains('?'))
                    randomJokesEndpoint += "&";
				else randomJokesEndpoint += "?";
                randomJokesEndpoint += "category=";
                randomJokesEndpoint += category;
			}

            string joke = Task.FromResult(client.GetStringAsync(randomJokesEndpoint).Result).Result;

            if (firstname != null && lastname != null)
            {
                int index = joke.IndexOf("Chuck Norris");
                string firstPart = joke.Substring(0, index);
                string secondPart = joke.Substring(0 + index + "Chuck Norris".Length, joke.Length - (index + "Chuck Norris".Length));
                joke = firstPart + " " + firstname + " " + lastname + secondPart;
            }

            return new string[] { JsonConvert.DeserializeObject<dynamic>(joke).value };
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

		public static async Task<string[]> GetCategories(string categoriesEndpoint)
		{
            string[] categories = null;
            //string url = "jokes/categories";
            HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(_url);
            HttpResponseMessage response = await client.GetAsync(categoriesEndpoint);
            string content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<string[]>(content);
            }
            return categories;
        }
    }
}
