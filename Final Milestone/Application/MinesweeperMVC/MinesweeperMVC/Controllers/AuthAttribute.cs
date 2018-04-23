/*
 * @authors:   Kaleb Eberhart
 * @date:      04/22/2018
 * @course:    CST-247
 * @professor: Mark Reha
 * @project:   Minesweeper Online 1.6
 * @file:      AuthAttribute.cs
 * @revision:  1.0
 * @synapsis:  This attribute is used for page security throughout my application.
 */

using System.Web.Mvc;

namespace MinesweeperMVC.Controllers
{
    public class AuthAttribute : FilterAttribute, IAuthorizationFilter
    {
        void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
        {
            //When users log in, Session["Logged"] is set to true. It is abandoned when they log out, making it null.
            if (filterContext.HttpContext.Session["Logged"] == null)
            {
                filterContext.Result = new RedirectResult("/Login");
            }
        }
    }
}