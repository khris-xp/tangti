

namespace tangti.Services;

public class UtilsService
{
	public static bool IsCollapse(DateTime d1_s, DateTime d1_e, DateTime d2_s, DateTime d2_e)
	{
		if (d1_s > d2_e || d1_e < d2_s)
		{
			return false;
		}
		return true;
	}
}