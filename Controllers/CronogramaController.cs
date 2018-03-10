using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Microsoft.AspNetCore.Mvc;
using unicron.Models;

namespace unicron.Controllers
{
    [Route("api/[controller]")]
    public partial class CronogramaController : Controller
    {
        [HttpGet()]
        public Cronograma Get(
            int cargaHorariaTotal,
            int segundaFeira,
            int tercaFeira,
            int quartaFeira,
            int quintaFeira,
            int sextaFeira,
            int sabado
            )
        {
            Calendar calendar;
            using (var sr = new StreamReader("Data/UBXXe1cv.vcs"))
            {
                calendar = Calendar.Load(sr.ReadToEnd());
            }
            var recessosCalendar = calendar.Events.Where(m => m.Categories.Any(n => n.Contains("Cedeteg")));
            recessosCalendar = recessosCalendar.Where(
                m => m.Summary.Contains("[RECESSO]") ||
                     m.Summary.Contains("[FERIADO]")
            );

            var dataInicio = calendar.Events.Where(m => m.Summary.Contains("[INICIO1SEM]")).First().DtStart.Date;
            var dataEncerramento = calendar.Events.Where(m => m.Summary.Contains("[ENCERRAMENTO1SEM]")).First().DtStart.Date;
            var recessos = recessosCalendar.Select(m => m.DtStart.Date).ToHashSet();

            return Cronograma.Generate(dataInicio, dataEncerramento, recessos, cargaHorariaTotal, segundaFeira, tercaFeira, quartaFeira, quintaFeira, sextaFeira, sabado);
        }

      
        
    }
}
