namespace RNRAlphaReport.Models
{
    public class ReportBuilderParameter
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public ReportBuilderParamTypes? Type { get; set; }
        public int? Order { get; set; }
        public Guid? SectionId { get; set; }
        public Guid ReportBuilderId { get; set; }
        public object Value { get; set;}

    }

    public enum ReportBuilderParamTypes
    {
        Guid = 1,
        DateOnly = 2,
        MultiLine = 3,
        SingleLine = 4,
        None = -1
    }
}
