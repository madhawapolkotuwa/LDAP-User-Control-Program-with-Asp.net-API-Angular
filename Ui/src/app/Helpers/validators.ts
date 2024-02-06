import { AbstractControl, ValidatorFn } from "@angular/forms";


export function coustomEmailValidator(): ValidatorFn{
    return (control: AbstractControl) => {
      const regex = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g;
      if(regex.test(control.value)){
        return null;
      }
      return { emailError:true }
    }
  }