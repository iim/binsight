
namespace APKInsight.Logic.Analysis.Data
{
    public class LibraryDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string RegExPattern { get; set; }
        public bool CloseLibrary { get; set; }
    }
}
