using System;
using EmergencySituationSimulator2013.Model;
using NodeMaster;

namespace EmergencySituationSimulator2013.Visitors
{
    public class MasterVisitor : IVisitor
    {
        public Transaction Transaction { get; protected set; }
        public string ReportPrividerId;
        protected string ReportId;
        protected ulong TimeStamp;
        protected bool FirstTransaction = true;


        public void StartTransaction()
        {
            
            Transaction = new Transaction
                {
                    PublishList = new Transaction.Content()
                };

            if (FirstTransaction)
            {
                ReportId = new Guid().ToString("D");
                TimeStamp = (ulong)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                FirstTransaction = false;
            }
            
        }

        public void Visit(Entity e)
        {
            // do nothing, I dislike entities
        }

        public void Visit(WheeledVehicle v)
        {
            var vehicle = new ResourceStatusModel
                {
                    ID = v.Id,
                    type = "WheeledVehicle",
                    Name = v.Name,
                    Status = "unknown",
                    Location = v.Location.ToLatLng()
                };
            Transaction.PublishList.ResourceStatusList.Add(vehicle);
        }

        public void Visit(Patient v)
        {
            ulong timeStamp = TimeStamp;
            
            var patient = new PatientModel
                {
                    ID = v.Id,
                    TriageInfo = new TriageInfoModel
                        {
                            ReportProviderId = ReportPrividerId,
                            ReportId = ReportId,
                            ReportDateTime = timeStamp
                        },
                    Location = v.Location.ToLatLng(),
                    Age = new AgeModel
                        {
                            Unit = "y",
                            Value = (uint) v.Age
                        }

                };

            switch (v.Sex)
            {
                case Person.SexEnum.Woman:
                    patient.Sex = "Woman";
                    break;
                case Person.SexEnum.Man:
                    patient.Sex = "Man";
                    break;
                default:
                    patient.Sex = "Other";
                    break;
            }

            // Warning : sophisticated algorithm
            switch (v.Triage)
            {
                case Patient.TriageEnum.Black:
                    patient.TriageStatus = PatientModel.TriageStatusEnum.BLACK;
                    break;
                case Patient.TriageEnum.Red:
                    patient.TriageStatus = PatientModel.TriageStatusEnum.RED;
                    break;
                case Patient.TriageEnum.Yellow:
                    patient.TriageStatus = PatientModel.TriageStatusEnum.YELLOW;
                    break;
                case Patient.TriageEnum.Green:
                    patient.TriageStatus = PatientModel.TriageStatusEnum.GREEN;
                    break;
                case Patient.TriageEnum.White:
                    patient.TriageStatus = PatientModel.TriageStatusEnum.WHITE;
                    break;
            }
            

            Transaction.PublishList.PatientList.Add(patient);
        }

        public void Visit(Zombie z)
        {
            throw new System.NotImplementedException();
        }
    }
}