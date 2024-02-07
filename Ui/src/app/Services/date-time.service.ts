import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DateTimeService {

  private timer:any;

  public currentDate : any;
  public currentTime : any;

  constructor(private datePipe: DatePipe) {
    this.currentDate  = this.datePipe.transform((new Date), 'dd.MM.yyyy');
    this.currentTime = this.datePipe.transform((new Date), 'H:mm:ss');
   }

   startTimeDateTimer(){
    this.stopTimeDateTimer(); // Stop existing timer if any
    this.timer = setInterval(() => {
      this.currentTime = this.datePipe.transform((new Date), 'H:mm:ss');
    },  1000);
  }

  stopTimeDateTimer(){
    clearInterval(this.timer);
  }
}
