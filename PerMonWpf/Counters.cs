using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerfMonManager;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace PerMonWpf
{
    //https://blogs.u2u.be/diederik/post/Codeless-two-way-data-binding-to-a-WPF-DataGrid
    //http://www.dotnetforall.com/addeditdelete-datagrid-using-master-details-view-wpf/

    public class Counters
    {
        public static ObservableCollection<PerformanceCounter> GetTCPv6
        {
            get
            {
                PerfMonManager.Counters counters = new PerfMonManager.Counters();
                PerformanceCounter[] pcArray = new PerformanceCounter[] { };
                pcArray = counters.List("WFP");

                ObservableCollection<PerformanceCounter> performanceCounter =
                    new ObservableCollection<PerformanceCounter>(){ };

                foreach (PerformanceCounter pc in pcArray)
                {
                    performanceCounter.Add(pc);
                }

                return performanceCounter;
            }
        }
    }

    public class Categories
    {
        public static ObservableCollection<PerformanceCounterCategory> GetAll
        {
            get
            {
                PerfMonManager.Categories categories = new PerfMonManager.Categories();
                PerformanceCounterCategory[] pcArray = new PerformanceCounterCategory[] { };
                pcArray = categories.GetAll();

                ObservableCollection<PerformanceCounterCategory> performancCounterCategory =
                    new ObservableCollection<PerformanceCounterCategory>() { };

                foreach (PerformanceCounterCategory pcc in pcArray)
                {
                    performancCounterCategory.Add(pcc);
                }

                return performancCounterCategory;

            }
        }

    }
}
