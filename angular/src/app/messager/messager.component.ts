import { DatePipe } from '@angular/common';
import {
  AfterViewChecked,
  Component,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import { MentionOnSearchTypes } from 'ng-zorro-antd/mention';
// import { NavigationEnd, Router } from '@angular/router';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { Attachment } from '../models/attachment';
import { Message } from '../models/Message';
import { ConversationService } from '../Service/Conversation/conversation.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { SocketServiceService } from '../Service/SocketService/socket-service.service';
// import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
@Component({
  selector: 'app-messager',
  templateUrl: './messager.component.html',
  styleUrls: ['./messager.component.scss'],
})
export class MessagerComponent implements OnInit, AfterViewChecked {
  panels = [
    {
      active: false,
      name: 'Shared Images',
    },
  ];
  panelAtt = [
    {
      active: false,
      name: 'Shared Files',
    },
  ];
  @Output() count = new EventEmitter<number>();
  countUnread: number = 0;
  @ViewChild('messagePanel') messagePanel!: ElementRef;
  chats = [];
  isLoading = false;
  pages = 0;
  isStarting = true;
  disableScrollDown = false;
  //////file
  listFile: any = [];
  fileList: NzUploadFile[];
  listAttachments: Attachment[] = [];
  listAttachmentId: string[] = [];
  isVisible = false;
  isVisibleChangName = false;
  isVisibleAddmember = false;
  isOkLoading = false;
  listUserAddMember: any;
  searchAdd: string = '';
  listImageExtension = ['.jpg', '.png', '.jpeg', '.gif'];
  listAttachmentExtension = ['.zip', '.rar', '.txt', '.docx', '.pdf', '.doc', '.pptx', '.xlsx'];
  listUserAll: any;
  received: any;
  filterUser: string = '';
  checkConversation: boolean;
  search: string = '';
  MaxResultCount = 20;
  listMember: string[] = [];
  conversation: any;
  content: string = null;
  inputConten: string = '';
  type: number = 0;
  conversationId: string = '';
  connectionHub: any;
  message: any;
  members: [];
  nameConversation: '';
  lastMessage: object;
  groupChat: any;
  listAll: any;
  nameUser: string = '';
  file: File;
  attachment: string[] = [];
  loadAtt = 0;
  idUser: string;
  idAttachment: string = null;
  files: any;
  filesAvartar: any;
  public load = 'Load more Message';
  //public loadAt = 'Load more att'
  firstConver: string = '';
  userId: string;
  CountMember: number;
  groupName = '';
  AddMember: any;
  selected: any;
  typeConver: any;
  listUserAddMemberTest;
  listImage = [];
  isDisposeMessageOpen = false;
  createrName: any;
  ImgResult: number = 12;
  AttResult: number = 12;
  LoadAtt: string = 'View More';
  LoadAttNotImg: string = 'View More';
  listAtt = [];
  public show: boolean = false;
  listShowMessageTime = [];
  isEmojiPickerVisible: boolean = false;
  listConversation = [];
  onLoading = false;
  searchResult: any = null;
  text: string = '';
  showModalSearch: boolean = false;
  searchId: string = '';
  i: number = 0;
  skipCountSearch: number = 0;
  maxResultCountSearch: number;
  currentRouter = '';
  testMess: any;
  oldCount: number = 10;
  newCount: number = 10;
  idUserSender: string = '';
  isTyping = false;
  messagesLoading = false;
  memberAdd: string[] = [];
  AvartarUrl = '';
  AvartarFriend: any;
  isdelteContent = false;
  setBottom = true;
  swapMessage = false;
  conversationNow: any;
  MaxResultCountMember = 20;
  skipCountMember = 0
  loadMore = 0;
  listProjectTag = [];
  projectChoose: string = '';
  tagName: any;
  memtionTemp;
  isCheckMention = false;
  // activeElement:any=null;
  // @Output() onClickConversation = new EventEmitter<listUserAll>();

  get token() {
    return localStorage.getItem('accessToken');
  }
  ////group 2 user
  nameConverGroup2User: string = '';
  membersConverGroup2User: [];
  groupChat2User: any;
  previousScrollHeightMinusTop: number;
  Users = [];
  constructor(
    private socketService: SocketServiceService,
    private createMessageService: CreateMessageService,
    public shareService: ShareServiceService,
    public conversationService: ConversationService,
    private signalRService: SignalRService,
    private msg: NzMessageService,
    private modal: NzModalService,
    public router: Router,
    public datepipe: DatePipe,
    public loaderService: LoaderService
  ) {}
  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }
  ngOnInit(): void {
    if (this.shareService.getUserData?.id) {
      this.getAllUser();
      this.getListMember();
      this.signalRService.SetConnection();
      this.connectionHub = this.signalRService.connection;
      // this.ReLoadMessage();
      this.addmemberGroup();
      this.removeMemberGroup();
      // this.seenMess()
      this.UpdateUnread();
      // this.getCurrentRout();
      this.typing();
      this.getInfoToGetAvartar();
      this.p_getChat();
      this.listenDelete();
      if (this.conversationService.gotoMess != '') {
        this.getConversation(this.conversationService.gotoMess);
      }
    }
  }
  callVideo() {
    this.createMessageService.createMessage('info', 'This feature is in development!');
  }
  typing() {
    this.connectionHub.on('Typing', (conversationId, content, userId) => {
      if (
        content != '' &&
        this.conversationId == conversationId &&
        userId != this.shareService.getUserData.id
      ) {
        this.isTyping = true;
        this.scrollToBottom();
      } else if (content == '') {
        this.isTyping = false;
      }
    });
  }
  upMess(messageId) {
    if (this.i < this.searchResult.totalCount - 1) {
      this.searchId = messageId;
      this.ScrollToMess(messageId);
      this.i++;
      const itemToScrollTo = document.getElementById(this.searchId);
      if (itemToScrollTo) {
        itemToScrollTo.scrollIntoView(true);
      }
    }
  }
  downMess(messageId) {
    if (this.i > 0) {
      this.i--;
      this.searchId = messageId;
      this.ScrollToMess(messageId);
      const itemToScrollTo = document.getElementById(this.searchId);
      if (itemToScrollTo) {
        itemToScrollTo.scrollIntoView(true);
      }
    }
  }
  //search tin nhắn
  searchMessenger(text) {
    if (text != '') {
      this.i = 0;
      this.searchId = '';
      this.text = text;
      this.createMessageService
        .SearchMessageForTotalCount(this.conversationId, text)
        .subscribe(res => {
          this.maxResultCountSearch = res.message.totalCount;
          this.createMessageService
            .SearchMessage(
              this.conversationId,
              text,
              this.skipCountSearch,
              this.maxResultCountSearch
            )
            .subscribe(data => {
              data.message.items.reverse();
              this.searchResult = data.message;
              this.searchId = this.searchResult.items[this.i].id;
            });
        });
    }
  }

  //danh sách attachment not img
  getAttNotImg(conversationId) {
    if (conversationId != '') {
      if (this.conversationId == conversationId) {
        this.createMessageService
          .listAttachmentNotImage(conversationId, this.AttResult)
          .subscribe((res: any) => {
            this.listAtt = res.items;
            // if(this.loadAtt == this.listAtt.length) this.loadAt = null;
            // this.loadAtt = this.listImage.length;
            if (this.listAtt.length == 0) {
              this.LoadAttNotImg = 'No items';
            }
            // if(this.listAtt.length < this.AttResult)
            //   {
            //     this.LoadAttNotImg = "No more"
            //   }
          });
      }
    }
  }
  getMoreAttNotImg() {
    this.isLoading = true;
    this.AttResult += 10;
    setTimeout(() => {
      this.isLoading = false;
      if (this.conversationId) {
        this.getAttNotImg(this.conversationId);
      }
    }, 1000);
  }

  //danh sách attachment img
  getListAttachment(conversationId) {
    if (conversationId != '') {
      if (this.conversationId == conversationId) {
        this.createMessageService
          .listAttachmentConversation(conversationId, this.ImgResult)
          .subscribe((res: any) => {
            this.listImage = res.items;
            if(this.loadMore == this.listImage.length) this.load = null;
            this.loadMore = this.listImage.length;
            console.log(this.listImage.length)
            if (this.listImage.length == 0) {
              this.LoadAtt = 'No items';
            }
          });
      }
    }
  }
  // //xem thêm att
  getMoreAtt() {
    this.isLoading = true;
    this.ImgResult += 10;
    setTimeout(() => {
      this.isLoading = false;
      if (this.conversationId) {
        this.getListAttachment(this.conversationId);
      }
    }, 1000);
  }

  GetCharAt(name) {
    return name?.charAt(0);
  }
  playAudio() {
    let audio = new Audio();
    audio.src = '../../assets/audio/Nhac-chuong-tin-nhan-Messenger.mp3';
    audio.load();
    audio.play();
  }
  //Modal addmember
  searchAddMember(input) {
    this.searchAdd = input;
    if (this.searchAdd != '') {
      this.conversationService.getListUserAll(input).subscribe(data => {
        this.listUserAddMember = data.items;
        this.AddMember.forEach(element => {
          this.listUserAddMember = this.listUserAddMember.filter(x => x.id !== element.userId);
        });
      });
    }
  }

  showModalAddmember(): void {
    this.isVisibleAddmember = true;
  }

  handleOkAddmember(): void {
    this.isVisibleAddmember = false;
  }

  handleCancelAddmember(): void {
    this.isVisibleAddmember = false;
  }
  //lấy thông tin conversation
  getInfo(conversationId) {
    this.createMessageService.getConversaion(conversationId).subscribe(data => {
      let info = data;
      this.typeConver = data['conversationType'];
      if (data['conversationType'] == 1) {
        this.nameUser = data['nameConversation'];
        this.groupName = data['nameConversation'];
        this.AddMember = data['listMembers'].filter(
          x => x.userId != this.shareService.getUserData.id
        );
        this.AvartarFriend = data['avatarUrl'];
        data['listMembers'].forEach(element => {
          if (element.userId != this.shareService.getUserData.id && element.name == null) {
            this.conversationService
              .getName(element.userId)
              .subscribe(data => (element.name = data.name));
          }
        });
        this.searchAdd = '';
      } else {
        info['listMembers'].forEach(element => {
          if (element.userId == this.shareService.getUserData.id) {
            element.unReadMessage = 0;
          }
          if (element.userId != this.shareService.getUserData.id) {
            this.AvartarFriend = element.avatarUrl;
          }
          if (element.userId != this.shareService.getUserData.id && element.name == null) {
            this.conversationService.getName(element.userId).subscribe(data => {
              this.nameUser = data.name;
            });
          } else if (element.userId != this.shareService.getUserData.id && element.name != null) {
            this.nameUser = element.name;
          }
        });
      }
    });
  }
  //modal
  showModalGroup2User(): void {
    this.isVisible = true;
  }

  // tạo chat group
  handleOkGroup2User(): void {
    let input = {
      nameConversation: this.nameConverGroup2User,
      menbers: this.membersConverGroup2User,
      conversationId: this.conversationId,
    };
    if (this.membersConverGroup2User.length <= 1) {
      this.createMessage('error');
    } else {
      this.isOkLoading = true;
      setTimeout(() => {
        this.createMessageService
          .CreateGroupChatFrom2User(input.conversationId, input.nameConversation, input.menbers)
          .subscribe((res: any) => {
            this.conversationId = res.conversationId;
            this.groupChat2User = res;
            this.getMessage(res.conversationId);
            this.getInfo(res.conversationId);
            this.getListMember();
            this.selected = res.conversationId;
          });
        this.isVisible = false;
        this.isOkLoading = false;
      }, 2000);
    }
  }
  handleCancelGroup2User(): void {
    this.isVisible = false;
  }
  //modal taoj chat group
  showModal(): void {
    this.isVisible = true;
  }

  // notification cho chat group
  createMessage(type: string): void {
    this.msg.create(type, `Group chat phải lớn hơn 2 thành viên`);
  }
  // tạo chat group
  handleOk(): void {
    let input = {
      nameConversation: this.nameConversation,
      menbers: this.members,
    };
    if (this.members.length <= 1) {
      this.createMessage('error');
    } else {
      this.createMessageService
        .CreateGroupChat(input.nameConversation, input.menbers)
        .subscribe((res: any) => {
          this.connectionHub.invoke(
            'addMemberGroup',
            this.conversationId,
            this.shareService.getUserData.id
          );
          this.groupChat = res;
          this.conversationId = res.conversationId;
          this.getMessage(res.conversationId);
          this.getInfo(res.conversationId);
          // this.getListMember();
          this.selected = res.conversationId;
        });
      this.isOkLoading = true;
      setTimeout(() => {
        this.isVisible = false;
        this.isOkLoading = false;
      }, 2000);
    }
  }

  handleCancel(): void {
    this.isVisible = false;
  }
  //////group
  showModalGroup(): void {
    this.isVisibleChangName = true;
  }
  handleOkGroup(): void {
    let input = {
      idConversation: this.conversationId,
      newName: this.groupName,
    };
    this.isOkLoading = true;
    setTimeout(() => {
      this.createMessageService.changeNamegroup(input.idConversation, input.newName).subscribe();
      this.connectionHub.invoke(
        'addMemberGroup',
        this.conversationId,
        this.shareService.getUserData.id
      );
      this.isVisibleChangName = false;
      this.isOkLoading = false;
      this.createMessageService.createMessage('success', 'Change name Success');
    }, 2000);
  }
  handleCancelGroup(): void {
    this.isVisibleChangName = false;
  }
  addmember(userId) {
    this.memberAdd.push(userId);
    this.modal.warning({
      nzTitle: 'This is an warning message',
      nzContent: 'You want to add this member ?',
      nzOnOk: () => {
        this.isOkLoading = true;
        setTimeout(() => {
          this.createMessageService.addMember(this.conversationId, this.memberAdd).subscribe(() => {
            this.connectionHub.invoke('addMemberGroup', this.conversationId, userId);
            this.getInfo(this.conversationId);
          });
          this.createMessageService.createMessage('success', 'Success');
          this.isOkLoading = false;
        }, 200);
      },
    });
  }
  removeMemberGroup() {
    this.connectionHub.on('removeMemberGroup', (conversationId, userId) => {
      if (this.shareService.getUserData.id == userId) {
        if (this.conversationId == conversationId) {
          this.message = [];
          this.nameUser = '';
        }
        this.getListMember();
      }
    });
  }
  //delete member
  deleteMember(userId): void {
    this.modal.warning({
      nzTitle: 'This is an warning message',
      nzContent: 'You want to delete this member ?',
      nzOnOk: () => {
        this.isOkLoading = true;
        setTimeout(() => {
          this.createMessageService.deleteMember(this.conversationId, userId).subscribe(() => {
            this.connectionHub.invoke('removeMemberGroup', this.conversationId, userId);
            this.getInfo(this.conversationId);
          });
          this.createMessageService.createMessage('success', 'Delete success');
          this.isOkLoading = false;
        }, 200);
      },
    });
  }
  UpdateUnread() {
    this.connectionHub.on('UpdateUnread', (conversationId, userId) => {
      if (this.shareService.getUserData.id == userId) {
        this.getMessage(conversationId);
        this.getListMember();
      }
    });
  }
  addmemberGroup() {
    this.connectionHub.on('addMemberGroup', (conversationId, userId) => {
      this.getListMember();
      if (this.conversationId == conversationId) {
        this.getInfo(conversationId);
      }
    });
  }
  // show tất cả user
  getAllUser() {
    this.conversationService.getAll().subscribe(data => {
      this.listAll = data;
    });
  }
  getNameUserInGroup(userId) {
    if (this.listAll != null) {
      if (this.shareService.getUserData?.id != null) {
        var result = this.listAll?.items.filter(obj => {
          return obj.id == userId;
        });
        return result[0]?.name;
      }
    }
  }
  isActive(item) {
    return this.selected === item;
  }
  //lấy Id conversation
  getConversation(conversationId) {
    if (this.conversationId != conversationId) {
      this.showModalSearch = false;
      this.text = '';
      this.searchResult = null;
      this.MaxResultCount = 20;
      this.ImgResult = 12;
      this.conversationId = conversationId;
      this.setBottom = true;
      this.swapMessage = true;
      console.log(this.conversationId);
      
      if (this.search == '') {
        this.getInfo(conversationId);
        this.getMessage(conversationId);
        this.getListAttachment(conversationId);
        this.getAttNotImg(conversationId);
        this.selected = conversationId;
      }

    }
  }
  typingInputchange(event) {
    this.connectionHub.invoke(
      'Typing',
      this.conversationId,
      event,
      this.shareService.getUserData.id
    );
  }
  clearEmoji() {
    this.isEmojiPickerVisible = false;
    this.listUserAll.forEach(item => {
      if (item.conversationId == this.conversationId) {
        if (item.unReadMessage > 0) {
          this.conversationService.countUnReadMessage -= 1;
        }
        item.unReadMessage = 0;
      }
    });
  }
  addEmoji(event) {
    this.inputConten = `${this.inputConten}${event.emoji.native}`;
  }
  ///create tin nhắn trong cuộc trò truyện
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
    var index = this.listAttachmentId.lastIndexOf(id);
    this.listAttachments.splice(index, 1);
    this.listAttachmentId.splice(index, 1);
    this.createMessageService.deleteAttachment(id).subscribe();
  }
  p_getChat(): void {
    // socket nhận tin nhắn mới
    this.socketService.listen('chat').subscribe(data => {
      var conversationNewest = this.listUserAll.find(
        x => x.conversationId == data.conversationId
      );
      this.listUserAll = this.listUserAll.filter(
        x => x.conversationId != data.conversationId
      );
      this.listUserAll.unshift(conversationNewest);
      conversationNewest.lastMessage.lastMessageTime = Date.now();
      conversationNewest.lastMessage.lastMessageContent = data.content;
      this.listUserAll = this.listUserAll.map(x => {
        if (x.conversationId === data.conversationId) {
          if (data.senderId !== this.shareService.getUserData?.id) {
            if (x.unReadMessage === 0) {
              this.conversationService.countUnReadMessage += 1;
            }
            x.unReadMessage++;
          }
          // x.listMembers.forEach(item => {
          //   if(item.userId !==  this.shareService.getUserData?.id )
          //   {
          //     if(item.unReadMessage === 0)
          //     {
          //       this.conversationService.countUnReadMessage +=1;
          //     }
          //     item.unReadMessage ++
          //   }
          // });
        }
        return x;
      });

      if (data.senderId != this.shareService.getUserData?.id) {
        this.playAudio();
      }
      if (this.conversationId != null && data.conversationId === this.conversationId) {
        if (this.socketService.socket.id !== data.socketId) {
          data.lastModificationTime = null;
          this.message?.items.push(data);
        }
      }
      this.messagePanel.nativeElement.scrollTop = !this.messagePanel.nativeElement.scrollTop
        ? this.messagePanel.nativeElement.scrollTop + 1
        : this.messagePanel.nativeElement.scrollTop;

      this.previousScrollHeightMinusTop =
        this.messagePanel.nativeElement.scrollHeight - this.messagePanel.nativeElement.scrollTop;
      setTimeout(() => {
        this.messagePanel.nativeElement.scrollTop =
          this.messagePanel.nativeElement.scrollHeight - this.previousScrollHeightMinusTop;
      }, 100);
    });
  }

  creatMessInConve() {
    this.createMessageService
      .CreateMessageAPI(this.content, this.type, this.conversationId, this.listAttachmentId)
      .subscribe(data => {
        if (this.listAttachmentId.length > 0) {
          this.getListAttachment(this.conversationId);
          this.getAttNotImg(this.conversationId);
        }
        this.connectionHub.invoke(
          'MiniChat',
          this.conversationId,
          this.shareService.getUserData.id
        );
        this.inputConten = '';
        this.typingInputchange(this.inputConten);
        this.listAttachmentId = [];
        this.listAttachments = [];
        this.type = 0;
        try {
          let t = 0;
          let h = this.messagePanel.nativeElement.offsetHeight;
          setTimeout(() => {
            while (
              this.messagePanel?.nativeElement.scrollTop + h <
                this.messagePanel?.nativeElement.scrollHeight &&
              t < 5000
            ) {
              this.messagePanel.nativeElement.scrollTop =
                this.messagePanel.nativeElement.scrollHeight;
              t++;
            }
          }, 200);
        } catch (err) {}
      });
  }
  //get tin nhắn trong cuộc trò truyện
  getMessage(conversationId) {
    if (conversationId != '' && this.conversationId == conversationId) {
      {
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
                  new Date(next.creationTime).getMonth() ===
                    new Date(pre.creationTime).getMonth() &&
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
            this.setBottom = false;
            data['items'].reverse();
          });
      }
    }
  }
  //scroll to message
  ScrollToMess(messageId) {
    this.createMessageService
      .scrollToMessage(messageId, this.oldCount, this.newCount)
      .subscribe(data => {
        this.message = data;
      });
  }
  //xem them tin nhắn
  showmore() {
    this.isLoading = true;
    setTimeout(() => {
      this.isLoading = false;
      this.MaxResultCount += 10;
      this.getMessage(this.conversationId);
      this.messagePanel.nativeElement.scrollTop = 125;
    }, 200);
  }
  //seachUser
  seachUser(input) {
    if (input == '') {
      this.getListMember();
    } else {
      this.conversationService.getListUserAll(input).subscribe(data => {
        if (data) {
          this.listUserAll = data['items'];
        }
      });
    }
  }
  onScrollMember(event) {
    if (event.target.scrollHeight - event.target.scrollTop == event.target.offsetHeight) {
      this.createMessageService.getListMember(this.listUserAll.length, 10).subscribe(data => {
        this.listUserAll = this.listUserAll.concat(data['items']);
      });
    }
  }
  //ListMember
  getListMember() {
    this.messagesLoading = true;
    this.createMessageService.getListMember(this.skipCountMember,this.MaxResultCountMember).subscribe(
      data => {
        this.listUserAll = data['items'];
        this.conversationService.countUnReadMessage = data['unreadConversation'];
        this.firstConver = data['items'][0]?.conversationId;
        this.listUserAll.map(x => {
          const memberMe = x.listMembers.find(y => y.userId == this.shareService.getUserData?.id);
          x.unReadMessage = memberMe.unReadMessage;
        });
        this.messagesLoading = false;
      },
      err => (this.messagesLoading = false)
    );
  }
  ///// tạo cuộc trò truyện
  CreateConversition(IdUserReceived) {
    this.received = IdUserReceived;
    this.createMessageService.CheckConversation(IdUserReceived).subscribe(data => {
      if (data['hasConversation']) {
        this.conversationId = data['conversationId'];
        this.getMessage(data['conversationId']);
        this.getInfo(data['conversationId']);
        this.getListMember();
        this.selected = data['conversationId'];
        this.search = '';
      } else {
        this.createMessageService.CreateConversition(IdUserReceived).subscribe(data => {
          this.conversationId = data.conversationId;
          this.Content('Hello');
          this.getInfo(data.conversationId);
          this.getListMember();
          // this.getMessage(data.conversationId);
          this.selected = data.conversationId;
          this.search = '';
        });
      }
    });
  }
  messDelete(type: string): void {
    this.msg.create(type, `Xóa cuộc trò truyện thành công`);
  }
  //xóa cuộc trò truyện
  warning(): void {
    this.modal.warning({
      nzTitle: 'This is an warning message',
      nzContent: 'Bạn có chắc muốn xóa cuộc trò chuyện này',
      nzOnOk: () => {
        this.createMessageService.delete(this.conversationId).subscribe();
        setTimeout(() => {
          this.getInfo(this.firstConver);
          this.getListMember();
          this.getMessage(this.firstConver);
          this.messDelete('success');
          this.selected = this.firstConver;
        }, 2000);
      },
    });
  }

  //tạo attachment
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
  ///scroll into view
  showtime(idMessage) {
    if (this.listShowMessageTime.includes(idMessage)) {
      var index = this.listShowMessageTime.lastIndexOf(idMessage);
      this.listShowMessageTime.splice(index, 1);
    } else {
      this.listShowMessageTime.push(idMessage);
    }
  }
  listenDelete() {
    this.socketService.listen('delete-message').subscribe(data => {
      this.listUserAll.map(element => {
        if (element.conversationId == data.conversationId) {
          element.lastMessage.lastMessageContent = 'Nội dung đã xóa';
        }
      });
      // this.isdelteContent = true;
      this.message?.items.map(res => {
        if (res.id == data.id) {
          res.content = 'Nội dung đã xoá';
        }
      });
    });
  }
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
          this.createMessageService
            .deleteMessage(idMessage, this.conversationId)
            .subscribe(data => {});
        }, 200),
      nzCancelText: 'No',
    });
  }
  toggleCollapsed() {
    this.show = !this.show;
  }
  showSearch() {
    if (this.showModalSearch) {
      this.showModalSearch = false;
      this.text = '';
      this.searchId = '';
      this.searchResult = null;
      this.i = 0;
      this.getMessage(this.conversationId);
    } else {
      this.showModalSearch = true;
    }
  }

  /////////////full scroll
  scrollToBottom(): void {
    if (this.setBottom) {
      try {
        let t = 0;
        let h = this.messagePanel.nativeElement.offsetHeight;
        setTimeout(() => {
          // this.messagePanel.nativeElement.scrollTop = this.messagePanel.nativeElement.scrollHeight;
          while (
            this.messagePanel.nativeElement.scrollTop + h <
              this.messagePanel.nativeElement.scrollHeight &&
            t < 5000
          ) {
            this.messagePanel.nativeElement.scrollTop =
              this.messagePanel.nativeElement.scrollHeight;
            t++;
          }
        }, 200);
      } catch (err) {}
    }
  }
  // @HostListener('scroll', ['$event'])
  onScroll(event): void {
    this.setBottom = false;
    if (this.searchId == '') {
      if (this.onLoading) return;
      if (this.messagePanel.nativeElement.scrollTop == 0) {
        // kiểm tra xem đã scroll đến đầu top hay chưa
        if (this.message.items?.length >= this.message.totalCount) {
          return;
        } else {
          this.showmore();
        }
        event.preventDefault();
      }
      event.preventDefault();
    } else {
      if (this.onLoading) return;
      if (this.messagePanel.nativeElement.scrollTop == 0) {
        // kiểm tra xem đã scroll đến đầu top hay chưa
        if (this.newCount + this.oldCount >= this.message.totalCount) {
          return;
        } else {
          this.oldCount += 10;
          this.ScrollToMess(this.searchId);
          this.messagePanel.nativeElement.scrollTop = 100;
        }
        event.preventDefault();
      }
      if (
        this.messagePanel.nativeElement.scrollHeight - this.messagePanel.nativeElement.scrollTop == this.messagePanel.nativeElement.offsetHeight
      ) {
        // kiểm tra xem đã scroll đến đầu top hay chưa
        if (this.newCount + this.oldCount >= this.message.totalCount) {
          return;
        } else {
          this.newCount += 10;
          this.ScrollToMess(this.searchId);
        }
        event.preventDefault();
      }
      event.preventDefault();
    }
  }
  /////////////////////////////////// avartar
  /////avartar
  createAvartar(file: File) {
    this.filesAvartar = file;
    for (const file of this.filesAvartar) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.createMessageService.UploadAvartar(file).subscribe(
          data => {
            if (data.headers) {
              this.getInfoToGetAvartar();
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
  //create avatar group
  createAvartarGroup(file: File, conversaitonId) {
    this.filesAvartar = file;
    for (const file of this.filesAvartar) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.createMessageService.UploadAvartarGroup(file, conversaitonId).subscribe(
          data => {
            if (data.body) {
              this.AvartarFriend = data.body.avatarUrl;
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
  //getInfo
  getInfoToGetAvartar() {
    this.createMessageService.getInfoToGetAvartar().subscribe(data => {
      this.AvartarUrl = data['avatarUrl'];
    });
  }
  swapListMember() {
    this.swapMessage = false;
  }
  ///tag
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
