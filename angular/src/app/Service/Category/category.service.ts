
import { ShareServiceService } from './../share-service.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  constructor(
    private shareService:ShareServiceService
  ) {
  }
  
  public getListCategory(): Observable<any>{
  const url = `${this.shareService.REST_API_SERVER}/api/app/category`;
  return this.shareService.returnHttpClient(url);
  }

  public getList(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/category/only`;
    return this.shareService.returnHttpClient(url);
  }

  public CreateCategory(data): Observable<any>{
  const url = `${this.shareService.REST_API_SERVER}/api/app/category`;
  return this.shareService.postHttpClient(url,data);
  }

  public getListCategoryByProject(IdProject): Observable<any>{
    const url = `${this.shareService.REST_API_SERVER}/api/app/category/category-with-issue?IdProject=${IdProject}`;
    return this.shareService.returnHttpClient(url);
    }
  public deleteCategory(categoryId): Observable<any> {
      const url = `${this.shareService.REST_API_SERVER}/api/app/category/${categoryId}`;
      return this.shareService.deleteHttpClient(url);
    }
    public PutCategory(data,categoryID): Observable<any>{
      const url = `${this.shareService.REST_API_SERVER}/api/app/category?Id=${categoryID}`;
      return this.shareService.putHttpClient(url,data);
      }
}
