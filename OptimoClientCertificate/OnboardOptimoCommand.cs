namespace OptimoClientCertificate
{
    public class OnboardOptimoCommand
    {        // Optimo        
        public OptimoType OptimoType { get; private set; }
        public string SerialNumber { get; private set; }
        public string Language { get; private set; }
        public string Country { get; private set; }
        public string SoftwareVersion { get; private set; }
        public string Email { get; private set; }
        public string RollingRoadType { get; private set; }
        public string IBFirmware { get; private set; }
        public string WirelessFirmware { get; private set; }
        public string CalibrationDueDate { get; private set; }
        public int FixedDistanceLength { get; private set; }
        public int FixedDistanceTwoRuns { get; private set; }
        public int WirelessPanId { get; private set; }
        public int WirelessChannelId { get; private set; }

        public string CertificateSigningRequest { get; set; }
        
        //Workshop
        public string SealNumber { get; private set; }
        public string CompanyName { get; private set; }
        public string WorkshopAddress { get; private set; }

        //Distributor
        public string DistributorName { get; private set; }

        public OnboardOptimoCommand(OptimoType optimoType, string serialNumber, string language, string country, string softwareVersion, string email, string rollingRoadType, string iBFirmware, string wirelessFirmware, string calibrationDueDate, int fixedDistanceLength, int fixedDistanceTwoRuns, int wirelessPanId, int wirelessChannelId, string sealNumber, string companyName, string workshopAddress, string distributorName, string certificateSigningRequest)
        {
            OptimoType = optimoType;
            SerialNumber = serialNumber;
            Language = language;
            Country = country;
            SoftwareVersion = softwareVersion;
            Email = email;
            RollingRoadType = rollingRoadType;
            IBFirmware = iBFirmware;
            WirelessFirmware = wirelessFirmware;
            CalibrationDueDate = calibrationDueDate;
            FixedDistanceLength = fixedDistanceLength;
            FixedDistanceTwoRuns = fixedDistanceTwoRuns;
            WirelessPanId = wirelessPanId;
            WirelessChannelId = wirelessChannelId;
            SealNumber = sealNumber;
            CompanyName = companyName;
            WorkshopAddress = workshopAddress;
            DistributorName = distributorName;
            CertificateSigningRequest = certificateSigningRequest;
        }

    }
}
