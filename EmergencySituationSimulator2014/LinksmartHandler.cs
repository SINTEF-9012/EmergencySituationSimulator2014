using DataHandler.Models;
using DataHandler.Repositories;
using DataHandler.Repositories.Contracts;
using EmergencySituationSimulator2014.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmergencySituationSimulator2014
{
    public class LinksmartHandler
    {
        private IIncidentObjectRepository _incidentObjectRepository;
        private IETriageRepository _eTriageRepository;
        private IResourceManagerRepository _resourceManagerRepository;
        private IMediaRepository _mediaRepository;
        private IHelpBeaconRepository _helpBeaconRepository;
        private IHospitalRepository _hospitalRepository;
        private IAdviseRepository _adviseRepository;

        public bool IsConnectedToLinksmart { get; set; }

        public LinksmartHandler()
        {
            SetupLinksmartConnections();
        }

        private void SetupLinksmartConnections()
        {
            ConfigureNinjectConfiguration();
            //SetupLinksmartWebServersAndBindEventHandlers();
            IsConnectedToLinksmart = ConnectToLinksmart();            
        }

        private bool ConnectToLinksmart()
        {
            return (IncidentObjectRepository.ConnectToLinkSmart("MASTER.IncidentObject" + DataHandler.Toolbox.DataHandlerLibrary.GetComputerID(), "IncidentObject")
                && ResourceManagerRepository.ConnectToLinkSmart("MASTER.ResourceStatus" + DataHandler.Toolbox.DataHandlerLibrary.GetComputerID(), "ResourceStatus")                
                && MessengerRepository.ConnectToLinkSmart("MASTER.Messenger" + DataHandler.Toolbox.DataHandlerLibrary.GetComputerID(), "Messenger"));
        }

        private static void ConfigureNinjectConfiguration()
        {
            DataHandler.Bootstrapper.NinjectConfiguration.Configure();
        }        

        private bool SetupLinksmartWebServersAndBindEventHandlers()
        {
            return (_incidentObjectRepository.SetupWebServerAndBindEventHandler("MASTER.IncidentObject", "IncidentObject") &&
                _resourceManagerRepository.SetupWebServerAndBindEventHandler("MASTER.ResourceStatus", "ResourceStatus") &&
                _eTriageRepository.SetupWebServerAndBindEventHandler("MASTER.eTriage", "eTriage") &&
                _helpBeaconRepository.SetupWebServerAndBindEventHandler("MASTER.HelpBeacon", "HelpBeacon") &&
                _hospitalRepository.SetupWebServerAndBindEventHandler("MASTER.HospitalAvailability", "HospitalAvailability") &&
                _mediaRepository.SetupWebServerAndBindEventHandler("MASTER.Media", "Media"));
        }

        public void SendIncidentObjectToS2D2S(Incident incident)
        {
            var newObjectModel = new IncidentObjectModel
            {
                GraphicID = incident.Id,
                GraphicValue = incident.Name,
                LinearRingPointList = new List<double>(),
                Latitude = incident.Location.lat,
                Longitude = incident.Location.lng
            };

            var senderID = DataHandler.Toolbox.DataHandlerLibrary.GetComputerID();
            IncidentObjectRepository.PublishToS2D2S(senderID, "App.Global.IncidentObject", newObjectModel);
        }

        public void SendChatMessageToS2D2S(ChatMessage chatMessage)
        {
            var newObjectModel = new MessengerModel
            {
                ActionPlan = chatMessage.Message,
                DateTimeSent = DateTime.UtcNow,
                MessageID = chatMessage.Id + DateTime.UtcNow,
                SenderID = DataHandler.Toolbox.DataHandlerLibrary.GetComputerID()
            };

            MessengerRepository.PublishToS2D2S(newObjectModel.SenderID, "App.Global.Messenger", newObjectModel);

        }

        public void SendResourceAllocationToS2D2S(ResourceAllocation resourceAllocation)
        {
            var newObjectModel = new ResourceMobilizationModel
            {
                ResourceMobilizationID = resourceAllocation.ResourceMobilizationID,
                Task = resourceAllocation.Task
            };

            var published = ResourceManagerRepository.PublishToS2D2S(DataHandler.Toolbox.DataHandlerLibrary.GetComputerID(), "App.Global.ResourceAllocation", newObjectModel);
        }

        
    }
}
