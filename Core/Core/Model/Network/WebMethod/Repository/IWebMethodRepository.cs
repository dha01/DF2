using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.Network.WebMethod.Repository
{
    public interface IWebMethodRepository
    {
	    TOut SendRequest<TIn, TOut>(string uri, TIn input, int timeout = 1000) where TIn : class;

	    TOut SendRequest<TOut>(string uri, int timeout = 1000);
    }
}
