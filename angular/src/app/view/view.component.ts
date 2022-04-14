import {
  AfterContentInit,
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { formatDistance } from 'date-fns';
import { CommentService } from '../Service/Comment/comment.service';
import { IssueService } from '../Service/Issue/issue.service';
import { ShareServiceService } from '../Service/share-service.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { AssigneeService } from '../Service/Assignee/assignee.service';
import { FollowService } from '../Service/Follow/follow.service';
import { MemberService } from '../Service/Member/member.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { StatusService } from '../Service/Status/status.service';
import { UserService } from '../Service/User/user.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { NzUploadFile } from 'ng-zorro-antd/upload';
import { QuillEditorComponent } from 'ngx-quill';
import { ViewChild } from '@angular/core';
import 'quill-mention';
import { CategoryService } from '../Service/Category/category.service';
import { CalendarService } from '../Service/Calendar/calendar.service';
import { NzUploadChangeParam } from 'ng-zorro-antd/upload';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observable } from 'rxjs';
import { AttachmentService } from '../Service/Attachment/attachment.service';
import { HistoryService } from '../Service/History/history.service';

function getBase64(file: File): Promise<string | ArrayBuffer | null> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = error => reject(error);
  });
}
@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss'],
})
export class ViewComponent implements OnInit, OnDestroy, AfterContentInit, AfterViewInit {
  @ViewChild(QuillEditorComponent, { static: true })
  editor1: QuillEditorComponent;
  pageSizeHistory = 20;
  inputValue123: string = '@afc163';
  suggestions = ['afc163', 'benjycui', 'yiminghe', 'RaoHai', '中文', 'にほんご'];
  fileList: NzUploadFile[] = [
    {
      uid: '-1',
      name: 'xxx.png',
      status: 'done',
      url: 'http://www.baidu.com/xxx.png',
    },
  ];
  inputValueReply;
  isMention = false;
  userIdMention = [];
  atValues = [];
  config = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'], // toggled buttons
      ['blockquote', 'code-block'],
      [{ header: 1 }, { header: 2 }], // custom button values
      [{ list: 'ordered' }, { list: 'bullet' }],
      [{ indent: '-1' }, { indent: '+1' }], // outdent/indent
      [{ direction: 'rtl' }], // text direction
      [{ color: [] }, { background: [] }], // dropdown with defaults from theme
      [{ align: [] }],
      ['clean'],
    ],
    mention: {
      mentionListClass: 'ql-mention-list mat-elevation-z8',
      allowedChars: /^[A-Za-z\sÅÄÖåäö]*$/,
      showDenotationChar: false,
      spaceAfterInsert: false,
      onSelect: (item, insertItem) => {
        insertItem(item);
        if (this.userIdMention.length == 0) {
          this.userIdMention.push(item.id);
        } else {
          var temp = 0;
          this.userIdMention.forEach(i => {
            if (i === item.id) {
              temp++;
            }
          });
          if (temp == 0) {
            this.userIdMention.push(item.id);
          }
        }
      },
      source: (searchTerm, renderList) => {
        const values = this.atValues;

        if (searchTerm.length === 0) {
          renderList(values, searchTerm);
        } else {
          const matches = [];

          values.forEach(entry => {
            if (entry.value.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1) {
              matches.push(entry);
            }
          });
          renderList(matches, searchTerm);
        }
      },
    },
  };

  config2 = {
    toolbar: [
      ['bold', 'italic', 'underline'], // toggled buttons
      ['blockquote', 'code-block'],
      [{ header: 1 }, { header: 2 }], // custom button values
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['clean'],
    ],
    mention: {
      mentionListClass: 'ql-mention-list mat-elevation-z8',
      allowedChars: /^[A-Za-z\sÅÄÖåäö]*$/,
      showDenotationChar: false,
      spaceAfterInsert: false,
      onSelect: (item, insertItem) => {
        insertItem(item);
        if (this.userIdMention.length == 0) {
          this.userIdMention.push(item.id);
        } else {
          var temp = 0;
          this.userIdMention.forEach(i => {
            if (i === item.id) {
              temp++;
            }
          });
          if (temp == 0) {
            this.userIdMention.push(item.id);
          }
        }
      },
      source: (searchTerm, renderList) => {
        const values = this.atValues;

        if (searchTerm.length === 0) {
          renderList(values, searchTerm);
        } else {
          const matches = [];

          values.forEach(entry => {
            if (entry.value.toLowerCase().indexOf(searchTerm.toLowerCase()) !== -1) {
              matches.push(entry);
            }
          });
          renderList(matches, searchTerm);
        }
      },
    },
  };
  previewImage: string | undefined = '';
  previewVisible = false;
  fileImage: boolean = true;
  listFile: any = [];
  templistFile: any = [];
  listCategory: any;
  listImg = [];
  urlTemp = [];
  urlBinding;
  fileSubmit: any;
  selectedFile: File = null;
  lengthAssignee = false;
  listUser;
  listUserAssign = [];
  listUserAssignTemp = [];
  listAssign1 = [];
  private currentRout: boolean = true;
  idProject = localStorage.getItem('ProjectId');
  null = null;
  listOldUserAssign;
  selectedStatus: any;
  selectedCat: any;
  statusID: any;
  catID: any;
  listStatus: any;
  showListStatus = false;
  checkLength = false;
  chekcAssignee = null;
  data: any[] = [];
  checkFollow: any;
  checkAdmin = false;
  userLogin: any;
  followId: any;
  projectLogin: string;
  userId: string;
  Issue: any;
  listComments: any;
  listAssign: any;
  issueLogin: string;
  submitting = false;
  event = true;
  isVisibleEditAssignee = false;
  isVisible = false;
  isVisible2 = false;
  countComent: any;
  selectIdComment: any;
  checkUserId = false;
  pageIndex = 1;
  pageSize = 4;
  totalCount = 1;
  Filter: any = null;
  url;
  inputValue = '';
  inputValue1: '';
  tempValue: any;
  listHistory: any = [];
  public load =' load more'
  currentIssueParentId: any;
  checkHistory = false;
  check;
  time = formatDistance(new Date(), new Date());
  issueName;
  attachments;
  priorityEnumList;
  levelEnumList;
  selectedPriority;
  selectedLevel;
  tempPriority;
  tempLevel;
  tempNameIssues;
  quillw;
  ////history
  historyLoading = false;
  limitPageSize = 0;
  isShowMoreLoading = false;
  startDate: any = null;
  endDate: any = null;
  // edit issue
  dataIssue = {
    Id : '',
    Name: '',
    Description: '',
    Priority: '',
    CategoryID: '',
    DueDate: '',
    ProjectID: '',
    StartDate: '',
    Level: '',
    creationTime: Date,
    FinishDate: '',
    Creator: '',
    StatusName: '',
  };
  interval;
  isValueIssuesChange = false;
  isShowDueDate = false;
  isShowAttachmentsEdit = false;
  tempDescrip;
  selectedDescrip;
  selectedDueDate;
  tempDueDate;
  @ViewChild('quillInput', { static: false })
  quillInput?: ElementRef<HTMLElement>;
  constructor(
    private createMessage: CreateMessageService,
    private userService: UserService,
    public shareService: ShareServiceService,
    private commentService: CommentService,
    private issueService: IssueService,
    private rout: ActivatedRoute,
    private assignService: AssigneeService,
    public router: Router,
    private followService: FollowService,
    public loaderService: LoaderService,
    private memberService: MemberService,
    private signalRService: SignalRService,
    private statusService: StatusService,
    private categoryService: CategoryService,
    private msg: NzMessageService,
    public historyService: HistoryService,
    private attachmentService: AttachmentService,
    private calendarService: CalendarService
  ) {}
  ngAfterViewInit(): void {
    this.getListComments(this.issueLogin);
  }
  ngAfterContentInit(): void {
    this.getIssueDetail(this.issueLogin);
    this.listenChangeTfs();
  }
  ngOnDestroy(): void {
    this.currentRout = false;
    clearInterval(this.interval);
  }

  ngOnInit(): void {
    this.signalRService.SetConnection();
    this.userLogin = this.shareService.getUserData;
    if(this.router.url.includes('/project'))
    {
      this.projectLogin = this.rout.snapshot.params.idProject;
      this.issueLogin = this.rout.snapshot.params.id;
    } else {
      this.projectLogin = this.calendarService.ProjectIdCalendar;
      this.issueLogin = this.calendarService.IssueIdCalendar;
    }
    this.checkUserInProject(this.projectLogin);
    this.checkRoleAdmin();
    this.getIssue(this.issueLogin);
    this.getListCategory();
    this.getListStatus();
    this.checkFollowIssue(this.issueLogin);
    this.getListUserByIdProject(this.projectLogin);
    this.getListUserComments(this.issueLogin);
    this.ReloadDB();
    this.ReloadComments();
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    
  }

  listenChangeTfs() {
    this.signalRService.connection.on('ChangesFromTfs', data => {
      if (data.includes(this.issueLogin) && this.currentRout) {
        this.getIssue(this.issueLogin);
        this.GetListAssign();
        this.getListComments(this.issueLogin);
      }
    });
  }
  ReloadDB() {
    this.signalRService.connection.on('ReloadNotify', issuesId => {
      if (issuesId == this.issueLogin && this.currentRout) {
        this.getIssue(this.issueLogin);
        this.GetListAssign();
        this.isVisible2 = false;
      }
    });
  }
  ReloadComments() {
    this.signalRService.connection.on('ReloadComments', issuesId => {
      if (issuesId == this.issueLogin && this.currentRout) {
        this.getListComments(this.issueLogin);
        this.getListUserComments(this.issueLogin);
      }
    });
  }
  GetCharAt(name) {
    var rs = '';
    name.split(' ').forEach(char => (rs += char.charAt(0)));
    return rs;
  }
  filterChange(Filter) {
    if (Filter == '') {
      Filter = null;
    }
    this.Filter = Filter;
    this.getListComments(this.rout.snapshot.params.id);
  }
  getListComments(issueID) {
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    this.commentService
      .getListCommentResult(skipCount, this.pageSize, issueID, this.Filter)
      .subscribe(data => {
        if (data) {
          this.listComments = data;
          this.countComent = data.totalCount;
          this.totalCount = data.totalCount;
        } else this.countComent = 0;
      });
  }
  page(page) {
    this.pageIndex = page;
    this.getListComments(this.issueLogin);
  }
  GetListAssign() {
    this.assignService.GetListAssigneeByIssue(this.issueLogin).subscribe(data => {
      if (data.totalCount > 3) {
        this.lengthAssignee = true;
        this.listAssign1[0] = data.items[0];
        this.listAssign1[1] = data.items[1];
        this.listAssign1[2] = data.items[2];
      }
      if (data.totalCount <= 3) {
        this.lengthAssignee = false;
      }
      this.listAssign = data;
      this.listUserAssign = [];
      data.items.forEach(element => {
        this.listUserAssign.push(element.userID);
      });
      this.listUserAssignTemp = [];
      this.listUserAssignTemp = this.listUserAssign.reverse();
    });
  }
  getIssue(issueID) {
    this.issueService.getIssueById(issueID).subscribe(data => {
      this.Issue = data;
      this.selectedStatus = data.statusID;
      this.selectedCat = data.categoryID;
      this.statusID = data.statusID;
      this.catID = data.categoryID;
      this.dataIssue.Name = this.Issue.name;
      this.dataIssue.Id = this.Issue.id;
      this.selectedDueDate = data?.dueDate;
      this.tempDueDate = data?.dueDate;
      this.tempNameIssues = data.name;
      this.dataIssue.Description = this.Issue.description;
      this.selectedDescrip = data.description;
      this.tempDescrip = data.description;
      this.dataIssue.Priority = this.Issue.priorityValue;
      this.selectedPriority = data.priorityValue;
      this.tempPriority = data.priorityValue;
      this.selectedLevel = data.levelValue;
      this.tempLevel = data.levelValue;
      this.dataIssue.Level = this.Issue.levelValue;
      this.dataIssue.Creator = this.Issue.userName;
      this.dataIssue.CategoryID = this.Issue.categoryName;
      this.getHistory();
      if (data.attachmentListImage) {
        this.listFile = [...data.attachmentListImage, ...data.attachmentListVideo];
        this.templistFile = [...data.attachmentListImage, ...data.attachmentListVideo];
      } else {
        this.listFile = [];
        this.templistFile = [];
      }
      this.GetListAssign();
      if (this.Issue.creatorId == this.userLogin.id) {
        this.checkUserId = true;
      }
    });
    
    this.getListEnumPriority();
    this.getListEnumLevel();
    
  }
  updateCmt(id) {
    if (this.listImg.length != 0) {
      const data1 = {
        IssueID: this.issueLogin,
        UserID: this.userLogin.id,
        Content: this.inputValue1,
        Url: this.listImg,
      };
      if (
        this.tempValue !== data1.Content ||
        (this.tempValue == data1.Content && this.urlTemp !== this.listImg)
      ) {
        this.commentService.updateComment(id, data1).subscribe(
          () => {
            this.listImg = [];
            this.urlTemp = [];
            this.getListComments(this.issueLogin);
            this.signalRService.connection.invoke('ReloadComments', this.issueLogin);
            this.getIssueDetail(this.issueLogin);
          },
          err =>
            err.error.message
              ? this.createMessage.createMessage('error', err.error.message)
              : this.shareService.errorHandling(err)
        );
      }
      this.event = !this.event;
      this.listImg = [];
      this.urlTemp = [];
    } else {
      const data1 = {
        IssueID: this.issueLogin,
        UserID: this.userLogin.id,
        Content: this.inputValue1,
        Url: [],
      };
      if (
        this.tempValue !== data1.Content ||
        (this.tempValue == data1.Content && this.urlTemp.length > 0)
      ) {
        data1.Url[0] = '';
        this.commentService.updateComment(id, data1).subscribe(
          () => {
            this.getListComments(this.issueLogin);
            this.signalRService.connection.invoke('ReloadComments', this.issueLogin);
            this.getIssueDetail(this.issueLogin);
          },
          err =>
            err.error.message
              ? this.createMessage.createMessage('error', err.error.message)
              : this.shareService.errorHandling(err)
        );
      }
      this.event = !this.event;
      this.listImg = [];
      this.urlTemp = [];
    }
  }
  selectedID(id) {
    this.selectIdComment = id;
    if (this.event) {
      this.commentService.getCommentById(id).subscribe(data => {
        this.inputValue1 = data.content;
        this.tempValue = data.content;
        this.urlTemp = [];
        if (data?.attachmentList) {
          for (const files of data?.attachmentList) {
            this.listImg.push(files.url);
          }
          this.urlTemp = this.listImg;
        }
      });
    }
    if (!this.event) {
      this.listImg = [];
    }
    this.event = !this.event;
    document.getElementById('textEditor').scrollIntoView({ behavior: 'smooth' });
  }
  deleteComment(idComment) {
    this.commentService.deleteComment(idComment).subscribe(
      () => {
        this.getListComments(this.issueLogin);
        this.signalRService.connection.invoke('ReloadComments', this.issueLogin);
        this.event = true;
        this.listImg = [];
        this.getIssueDetail(this.issueLogin);
      },
      err =>
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err)
    );
    this.getIssueDetail(this.issueLogin);
  }
  deleteIssue(issue) {
    this.issueService.DeleteIssue(issue.id).subscribe(
      () => {
        if (!this.router.url.includes('/myFollow')) {
          this.router.navigate(['project/' + this.projectLogin + '/issues']);
        } else {
          this.signalRService.connection.invoke('calendar', this.shareService.getUserData.id);
        }
        this.getIssueDetail(this.issueLogin);
        this.signalRService.connection.invoke('ReloadBoardByIdProjectAndCallerDelete', issue);
      },
      err =>
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err)
    );
  }
  deleteChildTask(issue) {
    this.issueService.DeleteIssue(issue.id).subscribe(
      () => {
        this.getIssue(this.issueLogin);
        this.signalRService.connection.invoke('ReloadBoardByIdProjectAndCallerDelete', issue);
      },
      err =>
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err)
    );
  }
  //createComment
  handleSubmit(): void {
    if (this.listImg.length != 0) {
      this.submitting = true;
      this.userId = this.userLogin.id;
      const data1 = {
        IssueID: this.issueLogin,
        UserID: this.userId,
        Content: this.inputValue,
        Url: this.listImg,
        Mention: this.userIdMention,
      };
      if (data1.Url) {
        this.commentService.createComment(data1).subscribe(
          () => {
            this.listImg = [];
            this.userIdMention = [];
            this.getListComments(this.issueLogin);
            this.signalRService.connection.invoke('ReloadComments', this.issueLogin);
            this.signalRService.connection.invoke('ReloadNotify', this.issueLogin);
            this.getIssueDetail(this.issueLogin);
          },
          err =>
            err.error.message
              ? this.createMessage.createMessage('error', err.error.message)
              : this.shareService.errorHandling(err)
        );
      }
    } else {
      this.listImg[0] = '';
      this.submitting = true;
      this.userId = this.userLogin.id;
      const data = {
        IssueID: this.issueLogin,
        UserID: this.userId,
        Content: this.inputValue,
        Url: this.listImg,
        Mention: this.userIdMention,
      };
      this.commentService.createComment(data).subscribe(
        () => {
          this.getListComments(this.issueLogin);
          this.signalRService.connection.invoke('ReloadComments', this.issueLogin);
          this.signalRService.connection.invoke('ReloadNotify', this.issueLogin);
          this.getIssueDetail(this.issueLogin);
        },
        err =>
          err.error.message
            ? this.createMessage.createMessage('error', err.error.message)
            : this.shareService.errorHandling(err)
      );
      this.listImg = [];
      this.userIdMention = [];
    }
    setTimeout(() => {
      this.submitting = false;
      this.inputValue = '';
    }, 500);
  }
  checkFollowIssue(idIssue) {
    this.followService.CheckFollow(idIssue).subscribe(data => {
      if (data) {
        this.checkFollow = true;
      } else {
        this.checkFollow = false;
      }
    });
  }
  updateFollow() {
    this.checkFollow ? this.deleteFollow() : this.createFollow();
  }
  createFollow(): void {
    this.userId = this.userLogin.id;
    const dataFollow = {
      IssueID: this.issueLogin,
      UserID: this.userId,
    };
    this.followService.CreateFollow(dataFollow).subscribe(data => {
      if (data) {
        this.followId = data.id;
        this.checkFollow = !this.checkFollow;
      }
    });
  }
  deleteFollow(): void {
    this.followService.CheckFollowUser(this.issueLogin, this.userLogin.id).subscribe(data => {
      if (data) {
        this.followId = data.id;
        this.followService.DeleteFollow(this.followId).subscribe(() => {
          this.checkFollow = !this.checkFollow;
          this.signalRService.connection.invoke('calendar', this.shareService.getUserData.id);
        });
      }
    });
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

  //edit issue

  showModal2(): void {
    if (this.Issue.issueParentDto) {
      this.currentIssueParentId = this.Issue.issueParentDto.id;
    }
    this.isVisible2 = true;
  }

  handleOk2(): void {
    this.isVisible2 = false;
  }

  handleCancel2(): void {
    this.isVisible2 = false;
  }
  checkUserInProject(projectId) {
    if (projectId) {
      this.memberService.checkUserInProject(projectId).subscribe(data => {
        if (!data) {
          this.shareService.deleteLocalData();
          this.router.navigate(['/sign-in']);
        }
      });
    }
  }
  checkRoleAdmin(): void {
    this.issueService.CheckAdmin(this.shareService.getUserData.id).subscribe(data => {
      if (data) {
        this.checkAdmin = true;
      }
    });
  }

  showHistory(): void {
    this.checkHistory = !this.checkHistory;
  }

  //getIsssueDetail
  getIssueDetail(issueId) {
    this.calendarService.GetHistoryByIssue(issueId, this.pageSizeHistory).subscribe(data => {
     // this.listHistory = data;
    });
  }
  loadMore() {
    this.pageSizeHistory += 10;
    this.getIssueDetail(this.issueLogin);
  }
  showStatusList(): void {
    this.showListStatus = !this.showListStatus;
  }
  getListStatus() {
    this.statusService.getListStatus().subscribe(
      data => {
        this.listStatus = data;
      },
      error => {
        console.log(error);
      }
    );
  }
  updateStatus(input) {
    if (input !== this.statusID) {
      this.isValueIssuesChange = true;
    }
  }
  updateCategory(input) {
    if (input !== this.catID) {
      this.isValueIssuesChange = true;
    }
  }
  showModalEditAssignee(): void {
    if (this.Issue.isHaveParent) {
      this.getListAssigneeByIdParent();
    }
    this.isVisibleEditAssignee = true;
  }
  handleCancelEditAssignee(): void {
    if (this.listUserAssign !== this.listUserAssignTemp) {
      this.listUserAssign = [];
      this.listUserAssign = this.listUserAssignTemp;
    }
    this.isVisibleEditAssignee = false;
  }
  EditAssignee(): void {
    let set = new Set(this.listUserAssign.concat(this.listUserAssignTemp));
    this.issueLogin = this.rout.snapshot.params.id;
    this.issueService.updateAssigneeIssues(this.issueLogin, this.listUserAssign).subscribe(
      () => {
        this.createMessage.createMessage('success', 'Update Assignees Successfully !!!');
        this.isVisibleEditAssignee = false;
        this.signalRService.connection.invoke('ReloadNotify', this.issueLogin);
        let assignees = [...set];
        this.signalRService.connection.invoke('ReloadBugAssign', this.idProject, assignees);
      },
      err =>
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err)
    );
  }
  onChangeSelectedUserAssign(result): void {
    this.listUserAssign = result;
  }
  getListUserByIdProject(idProject) {
    if (this.shareService.getIdProject()) {
      this.userService.getListUserByIdProject(idProject).subscribe(data => {
        if (data) {
          this.listUser = data;
        }
      });
    }
  }
  getListAssigneeByIdParent() {
    this.assignService.GetUsersAssigneeIssueParent(this.Issue.idParent, null).subscribe(data => {
      this.listUser = data;
    });
  }
  //uploadfile
  //selectFileCmt
  onFileSelected(file: File) {
    this.fileSubmit = file;
    for (const file of this.fileSubmit) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.createMessage.createAttachment(file).subscribe(
          data => {
            if (data.body) {
              this.urlBinding = data.body.fileUrl;
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
  deleteImg() {
    this.listImg = [];
  }
  selectListImg(file: File) {
    this.listImg = [];
    this.fileSubmit = file;
    for (const file of this.fileSubmit) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.createMessage.createAttachment(file).subscribe(
          data => {
            if (data.body) {
              this.listImg.push(data.body.fileUrl);
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
  ///lay danh sach user cmt o issue
  getListUserComments(issueID) {
    this.commentService.getListUserCommentsByIssueId(issueID).subscribe(data => {
      this.atValues = data;
    });
  }
   //scroll
   onScroll() {
    this.pageSize += 10;
    this.getHistory();
  }
  getListCategory() {
    this.categoryService.getListCategory().subscribe(data => {
      if (data) {
        this.listCategory = data;
        this;
      }
    });
  }
  getListEnumPriority() {
    this.issueService.getListEnumPriority().subscribe(data => {
      if (data) {
        this.priorityEnumList = data;
      }
    });
  }
  updatePriority(input) {
    if (input !== this.tempPriority) {
      this.isValueIssuesChange = true;
    }
  }
  getListEnumLevel() {
    this.issueService.getListEnumLevel().subscribe(data => {
      if (data) {
        this.levelEnumList = data;
      }
    });
  }
  updateLevel(input) {
    if (input !== this.tempLevel) {
      this.isValueIssuesChange = true;
    }
  }
  //Tra loi comment
  onChangesIssuesName(input) {
    if (input !== this.tempNameIssues) {
      this.isValueIssuesChange = true;
    }
  }
  onChangesDescription(input) {
    if (input !== this.tempDescrip) {
      this.isValueIssuesChange = true;
    }
  }
  onChangesDueDate(input) {
    {
      if (input !== this.tempDueDate) {
        this.isValueIssuesChange = true;
      }
    }
  }
  showDueDateEdit() {
    this.isShowDueDate = !this.isShowDueDate;
  }
  onUndoUpdateIssue() {
    this.isValueIssuesChange = false;
    this.isShowDueDate = false;
    this.dataIssue.Name = this.tempNameIssues;
    this.selectedPriority = this.tempPriority;
    this.selectedLevel = this.tempLevel;
    this.selectedCat = this.catID;
    this.selectedStatus = this.statusID;
    this.selectedDescrip = this.tempDescrip;
    this.selectedDueDate = this.tempDueDate;
    this.listFile = this.templistFile;
  }
  showAttachmentsEdit() {
    this.isShowAttachmentsEdit = !this.isShowAttachmentsEdit;
  }
  handleChange({ file, fileList }: NzUploadChangeParam): void {
    const status = file.status;

    if (status !== 'uploading') {
    }
    if (status === 'done') {
      this.msg.success(`${file.name} file uploaded successfully.`);
    } else if (file.size > 104857600 && status === 'error') {
      this.msg.error(`${file.name} file upload failed. ${file.size} is too large !`);
    } else if (status === 'error') {
      this.msg.error(`${file.name} file upload failed.`);
    }
    fileList.forEach(file => {
      if (file.status == 'error') {
        fileList.splice(
          fileList.findIndex(i => i.name == file.name && i.status == 'error'),
          1
        );
      }
    });
    this.listFile = fileList;
    if (this.listFile !== this.templistFile) {
      this.isValueIssuesChange = true;
    }
  }
  handlePreview = async (file: NzUploadFile) => {
    if (!file.url && !file.preview) {
      file.preview = await getBase64(file.originFileObj!);
    }
    file.isImageUrl ? (this.fileImage = true) : (this.fileImage = false);
    this.previewImage = file.preview || this.shareService.REST_API_SERVER + file.url;
    this.previewVisible = true;
  };
  handleRemove = (file: File) =>
    new Observable<boolean>(() => {
      let index = this.listFile.findIndex(i => i.name == file.name && i.status == 'removed');
      this.listFile.splice(index, 1);
      this.attachmentService.RemoveAttachment(file.name).subscribe();
      if (this.listFile !== this.templistFile) {
        this.isValueIssuesChange = true;
      }
    });
     // getHistory
  getHistory() {
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    if (!this.historyLoading && this.limitPageSize == 0) {
      this.historyLoading = true;
    }
    this.isShowMoreLoading = true;
    var newDate;
    var newMonth;
    var newDate1;
    var newMonth1;
    var StartDate = '';
    var EndDate = '';
    if (this.startDate == null) {
      this.startDate = null;
    } else {
      var resultDate = this.startDate.getDate();
      if (resultDate < 10) {
        newDate = '0' + resultDate.toString();
      } else newDate = resultDate.toString();
      var resultMonth = this.startDate.getMonth() + 1;
      if (resultMonth < 10) {
        newMonth = '0' + resultMonth.toString();
      } else newMonth = resultMonth.toString();
      StartDate = this.startDate.getFullYear() + '-' + newMonth + '-' + newDate;
    }
    //
    if (this.endDate == null) {
      this.endDate = null;
    } else {
      var resultDate1 = this.endDate.getDate() + 1;
      if (resultDate1 < 10) {
        newDate1 = '0' + resultDate1.toString();
      } else newDate1 = resultDate1.toString();
      var resultMonth1 = this.endDate.getMonth() + 1;
      if (resultMonth1 < 10) {
        newMonth1 = '0' + resultMonth1.toString();
      } else newMonth1 = resultMonth1.toString();
      EndDate = this.endDate.getFullYear() + '-' + newMonth1 + '-' + newDate1;
    }
    
    this.historyService
      .GetHistory(
        this.projectLogin,
        StartDate,
        EndDate,
        null,
        null,
        skipCount,
        this.pageSize,
        this.dataIssue.Id
      )
      .subscribe(
        data => {
          this.limitPageSize = data.count;
          this.listHistory = Object.keys(data.result).map(key => [key, data.result[key]]);
          if (this.pageSize >= this.limitPageSize + 10) {
            this.load = null;
          }
          this.historyLoading = false;
          this.isShowMoreLoading= false;
        },
        err => {
          this.historyLoading = false;
          this.isShowMoreLoading = false;
        }
      );
  }
  updateIssue() {
    const dataIssue1 = {
      Name: this.dataIssue.Name,
      Description: this.selectedDescrip,
      Priority: this.selectedPriority,
      CategoryID: this.selectedCat,
      DueDate: this.selectedDueDate,
      IssueLevel: this.selectedLevel,
      ProjectID: this.projectLogin,
      Assignees: [],
      Attachments: this.listFile,
      NotifyMail: [],
    };
    this.issueService.updateIssueWithAttachment(this.issueLogin, dataIssue1).subscribe(
      () => {
        if (this.selectedStatus !== this.statusID) {
          this.issueService.updateIssueByStatus(this.selectedStatus, this.issueLogin).subscribe(
            () => {
              this.getIssue(this.issueLogin);
              this.showListStatus = false;
              this.signalRService.connection.invoke(
                'ReloadBugAssign',
                this.idProject,
                this.listUserAssign
              );
              this.signalRService.connection.invoke('calendar', this.shareService.getUserData.id);
              this.signalRService.connection.invoke('ReloadNotify', this.issueLogin);
            },
            err =>
              err.error.message
                ? this.createMessage.createMessage('error', err.error.message)
                : this.shareService.errorHandling(err)
          );
        }
        this.createMessage.createMessage('success', 'Update Successfully !!!');
      },
      err => {
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err);
      }
    );

    this.isValueIssuesChange= false;
    this.isShowDueDate = false;
    this.isShowAttachmentsEdit = false;
  }

  onSelectionChanged = event => {
    if (event.oldRange == null) {
      this.onFocus(event);
    }
    if (event.range == null) {
      this.onBlur(event);
    }
  };

  onFocus(event) {
    event.editor.theme.modules.toolbar.container.style.visibility = 'visible';
  }

  onBlur(event) {
    event.editor.theme.modules.toolbar.container.style.visibility = 'hidden';
  }
}
