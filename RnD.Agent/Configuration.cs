
namespace RnD.Agent
{
    public class Configuration
    {
        public static string ControllerName { get; set; }
        public static int ControllerPortNumber { get; set; }
        public static string ControllerMainControllerName { get; set; }
        public static string ControllerConnectMethodName { get; set; }
        public static int PollingInterval { get; set; }

        static Configuration()
        {
            ControllerName = "localhost";
            ControllerPortNumber = 8080;
            ControllerMainControllerName = "Main";
            ControllerConnectMethodName = "Connect";
            PollingInterval = 3000;
        }

        public static string GetControllerAddress()
        {
            return string.Format("http://{0}:{1}", ControllerName, ControllerPortNumber);
        }
    }
}
