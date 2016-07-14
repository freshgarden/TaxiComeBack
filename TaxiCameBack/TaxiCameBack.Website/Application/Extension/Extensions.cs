using System;
using System.Web.Mvc;
using System.Web.Routing;
using TaxiCameBack.Core.Constants;

namespace TaxiCameBack.Website.Application.Extension
{
    public static class Extensions
    {
        //     <nav>
        //   <ul class="pagination">
        //     <li class="disabled"><a href="#" aria-label="Previous"><span aria-hidden="true">«</span></a></li>
        //     <li class="active"><a href="#">1 <span class="sr-only">(current)</span></a></li>
        //     <li><a href="#">2</a></li>
        //     <li><a href="#">3</a></li>
        //     <li><a href="#">4</a></li>
        //     <li><a href="#">5</a></li>
        //     <li><a href="#" aria-label="Next"><span aria-hidden="true">»</span></a></li>
        //  </ul>
        //</nav>

        public static MvcHtmlString Pager(this HtmlHelper helper, int currentPage, int pageSize, int totalItemCount, object routeValues, string actionOveride = null, string controllerOveride = null)
        {
            // how many pages to display in each page group const  	
            var cGroupSize = AppConstants.PagingGroupSize;
            var pageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);

            if (pageCount <= 0)
            {
                return null;
            }

            // cleanup any out bounds page number passed  	
            currentPage = Math.Max(currentPage, 1);
            currentPage = Math.Min(currentPage, pageCount);

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var containerdiv = new TagBuilder("nav");
            var container = new TagBuilder("ul");
            container.AddCssClass("pagination");
            var actionName = !string.IsNullOrEmpty(actionOveride) ? actionOveride : helper.ViewContext.RouteData.GetRequiredString("action");
            var controllerName = !string.IsNullOrEmpty(controllerOveride) ? controllerOveride : helper.ViewContext.RouteData.GetRequiredString("controller");

            // calculate the last page group number starting from the current page  	
            // until we hit the next whole divisible number  	
            var lastGroupNumber = currentPage;
            while ((lastGroupNumber % cGroupSize != 0)) lastGroupNumber++;

            // correct if we went over the number of pages  	
            var groupEnd = Math.Min(lastGroupNumber, pageCount);

            // work out the first page group number, we use the lastGroupNumber instead of  	
            // groupEnd so that we don't include numbers from the previous group if we went  	
            // over the page count  	
            var groupStart = lastGroupNumber - (cGroupSize - 1);

            // if we are past the first page  	
            if (currentPage > 1)
            {
                var previousli = new TagBuilder("li");
                var previous = new TagBuilder("a");
                previous.SetInnerText("«");
                previous.AddCssClass("previous");
                var routingValues = new RouteValueDictionary(routeValues) { { "p", currentPage - 1 } };
                previous.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routingValues));
                previousli.InnerHtml = previous.ToString();
                container.InnerHtml += previousli;
            }

            // if we have past the first page group  	
            if (currentPage > cGroupSize)
            {
                var previousDotsli = new TagBuilder("li");
                var previousDots = new TagBuilder("a");
                previousDots.SetInnerText("...");
                previousDots.AddCssClass("previous-dots");
                var routingValues = new RouteValueDictionary(routeValues) { { "p", groupStart - cGroupSize } };
                previousDots.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routingValues));
                previousDotsli.InnerHtml = previousDots.ToString();
                container.InnerHtml += previousDotsli.ToString();
            }

            for (var i = groupStart; i <= groupEnd; i++)
            {
                var pageNumberli = new TagBuilder("li");
                pageNumberli.AddCssClass(((i == currentPage)) ? "active" : "p");
                var pageNumber = new TagBuilder("a");
                pageNumber.SetInnerText((i).ToString());
                var routingValues = new RouteValueDictionary(routeValues) { { "p", i } };
                pageNumber.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routingValues));
                pageNumberli.InnerHtml = pageNumber.ToString();
                container.InnerHtml += pageNumberli.ToString();
            }

            // if there are still pages past the end of this page group  	
            if (pageCount > groupEnd)
            {
                var nextDotsli = new TagBuilder("li");
                var nextDots = new TagBuilder("a");
                nextDots.SetInnerText("...");
                nextDots.AddCssClass("next-dots");
                var routingValues = new RouteValueDictionary(routeValues) { { "p", groupEnd + 1 } };
                nextDots.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routingValues));
                nextDotsli.InnerHtml = nextDots.ToString();
                container.InnerHtml += nextDotsli.ToString();
            }

            // if we still have pages left to show  	
            if (currentPage < pageCount)
            {
                var nextli = new TagBuilder("li");
                var next = new TagBuilder("a");
                next.SetInnerText("»");
                next.AddCssClass("next");
                var routingValues = new RouteValueDictionary(routeValues) { { "p", currentPage + 1 } };
                next.MergeAttribute("href", urlHelper.Action(actionName, controllerName, routingValues));
                nextli.InnerHtml = next.ToString();
                container.InnerHtml += nextli.ToString();
            }
            containerdiv.InnerHtml = container.ToString();
            return MvcHtmlString.Create(containerdiv.ToString());
        }
    }
}