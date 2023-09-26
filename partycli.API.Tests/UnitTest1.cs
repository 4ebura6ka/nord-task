using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyCli.API;
using System;
using System.Net.Http;

namespace partycli.API.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var httpClient = new HttpClient();
            var serverService = new ServerService();

        }
    }
}
