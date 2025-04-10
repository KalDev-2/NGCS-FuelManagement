using NEXT_BMS.Models;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Utilities
{
    public interface IValidateRequest
    {
        bool ValidateKeys(string Key);

        bool IsValidServiceRequest(string Key, string ServiceName);

        bool ValidateIsServiceActive(string Key);

        //bool CalculateCountofRequest(string Key);
    }
    public class ValidateRequestConcrete : IValidateRequest
    {
        private NEXT_BMSContext _context;

        public ValidateRequestConcrete(NEXT_BMSContext context)
        {
            _context = context;
        }

        public bool ValidateKeys(string Key)
        {
            //try
            //{
            //    var result = (from accessKeyManager in _context.AccessKeys
            //                  where EF.Functions.Like(accessKeyManager.AccessKey1, "%" + Key + "%")
            //                  select accessKeyManager).Count();

            //    if (result > 0)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return false;
        }

        public bool IsValidServiceRequest(string Key, string ServiceName)
        {
            //try
            //{
            //    var result = _context.AccessKeys.Include(x=>x.Institute).Where(x=>x.AccessKey1==Key && x.Institute.Code==ServiceName).Count();

            //    //if (string.Equals(ServiceName, serviceName, StringComparison.InvariantCultureIgnoreCase))
            //    if(result > 0)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

            return false;
        }

        public bool ValidateIsServiceActive(string Key)
        {
            //try
            //{
            //    var result = (from accessKeyManager in _context.AccessKeys
            //                  where accessKeyManager.AccessKey1 == Key && accessKeyManager.IsActive == true
            //                  select accessKeyManager).Count();

            //    if (result > 0)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return false;
        }

        //public bool CalculateCountofRequest(string Key)
        //{
        //    try
        //    {

        //        var totalRequestCount = (from apimanagertb in _context.APIManagerTB
        //                                 join hittb in _context.HitsTB on apimanagertb.HitsID equals hittb.HitsID
        //                                 where apimanagertb.APIKey == Key
        //                                 select hittb.Hits).FirstOrDefault();

        //        var totalCurrentRequestCount = (from loggertb in _context.LoggerTB
        //                                        where loggertb.APIKey == Key
        //                                        select loggertb).Count();

        //        if (totalCurrentRequestCount >= totalRequestCount)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return true;
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
