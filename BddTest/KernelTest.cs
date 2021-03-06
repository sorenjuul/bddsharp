﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using BddSharp.Kernel;
namespace BddTest
{
    /// <summary>
    ///This is a test class for BddSharp.Kernel.Bdd and is intended
    ///to contain all BddSharp.Kernel.Bdd Unit Tests
    ///</summary>
    [TestClass()]
    public class BddTest
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Kernel.Setup();
        }

        //Use TestCleanup to run code after each test has run
        //
        [TestCleanup()]
        public void MyTestCleanup()
        {
            
        }
        //
        #endregion


        /// <summary>
        ///A test for Bdd (bool)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            bool constantFalse = false;
            bool constantTrue = true;

            Bdd targetFalse = new Bdd(constantFalse);
            Bdd targetTrue = new Bdd(constantTrue);

            Assert.AreEqual(0, targetFalse.U, "Wrong U value");
            Assert.AreEqual(0, targetFalse.Low.U, "Wrong Low value");
            Assert.AreEqual(0, targetFalse.High.U, "Wrong High value");

            Assert.AreEqual(1, targetTrue.U, "Wrong U value");
            Assert.AreEqual(1, targetTrue.Low.U, "Low value");
            Assert.AreEqual(1, targetTrue.High.U, "High value");
        }

        /// <summary>
        ///A test for Bdd (int)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest1()
        {
            int var0 = 0;
            int var43875 = 43875;

            Bdd target0 = new Bdd(var0);
            Bdd target43875 = new Bdd(var43875);

            Assert.AreEqual(0, target0.Var, "Var");
            Assert.AreEqual(0, target0.Low.U, "Low");
            Assert.AreEqual(1, target0.High.U, "High");

            Assert.AreEqual(43875, target43875.Var, "Var");
            Assert.AreEqual(0, target43875.Low.U, "Low");
            Assert.AreEqual(1, target43875.High.U, "High");
        }

        /// <summary>
        ///A test for Dispose ()
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            bool constant = false;

            Bdd target1 = new Bdd(constant);
            Bdd target2 = new Bdd(false);

            target1.Dispose();
            //Assert.AreEqual(target2, target1);


            Assert.Inconclusive("Not possible to implement good test /Soren");
        }

        /// <summary>
        ///A test for Equals (Bdd)
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            bool constant = false;

            Bdd target = new Bdd(constant);

            Bdd bdd = null;

            bool expected = false;
            bool actual;

            actual = target.Equals(bdd);

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Bdd.Equals did not return the expected value.");

            Bdd target2 = new Bdd(2);
            Bdd target22 = new Bdd(2);

            bool expected2 = true;
            bool actual2 = target2.Equals(target22);

            Assert.AreEqual(expected2, actual2, "Equals with same var");

            target = new Bdd(3);
            expected = false;
            actual = target.Equals(target2);

            Assert.AreEqual(expected, actual, "Equals with dif var");
        }

        /// <summary>
        ///A test for GetHashCode ()
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            Bdd target = new Bdd(2);

            int expected = 2;
            int actual;

            actual = target.GetHashCode();

            Assert.AreEqual(expected, actual, "wrong hashcode returned");

            target = new Bdd(54);
            expected = 3;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual, "wrong hashcode returned");
        }

        /// <summary>
        ///A test for High
        ///</summary>
        [TestMethod()]
        public void HighTest()
        {
            bool constant = false;

            Bdd target = new Bdd(constant);

            Bdd val = new Bdd(false);
            bool result = val.Equals(target.High);

            Assert.AreEqual(true, result, "BddSharp.Kernel.Bdd.High was not set correctly.");

            target = new Bdd(2);
            val = new Bdd(true);
            result = val.Equals(target.High);
            Assert.AreEqual(true, result, "BddSharp.Kernel.Bdd.High was not set correctly.");
        }

        /// <summary>
        ///A test for IsTerminal ()
        ///</summary>
        [TestMethod()]
        public void IsTerminalTest()
        {
            bool constant = false;

            Bdd target = new Bdd(constant);

            bool expected = true;
            bool actual;

            actual = target.IsTerminal();

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Bdd.IsTerminal did not return the expected value.");

            constant = true;

            target = new Bdd(constant);

            expected = true;

            actual = target.IsTerminal();

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Bdd.IsTerminal did not return the expected value.");
        }

        /// <summary>
        ///A test for Low
        ///</summary>
        [TestMethod()]
        public void LowTest()
        {
            bool constant = true;

            Bdd target = new Bdd(constant);

            Bdd val = new Bdd(true);
            bool result = val.Equals(target.Low);

            Assert.AreEqual(true, result, "BddSharp.Kernel.Bdd.Low was not set correctly1.");

            target = new Bdd(2);
            val = new Bdd(false);
            result = val.Equals(target.Low);
            Assert.AreEqual(true, result, "BddSharp.Kernel.Bdd.Low was not set correctly2.");
        }

        /// <summary>
        ///A test for ToString ()
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            bool constant = false;

            Bdd target = new Bdd(constant);

            string expected = "0";
            string actual;

            actual = target.ToString();

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Bdd.ToString did not return the expected value.");
        }

        ///// <summary>
        /////A test for U
        /////</summary>
        //[TestMethod()]
        //public void UTest()
        //{
        //    bool constant = false;

        //    Bdd target = new Bdd(constant);

        //    int val = 0;

        //    target.U = val;


        //    Assert.AreEqual(val, target.U, "BddSharp.Kernel.Bdd.U was not set correctly.");
        //}

        /// <summary>
        ///A test for Var
        ///</summary>
        [TestMethod()]
        public void VarTest()
        {
            bool constant = false;

            Bdd target = new Bdd(constant);

            int val = int.MaxValue;

            Assert.AreEqual(val, target.Var, "BddSharp.Kernel.Bdd.Var was not set correctly.");



            target = new Bdd(5);

            val = 5;

            Assert.AreEqual(val, target.Var, "BddSharp.Kernel.Bdd.Var was not set correctly.");
        }

    }


    /// <summary>
    ///This is a test class for BddSharp.Kernel.Kernel and is intended
    ///to contain all BddSharp.Kernel.Kernel Unit Tests
    ///</summary>
    [TestClass()]
    public class KernelTest
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Kernel.Setup();
        }

        //Use TestCleanup to run code after each test has run
        //
        [TestCleanup()]
        public void MyTestCleanup()
        {
            
        }
        //
        #endregion


        /// <summary>
        ///A test for AllSat (int)
        ///</summary>
        [TestMethod()]
        public void AllSatTest()
        {
            Bdd var1 = new Bdd(1);
            Bdd var2 = new Bdd(2);
            Bdd result = Kernel.Or(var1, var2);

            string expected = "var1=0 var2=1 " + Environment.NewLine + "var1=1 " + Environment.NewLine;
            string actual = Kernel.AllSat(result);

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.AllSat did not return the expected value.");

        }

        /// <summary>
        ///A test for And (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void AndTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.And(u1, u2);
            int result1 = actual.Low.U;
            int result2 = actual.High.Low.U;
            int result3 = actual.High.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value.");
        }


        /// <summary>
        ///A test for CompactAnySat (int, int)
        ///</summary>
        [TestMethod()]
        public void CompactAnySatTest()
        {
            Bdd u1 = new Bdd(1);
            Bdd u2 = new Bdd(2);
            Bdd result = Kernel.And(u1, u2);
            string expected = "11";
            string actual;

            actual = BddSharp.Kernel.Kernel.CompactAnySat(result);

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.CompactAnySat did not return the expected value.");
        }

        /// <summary>
        ///A test for DelegateAnd (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateAndTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateAnd(u1, u2);
            int result1 = actual.Low.U;
            int result2 = actual.High.Low.U;
            int result3 = actual.High.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value.");
        }

        /// <summary>
        ///A test for DelegateEqual (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateEqualTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateEqual(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(1, result4, "BddSharp.Kernel.Kernel.And did not return the expected value.");
        }

        /// <summary>
        ///A test for DelegateGreater (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateGreaterTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Greater(u1, u2);
            int result1 = actual.Low.U;

            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value.");

            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result4, "BddSharp.Kernel.Kernel.And did not return the expected value.");
        }

        /// <summary>
        ///A test for DelegateImp (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateImpTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateImp(u1, u2);
            int result1 = actual.Low.U;

            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result4, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for DelegateInvImp (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateInvImpTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateInvImp(u1, u2);
            int result1 = actual.Low.High.U;
            int result2 = actual.Low.Low.U;
            int result3 = actual.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for DelegateLess (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateLessTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateLess(u1, u2);
            int result1 = actual.Low.High.U;
            int result2 = actual.Low.Low.U;
            int result3 = actual.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for DelegateNand (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateNandTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateNand(u1, u2);
            int result1 = actual.Low.U;
            int result2 = actual.High.High.U;
            int result3 = actual.High.Low.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for DelegateNor (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateNorTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateNor(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for DelegateNot (Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateNotTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateNot(u1);

            int result1 = actual.Low.U;
            int result2 = actual.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.Not did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.Not did not return the expected value.");
        }

        /// <summary>
        ///A test for DelegateOr (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateOrTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateOr(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for DelegateXor (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void DelegateXorTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.DelegateXor(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
            Assert.AreEqual(0, result4, "BddSharp.Kernel.Kernel.And did not return the expected value1.");
        }

        /// <summary>
        ///A test for Equal (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void EqualTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Equal(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(1, result4, "BddSharp.Kernel.Kernel.And did not return the expected value.");
        }

        /// <summary>
        ///A test for Exists (Bdd, int)
        ///</summary>
        [TestMethod()]
        public void ExistsTest()
        {
            Bdd u1 = new Bdd(2);
            Bdd u2 = new Bdd(3);
            Bdd u3 = new Bdd(1);
            Bdd result1 = Kernel.Nand(Kernel.Or(u1, u2), u3);
            int var = 3;

            Bdd actual;

            actual = BddSharp.Kernel.Kernel.Exists(var, result1);

            int number1 = actual.Low.U;
            int number2 = actual.High.Low.U;
            int number3 = actual.High.High.U;

            Assert.AreEqual(1, number1, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
            Assert.AreEqual(1, number2, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
            Assert.AreEqual(0, number3, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");

        }

        /// <summary>
        ///A test for ForAll (Bdd, int)
        ///</summary>
        [TestMethod()]
        public void ForAllTest()
        {
            Bdd u1 = new Bdd(2);
            Bdd u2 = new Bdd(3);
            Bdd u3 = new Bdd(1);
            Bdd result1 = Kernel.Nand(Kernel.Or(u1, u2), u3);
            int var = 3;

            Bdd actual;

            actual = BddSharp.Kernel.Kernel.ForAll(var, result1);

            int number1 = actual.Low.U;
            int number2 = actual.High.U;
            int number3 = actual.Var;

            Assert.AreEqual(1, number1, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
            Assert.AreEqual(0, number2, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
            Assert.AreEqual(1, number3, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        }

        /// <summary>
        ///A test for Greater (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void GreaterTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Greater(u1, u2);
            int result1 = actual.Low.U;

            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value.");

            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value.");
            Assert.AreEqual(0, result4, "BddSharp.Kernel.Kernel.And did not return the expected value.");
        }

        /// <summary>
        ///A test for IfThen (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void IfThenTest()
        {
            Bdd Condition = null; // TODO: Initialize to an appropriate value

            Bdd Then = null; // TODO: Initialize to an appropriate value

            Bdd expected = null;
            Bdd actual;

            actual = BddSharp.Kernel.Kernel.IfThen(Condition, Then);

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.IfThen did not return the expected value.");
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IfThenElse (Bdd, Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void IfThenElseTest()
        {
            Bdd Condition = null; // TODO: Initialize to an appropriate value

            Bdd Then = null; // TODO: Initialize to an appropriate value

            Bdd Else = null; // TODO: Initialize to an appropriate value

            Bdd expected = null;
            Bdd actual;

            actual = BddSharp.Kernel.Kernel.IfThenElse(Condition, Then, Else);

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.IfThenElse did not return the expected value.");
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Imp (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void ImpTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Imp(u1, u2);
            int result1 = actual.Low.U;

            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result4, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for InvImp (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void InvImpTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.InvImp(u1, u2);
            int result1 = actual.Low.High.U;
            int result2 = actual.Low.Low.U;
            int result3 = actual.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for Lesser (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void LesserTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Lesser(u1, u2);
            int result1 = actual.Low.High.U;
            int result2 = actual.Low.Low.U;
            int result3 = actual.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for Nand (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void NandTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Nand(u1, u2);
            int result1 = actual.Low.U;
            int result2 = actual.High.High.U;
            int result3 = actual.High.Low.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for Nor (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void NorTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Nor(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for Not (Bdd)
        ///</summary>
        [TestMethod()]
        public void NotTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd actual = BddSharp.Kernel.Kernel.Not(u1);

            int result1 = actual.Low.U;
            int result2 = actual.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.Not did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.Not did not return the expected value.");
        }

        /// <summary>
        ///A test for Or (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void OrTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Or(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
        }

        /// <summary>
        ///A test for SatCount (Bdd)
        ///</summary>
        [TestMethod()]
        public void SatCountTest()
        {
            

            long expected = 0;
            long actual;

            

            
            Bdd root2 = Kernel.Or(new Bdd(5897), new Bdd(655555));
            actual = BddSharp.Kernel.Kernel.SatCount(root2);
            expected = 3;
            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.SatCount did not return the expected value.");

            Bdd root3 = Kernel.Equal(new Bdd(4587), Kernel.Or(new Bdd(5897), new Bdd(655555)));
            root3 = Kernel.Restrict(root3, 5897, true);
            actual = BddSharp.Kernel.Kernel.SatCount(root3);
            expected = 1;
            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.SatCount did not return the expected value.");

        }

        ///// <summary>
        /////A test for Setup ()
        /////</summary>
        //[TestMethod()]
        //public void SetupTest()
        //{
        //    int [,] Lookup = new int[11, 4]  
        //    { 
        //      {0,0,0,1},  /* and                       ( & )         */
        //      {0,1,1,0},  /* xor                       ( ^ )         */
        //      {0,1,1,1},  /* or                        ( | )         */
        //      {1,1,1,0},  /* nand                                    */
        //      {1,0,0,0},  /* nor                                     */
        //      {1,1,0,1},  /* implication               ( >> )        */
        //      {1,0,0,1},  /* bi-implication                          */
        //      {0,0,1,0},  /* difference /greater than  ( - ) ( > )   */
        //      {0,1,0,0},  /* less than                 ( < )         */
        //      {1,0,1,1},  /* inverse implication       ( << )        */
        //      {1,1,0,0}   /* not                       ( ! )         */
        //    };

        //    Bdd root = new Bdd(true);

        //    for (int i = 0; i <= 15; i++)
        //    {
        //        root = Kernel.And(root, new Bdd(i));
        //    }

        //    BddSharp.Kernel.Kernel.Setup();
        //    BddGCache G = BddSharp_Kernel_KernelAccessor.G;

            
        //    Assert.AreEqual(1, BddSharp_Kernel_KernelAccessor.MaxU);
           
        //    CollectionAssert.AreEquivalent(Lookup, BddSharp_Kernel_KernelAccessor.Lookup);

        //}

        /// <summary>
        ///A test for Xor (Bdd, Bdd)
        ///</summary>
        [TestMethod()]
        public void XorTest()
        {
            Bdd u1 = new Bdd(1);

            Bdd u2 = new Bdd(2);

            Bdd actual = BddSharp.Kernel.Kernel.Xor(u1, u2);
            int result1 = actual.Low.Low.U;
            int result2 = actual.Low.High.U;
            int result3 = actual.High.Low.U;
            int result4 = actual.High.High.U;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.And did not return the expected value1.");

            Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.And did not return the expected value2.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.And did not return the expected value3.");
            Assert.AreEqual(0, result4, "BddSharp.Kernel.Kernel.And did not return the expected value1.");
        }

        ///// <summary>
        /////A test for Exists (List&lt;int&gt;, Bdd)
        /////</summary>
        //[TestMethod()]
        //public void ExistsTest1()
        //{
        //    System.Collections.Generic.List<int> varList = new List<int>();
        //    varList.Add(1);
        //    varList.Add(3);

        //    Bdd root = new Bdd(true);

        //    for (int i = 0; i <= 3; i++)
        //    {
        //        root = Kernel.And(root, new Bdd(i));
        //    }

        //    Bdd actual;

        //    actual = BddSharp.Kernel.Kernel.Exists(varList, root);

        //    int result1 = actual.Low.U;
        //    int result2 = actual.High.Low.U;
        //    int result3 = actual.High.High.U;

        //    Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        //    Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        //    Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        //}

        ///// <summary>
        /////A test for ForAll (List&lt;int&gt;, Bdd)
        /////</summary>
        //[TestMethod()]
        //public void ForAllTest1()
        //{
        //    System.Collections.Generic.List<int> varList = new List<int>();
        //    varList.Add(1);
        //    varList.Add(3);


        //    Bdd root = new Bdd(false);

        //    for (int i = 0; i <= 3; i++)
        //    {
        //        root = Kernel.Or(root, new Bdd(i));
        //    }

        //    Bdd actual;

        //    actual = BddSharp.Kernel.Kernel.ForAll(varList, root);

        //    int result1 = actual.High.U;
        //    int result2 = actual.Low.Low.U;
        //    int result3 = actual.Low.High.U;

        //    Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        //    Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        //    Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        //}

        /// <summary>
        ///A test for Restrict (Bdd, int, bool)
        ///</summary>
        [TestMethod()]
        public void RestrictTest()
        {
            Bdd root = new Bdd(false);

            for (int i = 0; i <= 2; i++)
            {
                root = Kernel.Or(root, new Bdd(i));
            }

            int restrictVar = 1;

            bool restrictValue = false;

            Bdd actual;

            actual = BddSharp.Kernel.Kernel.Restrict(root, restrictVar, restrictValue);

            int result1 = actual.High.U;
            int result2 = actual.Low.Low.U;
            int result3 = actual.Low.High.U;

            Assert.AreEqual(1, result1, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
            Assert.AreEqual(0, result2, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
            Assert.AreEqual(1, result3, "BddSharp.Kernel.Kernel.Exists did not return the expected value.");
        }

        /// <summary>
        ///A test for AnySat (Bdd)
        ///</summary>
        [TestMethod()]
        public void AnySatTest()
        {
            Bdd root = new Bdd(false);

            for (int i = 0; i <= 2; i++)
            {
                root = Kernel.Or(root, new Bdd(i));
            }

            string expected = "var0=0 var1=0 var2=1 []";
            string actual;

            actual = BddSharp.Kernel.Kernel.AnySat(root);

            Assert.AreEqual(expected, actual, "BddSharp.Kernel.Kernel.AnySat did not return the expected value.");
        }

        ///// <summary>
        /////A test for Compose (Bdd, List&lt;int&gt;, List&lt;int&gt;)
        /////</summary>
        //[TestMethod()]
        //public void ComposeTest()
        //{
        //    Bdd t = Kernel.And(new Bdd(1), new Bdd(2));

        //    System.Collections.Generic.List<int> tp = new List<int>();
        //    tp.Add(3);
        //    tp.Add(4);

        //    System.Collections.Generic.List<int> x = new List<int>();
        //    x.Add(1);
        //    x.Add(2);

        //    Bdd actual;

        //    actual = BddSharp.Kernel.Kernel.Compose(t, tp, x);

        //    int result1 = actual.High.Low.U;
        //    int result2 = actual.High.High.U;
        //    int result3 = actual.Low.U;
        //    int result4 = actual.Var;
        //    int result5 = actual.High.Var;

        //    Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.Compose did not return the expected value.");
        //    Assert.AreEqual(1, result2, "BddSharp.Kernel.Kernel.Compose did not return the expected value.");
        //    Assert.AreEqual(0, result3, "BddSharp.Kernel.Kernel.Compose did not return the expected value.");
        //    Assert.AreEqual(3, result4, "BddSharp.Kernel.Kernel.Compose did not return the expected value.");
        //    Assert.AreEqual(4, result5, "BddSharp.Kernel.Kernel.Compose did not return the expected value.");

        //}

        /// <summary>
        ///A test for Compose (Bdd, int, int)
        ///</summary>
        [TestMethod()]
        public void ComposeTest1()
        {
            Bdd t = Kernel.And(new Bdd(1), new Bdd(2));

            int tp = 3;

            int x = 1;

            Bdd expected = new Bdd(true);
            Bdd actual;

            actual = BddSharp.Kernel.Kernel.Compose(t, tp, x);

            int result1 = actual.High.Low.U;
            int result2 = actual.Low.U;
            int result3 = actual.High.High.U;
            int result4 = actual.High.Var;

            Assert.AreEqual(0, result1, "BddSharp.Kernel.Kernel.Compose did not return the expected value.");
            Assert.AreEqual(0, result2);
            Assert.AreEqual(1, result3);
            Assert.AreEqual(3, result4);
        }
    }
}
