namespace BE_App_Scores.Models
{
    public class Scoruri
    {
        public int Id { get; set; }
        public int Scor { get; set; }
        public DateTime Data { get; set; }
        public ICollection<GestionareMeci> GestionareMeciuri { get; set; }

    }
}
