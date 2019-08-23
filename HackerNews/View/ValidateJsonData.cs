﻿using System;
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