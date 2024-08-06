namespace BE_App_Scores.Models
{
    public class GestionareMeci
    {
      //  public int Id { get; set; }
        public int IdActivitate { get; set; }
        public Activitate Activitate { get; set; }

        public int IdEchipa { get; set; }
        public Echipe Echipa { get; set; }


        public int IdMeci { get; set; }
        public Meci Meci { get; set; }


        public int IdScor { get; set; }
        public Scoruri Scoruri { get; set; }
    }
}
