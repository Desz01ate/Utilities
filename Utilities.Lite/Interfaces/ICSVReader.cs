namespace Utilities.Interfaces
{
    /// <summary>
    /// Interface required for CSV Reader.
    /// </summary>
    public interface ICsvReader
    {
        /// <summary>
        /// Content of csv data line in form of string.
        /// </summary>
        /// <param name="content"></param>
        void ReadFromCsv(string content);
    }
}