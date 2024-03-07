using tangti.Models;

namespace tangti.Services;

public class UtilsService
{
	public static bool IsCollapse(tangti.Models.Event.StartEndDate event_date, tangti.Models.Event.StartEndDate enroll_date)
	{
		if (event_date.StartDate > enroll_date.EndDate || event_date.EndDate < enroll_date.StartDate)
		{
			return false;
		}
		return true;
	}

	public static string ValidateErrorTime(tangti.Models.Event.StartEndDate event_date, tangti.Models.Event.StartEndDate enroll_date)
	{
		// enroll date and event date is collapse
		if (IsCollapse(event_date, enroll_date))
			return ("Event and Enroll date duration is overlapping");

		// event date is before enroll date 
		if (enroll_date.EndDate > event_date.StartDate)
			return ("Event date cannot before Enroll date");

		// start of event is after end of event
		if (event_date.StartDate > event_date.EndDate)
			return ("Event: Start Date must before EndDate");
		// start of enroll is after end of enroll
		if (enroll_date.StartDate > enroll_date.EndDate)
			return ("Enroll Start Date must before EndDate");
		return ("");

	}

	public static string ValidateRoleAdmin(string role)
	{
		if (role != "admin")
			return ("You are not authorized to perform this action");
		return ("");
	}
}
