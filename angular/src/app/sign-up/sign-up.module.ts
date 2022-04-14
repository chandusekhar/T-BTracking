import { NgModule, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShareServiceService } from '../Service/share-service.service';
import { Router } from '@angular/router';

@NgModule({
  declarations: [],
  imports: [CommonModule],
})
export class SignUpModule implements OnInit {
  constructor(private shareService: ShareServiceService, private router: Router) {}
  ngOnInit(): void {
    if (this.shareService.getUserData?.id) {
      this.router.navigate(['/dashboard']);
    }
  }
}
