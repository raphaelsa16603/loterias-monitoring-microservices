using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Loterias.CaixaClientLib.Exceptions
{
    public class CaixaApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseContent { get; }

        public CaixaApiException(HttpStatusCode statusCode, string responseContent)
            : base($"Erro ao consultar API da Caixa (Status: {(int)statusCode})")
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}

