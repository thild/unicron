using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Microsoft.AspNetCore.Mvc;
using unicron.Extensions;
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
            int sabado,
            bool disciplinaSemestral = true,
            bool primeiroSemestre = true
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

            var dataInicio = DateTime.Now;
            var dataEncerramento = DateTime.Now;
            if (disciplinaSemestral)
            {
                if (primeiroSemestre)
                {
                    dataInicio = calendar.Events.Where(m => m.Summary.Contains("[INICIO_SEM1]")).First().DtStart.Date;
                    dataEncerramento = calendar.Events.Where(m => m.Summary.Contains("[ENCERRAMENTO_SEM1_SEMESTRAL]")).First().DtStart.Date;
                }
                else
                {
                    dataInicio = calendar.Events.Where(m => m.Summary.Contains("[INICIO_SEM2]")).First().DtStart.Date;
                    dataEncerramento = calendar.Events.Where(m => m.Summary.Contains("[ENCERRAMENTO_SEM2_SEMESTRAL]")).First().DtStart.Date;
                }
            }
            else
            {
                dataInicio = calendar.Events.Where(m => m.Summary.Contains("[INICIO_SEM1]")).First().DtStart.Date;
                dataEncerramento = calendar.Events.Where(m => m.Summary.Contains("[ENCERRAMENTO_SEM2_ANUAL]")).First().DtStart.Date;
            }
            var recessos = recessosCalendar.SelectMany(m => m.DtStart.Date.Range(m.DtEnd.Date.AddDays(-1))).ToHashSet();

            return Cronograma.Generate(
                dataInicio,
                dataEncerramento,
                recessos,
                cargaHorariaTotal,
                segundaFeira,
                tercaFeira,
                quartaFeira,
                quintaFeira,
                sextaFeira,
                sabado);
        }



    }
}
