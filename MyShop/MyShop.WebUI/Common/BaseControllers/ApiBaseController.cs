using MyShopDataAccess.SQL.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace MyShop.WebUI.Common.BaseControllers
{
    public abstract class ApiBaseController : Controller
    {
        #region Properties

        protected Cache _cache;
        protected UnitOfWorkRepositories _uow = new UnitOfWorkRepositories();
        //protected Log _logger = new Log();

        //protected string BackofficeUrl { get { return WebConfigurationManager.AppSettings["Backoffice_url"]; } }

        protected int LoggedUserId
        {
            get
            {
                var jtwToken = ReadJTWToken();
                return jtwToken.payload.userId;
            }
        }

        protected int LoggedRoleId
        {
            get
            {
                var jtwToken = ReadJTWToken();
                return jtwToken.payload.roleId;
            }
        }


        #endregion

        #region Constructor

        public ApiBaseController()
        {
            _cache = new Cache();
        }

        #endregion

        #region Internal

        protected dynamic ReadJTWToken()
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtInput = Request.Headers.ToString();
            jwtInput = jwtInput.StartsWith("Bearer ") ? jwtInput.Substring(7) : jwtInput;

            var token = jwtHandler.ReadJwtToken(jwtInput);

            //Extract the headers of the JWT
            var headers = token.Header;
            var jwtHeader = "{";
            foreach (var h in headers)
                jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";

            jwtHeader += "}";
            var jsonToken = string.Format("{{header:{0},", JToken.Parse(jwtHeader).ToString(Formatting.Indented));

            //Extract the payload of the JWT
            var claims = token.Claims;
            var jwtPayload = "{";
            foreach (Claim c in claims)
                jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";

            jwtPayload += "}";
            jsonToken += string.Format("payload:{0} }}", JToken.Parse(jwtPayload).ToString(Formatting.Indented));

            return JObject.Parse(jsonToken);

        }


        protected ActionResult NoContent()
        {
            return NoContent();
        }

        protected void BusinessException(string message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent(message),
                ReasonPhrase = "Regla de negocio"
            };
            throw new Exception(resp.RequestMessage.ToString());
        }

        #endregion
    }
}