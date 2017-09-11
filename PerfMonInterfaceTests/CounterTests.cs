using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


namespace PerfMonManager.Tests
{
    [TestClass()]
    public class CounterTests
    {
        [TestMethod()]
        public void ListCategoryWithoutInstanceTest()
        {
            PerformanceCounter[] counters = new Counters().List("TCPv4");
            Assert.IsTrue(counters.Length > 1);
        }

        [TestMethod()]
        public void ListCategoryWithInstanceTest()
        {
            PerformanceCounter[] counters = new Counters().List("Processor", "0");
            Assert.IsTrue(counters.Length > 1);
        }

        [TestMethod()]
        public void ListCategoryWithInvalidInstanceTest()
        {
            string invalidInstance = "foo";
            Exception expectedExcetpion = null;

            try
            {
                new Counters().List("Processor", invalidInstance);
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual($"Instance {invalidInstance} does not exist in category Processor.",
                expectedExcetpion.Message);
        }

        [TestMethod()]
        public void ListNonExistentCategoryTest()
        {
            Exception expectedExcetpion = null;

            try
            {
                new Counters().List("foo");
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual("Category does not exist.", expectedExcetpion.Message);
        }

        [TestMethod()]
        public void CreateCategoryAndSingleCounterTest()
        {
            string categoryName = "foo-category";
            DeleteCategory(categoryName);

            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData() {
                CounterName = "foo-counter",
                CounterHelp = "foo-counter-help",
                CounterType = PerformanceCounterType.NumberOfItems64
            };
            counters.Add(ccd);

            try
            {
                new Counters().Create(categoryName, $"{categoryName}-help",
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
                    DeleteCategory(categoryName);
                }
            }
        }

        [TestMethod()]
        public void CreateCategoryAndMultipleCounterTest()
        {
            string categoryName = "foo-category-multiple-counters";
            DeleteCategory(categoryName);

            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData()
            {
                CounterName = "foo-counter",
                CounterHelp = "foo-counter-help",
                CounterType = PerformanceCounterType.NumberOfItems64
            };
            counters.Add(ccd);

            CounterCreationData ccd2 = new CounterCreationData() {
                CounterName = "foo-counter-two",
                CounterHelp = "foo-counter-help-two",
                CounterType = PerformanceCounterType.CounterTimer
            };
            counters.Add(ccd2);

            try
            {
                new Counters().Create(categoryName, "foo-category-help",
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
                    DeleteCategory(categoryName);
                }
            }
        }

        [TestMethod()]
        public void DeleteOneCounterTest()
        {
            string categoryName = "foo-category-Delete-one-counter";
            HashSet < String > counterNames = new HashSet<String>() {
                "foo-counter-to-Delete", "foo-counter-to-remain"};

            try
            { 
                // Create a category to test
                CreateCategory(categoryName, counterNames);
                
                // Delete a counter in a category (copies, deletes & recreates category/counter)
                new Counters().DeleteOne(categoryName, counterNames.First<String>());

                var perfCategories = new Categories().GetAll();
                var categoryRef =
                    perfCategories
                    .Where(x => x.CategoryName == categoryName).ToArray().First();

                // Assert recreated category
                Assert.AreEqual(categoryName, categoryRef.CategoryName);
                Assert.AreEqual($"{categoryName}-help", categoryRef.CategoryHelp);
                Assert.AreEqual(".", categoryRef.MachineName);
                Assert.AreEqual(PerformanceCounterCategoryType.SingleInstance, 
                    categoryRef.CategoryType);

                // Assert recreated counter and one removed
                PerformanceCounter[] counters = new Counters().List(categoryName);
                Assert.AreEqual(1, counters.Length);
                Assert.AreEqual(counterNames.Last(), counters[0].CounterName);
                Assert.AreEqual($"{counterNames.Last()}-help", counters[0].CounterHelp);
                Assert.AreEqual(PerformanceCounterType.NumberOfItems64, counters[0].CounterType);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Assert was not executed: {ex.Message}");
            }
            finally { 
                DeleteCategory(categoryName);
            }
        }

        [TestMethod()]
        public void AddOneCounterTest()
        {
            string categoryName = "foo-category-Add-one-counter";
            HashSet<String> counterNames = new HashSet<String>() {
                "foo-counter-1", "foo-counter-2"};

            try
            {
                // Create a category to test
                CreateCategory(categoryName, counterNames);

                var perfCategories = new Categories().GetAll();
                var categoryRef =
                    perfCategories
                    .Where(x => x.CategoryName == categoryName).ToArray().First();

                // Add a counter
                CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
                CounterCreationData ccd = new CounterCreationData();
                string AddCounterName = "foo-counter-3";
                ccd.CounterName = AddCounterName;
                ccd.CounterHelp = $"{AddCounterName}-help";
                ccd.CounterType = PerformanceCounterType.NumberOfItems64;
                ccdc.Add(ccd);
                new Counters().Add(categoryName, ccdc);

                // Assert recreated category
                Assert.AreEqual(categoryName, categoryRef.CategoryName);
                Assert.AreEqual($"{categoryName}-help", categoryRef.CategoryHelp);
                Assert.AreEqual(".", categoryRef.MachineName);
                Assert.AreEqual(PerformanceCounterCategoryType.SingleInstance,
                    categoryRef.CategoryType);

                // Assert recreated counter and new addition
                PerformanceCounter[] counters = new Counters().List(categoryName);
                Assert.AreEqual(3, counters.Length);
                Assert.AreEqual(AddCounterName, counters[2].CounterName);
                Assert.AreEqual($"{AddCounterName}-help", counters[2].CounterHelp);
                Assert.AreEqual(PerformanceCounterType.NumberOfItems64, counters[2].CounterType);         
            }
            catch (Exception ex)
            {
                Assert.Fail($"Assert was not executed: {ex.Message}");
            }
            finally
            {
                DeleteCategory(categoryName);
            }

        }

        private void CreateCategory(string categoryName, HashSet<String> counterNames)
        {
            DeleteCategory(categoryName);
            CounterCreationDataCollection counters = new CounterCreationDataCollection();

            foreach (String counterName in counterNames)
            {  
                CounterCreationData ccd = new CounterCreationData();
                ccd.CounterName = counterName;
                ccd.CounterHelp = $"{counterName}-help";
                ccd.CounterType = PerformanceCounterType.NumberOfItems64;
                counters.Add(ccd);
            }

            new Counters().Create(categoryName, $"{categoryName}-help",
                PerformanceCounterCategoryType.SingleInstance, counters);
        }

        private void DeleteCategory(string categoryName)
        {
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                new Categories().Delete(categoryName);
            }
        }
    }
}