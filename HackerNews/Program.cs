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

