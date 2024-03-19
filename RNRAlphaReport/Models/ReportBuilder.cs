namespace RNRAlphaReport.Models
{
    public class ReportBuilder
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ReportBuilderQueryTypes? QueryType { get; set; }
        public string DataProviderId { get; set; }
        public string DataProviderName { get; set; }
        public string ReportQuery { get; set; }
        public IList<ReportBuilderParameter> Parameters { get; set; }
        public IDictionary<string, object> ParametersAsDictionary
        {
            get
            {
                return Parameters.ToDictionary(a => a.Key, a => a.Value);
            }
        }
    }

    public enum ReportBuilderQueryTypes
    {
        SQLQuery = 1,
        StoredProcedure = 2
    }
}
