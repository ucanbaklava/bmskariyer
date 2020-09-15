namespace Sinav.Business.DTOs
{
    public class LeaderboardResult
    {
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public int Wrong { get; set; }
        public int Correct { get; set; }
        public int Empty { get; set; }
        public string UserName { get; set; }
    }
}