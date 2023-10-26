namespace MagicVilla_VillaAPI.Models
{
    public class Villa
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // 9, A column in the DB which we do not want to expose it to the end user
        public DateTime CreatedDate { get; set; }
    }
}