namespace GymTracker.Models;

public class TrainingPlan
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public List<ExercisePlan> ExercisePlans { get; set; }
    
    
    public TrainingPlan()
    {
        ExercisePlans = new List<ExercisePlan>();
    }
}