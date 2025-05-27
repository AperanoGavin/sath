import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo'
})
export class TimeAgoPipe implements PipeTransform {

  transform(value: string): string {
    const valueAsDate = new Date(value)

    const now = new Date();
    const seconds = Math.floor((now.getTime() - valueAsDate.getTime()) / 1000);

    let interval = Math.floor(seconds / 31536000);
    if (interval >= 1) {
      return `il y a ${interval} an${interval === 1 ? '' : 's'}`;
    }

    interval = Math.floor(seconds / 2592000);
    if (interval >= 1) {
      return `il y a ${interval} mois`;
    }

    interval = Math.floor(seconds / 86400);
    if (interval >= 1) {
      return `il y a ${interval} jour${interval === 1 ? '' : 's'}`;
    }

    interval = Math.floor(seconds / 3600);
    if (interval >= 1) {
      return `il y a ${interval} heure${interval === 1 ? '' : 's'}`;
    }

    interval = Math.floor(seconds / 60);
    if (interval >= 1) {
      return `il y a ${interval} minute${interval === 1 ? '' : 's'}`;
    }

    return `il y a ${Math.floor(seconds)} seconde${seconds === 1 ? '' : 's'}`;
  }
}