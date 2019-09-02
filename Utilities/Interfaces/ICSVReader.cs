using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Interfaces
{
    /// <summary>
    /// Interface required for CSV Reader.
    /// </summary>
    public interface ICSVReader
    {
        /// <summary>
        /// Content of csv data line in form of string. 
        /// </summary>
        /// <param name="content"></param>
        public void ReadFromCSV(string content);
    }
}
