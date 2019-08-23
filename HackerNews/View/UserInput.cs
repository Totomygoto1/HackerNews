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
