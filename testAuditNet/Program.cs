using System;
using System.Threading.Tasks;
using Audit.Core;

namespace testAuditNet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");

            var order = new Order();

            using (var audit = AuditScope.Create("Order:Update", () => order, new { UserName = "cash" }))
            {
                Console.WriteLine("Audit");
                order.Status = EnumStatus.Finish;
                
                audit.Comment("commit");
            }

            AuditScope auditScope = null;
            try
            {
                auditScope = await AuditScope.CreateAsync("Test:Identity", () => order, new { user = "cash" });
                {
                    order.Status = EnumStatus.Start;
                }
                auditScope.Comment("01");
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            Console.ReadKey();
        }
    }

    public class Order
    {
        public Order()
        {
            Id = 1;
            Name = "name";
            Status = EnumStatus.Init;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public EnumStatus Status { get; set; }
    }

    public enum EnumStatus
    {
        Init = 0,
        Start = 1,
        Finish = 2
    }
}