using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RNRAlphaReport.Common;
using RNRAlphaReport.DataAccess.DatabaseUtil;
using RNRAlphaReport.Models;
using RNRAlphaReport.Services;
using System.Data;
using System.Diagnostics;


namespace RNRAlphaReport.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDatabaseUtil databaseUtil;
        private readonly ReportBuilderService reportBuilderService;
        private const int PAGE_SIZE = 10;

        public HomeController(ILogger<HomeController> logger, IDatabaseUtil databaseUtil, ReportBuilderService reportBuilderService)
        {
            _logger = logger;
            this.databaseUtil = databaseUtil;
            this.reportBuilderService = reportBuilderService;
        }

        public IActionResult Index(string? SearchString = null)
        {
            var allReports = reportBuilderService.GetAllReportsOrById();
            var gen_ledgersReport = allReports.Where(a => a.Id == new Guid("{C242E675-D4E5-EE11-88A6-000D3AB06F75}")).FirstOrDefault();
            if (gen_ledgersReport == null)
                return NotFound();

            foreach (var param in gen_ledgersReport.Parameters)
            {
                if (param.Key.ToLower() == "size")
                    param.Value = 10;
                else if (param.Key.ToLower() == "searchkey")
                    param.Value = "%";
            }
            var datatable = reportBuilderService.GetReportData(gen_ledgersReport);
            return View(datatable);
        }


        [HttpPost]
        public IActionResult Export(string? SearchString = null)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                DataTable dt = this.GetLedgerData(SearchString, int.MaxValue);
                if (string.IsNullOrWhiteSpace(dt.TableName))
                    dt.TableName = "LedgerData"; // Set a default table name if not set
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        public DataTable GetLedgerData(string? SearchString = null, int rowsnumber = 100)
        {


//            string SQL = @"
//--,Gen_Ledgers.A5,Gen_Ledgers.A6,Gen_Ledgers.A7,Gen_Ledgers.A8,C1,C2,C3,C4,C5
//                      select 
// Top (@size)                           DOC_DATE,YEAR,PERIOD_NO,Gen_Ledgers.TR_CODE,Gen_Ledgers.TRAN_NO,TR_TYPE.NAME,Gen_Ledgers.ACC_NO,ACC_MASTER_FILE.ACC_NAME
//                           ,Debit,Credit,L_Debit,L_Credit,DOC_NO,Describt, Gen_Ledgers.A1,Gen_Ledgers.A2,Gen_Ledgers.A3,Gen_Ledgers.A4
//                           ,Gen_Ledgers.Created_BY
//                           --,Gen_Ledgers.Created_Date
//                           from RNR.Gen_Ledgers
//                           inner join RNR.Tr_Type on TR_TYPE.TR_CODE=Gen_Ledgers.TRAN_NO
//                           inner join RNR.ACC_MASTER_FILE on ACC_MASTER_FILE.ACC_NO=Gen_Ledgers.ACC_NO
// where 1<2             
//order by DOC_DATE,Year,Period_NO,Gen_Ledgers.TR_CODE,Gen_Ledgers.TRAN_no,Gen_Ledgers.LINE_NO Desc";

//            var _params = new Dictionary<string, object>();
//            _params.Add("@size", rowsnumber);

//            if (!string.IsNullOrEmpty(SearchString))
//            {
//                SQL = SQL.Replace("1<2", "(  Cast(Gen_Ledgers.ACC_NO  AS VARCHAR2(30)) like '%@Searchkey%'  or ACC_MASTER_FILE.ACC_NAME like '%@Searchkey%'  or Describt like '%@Searchkey%' )");
//                _params.Add("@Searchkey", SearchString);

//            }

//            DataTable dt = databaseUtil.ExecuteQuery(SQL, _params);
            return null;

        }




        public IActionResult ShowPaging(ShowPaging model,
                           int page = 1, int inputNumber = 1)
        {

            var displayResult = new List<string>();
            string message;

            //set model.pageinfo
            model.PageInfo = new PageInfo();
            model.PageInfo.CurrentPage = page;
            model.PageInfo.ItemsPerPage = PAGE_SIZE;
            model.PageInfo.TotalItems = inputNumber;

            //Set model.displayresult - numbers list
            for (int count = model.PageInfo.PageStart;
                     count <= model.PageInfo.PageEnd; count++)
            {
                message = count.ToString();
                displayResult.Add(message.Trim());
            }
            model.DisplayResult = displayResult;

            //return view model
            return View(model);
        }

        public IActionResult Privacy(string? SearchString = null)
        {

            return View(GetTrailBalance(SearchString));
        }

        [HttpPost]
        public IActionResult ExportTRial(string? SearchString = null)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                DataTable dt = this.GetTrailBalance(SearchString, int.MaxValue);
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

        public DataTable GetTrailBalance(string? SearchString = null, int rowsnumber = 100)
        {


//            string SQL = @"
//     select 
//top(@size)
//                           RNR.ACC_MASTER_FILE.ACC_NAME
//                         ,Acc_BAL.*
                          
//                           from RNR.ACC_BAL
//                           inner join RNR.ACC_MASTER_FILE on ACC_MASTER_FILE.ACC_NO=ACC_BAL.ACC_NO
//                           where 1<2 
//             order by Year ,Period_NO Desc
//";

//            var _params = new Dictionary<string, object>();
//            _params.Add("@size", rowsnumber);


//            if (!string.IsNullOrEmpty(SearchString))
//            {

//                SQL = SQL.Replace("1<2", "(  Cast(ACC_BAL.ACC_NO  AS VARCHAR2(30)) like '%@Searchkey%'  or ACC_MASTER_FILE.ACC_NAME like '%@Searchkey%' )");
//                _params.Add("@Searchkey", SearchString);
//            }


//            DataTable dt = databaseUtil.ExecuteQuery(SQL, _params);
            return null;

        }


        public IActionResult CustomerBalance(string? SearchString = null)
        {


            return View(GetTrailBalance(SearchString));
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}