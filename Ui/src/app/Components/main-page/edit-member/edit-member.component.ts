import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgToastService } from 'ng-angular-popup';
import { coustomEmailValidator } from 'src/app/Helpers/validators';
import { ModifiedMember } from 'src/app/Models/member';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-edit-member',
  templateUrl: './edit-member.component.html',
  styleUrls: ['./edit-member.component.css']
})
export class EditMemberComponent {

  isOpen:boolean = false;

  @Output()
  editMemberCloseBtnClicked: EventEmitter<boolean> = new EventEmitter<boolean>();


  public editMemberForm!: FormGroup

  modifiedMember:ModifiedMember = new ModifiedMember();
  
  constructor(
    private fb:FormBuilder,
    public auth:AuthService,
    private toast:NgToastService) {}

    ngOnInit(): void {
      this.editMemberForm = this.fb.group({
        password:['',[Validators.required]],
        surename:[`${this.auth.selectedMember.sn}`,[Validators.required]],
        commonname:[`${this.auth.selectedMember.cn}`,[Validators.required]],
        employeenumber:[`${this.auth.selectedMember.employeeNumber}`,[Validators.required]],
        employeetype:[`${this.auth.selectedMember.employeeType}`,[Validators.required]],
      }) 
  }

  onClose(){
    this.editMemberCloseBtnClicked.emit(true);
  }

  onSubmit(){
    this.modifiedMember.email= this.auth.selectedMember.uid;
    this.modifiedMember.password = this.editMemberForm.get('password')?.value;
    this.modifiedMember.commonname = this.auth.selectedMember.cn;
    this.modifiedMember.surename = this.auth.selectedMember.sn;
    this.modifiedMember.employeetype = this.auth.selectedMember.employeeType;
    this.modifiedMember.employeenumber = parseInt(this.auth.selectedMember.employeeNumber,10);

    this.modifiedMember.modifiedSurename = this.editMemberForm.get('surename')?.value;
    this.modifiedMember.modifiedCommonname = this.editMemberForm.get('commonname')?.value;
    this.modifiedMember.modifiedEmployeenumber = this.editMemberForm.get('employeenumber')?.value;
    this.modifiedMember.modifiedEmployeetype = this.editMemberForm.get('employeetype')?.value;

    if(this.editMemberForm.valid){
      this.auth.modifyMember(this.modifiedMember).subscribe({
        next:(res)=>{
          //console.log(res);
          this.auth.getMemberArray();
          this.toast.success({detail:"SUCCESS",summary:res.message, duration:5000});
          this.editMemberCloseBtnClicked.emit(true);
        },
        error:(err) =>{
          //console.log(err);
          this.toast.success({detail:"ERROR",summary:err.message, duration:5000});
        }
      })
    }
  }

  get email() {
    return this.editMemberForm.get('email')!;
  }
  get password(){
    return this.editMemberForm.get('password')!;
  }
  get surname(){
    return this.editMemberForm.get('surename')!;
  }
  get commonname(){
    return this.editMemberForm.get('commonname')!;
  }
  get employeenumber(){
    return this.editMemberForm.get('employeenumber')!;
  }

}
