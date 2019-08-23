using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using HackerNews.View;

namespace HackerNews.RestAPI
{

    public class GetRepository
    {
        static HttpClient client = new HttpClient();

        public static async Task<Story> GetNewsItemAsync(string path, int posts)
        {
            Story story = null;
            // Make an asynchronous Get Request to HackerNews REST/API to retreive one NewsItem(topstory).
            HttpResponseMessage response = await client.GetAsync(path);

            // The GetAsync() method sends the HTTP GET request. 
            // When the method completes, it returns an HttpResponseMessage that contains the HTTP response. 
            // If the status code in the response is a success code, the response body contains the JSON 
            // representation of a Story. Call ReadAsAsync() to deserialize the JSON payload to a Story instance(Story.cs).

            if (response.IsSuccessStatusCode)
            {
                story = await response.Content.ReadAsAsync<Story>();
            }

            // Serialize 1 Story Object into JSON data. Pass on how many posts should be retreived.
            HackerNews.View.SerializeJsonObject.Serialize(story,posts);

            return story;
        }

        public static async Task<String[]> GetURLList(int posts)
        {
            // The preceding code:
            // Sets the base URI for HTTP requests. Change the port number to the port used in the server app.
            // Sets the Accept header to "application/json". Setting this header tells the server to send data in JSON format.

            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // Make an asynchronous Get Request to HackerNews REST/API to retreive an Array of NewsItem(topstory) IDs.
            HttpResponseMessage response = await client.GetAsync("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Split the Array and build an id-list Array that gets read into a paths Array.
            string[] id_list = responseBody.Split(',');

            string[] paths = new string[posts];

            for (int i = 0; i < posts; i++)
            {
                if (i == 0)
                {
                    string sub_id = id_list[i].Substring(1);
                    string id = sub_id.Trim();                   
                    string path = "https://hacker-news.firebaseio.com/v0/item/" + id + ".json?print=pretty";                   
                    paths[i] = path;
                }
                else
                {
                    string id = id_list[i].Trim();                    
                    string path = "https://hacker-news.firebaseio.com/v0/item/" + id + ".json?print=pretty";                    
                    paths[i] = path;
                }
            }

            // Return the paths Array with all the paths for the different NewsItems, topstories.
            return paths;
        }

    }
}
