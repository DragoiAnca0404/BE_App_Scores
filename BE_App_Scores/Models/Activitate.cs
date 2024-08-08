namespace BE_App_Scores.Models
{
    public class Activitate
    {
        public int Id { get; set; }
        public string Titlu { get; set; }

        public string ImagineUrl { get; set; }  // Adăugat pentru URL-ul imaginii

        public ICollection<GestionareMeci> GestionareMeciuri { get; set; }
    }
}
