import { Component, Inject } from '@angular/core';
import { Http, RequestOptions, RequestOptionsArgs } from '@angular/http';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent {
    public cronograma?: Cronograma = undefined;
    public numbers?: number[];
    public cargaHorariaTotal: number = 68;
    public disciplinaSemestral: boolean = true;
    public primeiroSemestre: boolean = true;
    public campus: string = "Cedeteg";
    public campusAvancado: string = "";

    diasSemanaAula = [
        { name: 'segundaFeira', label: 'Segunda-Feira', value: '' },
        { name: 'tercaFeira', label: 'Terça-Feira', value: '' },
        { name: 'quartaFeira', label: 'Quarta-Feira', value: '' },
        { name: 'quintaFeira', label: 'Quinta-Feira', value: '' },
        { name: 'sextaFeira', label: 'Sexta-Feira', value: '' },
        { name: 'sabado', label: 'Sábado', value: '' },
    ]

    campi = [
        { value: 'Cedeteg', label: 'Cedeteg' },
        { value: 'Irati', label: 'Irati' },
        { value: 'Santa Cruz', label: 'Santa Cruz' }
    ]

    
    campiAvancado = [
        { value: '', label: 'Nenhum' },
        { value: 'Chopinzinho', label: 'Chopinzinho' },
        { value: 'Coronel Vivida', label: 'Coronel Vivida' },
        { value: 'Laranjeiras do Sul', label: 'Laranjeiras do Sul' },
        { value: 'Pitanga', label: 'Pitanga' },
        { value: 'Prudentópolis', label: 'Prudentópolis' },
    ]

    public tdPadRight(aulasMes: number, maximoAulasMes: number): number[] {
        return Array.from({ length: (maximoAulasMes - aulasMes) }, (v, k) => k + 1);
    }

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }

    public gerarCronograma() {
        const payload = {
            cargaHorariaTotal: this.cargaHorariaTotal,
            segundaFeira: this.diasSemanaAula[0].value,
            tercaFeira: this.diasSemanaAula[1].value,
            quartaFeira: this.diasSemanaAula[2].value,
            quintaFeira: this.diasSemanaAula[3].value,
            sextaFeira: this.diasSemanaAula[4].value,
            sabado: this.diasSemanaAula[5].value,
            campus: this.campus,
            campusAvancado: this.campusAvancado,
            disciplinaSemestral: this.disciplinaSemestral,
            primeiroSemestre: this.primeiroSemestre
        }
        console.log(payload);

        let options: RequestOptionsArgs = {};
        options.params = payload;
        this.http.get(this.baseUrl + 'api/cronograma', options).subscribe(result => {
            this.cronograma = result.json() as Cronograma;
            this.numbers = Array.from({ length: (this.cronograma.maximoAulasMes) }, (v, k) => k + 1);
        }, error => console.error(error));
    }
}

class Cronograma {
    public meses: Mes[] = [];
    public cargaHorariaTotal: number = 0;
    public maximoAulasMes: number = 0;
}

class Mes {
    nome: string = "";
    dias: number[] = [];
}
