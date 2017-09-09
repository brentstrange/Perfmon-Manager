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
            Assert.AreEqual($"Instance {invalidInstance} does not exist in category Processor.",
                expectedExcetpion.Message);
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
            Assert.AreEqual("Category does not exist.", expectedExcetpion.Message);
        }

        [TestMethod()]
        public void createCategoryAndSingleCounterTest()
        {
            string categoryName = "foo-category";
            deleteCategory(categoryName);

            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData();
            ccd.CounterName = "foo-counter";
            ccd.CounterHelp = $"{ccd.CounterName}-help";
            ccd.CounterType = PerformanceCounterType.NumberOfItems64;
            counters.Add(ccd);

            try
            {
                new Counters().create(categoryName, $"{categoryName}-help",
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

        [TestMethod()]
        public void deleteOneCounterTest()
        {
            string categoryName = "foo-category-delete-one-counter";
            HashSet < String > counterNames = new HashSet<String>() {
                "foo-counter-to-delete", "foo-counter-to-remain"};

            try
            { 
                // Create a category to test
                createCategory(categoryName, counterNames);
                
                // Delete a counter in a category (copies, deletes & recreates category/counter)
                new Counters().deleteOne(categoryName, counterNames.First<String>());

                var perfCategories = new Categories().getAll();
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
                PerformanceCounter[] counters = new Counters().list(categoryName);
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
                deleteCategory(categoryName);
            }
        }

        [TestMethod()]
        public void addOneCounterTest()
        {
            string categoryName = "foo-category-add-one-counter";
            HashSet<String> counterNames = new HashSet<String>() {
                "foo-counter-1", "foo-counter-2"};

            try
            {
                // Create a category to test
                createCategory(categoryName, counterNames);

                var perfCategories = new Categories().getAll();
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
                new Counters().add(categoryName, ccdc);

                // Assert recreated category
                Assert.AreEqual(categoryName, categoryRef.CategoryName);
                Assert.AreEqual($"{categoryName}-help", categoryRef.CategoryHelp);
                Assert.AreEqual(".", categoryRef.MachineName);
                Assert.AreEqual(PerformanceCounterCategoryType.SingleInstance,
                    categoryRef.CategoryType);

                // Assert recreated counter and new addition
                PerformanceCounter[] counters = new Counters().list(categoryName);
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
                deleteCategory(categoryName);
            }

        }

        private void createCategory(string categoryName, HashSet<String> counterNames)
        {
            deleteCategory(categoryName);
            CounterCreationDataCollection counters = new CounterCreationDataCollection();

            foreach (String counterName in counterNames)
            {  
                CounterCreationData ccd = new CounterCreationData();
                ccd.CounterName = counterName;
                ccd.CounterHelp = $"{counterName}-help";
                ccd.CounterType = PerformanceCounterType.NumberOfItems64;
                counters.Add(ccd);
            }

            new Counters().create(categoryName, $"{categoryName}-help",
                PerformanceCounterCategoryType.SingleInstance, counters);
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