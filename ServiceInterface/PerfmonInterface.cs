using System.Diagnostics;

namespace PerfMonManager
{
    interface Category
    {
        PerformanceCounterCategory[] getAll();
        void delete(string category);
    }

    interface Counter
    {
        PerformanceCounter[] list(string category, string instanceName = null);
        void create(string category, string categoryHelp, PerformanceCounterCategoryType categoryType,
            CounterCreationDataCollection countCreationData);
    }
}
