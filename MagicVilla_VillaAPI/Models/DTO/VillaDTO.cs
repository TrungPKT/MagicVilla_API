using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaDTO
    {
        // 9, Same as Villa model - without CreatedDate (type DateTime) property which defines when this record was created. However, we do not want to exposed this info. to the end users -- FOR SECURITY (properties like password, SSN, etc.), TO FILTER OTHER uneccessary properties to users
        public int Id { get; set; }
        [Required]          // 18, Add using System.ComponentModel.DataAnnotations;
        [MaxLength(30)]     // 18, Usually, these data annotations are use by EF to create constraint for tables of Database. However, APIController does know about these requirement [APIController] attribute -> back to VillaAPIController
        public string Name { get; set; }
    }
}
