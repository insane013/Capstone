namespace TodoListApp.WebApp.Helpers;

public class DeadlineHelper
{
    public static string GetDeadlineStatus(DateTime deadline)
    {
        var now = DateTime.UtcNow;
        if (deadline < now)
        {
            return "Overdue";
        }

        var timeLeft = deadline - now;

        if (timeLeft.TotalDays >= 30)
        {
            return $"{Math.Round(timeLeft.TotalDays / 30)} months left.";
        }
        else if (timeLeft.TotalDays >= 7)
        {
            return $"{Math.Round(timeLeft.TotalDays / 7)} weeks left";
        }
        else if (timeLeft.TotalDays >= 1)
        {
            return $"{Math.Round(timeLeft.TotalDays)} days left.";
        }
        else
        {
            return $"{Math.Round(timeLeft.TotalHours)} hours left.";
        }
    }
}
