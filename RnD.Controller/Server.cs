using RnD.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace RnD.Controller
{
    public class Server
    {
        #region AgentMessageSendEvent

        public event AgentMessageSendHandler AgentMessageSend;
        public delegate void AgentMessageSendHandler(AgentInfo info);
        private void OnAgentMessageSend(AgentInfo info)
        {
            if (AgentMessageSend != null)
                AgentMessageSend(info);
        }

        #endregion //AgentMessageSendEvent

        #region Private Members

        private HttpSelfHostServer _server;
        private AgentsRepository _agentsRepository;
        private Dispatcher _dispatcher;

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set { _dispatcher = value; }
        }
        #endregion //Private Members

        #region Singleton Implementatio

        private static Server instance;

        private Server()
        {
            _dispatcher = new Dispatcher();
        }

        public static Server Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Server();
                }
                return instance;
            }
        }

        #endregion //Singleton Implementatio

        public static string BaseAddress = "http://localhost:8080";

        public void Start()
        {
            var config = new HttpSelfHostConfiguration(BaseAddress);

            //config.Routes.MapHttpRoute(
            //    "API Default", "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional });
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);

            _server.OpenAsync().Wait();


        }

        public void Stop()
        {
            _server.CloseAsync();
        }
    }
}
