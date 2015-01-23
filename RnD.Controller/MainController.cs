using RnD.Business;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace RnD.Controller
{
    //[SD] ********** We should create a service and interface class that does all the logic below so it can be tested easily and reduce the code in the controller 
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
            try
            {
                var registerResult = Server.Instance.AgentsRepository.RegisterAgent(agentInfo);

                if (registerResult)
                {
                    Logger.Logg("Successfully registered agent: {0}", agentInfo.Name);
                    return HttpStatusCode.OK;
                }
                else
                {
                    Logger.Logg("Error while trying registered agent: {0}", agentInfo.Name);
                    return HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                Logger.Logg("Exception while trying registered agent: {0}{1}{2}", agentInfo.Name, Environment.NewLine, ex.Message);
                return HttpStatusCode.InternalServerError;
            }

        }

        /// <summary>
        /// Called every 10 seconds by agent to report status information. [e.g. Enabled, ExecutingTests, etc.]
        /// </summary>
        [HttpPost]
        public HttpStatusCode RecieveAgentStatus(string agentId, AgentStatus status)
        {
            try
            {
                var registerResult = Server.Instance.AgentsRepository.UpdateAgentStatus(agentId, status);

                if (registerResult)
                {
                    Logger.Logg("Successfully updated agent '{0}' status to '{1}'", agentId, status);
                    return HttpStatusCode.OK;
                }
                else
                {
                    Logger.Logg("Error updated agent '{0}' status to '{1}'", agentId, status);
                    return HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                Logger.Logg("Error updated agent '{0}' status to '{1}'{2}{3}", agentId, status, Environment.NewLine, ex.Message);

                return HttpStatusCode.InternalServerError;
            }
        }


        [HttpGet]
        public AgentTask GetTask(string agentId)
        {
            Logger.Logg("Successfully send Task: {0} to agent '{0}'.", AgentTask.DownloadMaterials, agentId);

            //  if (Server.Instance.AgentsRepository[agentId].Status == AgentStatus.DownloadedMaterials)
            return AgentTask.DownloadMaterials;
        }

        [HttpPost]
        public HttpStatusCode UploadReport(string agentId)
        {
            return HttpStatusCode.OK;
        }



        [HttpGet]
        public HttpResponseMessage DownloadMaterial()
        {
            HttpResponseMessage response = Request.CreateResponse();

            var filePath = "C:\\01.txt";

            FileInfo fileInfo = new FileInfo(filePath);
            response.Headers.AcceptRanges.Add("bytes");
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StreamContent(fileInfo.OpenRead());
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = "01.txt";
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = fileInfo.Length;

            return response;
        }
    }
}
