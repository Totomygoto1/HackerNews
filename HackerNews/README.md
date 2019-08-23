Read HackerNews from a .NET console application


This tutorial shows how to call a web API from a .NET console application, 
using System.Net.Http.HttpClient.

We will use the HackerNews web API at:
https://hackernews.api-docs.io/v0/overview/introduction

For this project look at 
MODEL Story: 
https://hackernews.api-docs.io/v0/items/story
GET News and Top Stories:
https://hackernews.api-docs.io/v0/live-data/new-and-top-stories

In Visual Studio, create a new Windows console app (.NET Framework) named HackerNewsAPI.


Install the Web API Client Libraries.


From the Tools menu, select NuGet Package Manager > Package Manager Console. 

In the Package Manager Console (PMC), type the following command:

Install-Package Microsoft.AspNet.WebApi.Client

The preceding command adds the following NuGet packages to the project:

Microsoft.AspNet.WebApi.Client

Newtonsoft.Json


Add a file, Model Class. Story.cs.


using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

namespace HackerNewsAPI

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

This class matches the data model used by the web API. An app can use HttpClient 
to read a Story instance from an HTTP response. 
The app doesn't have to write any deserialization code.


Adding the Code


In Program.cs paste in the following code: 

using System;

using System.Net;

using HackerNewsAPI.View;

namespace HackerNewsAPI

{

class Program

{   

static void Main()

{

HackerNewsAPI.View.UserInput.UserInteraction();
            
}

}

}

Create a folder called RestAPI and in that folder create a file/class named GetStoryList.cs,
and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using System.Net.Http;

using System.Net.Http.Headers;

using HackerNewsAPI.View;

namespace HackerNewsAPI.RestAPI

{

public class GetStoryList

{
        
static HttpClient client = new HttpClient();

        public static async Task<String[]> GetURLList(int posts)
        {
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

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
            return paths;
        }

        public static async Task<Story> GetNewsItemAsync(string path, int rank, int posts)
        {
            Story story = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                story = await response.Content.ReadAsAsync<Story>();
            }
            HackerNewsAPI.View.ShowStoryFields.ShowStory(story, rank, posts);

            return story;
        }

    }
}

Create a folder called View and in that folder create a file/class named UserInput.cs,
and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Text.RegularExpressions;

using HackerNewsAPI.RestAPI;

namespace HackerNewsAPI.View

{

public class UserInput
    
{

        public static void UserInteraction()
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
                        Console.WriteLine("Hit enter to read the HackerNews items ..");

                        string[] urls = HackerNewsAPI.RestAPI.GetStoryList.GetURLList(posts).GetAwaiter().GetResult();

                        Console.WriteLine("[");
                        for (int i = 0; i < posts; i++)
                        {
                            int rank = i + 1;

                            HackerNewsAPI.RestAPI.GetStoryList.GetNewsItemAsync(urls[i], rank, posts).GetAwaiter().GetResult();

                            Console.WriteLine("Hit enter to read the next Post ..");
                        }
                        Console.WriteLine("]");
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

In the View folder create a file/class named ShowStoryFields.cs,
and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using HackerNewsAPI.View;

namespace HackerNewsAPI.View

{

public class ShowStoryFields

{

        public static void ShowStory(Story story, int rank, int posts)
        {
            Console.WriteLine("{");
            ShowTitle(story);
            ShowURI(story);
            ShowAuthor(story);
            ShowPoints(story);
            ShowComments(story);
            ShowRank(rank);
            Console.Write("\n}");
            if (rank != posts)
            {
                Console.Write(",");
            }

            Console.ReadLine();
        }

        static void ShowTitle(Story story)
        {
            Console.Write("'title': '");

            if (HackerNewsAPI.View.CheckStoryFields.CheckTitle(story) == 1)
            {
                Console.Write(story.title);
            }
            else
            {
                Console.Write("No Title Specified,");
            }
        }

        static void ShowURI(Story story)
        {
            Console.Write("',\n'uri': '");
            if (HackerNewsAPI.View.CheckStoryFields.CheckURI(story) == 1)
            {
                Console.Write(story.url);
            }
            else
            {
                Console.Write("No Valid URI,");
            }
        }

        static void ShowAuthor(Story story)
        {
            Console.Write("',\n'author': '");
            if (HackerNewsAPI.View.CheckStoryFields.CheckAuthor(story) == 1)
            {
                Console.Write(story.by);
            }
            else
            {
                Console.Write("No Author Specified,");
            }
        }

        static void ShowPoints(Story story)
        {
            Console.Write("',\n'points': ");
            if (HackerNewsAPI.View.CheckStoryFields.CheckPoints(story) == 1)
            {
                Console.Write(story.score);
            }
            else
            {
                Console.Write("No points");
            }
        }

        static void ShowComments(Story story)
        {
            Console.Write(",\n'comments': ");
            if (HackerNewsAPI.View.CheckStoryFields.CheckComments(story) == 1)
            {
                Console.Write(story.descendants);
            }
            else
            {
                Console.Write("No comments");
            }
        }

        static void ShowRank(int rank)
        {
            Console.Write(",\n'rank': ");
            Console.Write(rank);
        }

    }
}

In the View folder create a file/class named CheckStoryFields.cs,
and paste in the following code:

using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Text.RegularExpressions;

namespace HackerNewsAPI.View

{

public class CheckStoryFields
    
{

        public static int CheckTitle(Story story)
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

        public static int CheckAuthor(Story story)
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

        public static int CheckURI(Story story)
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

        public static int CheckPoints(Story story)
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

        public static int CheckComments(Story story)
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

On the Build menu, click Build Solution.
Then Run 

