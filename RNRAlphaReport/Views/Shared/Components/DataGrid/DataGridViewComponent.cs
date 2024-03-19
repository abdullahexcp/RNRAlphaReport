using System.Data;

using Microsoft.AspNetCore.Mvc;

namespace RNRAlphaReport.Views.Components.DataGrid
{
    public class DataGridViewComponent : ViewComponent
    {
        public record GridObject(DataTable dt, string ControllerName, string ActionName);

       

       

        public async Task<IViewComponentResult> InvokeAsync(DataTable DTObject,string ControllerName, string ActionName)
        {
            var recordGridObject =  new GridObject(DTObject, ControllerName, ActionName);
                 await Task.Run(() => {  }) ;
            return View(recordGridObject);
        }




    }
}
