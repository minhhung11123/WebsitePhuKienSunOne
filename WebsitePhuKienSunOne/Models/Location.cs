using System;
using System.Collections.Generic;

#nullable disable

namespace WebsitePhuKienSunOne.Models
{
    public partial class Location
    {
        public Location()
        {
            CustomerDistrictNavigations = new HashSet<Customer>();
            CustomerLocations = new HashSet<Customer>();
            CustomerWardNavigations = new HashSet<Customer>();
            OrderCityNavigations = new HashSet<Order>();
            OrderDistrictNavigations = new HashSet<Order>();
            OrderWardNavigations = new HashSet<Order>();
        }

        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }
        public int? ParentCode { get; set; }
        public int? Levels { get; set; }
        public int? Code { get; set; }

        public virtual ICollection<Customer> CustomerDistrictNavigations { get; set; }
        public virtual ICollection<Customer> CustomerLocations { get; set; }
        public virtual ICollection<Customer> CustomerWardNavigations { get; set; }
        public virtual ICollection<Order> OrderCityNavigations { get; set; }
        public virtual ICollection<Order> OrderDistrictNavigations { get; set; }
        public virtual ICollection<Order> OrderWardNavigations { get; set; }
    }
}
