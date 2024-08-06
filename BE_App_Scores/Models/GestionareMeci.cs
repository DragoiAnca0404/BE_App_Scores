namespace BE_App_Scores.Models
{
    public class GestionareMeci
    {
      //  public int Id { get; set; }
        public int IdActivitate { get; set; }
        public Activitate Activitate { get; set; }

        public int IdEchipa { get; set; }
        public Echipe Echipa { get; set; }

        public int Scor { get; set; }
        public DateTime Data { get; set; }
    }
}
