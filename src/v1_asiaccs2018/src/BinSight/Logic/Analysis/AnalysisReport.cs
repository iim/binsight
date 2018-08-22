using System;
using System.IO;
using System.Text;

namespace APKInsight.Logic.Analysis
{
    /// <summary>
    /// Helper class to report results from an analysis step
    /// </summary>
    public class AnalysisReport
    {
        private readonly StringBuilder _reportData = new StringBuilder();
        private int _nextId = 1;
        private readonly object _lock = new object();

        public void AddLineWithCounter(string line)
        {
            lock (_lock)
            {
                if (line.EndsWith(Environment.NewLine))
                {
                    _reportData.Append(string.Format(line, _nextId));
                }
                else
                {
                    _reportData.AppendLine(string.Format(line, _nextId));
                }
                _nextId++;
            }
        }

        public void AddLineWithoutCounter(string line)
        {
            lock (_lock)
            {
                if (line.EndsWith(Environment.NewLine))
                {
                    _reportData.Append(line);
                }
                else
                {
                    _reportData.AppendLine(line);
                }
            }
        }

        public bool SaveReport(string filename)
        {
            lock (_lock)
            {
                try
                {
                    File.WriteAllText(filename, _reportData.ToString());
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
