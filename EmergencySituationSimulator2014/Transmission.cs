using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NodeMaster;
using ProtoBuf;
using WebSocketSharp;

namespace EmergencySituationSimulator2013
{
    class Transmission
    {
        private WebSocket Client;
        private MemoryStream Output;
        private string SenderID;


        public Transmission(string path, string senderID)
        {
            this.SenderID = senderID;
            Output = new MemoryStream();
            Client = new WebSocket(path);

            Client.OnMessage += delegate(object sender, MessageEventArgs args)
                {
                    // TODO
                    Console.WriteLine("new message");
                };

            Client.Connect();
        }

        public void Send(Transaction transaction)
        {
            transaction.SenderID = SenderID;
            Output.SetLength(0);
            Serializer.Serialize(Output, transaction);
            Output.Position = 0;

            Client.Send(Output, (int)Output.Length);
        }

        /*public void init(List<OLdEntity> patients)
        {
            var transaction = new NodeMaster.Transaction();
            transaction.SenderID = this.SenderID;
            transaction.PublishList = new Transaction.Content();
            
            foreach (var patient in patients)
            {
                var message = new NodeMaster.PatientModel();
                message.ID = patient.sID;
                message.Age = new AgeModel()
                    {
                        Unit = "years",
                        Value = 42
                    };
                message.IncidentId = "simulation";
                message.Sex = "F";
                /*message.TriageStatus = 
                message.Location = new LatLng()
                    {
                        lat = patient.location.lat,
                        lng = patient.location.lng
                    };* /

                transaction.PublishList.PatientList.Add(message);
            }

            Output.SetLength(0);
            Serializer.Serialize(Output, transaction);
            Output.Position = 0;

            Client.Send(Output, (int) Output.Length);
        }

        public void update(List<OLdEntity> patients)
        {
            var transaction = new NodeMaster.Transaction();
            transaction.SenderID = this.SenderID;
            transaction.PublishList = new Transaction.Content();

            foreach (var patient in patients)
            {
                var message = new NodeMaster.PatientModel();
                message.ID = patient.sID;
                
                message.Location = new LatLng()
                {
                    lat = patient.location.lat,
                    lng = patient.location.lng
                };

                transaction.PublishList.PatientList.Add(message);
            }

            Output.SetLength(0);
            Serializer.Serialize(Output, transaction);
            Output.Position = 0;

            Client.Send(Output, (int)Output.Length);
        }*/
    }
}
