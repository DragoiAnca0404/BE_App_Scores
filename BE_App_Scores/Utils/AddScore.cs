namespace BE_App_Scores.Utils
{
    public class TeamScore
    {
        public string DenumireEchipa { get; set; }
        public int Scor { get; set; }
    }
    public class AddScore
    {
        public string DenumireActivitate { get; set; }
        public string DenumireMeci { get; set; }
        public string TipMeci { get; set; }

        public DateTime DataMeci { get; set; }
        public List<TeamScore> Echipe { get; set; }
    }

}
