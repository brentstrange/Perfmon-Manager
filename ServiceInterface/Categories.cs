using System;
using System.Diagnostics;

namespace PerfMonManager
{
    public class Categories : Category
    {
        public PerformanceCounterCategory[] getAll()
        {
            var categories = new PerformanceCounterCategory[] { };

            try
            {
               categories = PerformanceCounterCategory.GetCategories();
            }
            catch (Exception)
            {
                throw;
            }

            return categories;
        }

        public void delete(string category)
        {
            try
            {
                PerformanceCounterCategory.Delete(category);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
