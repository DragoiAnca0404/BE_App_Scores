namespace BE_App_Scores.Models
{
    public class Activitate
    {
        public int Id { get; set; }
        public string Titlu { get; set; }
        public string Descriere { get; set; }
        public ICollection<GestionareMeci> GestionareMeciuri { get; set; }
    }
}
