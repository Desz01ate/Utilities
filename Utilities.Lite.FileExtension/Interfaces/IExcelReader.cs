﻿namespace Utilities.Interfaces
{
    /// <summary>
    /// Interface required for Excel Data Reader
    /// </summary>
    public interface IExcelReader
    {
        /// <summary>
        /// Map object property name to its index inside excel file.
        /// </summary>
        /// <param name="property">Property name per implemented class</param>
        /// <returns></returns>
        int GetExternalColumnIndex(string property);
    }
}


