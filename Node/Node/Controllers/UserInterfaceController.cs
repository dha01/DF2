using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class UserInterfaceController : Controller
	{
		public UserInterfaceController()
		{
		}

		public string ExecCode(string code, string input)
		{
			return StaticVariables.NodeServer.InvokeCode(code, input);
		}

		public ActionResult Index()
		{
			return View(); 
		}
	}
}
