import { AssigneeService } from './../Service/Assignee/assignee.service';
import { UserService } from './../Service/User/user.service';
import { CategoryService } from './../Service/Category/category.service';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NzButtonSize } from 'ng-zorro-antd/button';
import { BehaviorSubject, Observable } from 'rxjs';
import { ShareServiceService } from '../Service/share-service.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { IssueService } from '../Service/Issue/issue.service';
import { AttachmentService } from '../Service/Attachment/attachment.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzUploadChangeParam, NzUploadFile } from 'ng-zorro-antd/upload';
import { MemberService } from '../Service/Member/member.service';
import { FollowService } from '../Service/Follow/follow.service';
import { ElsaService } from '../Service/elsa/elsa.service';


function getBase64(file: File): Promise<string | ArrayBuffer | null> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = error => reject(error);
  });
}
@Component({
  selector: 'app-add-issue',
  templateUrl: './add-issue.component.html',
  styleUrls: ['./add-issue.component.scss'],
})
export class AddIssueComponent implements OnInit {
  @Input() issueParentId = '';
  @Input() idProjectCalendar: string = '';
  @Output() calendarEvent = new EventEmitter<Boolean>();
  config = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'], // toggled buttons
      ['blockquote', 'code-block'],

      [{ header: 1 }, { header: 2 }], // custom button values
      [{ list: 'ordered' }, { list: 'bullet' }],
      [{ script: 'sub' }, { script: 'super' }], // superscript/subscript
      [{ indent: '-1' }, { indent: '+1' }], // outdent/indent
      [{ direction: 'rtl' }], // text direction

      [{ size: ['small', false, 'large', 'huge'] }], // custom dropdown
      [{ header: [1, 2, 3, 4, 5, 6, false] }],

      [{ color: [] }, { background: [] }], // dropdown with defaults from theme
      [{ font: [] }],
      [{ align: [] }],

