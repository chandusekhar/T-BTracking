import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ConversationService } from 'src/app/Service/Conversation/conversation.service';
import { CreateMessageService } from 'src/app/Service/Message/create-message.service';
import { ShareServiceService } from 'src/app/Service/share-service.service';
import { SignalRService } from 'src/app/Service/SignalR/signal-r.service';
import { UserService } from 'src/app/Service/User/user.service';

@Component({
  selector: 'app-drawer-chat',
  templateUrl: './drawer-chat.component.html',
  styleUrls: ['./drawer-chat.component.scss'],
})
export class DrawerChatComponent implements OnInit {
  @Output() drawerFalse = new EventEmitter<boolean>();
  listUserAll: any;
  listAll: any;
  conversationId: string = '';
  selected: any;
  connectionHub: any;
  listIdUser = [];
  nameProject: string = '';
  conversationIdGroup: string = '';
  search: string = '';
  received: any;
  messagesLoading = false;
  MaxResultCountMember = 20;
  skipCountMember = 0;
  constructor(
    private createMessageService: CreateMessageService,
    public conversationService: ConversationService,
    public shareService: ShareServiceService,
    private signalRService: SignalRService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.signalRService.SetConnection();
    this.connectionHub = this.signalRService.connection;
    this.getAllUser();
    this.getListMember();
    this.getNameProject();
  }
  seachUser(input) {
    if (input == '') {
      this.getListMember();
    } else {
      this.conversationService.getListUserAll(input).subscribe(data => {
        if (data) {
          this.listUserAll = data.items;
        }
      });
    }
  }
  onScroll(event) {
    if(this.listUserAll.length> 0)
    {
      if (event.target.scrollHeight - event.target.scrollTop == event.target.offsetHeight) {
        this.createMessageService.getListMember(this.listUserAll.length, 10).subscribe(data => {
          this.listUserAll = this.listUserAll.concat(data['items']);
        });
      }
    }
  }
  getListMember() {
    if (this.listUserAll) {
      this.messagesLoading = false;
    } else {
      this.messagesLoading = true;
    }
    this.createMessageService
      .getListMember(this.skipCountMember, this.MaxResultCountMember)
      .subscribe(data => {
        this.listUserAll = data['items'];
        console.log(data)
        this.messagesLoading = false;
      });
  }
  getAllUser() {
    this.conversationService.getAll().subscribe(data => {
      this.listAll = data.items;
    });
  }
  getNameUserInGroup(userId) {
    if (this.listAll != null) {
      if (this.shareService.getUserData?.id != null) {
        var result = this.listAll?.filter(obj => {
          return obj.id == userId;
        });
        return result[0]?.name;
      }
    }
  }
  GetCharAt(name) {
    var rs = '';
    name.split(' ').forEach(char => (rs += char.charAt(0)));
    return rs;
  }
  CreateConversition(IdUserReceived) {
    this.received = IdUserReceived;
    this.createMessageService.CheckConversation(IdUserReceived).subscribe(data => {
      if (data['hasConversation']) {
        this.conversationId = data['conversationId'];
        this.search = '';
        this.connectionHub.invoke(
          'DrawerChat',
          data['conversationId'],
          this.shareService.getUserData.id
        );
        this.drawerFalse.emit(false);
      } else {
        this.createMessageService.CreateConversition(IdUserReceived).subscribe(data => {
          this.conversationId = data.conversationId;
          this.connectionHub.invoke(
            'DrawerChat',
            data.conversationId,
            this.shareService.getUserData.id
          );
          this.drawerFalse.emit(false);
          this.search = '';
        });
      }
    });
  }
  getConversation(conversationId) {
    if (this.conversationId != conversationId) {
      if (this.search == '') {
        this.drawerFalse.emit(false);
        this.conversationId = conversationId;
        this.connectionHub.invoke('DrawerChat', conversationId, this.shareService.getUserData.id);
      }
    }
  }
  getNameProject() {
    if (localStorage?.getItem('ProjectId')) {
      this.conversationService
        .getNameProject(localStorage?.getItem('ProjectId'))
        .subscribe(data => {
          this.nameProject = data.name;
        });
    }
  }
  getConversationProject() {
    this.conversationService
      .CheckConversation(localStorage?.getItem('ProjectId'))
      .subscribe(res => {
        if (res) {
          if (localStorage.getItem('ProjectId') != null) {
            this.conversationService
              .getConversationDetail(localStorage?.getItem('ProjectId'))
              .subscribe(res => {
                this.connectionHub.invoke(
                  'DrawerChat',
                  res.conversationId,
                  this.shareService.getUserData.id
                );
              });
            this.drawerFalse.emit(false);
          }
        } else {
          this.conversationService
            .getNameProject(localStorage?.getItem('ProjectId'))
            .subscribe(id => {
              this.userService.getListUserByIdProject(id.id).subscribe(name => {
                if (name.items.length == 1) {
                  this.createMessageService.createMessage('error', 'Group has no Member');
                } else {
                  name.items.forEach(element => {
                    if (element.id != this.shareService.getUserData?.id) {
                      this.listIdUser.push(element.id);
                    }
                  });
                  this.createMessageService
                    .CreateGroupChat(id.name, this.listIdUser)
                    .subscribe(data => {
                      const res = {
                        idProject: localStorage?.getItem('ProjectId'),
                        conversationId: data.conversationId,
                      };
                      this.connectionHub.invoke(
                        'DrawerChat',
                        res.conversationId,
                        this.shareService.getUserData.id
                      );
                      this.CreateConversationDatabase(res);
                      this.drawerFalse.emit(false);
                    });
                }
              });
            });
        }
      });
  }
  CreateConversationDatabase(Data) {
    this.conversationService.CreateConversationInDatabase(Data).subscribe((res: any) => {});
  }
}
