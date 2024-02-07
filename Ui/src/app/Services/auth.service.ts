import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { BehaviorSubject, Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { Member } from '../Models/member';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  membersResult!:any;
  public Role:string = "";
  public userEmail:string = ""; 
  public loggedemployee: Member = new Member();
  public members: Member[] = [];
  public isLoggedin: boolean = false;

  private memberSubject = new BehaviorSubject<Member[]>([]);
  public member$ = this.memberSubject.asObservable();

  public selectedMember:Member = new Member();

  constructor(
    private http:HttpClient,
    private toast:NgToastService,
    private router:Router,
  ) { }

  login(loginObj: any){
    this.userEmail = loginObj.email;
    return this.http.post<any>(`${environment.apiUrl}api/User/login`,loginObj,{withCredentials:true});
  }

  logout() {
    return this.http.get<any>(`${environment.apiUrl}api/User/logout`,{withCredentials:true});
  }

  // getMembers(){
  //   this.http.get(`${environment.apiUrl}api/User/`);
  // }

  getMembers(): Observable<any[]> {
    // Assuming you are making a GET request to fetch members
    return this.http.get<any[]>(`${environment.apiUrl}api/User/`);
  }

  getMemberArray(){
    this.getMembers().subscribe(
      {
        next:(res:any[]) => {
          const urmembers = res.map(item => {
            const member = new Member();
            member.uid = item.uid;
            member.cn = item.cn;
            member.sn = item.sn;
            member.employeeNumber = item.employeeNumber;
            member.employeeType = item.employeeType;
            member.createdTimestamp = item.createdTimestamp;
            return member;
          });
          this.members = urmembers.sort((a,b) => a.employeeNumber.localeCompare(b.employeeNumber));
          this.updateMembers(this.members);
        },
        error: err =>{
          console.log(err);
        }
      }
    )
  }

  updateMembers(newMembers: Member[]){
    this.memberSubject.next(newMembers);
  }

  addNewMember(newMember:any){
    return this.http.post<any>(`${environment.apiUrl}api/Admin/register`,newMember,{withCredentials:true});
  }

  modifyMember(editedMember:any){
    return this.http.post<any>(`${environment.apiUrl}api/Admin/modify`,editedMember,{withCredentials:true});
  }

  deleteMember(deleteMember:any){
    return this.http.post<any>(`${environment.apiUrl}api/Admin/delete`,deleteMember,{withCredentials:true});
  }

  storeLoggedEmployee(employee: any){
    this.loggedemployee.cn = employee.commonname;
    this.loggedemployee.sn = employee.surename;
    this.loggedemployee.employeeNumber = employee.employeenumber;
    this.loggedemployee.employeeType = employee.employeetype;
  }

  getRole(): string {
    return this.loggedemployee.employeeType;
  }

  saveSelectedMember(member: Member){
    this.selectedMember = member;
  }

  canActivate():boolean{
    if(this.isLoggedin){
      return true;
    }
    this.router.navigate(['']);
    return false;
  }
}
