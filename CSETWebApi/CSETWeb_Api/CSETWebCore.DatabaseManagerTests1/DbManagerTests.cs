//////////////////////////////// 
// 
//   Copyright 2023 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSETWebCore.DatabaseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CSETWebCore.DataLayer.Model;
using System.Net;
using log4net;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using UpgradeLibrary.Upgrade;

namespace CSETWebCore.DatabaseManager.Tests
{
    [TestClass()]
    public class DbManagerTests
    {
        [TestMethod()]
        public void CopyDBAcrossServersTest()
        {   
            string clientCode = "DHS";
            string appCode = "CSET";
            DbManager manager = new DbManager(new Version("12.0.0.19"),clientCode, appCode);
            //TODO finish this.
            //manager.CopyDBAcrossServers();
        }

        [TestMethod()]
        public void CopyDBFromInstallationSource()
        {
            //setup a copy file
            //setup a destination file
            string clientCode = "DHS";
            string appCode = "CSET";
            DbManager manager = new DbManager(new Version("12.0.0.19"), clientCode, appCode);
            //run the same test twice and make sure that the number increment works
            string mdf = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\CSETWebTest.mdf";
            string ldf = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\CSETWebTest_log.ldf";
            manager.CopyDBFromInstallationSource(mdf,ldf);
            manager.CopyDBFromInstallationSource(mdf, ldf);
            manager.CopyDBFromInstallationSource(mdf, ldf);
            manager.CopyDBFromInstallationSource(mdf, ldf);
        }

        [TestMethod()]
        public void TestUpgrade()
        {
            //create a copy of the CSET 9.0 database as CSETWeb
            //detach the csetweb database what evers it 
            string testdb = Path.Combine(InitialDbInfo.GetExecutingDirectory().FullName, "data", "TestWeb.mdf");
            string testlog = Path.Combine(InitialDbInfo.GetExecutingDirectory().FullName, "data", "TestWeb_log.ldf");
            File.Copy("data\\CSETWeb90.mdf", Path.Combine("data", testdb),true);
            File.Copy("data\\CSETWeb90_log.ldf", Path.Combine("data", testlog),true);

            //C:\src\repos\cset\CSETWebApi\CSETWeb_Api\CSETWebCore.DatabaseManagerTests1\bin\Debug\net7.0\Data\TestWeb.mdf

            //setup a copy file
            //setup a destination file
            string clientCode = "Test";
            string appCode = "Test";
            DbManager manager = new DbManager(new Version("12.0.0.19"), clientCode, appCode);
            
            manager.ForceCloseAndDetach(DbManager.CurrentMasterConnectionString, "TestWeb");
            manager.AttachTest("TestWeb", testdb, testlog);
            VersionUpgrader upgrader = new VersionUpgrader(Assembly.GetAssembly(typeof(DbManager)).Location);
            manager.SetupDb();

            upgrader.UpgradeOnly(new Version("12.0.0.19"), "data source=(localdb)\\mssqllocaldb;initial catalog=TestWeb;persist security info=True;Integrated Security=SSPI;MultipleActiveResultSets=True");
            
            
        }


        [TestMethod()]
        public void TestSetup()
        {
            //setup a copy file
            //setup a destination file
            string clientCode = "DHS";
            string appCode = "CSET";
            DbManager manager = new DbManager(new Version("12.0.0.19"), clientCode, appCode);
            manager.SetupDb();
        }

        [TestMethod()]
        public void CopyDBFromInstallationSourceTest2()
        {
            string clientCode = "DHS";
            string appCode = "CSET";
            string mdf = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\CSETWebTest.mdf";
            string ldf = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\CSETWebTest_log.ldf";

            DbManager manager = new DbManager(new Version("12.0.0.19"), clientCode, appCode);
            //run the same test twice and make sure that the number increment works
            string conString = "Server=(localdb)\\mssqllocaldb;Integrated Security=true;AttachDbFileName=" + mdf;
            using (SqlConnection conn = new SqlConnection(conString))
            {
               conn.Open();
               SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "select * from CSET_Version;";
               SqlDataReader reader = cmd.ExecuteReader();
                while (reader.NextResult())
                { 
                    Assert.IsNotNull(reader.GetString(2)); 
                }
                manager.CopyDBFromInstallationSource(mdf, ldf);
            }
        }

    }
}