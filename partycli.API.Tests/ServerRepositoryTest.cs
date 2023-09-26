using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartyCli;
using PartyCli.Models;
using System.Collections.Generic;

namespace partycli.Data.Tests
{
    [TestClass]
    public class ServerRepositoryTest
    {
        [TestMethod]
        public void IsData_Saved()
        {
            var serverRepository = new ServerRepository();

            var storedServer = new List<ServerModel>()
            {
                new ServerModel()
                {
                    Name = "Visaginas",
                    Load = 60,
                    Status = "Online"
                }
            };

            serverRepository.StoreValue("testvalue", storedServer, false);

            var retrievedServer = serverRepository.RetrieveValue("testvalue");

            Assert.AreEqual(storedServer.Count, retrievedServer.Count);
            Assert.AreEqual(storedServer[0].Name, retrievedServer[0].Name);
            Assert.AreEqual(storedServer[0].Load, retrievedServer[0].Load);
            Assert.AreEqual(storedServer[0].Status, retrievedServer[0].Status);
        }
    }
}
