import { Pipe, PipeTransform } from "@angular/core";

@Pipe({name: 'formatHistory'})
export class FomateHistoryPipe implements PipeTransform {
  transform(value: string): string {
      return value.replace('"','');
    }
}