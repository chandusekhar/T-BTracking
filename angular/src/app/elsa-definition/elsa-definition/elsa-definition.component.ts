import { Component, Input, OnInit } from '@angular/core';
import { ElsaService } from 'src/app/Service/elsa/elsa.service';

@Component({
  selector: 'app-elsa-definition',
  templateUrl: './elsa-definition.component.html',
  styleUrls: ['./elsa-definition.component.scss'],
})
export class ElsaDefinitionComponent implements OnInit {
  @Input() definition = '2001f35934b5479a883f75b8d6c4ddaa';
  definitionTemplate: any;
  constructor(private elsaService: ElsaService) {}

  ngOnInit() {
    this.getDefinition(this.definition);
  }
  getDefinition(id) {
    this.elsaService.getDefinition(id).subscribe(data => {
      this.definitionTemplate = data;
    });
  }
}
