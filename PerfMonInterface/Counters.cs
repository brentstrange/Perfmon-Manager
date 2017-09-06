using System;
using System.Diagnostics;
using System.Linq;

namespace PerfMonManager
{
    public class Counters : Counter
    {
        /// <summary>
        /// List PerformanceCounters in a category
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <param name="instanceName">Category instance name.</param>
        /// <returns>Array of PerformanceCounter.</returns>
        public PerformanceCounter[] list(string categoryName, string instanceName = null)
        {
            PerformanceCounter[] counters = new PerformanceCounter[] { };

            try
            {
                PerformanceCounterCategory perfCategory = new PerformanceCounterCategory(categoryName);

                if (String.IsNullOrEmpty(instanceName))
                {
                    counters = perfCategory.GetCounters();
                }
                else
                {
                    counters = perfCategory.GetCounters(instanceName);
                }

            }
            catch (Exception)
            {
                throw;
            }

            return counters;
        }

        /// <summary>
        /// Create a category with counters
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <param name="categoryHelp">Category help.</param>
        /// <param name="categoryType">Category type.</param>
        /// <param name="counterCreationData">Counter creation data.</param>
        public void create(string categoryName, string categoryHelp,
            PerformanceCounterCategoryType categoryType,
            CounterCreationDataCollection counterCreationData)
        {
            try
            {
                PerformanceCounterCategory.Create(
                    categoryName,
                    categoryHelp,
                    categoryType,
                    counterCreationData
                    );
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete one counter from a category by copying, deleting, modifying and recreating 
        /// the category and it's counters (since counters can't be modified).
        /// </summary>
        /// <param name="categoryName">The category name to delete a counter from.</param>
        /// <param name="counterName">The counter name to delete.</param>
        /// <param name="instanceName">The category instance name.</param>
        /// <param name="machineName">The machine name the category exists on.</param>
        public void deleteOne(string categoryName, string counterName, string instanceName=null,
            string machineName=null)
        {
            CategoryCounter categoryCounter = 
                copyCategoryAndCounters(categoryName, instanceName, machineName);

            var countersWithOneRemoved =
                categoryCounter.CounterData.Cast<CounterCreationData>()
                .SkipWhile(x => x.CounterName == counterName)
                .ToArray();

            // Copy the counters to type CounterCreationDataCollection
            CounterCreationDataCollection finalCounters = new CounterCreationDataCollection();
            foreach (CounterCreationData pc in countersWithOneRemoved)
            {
                finalCounters.Add(pc);
            }

            // Delete the category (since you can't modify counters in a category)
            new Categories().delete(categoryName);
            
            // Recreate the category with the counters (minus the the "deleted" one)
            new Counters().create(categoryName, categoryCounter.categoryData.CategoryHelp,
                categoryCounter.categoryData.CategoryType, finalCounters);    
        }

        /// <summary>
        /// Add counter(s) to a category
        /// </summary>
        /// <param name="categoryName">Name of category to add the counter to.</param>
        /// <param name="counterCreationDataCollection">Collection of counter creation data.</param>
        /// <param name="instanceName">The category instance name.</param>
        /// <param name="machineName">The machine name to add the counters to.</param>
        public void add(string categoryName,
            CounterCreationDataCollection counterCreationDataCollection,
            string instanceName = null,
            string machineName = null)
        {

            CategoryCounter categoryCounter =
                copyCategoryAndCounters(categoryName, instanceName, machineName);

            var countersToArray =
                categoryCounter.CounterData.Cast<CounterCreationData>()
                .ToArray();

            // Convert the counters to the type CounterCreationDataCollection[]
            CounterCreationDataCollection finalCounters = new CounterCreationDataCollection();
            foreach (CounterCreationData ccd in countersToArray)
            {
               finalCounters.Add(
                    new CounterCreationData(ccd.CounterName, ccd.CounterHelp, ccd.CounterType));
            }

            // Add the new counters
            finalCounters.AddRange(counterCreationDataCollection);

            // Delete the category (since you can't modify counters in a category)
            new Categories().delete(categoryName);

            // Recreate the category with the counters (minus the the "deleted" one)
            new Counters().create(categoryName, categoryCounter.categoryData.CategoryHelp,
                categoryCounter.categoryData.CategoryType, finalCounters);
        }

        private CategoryCounter copyCategoryAndCounters(string categoryName, 
            string instanceName = null, string machineName = null)
        {
            CategoryCounter cc = new CategoryCounter();
            CategoryData cd = new CategoryData();
            Categories categories = new Categories();
            Counters counters = new Counters();
            PerformanceCounter[] copiedCounters = new PerformanceCounter[] { };

            // Get the category info
            PerformanceCounterCategory[] arrOfCatagories;
            if (String.IsNullOrEmpty(machineName))
            {
                arrOfCatagories = categories.getAll();
            }
            else
            {
                arrOfCatagories = categories.getAll(machineName);
            }

            var categoryRef =
                arrOfCatagories
                .Where(x => x.CategoryName == categoryName).ToArray().First();

            cd.CategoryName = categoryRef.CategoryName;
            cd.CategoryHelp = categoryRef.CategoryHelp;
            cd.CategoryType = categoryRef.CategoryType;
            cc.categoryData = cd;

            // Get the category and list of counters
            if (String.IsNullOrEmpty(instanceName))
            {
                copiedCounters = counters.list(categoryName);
            }
            else
            {
                copiedCounters = counters.list(categoryName, instanceName);
            }

            // Convert the counters to the type CounterCreationDataCollection[]
            cc.CounterData = new CounterCreationDataCollection();
            foreach (PerformanceCounter pc in copiedCounters)
            {
                cc.CounterData.Add(
                    new CounterCreationData(pc.CounterName, pc.CounterHelp, pc.CounterType));
            }

            return cc;
        }

        private struct CategoryCounter
        {
            public CategoryData categoryData { get; set; }
            public CounterCreationDataCollection CounterData { get; set; }
        }

        private struct CategoryData
        {
            public string CategoryName { get; set; }
            public string CategoryHelp { get; set; }
            public PerformanceCounterCategoryType CategoryType { get; set; }
        }

    }
}
