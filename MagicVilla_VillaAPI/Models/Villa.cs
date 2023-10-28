using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // 29,
        public string Details { get; set; }
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        // 29, Image maybe stored in a different DB (Azure storage)
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        // 9, A column in the DB which we do not want to expose it to the end user
        public DateTime CreatedDate { get; set; }
        // 29,
        public DateTime UpdatedDate { get; set; }
    }
}