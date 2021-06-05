using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class CustomerGroup
    {
        public CustomerGroup()
        {
            Customer = new HashSet<Customer>();
        }

        public Guid CustomerGroupId { get; set; }
        public string CustomerGroupName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }

    }
}
