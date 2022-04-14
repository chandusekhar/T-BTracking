import { Component, OnInit } from '@angular/core';
import { ElsaService } from '../Service/elsa/elsa.service';
import { ShareServiceService } from '../Service/share-service.service';

@Component({
  selector: 'app-elsa',
  templateUrl: './elsa.component.html',
  styleUrls: ['./elsa.component.scss'],
})
export class ElsaComponent implements OnInit {
  definitions: any;
  definitionId: any;
  isVisible = false;
  constructor(private elsaService: ElsaService, public shareService: ShareServiceService) {}

  ngOnInit() {
    this.getListDefinitions();
  }
  getListDefinitions() {
    this.elsaService.getListDefinitions().subscribe(
      data => {
        this.definitions = data;
      },
      err => {
        console.log(err);
      }
    );
  }
  export(id) {
    this.definitionId = id;
    this.showModal();
  }
  showModal(): void {
    this.isVisible = true;
  }

  handleOk(): void {
    this.isVisible = false;
  }

  handleCancel(): void {
    this.isVisible = false;
  }
}
