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
    }
}
