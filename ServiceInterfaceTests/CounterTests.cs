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

        [TestMethod()]
        public void createCategoryAndSingleCounterTest()
        {
            string categoryName = "foo-category";
            deleteCategory(categoryName);

            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData();
            ccd.CounterName = "foo-counter";
            ccd.CounterHelp = "foo-counter-help";
            ccd.CounterType = PerformanceCounterType.NumberOfItems64;
            counters.Add(ccd);

            try
            {
                new Counters().create(categoryName, "foo-category-help",
                    PerformanceCounterCategoryType.SingleInstance, counters);
                Assert.IsTrue(PerformanceCounterCategory.Exists(categoryName));
            }
            catch (Exception ex)
            {
                Assert.Fail($"Assert was not executed: {ex.Message}");
            }
            finally
            {
                if (PerformanceCounterCategory.Exists(categoryName))
                {
                    deleteCategory(categoryName);
                }
            }
        }

        [TestMethod()]
        public void createCategoryAndMultipleCounterTest()
        {
            string categoryName = "foo-category-multiple-counters";
            deleteCategory(categoryName);

            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData();
            ccd.CounterName = "foo-counter";
            ccd.CounterHelp = "foo-counter-help";
            ccd.CounterType = PerformanceCounterType.NumberOfItems64;
            counters.Add(ccd);

            CounterCreationData ccd2 = new CounterCreationData();
            ccd2.CounterName = "foo-counter-two";
            ccd2.CounterHelp = "foo-counter-help-two";
            ccd2.CounterType = PerformanceCounterType.CounterTimer;
            counters.Add(ccd2);

            try
            {
                new Counters().create(categoryName, "foo-category-help",
                    PerformanceCounterCategoryType.SingleInstance, counters);
                Assert.IsTrue(PerformanceCounterCategory.Exists(categoryName));
            }
            catch (Exception ex)
            {
                Assert.Fail($"Assert was not executed: {ex.Message}");
            }
            finally
            {
                if (PerformanceCounterCategory.Exists(categoryName))
                {
                    deleteCategory(categoryName);
                }
            }
        }

        private void deleteCategory(string categoryName)
        {
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                new Categories().delete(categoryName);
            }
        }
    }
}