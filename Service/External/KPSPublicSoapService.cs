using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcDogrula;

namespace Service.External
{
    public class KPSPublicSoapService
    {
        private readonly KPSPublicSoapClient _client;

        public KPSPublicSoapService()
        {
            _client = new KPSPublicSoapClient(KPSPublicSoapClient.EndpointConfiguration.KPSPublicSoap);
        }

        public async Task<bool> TcknDogrula(long Tc,string FirstName,string LastName,int Year)
        {
            var kpsResult = await _client.TCKimlikNoDogrulaAsync(Tc,FirstName,LastName,Year);
            return kpsResult.Body.TCKimlikNoDogrulaResult;
        }
    }
}
