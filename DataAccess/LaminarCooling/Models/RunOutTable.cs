using Common.Enums;

namespace DataAccess.LaminarCooling.Models
{
    public class RunOutTable
    {
        public List<Intensive> IntensiveHeaders { get; set; }
        public List<Normal> NormalHeaders { get; set; }
        public List<Trim> TrimHeaders { get; set; }
    }

    public abstract class LaminarHeader
    {
        public double FlowRate { get; set; }
        public Common.Enums.BankPosition Position { get; set; }
    }

    public class Intensive : LaminarHeader
    {
        public bool isBoosted;

    }

    public class Trim : LaminarHeader
    {

    }

    public class Normal : LaminarHeader
    {

    }
}
