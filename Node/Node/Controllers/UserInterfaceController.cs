using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Model.Commands.Build;
using Core.Model.Computing;
using Core.Model.Network.Node;
using Core.Model.Network.Node.DataModel;
using Core.Model.Network.Node.Repository;
using Core.Model.Network.WebMethod.Repository;
using Microsoft.AspNetCore.Mvc;
using Node.Static;

namespace Node.Controllers
{
	public class UserInterfaceController : Controller
	{
		public UserInterfaceController()
		{
		}

		public string ExecCode(string code)
		{
			return StaticVariables.NodeServer.InvokeCode(code);
		}

		public ActionResult Index()
		{
			var responce = @"
<html>
	<head>
		
	</head>
	<body>

	<textarea id='myTextArea'></textarea>
	<input type='button' value='Ok' onclick='httpRequestPost(""/UserInterface/ExecCode"", document.getElementById(""myTextArea"").value)'>
	<textarea id='result'></textarea>

<script>
			/* Данная функция создаёт кроссбраузерный объект XMLHTTP */
  function getXmlHttp() {
    var xmlhttp;
    try {
      xmlhttp = new ActiveXObject(""Msxml2.XMLHTTP"");
    } catch (e) {
    try {
      xmlhttp = new ActiveXObject(""Microsoft.XMLHTTP"");
    } catch (E) {
      xmlhttp = false;
    }
    }
    if (!xmlhttp && typeof XMLHttpRequest!='undefined') {
      xmlhttp = new XMLHttpRequest();
    }
    return xmlhttp;
  }
  function httpRequestPost(url, code) {
	//var a = document.getElementById(""a"").value; // Считываем значение a
    //var b = document.getElementById(""b"").value; // Считываем значение b
    var xmlhttp = getXmlHttp(); // Создаём объект XMLHTTP
    xmlhttp.open('POST', url, true); // Открываем асинхронное соединение
    xmlhttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded'); // Отправляем кодировку
    xmlhttp.send(""code="" + encodeURIComponent(code)); // Отправляем POST-запрос
    xmlhttp.onreadystatechange = function() { // Ждём ответа от сервера
      if (xmlhttp.readyState == 4) { // Ответ пришёл
        if(xmlhttp.status == 200) { // Сервер вернул код 200 (что хорошо)
          document.getElementById(""result"").innerHTML = xmlhttp.responseText; // Выводим ответ сервера
        }
      }
    };
  }

		function post(path, params, method) {
					method = method || 'post'; // Set method to post by default if not specified.

						// The rest of this code assumes you are not using a library.
						// It can be made less wordy if you use one.
						var form = document.createElement('form');
						form.setAttribute('method', method);
						form.setAttribute('action', path);

						for(var key in params) {
							if (params.hasOwnProperty(key)) {
							var hiddenField = document.createElement('input');
							hiddenField.setAttribute('type', 'hidden');
							hiddenField.setAttribute('name', key);
							hiddenField.setAttribute('value', params[key]);

							form.appendChild(hiddenField);
						}
					}

					document.body.appendChild(form);
					var x = form.submit();
				}
		</script>

	</body>
</html>
";

			return new ContentResult()
			{
				Content = responce,
				ContentType = "text/html"
			}; 
		}
	}
}
