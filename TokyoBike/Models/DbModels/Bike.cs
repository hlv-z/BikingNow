namespace TokyoBike.Models.DbModels
{
    public class Bike
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public Station Station { get; set; }
    }
}