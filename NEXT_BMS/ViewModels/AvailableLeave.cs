namespace NEXTTIMS;
public class AvailableLeave
{
    public int EmployeeId { set; get; }
    public int LeaveTypeId { set; get; }
    public int FiscalYearId { set; get; }
    public int FiscalYear { set; get; }
    public int AvailableLeaveDays { set; get; }
    public int? ExpireAfter { set; get; }

    public int RequestFiscalYear { set; get; }
}
