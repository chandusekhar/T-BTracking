import { Component, OnInit } from '@angular/core';
import { UserService } from '../Service/User/user.service';
import { CreateMessageService } from '../Service/Message/create-message.service';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.scss'],
})
export class BoardComponent implements OnInit {
  Host;
  Collection;
  disable = true;
  ButtonText: string = '';
  HostId;
  expandSet = new Set<number>();
  ProjectTFS;

  constructor(private userService: UserService, private createMessage: CreateMessageService) {}

  ngOnInit(): void {
    this.getHost();
  }

  getHost() {
    this.userService.GetHost().subscribe(data => {
      if (data) {
        this.ButtonText = 'Update';
        this.Host = data.host;
        this.Collection = data.collection;
        this.HostId = data.id;
        this.disable = false;
      } else this.ButtonText = 'Add';
    });
  }
  updateHostId(data, HostId) {
    this.userService.updateHost(data, HostId).subscribe(
      () => {
        this.getHost();
        this.createMessage.createMessage('success', 'successfully');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  createHost(data) {
    this.userService.CreateHost(data).subscribe(
      () => {
        this.getHost();
        this.createMessage.createMessage('success', 'successfully');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  updateHost() {
    if (!this.Host || !this.Collection) {
      this.createMessage.createMessage('error', 'Input not null, check and try again!');
      return;
    }
    const data = { host: this.Host, collection: this.Collection };
    if (this.HostId) {
      this.updateHostId(data, this.HostId);
    } else {
      this.createHost(data);
    }
  }
}
