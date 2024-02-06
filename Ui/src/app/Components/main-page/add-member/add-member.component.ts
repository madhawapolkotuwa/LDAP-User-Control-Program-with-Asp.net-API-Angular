import { Component, ElementRef, EventEmitter, OnInit, Output, QueryList, ViewChildren } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgToastService } from 'ng-angular-popup';
import { coustomEmailValidator } from 'src/app/Helpers/validators';
import { NewMember } from 'src/app/Models/member';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-add-member',
  templateUrl: './add-member.component.html',
  styleUrls: ['./add-member.component.css']
})
export class AddMemberComponent implements OnInit{

  isOpen:boolean = false;

  public newMemberForm!: FormGroup

  @Output()
  addMemberCloseBtnClicked: EventEmitter<boolean> = new EventEmitter<boolean>();


  constructor(
    private fb:FormBuilder,
    public auth:AuthService,
    private toast:NgToastService
  ){}

  ngOnInit(): void {
      this.newMemberForm = this.fb.group({
        email:['',[Validators.required, coustomEmailValidator()]],
        password:['',[Validators.required]],
        surename:['',[Validators.required]],
        commonname:['',[Validators.required]],
        employeenumber:['',[Validators.required]],
        employeetype:['',[Validators.required]],
      }) 
  }

  onClose(): void {
    this.isOpen = !this.isOpen;
    this.addMemberCloseBtnClicked.emit(this.isOpen);
  }

  onSubmit():void{
    if(this.newMemberForm.valid){
      const newMember:NewMember = this.newMemberForm.value;

      this.auth.addNewMember(newMember).subscribe({
        next:res =>{
          this.auth.getMemberArray();
          this.toast.success({detail:"SUCCESS",summary:res.message, duration:5000});
        },
        error:err =>{
          this.toast.success({detail:"ERROR",summary:err.message, duration:5000});
        }
      });
      this.addMemberCloseBtnClicked.emit(this.isOpen);
    }else{
      this.toast.success({detail:"ERROR",summary:"FORM ERROR", duration:5000});
    }
  }

  get email() {
    return this.newMemberForm.get('email')!;
  }
  get password(){
    return this.newMemberForm.get('password')!;
  }
  get surname(){
    return this.newMemberForm.get('surename')!;
  }
  get commonname(){
    return this.newMemberForm.get('commonname')!;
  }
  get employeenumber(){
    return this.newMemberForm.get('employeenumber')!;
  }
}
