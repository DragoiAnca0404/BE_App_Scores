namespace BE_App_Scores.Models
{
    public enum ActivityType
    {
        Sport,
        Muzica,
        Dans,
        Teatru,
        Curs,
        Joc
    }

    public class Game
    {
        public int Id { get; set; }
        public string DenumireJoc { get; set; }
        public ActivityType Activitate { get; set; }
    }
}
