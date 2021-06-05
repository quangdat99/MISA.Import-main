using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Customer:BaseEntity
    {
        public Customer()
        {
            CustomerReffered = new HashSet<CustomerReffered>();
        }

        public Guid CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public int? Gender { get; set; }
        public string GenderName
        {
            get
            {
                var name = string.Empty;
                switch ((Enumeration.Gender)Gender)
                {
                    case Enumeration.Gender.Female:
                        name = Properties.Resources.Enum_Gender_Female;
                        break;
                    case Enumeration.Gender.Male:
                        name = Properties.Resources.Enum_Gender_Male;
                        break;
                    case Enumeration.Gender.Other:
                        name = Properties.Resources.Enum_Gender_Other;
                        break;
                    default:
                        break;
                }
                return name;
            }
        }

        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Guid? CustomerGroupId { get; set; }

        [NotMapped]
        public string CustomerGroupName { get; set; }
        [NotMapped]
        public string NationalityName { get; set; }
        [NotMapped]
        public string EthnicName { get; set; }


        [NotMapped]
        public int? Sort { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual CustomerGroup CustomerGroup { get; set; }

        public virtual Ethnic Ethnic { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual ICollection<CustomerReffered> CustomerReffered { get; set; }

    }
}
