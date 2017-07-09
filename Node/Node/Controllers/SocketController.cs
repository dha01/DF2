using Core.Model.OpenInterfaces.Node;
using Core.Model.OpenInterfaces.Node.DataModel;
using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class SocketController : Controller
	{
		public SocketController()
		{
		}

		public ActionResult Index()
		{
			return View();
		}
	}
}
