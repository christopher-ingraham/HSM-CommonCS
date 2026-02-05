namespace DataAccess.LaminarCooling.Models
{
    public class AccStatus
    {
        public string AreaId { get; set; }
        public string CenterId { get; set; }
        public int BankNo { get; set; }
        public int BankPos { get; set; }
        public int DeviceType { get; set; }
        public int BankOutOfOrder{ get; set; }
        public int DeviceOutOfOrder{ get; set; }
        public int RtdbAccStatusNo{ get; set; }

        public int ZoneNo { get; set; }
        public string ZoneId { get; set; }
        public int BankSeq { get; set; }

        // Add only what Cooling actually uses at first
        public DateTime UpdateTime { get; set; }
    }
}
