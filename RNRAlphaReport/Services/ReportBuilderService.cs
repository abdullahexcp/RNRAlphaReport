using RNRAlphaReport.DataAccess.DatabaseUtil;
using RNRAlphaReport.Models;
using System.Data;

namespace RNRAlphaReport.Services
{
    public class ReportBuilderService
    {
        private readonly IDatabaseUtil databaseUtil;
        private readonly SettingsService settingsService;
        private List<ReportBuilder> _allReportBuilders;
        private const string reportBuilderProviderName = "RNR_MSCRM";
        private readonly string reportBuilderConnectionString;

        public ReportBuilderService(IDatabaseUtil databaseUtil, SettingsService settingsService)
        {
            this.databaseUtil = databaseUtil;
            this.settingsService = settingsService;
            reportBuilderConnectionString = settingsService.GetConnectionString(reportBuilderProviderName);
        }


        public List<ReportBuilder> GetAllReportsOrById(Guid? Id = null)
        {
            if (_allReportBuilders != null)
                return _allReportBuilders;

            _allReportBuilders = new List<ReportBuilder>();

            var query = @"select 
                            new_customreportdsid Id,
                            r.new_name reportName,
                            r.new_category categoryId,
                            c.new_name categoryName,
                            r.new_querytype queryType,
                            d.new_customreportproviderId dataProviderId,
                            d.new_name dataProviderName,
                            r.new_reportquery reportQuery

                        from new_customreportds r 
                        join new_customreportsdscategory c on r.new_category = c.new_customreportsdscategoryid 
                        join new_customreportprovider d on r.new_dataprovider = d.new_customreportproviderId
                          
                        where r.statecode = 0" + (Id != null ? $" and new_customreportdsid = '{Id}'" : string.Empty);



            var allReportsDataTable = databaseUtil.ExecuteQuery(reportBuilderConnectionString, query);

            foreach (DataRow row in allReportsDataTable.Rows)
            {
                _allReportBuilders.Add(new ReportBuilder
                {
                    Id = new Guid(row["Id"].ToString()),
                    Name = row["reportName"].ToString(),
                    CategoryId = row["categoryId"] != null ? new Guid(row["categoryId"].ToString()) : null,
                    CategoryName = row["categoryName"].ToString(),
                    DataProviderId = row["dataProviderId"].ToString(),
                    DataProviderName = row["dataProviderName"].ToString(),
                    QueryType = row["queryType"] != null ? (ReportBuilderQueryTypes)(int)row["queryType"] : null,
                    ReportQuery = row["reportQuery"].ToString()
                });
            }

            return _allReportBuilders;
        }

        public ReportBuilder PopulateGetReportParams(ReportBuilder reportBuilder)
        {
            var query = @"select 
                            p.new_customreportdsparameterid [Id],
							p.new_nameparameter [Name],
                            p.new_name [Key],
                            p.new_type [Type],
                            p.new_order [Order],
                            p.new_section [SectionId]

                        from new_new_customreportds_new_customreportdspa rp
                        join new_customreportdsparameter p on rp.new_customreportdsparameterid = p.new_customreportdsparameterid
                        
                        where p.statecode = 0 and rp.new_customreportdsid = '" + reportBuilder.Id + "'";

            var connString = settingsService.GetConnectionString("RNR_CRM_ConnString");

            var reportParamsDataTable = databaseUtil.ExecuteQuery(reportBuilderConnectionString, query);
            var reportBuilderParamsList = new List<ReportBuilderParameter>();
            foreach (DataRow row in reportParamsDataTable.Rows)
            {
                reportBuilderParamsList.Add(new ReportBuilderParameter
                {
                    Id = new Guid(row["Id"].ToString()),
                    Name = row["Name"].ToString(),
                    SectionId = !string.IsNullOrEmpty(row["SectionId"].ToString()) ? new Guid(row["SectionId"].ToString()) : null,
                    Key = row["Key"].ToString(),
                    Order = (int)row["Order"],
                    Type = row["Type"] != null ? (ReportBuilderParamTypes)(int)row["Type"] : null,
                    ReportBuilderId = reportBuilder.Id
                });
            }

            reportBuilder.Parameters = reportBuilderParamsList;

            return reportBuilder;
        }

        public DataTable GetReportData(ReportBuilder reportBuilder)
        {
            var reportConnString = settingsService.GetConnectionString(reportBuilder.DataProviderName);

            var datatable = databaseUtil.ExecuteQuery(reportConnString, reportBuilder.ReportQuery, reportBuilder.ParametersAsDictionary);

            return datatable;
        }
    }
}
