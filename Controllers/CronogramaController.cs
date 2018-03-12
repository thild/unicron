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
            string campus,
            string campusAvancado = "",
            bool disciplinaSemestral = true,
            bool primeiroSemestre = true
            )
        {
            Calendar calendar;
            IEnumerable<Ical.Net.CalendarComponents.CalendarEvent> recessosCalendar;
            using (var sr = new StreamReader("Data/calendario.vcs"))
            {
                calendar = Calendar.Load(sr.ReadToEnd());
            }
            if (string.IsNullOrEmpty(campusAvancado))
            {
                recessosCalendar = calendar.Events.Where(m => m.Categories.Any(n => n.Contains(campus)));
            }
            else
            {
                recessosCalendar = calendar.Events.Where(m => m.Categories.Any(n => n.Contains(campus) || n.Contains(campusAvancado)));
            }

            // recessosCalendar = recessosCalendar.Where(
            //     m => m.Summary.Contains("[RECESSO]") ||
            //          m.Summary.Contains("[FERIADO]")
            // );

            var dataInicio = DateTime.Now;
            var dataEncerramento = DateTime.Now;
            HashSet<DateTime> recessos;
            if (disciplinaSemestral)
            {
                recessosCalendar = recessosCalendar.Where(
                    m => m.Summary.Contains("[RECESSO]") ||
                         m.Summary.Contains("[RECESSO_SEMESTRAL]") ||
                         m.Summary.Contains("[FERIADO]")
                );
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
                recessosCalendar = recessosCalendar.Where(
                    m => m.Summary.Contains("[RECESSO]") ||
                         m.Summary.Contains("[RECESSO_ANUAL]") ||
                         m.Summary.Contains("[FERIADO]")
                );
                dataInicio = calendar.Events.Where(m => m.Summary.Contains("[INICIO_SEM1]")).First().DtStart.Date;
                dataEncerramento = calendar.Events.Where(m => m.Summary.Contains("[ENCERRAMENTO_SEM2_ANUAL]")).First().DtStart.Date;
            }
            recessos = recessosCalendar
                    .SelectMany(m => m.DtStart.Date.Range(m.DtEnd.Date.AddDays(-1))).ToHashSet();
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
