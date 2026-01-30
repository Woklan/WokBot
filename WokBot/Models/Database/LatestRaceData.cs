using System.ComponentModel.DataAnnotations;

namespace WokBot.Models.Database
{
    public class LatestRaceData
    {
        [Key]
        public int Id { get; set; }

        public int RaceId { get; set; }
    }
}
