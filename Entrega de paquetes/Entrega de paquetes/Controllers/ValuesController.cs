using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Entrega_de_paquetes.Controllers
{
    public class Camion
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public DateTime FechaYHoraSalida { get; set; }
        public DateTime FechaYHoraDestino { get; set; }
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "El peso máximo es obligatorio")]
        public int PesoMaximo { get; set; }
        public int Carga { get; set; }
        public bool Baja { get; set; }
        public int PesoDisponible { get; set; }
        public List<Paquete> PaquetesAsignados { get; set; }


    }

    [RoutePrefix("Api/Camion")]
    public class CamionController : ApiController
    {
        public static List<Camion> Camiones = new List<Camion>(); 

        [Route("ObtenerCamiones")]
        public IHttpActionResult Get()
        {
            return Ok(Camiones);
        }

        [Route("ObtenerCamion/{id}")]
        public IHttpActionResult Get(int id)
        {
            var camion = BuscarCamion(id);

            if (!ModelState.IsValid)
                return BadRequest();

            if (camion == null)
                return NotFound();

            return Ok(camion);
        }

        [Route("CargarCamion")]
        public IHttpActionResult Post(Camion camion)
        {
            if (!ModelState.IsValid)
                return BadRequest("Error en la carga de datos");

            camion.PaquetesAsignados = new List<Paquete>();
            Camiones.Add(camion);

            return Created("", $"Id: {camion.Id}. Fecha de llegada: {camion.FechaYHoraDestino.ToShortDateString()}");
        }

        [Route("ModificarPesoMaximo")]
        public IHttpActionResult Put(int id, int pesoMaximo)
        {
            var camion = BuscarCamion(id);
            if (camion == null)
                return NotFound();

            if (camion.Carga > pesoMaximo)
                return BadRequest("El peso maximo que quiere modificar es menor que la carga asignada");

            camion.PesoMaximo = pesoMaximo;

            return Ok();
        }

        [Route("BorrarCamion")]
        public IHttpActionResult Delete(int id)
        {
            Camion camionBaja = BuscarCamion(id);
            if (camionBaja == null)
                return NotFound();

            camionBaja.Baja = true;

            return Ok(camionBaja);
        }

        public Camion BuscarCamion(int id)
        {
            return Camiones.FirstOrDefault(x=>x.Id==id);
        }
    }
    public class Paquete
    {
        [Required(ErrorMessage = "Falta cargar el peso")]
        public int Peso { get; set; }
        public int Id { get; set; }
        public string CiudadDeOrigen { get; set; }
        public bool Baja { get; set; }

    }

    [RoutePrefix("Api/Paquete")]
    public class PaqueteController : ApiController
    {

        [Route("CargarPaquete")]
        public IHttpActionResult Post(Paquete paquete)
        {
            if (!ModelState.IsValid)
                return BadRequest("Estan mal cargados los datos");

            if(!AsignarPaquete(paquete))
            {
                return BadRequest("No hay camion disponible para el tamaño del paquete que se quiere cargar");
            }

            return Ok(paquete);
        }

        public void Put(int id, Paquete paquete)
        {

        }

        public void Delete(int id)
        {

        }

        public bool AsignarPaquete(Paquete paquete)
        {
            foreach (Camion camion in CamionController.Camiones)
            {
                if (camion.PesoDisponible >= paquete.Peso)
                {
                    camion.PaquetesAsignados.Add(paquete);
                    camion.PesoDisponible -= paquete.Peso;
                    return true;
                }   
            }
            return false;
        }
    }


}
