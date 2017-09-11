using System;
using System.Diagnostics;
using System.Linq;

namespace PerfMonManager
{
    public class Counters : ICounter
    {
        /// <summary>
        /// List PerformanceCounters in a category
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <param name="instanceName">Category instance name.</param>
        /// <returns>Array of PerformanceCounter.</returns>
        public PerformanceCounter[] List(string categoryName, string instanceName = null)
        {
            PerformanceCounter[] counters = new PerformanceCounter[] { };
            String[] instanceNames = new Categories().GetInstanceNames(categoryName);

            try
            {
                PerformanceCounterCategory perfCategory = 
                    new PerformanceCounterCategory(categoryName);

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
                // TODO, some return nothing even though I see them in Perfmon
                //https://social.msdn.microsoft.com/Forums/vstudio/en-US/7992f5dc-b3a6-4e87-bc56-bdd1e3c898b7/performancecountercategory-getinstancenames-do-not-return-all-instances-smb-client-shares?forum=csharpgeneral
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
        public void Create(string categoryName, string categoryHelp,
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
        /// <param name="categoryName">The category name to Delete a counter from.</param>
        /// <param name="counterName">The counter name to Delete.</param>
        /// <param name="instanceName">The category instance name.</param>
        /// <param name="machineName">The machine name the category exists on.</param>
        public void DeleteOne(string categoryName, string counterName, string instanceName=null,
            string machineName=null)
        {
            CategoryCounter categoryCounter = 
                CopyCategoryAndCounters(categoryName, instanceName, machineName);

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
            new Categories().Delete(categoryName);
            
            // Recreate the category with the counters (minus the the "deleted" one)
            new Counters().Create(categoryName, categoryCounter.CategoryData.CategoryHelp,
                categoryCounter.CategoryData.CategoryType, finalCounters);    
        }

        /// <summary>
        /// Add counter(s) to a category
        /// </summary>
        /// <param name="categoryName">Name of category to Add the counter to.</param>
        /// <param name="counterCreationDataCollection">Collection of counter creation data.</param>
        /// <param name="instanceName">The category instance name.</param>
        /// <param name="machineName">The machine name to Add the counters to.</param>
        public void Add(string categoryName,
            CounterCreationDataCollection counterCreationDataCollection,
            string instanceName = null,
            string machineName = null)
        {

            CategoryCounter categoryCounter =
                CopyCategoryAndCounters(categoryName, instanceName, machineName);

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
            new Categories().Delete(categoryName);

            // Recreate the category with the counters (minus the the "deleted" one)
            new Counters().Create(categoryName, categoryCounter.CategoryData.CategoryHelp,
                categoryCounter.CategoryData.CategoryType, finalCounters);
        }

        private CategoryCounter CopyCategoryAndCounters(string categoryName, 
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
                arrOfCatagories = categories.GetAll();
            }
            else
            {
                arrOfCatagories = categories.GetAll(machineName);
            }

            var categoryRef =
                arrOfCatagories
                .Where(x => x.CategoryName == categoryName).ToArray().First();

            cd.CategoryName = categoryRef.CategoryName;
            cd.CategoryHelp = categoryRef.CategoryHelp;
            cd.CategoryType = categoryRef.CategoryType;
            cc.CategoryData = cd;

            // Get the category and list of counters
            if (String.IsNullOrEmpty(instanceName))
            {
                copiedCounters = counters.List(categoryName);
            }
            else
            {
                copiedCounters = counters.List(categoryName, instanceName);
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
            public CategoryData CategoryData { get; set; }
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
