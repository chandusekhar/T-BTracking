import {
  AfterViewChecked,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { MentionOnSearchTypes } from 'ng-zorro-antd/mention';
import { NzModalService } from 'ng-zorro-antd/modal';
import { Attachment } from 'src/app/models/attachment';
import { Message } from 'src/app/models/Message';
import { ConversationService } from 'src/app/Service/Conversation/conversation.service';
import { CreateMessageService } from 'src/app/Service/Message/create-message.service';
import { ShareServiceService } from 'src/app/Service/share-service.service';
import { SignalRService } from 'src/app/Service/SignalR/signal-r.service';
import { SocketServiceService } from 'src/app/Service/SocketService/socket-service.service';

@Component({
  selector: 'app-scroll-mini',
  templateUrl: './scroll-mini.component.html',
  styleUrls: ['./scroll-mini.component.scss'],
})
export class ScrollMiniComponent implements OnInit, AfterViewChecked {
  @Input() item: any;
  @ViewChild('mini') MiniPanel!: ElementRef;
  @Output() closeChat = new EventEmitter<string>();
  @Input() test1 = '';
  @Output() resetCount = new EventEmitter<number>();
  @Output() NoMember = new EventEmitter<string>();
  @Input() ShowUnread: boolean;

  isCheckMention = false;
  isShowInfo = false;
  showUnread = true;
  listUserAll: any;
  MaxResultCount = 20;
  pages = 0;
  test = [];
  conversationList = [];
  listImageExtension = ['.jpg', '.png', '.jpeg', '.gif'];
  listAttachmentExtension = ['.zip', '.rar', '.txt', '.docx', '.pdf', '.doc', '.pptx', '.xlsx'];
  content = '';
  type = 0;
  inputConten = '';
  listAttachmentId = [];
  listAttachments: Attachment[] = [];
  conversationId = [];
  isLoading = false;
  load = 'Load more Message';
  typeConver: number;
  nameUser = [];
  files: any;
  visible: boolean = true;
  conversationCurrent;
  show: boolean = false;
  conversationListAvtive = [];
  currentRouter = '';
  showMessProject: boolean = true;
  message: any;
  onLoading = false;
  ScrollMini: boolean = true;
  unreadMess: number = 0;
  connectionHub: any;
  countUnread: number = 0;
  listShowMessageTime = [];
  listAll: any;
  typeConversation: any;
  avartarUser: string = '';
  showIconChat = false;
  setBottom = true;
  previousScrollHeightMinusTop: number;
  infoUser: any;
  listUserInGroup: any;
  MaxResultCountMember = 20;
  skipCountMember = 0;
  ///////////tag
  listProjectTag = [];
  projectChoose: string = '';
  tagName: any;
  memtionTemp;
  constructor(
    private createMessageService: CreateMessageService,
    public shareService: ShareServiceService,
    public conversationService: ConversationService,
    private signalRService: SignalRService,
    private modal: NzModalService,
    public router: Router,
    private socketService: SocketServiceService,
  ) {}
  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnInit(): void {
    this.connectionHub = this.signalRService.connection;
    this.signalRService.SetConnection();
    this.getCurrentRout();
    this.p_getChat();
    this.getName();
    this.getListMember();
    this.getAllUser();
    this.toggle(this.item);
    this.getMessage(this.item);
    this.listenDelete();
    // this.getListProjectTag()
  }
  getCurrentRout() {
    this.router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        this.currentRouter = e.url;
      }
    });
  }
  showtime(idMessage, content) {
    if (this.listShowMessageTime.includes(idMessage)) {
      var index = this.listShowMessageTime.lastIndexOf(idMessage);
      this.listShowMessageTime.splice(index, 1);
    } else {
      this.listShowMessageTime.push(idMessage);
    }
  }
  playAudio() {
    let audio = new Audio();
    audio.src = '../../assets/audio/Nhac-chuong-tin-nhan-Messenger.mp3';
    audio.load();
    audio.play();
  }
  //get tin nhắn trong cuộc trò truyện
  getMessage(conversationId) {
    if (conversationId == this.item) {
      this.createMessageService
        .getMessage(conversationId, this.MaxResultCount, this.pages)
        .subscribe(data => {
          this.message = data;
          if (this.message.items.length >= 1) {
            this.message.items.reduce((pre: Message, next: Message) => {
              if (this.message.items.length === 1) {
                this.message.items[0].showDate = true;
              }
              if (
                new Date(next.creationTime).getUTCDate() ===
                  new Date(pre.creationTime).getUTCDate() &&
                new Date(next.creationTime).getMonth() === new Date(pre.creationTime).getMonth() &&
                new Date(next.creationTime).getFullYear() ===
                  new Date(pre.creationTime).getFullYear()
              ) {
                next.showDate = false;
                pre.showDate = false;
              } else {
                next.showDate = true;
                pre.showDate = true;
              }
              return next;
            });
          }
          data['items'].reverse();
          this.setBottom = false;
        });
    }
  }
  //getMore
  //xem them tin nhắn
  showmore() {
    this.isLoading = true;
    setTimeout(() => {
      this.isLoading = false;
      this.MaxResultCount += 10;
      this.getMessage(this.item);
      this.MiniPanel.nativeElement.scrollTop = 125;
    }, 500);
  }
  getCurrentConversation(id) {
    this.conversationCurrent = id;
  }
  ////////get message
  Content(content: string) {
    if (!this.isCheckMention) {
      if (!content.trim()) {
        this.inputConten = '';
      }
      if (this.listAttachments.length > 0) {
        this.type = 1;
        (this.content = content), this.creatMessInConve();
      } else if (content.length === 0 || content.trim()) {
        this.content = content;
        this.creatMessInConve();
      }
    } else {
      this.isCheckMention = false;
    }
  }
  deleteImg(id) {
    var index = this.listAttachments.lastIndexOf(id);
    this.listAttachments.splice(index, 1);
    this.listAttachmentId.forEach(element => {
      if (element == id) {
        this.createMessageService.deleteAttachment(element).subscribe();
      }
    });
  }
  creatMessInConve() {
    this.createMessageService
      .CreateMessageAPI(this.content, this.type, this.item, this.listAttachmentId)
      .subscribe(data => {
        this.signalRService.connection.invoke(
          'MiniChat',
          this.item,
          this.shareService.getUserData?.id
        );
        this.inputConten = '';
        this.listAttachmentId = [];
        this.listAttachments = [];
        this.type = 0;
        try {
          let t = 0;
          let h = this.MiniPanel.nativeElement.offsetHeight;
          setTimeout(() => {
            while (
              this.MiniPanel?.nativeElement.scrollTop + h <
                this.MiniPanel?.nativeElement.scrollHeight &&
              t < 5000
            ) {
              this.MiniPanel.nativeElement.scrollTop = this.MiniPanel.nativeElement.scrollHeight;
              t++;
            }
          }, 200);
        } catch (err) {}
      });
  }
  checkAttachmentOrImage(file: string): any {
    if (this.listAttachmentExtension.indexOf(file) === -1) {
      // là image
      return false;
    } else {
      //là attachment
      return true;
    }
  }
  download(downloadUrl: string): void {
    window.open(downloadUrl, '_blank');
  }

  ///attachMent
  createAttachMent(file: File) {
    this.files = file;
    for (const file of this.files) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.createMessageService.createAttachment(file).subscribe(
          data => {
            if (data.body) {
              this.listAttachments.push(data.body);
              this.listAttachmentId.push(data.body.id);
              this.Content('');
            }
          },
          error => {
            console.log('err', error);
          }
        );
      };
      reader.readAsDataURL(file);
    }
  }
  getListMember() {
    this.createMessageService
      .getListMember(this.skipCountMember, this.MaxResultCountMember)
      .subscribe(data => {
        this.conversationService.countUnReadMessage = data['unreadConversation'];
      });
  }
  //close chat
  toggle(conversationId) {
    if (conversationId == this.item) {
      this.show = !this.show;
      this.setBottom = true;
      this.conversationService.positionMini = this.show;
    }
  }
  closedChat() {
    this.closeChat.emit(this.item);
    this.show = false;
    this.createMessageService.updateUnread(this.item).subscribe(data => {
      if (this.unreadMess > 0) {
        this.conversationService.countUnReadMessage--;
        this.unreadMess = 0;
      }
    });
  }
  getName() {
    this.createMessageService.getConversaion(this.item).subscribe(res => {
      if (res['conversationType'] == 1) {
        this.avartarUser = res['avatarUrl'];
      }
      this.typeConversation = res['conversationType'];
      res['listMembers'].forEach(element => {
        if (element.userId == this.shareService.getUserData?.id) {
          this.unreadMess = element.unReadMessage;
        }
      });
      if (res['conversationType'] == 1) {
        this.nameUser = res['nameConversation'];
        this.listUserInGroup = res['listMembers'];
      } else {
        res['listMembers'].forEach(element => {
          if (element.userId != this.shareService.getUserData?.id && res['conversationType'] == 0) {
            this.avartarUser = element.avatarUrl;
            this.conversationService.getInfoUser(element.userId).subscribe(data => {
              this.infoUser = data;
            });
          }
          if (element.userId != this.shareService.getUserData?.id) {
            if (element.name == null) {
              this.conversationService.getName(element.userId).subscribe(n => {
                this.nameUser = n.name;
              });
            } else {
              this.nameUser = element.name;
            }
          }
        });
      }
    });
  }
  ///scroll
  scrollToBottom(): void {
    if (this.setBottom) {
      try {
        let t = 0;
        let h = this.MiniPanel.nativeElement.offsetHeight;
        setTimeout(() => {
          while (
            this.MiniPanel?.nativeElement.scrollTop + h <
              this.MiniPanel?.nativeElement.scrollHeight &&
            t < 5000
          ) {
            this.MiniPanel.nativeElement.scrollTop = this.MiniPanel.nativeElement.scrollHeight;
            t++;
          }
        }, 200);
      } catch (err) {}
    }
  }
  onScroll(event): void {
    this.setBottom = false;
    if (this.onLoading) return;
    if (this.MiniPanel.nativeElement.scrollTop == 0) {
      // kiểm tra xem đã scroll đến đầu top hay chưa
      if (this.message.items?.length >= this.message.totalCount) {
        return;
      } else {
        this.showmore();
      }
      event.preventDefault();
    }
    event.preventDefault();
  }

  //////////delte
  deleteMessage(idMessage): void {
    this.modal.confirm({
      nzTitle: 'Are you sure delete this message?',
      nzContent:
        '<b style="color: red;">If you delete this Message, will not anyone see this message more</b>',
      nzOkText: 'Yes',
      nzOkType: 'primary',
      nzOkDanger: true,
      nzOnOk: () =>
        setTimeout(() => {
          this.createMessageService.deleteMessage(idMessage, this.item).subscribe(data => {});
        }, 200),
      nzCancelText: 'No',
    });
  }
  ////////////getName
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
  gotoMess() {
    this.router.navigate(['messenger']);
    this.conversationService.gotoMess = this.item;
  }

  //////////////socket
  p_getChat(): void {
    // socket nhận tin nhắn mới
    this.socketService.listen('chat').subscribe(data => {
      this.signalRService.connection.invoke(
        'MiniChat',
        data.conversationId,
        this.shareService.getUserData?.id
      );
      if (
        this.shareService.getUserData?.id != data.senderId &&
        this.unreadMess == 0 &&
        this.item == data.conversationId
      ) {
        this.conversationService.countUnReadMessage++;
      }
      if (data.senderId != this.shareService.getUserData?.id && this.item == data.conversationId) {
        this.showUnread = true;
        this.unreadMess++;
      }
      if (this.item != null && data.conversationId === this.item) {
        if (this.socketService.socket.id !== data.socketId) {
          data.lastModificationTime = null;
          this.message?.items.push(data);
        }
      }
      if (this.show) {
        // Lưu lại ví trí cuộn
        this.MiniPanel.nativeElement.scrollTop = !this.MiniPanel.nativeElement.scrollTop
          ? this.MiniPanel.nativeElement.scrollTop + 1
          : this.MiniPanel.nativeElement.scrollTop;

        this.previousScrollHeightMinusTop =
          this.MiniPanel.nativeElement.scrollHeight - this.MiniPanel.nativeElement.scrollTop;
        setTimeout(() => {
          this.MiniPanel.nativeElement.scrollTop =
            this.MiniPanel.nativeElement.scrollHeight - this.previousScrollHeightMinusTop;
        }, 100);
      }
    });
  }
  updateUnread() {
    this.showUnread = false;
    this.createMessageService.updateUnread(this.item).subscribe(data => {
      if (this.unreadMess > 0) {
        this.conversationService.countUnReadMessage--;
        this.unreadMess = 0;
      }
    });
  }
  ///xóa message
  listenDelete() {
    this.socketService.listen('delete-message').subscribe(data => {
      this.message?.items.map(res => {
        if (res.id == data.id) {
          res.content = 'Nội dung đã xoá';
        }
      });
    });
  }
  onSearchChange({ value, prefix }: MentionOnSearchTypes): void {
    if (prefix === '@') {
      this.getListProjectTag();
      this.isCheckMention = true;
    }
  }
  getListProjectTag() {
    if (this.router.url.includes('/project')) {
      this.projectChoose = this.shareService.getIdProject();
    } else {
      this.projectChoose = null;
    }
    this.conversationService.getListProjectTag(this.projectChoose).subscribe(data => {
      this.listProjectTag = data;
    });
  }
  ////////////////////////aaaaaaaaaaaaa
  valueWith = (data: { name: string; id: string }): string => data.name;

  onSelect(value): void {
    console.log(value);
    this.memtionTemp = '@' + value.name;
    this.tagName = value;
  }
}
