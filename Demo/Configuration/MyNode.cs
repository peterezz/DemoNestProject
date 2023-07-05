using Demo.Models;

namespace Demo.Configuration
{
    public class MyNode
    {
        public string IndexName { get; set; } = nameof(demoindex);
        public bool AutoMap { get; set; } = true;
        public Mapping? Mapping { get; set; }
        public string Url { get; set; } = "https://197.50.203.156:9200";
        public string UserName { get; set; } = "elastic";
        public string Password { get; set; } = "kRG1Ss-IgseVkvkuVl+O";
    }

    public class Mapping
    {
        public string MappingFilePath { get; set; } = string.Empty;
    }
}