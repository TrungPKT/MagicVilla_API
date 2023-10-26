namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaDTO
    {
        // 9, Same as Villa model - without CreatedDate (type DateTime) property which defines when this record was created. However, we do not want to exposed this info. to the end users 
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
