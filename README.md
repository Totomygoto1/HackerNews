# Read HackerNews from a .NET console application


This tutorial shows how to call a web API from a .NET console application, using System.Net.Http.HttpClient.

We will use the HackerNews web API at: https://hackernews.api-docs.io/v0/overview/introduction

For this project look at MODEL Story: https://hackernews.api-docs.io/v0/items/story GET News and Top Stories: https://hackernews.api-docs.io/v0/live-data/new-and-top-stories

In Visual Studio, create a new Windows console app (.NET Framework) named HackerNews.


# Install the Web API Client Libraries.


From the Tools menu, select NuGet Package Manager > Package Manager Console.

In the Package Manager Console (PMC), type the following command:

Install-Package Microsoft.AspNet.WebApi.Client

The preceding command adds the following NuGet packages to the project:

Microsoft.AspNet.WebApi.Client

Newtonsoft.Json


# Add a file, Model Class. Story.cs.

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

namespace HackerNews

{

public class Story

{

public string title { get; set; }

public string url { get; set; }

public string by { get; set; }

public int score { get; set; }

public int descendants { get; set; }

}

}

This class matches the data model used by the web API. An app can use HttpClient to read a Story instance from an HTTP response. The app doesn't have to write any deserialization code.


# Adding the Code


# In Program.cs paste in the following code:

using System;

using System.Net;

using HackerNews.View;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Text.RegularExpressions;

using HackerNews.RestAPI;

namespace HackerNews 

{

class Program

{

    static void Main(string[] args)
    {
        HackerNews.View.UserInput.UserCommand();

        Console.ReadLine();
    }

}

}


# Create a folder called RestAPI and in that folder create a file/class named GetRepository.cs, and paste in the following code:

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


# Create a folder called View and in that folder create a file/class named UserInput.cs, and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Text.RegularExpressions;

using HackerNews.RestAPI;

namespace HackerNews.View 

{

public class UserInput

{
    
    public static async Task GetHackerNewsItemsAsync(int posts)
    {
        int i = 0;

        // This method = GetURLList() retreives all the diffrent topstory IDs,
        // then builds a URL for the different topstories based on 'ID' eg. 20686992, https://hacker-news.firebaseio.com/v0/item/20686992.json?print=pretty,
        // then returns an array with all the built URLs = topstory paths.

        string[] urls = HackerNews.RestAPI.GetRepository.GetURLList(posts).GetAwaiter().GetResult();

        // await Task.WhenAll() makes sure that GetNewsItemAsync() gets called for each single NewsItem in a loop,
        // it loops through the urls Array that contains the paths for each NewsItem(topstory).
        // await Task.WhenAll() = Returns a collection of values, jsonData Stories, when our Task finishes.

        // This method = GetNewsItemAsync() retreives all the topstories based on how many items the user has requested.  
        
        await Task.WhenAll(urls.Select(j => HackerNews.RestAPI.GetRepository.GetNewsItemAsync(urls[i++], posts)));
        
    }

    static async Task RunAsync(int posts)
    {
        await GetHackerNewsItemsAsync(posts);
    }

    // This method UserCommand() = handles the user's input.
    // If you type: hackernews--posts , followed by how many number of posts you want to read, it will list that many HackerNews topstories.
    // If you type: hackernews--posts 10 , you can read 10 topstories.

    public static void UserCommand()
    {
        string input = "";

        while (input != "q")
        {

            Console.WriteLine("\nPlease Enter how many HackerNews items you want to read. \n");
            Console.WriteLine("We expect this Syntax: \n");
            Console.WriteLine("hackernews--posts n \n");
            Console.WriteLine("how many 'n' posts to print. \n");
            Console.WriteLine("n = A positive integer <= 100. \n");
            Console.WriteLine("To quit write 'q'. \n");

            input = Console.ReadLine();
            input = input.ToLower();

            if (Regex.IsMatch(input, "^hackernews--posts "))
            {
                Match m = Regex.Match(input, @"\d+");
                int posts = Convert.ToInt32(m.Value);

                if (m.Success && posts <= 100)
                {
                    Console.WriteLine("News items selected: " + m.Value + "\n");

                    Console.WriteLine("[");

                    // This method will call GetHackerNewsItemsAsync(), that will list the HackerNews topstories.
                    RunAsync(posts).GetAwaiter().GetResult();
                }
            }
            else if (input == "q")
            {
                Console.WriteLine("You want to quit? Please press 'Enter' to confirm.");
            }
            else
            {
                Console.WriteLine("Wrong Syntax. Please try again.");
            }

        }

        Console.ReadLine();
    }

}

}


