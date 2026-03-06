import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  private titleSubject = new BehaviorSubject<string>('Title');
  public title$: Observable<string> = this.titleSubject.asObservable();

  constructor() { }

  setTitle(title: string): void {
    this.titleSubject.next(title);
  }

  getTitle(): string {
    return this.titleSubject.value;
  }
}
