using RnD.Business;
using System.Net;
using System.Web.Http;

namespace RnD.Controller
{
    public class MainController : ApiController
    {

        /// <summary>
        /// [SD] For DEV testing only. This should be deleted from production code.
        /// </summary>
        [HttpGet]
        public string Ping()
        {
            return "Server is online";
        }


        /// <summary>
        /// Thi method is called from an agent the first time it is started and then it is added in the agents repository.
        /// </summary>
        /// <param name="agentInfo">Information for the agent.</param>
        [HttpPost]
        public HttpStatusCode Register(AgentInfo agentInfo)
        {
            return HttpStatusCode.OK;
        }

        /// <summary>
        /// Called every 10 seconds by agent to report status information. [e.g. Enabled, ExecutingTests, etc.]
        /// </summary>
        [HttpPost]
        public HttpStatusCode RecieveAgentStatus(string agentId, AgentStatus status)
        {
            return HttpStatusCode.OK;
        }


        [HttpGet]
        public AgentTask GetTask(string agentId)
        {
            return AgentTask.DownloadMaterials;
        }

        [HttpPost]
        public HttpStatusCode UploadReport(string agentId)
        {
            return HttpStatusCode.OK;
        }

        [HttpGet]
        public HttpStatusCode DownloadMaterial()
        {
            return HttpStatusCode.OK;
        }

    }
}
