using Data.Dtos.Base;
using Data.Dtos.Customer;
using Data.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ApiBaseResponseModel<List<CustomerListDto>>> GetAllCustomer();
        Task<ApiBaseResponseModel<List<CustomerListDto>>> GetCustomersBySearch(GetCustomersBySearchRequestParameters requestModel);
        Task<ApiBaseResponseModel<CustomerDto>> GetCustomerById(BaseRequestParameters requestModel);
        Task<ApiBaseResponseModel<bool>> AddCustomer(AddCustomerRequestParameters requestModel);
        Task<ApiBaseResponseModel<bool>> RemoveCustomer(BaseRequestParameters requestModel);
        Task<ApiBaseResponseModel<bool>> UpdateCustomer(UpdateCustomerRequestParameters requestModel);
    }
}
