namespace BE_App_Scores.Models
{
    public class Meci
    {
        public int Id { get; set; }
        public string DenumireMeci { get; set; }

        //   public ICollection<GestionareMeci> GestionareMeciuri { get; set; }
        public DateTime Data { get; set; }

        public List<GestionareMeci> GestionareMeciuri { get; set; } = new List<GestionareMeci>();

        public string TipMeci { get; set; }  // Adaugă câmpul nou

    }
}
