using RnD.Business;
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

        public AgentsRepository AgentsRepository
        {
            get { return _agentsRepository; }
            set { _agentsRepository = value; }
        }
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
            _agentsRepository = new AgentsRepository();
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

        #endregion //Singleton Implementation

        public static string BaseAddress = "http://localhost:8080";

        public void Start()
        {
            Logger.Logg("Starting server on address: {0}", BaseAddress);
            var config = new HttpSelfHostConfiguration(BaseAddress);

            //config.Routes.MapHttpRoute(
            //    "API Default", "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional });
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);

            _server.OpenAsync().Wait();

            Logger.Logg("Server started on address: {0}", BaseAddress);

        }

        public void Stop()
        {
            Logger.Logg("Stopping server.");
            _server.CloseAsync().Wait();
            Logger.Logg("Server was Stopped.");
        }
    }
}
