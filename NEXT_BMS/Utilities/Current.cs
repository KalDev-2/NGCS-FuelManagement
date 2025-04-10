using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using NEXT_BMS.Models;

namespace NEXT_BMS.Utilities;

public class CurrentUser
{
    private readonly NEXT_BMSContext _context = new NEXT_BMSContext();
    private readonly IHttpContextAccessor _httpContextAccessor;
  
    public CurrentUser(NEXT_BMSContext context, IHttpContextAccessor httpContextAccessor)
    {
       
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    //public static int DepartmentId()
    //{
    //    return HttpContext.Current.Session["UserDepartmentId"] != null ? int.Parse(HttpContext.Current.Session["UserDepartmentId"].ToString()) : 0;
    //}

    //public static int BranchDepartmentId()
    //{
    //    return HttpContext.Current.Session["UserBranchDepartmentId"] != null ? int.Parse(HttpContext.Current.Session["UserBranchDepartmentId"].ToString()) : 0;
    //}

    //public static int BranchId()
    //{
    //    return HttpContext.Current.Session["UserBranchId"] != null ? int.Parse(HttpContext.Current.Session["UserBranchId"].ToString()) : 0;
    //}

    //public static string BranchDepartmentName()
    //{
    //    return HttpContext.Current.Session["UserBranchDepartmentName"] != null ? HttpContext.Current.Session["UserBranchDepartmentName"].ToString() : "";
    //}

    public  int GetUserId()
    {
        return _httpContextAccessor.HttpContext.Session.GetInt32("UserId") != null ? (int)_httpContextAccessor.HttpContext.Session.GetInt32("UserId") : 0;
    }

    public   int GetActionBy()
    {
        return _httpContextAccessor.HttpContext.Session.GetInt32("ActionBy") != null ? (int)_httpContextAccessor.HttpContext.Session.GetInt32("ActionBy") : 0;
    }



    //public static int FiscalYearId()
    //{
    //    return HttpContext.Current.Session["FiscalYearId"] != null ? int.Parse(HttpContext.Current.Session["FiscalYearId"].ToString()) : 0;
    //}

    //public static List<int> DepartmentList() => new DataLevelAuthorization().GetUserDepartments();


    //public List<int> UserBranchs()
    //{
    //    List<int> result = new List<int>();
    //    int userId= HttpContext.Current.Session["ActionBy"] != null ? int.Parse(HttpContext.Current.Session["ActionBy"].ToString()) : 0;
    //    var userBranches= _context.UserBranches.Where(x=>x.UserId==userId && x.IsActive).Select(x=>x.Id);
    //    result.AddRange(userBranches);
    //    if (!result.Contains(BranchId()))
    //    {
    //        result.Add(BranchId());
    //    }
    //    return result;
    //}

    //public  List<int> AssingedOrganizations()
    //{
    //    List<int> userOrganizations =new List<int>();
    //    int employeeId = GetActionBy();
    //    userOrganizations = _context.UserOrganizations.Where(x => x.UserId == employeeId).Select(s=>s.OrganizationId).ToList();
        
    //    int countuserOrganizations= userOrganizations.Count();

    //    return userOrganizations;
    //}

    //public List<int> getUserOrganizations()
    //{
    //    List<int> userOrganizations = new List<int>();
    //    int employeeId = GetActionBy();
    //    userOrganizations = _context.UserOrganizations.Where(x => x.UserId == employeeId).Select(s => s.Id).ToList();
    //    int countuserOrganizations = userOrganizations.Count();
    //    return userOrganizations;
    //}

    //public List<int> GetUserDocumentType(List<int> userOrganizationIds)
    //{
    //    return _context.UserDocumentTypes.Where(x => userOrganizationIds.Any(a=>a==x.UserOrganizationId) && x.IsActive).Select(a=>a.DocumentTypeId).ToList();
    //}
}
