using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using unicron.Extensions;

namespace unicron.Models
{
    public class Cronograma
    {
        private Cronograma(
            int cargaHorariaTotal,
            int maximoAulasMes,
            int segundaFeira,
            int tercaFeira,
            int quartaFeira,
            int quintaFeira,
            int sextaFeira,
            int sabado,
            IEnumerable<Mes> meses)
        {
            this.CargaHorariaTotal = cargaHorariaTotal;
            this.MaximoAulasMes = maximoAulasMes;
            this.SegundaFeira = segundaFeira;
            this.TercaFeira = tercaFeira;
            this.QuartaFeira = quartaFeira;
            this.QuintaFeira = quintaFeira;
            this.SextaFeira = sextaFeira;
            this.Sabado = sabado;
            this.Meses = meses;
        }

        public IEnumerable<Mes> Meses { get; }

        public int CargaHorariaTotal { get; } = 68;
        public int SegundaFeira { get; }
        public int MaximoAulasMes { get; } = 20;
        public int TercaFeira { get; }
        public int QuartaFeira { get; }
        public int QuintaFeira { get; }
        public int SextaFeira { get; }
        public int Sabado { get; }

        public static Cronograma Generate(
            DateTime dataInicio,
            DateTime dataEncerramento,
            HashSet<DateTime> recessos,
            int cargaHorariaTotal,
            int segundaFeira,
            int tercaFeira,
            int quartaFeira,
            int quintaFeira,
            int sextaFeira,
            int sabado
            )
        {
            var diasAulas = new HashSet<DateTime>();

            var aulasDiaSemana = new int[] { 0, segundaFeira, tercaFeira, quartaFeira, quintaFeira, sextaFeira, sabado };

            foreach (var dia in dataInicio.Range(dataEncerramento))
            {
                if (aulasDiaSemana[(int)dia.DayOfWeek] != 0)
                {
                    diasAulas.Add(dia);
                    continue;
                }
            }
            var diasAulasDisponiveis = diasAulas.Except(recessos);

            var limiteCargaHoraria = 0;
            var meses = new Dictionary<int, Mes>();

            TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;

            foreach (var dia in diasAulasDisponiveis)
            {
                if (limiteCargaHoraria == cargaHorariaTotal) break;
                Mes mes;
                if (!meses.TryGetValue(dia.Month, out mes))
                {
                    mes = new Mes();
                    mes.Nome = textInfo.ToTitleCase(dia.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR")));
                    meses.Add(dia.Month, mes);
                }
                for (int i = 0; i < aulasDiaSemana[(int)dia.DayOfWeek]; i++)
                {
                    if (limiteCargaHoraria == cargaHorariaTotal) break;
                    mes.Dias.Add(dia.Day);
                    ++limiteCargaHoraria;
                }
            }
            var maximoAulasMes = 0;
            foreach (var mes in meses.Values)
            {
                int count = mes.Dias.Count;
                if (count > maximoAulasMes)
                {
                    maximoAulasMes = count;
                }
            }
            return new Cronograma(cargaHorariaTotal,
                maximoAulasMes,
                segundaFeira,
                tercaFeira,
                quartaFeira,
                quintaFeira,
                sextaFeira,
                sabado,
                meses.Values);
        }

    }
    public class Mes
    {
        public string Nome { get; set; }
        public IList<int> Dias { get; } = new List<int>();
    }

    public enum Categoria
    {
        Cedeteg=0,
        Chopinzinho=1,
        CoronelVivida=2,
        Irati=3,
        LaranjeirasDoSul=4,
        Pitanga=5,
        Prudentopolis=6,
        SantaCruz=7
    }
}