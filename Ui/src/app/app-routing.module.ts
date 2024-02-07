import { NgModule } from '@angular/core';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { MainPageComponent } from './Components/main-page/main-page.component';
import { AuthService } from './Services/auth.service';

const routes: Routes = [
  {path:'',component: LoginComponent},
  {path:'main',component:MainPageComponent, canActivate:mapToCanActivate([AuthService])}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
