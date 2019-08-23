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