      ['clean'],
    ],
  };
  config2 = {
    toolbar: [
      ['bold', 'italic', 'underline', 'strike'], // toggled buttons
      ['blockquote', 'code-block'],
      [{ header: 1 }, { header: 2 }], // custom button values
      [{ list: 'ordered' }, { list: 'bullet' }],
      ['clean'],
    ],
  };
  issuesId;
  size: NzButtonSize = 'large';
  smallSize: NzButtonSize = 'small';
  Name: string;
  idIssueLogin: any;
  StartDate = new Date();
  descriptionValue: string;
  DueDate = null;
  ReceivedDate = null;
  valueAddCategory: string = null;
  buttonNameAdd: string = 'Add';
  buttonNameEdit: string = 'Save';
  isVisible = false;
  isOkLoading = false;
  isCreated = false;
  fileName = 'Choose File';
  fileName1 = '';
  fileToUpload: File = null;
  listFile: any = [];
  searchChange$ = new BehaviorSubject('');
  selectedCategory?: any;
  selectedPriority?: string;
  selectedLevel?: string;
  selectedParent?: string;
  isLoading = false;
  listCategory: any;
  listUser;
  attID: any;
  listAssign;
  listUserNotify = [];
  priorityEnumList;
  levelEnumList;
  listUserAssign = [];
  listUserAssign1 = [];
  previewImage: string | undefined = '';
  previewVisible = false;
  data1 = [];
  fileImage: boolean = true;
  lengthOriginListFile;
  projectLogin;
  issueLogin: string;
  checkText = '';
  issuesNoParent;
  projectId = '';
  definitions;
  workflowsSelect;
  showModalAddIssue: boolean = false;

  constructor(
    public shareService: ShareServiceService,
    private rout: ActivatedRoute,
    private categoryService: CategoryService,
    private createMessage: CreateMessageService,
    private issueService: IssueService,
    private attachmentService: AttachmentService,
    public loaderService: LoaderService,
    private userService: UserService,
    private assignService: AssigneeService,
    private signalRService: SignalRService,
    private msg: NzMessageService,
    public router: Router,
    private memberService: MemberService,
    private followService: FollowService,
    private elsaService: ElsaService
  ) {}

  ngOnInit(): void {
    this.signalRService.SetConnection();
    this.projectLogin = this.rout.snapshot.params.idProject;
    this.issueLogin = this.rout.snapshot.params.id;
    this.getListCategory();
    this.getListEnumPriority();
    this.getListEnumLevel();
    this.getListUserByIdProject();
    this.UpdateViewIssue();
    this.getIssuesNoParent();
    this.getListDefinitions();
    this.ReloadDB();
  }
  getListDefinitions() {
    this.elsaService.getListDefinitions().subscribe(data => {
      this.definitions = data;
    });
  }
  getIssuesNoParent() {
    if (!this.router.url.includes('/project') || !this.shareService.getIdProject()) {
      this.projectId = this.idProjectCalendar;
    } else {
      this.projectId = this.shareService?.getIdProject();
    }
    this.issueService.GetIssuesNoParent(this.projectId).subscribe(data => {
      this.issuesNoParent = data;
    });
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
  }
  handlePreview = async (file: NzUploadFile) => {
    if (!file.url && !file.preview) {
      file.preview = await getBase64(file.originFileObj!);
    }
    file.isImageUrl ? (this.fileImage = true) : (this.fileImage = false);
    this.previewImage = file.preview || this.shareService.REST_API_SERVER + file.url;
    this.previewVisible = true;
  };
  checkNameExist(name) {
    if (!this.router.url.includes('/project') || !this.shareService.getIdProject()) {
      this.projectId = this.idProjectCalendar;
    } else {
      this.projectId = this.shareService.getIdProject();
    }
    this.issueService.getCheckName(this.projectId, name).subscribe(data => {
      this.checkText = data;
    });
  }
  getListUserByIdProject() {
    if (this.issueParentId != '' && this.issueParentId != undefined) {
      this.assignService.GetUsersAssigneeIssueParent(this.issueParentId, null).subscribe(data => {
        this.listUser = data;
      });
    } else {
      if (this.selectedParent) {
        this.assignService
          .GetUsersAssigneeIssueParent(this.selectedParent, null)
          .subscribe(data => {
            this.listUser = data;
            var listAssignee = this.listUser.items.map(user => user.id);
            this.listUserAssign = this.listUserAssign.filter(e => listAssignee.indexOf(e) != -1);
          });
      } else {
        if (this.shareService.getIdProject() || this.router.url.includes('/project') ) {
          this.userService
            .getListUserByIdProject(localStorage.getItem('ProjectId'))
            .subscribe(data => {
              if (data) {
                this.listUser = data;
              }
            });
        } else {
          this.userService.getListUserByIdProject(this.idProjectCalendar).subscribe(data => {
            if (data) {
              this.listUser = data;
            }
          });
        }
      }
    }
  }
  getListEnumPriority() {
    this.issueService.getListEnumPriority().subscribe(data => {
      if (data) {
        this.priorityEnumList = data;
      }
    });
  }
  getListEnumLevel() {
    this.issueService.getListEnumLevel().subscribe(data => {
      if (data) {
        this.levelEnumList = data;
      }
    });
  }
  getListCategory() {
    this.categoryService.getListCategory().subscribe(data => {
      if (data) {
        this.listCategory = data;
      }
    });
  }
  CreateCategory(input) {
    this.categoryService.CreateCategory(input).subscribe(
      data => {
        this.selectedCategory = data.id;
        this.createMessage.createMessage('success', 'Create Successfully !!!');
        this.getListCategory();
        this.isVisible = false;
      },
      err => {
        this.createMessage.createMessage('error', err.error.message);
        this.isVisible = true;
      }
    );
  }
  CreateIssue() {
    if (!this.Name || this.Name.trim() == '') {
      this.createMessage.createMessage('error', 'Input Name is not null!');
      return;
    }
    if (this.selectedParent == null) {
      this.selectedParent = undefined;
    }
    if (!this.router.url.includes('/project') || !this.shareService.getIdProject()) {
      this.projectId = this.idProjectCalendar;
    } else {
      this.projectId = this.shareService.getIdProject();
    }
    this.isOkLoading = true;
    const dataIssue = {
      Name: this.Name,
      Description: this.descriptionValue,
      Priority: this.selectedPriority,
      CategoryID: this.selectedCategory,
      DueDate: this.DueDate,
      ProjectID: this.projectId,
      StartDate: this.StartDate,
      ReceivedDate: this.ReceivedDate,
      IssueLevel: this.selectedLevel,
      Attachments: this.listFile,
      Assignees: this.listUserAssign,
      NotifyMail: this.listUserNotify,
      IdParent: this.issueParentId != '' ? this.issueParentId : this.selectedParent,
    };
    this.issueService.CreateIssueWithAttachment(dataIssue).subscribe(
      data => {
        if (data) {
          this.issuesId = data.id;
          this.listUserNotify = [];
          this.clearFileInput();
          this.isOkLoading = false;
          this.createMessage.createMessage('success', 'Create Successfully');
          //this.shareService.modalAddIssue = false;
          this.isCreated = true;
          this.getIssuesNoParent();
          if (this.listUserAssign.length > 0) {
            this.sendNotifyReloadBugAssign(this.listUserAssign);
            this.signalRService.connection.invoke('ReloadNotify', data.id);
          }
          if (!this.router.url.includes('/project') || !this.shareService.getIdProject()) {
            const dataFollow = {
              IssueID: data.id,
              UserID: this.shareService.getUserData?.id,
            };
            this.followService.CreateFollow(dataFollow).subscribe(() => {
              this.signalRService.connection.invoke('calendar', this.shareService.getUserData.id);
            });
          }
          if (this.workflowsSelect) {
            if (this.workflowsSelect.length > 0) {
              this.workflowsSelect.forEach(workflow => this.executeDefinition(workflow));
            }
          }
          // this.listUserAssign = this.listUserAssign.push(this.shareService.getUserData.id)
          const dataCaller = {
            projectId: this.projectId,
            issueId: data['id'],
            parentId: this.issueParentId != '' ? this.issueParentId : this.selectedParent,
            caller: [...this.listUserAssign, this.shareService.getUserData.id],
          };
          this.listUserAssign = [];
          this.selectedParent = null;
          this.signalRService.connection.invoke(
            'ReloadBoardByIdProjectAndCallerCreate',
            // this.shareService.getIdProject()
            dataCaller
          );

          //Add follow
          // var follow = {
          //   issueID: data.id,
          //   userID: this.shareService.getUserData.id
          // }
          // const url = `${this.shareService.REST_API_SERVER}/api/app/follow`;
          // this.shareService.postHttpClient(url, follow).subscribe();
        }
      },
      err => {
        this.isOkLoading = false;
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err);
      }
    );
  }
  // deleteAssign(id) {
  //   this.assignService.DeleteAssignee(id).subscribe();
  // }
  makeId(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
  }

  executeDefinition(id) {
    const data = {
      correlationId: this.makeId(32),
    };
    this.elsaService.executeDefinition(id, data).subscribe();
  }
  UpdateIssue() {
    this.isOkLoading = true;
    this.idIssueLogin = this.rout.snapshot.params.id;
    const dataIssue1 = {
      Name: this.Name,
      Description: this.descriptionValue,
      Priority: this.selectedPriority,
      CategoryID: this.selectedCategory,
      DueDate: this.DueDate,
      IssueLevel: this.selectedLevel,
      ProjectID: this.shareService.getIdProject(),
      NotifyMail: this.listUserNotify,
      Assignees: this.listUserAssign,
      Attachments: this.listFile,
    };
    this.issueService.updateIssueWithAttachment(this.rout.snapshot.params.id, dataIssue1).subscribe(
      () => {
        let set = new Set(this.listUserAssign.concat(this.listUserAssign1));
        let listUserAssignees = [...set];
        if (listUserAssignees.length > 0) {
          this.sendNotifyReloadBugAssign(listUserAssignees);
        }
        this.createMessage.createMessage('success', 'Update Successfully !!!');
        setTimeout(() => {
          this.router.navigate([
            'project/' + localStorage.getItem('ProjectId') + '/issues/view/' + this.idIssueLogin,
          ]);
        }, 1000);
        this.isOkLoading = false;
        this.signalRService.connection.invoke('ReloadNotify', this.issueLogin);
      },
      err => {
        this.isOkLoading = false;
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err);
      }
    );
  }
  sendNotifyReloadBugAssign(list) {
    this.signalRService.connection.invoke(
      'ReloadBugAssign',
      this.shareService.getIdProject(),
      list
    );
  }
  onSearch(value: string): void {
    this.isLoading = true;
    this.searchChange$.next(value);
  }
  onChangeSelectedUserAssign(result): void {
    this.listUserAssign = result;
  }
  onChange(result: Date): void {
    
  }
  onChangeUserNotify(result) {
    this.listUserNotify = result;
  }

  clearFileInput() {
    this.listFile = [];
  }
  showModalAddCategory(): void {
    this.isVisible = true;
  }
  AssignMyself() {
    this.listUserAssign = [];
    var listAssignee = this.listUser.items.map(user => user.id);
    if (listAssignee.includes(this.shareService.getUserData.id)) {
      this.listUserAssign.push(this.shareService.getUserData.id);
    }
  }
  CreateCategoryModal(): void {
    if (this.valueAddCategory == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      this.isVisible = true;
      return;
    }
    this.CreateCategory({ name: this.valueAddCategory });
  }

  handleCancel(): void {
    this.isVisible = false;
  }
  handleCancelAddIssue(): void {
    this.shareService.modalAddIssue = false;
  }
  NewBugModal(){
    this.showModalAddIssue = true;
  }
  NewCreate() {
    this.isCreated = false;
    this.shareService.modalAddIssue = false;
    this.SetNullValue();
  }
  showBug(){
    this.shareService.modalAddIssue = false;
  }
  SetNullValue() {
    this.Name = null;
    this.descriptionValue = null;
    this.selectedCategory = null;
  }
  handleRemove = (file: File) =>
    new Observable<boolean>(() => {
      let index = this.listFile.findIndex(i => i.name == file.name && i.status == 'removed');
      this.listFile.splice(index, 1);
      this.attachmentService.RemoveAttachment(file.name).subscribe();
    });
  UpdateViewIssue() {
    if (this.rout.snapshot.params.id) {
      this.idIssueLogin = this.rout.snapshot.params.id;
      this.issueService.getIssueById(this.rout.snapshot.params.id).subscribe(data => {
        if (data) {
          this.Name = data.name;
          this.descriptionValue = data.description;
          this.selectedPriority = data.priorityValue;
          this.selectedCategory = data.categoryID;
          this.DueDate = data.dueDate;
          this.StartDate = data.startDate;
          this.ReceivedDate = data.receivedDate;
          this.selectedLevel = data.levelValue;
          this.assignService.GetListAssigneeByIssue(this.idIssueLogin).subscribe(data => {
            this.listUserAssign = [];
            data.items.forEach(element => {
              this.listUserAssign.push(element.userID);
              this.data1.push(element.id);
            });
            this.listUserAssign1 = this.listUserAssign.reverse();
          });
          if (data.attachmentListImage) {
            this.listFile = [...data.attachmentListImage, ...data.attachmentListVideo];
          } else this.listFile = [];
          this.getListUserByIdProject();
          this.lengthOriginListFile = this.listFile.length | 0;
        }
      });
    }
  }
  ReloadDB() {
    this.signalRService.connection.on('ReloadNotify', issuesId => {
      if (issuesId == this.issueLogin) {
        this.UpdateViewIssue();
      }
    });
  }
  checkUserInProject(projectId) {
    this.memberService.checkUserInProject(projectId).subscribe(data => {
      if (!data) {
        this.shareService.deleteLocalData();
        this.router.navigate(['/sign-in']);
      }
    });
  }
}
