using System;
using System.Diagnostics;

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
    }
}
