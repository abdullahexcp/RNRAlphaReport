using RNRAlphaReport.Common;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace RNRAlphaReport.Models
{
    public class ShowPaging
    {

        //validation for required, only numbers, allowed range-1 to 500
      
        public int InputNumber { get; set; }

        public List<string> DisplayResult { get; set; } = new();

        public PageInfo PageInfo=new();
    }
}
