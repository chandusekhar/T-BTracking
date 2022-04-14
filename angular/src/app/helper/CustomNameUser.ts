import { Pipe, PipeTransform } from "@angular/core";

@Pipe({ name: 'customName' })
export class CustomName implements PipeTransform {
    transform(value: string): string {
        return value.split(' ').map(char => char.charAt(0)).join('');
    }
}