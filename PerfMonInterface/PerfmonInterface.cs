using System.Diagnostics;

namespace PerfMonManager
{
    interface ICategory
    {
        PerformanceCounterCategory[] GetAll(string machineName = null);

        void Delete(string category);

        string[] GetInstanceNames(string categoryName);
    }

    interface ICounter
    {
        PerformanceCounter[] List(string category, string instanceName = null);

        void Create(string category, string categoryHelp, 
            PerformanceCounterCategoryType categoryType,
            CounterCreationDataCollection countCreationData);

        void Add(string categoryName,
            CounterCreationDataCollection countCreationData,
            string instanceName = null,
            string machineName = null);

        void DeleteOne(string categoryName, string counterName,
            string instanceName = null,
            string machineName = null);
    }
}