# In the View folder create a file/class named SerializeJsonObject.cs, and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using Json.Net;

namespace HackerNews.View 

{

public class SerializeJsonObject
{
    public static int count = 1;

    public static void DotNetSerialize(Story story, int posts)
    {
        string jsonData = Json.Net.JsonNet.Serialize(story);

        Console.Write(jsonData);
    }

    // This method Serialize() serializes the .Net Story object.
    // It converts a custom .Net object to a Json string.
    // It checks if certain fields are valid.

    public static void Serialize(Story story, int posts)
    {
        // Convert/Serialize the .Net Story object into a Json string.

        string title = "";
        string uri = "";
        string author = "";
        string points = "";
        string comments = "";
        int rank = count++;

        string jsonData = "";          

        // Check if data fields are valid that are retrevied from the HackerNews REST/API.

        if (HackerNews.View.ValidateJsonData.ValidateTitle(story) == 1)
        {
            title = story.title;
        }
        else
        {
            title = "No Title Specified,";
        }

        if (HackerNews.View.ValidateJsonData.ValidateURI(story) == 1)
        {
            uri = story.url;
        }
        else
        {
            uri = "No Valid URI,";
        }

        if (HackerNews.View.ValidateJsonData.ValidateAuthor(story) == 1)
        {
            author = story.by;
        }
        else
        {
            author = "No Author Specified,";
        }

        if (HackerNews.View.ValidateJsonData.ValidatePoints(story) == 1)
        {
            points = story.score.ToString();
        }
        else
        {
            points = "No points";
        }

        if (HackerNews.View.ValidateJsonData.ValidateComments(story) == 1)
        {
            comments = story.descendants.ToString();
        }
        else
        {
            comments = "No comments";
        }

        jsonData = jsonData + "{\n'title': '"
            + title
            + "',\n'uri': '"
            + uri
            + "',\n'author': '"
            + author
            + "',\n'points': "
            + points
            + ",\n'comments': "
            + comments
            + ",\n'rank': "
            + rank.ToString()
            + "\n";

        if (rank == posts)
        {
            jsonData = jsonData + "}";
        }
        else
        {
            jsonData = jsonData + "},";
        }

        if (rank == posts)
        {
            jsonData = jsonData + "\n]";
        }
        Console.WriteLine(jsonData);

        Console.ReadLine();
    }

}

}


# In the View folder create a file/class named ValidateJsonData.cs, and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace HackerNews.View 

{

public class ValidateJsonData

{

    public static int ValidateTitle(Story story)
    {
        int valid = 1;
        bool b1 = string.IsNullOrEmpty(story.title);

        if (story.title.Length < 257 && b1 == false)
        {
            valid = 1;
        }
        else
        {
            valid = 0;
        }
        return valid;
    }

    public static int ValidateAuthor(Story story)
    {
        int valid = 1;
        bool b1 = string.IsNullOrEmpty(story.by);

        if (story.by.Length < 257 && b1 == false)
        {
            valid = 1;
        }
        else
        {
            valid = 0;
        }
        return valid;
    }

    public static int ValidateURI(Story story)
    {
        int valid = 1;

        string input = story.url.ToLower();
        if (Regex.IsMatch(input, "^http://"))
        {
            valid = 1;
        }
        else if (Regex.IsMatch(input, "^https://"))
        {
            valid = 1;
        }
        else
        {
            valid = 0;
        }
        return valid;
    }

    public static int ValidatePoints(Story story)
    {
        int valid = 1;

        if (story.score >= 0)
        {
            valid = 1;
        }
        else
        {
            valid = 0;
        }
        return valid;
    }

    public static int ValidateComments(Story story)
    {
        int valid = 1;

        if (story.descendants >= 0)
        {
            valid = 1;
        }
        else
        {
            valid = 0;
        }
        return valid;
    }

}

}

On the Build menu, click Build Solution. Then Run.
