using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HackerNews.RestAPI;
using HackerNews.View;
using HackerNews;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void CheckValidURL()
        {
            // Check if URL field is valid , if valid = 1.

            Story story = new Story
            {
                title = "The north pole moved to the North Pole in a single human lifetime",
                url = "https://arstechnica.com/science/2014/10/the-north-pole-moved-to-the-north-pole-in-a-single-human-lifetime/",
                by = "berkeleyjunk",
                score = 7,
                descendants = 2
            };

            int valid = HackerNews.View.ValidateJsonData.ValidateURI(story);

            Assert.AreEqual(valid, 1);

        }

        [TestMethod]
        public void CheckValidAuthor()
        {
            // Check if Author field is not valid, if valid = 0.

            Story story = new Story
            {
                title = "The north pole moved to the North Pole in a single human lifetime",
                url = "https://arstechnica.com/science/2014/10/the-north-pole-moved-to-the-north-pole-in-a-single-human-lifetime/",
                by = "berkeleyjunk...............................................................................................................................................................................................................................................................",
                score = 7,
                descendants = 2
            };

            int valid = HackerNews.View.ValidateJsonData.ValidateAuthor(story);

            Assert.AreEqual(valid, 0);

        }

        [TestMethod]
        public void CheckValidPoints()
        {
            // Check if Points field is not valid, if valid = 0. 

            Story story = new Story
            {
                title = "The north pole moved to the North Pole in a single human lifetime",
                url = "https://arstechnica.com/science/2014/10/the-north-pole-moved-to-the-north-pole-in-a-single-human-lifetime/",
                by = "berkeleyjunk...............................................................................................................................................................................................................................................................",
                score = -1,
                descendants = 2
            };

            int valid = HackerNews.View.ValidateJsonData.ValidatePoints(story);

            Assert.AreEqual(valid, 0);

        }

    }
}
