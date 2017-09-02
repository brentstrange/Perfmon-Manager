using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace PerfMonManager
{
    public class Counters : Counter
    {
        public PerformanceCounter[] list(string category, string instanceName = null)
        {
            PerformanceCounter[] counters = new PerformanceCounter[] { };

            try
            {
                PerformanceCounterCategory perfCategory = new PerformanceCounterCategory(category);

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

        public void create(string category, string categoryHelp,
            PerformanceCounterCategoryType categoryType,
            CounterCreationDataCollection countCreationData)
        {
            try
            {
                PerformanceCounterCategory.Create(
                    category,
                    categoryHelp,
                    categoryType,
                    countCreationData
                    );
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
