using EverySearch.Lib;
using EverySearch.Models;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace EverySearchTests
{
    public class ProvidersTest
    {
        private IConfiguration configuration;
        private readonly string resourcesDir = "EverySearchTests.Resources.";

        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<ProvidersTest>();
            configuration = builder.Build();
        }

        [Test]
        public void TestGoogleUrl()
        {
            SearchProvider search = new GoogleProvider(configuration);
            string query = "Bible";
            int num = 7;
            string host = "https://www.googleapis.com/customsearch/v1";
            string key = System.Web.HttpUtility.UrlPathEncode(configuration["Google:key"]);
            string cx = System.Web.HttpUtility.UrlPathEncode(configuration["Google:cx"]);
            string expected = $"{host}?q={query}&cx={cx}&key={key}&num={num}";
            string url = search.MakeRequestUrl(query, num);
            Console.WriteLine($"got:\n{url}");
            Console.WriteLine($"expected:\n{expected}");
            Assert.IsTrue(url == expected);
        }

        [Test]
        public void TestGoogleParse()
        {
            SearchProvider search = new GoogleProvider(configuration);
            string filename = "google.json";
            string json = ReadResource(resourcesDir + filename);
            var response = search.ParseResult(json);
            Assert.IsTrue(response.Any(r => r.Snippet.Contains("Bible", System.StringComparison.InvariantCultureIgnoreCase)));
        }

        [Test]
        public void TestYandexUrl()
        {
            SearchProvider search = new YandexProvider(configuration);
            string query = "Bible";
            int num = 7;
            string host = "https://yandex.com/search/xml";
            string key = System.Web.HttpUtility.UrlPathEncode(configuration["Yandex:key"]);
            string user = System.Web.HttpUtility.UrlPathEncode(configuration["Yandex:user"]);
            string expected=$"{host}?query={query}&key={key}&user={user}&groupby=attr%3D%22%22.mode%3Dflat.groups-on-page%3D{num}.docs-in-group%3D1";
            string url = search.MakeRequestUrl(query, num);
            Console.WriteLine($"got:\n{url}");
            Console.WriteLine($"expected:\n{expected}");
            Assert.IsTrue(url == expected);
        }

        [Test]
        public void TestYandexParse()
        {
            SearchProvider search = new YandexProvider(configuration);
            string filename = "yandex.xml";
            string xml = ReadResource(resourcesDir + filename);
            var response = search.ParseResult(xml);
            Assert.IsTrue(response.Any(r => r.Snippet.Contains("Bible", System.StringComparison.InvariantCultureIgnoreCase)));
        }

        private static string ReadResource(string path)
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Console.WriteLine(String.Join(" ", (thisAssembly.GetManifestResourceNames())));
            var reader = new StreamReader(thisAssembly.GetManifestResourceStream(path));
            string xml = reader.ReadToEnd();
            return xml;
        }
    }
}