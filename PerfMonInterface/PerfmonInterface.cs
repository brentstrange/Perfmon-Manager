using System.Diagnostics;

namespace PerfMonManager
{
    interface Category
    {
        PerformanceCounterCategory[] getAll(string machineName = null);

        void delete(string category);
    }

    interface Counter
    {
        PerformanceCounter[] list(string category, string instanceName = null);

        void create(string category, string categoryHelp, 
            PerformanceCounterCategoryType categoryType,
            CounterCreationDataCollection countCreationData);

        void add(string categoryName,
            CounterCreationDataCollection countCreationData,
            string instanceName = null,
            string machineName = null);

        void deleteOne(string categoryName, string counterName,
            string instanceName = null,
            string machineName = null);
    }
}
