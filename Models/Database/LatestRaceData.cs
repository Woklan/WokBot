using System.ComponentModel.DataAnnotations;

namespace WokBotModels.Database
{
    public class LatestRaceData
    {
        [Key]
        public int Id { get; set; }

        public int RaceId { get; set; }
    }
}
