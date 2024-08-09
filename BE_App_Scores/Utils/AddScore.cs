namespace BE_App_Scores.Utils
{
    /*  public class AddScore
      {
          public string denumire_activitate { get; set; }

          public string denumire_meci { get; set; }
          public DateTime Data_Meci { get; set; }
          public string DenumireEchipa { get; set; }

          public int scor { get; set; }
          public DateTime Data_Scor { get; set; }

      }*/

    public class TeamScore
    {
        public string DenumireEchipa { get; set; }
        public int Scor { get; set; }
    }
    public class AddScore
    {
        public string DenumireActivitate { get; set; }
        public string DenumireMeci { get; set; }
        public DateTime DataMeci { get; set; }
        public List<TeamScore> Echipe { get; set; }
    }

}
/* 
 
 public class AddScore
{
    public string DenumireActivitate { get; set; }
    public string DenumireMeci { get; set; }
    public DateTime DataMeci { get; set; }
    public string DenumireEchipa1 { get; set; }
    public string DenumireEchipa2 { get; set; }
    public int ScorEchipa1 { get; set; }
    public int ScorEchipa2 { get; set; }
}
 
 
 */