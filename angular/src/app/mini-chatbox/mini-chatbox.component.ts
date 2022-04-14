import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Attachment } from '../models/attachment';
import { ConversationService } from '../Service/Conversation/conversation.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { SocketServiceService } from '../Service/SocketService/socket-service.service';
@Component({
  selector: 'app-mini-chatbox',
  templateUrl: './mini-chatbox.component.html',
  styleUrls: ['./mini-chatbox.component.scss'],
})
export class MiniChatboxComponent implements OnInit {
  @ViewChild('mini') MiniPanel!: ElementRef;
  @Output() messProject = new EventEmitter<string>();
  @Input() test1 = '';
  @Output() positionMini = new EventEmitter();
  showUnread : boolean
  conversationList = [];
  type = 0;
  inputConten = '';
  listAttachmentId = [];
  listAttachments: Attachment[] = [];
  currentRouter = '';
  showMessProject: boolean = true;
  connectionHub: any;
  conversationId: string = ''
  countUnread: number = 0;
  listIdUser = []
  MaxResultCountMember = 20;
  skipCountMember = 0;
  constructor(
    private createMessageService: CreateMessageService,
    public shareService: ShareServiceService,
    public conversationService: ConversationService,
    private signalRService: SignalRService,
    public router: Router,
    private socketService : SocketServiceService,
  ) { }

  ngOnInit(): void {
    if (this.shareService.getUserData?.id)
    {
      this.signalRService.SetConnection();
      this.connectionHub = this.signalRService.connection;
      this.ReLoadMessage();
      this.getCurrentRout();
      this.getListMember();
      this.DrawerChat()
      this.p_getChat()
    }
  }
  getCurrentRout() {
    this.router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        this.currentRouter = e.url;

      }
    });
  }
    getListMember() {
    this.createMessageService.getListMember(this.skipCountMember,this.MaxResultCountMember).subscribe(data => {
      this.conversationService.countUnReadMessage = data['unreadConversation']
    });
  }
  DrawerChat() {
    this.signalRService.connection.on('DrawerChat', (conversationId, userId) => {
      this.getCurrentRout();
      if (!this.currentRouter.includes('messenger')) {
        this.conversationId = conversationId
        this.createMessageService.CheckUserInConversation(conversationId).subscribe(data => {
          if (data) {
            if (this.shareService.getUserData?.id == userId && !this.conversationList.includes(conversationId)) {
              this.conversationList.push(conversationId)
              this.positionMini.emit(this.conversationList)
            }
          }
        });
      }
    });
  }
  ReLoadMessage() {
    this.signalRService.connection.on('MiniChat', (conversationId, userId) => {
      this.getCurrentRout();
      if (!this.currentRouter.includes('messenger')) {
        this.conversationId = conversationId
        this.createMessageService.CheckUserInConversation(conversationId).subscribe(data => {
          if (data) {
            // console.log(userId)
            // console.log(this.shareService.getUserData?.id)
            if (this.shareService.getUserData?.id != userId && !this.conversationList.includes(conversationId)) {
              this.conversationList.push(conversationId)
              this.positionMini.emit(this.conversationList)
            }
          }
        });
      }
    });
  }
  CreateConversationDatabase(Data) {
    this.conversationService.CreateConversationInDatabase(Data).subscribe((res: any) => { });
  }
  closeChat($event) {
    var index = this.conversationList.lastIndexOf($event);
    this.conversationList.splice(index, 1);
    this.positionMini.emit(this.conversationList)
  }
  playAudio() {
    let audio = new Audio();
    audio.src = '../../assets/audio/Nhac-chuong-tin-nhan-Messenger.mp3';
    audio.load();
    audio.play();
  }
  p_getChat(): void {
    // socket nhận tin nhắn mới
    this.socketService.listen('chat').subscribe(data => {
      this.signalRService?.connection.invoke(
          'MiniChat',
          data.conversationId,
          data.senderId
      );
      if (data.senderId != this.shareService.getUserData?.id && this.conversationId == data.conversationId) {
        this.playAudio()
      }
    })
  }
}
