using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;


namespace PerfMonManager.Tests
{
    [TestClass()]
    public class CounterTests
    {

        [TestMethod()]
        public void listCategoryWithoutInstanceTest()
        {
            PerformanceCounter[] counters = new Counters().list("TCPv4");
            Assert.IsTrue(counters.Length > 1);
        }

        [TestMethod()]
        public void listCategoryWithInstanceTest()
        {
            PerformanceCounter[] counters = new Counters().list("Processor", "0");
            Assert.IsTrue(counters.Length > 1);
        }

        [TestMethod()]
        public void listCategoryWithInvalidInstanceTest()
        {
            string invalidInstance = "foo";
            Exception expectedExcetpion = null;

            try
            {
                new Counters().list("Processor", invalidInstance);
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual(expectedExcetpion.Message, 
                $"Instance {invalidInstance} does not exist in category Processor.");
        }

        [TestMethod()]
        public void listNonExistentCategoryTest()
        {
            Exception expectedExcetpion = null;

            try
            {
                new Counters().list("foo");
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual(expectedExcetpion.Message, "Category does not exist.");
        }
    }
}