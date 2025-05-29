namespace API.Entities
{
    public class NutritionGuide
    {
        public int Id { get; set; }
        public int AgeRangeStart { get; set; }
        public int AgeRangeEnd { get; set; }
        public required string Gender { get; set; }
        public required string Goal { get; set; }
        public int Calories { get; set; }
        public int ProteinGrams { get; set; }
        public int CarbsGrams { get; set; }
        public int FatGrams { get; set; }
        public required string Sunday { get; set; }
        public required string Monday { get; set; }
        public required string Tuesday { get; set; }
        public required string Wednesday { get; set; }
        public required string Thursday { get; set; }
        public required string Friday { get; set; }
        public required string Saturday { get; set; }
    }
}
