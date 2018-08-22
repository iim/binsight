using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKInsight.Logic.Analysis.Data
{
    public class ApkInfo
    {
        public int Id;
        public string Sha1;
        public string ApplicationId;
        public string Filename;
        public List<UseCase> UseCases = new List<UseCase>();
    }
}
