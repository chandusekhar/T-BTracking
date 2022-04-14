import { Pipe, PipeTransform } from "@angular/core";

@Pipe({name: 'formatNumber'})
export class FomateNumberPipe implements PipeTransform {
  transform(value: number): any {
      return value.toFixed(0);
    }
}