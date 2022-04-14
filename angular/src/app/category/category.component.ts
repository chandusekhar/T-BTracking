import { LoaderService } from './../Service/Loader/loader.service';
import { ConfirmationService } from '@abp/ng.theme.shared';
import { Confirmation } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { NzButtonSize } from 'ng-zorro-antd/button';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { CategoryService } from '../Service/Category/category.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
import { MemberService } from '../Service/Member/member.service';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.scss'],
})
export class CategoryComponent implements OnInit {
  size: NzButtonSize = 'small';
  projectLogin;
  listCategories: any;
  isVisibleAddCategory = false;
  isVisible = false;
  selectedCategory = null;
  selectedCategoryEdit = null;
  isOkLoadingAddCategory = false;
  isOkLoadingEditCategory = false;
  confirmModal?: NzModalRef;
  event;
  idCategoryEdit;
  constructor(
    // private statusService: StatusService,
    private categoryService: CategoryService,
    private createMessage: CreateMessageService,
    private confirmation: ConfirmationService,
    private shareService: ShareServiceService,
    public loaderService: LoaderService,
    private memberService: MemberService,
    public router: Router,
    private rout: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.projectLogin = this.rout.snapshot.params.idProject;
    this.checkUserInProject(this.projectLogin);
    this.getListCategories();
  }

  getListCategories() {
    this.categoryService.getListCategory().subscribe(data => {
      this.listCategories = data;
    });
  }

  handleCancelAddCategory() {
    this.isVisibleAddCategory = false;
  }
  handleCancelEditCategory() {
    this.isVisible = false;
  }
  CreateCategory(): void {
    if (this.selectedCategory == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      return;
    } else {
      this.addCategory();
      this.isVisibleAddCategory = false;
      this.selectedCategory = null;
    }
  }
  EditCategory(): void {
    if (this.selectedCategoryEdit == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      return;
    } else {
      this.editCategory();

    }
  }
  showModalAddCategory(): void {
    this.isVisibleAddCategory = true;
    this.getListCategories();
  }
  showModalEditCategory(id, name): void {
    this.selectedCategoryEdit = name;
    this.idCategoryEdit = id;
    this.isVisible = true;
  }
  editCategory() {
    const dataEdit = {
      Name: this.selectedCategoryEdit,
    };
    this.categoryService.PutCategory(dataEdit, this.idCategoryEdit).subscribe(
      () => {
        this.getListCategories();
        this.isVisible = false;
        this.selectedCategoryEdit = null;
        this.createMessage.createMessage('success', 'Edit category success!');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }

  addCategory() {
    const dataAddStatus = {
      Name: this.selectedCategory,
    };
    this.categoryService.CreateCategory(dataAddStatus).subscribe(
      () => {
        this.getListCategories();
        this.createMessage.createMessage('success', 'Add category success!');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  RemoveCategory(categoryID) {
    this.confirmation.warn('::Bạn chắc chắn xóa?', '::Chắc chắn xóa').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.categoryService.deleteCategory(categoryID).subscribe(
          () => {
            this.createMessage.createMessage('success', 'Delete category success!');
            this.getListCategories();
          },
          err => this.createMessage.createMessage('error', err.error.message)
        );
      }
    });
  }
  checkUserInProject(projectId){
    if(projectId){
      this.memberService.checkUserInProject(projectId).subscribe(data=>{
        if(!data){
          this.shareService.deleteLocalData();
          this.router.navigate(['/sign-in']);
        }
      });
    }
  }
}
