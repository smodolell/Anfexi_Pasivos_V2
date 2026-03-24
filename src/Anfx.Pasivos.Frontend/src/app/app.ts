import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UtilsService } from './services/utils.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('Anfexi Pasivos');
  private readonly utils = inject(UtilsService);
  
  ngOnInit(): void {
    this.utils.hidePreloader();
  }
}
