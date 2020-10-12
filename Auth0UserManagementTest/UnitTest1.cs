using Auth0UserManagement;
using Auth0UserManagement.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace Tests
{
    [SingleThreadedAttribute]
    public class Tests
    {
        private Auth0UserManager mgr;
        private string randomEmail;

        [OneTimeSetUp]
        public void Setup()
        {
            var config = new Auth0UserManagementConfig(
                ConfigurationManager.AppSettings["BaseDomain"].ToString(),
                ConfigurationManager.AppSettings["Connection"].ToString(),
                ConfigurationManager.AppSettings["ConnectionID"].ToString(),
                ConfigurationManager.AppSettings["ClientID"].ToString(),
                ConfigurationManager.AppSettings["ClientSecret"].ToString()
                );

            mgr = new Auth0UserManager(config);

            Random random = new Random();
            double ndbl = random.NextDouble();
            var number = Math.Round(ndbl * 1000000);

            if (randomEmail == null)
                randomEmail = $"{ number }@mytestdataforauth0.com";
        }

        [Test, Order(1)]
        public void SearchUsersTest1()
        {
            List<User> users = mgr.SearchUsers($"email:{ randomEmail }");
            Assert.AreEqual(0, users.Count);
        }

        [Test, Order(2)]
        public void InsertUserTest()
        {
            User testUser = new User()
            {
                email = randomEmail,
                name = "John Doe"               
            };

            //add metadata            
            testUser.AddUserMetadata("my_user_id", "ff12345ee");
            List<User> testUsers = new List<User>() { testUser };

            OperationResult ret = mgr.InsertUsers(testUsers);

            Assert.AreEqual(true, ret.Success);
        }

        [Test, Order(3)]
        public void SearchUsersTest2()
        {
            //Thread.Sleep(500);
            List<User> users = mgr.SearchUsers($"email:{ randomEmail }");
            Assert.AreEqual(1, users.Count);
        }

        [Test, Order(4)]
        public void UpdateUserTest()
        {
            List<User> users = mgr.SearchUsers($"email:{ randomEmail }");
            Assert.AreEqual(1, users.Count);

            users.First().name = "Jane Doe";
            OperationResult ret = mgr.UpdateUsers(users);
            Assert.AreEqual(true, ret.Success);
        }

        [Test, Order(5)]
        public void SearchUsersTest3()
        {
            //Thread.Sleep(500); //Test runs to fast and Auth0 is not updated in time
            List<User> users = mgr.SearchUsers($"email:{ randomEmail }");
            Assert.AreEqual("Jane Doe", users.First().name);
        }

        [Test, Order(6)]
        public void DeleteUserTest()
        {
            List<User> users = mgr.SearchUsers($"email:{ randomEmail }");
            OperationResult ret = mgr.DeleteUsers(users.Select(u => u.user_id).ToList());
            Assert.AreEqual(true, ret.Success);
        }


    }
}