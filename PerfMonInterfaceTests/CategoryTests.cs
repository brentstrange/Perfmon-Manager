using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;

namespace PerfMonManager.Tests
{
    [TestClass()]
    public class CategoryTests
    {
        [TestMethod()]
        public void GetAllCategoriesTest()
        {
            Categories cat = new Categories();
            var catArr = cat.GetAll();
            Assert.IsTrue(catArr.Length >= 1);
        }

        [TestMethod()]
        public void DeleteNonExistentCategoryTest()
        {
            Exception expectedExcetpion = null;

            try
            {
                new Categories().Delete("foo");
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual("Cannot delete Performance Category because this category is not " + 
                "registered or is a system category.", expectedExcetpion.Message);
        }

        [TestMethod()]
        public void DeleteCategoryTest()
        {
            String categoryName = "foo-category";
            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData() {
                CounterName = "foo-counter",
                CounterHelp = "foo-counter-help",
                CounterType = PerformanceCounterType.NumberOfItems64
            };
            counters.Add(ccd);

            if (!PerformanceCounterCategory.Exists(categoryName))
            {
                new Counters().Create(categoryName, "foo-category-help",
                    PerformanceCounterCategoryType.SingleInstance, counters);
            }

            new Categories().Delete(categoryName);
            Assert.IsFalse(PerformanceCounterCategory.Exists(categoryName));
        }

        [TestMethod()]
        public void GetInstanceNamesTest()
        {
            String[] instanceNames = new Categories().GetInstanceNames("Processor");
            Assert.AreEqual(instanceNames.Length, 3);
            Assert.AreEqual("_Total", instanceNames[0]);
            Assert.AreEqual("0", instanceNames[1]);
            Assert.AreEqual("1", instanceNames[2]);
        }

        [TestMethod()]
        public void GetInstanceNamesForNonExistentCategoryTest()
        {
            Exception expectedExcetpion = null;
            String[] instanceNames = new String[] { };

            try
            {
                instanceNames = new Categories().GetInstanceNames("foo-category");
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual("Category does not exist.", expectedExcetpion.Message);
        }
    }
}