﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using BddSharp.Kernel;
namespace SerializerTest
{
    /// <summary>
    ///This is a test class for BddSharp.Serializer.BddSerializer and is intended
    ///to contain all BddSharp.Serializer.BddSerializer Unit Tests
    ///</summary>
    [TestClass()]
    public class BddSerializerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Serialize (Bdd, string)
        ///</summary>
        [TestMethod()]
        public void SerializeTest()
        {
            Bdd root = null; // TODO: Initialize to an appropriate value

            string filename = null; // TODO: Initialize to an appropriate value

            BddSharp.Serializer.BddSerializer.Serialize(root, filename);

            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

    }


}
