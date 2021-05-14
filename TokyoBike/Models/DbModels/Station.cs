namespace TokyoBike.Models.DbModels
{
    public class Station : IPointable
    {
        public int Id { get ; set ; }
        public Point Point { get; set; }
        public string Type { get; set; }
        public string District { get; set; }
    }
}