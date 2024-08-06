namespace BE_App_Scores.Models
{
    public class Meci
    {
        public int Id { get; set; }
        public string DenumireMeci { get; set; }

        public ICollection<GestionareMeci> GestionareMeciuri { get; set; }

    }
}
