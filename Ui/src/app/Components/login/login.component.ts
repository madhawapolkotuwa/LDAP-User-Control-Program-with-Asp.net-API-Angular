import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { coustomEmailValidator } from 'src/app/Helpers/validators';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{

  public loginForm!: FormGroup;

  public login = {email:'',password:''}

  constructor(
    private fb:FormBuilder,
    private auth:AuthService,
    private router:Router,
    private toast:NgToastService
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['',[Validators.required, coustomEmailValidator()]],
      password:['',Validators.required]
    });
  }

  get email() {
    return this.loginForm.get('email')!;
  }
  get password(){
    return this.loginForm.get('password')!;
  }

  onLogin(): void {
    if(this.loginForm.valid){
      //console.log("valid",this.loginForm.value)
      this.auth.login(this.loginForm.value).subscribe({
        next:(res)=>{
          
          this.auth.storeLoggedEmployee(res.loggedEmployee);
          //console.log(res.loggedEmployee);

          this.toast.success({detail:"SUCCESS", summary:res.message, duration:5000});
          //console.log("logrdin");
          this.router.navigate(['main']);
        },
        error:(err)=>{
          this.toast.error({detail:"ERROR", summary:err.error.message, duration:5000});
          //console.log("error");
        }
      })
    }
  }

}

// export function coustomEmailValidator(): ValidatorFn{
//   return (control: AbstractControl) => {
//     const regex = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g;
//     if(regex.test(control.value)){
//       return null;
//     }
//     return { emailError:true }
//   }
// }
