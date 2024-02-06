import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';


import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './Components/login/login.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { NgToastModule } from 'ng-angular-popup';
import { HttpClientModule } from '@angular/common/http';
import { MainPageComponent } from './Components/main-page/main-page.component';
import { AddMemberComponent } from './Components/main-page/add-member/add-member.component';
import { DatePipe } from '@angular/common';
import { EditMemberComponent } from './Components/main-page/edit-member/edit-member.component';
import { DeleteMemberComponent } from './Components/main-page/delete-member/delete-member.component';
import { DetailsMemberComponent } from './Components/main-page/details-member/details-member.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    MainPageComponent,
    AddMemberComponent,
    EditMemberComponent,
    DeleteMemberComponent,
    DetailsMemberComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgToastModule,
  ],
  providers: [DatePipe],
  bootstrap: [AppComponent]
})
export class AppModule { }
