namespace RaceDayAPI.DTOs
{
    public class UpdateRouteDto
    {
        public string? RouteName { get; set; }
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public decimal Distance { get; set; }
        public decimal? ElevationGain { get; set; }
        public string? RouteMapURL { get; set; }
        public string? GPXFileURL { get; set; }
    }
}
