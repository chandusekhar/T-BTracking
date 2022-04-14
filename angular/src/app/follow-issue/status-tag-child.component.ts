import { Component, OnInit} from '@angular/core';

@Component({

  selector: 'status-tag-child.',
  template: `<nz-tag [nzColor]=color>{{name}}</nz-tag>`,
})
export class StatusTagChildComponent implements OnInit {
  color: string;
  name: string;
  constructor() { }
  ngOnInit() {
  }
}
