using System;
using System.Diagnostics;

namespace PerfMonManager
{
    public class Categories : Category
    {
        /// <summary>
        /// Get all categories
        /// </summary>
        /// <param name="machineName">Machine name to get categories from.</param>
        /// <returns>Array of PerformanceCounterCategory</returns>
        public PerformanceCounterCategory[] getAll(string machineName = null)
        {
            var categories = new PerformanceCounterCategory[] { };

            try
            {
                if(String.IsNullOrEmpty(machineName))
                {
                    categories = PerformanceCounterCategory.GetCategories();
                }
                else
                {
                    categories = PerformanceCounterCategory.GetCategories(machineName);
                }
                
            }
            catch (Exception)
            {
                throw;
            }

            return categories;
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="categoryName">Category name to delete</param>
        public void delete(string categoryName)
        {
            try
            {
                PerformanceCounterCategory.Delete(categoryName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get instance names for a Performance Counter Category
        /// </summary>
        /// <param name="categoryName">Category name to get intances for</param>
        /// <returns></returns>
        public String[] getInstanceNames(string categoryName)
        {
            try
            { 
                PerformanceCounterCategory pcc = new PerformanceCounterCategory(categoryName);
                return pcc.GetInstanceNames();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
