namespace API.Entities;

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
        public required string MealPlan { get; set; }
    }
