import { Component, EventEmitter, Output } from '@angular/core';
import { NgToastService } from 'ng-angular-popup';
import { DeleteMember } from 'src/app/Models/member';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-delete-member',
  templateUrl: './delete-member.component.html',
  styleUrls: ['./delete-member.component.css']
})
export class DeleteMemberComponent {

  deleteMember:DeleteMember = new DeleteMember();

  @Output()
  deleteMemberCloseBtnClicked: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private auth: AuthService,
    private toast: NgToastService
    ){}

  onClose(){
    this.deleteMemberCloseBtnClicked.emit(true);
  }

  onDelete(){
    this.deleteMember.email = this.auth.selectedMember.uid;
    this.deleteMember.employeetype = this.auth.selectedMember.employeeType;
    this.auth.deleteMember(this.deleteMember).subscribe({
      next: res => {
        this.auth.getMemberArray();
        this.toast.success({detail:"SUCCESS",summary:res.message, duration:5000});
        this.deleteMemberCloseBtnClicked.emit(true);
      },
      error:err => {
        this.toast.success({detail:"ERROR",summary:err.message, duration:5000});
      }
    });
  }


}
