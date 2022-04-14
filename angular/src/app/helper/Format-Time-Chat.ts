import { DatePipe, registerLocaleData } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

import vi_VN from '@angular/common/locales/vi';
registerLocaleData(vi_VN);
@Pipe({
  name: 'customDayPipe',
})
export class CustomDayPipe implements PipeTransform {
  transform(inputDate: string): string {
    const date = new Date(inputDate);
    const datePipe = new DatePipe('vi_VN');
    let fDay = '';
    const today = new Date();
    const diffTime: number = Math.abs(today.getTime() - date.getTime());
    const MILISECONDS_OF_WEEK = 24 * 60 * 60 * 7 * 1000;
    if (
      date.getDate() === today.getDate() &&
      date.getMonth() === today.getMonth() &&
      date.getFullYear() === today.getFullYear()
    ) {
      return 'Today';
    }
    if (
      today.getDate() - date.getDate() == 1 &&
      date.getMonth() === today.getMonth() &&
      date.getFullYear() === today.getFullYear()
    ) {
      return 'Yesterday';
    }

    if (diffTime < MILISECONDS_OF_WEEK) {
      switch (date.getDay()) {
        case 0:
          fDay = 'Sunday';
          break;
        case 1:
          fDay = 'Monday';
          break;
        case 2:
          fDay = 'Tuesday';
          break;
        case 3:
          fDay = 'Wednesday';
          break;
        case 4:
          fDay = 'Thursday';
          break;
        case 5:
          fDay = 'Friday';
          break;
        case 6:
          fDay = 'Saturday';
          break;
      }
      if (today === date) {
        return (fDay = 'Today');
      } else if (today.getDate() - date.getDate() === 1) {
        return (fDay = 'Yesterday');
      } else {
        return fDay;
      }
    } else {
      return datePipe.transform(date, 'dd/MM/YYYY');
    }
  }
}
