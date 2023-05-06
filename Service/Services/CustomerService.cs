using Dapper;
using Data.Dtos.Base;
using Data.Dtos.Customer;
using Data.Models.Base;
using Data.Models.Connections;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Service.External;
using Service.Services.Cache.Interfaces;
using Service.Services.Interfaces;
using System;
using System.Globalization;
using System.Reflection;

namespace Service.Services
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IValidator<AddCustomerRequestParameters> _validatorAddCustomer;
        private readonly IValidator<UpdateCustomerRequestParameters> _validatorUpdateCustomer;
        public CustomerService(IOptions<ConnectionStrings> _connStrings, IHttpContextAccessor contextAccessor, IRedisCacheService redisCacheService, IValidator<AddCustomerRequestParameters> validatorAddCustomer, IValidator<UpdateCustomerRequestParameters> validatorUpdateCustomer) : base(_connStrings, contextAccessor)
        {
            _redisCacheService = redisCacheService;
            _validatorAddCustomer = validatorAddCustomer;
            _validatorUpdateCustomer = validatorUpdateCustomer;
        }

        public async Task<ApiBaseResponseModel<List<CustomerListDto>>> GetAllCustomer()
        {
            ApiBaseResponseModel<List<CustomerListDto>> responseModel = new();

            try
            {
                string cacheKey = "GetCustomersBySearch";
                var resultCache = await _redisCacheService.GetAsync<List<CustomerListDto>>(cacheKey);

                if (resultCache == null)
                {
                    using (var con = new SqlConnection(conStrings.DefaultConnection))
                    {
                        string allCustomerQuery = $@"SELECT
                                                Id,
                                                CONCAT(SUBSTRING(FirstName,1,2),'*****') FirstName,
                                                CONCAT(SUBSTRING(LastName,1,2),'*****') LastName,
                                                CONCAT('*******',SUBSTRING(TCKN,8,4)) TCKN,
                                                FORMAT (Birthdate, '**/**/yyyy') Birthdate
                                                FROM Customer WITH(NOLOCK)
                                                WHERE
                                                IsDeleted = 0";

                        responseModel.Data = (await con.QueryAsync<CustomerListDto>(allCustomerQuery)).ToList();
                        await _redisCacheService.SetAsync(cacheKey, responseModel.Data, TimeSpan.FromHours(30));
                    }
                }
                else
                {
                    responseModel.Data = resultCache?.ToList();
                }

                return responseModel;

            }
            catch (Exception ex)
            {
                responseModel.setCustomError("Müşteri listesi getirilemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                return responseModel;
            }
        }

        public async Task<ApiBaseResponseModel<List<CustomerListDto>>> GetCustomersBySearch(GetCustomersBySearchRequestParameters requestModel)
        {
            ApiBaseResponseModel<List<CustomerListDto>> responseModel = new();

            try
            {
                string cacheKey = "GetCustomersBySearch-" + requestModel.SearchKey;
                var resultCache = await _redisCacheService.GetAsync<List<CustomerListDto>>(cacheKey);

                var searchQuery = "";
                if (requestModel.SearchKey != null)
                {
                    searchQuery = "AND (FirstName LIKE '%'+@SearchKey+'%' OR LastName LIKE '%'+@SearchKey+'%' OR TCKN LIKE '%'+@SearchKey+'%')";
                }

                if (resultCache == null)
                {
                    using (var con = new SqlConnection(conStrings.DefaultConnection))
                    {
                        string allCustomerQuery = $@"SELECT
                                                Id,
                                                CONCAT(SUBSTRING(FirstName,1,2),'*****') FirstName,
                                                CONCAT(SUBSTRING(LastName,1,2),'*****') LastName,
                                                CONCAT('*******',SUBSTRING(TCKN,8,4)) TCKN,
                                                FORMAT (Birthdate, '**/**/yyyy') Birthdate
                                                FROM Customer WITH(NOLOCK)
                                                WHERE
                                                IsDeleted = 0
                                                {searchQuery}";

                        responseModel.Data = (await con.QueryAsync<CustomerListDto>(allCustomerQuery, new { requestModel.SearchKey })).ToList();
                        await _redisCacheService.SetAsync(cacheKey, responseModel.Data, TimeSpan.FromHours(30));
                    }
                }
                else
                {
                    responseModel.Data = resultCache?.ToList();
                }

                return responseModel;

            }
            catch (Exception ex)
            {
                responseModel.setCustomError("Müşteri listesi getirilemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                return responseModel;
            }
        }

        public async Task<ApiBaseResponseModel<CustomerDto>> GetCustomerById(BaseRequestParameters requestModel)
        {
            ApiBaseResponseModel<CustomerDto> responseModel = new();

            try
            {
                string cacheKey = "GetCustomersBySearch-" + requestModel.Id;
                var resultCache = await _redisCacheService.GetAsync<CustomerDto>(cacheKey);

                if (resultCache == null)
                {
                    using (var con = new SqlConnection(conStrings.DefaultConnection))
                    {
                        string customerQuery = @"SELECT
                                                Id,
                                                FirstName,
                                                LastName,
                                                TCKN,
                                                Birthdate
                                                FROM Customer WITH(NOLOCK)
                                                WHERE
                                                IsDeleted = 0
                                                AND Id = @Id";

                        var result = (await con.QueryAsync<CustomerDto>(customerQuery, new { requestModel.Id })).FirstOrDefault();
                        if (result == null)
                        {
                            responseModel.setCustomError("Müşteri bulunamdı.", MethodBase.GetCurrentMethod());
                            return responseModel;
                        }

                        responseModel.Data = result;
                        await _redisCacheService.SetAsync(cacheKey, responseModel.Data, TimeSpan.FromHours(30));
                    }
                }
                else
                {
                    responseModel.Data = resultCache;
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel.setCustomError("Müşteri getirilemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                return responseModel;
            }
        }

        public async Task<ApiBaseResponseModel<bool>> AddCustomer(AddCustomerRequestParameters requestModel)
        {
            ApiBaseResponseModel<bool> responseModel = new();

            try
            {
                var validationResult = await _validatorAddCustomer.ValidateAsync(requestModel);
                if (!validationResult.IsValid)
                {
                    responseModel.setCustomError(validationResult.ToString(), MethodBase.GetCurrentMethod());
                    return responseModel;
                }

                using (var con = new SqlConnection(conStrings.DefaultConnection))
                {
                    string customerCheckSql = "SELECT Id FROM Customer WITH(NOLOCK) WHERE TCKN = @TCKN AND IsDeleted = 0";
                    var customerCheckresult = (await con.QueryAsync<int>(customerCheckSql, new { requestModel.TCKN })).FirstOrDefault();
                    if (customerCheckresult > 0)
                    {
                        responseModel.setCustomError("Müşteri zaten kayıtlı.", MethodBase.GetCurrentMethod());
                        return responseModel;
                    }

                    DateTime convertedDate = DateTime.ParseExact(requestModel.Birthdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    KPSPublicSoapService kPSPublicSoapService = new KPSPublicSoapService();
                    var kpsResult = await kPSPublicSoapService.TcknDogrula(long.Parse(requestModel.TCKN), requestModel.FirstName, requestModel.LastName, convertedDate.Year);

                    if (!kpsResult)
                    {
                        responseModel.setCustomError("Müşteri bilgileri hatalı.", MethodBase.GetCurrentMethod());
                        return responseModel;
                    }

                    string sql = $@"INSERT INTO Customer(FirstName,LastName,Birthdate,TCKN,IsDeleted,CreatedOn,CreatedBy) VALUES(@FirstName,@LastName,@Birthdate,@TCKN,0,GETDATE(),@AppUserEmail)";
                    var result = await con.ExecuteAsync(sql, new { requestModel.FirstName, requestModel.LastName, requestModel.TCKN, BirthDate= convertedDate, AppUserEmail = "" });

                    if (result < 1)
                    {
                        responseModel.setCustomError("Müşteri oluşturulamadı. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                    }

                    _redisCacheService.RemoveKeysPattern("Customers");
                    responseModel.Data = true;
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                responseModel.setCustomError("Müşteri oluşturulamadı. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                return responseModel;
            }
        }

        public async Task<ApiBaseResponseModel<bool>> UpdateCustomer(UpdateCustomerRequestParameters requestModel)
        {
            ApiBaseResponseModel<bool> responseModel = new();

            try
            {
                var validationResult = await _validatorUpdateCustomer.ValidateAsync(requestModel);
                if (!validationResult.IsValid)
                {
                    responseModel.setCustomError(validationResult.ToString(), MethodBase.GetCurrentMethod());
                    return responseModel;
                }

                using (var con = new SqlConnection(conStrings.DefaultConnection))
                {
                    string customerCheckSql = "SELECT Id FROM Customer WITH(NOLOCK) WHERE Id = @Id";
                    var customerCheckresult = (await con.QueryAsync<int>(customerCheckSql, new { requestModel.Id })).FirstOrDefault();
                    if (customerCheckresult < 1)
                    {
                        responseModel.setCustomError("Müşteri bulunamadı.", MethodBase.GetCurrentMethod());
                        return responseModel;
                    }
                    
                    DateTime convertedDate = DateTime.ParseExact(requestModel.Birthdate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    KPSPublicSoapService kPSPublicSoapService = new KPSPublicSoapService();
                    var kpsResult = await kPSPublicSoapService.TcknDogrula(long.Parse(requestModel.TCKN), requestModel.FirstName, requestModel.LastName, convertedDate.Year);

                    if (!kpsResult)
                    {
                        responseModel.setCustomError("Müşteri bilgileri hatalı.", MethodBase.GetCurrentMethod());
                        return responseModel;
                    }

                    string sql = $@"UPDATE Customer SET FirstName = @FirstName , LastName = @LastName , TCKN = @TCKN, Birthdate = @BirthDate , ModifiedOn = GETDATE() , ModifiedBy = @AppUserEmail WHERE Id = @Id";
                    var result = await con.ExecuteAsync(sql, new { requestModel.Id, requestModel.FirstName, requestModel.LastName, requestModel.TCKN, Birthdate = convertedDate, AppUserEmail = "" });

                    if (result < 1)
                    {
                        responseModel.setCustomError("Müşteri güncellenemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                    }

                    _redisCacheService.RemoveKeysPattern("Customers");
                    responseModel.Data = true;
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                responseModel.setCustomError("Müşteri güncellenemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                return responseModel;
            }
        }

        public async Task<ApiBaseResponseModel<bool>> RemoveCustomer(BaseRequestParameters requestModel)
        {
            ApiBaseResponseModel<bool> responseModel = new();

            try
            {
                using (var con = new SqlConnection(conStrings.DefaultConnection))
                {
                    string customerCheckSql = "SELECT Id FROM Customer WITH(NOLOCK) WHERE Id = @Id AND IsDeleted = 0";
                    var customerCheckresult = (await con.QueryAsync<int>(customerCheckSql, new { requestModel.Id })).FirstOrDefault();
                    if (customerCheckresult < 1)
                    {
                        responseModel.setCustomError("Müşteri bulunamadı.", MethodBase.GetCurrentMethod());
                        return responseModel;
                    }

                    string sql = $@"UPDATE Customer SET IsDeleted = 1 WHERE Id = @Id";
                    var result = await con.ExecuteAsync(sql, new { requestModel.Id });

                    if (result < 1)
                    {
                        responseModel.setCustomError("Müşteri silinemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                    }

                    responseModel.Data = true;
                    _redisCacheService.RemoveKeysPattern("Customers");
                    return responseModel;
                }
            }
            catch (Exception ex)
            {
                responseModel.setCustomError("Müşteri silinemedi. Beklenmeyen bir hata oluştu.", MethodBase.GetCurrentMethod());
                return responseModel;
            }
        }
    }
}
