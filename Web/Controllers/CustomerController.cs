using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Web.Models.Authentication;
using Web.Models.Customer;
using Web.Models.Base;
using Web.Views.Base;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.Claims.SingleOrDefault(x => x.Type == "accessToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7231/api/Customer/GetAllCustomer");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                var list = JsonSerializer.Deserialize<ApiBaseResponseModel<List<CustomerListDto>>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return View(list?.Data);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchKey)
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.Claims.SingleOrDefault(x => x.Type == "accessToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response =
              client.SendAsync(
              new HttpRequestMessage(HttpMethod.Get, "https://localhost:7231/api/Customer/GetCustomersBySearch?SearchKey=" + searchKey)).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ApiBaseResponseModel<List<CustomerListDto>>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

                if (responseModel.Success)
                {
                    return View("Index", responseModel.Data);
                }
            }
            return RedirectToAction("Index", "Customer");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCustomerRequestModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.Claims.SingleOrDefault(x => x.Type == "accessToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var requestContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7231/api/Customer/AddCustomer", requestContent);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ApiBaseResponseModel<bool>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });


                if (responseModel.Success && responseModel.Data)
                {
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ModelState.AddModelError("", responseModel.Error);
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu.");
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(BaseRequestParameters model)
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.Claims.SingleOrDefault(x => x.Type == "accessToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var requestContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response =
              client.SendAsync(
              new HttpRequestMessage(HttpMethod.Delete, "https://localhost:7231/api/Customer/RemoveCustomer")
              {
                  Content = requestContent
              }).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ApiBaseResponseModel<bool>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });


                if (responseModel.Success && responseModel.Data)
                {
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ModelState.AddModelError("", responseModel.Error);
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu.");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(BaseRequestParameters model)
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.Claims.SingleOrDefault(x => x.Type == "accessToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response =
              client.SendAsync(
              new HttpRequestMessage(HttpMethod.Get, "https://localhost:7231/api/Customer/GetCustomerById?Id=" + model.Id)).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ApiBaseResponseModel<CustomerDto>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

                if (responseModel.Success)
                {
                    var updateDto = new UpdateCustomerModel()
                    {
                        Id = model.Id,
                        FirstName = responseModel.Data.FirstName,
                        LastName = responseModel.Data.LastName,
                        Birthdate = responseModel.Data.Birthdate,
                        TCKN = responseModel.Data.TCKN,
                    };

                    return View(updateDto);
                }
            }
            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCustomerModel model)
        {
            var client = _httpClientFactory.CreateClient();
            var token = User.Claims.SingleOrDefault(x => x.Type == "accessToken")?.Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var requestContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response =
              client.SendAsync(
              new HttpRequestMessage(HttpMethod.Put, "https://localhost:7231/api/Customer/UpdateCustomer")
              {
                  Content = requestContent
              }).Result;

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var responseModel = JsonSerializer.Deserialize<ApiBaseResponseModel<bool>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });


                if (responseModel.Success && responseModel.Data)
                {
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ModelState.AddModelError("", responseModel.Error);
                    return View(model);
                }

            }
            else
            {
                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu.");
                return View(model);
            }
        }
    }
}
