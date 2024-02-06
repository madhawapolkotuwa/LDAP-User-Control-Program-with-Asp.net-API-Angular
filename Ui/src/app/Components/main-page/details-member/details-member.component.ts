import { Component, EventEmitter, Output } from '@angular/core';
import { AuthService } from 'src/app/Services/auth.service';

@Component({
  selector: 'app-details-member',
  templateUrl: './details-member.component.html',
  styleUrls: ['./details-member.component.css']
})
export class DetailsMemberComponent {

  @Output()
  CloseBtnClicked: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    public auth:AuthService
  ){}

  onClose(){
    this.CloseBtnClicked.emit(true);
  }

}
