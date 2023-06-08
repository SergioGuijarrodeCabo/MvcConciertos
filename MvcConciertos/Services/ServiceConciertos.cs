using MvcConciertos.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MvcConciertos.Services
{


    public class ServiceConciertos
    {

        private MediaTypeWithQualityHeaderValue header;
        private string UrlApi;

        public ServiceConciertos(IConfiguration configuration, KeysModel keysModel)
        {
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            //this.UrlApi = keysModel.ApiConciertos;
            this.UrlApi =  configuration.GetValue<string>("Urls:ApiConciertos");
        }

        #region GENERAL
        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                string url = this.UrlApi + request;

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        #endregion 

        #region CRUD
        

        public async Task<List<Evento>> GetEventosAsync()
        {
            string request = "/api/evento/geteventos";
            List<Evento> eventos = await this.CallApiAsync<List<Evento>>(request);
            return eventos;
        }

        public async Task<List<CategoriaEvento>> GetCategoriaEventosAsync()
        {
            string request = "/api/evento/getcategorias";
            List<CategoriaEvento> categorias = await this.CallApiAsync<List<CategoriaEvento>>(request);
            return categorias;
        }


        public async Task<List<Evento>> GetEventosporCategoriaAsync(int idcategoria)
        {
            string request = "/api/evento/"  + idcategoria;
            List<Evento> eventos = await this.CallApiAsync<List<Evento>>(request);
            return eventos;
        }


        public async Task CreateEventoAsync(Evento evento)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/evento";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                string json = JsonConvert.SerializeObject(evento);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(this.UrlApi + request, content);
            }
        }

       
       
        #endregion
    }
}
