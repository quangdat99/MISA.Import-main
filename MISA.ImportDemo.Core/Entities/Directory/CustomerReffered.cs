using MISA.ImportDemo.Core.Enumeration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities
{
    public class CustomerReffered:BaseEntity
    {
        public Guid CustomerRefferedId { get; set; }
        public string CustomerCode { get; set; }
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
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
        public DateDisplaySetting DobdisplaySetting { get; set; } =  DateDisplaySetting.ddmmyyyy;
        public int? NationalityId { get; set; }
        public int? EthnicId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public int? Sort { get; set; }
    }
}
