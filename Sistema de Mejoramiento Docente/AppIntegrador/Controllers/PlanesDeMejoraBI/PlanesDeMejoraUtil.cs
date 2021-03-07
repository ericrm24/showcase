using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using AppIntegrador.Models;
using System.IO;

namespace AppIntegrador.Controllers.PlanesDeMejoraBI
{
    public class PlanesDeMejoraUtil
    {
        private string RenderViewToString(PartialViewResult partialView, ControllerContext controllerContext)
        {
            //Sacado de https://blog.johnnyreilly.com/2015/03/partialview-tostring.html
            using (var sw = new StringWriter())
            {
                partialView.View = ViewEngines.Engines
                  .FindPartialView(controllerContext, partialView.ViewName).View;

                var vc = new ViewContext(
                  controllerContext, partialView.View, partialView.ViewData, partialView.TempData, sw);
                partialView.View.Render(vc, sw);

                var partialViewString = sw.GetStringBuilder().ToString();

                return partialViewString;
            }
        }

        public virtual string getView(PartialViewResult partialView, ControllerContext controllerContext)
        {
            return RenderViewToString(partialView, controllerContext);
        }
    }
}