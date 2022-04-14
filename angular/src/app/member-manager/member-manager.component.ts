import { ShareServiceService } from './../Service/share-service.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../Service/Member/member.service';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { ProjectService } from '../Service/Project/project.service';

@Component({
  selector: 'app-member-manager',
  templateUrl: './member-manager.component.html',
  styleUrls: ['./member-manager.component.scss'],
})
export class MemberManagerComponent implements OnInit {
  idProject: any;
  checkAdmin = false;
  listUser;
  Filter: any = "";
  pageIndex = 1;
  pageSize = 10;
  totalCount = 1;
  search: string = '';
  userLogin;
  project;
  confirmModal?: NzModalRef;
  memberLoading = false;

  constructor(
    private rout: ActivatedRoute,
    private memberService: MemberService,
    private createMessage: CreateMessageService,
    private modal: NzModalService,
    public loaderService: LoaderService,
    public shareService: ShareServiceService,
    private projectService: ProjectService
  ) {}

  ngOnInit(): void {
    this.idProject = this.rout.snapshot.params.idProject;
    this.userLogin = this.shareService.getUserData.id;
    this.searchData();
    this.getProjectById(localStorage.getItem('ProjectId'));
  }
  RemoveMember(idMember): void {
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Bạn muốn loại thành viên này khỏi Project?',
      nzContent:
        'Khi xóa thành viên thì sẽ xóa tất cả issue mà người này tạo! và những thẻ được tag',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.memberService.DeleteMember(idMember).subscribe(
            () => {
              this.searchData();
              this.createMessage.createMessage('Thành công', 'Đã xóa thành viên!');
            },
            err => this.createMessage.createMessage('error', err.error.message)
          );
        }),
    });
  }

  getProjectById(input) {
    this.projectService.getProjectByID(input).subscribe(data => {
      this.project = data;
      if (data.idUserAdmin == this.userLogin) {
        this.checkAdmin = true;
      }
    });
  }
  filterChange(Filter) {
    if (Filter == null) {
      Filter = '';
    }
    this.Filter = Filter;
    this.searchData();
  }
  // searchData(): void {
  //   this.memberLoading = true;
  //   let skipCount = (this.pageIndex - 1) * this.pageSize;
  //   this.userService
  //     .getListUserbyIdProject(skipCount, this.pageSize, this.Filter, this.idProject)
  //     .subscribe(
  //       (data: any) => {
  //         this.totalCount = data.totalCount;
  //         this.listUser = data;
  //         this.memberLoading = false;
  //       },
  //       err => (this.memberLoading = false)
  //     );
  // }
  searchData(){
    this.memberLoading = true;
    var regex = /^[0-9]+$/;
    if (regex.test(this.Filter)) {
      var url = `${this.shareService.REST_API_SERVER}/odata/UserOData('${this.idProject}')?$filter=contains(PhoneName, '${this.Filter}')`;
    }
    else {
      var url = `${this.shareService.REST_API_SERVER}/odata/UserOData('${this.idProject}')?$filter=contains(Name, '${this.Filter}')`;
    }
    //var url = `${this.shareService.REST_API_SERVER}/odata/UserOData(${this.idProject})?$filter=contains(Name, '${this.Filter}')`;
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listUser = data?.value;
      this.memberLoading = false;
      console.log(this.listUser)
     // this.getListTeam();
     //  console.log(this.listUser)
    },err => (this.memberLoading = false))
  }
}
