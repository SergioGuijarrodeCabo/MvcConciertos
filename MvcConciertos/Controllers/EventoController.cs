using Microsoft.AspNetCore.Mvc;
using MvcConciertos.Models;
using MvcConciertos.Services;

namespace MvcConciertos.Controllers
{
    public class EventoController : Controller
    {

        private ServiceConciertos service;
        private ServiceStorageS3 serviceS3;

        public EventoController(ServiceConciertos service, ServiceStorageS3 serviceS3)
        {
            this.service = service;
           this.serviceS3 = serviceS3;
        }


        public async Task<IActionResult> Index(int? idcategoria)
        {
            List<Evento> eventos;

            if (idcategoria.HasValue)
            {
                eventos = await this.service.GetEventosporCategoriaAsync(idcategoria.Value);
            }
            else
            {
                eventos = await this.service.GetEventosAsync();
            }

            List<CategoriaEvento> categorias = await this.service.GetCategoriaEventosAsync();
            ViewBag.Categorias = categorias;

            foreach (var evento in eventos)
            {
                try
                {
                    using (Stream imageStream = await this.serviceS3.GetFileAsync(evento.Imagen))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await imageStream.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();
                            string base64Image = Convert.ToBase64String(bytes);
                            evento.Imagen = "data:image;base64," + base64Image;
                        }
                    }
                }
                catch (Exception ex)
                {
                    evento.Imagen = null;
                }
            }

            return View(eventos);
        }









        public async Task<IActionResult> Categorias()
        {
            List<CategoriaEvento> categorias = await this.service.GetCategoriaEventosAsync();

            return RedirectToAction("Index");
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Evento evento, IFormFile file)
        {
            evento.Imagen = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceS3.UploadFileAsync(evento.Imagen, stream);
            }
            await this.service.CreateEventoAsync(evento);
            return RedirectToAction("Index");
        }


    }
}
