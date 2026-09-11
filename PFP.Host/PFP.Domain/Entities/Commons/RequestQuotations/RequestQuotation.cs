using PFP.Host.PFP.Domain.Entities.Commons.RequestQuotationsDetails;
using PFP.Host.PFP.Domain.Enums;

namespace PFP.Host.PFP.Domain.Entities.Commons.RequestQuotations
{
    public class RequestQuotation
    {

        /// <summary>
        /// get or set the RequestQuotation Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// get or set the RequestQuotation DocNo
        /// </summary>
        public string DocNo { get; set; } = string.Empty;
        /// <summary>
        /// get or set the RequestQuotation PurchaseRequestId
        /// </summary>
        public int PurchaseRequestId { get; set; } = 0;
        /// <summary>
        /// get or set the RequestQuotation SupplierId
        /// </summary>
        public int SupplierId { get; set; } = 0;

        public decimal TotalAmount { get; set; } = 0;

        public RQStatus Status { get; set; }

        public bool RequiresL2 { get; set; }

        public DateTime CreatedAt { get; set; }

        public required byte[] RowVersion { get; set; }

        public ICollection<RequestQuotationDetails> RequestQuotationDetails { get; set; } = new List<RequestQuotationDetails>();
    }
}
