using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System;

namespace PerfMonManager.Tests
{
    [TestClass()]
    public class CategoryTests
    {
        [TestMethod()]
        public void getAllCategoriesTest()
        {
            Categories cat = new Categories();
            var catArr = cat.getAll();
            Assert.IsTrue(catArr.Length >= 1);
        }

        [TestMethod()]
        public void deleteNonExistentCategoryTest()
        {
            Exception expectedExcetpion = null;

            try
            {
                new Categories().delete("foo");
            }
            catch (Exception ex)
            {
                expectedExcetpion = ex;
            }

            Assert.IsNotNull(expectedExcetpion);
            Assert.IsInstanceOfType(expectedExcetpion, typeof(InvalidOperationException));
            Assert.AreEqual(expectedExcetpion.Message, 
               "Cannot delete Performance Category because this category is not registered or is "+
               "a system category.");
        }

        [TestMethod()]
        public void deleteCategoryTest()
        {
            String categoryName = "foo-category";
            CounterCreationDataCollection counters = new CounterCreationDataCollection();
            CounterCreationData ccd = new CounterCreationData();
            ccd.CounterName = "foo-counter";
            ccd.CounterHelp = "foo-counter-help";
            ccd.CounterType = PerformanceCounterType.NumberOfItems64;
            counters.Add(ccd);

            if(!PerformanceCounterCategory.Exists(categoryName))
            {
                new Counters().create(categoryName, "foo-category-help",
                    PerformanceCounterCategoryType.SingleInstance, counters);
            }

            new Categories().delete(categoryName);
            Assert.IsFalse(PerformanceCounterCategory.Exists(categoryName));
        }
    }
}