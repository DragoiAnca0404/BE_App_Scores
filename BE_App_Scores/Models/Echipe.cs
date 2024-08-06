namespace BE_App_Scores.Models
{
    public class Echipe
    {
        public int Id { get; set; }
        public string DenumireEchipa { get; set; }
        public ICollection<CreareEchipe> CreareEchipe { get; set; }
        public ICollection<GestionareMeci> GestionareMeciuri { get; set; }
    }
}
