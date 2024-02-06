import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { Observable } from 'rxjs';
import { Member, NewMember } from 'src/app/Models/member';
import { AuthService } from 'src/app/Services/auth.service';
import { DateTimeService } from 'src/app/Services/date-time.service';

@Component({
  selector: 'app-main-page',
  templateUrl: './main-page.component.html',
  styleUrls: ['./main-page.component.css']
})
export class MainPageComponent implements OnInit{

  isPopupAddMember:boolean = false;
  isPopupEditMember:boolean = false;
  isPopupDeleteMember:boolean = false;
  isPopupDetailsMember:boolean = false;

  arrayOfMembers:Member[] = [];


  constructor(
    public auth: AuthService,
    private router:Router,
    private toast:NgToastService,
    public timeDate:DateTimeService
    ){
      this.auth.member$.subscribe((mebers) => {
        this.arrayOfMembers = mebers;
      })
    }

  ngOnInit(): void{
    this.timeDate.startTimeDateTimer();
    this.auth.getMemberArray();
  }

  ngOnDestroy(): void {
    this.timeDate.stopTimeDateTimer();
  }

  onSignOut(): void{
    this.auth.logout().subscribe({
      next:(res)=>{
        this.toast.success({detail:"SUCCESS",summary:res.message, duration:5000});
        //console.log("logrdin");
        this.router.navigate(['']);
      },
      error:(err)=>{
        this.toast.error({detail:"ERROR", summary:err.error.message, duration:5000});
        //console.log("error");
      }
    })
  }

  onAddMember():void{
    this.isPopupAddMember = !this.isPopupAddMember;
  }

  editMember(member: Member){
    this.auth.saveSelectedMember(member);
    this.isPopupEditMember = !this.isPopupEditMember;
  }

  closePopup(){
    this.isPopupEditMember = false;
    this.isPopupDeleteMember = false;
    this.isPopupDetailsMember = false;
  }

  deleteMember(member: Member){
    this.auth.saveSelectedMember(member);
    this.isPopupDeleteMember = !this.isPopupDeleteMember;
  }

  detailsMember(member: Member){
    this.auth.saveSelectedMember(member);
    this.isPopupDetailsMember = !this.isPopupDetailsMember;
  }

}
